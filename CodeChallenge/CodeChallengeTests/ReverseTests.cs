using Xunit;
using FluentAssertions;
using CodeChallenge;

namespace CodeChallengeTests
{
    public class ReverseTests
    {
        [Theory]
        [InlineData("hello", "olleh")]
        [InlineData("Hola", "aloH")]
        [InlineData("a", "a")]
        [InlineData("", "")]
        public void ReverseString_ShouldReverseInPlace(string input, string expectedResult)
        {
            // Act
            string result = Reverse.ReverseString(input);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ReverseString_WhenInputIsNull_ShouldReturnNull()
        {
            // Act
            string result = Reverse.ReverseString(null!);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("Hola mundo", "aloH odnum")]
        [InlineData("hello world", "olleh dlrow")]
        [InlineData("single", "elgnis")]
        [InlineData("", "")]
        public void ReverseEachWord_ShouldReverseEveryWordIndependently(string input, string expectedResult)
        {
            // Act
            string result = Reverse.ReverseEachWord(input);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void ReverseEachWord_WhenInputIsNull_ShouldReturnNull()
        {
            // Act
            string result = Reverse.ReverseEachWord(null!);

            // Assert
            result.Should().BeNull();
        }
    }
}