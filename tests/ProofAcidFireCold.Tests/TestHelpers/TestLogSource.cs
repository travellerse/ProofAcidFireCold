using System;
using System.Collections.Generic;
using ProofAcidFireCold.Logging;

namespace ProofAcidFireCold.Tests
{
    /// <summary>
    /// 测试用的日志源，捕获所有日志消息以便验证
    /// </summary>
    public class TestLogSource : ILogSource
    {
        private readonly List<string> _logs = new List<string>();

        public IReadOnlyList<string> Logs => _logs.AsReadOnly();

        public void LogInfo(object data)
        {
            _logs.Add($"[Info] {data}");
        }

        public void LogError(object data)
        {
            _logs.Add($"[Error] {data}");
        }

        public void LogWarning(object data)
        {
            _logs.Add($"[Warning] {data}");
        }

        public void LogDebug(object data)
        {
            _logs.Add($"[Debug] {data}");
        }
    }
}
