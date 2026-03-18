using BepInEx.Configuration;

namespace ProofAcidFireCold.Configuration
{
    /// <summary>
    /// ConfigFile 适配器，包装 BepInEx.Configuration.ConfigFile
    /// 用于生产环境
    /// </summary>
    public class ConfigFileAdapter : IConfigFile
    {
        private readonly ConfigFile _config;

        public ConfigFileAdapter(ConfigFile config)
        {
            _config = config;
        }

        public ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description)
        {
            return _config.Bind(section, key, defaultValue, description);
        }
    }
}
