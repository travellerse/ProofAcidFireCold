using System;
using HarmonyLib;
using ProofAcidFireCold.Constants;

namespace ProofAcidFireCold.Patches
{
    /// <summary>
    /// 偷窃免疫补丁
    /// 目标方法: ActEffect.Proc
    /// 补丁类型: Prefix - 阻止偷窃效果对角色生效
    /// </summary>
    [HarmonyPatch(typeof(ActEffect), "Proc", new Type[] {
        typeof(EffectId),
        typeof(int),
        typeof(BlessedState),
        typeof(Card),
        typeof(Card),
        typeof(ActRef)
    })]
    public static class StealProofPatch
    {
        /// <summary>
        /// 前置补丁：阻止偷窃效果并显示免疫消息
        /// </summary>
        /// <returns>false阻止原方法执行，true允许执行</returns>
        [HarmonyPrefix]
        public static bool BlockStealEffect(EffectId id, Card tc, ActRef actRef)
        {
            try
            {
                // 如果不是偷窃效果，允许原方法执行
                if (id != EffectId.Steal)
                {
                    return true;
                }

                // 验证目标
                if (tc?.Chara == null)
                {
                    Plugin.ModLogger?.LogWarning("Steal proof triggered but target has no character");
                    return false;
                }

                // 确定消息类型：偷钱或偷物品
                var isMoneySteal = !string.IsNullOrEmpty(actRef.n1) && actRef.n1 == GameConstants.ActRefKeys.Money;
                var messageKey = isMoneySteal
                    ? GameConstants.MessageKeys.StealNegateMoney
                    : GameConstants.MessageKeys.StealNegate;

                // 显示免疫消息
                tc.Chara.Say(messageKey, tc.Chara);

                Plugin.ModLogger?.LogDebug($"Blocked steal attempt on {tc.Chara.Name}: {messageKey}");

                // 阻止原方法执行，防止偷窃
                return false;
            }
            catch (Exception ex)
            {
                Plugin.ModLogger?.LogError($"Error in StealProofPatch: {ex}");
                return true; // 出错时允许原始行为
            }
        }
    }
}
