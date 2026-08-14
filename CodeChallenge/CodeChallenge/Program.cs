using System;

namespace CodeChallenge
{
    public  class Program
    {
        public static void Main(string[] args)
        {
            //    Console.WriteLine("=== Demostración de Algoritmo de Anagrama Optimizando O(N) ===\n");

            //    Anagram ana = new Anagram();
            //    // Casos de prueba
            //    ana.TestAnagram("Roma", "Amor");            
            //    ana.TestAnagram("Listen", "Silent");
            //    ana.TestAnagram("Casa", "Cara");
            //    ana.TestAnagram("Hola", "Mundo");

            //Balanced.TestBalanced("[{}]");

            // Palindrome.TestPalindrome("Anita lava la tina");
        //    Console.WriteLine(Reverse.ReverseString("HOla"));
          //  Console.WriteLine(Reverse.ReverseEachWord("HOla mundo"));

        string sample = "swiss";
        char? result = Basic.FindFirstNonRepeatingChar(sample);
        Console.WriteLine(result.HasValue ? $"First non-repeating char: {result}" : "No unique char found");

        }
        
    }
}