﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace ProofAcidFireCold
{
    [BepInPlugin("com.travellerse.plugins.ProofAcidFireCold", "Proof Acid Fire Cold", "0.4.2.0")]
    [BepInProcess("Elin.exe")]
    public class ProofAcidFireCold : BaseUnityPlugin
    {
        // Configuration entries
        private ConfigEntry<bool> configProofAcid = null!;
        private ConfigEntry<bool> configProofFire = null!;
        private ConfigEntry<bool> configMeatOnMapProofFire = null!;
        private ConfigEntry<bool> configGarbageProofFire = null!;
        private ConfigEntry<bool> configDisableBlanketsCost = null!;
        private ConfigEntry<bool> configProofCold = null!;
        private ConfigEntry<bool> configProofSteal = null!;

        // Static configuration accessors
        public static bool IsMeatFireproofEnabled { get; private set; }
        public static bool IsGarbageFireproofEnabled { get; private set; }
        public static bool IsBlanketCostDisabled { get; private set; }
        public static new ManualLogSource Logger { get; private set; } = null!;

        // Element constants
        public const int FIRE_ELEMENT_ID = 910;
        public const int COLD_ELEMENT_ID = 911;
        public const int ACID_ELEMENT_ID = 923;

        void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo("Initializing ProofAcidFireCold...");

            // Initialize configuration
            configProofAcid = Config.Bind("ProofAcidFireCold", "ProofAcid", true, "Immunity to acid damage");
            configProofFire = Config.Bind("ProofAcidFireCold", "ProofFire", true, "Immunity to fire damage");
            configProofCold = Config.Bind("ProofAcidFireCold", "ProofCold", true, "Immunity to cold damage");
            configProofSteal = Config.Bind("ProofAcidFireCold", "ProofSteal", true, "Immunity to steal effects");

            configMeatOnMapProofFire = Config.Bind("ProofAcidFireCold", "MeatOnMapProofFire", false,
                "Prevent meat from being cooked by map fire elements when enabled");
            configGarbageProofFire = Config.Bind("ProofAcidFireCold", "GarbageProofFire", false,
                "Prevent garbage from being destroyed by map fire elements when enabled");
            configDisableBlanketsCost = Config.Bind("ProofAcidFireCold", "DisableBlanketsCost", true,
                "Disable the cost of blankets");

            // Store meat proof configuration statically
            IsMeatFireproofEnabled = configMeatOnMapProofFire.Value;
            IsGarbageFireproofEnabled = configGarbageProofFire.Value;
            IsBlanketCostDisabled = configDisableBlanketsCost.Value;

            var harmony = new Harmony("com.travellerse.plugins.ProofAcidFireCold");

            // Apply patches based on configuration
            ApplyPatches(harmony);

            Logger.LogInfo("ProofAcidFireCold initialization complete");
        }

        private void ApplyPatches(Harmony harmony)
        {
            if (configProofAcid.Value)
            {
                harmony.PatchAll(typeof(AcidProofPatch));
                Logger.LogInfo("Acid proof enabled");
            }

            if (configProofFire.Value)
            {
                harmony.PatchAll(typeof(FireProofPatch));
                Logger.LogInfo("Fire proof enabled");
                Logger.LogInfo("MeatOnMapProofFire " + (IsMeatFireproofEnabled ? "enabled" : "disabled"));
                Logger.LogInfo("GarbageProofFire " + (IsGarbageFireproofEnabled ? "enabled" : "disabled"));
            }

            if (configProofCold.Value)
            {
                harmony.PatchAll(typeof(ColdProofPatch));
                Logger.LogInfo("Cold proof enabled");
            }

            if (configProofSteal.Value)
            {
                harmony.PatchAll(typeof(StealProofPatch));
                Logger.LogInfo("Steal proof enabled");
            }

            if (configDisableBlanketsCost.Value)
            {
                harmony.PatchAll(typeof(DisableBlanketsCostPatch));
                Logger.LogInfo("Blanket cost disabled");
            }
        }
    }

    [HarmonyPatch(typeof(Card), "isAcidproof", MethodType.Getter)]
    public static class AcidProofPatch
    {
        [HarmonyPostfix]
        public static void MakeAcidproof(ref bool __result) => __result = true;
    }

    [HarmonyPatch(typeof(Card), "isFireproof", MethodType.Getter)]
    public static class FireProofPatch
    {
        [HarmonyPostfix]
        public static void MakeFireproof(Card __instance, ref bool __result)
        {
            if (!ProofAcidFireCold.IsMeatFireproofEnabled &&
                __instance.IsFood &&
                __instance.category.IsChildOf("foodstuff") &&
                __instance.ExistsOnMap) return;

            // System.Boolean FactionBranch::TryTrash(Thing)
            if (!ProofAcidFireCold.IsGarbageFireproofEnabled &&
                (__instance.category.id == "garbage" || __instance.category.id == "junk")) return;

            __result = true;
        }
    }

    [HarmonyPatch(typeof(Map), "TryShatter")]
    public static class ColdProofPatch
    {
        [HarmonyPrefix]
        public static bool BlockColdDamage(Point pos, int ele)
        {
            if (ele != ProofAcidFireCold.COLD_ELEMENT_ID) return true;

            foreach (var card in pos.ListCards(false))
            {
                if (!pos.IsSync) continue;

                var messageKey = card.isChara ? "blanketInv_" : "blanketGround_";
                Msg.Say($"{messageKey}{Element.Create(ele, 0).source.alias}",
                    "ProofAcidFireCold Mod",
                    Msg.GetName(card));
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Map), "TryShatter")]
    public static class DisableBlanketsCostPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 5; i < codes.Count; i++)
            {
                if (codes[i].opcode == System.Reflection.Emit.OpCodes.Callvirt && codes[i].operand.ToString().Contains("ModCharge"))
                {
                    // 149	01D4	ldloc.s	V_8 (8)
                    // 150	01D6	brfalse	196 (0260) ldloc.s V_7 (7)
                    // 151	01DB	ldloc.s	V_8 (8)
                    // 152	01DD	ldc.i4.m1
                    // 153	01DE	ldc.i4.0
                    // 154	01DF	callvirt	instance void Card::ModCharge(int32, bool)
                    if (codes[i - 5].opcode == System.Reflection.Emit.OpCodes.Ldloc_S &&
                        codes[i - 4].opcode == System.Reflection.Emit.OpCodes.Brfalse &&
                        codes[i - 3].opcode == System.Reflection.Emit.OpCodes.Ldloc_S &&
                        codes[i - 2].opcode == System.Reflection.Emit.OpCodes.Ldc_I4_M1 &&
                        codes[i - 1].opcode == System.Reflection.Emit.OpCodes.Ldc_I4_0)
                    {
                        codes[i - 2].opcode = System.Reflection.Emit.OpCodes.Ldc_I4_0;
                        break;
                    }
                }
            }
            return codes;
        }
    }

    [HarmonyPatch(typeof(ActEffect), "Proc", new System.Type[] { typeof(EffectId), typeof(int), typeof(BlessedState), typeof(Card), typeof(Card), typeof(ActRef) })]
    public static class StealProofPatch
    {
        [HarmonyPrefix]
        public static bool BlockStealEffect(EffectId id, Card tc, ActRef actRef)
        {
            if (id != EffectId.Steal) return true;

            if (tc.Chara != null)
            {
                var messageKey = actRef.n1 == "money" ? "abStealNegateMoney" : "abStealNegate";
                tc.Chara.Say(messageKey, tc.Chara);
                ProofAcidFireCold.Logger.LogInfo("Blocked steal attempt: " + messageKey);
            }
            else
            {
                ProofAcidFireCold.Logger.LogInfo("Steal proof failed - no valid target");
            }
            return false;
        }
    }
}