using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChallenge
{
    public static class Palindrome
    {
        public static void TestPalindrome(string word)
        {
            //bool isBalanced = IsBalancedSimple(word);
            bool isPalindrome = IsPalindrome(word);
            string resultText = isPalindrome ? "SÍ " : "NO ";

            Console.WriteLine($"{word} -> {resultText}");
        }

        public static bool IsPalindrome(string s)
        {
            if (string.IsNullOrEmpty(s)) return true;

            int left = 0;
            int right = s.Length - 1;

            while (left < right)
            {
                // Ignorar caracteres que no sean letras o números por la izquierda
                while (left < right && !char.IsLetterOrDigit(s[left]))
                {
                    left++;
                }

                // Ignorar caracteres que no sean letras o números por la derecha
                while (left < right && !char.IsLetterOrDigit(s[right]))
                {
                    right--;
                }

                // Comparar convirtiendo a minúsculas
                if (char.ToLower(s[left]) != char.ToLower(s[right]))
                {
                    return false;
                }

                left++;
                right--;
            }

            return true;
        }
    }
}
