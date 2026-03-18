using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ProofAcidFireCold.Configuration;
using ProofAcidFireCold.Utils;
using ProofAcidFireCold.Logging;

namespace ProofAcidFireCold
{
    /// <summary>
    /// ProofAcidFireCold主插件类
    /// 提供对酸性、火焰、冰冻伤害的免疫，以及防盗和毯子耗损禁用功能
    /// </summary>
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Elin.exe")]
    public class Plugin : BaseUnityPlugin
    {
        // 静态访问器供补丁类使用
        public static ManualLogSource ModLogger { get; private set; }
        public static ModConfig ModConfig { get; private set; }

        private PatchManager _patchManager;
        private Harmony _harmony;

        /// <summary>
        /// 插件初始化入口点
        /// </summary>
        void Awake()
        {
            try
            {
                // 初始化日志
                ModLogger = Logger;
                ModLogger.LogInfo("===========================================");
                ModLogger.LogInfo($"Initializing {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION}...");
                ModLogger.LogInfo("===========================================");

                // 初始化Harmony
                _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
                ModLogger.LogInfo("Harmony instance created");

                // 初始化配置管理器
                var logAdapter = new LogSourceAdapter(ModLogger);
                var configAdapter = new ConfigFileAdapter(base.Config);
                ModConfig = new ModConfig(configAdapter, logAdapter);
                ModConfig.Initialize(OnPatchToggle);
                ModLogger.LogInfo("Configuration initialized");

                // 初始化补丁管理器
                _patchManager = new PatchManager(_harmony, logAdapter);
                ModLogger.LogInfo("Patch manager initialized");

                // 应用初始补丁
                ApplyInitialPatches();

                ModLogger.LogInfo("===========================================");
                ModLogger.LogInfo($"{PluginInfo.PLUGIN_NAME} initialization complete!");
                ModLogger.LogInfo($"Applied {_patchManager.GetAppliedPatchCount()} patches");
                ModLogger.LogInfo("===========================================");
            }
            catch (Exception ex)
            {
                ModLogger?.LogError("===========================================");
                ModLogger?.LogError("FATAL ERROR during plugin initialization:");
                ModLogger?.LogError(ex.ToString());
                ModLogger?.LogError("===========================================");
                ModLogger?.LogError("Plugin may not function correctly. Please report this error.");
                throw;
            }
        }

        /// <summary>
        /// 应用初始补丁（基于配置）
        /// </summary>
        private void ApplyInitialPatches()
        {
            ModLogger.LogInfo("Applying initial patches based on configuration...");

            if (ModConfig.ProofAcid.Value)
            {
                _patchManager.ApplyPatch(PatchType.Acid, true);
            }

            if (ModConfig.ProofFire.Value)
            {
                _patchManager.ApplyPatch(PatchType.Fire, true);
                LogFireProofDetails();
            }

            if (ModConfig.ProofCold.Value)
            {
                _patchManager.ApplyPatch(PatchType.Cold, true);
            }

            if (ModConfig.ProofSteal.Value)
            {
                _patchManager.ApplyPatch(PatchType.Steal, true);
            }

            if (ModConfig.DisableBlanketsCost.Value)
            {
                _patchManager.ApplyPatch(PatchType.BlanketCost, true);
            }
        }

        /// <summary>
        /// 记录火焰免疫的详细配置
        /// </summary>
        private void LogFireProofDetails()
        {
            var meatStatus = ModConfig.MeatOnMapProofFire.Value ? "enabled" : "disabled";
            var garbageStatus = ModConfig.GarbageProofFire.Value ? "enabled" : "disabled";

            ModLogger.LogInfo($"  MeatOnMapProofFire: {meatStatus}");
            ModLogger.LogInfo($"  GarbageProofFire: {garbageStatus}");
        }

        /// <summary>
        /// 补丁切换回调（用于运行时配置热重载）
        /// </summary>
        /// <param name="patchType">补丁类型</param>
        /// <param name="enable">是否启用</param>
        private void OnPatchToggle(PatchType patchType, bool enable)
        {
            ModLogger.LogInfo($"Runtime patch toggle requested: {patchType} = {enable}");

            try
            {
                _patchManager.ApplyPatch(patchType, enable);

                // 如果是火焰补丁，记录详细配置
                if (patchType == PatchType.Fire && enable)
                {
                    LogFireProofDetails();
                }
            }
            catch (Exception ex)
            {
                ModLogger.LogError($"Failed to toggle patch {patchType}: {ex}");
            }
        }

        /// <summary>
        /// 插件卸载时的清理
        /// </summary>
        void OnDestroy()
        {
            try
            {
                ModLogger?.LogInfo("Plugin unloading...");
                _harmony?.UnpatchSelf();
                ModLogger?.LogInfo("All patches removed");
            }
            catch (Exception ex)
            {
                ModLogger?.LogError($"Error during plugin cleanup: {ex}");
            }
        }
    }

    /// <summary>
    /// 插件元数据（由MSBuild自动生成）
    /// </summary>
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.travellerse.plugins.ProofAcidFireCold";
        public const string PLUGIN_NAME = "Proof Acid Fire Cold";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
