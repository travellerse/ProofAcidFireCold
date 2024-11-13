using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ProofAcidFire
{
    [BepInPlugin("com.travellerse.plugins.ProofAcidFire", "Proof Acid Fire", "0.1.0.0")]
    [BepInProcess("Elin.exe")]
    public class ProofAcidFire : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;
        void Awake()
        {
            Logger = base.Logger;
            Harmony harmony = new Harmony("com.travellerse.plugins.ProofAcidFire");
            Logger.LogInfo("ProofAcidFire loaded");
            harmony.PatchAll();
            Logger.LogInfo("ProofAcidFire patched");
            Logger.LogInfo("ProofAcidFire ready");
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
    public static class ProofAcidFirePatch
    {
        private static bool Prefix(Point pos, int ele, int power)
        {
            Element element = Element.Create(ele, 0);
            if (ele == 911)
            {
                if (pos.IsSync)
                {
                    Msg.Say((pos.HasChara ? "blanketInv_" : "blanketGround_") + element.source.alias, "Mod", pos.FirstChara?.ToString(), null, null);
                    ProofAcidFire.Logger.LogInfo((pos.HasChara ? "blanketInv_" : "blanketGround_") + element.source.alias);
                }
                return false;
            }
            return true;
        }
    }
}
