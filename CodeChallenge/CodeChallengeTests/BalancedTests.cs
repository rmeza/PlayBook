using Xunit;
using FluentAssertions;
using CodeChallenge;

namespace CodeChallengeTests
{
    public class BalancedTests
    {
        [Theory]
        [InlineData("", true)]
        [InlineData("()", true)]
        [InlineData("()()", true)]
        [InlineData("(())", true)]
        [InlineData(")(", false)]
        [InlineData("(()", false)]
        [InlineData("())", false)]
        public void IsBalancedSimple_ShouldValidateParentheses(string input, bool expectedResult)
        {
            // Act
            bool result = Balanced.IsBalancedSimple(input);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void IsBalancedSimple_WhenInputIsNull_ShouldReturnTrue()
        {
            // Act
            bool result = Balanced.IsBalancedSimple(null!);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("", true)]
        [InlineData("[{}]", true)]
        [InlineData("([{}])", true)]
        [InlineData("{[()]}", true)]
        [InlineData("([)]", false)]
        [InlineData("{[(])}", false)]
        [InlineData("({", false)]
        [InlineData("}{", false)]
        public void IsBalancedMulti_ShouldValidateMixedBrackets(string input, bool expectedResult)
        {
            // Act
            bool result = Balanced.IsBalancedMulti(input);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public void IsBalancedMulti_WhenInputIsNull_ShouldReturnTrue()
        {
            // Act
            bool result = Balanced.IsBalancedMulti(null!);

            // Assert
            result.Should().BeTrue();
        }
    }
}