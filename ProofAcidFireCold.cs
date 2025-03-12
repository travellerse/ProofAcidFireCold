using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace ProofAcidFireCold
{
    [BepInPlugin("com.travellerse.plugins.ProofAcidFireCold", "Proof Acid Fire Cold", "0.4.0.0")]
    [BepInProcess("Elin.exe")]
    public class ProofAcidFireCold : BaseUnityPlugin
    {
        // Configuration entries
        private ConfigEntry<bool> configProofAcid = null!;
        private ConfigEntry<bool> configProofFire = null!;
        private ConfigEntry<bool> configMeatOnMapProofFire = null!;
        private ConfigEntry<bool> configProofCold = null!;
        private ConfigEntry<bool> configProofSteal = null!;

        // Static configuration accessors
        public static bool IsMeatFireproofEnabled { get; private set; }
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
            configMeatOnMapProofFire = Config.Bind("ProofAcidFireCold", "MeatOnMapProofFire", false,
                "Prevent meat from being cooked by map fire elements when enabled");
            configProofCold = Config.Bind("ProofAcidFireCold", "ProofCold", true, "Immunity to cold damage");
            configProofSteal = Config.Bind("ProofAcidFireCold", "ProofSteal", true, "Immunity to steal effects");

            // Store meat proof configuration statically
            IsMeatFireproofEnabled = configMeatOnMapProofFire.Value;

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
                Logger.LogInfo("Fire proof enabled (Meat protection: " + IsMeatFireproofEnabled + ")");
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
            // Preserve original behavior for meat if configured
            if (!ProofAcidFireCold.IsMeatFireproofEnabled &&
                __instance.IsFood &&
                __instance.category.IsChildOf("foodstuff") &&
                __instance.ExistsOnMap) return;

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

    [HarmonyPatch(typeof(ActEffect), "Proc")]
    public static class StealProofPatch
    {
        private static readonly System.Type[] ParameterTypes = new System.Type[] {
            typeof(EffectId), typeof(int), typeof(BlessedState),
            typeof(Card), typeof(Card), typeof(ActRef)
        };

        [HarmonyPrefix]
        [HarmonyArgument(0, "id")]
        [HarmonyArgument(3, "tc")]
        [HarmonyArgument(5, "actRef")]
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