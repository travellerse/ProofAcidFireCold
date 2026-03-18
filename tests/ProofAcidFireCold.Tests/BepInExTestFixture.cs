using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;

namespace ProofAcidFireCold.Tests
{
    /// <summary>
    /// xUnit Fixture 用于初始化 BepInEx 测试环境
    /// 在所有测试运行之前执行一次
    /// </summary>
    public class BepInExTestFixture : IDisposable
    {
        private readonly string _testConfigDir;

        public BepInExTestFixture()
        {
            // 创建测试配置目录
            _testConfigDir = Path.Combine(Path.GetTempPath(), "ProofAcidFireCold_Tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testConfigDir);

            // 使用反射绕过 ConfigFile 静态构造函数的问题
            // 强制初始化 ConfigFile 类型，让静态构造函数运行
            try
            {
                // 获取 ConfigFile 的私有静态字段（如果有）
                var configFileType = typeof(ConfigFile);
                var staticFields = configFileType.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                // 尝试通过反射设置任何需要的静态字段
                foreach (var field in staticFields)
                {
                    if (field.FieldType == typeof(string) && field.GetValue(null) == null)
                    {
                        // 如果有 null 的 string 静态字段，设置一个临时路径
                        field.SetValue(null, Path.Combine(_testConfigDir, "default.cfg"));
                    }
                }
            }
            catch
            {
                // 反射失败，尝试直接创建实例
            }

            // 创建一个初始配置文件来触发静态初始化
            var initConfigPath = Path.Combine(_testConfigDir, "init.cfg");
            try
            {
                // 强制触发静态构造函数
                RuntimeHelpers.RunClassConstructor(typeof(ConfigFile).TypeHandle);
            }
            catch
            {
                // 静态构造函数可能失败，但我们继续
            }
        }

        public void Dispose()
        {
            // 清理测试目录
            try
            {
                if (Directory.Exists(_testConfigDir))
                {
                    Directory.Delete(_testConfigDir, true);
                }
            }
            catch
            {
                // 忽略清理错误
            }
        }
    }

    /// <summary>
    /// xUnit Collection 定义，用于共享 BepInExTestFixture
    /// </summary>
    [Xunit.CollectionDefinition("BepInEx Tests")]
    public class BepInExTestCollection : Xunit.ICollectionFixture<BepInExTestFixture>
    {
        // 这个类不需要任何代码，只是作为 Collection 的定义
    }
}
