using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;

namespace ProofAcidFireCold
{
    [BepInPlugin("com.travellerse.plugins.ProofAcidFireCold", "Proof Acid Fire Cold", "0.3.0.0")]
    [BepInProcess("Elin.exe")]
    public class ProofAcidFireCold : BaseUnityPlugin
    {
        private ConfigEntry<bool> configProofAcid;
        private ConfigEntry<bool> configProofFire;
        private ConfigEntry<bool> configMeatOnMapProofFire;
        private ConfigEntry<bool> configProofCold;
        private ConfigEntry<bool> configProofSteal;

        public static new ManualLogSource Logger;
        void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo("ProofAcidFireCold loaded");
            Harmony harmony = new Harmony("com.travellerse.plugins.ProofAcidFireCold");
            configProofAcid = Config.Bind("ProofAcidFireCold", "ProofAcid", true, "Proof against acid damage");
            configProofFire = Config.Bind("ProofAcidFireCold", "ProofFire", true, "Proof against fire damage");
            configMeatOnMapProofFire = Config.Bind("ProofAcidFireCold", "MeatOnMapProofFire", false, "A true value means that you cannot roast meat with fire elements on the map");
            configProofCold = Config.Bind("ProofAcidFireCold", "ProofCold", true, "Proof against cold damage");
            configProofSteal = Config.Bind("ProofAcidFireCold", "ProofSteal", true, "Proof against steal effect");

            if (configProofAcid.Value)
            {
                harmony.PatchAll(typeof(ProofAcidPatch));
                Logger.LogInfo("ProofAcid enabled");
            }
            if (configProofFire.Value)
            {
                if (configMeatOnMapProofFire.Value)
                    harmony.PatchAll(typeof(ProofFirePatch));
                else harmony.PatchAll(typeof(ProofFireAndMeatPatch));
                Logger.LogInfo("ProofFire enabled");
            }
            if (configProofCold.Value)
            {
                harmony.PatchAll(typeof(ProofColdPatch));
                Logger.LogInfo("ProofCold enabled");
            }
            if (configProofSteal.Value)
            {
                harmony.PatchAll(typeof(ProofStealPatch));
                Logger.LogInfo("ProofSteal enabled");
            }
            Logger.LogInfo("ProofAcidFireCold finished");
        }
    }

    [HarmonyPatch(typeof(Card), "isAcidproof", MethodType.Getter)]
    public static class ProofAcidPatch
    {
        private static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }

    [HarmonyPatch(typeof(Card), "isFireproof", MethodType.Getter)]
    public static class ProofFireAndMeatPatch
    {
        private static void Postfix(Card __instance, ref bool __result)
        {
            if (__instance.category.name == "Meat" && __instance.ExistsOnMap) return;
            __result = true;
        }
    }

    [HarmonyPatch(typeof(Card), "isFireproof", MethodType.Getter)]
    public static class ProofFirePatch
    {
        private static void Postfix(Card __instance, ref bool __result)
        {
            __result = true;
        }
    }

    [HarmonyPatch(typeof(Map), "TryShatter")]
    public static class ProofColdPatch
    {
        private static bool Prefix(Point pos, int ele, int power)
        {
            Element element = Element.Create(ele, 0);
            List<Card> cards = pos.ListCards(false);
            if (ele == 911)
            {
                foreach (Card card in cards)
                {
                    if (pos.IsSync)
                    {
                        Msg.Say((card.isChara ? "blanketInv_" : "blanketGround_") + element.source.alias, "ProofAcidFireCold Mod", pos.FirstChara.Name, null, null);
                        ProofAcidFireCold.Logger.LogInfo((card.isChara ? "blanketInv_" : "blanketGround_") + element.source.alias);
                    }
                }
                return false;
            }
            return true;
        }
    }

    //System.Void ActEffect::Proc(EffectId,System.Int32,BlessedState,Card,Card,ActRef)
    [HarmonyPatch(typeof(ActEffect), "Proc", new System.Type[] { typeof(EffectId), typeof(int), typeof(BlessedState), typeof(Card), typeof(Card), typeof(ActRef) })]
    public static class ProofStealPatch
    {
        private static bool Prefix(EffectId id, int power, BlessedState state, Card cc, Card tc = null, ActRef actRef = default(ActRef))
        {
            if (id == EffectId.Steal)
            {
                if (tc.Chara != null)
                {
                    tc.Chara.Say((actRef.n1 == "money") ? "abStealNegateMoney" : "abStealNegate", tc.Chara, null, null);
                    ProofAcidFireCold.Logger.LogInfo((actRef.n1 == "money") ? "abStealNegateMoney" : "abStealNegate");
                    return false;
                }
                ProofAcidFireCold.Logger.LogInfo("Proof steal effect failed");
            }
            return true;
        }
    }
}
