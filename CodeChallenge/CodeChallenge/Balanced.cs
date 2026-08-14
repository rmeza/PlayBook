using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChallenge
{
    public class Balanced
    {

        public static void TestBalanced(string word)
        {
            //bool isBalanced = IsBalancedSimple(word);
            bool isBalanced = IsBalancedMulti(word);
            string resultText = isBalanced ? "SÍ balanced" : "NO balanced";

            Console.WriteLine($"{word} -> {resultText}");
        }

        public static bool IsBalancedSimple(string s)
        {
            if (string.IsNullOrEmpty(s)) return true;
            if (s.Length % 2 != 0) return false;

            int balance = 0;

            foreach (char c in s)
            {
                if (c == '(')
                {
                    balance++;
                }
                else if (c == ')')
                {
                    balance--;

                    // Si el balance es negativo, hay un ')' que no tiene '(' antes.
                    // Ejemplo: ")(" -> En la primera posición balance pasa a -1
                    if (balance < 0) return false;
                }
            }

            return balance == 0;
        }

        public static bool IsBalancedMulti(string s)
        {
            if (string.IsNullOrEmpty(s)) return true;
            if (s.Length % 2 != 0) return false;

            Stack<char> expected = new Stack<char>();

            foreach (char c in s)
            {
                if (c == '(') expected.Push(')');
                else if (c == '{') expected.Push('}');
                else if (c == '[') expected.Push(']');
                else if (c == ')' || c == '}' || c == ']')
                {
                    if (expected.Count == 0 || expected.Pop() != c)
                        return false;
                }
            }

            return expected.Count == 0;
        }
    }
}
