namespace ProofAcidFireCold.Logging
{
    /// <summary>
    /// 日志记录器接口，用于解耦日志依赖，便于测试
    /// </summary>
    public interface ILogSource
    {
        /// <summary>
        /// 记录信息级别日志
        /// </summary>
        void LogInfo(object data);

        /// <summary>
        /// 记录错误级别日志
        /// </summary>
        void LogError(object data);

        /// <summary>
        /// 记录警告级别日志
        /// </summary>
        void LogWarning(object data);

        /// <summary>
        /// 记录调试级别日志
        /// </summary>
        void LogDebug(object data);
    }
}
