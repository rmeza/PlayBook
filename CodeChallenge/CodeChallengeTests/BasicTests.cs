using Xunit;
using FluentAssertions;
using CodeChallenge;

namespace CodeChallengeTests
{
    public class BasicTests
    {
        [Theory]
        [InlineData("swiss", 'w')]
        [InlineData("aabbccddeeffg", 'g')]
        [InlineData("abc", 'a')]
        [InlineData("aabbcc", null)]
        [InlineData("", null)]
        public void FindFirstNonRepeatingChar_ShouldReturnExpectedChar(string input, char? expectedResult)
        {
            // Act
            char? result = Basic.FindFirstNonRepeatingChar(input);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void FindFirstNonRepeatingChar_WhenInputIsNull_ShouldReturnNull()
        {
            // Act
            char? result = Basic.FindFirstNonRepeatingChar(null!);

            // Assert
            result.Should().BeNull();
        }
    }
}