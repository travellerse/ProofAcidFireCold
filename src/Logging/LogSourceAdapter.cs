using BepInEx.Logging;

namespace ProofAcidFireCold.Logging
{
    /// <summary>
    /// ManualLogSource的适配器，实现ILogSource接口
    /// </summary>
    public class LogSourceAdapter : ILogSource
    {
        private readonly ManualLogSource _logger;

        public LogSourceAdapter(ManualLogSource logger)
        {
            _logger = logger;
        }

        public void LogInfo(object data) => _logger.LogInfo(data);
        public void LogError(object data) => _logger.LogError(data);
        public void LogWarning(object data) => _logger.LogWarning(data);
        public void LogDebug(object data) => _logger.LogDebug(data);
    }
}
