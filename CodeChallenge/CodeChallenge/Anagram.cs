using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChallenge
{
    public static class Anagram
    {
        public static void TestAnagram(string word1, string word2)
        {
            bool isAnagram = AreAnagramsOptimized(word1, word2);
            string resultText = isAnagram ? "SÍ son anagramas" : "NO son anagramas";

            Console.WriteLine($"'${word1}' vs '${word2}' -> {resultText}");
        }

        /// <summary>
        /// Determina si dos cadenas son anagramas.
        /// Complejidad Temporal: O(N)
        /// Complejidad Espacial: O(1)
        /// </summary>
        public static bool AreAnagramsOptimized(string str1, string str2)
        {
            // 1. Guardias de seguridad iniciales
            if (str1 == null || str2 == null) return false;
            if (str1.Length != str2.Length) return false;

            // 2. Arreglo fijo de frecuencias (Efecto Balanza / Cajas)
            // 256 posiciones cubre ASCII extendido con O(1) memoria fija.
            int[] charCounts = new int[256];

            // 3. Un solo ciclo: Suma para la 1ra palabra, resta para la 2da
            for (int i = 0; i < str1.Length; i++)
            {
                char char1 = char.ToLower(str1[i]);
                char char2 = char.ToLower(str2[i]);

                charCounts[char1]++;
                charCounts[char2]--;
            }

            // 4. Verificación final de cajas en 0
            for (int i = 0; i < charCounts.Length; i++)
            {
                if (charCounts[i] != 0)
                    return false;
            }

            return true;
        }
    }
}
