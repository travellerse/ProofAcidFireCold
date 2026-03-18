using System;
using Xunit;
using FluentAssertions;
using ProofAcidFireCold.Constants;

namespace ProofAcidFireCold.Tests
{
    /// <summary>
    /// 游戏常量测试
    /// </summary>
    public class ConstantsTests
    {
        [Fact]
        public void ElementIds_ShouldHaveValidValues()
        {
            // Arrange & Act & Assert
            GameConstants.ElementIds.Fire.Should().Be(910);
            GameConstants.ElementIds.Cold.Should().Be(911);
            GameConstants.ElementIds.Acid.Should().Be(923);
        }

        [Fact]
        public void CategoryIds_ShouldNotBeEmpty()
        {
            // Arrange & Act & Assert
            GameConstants.CategoryIds.Foodstuff.Should().NotBeNullOrEmpty();
            GameConstants.CategoryIds.Garbage.Should().NotBeNullOrEmpty();
            GameConstants.CategoryIds.Junk.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void CategoryIds_ShouldHaveExpectedValues()
        {
            // Arrange & Act & Assert
            GameConstants.CategoryIds.Foodstuff.Should().Be("foodstuff");
            GameConstants.CategoryIds.Garbage.Should().Be("garbage");
            GameConstants.CategoryIds.Junk.Should().Be("junk");
        }

        [Fact]
        public void MessageKeys_ShouldNotBeEmpty()
        {
            // Arrange & Act & Assert
            GameConstants.MessageKeys.BlanketInventory.Should().NotBeNullOrEmpty();
            GameConstants.MessageKeys.BlanketGround.Should().NotBeNullOrEmpty();
            GameConstants.MessageKeys.StealNegateMoney.Should().NotBeNullOrEmpty();
            GameConstants.MessageKeys.StealNegate.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void ActRefKeys_ShouldNotBeEmpty()
        {
            // Arrange & Act & Assert
            GameConstants.ActRefKeys.Money.Should().NotBeNullOrEmpty();
            GameConstants.ActRefKeys.Money.Should().Be("money");
        }
    }
}
