using System;
using HarmonyLib;
using ProofAcidFireCold.Constants;

namespace ProofAcidFireCold.Patches
{
    /// <summary>
    /// 冰冻免疫补丁
    /// 目标方法: Map.TryShatter
    /// 补丁类型: Prefix - 阻止冰冻元素对卡片造成破碎伤害
    /// </summary>
    [HarmonyPatch(typeof(Map), "TryShatter")]
    public static class ColdProofPatch
    {
        /// <summary>
        /// 前置补丁：阻止冰冻伤害并显示保护消息
        /// </summary>
        /// <returns>false阻止原方法执行，true允许执行</returns>
        [HarmonyPrefix]
        public static bool BlockColdDamage(Point pos, int ele)
        {
            try
            {
                // 如果不是冰冻元素，允许原方法执行
                if (ele != GameConstants.ElementIds.Cold)
                {
                    return true;
                }

                // 空检查
                if (pos == null)
                {
                    Plugin.ModLogger?.LogWarning("ColdProofPatch called with null Point");
                    return false;
                }

                // 提前检查同步状态，避免不必要的ListCards调用
                if (!pos.IsSync)
                {
                    return false;
                }

                var cards = pos.ListCards(false);
                if (cards == null || cards.Count == 0)
                {
                    return false;
                }

                // 显示保护消息
                foreach (var card in cards)
                {
                    if (card == null) continue;

                    try
                    {
                        var element = Element.Create(ele, 0);
                        if (element?.source == null) continue;

                        var messageKey = card.isChara
                            ? GameConstants.MessageKeys.BlanketInventory
                            : GameConstants.MessageKeys.BlanketGround;

                        var fullMessageKey = $"{messageKey}{element.source.alias}";
                        Msg.Say(fullMessageKey, "ProofAcidFireCold Mod", Msg.GetName(card));

                        Plugin.ModLogger?.LogDebug($"Blocked cold damage on {card.Name}");
                    }
                    catch (Exception ex)
                    {
                        Plugin.ModLogger?.LogError($"Error showing cold protection message for card: {ex}");
                    }
                }

                // 阻止原方法执行，防止破碎
                return false;
            }
            catch (Exception ex)
            {
                Plugin.ModLogger?.LogError($"Error in ColdProofPatch: {ex}");
                return true; // 出错时允许原始行为
            }
        }
    }
}
