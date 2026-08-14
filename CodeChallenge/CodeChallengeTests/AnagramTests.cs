using Xunit;
using FluentAssertions;
using CodeChallenge; // Importa el namespace de tu clase Anagram

namespace CodeChallengeTests
{
    public class AnagramTests
    {
        [Theory]
        [InlineData("Roma", "Amor", true)]
        [InlineData("Listen", "Silent", true)]
        [InlineData("Casa", "Cara", false)]
        [InlineData("Hola", "Mundo", false)]
        [InlineData("a", "a", true)]
        public void AreAnagrams_ValidAndInvalidPairs_ShouldReturnExpectedResult(string str1, string str2, bool expectedResult)
        {
            // Act
            bool result = Anagram.AreAnagramsOptimized(str1, str2);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("abc", "abcd")]
        [InlineData("abcd", "abc")]
        public void AreAnagrams_DifferentLengths_ShouldReturnFalse(string str1, string str2)
        {
            // Act
            bool result = Anagram.AreAnagramsOptimized(str1, str2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void AreAnagrams_WhenEitherInputIsNull_ShouldReturnFalse()
        {
            // Act & Assert (Uso de null! para evitar advertencias de compilación si tus parámetros aceptan nulos)
            Anagram.AreAnagramsOptimized(null!, "Roma").Should().BeFalse();
            Anagram.AreAnagramsOptimized("Roma", null!).Should().BeFalse();
            Anagram.AreAnagramsOptimized(null!, null!).Should().BeFalse();
        }
    }
}