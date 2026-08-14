
using FluentAssertions;
using CodeChallenge;

namespace CodeChallengeTests
{
    public class PalindromeTests
    {
        [Theory]
        [InlineData("reconocer", true)]
        [InlineData("Anita lava la tina", true)]
        [InlineData("A man, a plan, a canal: Panama", true)]
        [InlineData("hola", false)]
        [InlineData("casi palindromo", false)]
        [InlineData("", true)]
        public void IsPalindrome_ShouldValidateCorrectly(string input, bool expectedResult)
        {
            // Act
            bool result = Palindrome.IsPalindrome(input);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void IsPalindrome_WhenInputIsNull_ShouldReturnTrue()
        {
            // Act
            bool result = Palindrome.IsPalindrome(null!);

            // Assert
            result.Should().BeTrue();
        }
    }
}
