using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ProofAcidFireCold.Configuration;
using ProofAcidFireCold.Logging;

namespace ProofAcidFireCold.Utils
{
    /// <summary>
    /// Harmony补丁管理器，负责动态应用和移除补丁
    /// </summary>
    public class PatchManager
    {
        private readonly Harmony _harmony;
        private readonly ILogSource _logger;
        private readonly Dictionary<PatchType, Type> _patchTypeMap;
        private readonly HashSet<PatchType> _appliedPatches;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="harmony">Harmony实例</param>
        /// <param name="logger">日志记录器</param>
        public PatchManager(Harmony harmony, ILogSource logger)
        {
            _harmony = harmony ?? throw new ArgumentNullException(nameof(harmony));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appliedPatches = new HashSet<PatchType>();

            // 初始化补丁类型映射
            _patchTypeMap = new Dictionary<PatchType, Type>
            {
                { PatchType.Acid, typeof(Patches.AcidProofPatch) },
                { PatchType.Fire, typeof(Patches.FireProofPatch) },
                { PatchType.Cold, typeof(Patches.ColdProofPatch) },
                { PatchType.Steal, typeof(Patches.StealProofPatch) },
                { PatchType.BlanketCost, typeof(Patches.BlanketCostPatch) }
            };
        }

        /// <summary>
        /// 应用补丁
        /// </summary>
        /// <param name="patchType">补丁类型</param>
        /// <param name="enable">是否启用</param>
        public void ApplyPatch(PatchType patchType, bool enable)
        {
            if (!_patchTypeMap.TryGetValue(patchType, out var patchClass))
            {
                _logger.LogWarning($"Unknown patch type: {patchType}");
                return;
            }

            try
            {
                if (enable)
                {
                    if (_appliedPatches.Contains(patchType))
                    {
                        _logger.LogDebug($"{patchType} patch already applied, skipping");
                        return;
                    }

                    _harmony.PatchAll(patchClass);
                    _appliedPatches.Add(patchType);
                    _logger.LogInfo($"{patchType} patch enabled");
                }
                else
                {
                    if (!_appliedPatches.Contains(patchType))
                    {
                        _logger.LogDebug($"{patchType} patch not applied, skipping unpatch");
                        return;
                    }

                    UnpatchAll(patchClass);
                    _appliedPatches.Remove(patchType);
                    _logger.LogInfo($"{patchType} patch disabled");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {(enable ? "apply" : "remove")} {patchType} patch: {ex}");
            }
        }

        /// <summary>
        /// 移除指定类型的所有补丁
        /// </summary>
        private void UnpatchAll(Type patchClass)
        {
            var patchedMethods = _harmony.GetPatchedMethods().ToArray();

            foreach (var method in patchedMethods)
            {
                var patches = Harmony.GetPatchInfo(method);
                if (patches == null) continue;

                // 移除Prefix补丁
                foreach (var patch in patches.Prefixes.Where(p => p.PatchMethod.DeclaringType == patchClass))
                {
                    _harmony.Unpatch(method, patch.PatchMethod);
                }

                // 移除Postfix补丁
                foreach (var patch in patches.Postfixes.Where(p => p.PatchMethod.DeclaringType == patchClass))
                {
                    _harmony.Unpatch(method, patch.PatchMethod);
                }

                // 移除Transpiler补丁
                foreach (var patch in patches.Transpilers.Where(p => p.PatchMethod.DeclaringType == patchClass))
                {
                    _harmony.Unpatch(method, patch.PatchMethod);
                }
            }
        }

        /// <summary>
        /// 检查补丁是否已应用
        /// </summary>
        public bool IsPatchApplied(PatchType patchType)
        {
            return _appliedPatches.Contains(patchType);
        }

        /// <summary>
        /// 获取已应用的补丁数量
        /// </summary>
        public int GetAppliedPatchCount()
        {
            return _appliedPatches.Count;
        }
    }
}
