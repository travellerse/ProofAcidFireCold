using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace ProofAcidFireCold.Tests
{
    /// <summary>
    /// 测试用的 ConfigFile 实现
    /// 完全在内存中，不依赖 BepInEx 的静态初始化
    /// </summary>
    public class TestConfigFile : Configuration.IConfigFile
    {
        private readonly Dictionary<string, ConfigEntryBase> _entries = new Dictionary<string, ConfigEntryBase>();

        // 内部临时配置文件，用于创建真实的 ConfigEntry 实例
        private static ConfigFile _tempConfig;

        static TestConfigFile()
        {
            // 在静态构造函数中初始化一个临时配置文件
            // 这会在第一次使用 TestConfigFile 时运行，并触发 BepInEx ConfigFile 的静态构造函数
            var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.cfg");
            _tempConfig = new ConfigFile(tempPath, false);
        }

        public ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description)
        {
            var entryKey = $"{section}.{key}";

            if (_entries.TryGetValue(entryKey, out var existing))
            {
                return existing as ConfigEntry<T>;
            }

            // 使用内部临时配置文件创建真实的 ConfigEntry
            // 这样可以保留所有 BepInEx 的行为（包括事件）
            var entry = _tempConfig != null
                ? _tempConfig.Bind(section, key, defaultValue, description)
                : null;

            if (entry != null)
            {
                _entries[entryKey] = entry;
            }

            return entry;
        }
    }
}
