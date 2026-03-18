using System;
using HarmonyLib;

namespace ProofAcidFireCold.Patches
{
    /// <summary>
    /// 酸性免疫补丁
    /// 目标方法: Card.isAcidproof (getter)
    /// 补丁类型: Postfix - 强制返回true使所有卡片免疫酸性伤害
    /// </summary>
    [HarmonyPatch(typeof(Card), "isAcidproof", MethodType.Getter)]
    public static class AcidProofPatch
    {
        /// <summary>
        /// 后置补丁：覆盖原始返回值，使所有物品免疫酸性伤害
        /// </summary>
        [HarmonyPostfix]
        public static void MakeAcidproof(ref bool __result)
        {
            try
            {
                __result = true;
            }
            catch (Exception ex)
            {
                Plugin.ModLogger?.LogError($"Error in AcidProofPatch: {ex}");
            }
        }
    }
}
