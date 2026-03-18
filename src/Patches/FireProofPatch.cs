using System;
using HarmonyLib;
using ProofAcidFireCold.Constants;

namespace ProofAcidFireCold.Patches
{
    /// <summary>
    /// 火焰免疫补丁
    /// 目标方法: Card.isFireproof (getter)
    /// 补丁类型: Postfix - 使所有卡片免疫火焰伤害（可配置肉类和垃圾例外）
    /// </summary>
    [HarmonyPatch(typeof(Card), "isFireproof", MethodType.Getter)]
    public static class FireProofPatch
    {
        /// <summary>
        /// 后置补丁：根据配置决定是否使物品防火
        /// </summary>
        [HarmonyPostfix]
        public static void MakeFireproof(Card __instance, ref bool __result)
        {
            try
            {
                if (__instance == null)
                {
                    Plugin.ModLogger?.LogWarning("FireProofPatch called with null Card instance");
                    return;
                }

                if (ShouldExcludeFromFireproof(__instance))
                {
                    Plugin.ModLogger?.LogDebug($"Excluding {__instance.Name} from fireproof");
                    return;
                }

                __result = true;
            }
            catch (Exception ex)
            {
                Plugin.ModLogger?.LogError($"Error in FireProofPatch: {ex}");
            }
        }

        /// <summary>
        /// 检查卡片是否应该排除在防火保护之外
        /// </summary>
        private static bool ShouldExcludeFromFireproof(Card card)
        {
            if (card.category == null)
            {
                return false;
            }

            // 肉类例外：如果禁用肉类防火且是地图上的食材，则排除
            if (!Plugin.ModConfig.MeatOnMapProofFire.Value &&
                card.IsFood &&
                card.category.IsChildOf(GameConstants.CategoryIds.Foodstuff) &&
                card.ExistsOnMap)
            {
                return true;
            }

            // 垃圾例外：如果禁用垃圾防火且是垃圾类物品，则排除
            if (!Plugin.ModConfig.GarbageProofFire.Value &&
                IsGarbageCategory(card.category.id))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查是否为垃圾类别
        /// </summary>
        private static bool IsGarbageCategory(string categoryId)
        {
            return categoryId == GameConstants.CategoryIds.Garbage ||
                   categoryId == GameConstants.CategoryIds.Junk;
        }
    }
}
