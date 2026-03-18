using System;
using System.IO;
using Xunit;
using FluentAssertions;
using ProofAcidFireCold.Configuration;

namespace ProofAcidFireCold.Tests
{
    /// <summary>
    /// 配置管理类测试
    /// </summary>
    public class ConfigTests : IDisposable
    {
        private readonly TestLogSource _testLogger;
        private readonly TestConfigFile _testConfigFile;

        public ConfigTests()
        {
            _testLogger = new TestLogSource();
            _testConfigFile = new TestConfigFile();
        }

        public void Dispose()
        {
            // 测试用配置文件不需要清理
        }

        [Fact(Skip = "BepInEx ConfigFile static constructor issue in Mono test environment")]
        public void Constructor_WithNullConfig_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Action act = () => new ModConfig(null, _testLogger);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("config");
        }

        [Fact(Skip = "BepInEx ConfigFile static constructor issue in Mono test environment")]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Action act = () => new ModConfig(_testConfigFile, null);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("logger");
        }

        [Fact(Skip = "BepInEx ConfigFile static constructor issue in Mono test environment")]
        public void Initialize_ShouldCreateAllConfigEntries()
        {
            // Arrange
            var modConfig = new ModConfig(_testConfigFile, _testLogger);

            // Act
            modConfig.Initialize((type, enabled) => { });

            // Assert
            modConfig.ProofAcid.Should().NotBeNull();
            modConfig.ProofFire.Should().NotBeNull();
            modConfig.ProofCold.Should().NotBeNull();
            modConfig.ProofSteal.Should().NotBeNull();
            modConfig.MeatOnMapProofFire.Should().NotBeNull();
            modConfig.GarbageProofFire.Should().NotBeNull();
            modConfig.DisableBlanketsCost.Should().NotBeNull();
        }

        [Fact(Skip = "BepInEx ConfigFile static constructor issue in Mono test environment")]
        public void Initialize_ShouldSetDefaultValues()
        {
            // Arrange
            var modConfig = new ModConfig(_testConfigFile, _testLogger);

            // Act
            modConfig.Initialize((type, enabled) => { });

            // Assert
            modConfig.ProofAcid.Value.Should().BeTrue();
            modConfig.ProofFire.Value.Should().BeTrue();
            modConfig.ProofCold.Value.Should().BeTrue();
            modConfig.ProofSteal.Value.Should().BeTrue();
            modConfig.MeatOnMapProofFire.Value.Should().BeFalse();
            modConfig.GarbageProofFire.Value.Should().BeFalse();
            modConfig.DisableBlanketsCost.Value.Should().BeTrue();
        }

        [Fact(Skip = "BepInEx ConfigFile static constructor issue in Mono test environment")]
        public void SettingChanged_ShouldTriggerCallback()
        {
            // Arrange
            var modConfig = new ModConfig(_testConfigFile, _testLogger);
            PatchType? capturedType = null;
            bool? capturedValue = null;

            modConfig.Initialize((type, enabled) =>
            {
                capturedType = type;
                capturedValue = enabled;
            });

            // Act
            modConfig.ProofAcid.Value = false;

            // Assert
            capturedType.Should().Be(PatchType.Acid);
            capturedValue.Should().BeFalse();
        }
    }
}
