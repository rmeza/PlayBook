using FluentAssertions;
using CodeChallenge;

namespace CodeChallengeTests
{
    public class LongestSubstringNoRepeatTests
    {
        [Theory]
        [InlineData("abcabcbb", 3)]
        [InlineData("bbbbb", 1)]
        [InlineData("pwwkew", 3)]
        [InlineData("", 0)]
        [InlineData("a", 1)]
        [InlineData("au", 2)]
        [InlineData("dvdf", 3)]
        [InlineData("abba", 2)]
        public void LongestSubstringLength_ShouldReturnCorrectLength(string input, int expectedResult)
        {
            // Act
            int result = LongestSubstringNoRepeat.LongestSubstringLength(input);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void LongestSubstringLength_WhenInputIsNull_ShouldReturnZero()
        {
            // Act
            int result = LongestSubstringNoRepeat.LongestSubstringLength(null!);

            // Assert
            result.Should().Be(0);
        }
    }
}