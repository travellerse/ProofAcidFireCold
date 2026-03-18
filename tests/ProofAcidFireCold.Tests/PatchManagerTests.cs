using System;
using Xunit;
using FluentAssertions;
using HarmonyLib;
using ProofAcidFireCold.Configuration;
using ProofAcidFireCold.Utils;

namespace ProofAcidFireCold.Tests
{
    /// <summary>
    /// 补丁管理器测试
    /// 使用 Mono 运行时测试 Harmony 补丁管理
    /// </summary>
    public class PatchManagerTests
    {
        private readonly TestLogSource _testLogger;
        private readonly Harmony _harmony;

        public PatchManagerTests()
        {
            _testLogger = new TestLogSource();
            _harmony = new Harmony("com.travellerse.plugins.ProofAcidFireCold.Tests");
        }

        [Fact]
        public void Constructor_WithNullHarmony_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Action act = () => new PatchManager(null, _testLogger);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("harmony");
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            var harmony = new Harmony("com.travellerse.plugins.ProofAcidFireCold.Tests");
            Action act = () => new PatchManager(harmony, null);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("logger");
        }

        [Fact]
        public void Constructor_ShouldInitializeSuccessfully()
        {
            // Arrange & Act
            var harmony = new Harmony("com.travellerse.plugins.ProofAcidFireCold.Tests");
            var patchManager = new PatchManager(harmony, _testLogger);

            // Assert
            patchManager.Should().NotBeNull();
            patchManager.GetAppliedPatchCount().Should().Be(0);
        }

        [Fact]
        public void GetAppliedPatchCount_Initially_ShouldBeZero()
        {
            // Arrange
            var harmony = new Harmony("com.travellerse.plugins.ProofAcidFireCold.Tests");
            var patchManager = new PatchManager(harmony, _testLogger);

            // Act
            var count = patchManager.GetAppliedPatchCount();

            // Assert
            count.Should().Be(0);
        }

        [Fact]
        public void IsPatchApplied_ForUnappliedPatch_ShouldReturnFalse()
        {
            // Arrange
            var harmony = new Harmony("com.travellerse.plugins.ProofAcidFireCold.Tests");
            var patchManager = new PatchManager(harmony, _testLogger);

            // Act
            var isApplied = patchManager.IsPatchApplied(PatchType.Acid);

            // Assert
            isApplied.Should().BeFalse();
        }

        // Note: Cannot test actual patch application without game assemblies
        // These tests verify the PatchManager's internal state management
    }
}
