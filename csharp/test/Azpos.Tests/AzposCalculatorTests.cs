using Xunit;
using Azpos;

namespace Azpos.Tests
{
    public class AzposCalculatorTests
    {
        [Fact]
        public void Midpoint_BasicCalculations_ReturnsExpectedResults()
        {
            // Arrange & Act & Assert
            Assert.Equal("az", AzposCalculator.Midpoint("a", "z"));
            Assert.Equal("bm", AzposCalculator.Midpoint("az", "bz"));
            Assert.Equal("am", AzposCalculator.Midpoint("a", "ab"));
            Assert.Equal("aaam", AzposCalculator.Midpoint("aaa", "aab"));
        }

        [Fact]
        public void Midpoint_EmptyStringInput_ReturnsExpectedResult()
        {
            // Arrange & Act & Assert
            Assert.Equal("am", AzposCalculator.Midpoint("", "a"));
        }

        [Fact]
        public void Midpoint_EqualInputs_ThrowsAzposException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<AzposException>(() => AzposCalculator.Midpoint("a", "a"));
            Assert.Equal("equal inputs", exception.Message);
        }

        [Fact]
        public void Midpoint_InvalidOrder_ThrowsAzposException()
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<AzposException>(() => AzposCalculator.Midpoint("b", "a"));
            Assert.Equal("a must be less than b", exception.Message);
        }

        [Theory]
        [InlineData("abc", true)]
        [InlineData("a", true)]
        [InlineData("", true)]
        [InlineData("z", false)] // reserved position
        [InlineData("ABC", false)] // uppercase
        [InlineData("ab1", false)] // numbers
        [InlineData("ab!", false)] // special characters
        public void Validate_VariousInputs_ReturnsExpectedResults(string pos, bool expected)
        {
            // Arrange & Act
            var result = AzposCalculator.Validate(pos);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Validate_TooLongString_ReturnsFalse()
        {
            // Arrange - create a string longer than MAX_LENGTH (25)
            var longString = new string('a', 26);

            // Act
            var result = AzposCalculator.Validate(longString);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("a", "z", false)]
        [InlineData("a", "aa", false)]
        public void NeedsRebalance_VariousInputs_ReturnsExpectedResults(string a, string b, bool expected)
        {
            // Arrange & Act
            var result = AzposCalculator.NeedsRebalance(a, b);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NeedsRebalance_EqualInputs_ReturnsTrue()
        {
            // Arrange & Act
            var result = AzposCalculator.NeedsRebalance("a", "a");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Rebalance3_BasicInputs_ReturnsThreeValues()
        {
            // Arrange & Act
            var result = AzposCalculator.Rebalance3("a", "z");

            // Assert
            Assert.Equal(3, result.Length);
            Assert.Matches(@"^a[a-z]h$", result[0]);
            Assert.Matches(@"^a[a-z]p$", result[1]);
            Assert.Matches(@"^a[a-z]x$", result[2]);
        }

        [Theory]
        [InlineData("a", "b", -1)]
        [InlineData("b", "a", 1)]
        [InlineData("a", "a", 0)]
        [InlineData("aa", "ab", -1)]
        [InlineData("abc", "abd", -1)]
        public void Compare_VariousInputs_ReturnsExpectedResults(string a, string b, int expected)
        {
            // Arrange & Act
            var result = AzposCalculator.Compare(a, b);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
