using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace ProofAcidFireCold.Patches
{
    /// <summary>
    /// 毯子耗损禁用补丁
    /// 目标方法: Map.TryShatter
    /// 补丁类型: Transpiler - 修改IL代码，将毯子耗损值从-1改为0
    /// </summary>
    /// <remarks>
    /// 此补丁通过IL代码转换，将调用Card.ModCharge(int, bool)时的参数从-1改为0，
    /// 从而禁用毯子在保护角色免受寒冷时的耐久度消耗。
    /// IL模式：
    ///   ldloc.s V_8 (8)
    ///   brfalse 196 (0260) ldloc.s V_7 (7)
    ///   ldloc.s V_8 (8)
    ///   ldc.i4.m1      <- 修改此处：-1改为0
    ///   ldc.i4.0
    ///   callvirt instance void Card::ModCharge(int32, bool)
    /// </remarks>
    [HarmonyPatch(typeof(Map), "TryShatter")]
    public static class BlanketCostPatch
    {
        /// <summary>
        /// IL代码转换器：修改毯子耗损逻辑
        /// </summary>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            bool patched = false;
            int patchLocation = -1;

            try
            {
                // 从索引5开始搜索（前5条指令不可能包含目标模式）
                for (int i = 5; i < codes.Count; i++)
                {
                    // 查找ModCharge方法调用
                    if (codes[i].opcode == OpCodes.Callvirt &&
                        codes[i].operand?.ToString()?.Contains("ModCharge") == true)
                    {
                        // 验证前面的指令模式
                        if (ValidateInstructionPattern(codes, i))
                        {
                            // 将ldc.i4.m1（加载-1）改为ldc.i4.0（加载0）
                            codes[i - 2].opcode = OpCodes.Ldc_I4_0;
                            patched = true;
                            patchLocation = i;
                            break;
                        }
                    }
                }

                if (patched)
                {
                    Plugin.ModLogger?.LogInfo($"Blanket cost transpiler successfully applied at instruction {patchLocation}");
                }
                else
                {
                    Plugin.ModLogger?.LogWarning(
                        "Blanket cost transpiler pattern not found - feature may not work correctly. " +
                        "This may indicate a game update has changed the target method.");
                }
            }
            catch (Exception ex)
            {
                Plugin.ModLogger?.LogError($"Error in BlanketCostPatch transpiler: {ex}");
            }

            return codes;
        }

        /// <summary>
        /// 验证IL指令模式是否匹配预期
        /// </summary>
        /// <param name="codes">指令列表</param>
        /// <param name="index">当前ModCharge调用的索引</param>
        /// <returns>模式是否匹配</returns>
        private static bool ValidateInstructionPattern(List<CodeInstruction> codes, int index)
        {
            // 确保有足够的前序指令
            if (index < 5) return false;

            // 验证指令序列：
            // [i-5] ldloc.s
            // [i-4] brfalse
            // [i-3] ldloc.s
            // [i-2] ldc.i4.m1  <- 要修改的位置
            // [i-1] ldc.i4.0
            // [i]   callvirt ModCharge
            return codes[index - 5].opcode == OpCodes.Ldloc_S &&
                   codes[index - 4].opcode == OpCodes.Brfalse &&
                   codes[index - 3].opcode == OpCodes.Ldloc_S &&
                   codes[index - 2].opcode == OpCodes.Ldc_I4_M1 &&
                   codes[index - 1].opcode == OpCodes.Ldc_I4_0;
        }
    }
}
