using BepInEx;
using HarmonyLib;

namespace ProofAcidFire
{
    [BepInPlugin("com.travellerse.plugins.ProofAcidFire", "Proof Acid Fire", "0.1.0.0")]
    [BepInProcess("Elin.exe")]
    public class ProofAcidFire : BaseUnityPlugin
    {
        void Awake()
        {
            Harmony harmony = new Harmony("com.travellerse.plugins.ProofAcidFire");
            Logger.LogInfo("ProofAcidFire loaded");
            harmony.PatchAll();
            Logger.LogInfo("ProofAcidFire patched");
            Logger.LogInfo("ProofAcidFire ready");
        }
    }


    /* 目标代码
     public class Card : BaseCard, IReservable, ICardParent, IRenderSource, IGlobalValue, IInspect
    {
        public bool isAcidproof
        {
            get
            {
                return this._bits1[22];
            }
            set
            {
                this._bits1[22] = value;
            }
        }

        public bool isFireproof
        {
            get
            {
                return this._bits1[21];
            }
            set
            {
                this._bits1[21] = value;
            }
        }

    }
     */


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
}
