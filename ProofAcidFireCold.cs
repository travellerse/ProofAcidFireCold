using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ProofAcidFireCold
{
    [BepInPlugin("com.travellerse.plugins.ProofAcidFireCold", "Proof Acid Fire Cold", "0.1.1.0")]
    [BepInProcess("Elin.exe")]
    public class ProofAcidFireCold : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;
        void Awake()
        {
            Logger = base.Logger;
            Harmony harmony = new Harmony("com.travellerse.plugins.ProofAcidFireCold");
            Logger.LogInfo("ProofAcidFireCold loaded");
            harmony.PatchAll();
            Logger.LogInfo("ProofAcidFireCold patched");
            Logger.LogInfo("ProofAcidFireCold ready");
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
    public static class ProofFirePatch
    {
        private static void Postfix(ref bool __result)
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
            if (ele == 911)
            {
                if (pos.IsSync)
                {
                    Msg.Say((pos.HasChara ? "blanketInv_" : "blanketGround_") + element.source.alias, "Mod", pos.FirstChara.Name, null, null);
                    ProofAcidFireCold.Logger.LogInfo((pos.HasChara ? "blanketInv_" : "blanketGround_") + element.source.alias);
                }
                return false;
            }
            return true;
        }
    }
}
