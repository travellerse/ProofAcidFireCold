using BepInEx.Configuration;

namespace ProofAcidFireCold.Configuration
{
    /// <summary>
    /// 配置文件接口，用于抽象 BepInEx.Configuration.ConfigFile
    /// 便于单元测试
    /// </summary>
    public interface IConfigFile
    {
        /// <summary>
        /// 绑定配置项
        /// </summary>
        ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description);
    }
}
