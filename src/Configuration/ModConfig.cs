using System;
using BepInEx.Configuration;
using ProofAcidFireCold.Logging;

namespace ProofAcidFireCold.Configuration
{
    /// <summary>
    /// Mod配置管理类，处理配置初始化、验证和运行时热重载
    /// </summary>
    public class ModConfig
    {
        private readonly IConfigFile _config;
        private readonly ILogSource _logger;
        private Action<PatchType, bool> _onPatchToggle;

        // 配置项
        public ConfigEntry<bool> ProofAcid { get; private set; }
        public ConfigEntry<bool> ProofFire { get; private set; }
        public ConfigEntry<bool> ProofCold { get; private set; }
        public ConfigEntry<bool> ProofSteal { get; private set; }
        public ConfigEntry<bool> MeatOnMapProofFire { get; private set; }
        public ConfigEntry<bool> GarbageProofFire { get; private set; }
        public ConfigEntry<bool> DisableBlanketsCost { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">配置文件接口</param>
        /// <param name="logger">日志记录器</param>
        public ModConfig(IConfigFile config, ILogSource logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 初始化所有配置项并设置热重载事件处理
        /// </summary>
        /// <param name="onPatchToggle">补丁切换回调，参数为(补丁类型, 是否启用)</param>
        public void Initialize(Action<PatchType, bool> onPatchToggle)
        {
            _onPatchToggle = onPatchToggle;

            // 初始化配置项
            ProofAcid = _config.Bind(
                "ProofAcidFireCold",
                "ProofAcid",
                true,
                "Immunity to acid damage");

            ProofFire = _config.Bind(
                "ProofAcidFireCold",
                "ProofFire",
                true,
                "Immunity to fire damage");

            ProofCold = _config.Bind(
                "ProofAcidFireCold",
                "ProofCold",
                true,
                "Immunity to cold damage");

            ProofSteal = _config.Bind(
                "ProofAcidFireCold",
                "ProofSteal",
                true,
                "Immunity to steal effects");

            MeatOnMapProofFire = _config.Bind(
                "ProofAcidFireCold",
                "MeatOnMapProofFire",
                false,
                "Prevent meat from being cooked by map fire elements when enabled");

            GarbageProofFire = _config.Bind(
                "ProofAcidFireCold",
                "GarbageProofFire",
                false,
                "Prevent garbage from being destroyed by map fire elements when enabled");

            DisableBlanketsCost = _config.Bind(
                "ProofAcidFireCold",
                "DisableBlanketsCost",
                true,
                "Disable the cost of blankets");

            // 注册热重载事件
            ProofAcid.SettingChanged += (sender, args) => OnSettingChanged(PatchType.Acid, ProofAcid.Value);
            ProofFire.SettingChanged += (sender, args) => OnSettingChanged(PatchType.Fire, ProofFire.Value);
            ProofCold.SettingChanged += (sender, args) => OnSettingChanged(PatchType.Cold, ProofCold.Value);
            ProofSteal.SettingChanged += (sender, args) => OnSettingChanged(PatchType.Steal, ProofSteal.Value);
            DisableBlanketsCost.SettingChanged += (sender, args) => OnSettingChanged(PatchType.BlanketCost, DisableBlanketsCost.Value);

            // 记录初始配置
            LogConfiguration();
        }

        /// <summary>
        /// 配置变更事件处理
        /// </summary>
        private void OnSettingChanged(PatchType patchType, bool newValue)
        {
            _logger.LogInfo($"Configuration changed: {patchType} = {newValue}");

            try
            {
                _onPatchToggle?.Invoke(patchType, newValue);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to apply configuration change for {patchType}: {ex}");
            }
        }

        /// <summary>
        /// 记录当前配置状态
        /// </summary>
        private void LogConfiguration()
        {
            _logger.LogInfo("=== Configuration Loaded ===");
            _logger.LogInfo($"ProofAcid: {ProofAcid.Value}");
            _logger.LogInfo($"ProofFire: {ProofFire.Value}");
            _logger.LogInfo($"ProofCold: {ProofCold.Value}");
            _logger.LogInfo($"ProofSteal: {ProofSteal.Value}");
            _logger.LogInfo($"MeatOnMapProofFire: {MeatOnMapProofFire.Value}");
            _logger.LogInfo($"GarbageProofFire: {GarbageProofFire.Value}");
            _logger.LogInfo($"DisableBlanketsCost: {DisableBlanketsCost.Value}");
            _logger.LogInfo("===========================");
        }
    }

    /// <summary>
    /// 补丁类型枚举
    /// </summary>
    public enum PatchType
    {
        Acid,
        Fire,
        Cold,
        Steal,
        BlanketCost
    }
}
