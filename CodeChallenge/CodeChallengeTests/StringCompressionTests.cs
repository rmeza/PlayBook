using Xunit;
using FluentAssertions;
using CodeChallenge;

namespace CodeChallengeTests
{
    public class StringCompressionTests
    {
        [Theory]
        [InlineData("aaabb", "a3b2")]
        [InlineData("aaaa", "a4")]
        [InlineData("aabbaa", "aabbaa")]
        [InlineData("aabbcc", "aabbcc")]
        [InlineData("abc", "abc")]
        [InlineData("a", "a")]
        [InlineData("aa", "aa")]
        [InlineData("", "")]
        public void Compression_ShouldApplyRunLengthEncoding(string input, string expectedResult)
        {
            // Act
            string result = StringCompression.Compression(input);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void Compression_WhenInputIsNull_ShouldReturnNull()
        {
            // Act
            string result = StringCompression.Compression(null!);

            // Assert
            result.Should().BeNull();
        }
    }
}