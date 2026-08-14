using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CodeChallenge
{
    internal class Reverse
    {
        /// <summary>
        /// Variación A: Revertir todo el texto en su lugar (In-Place)
        ///  Transformar "hello" en "olleh". Usamos el mismo patrón de Two Pointers
        ///  intercambiando(swapping) los caracteres desde los extremos hacia el centro.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReverseString(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            char[] chars = s.ToCharArray();
            int left = 0;
            int right = chars.Length - 1;

            while (left < right)
            {
                // Swapping (Intercambio de posiciones)
                char temp = chars[left];
                chars[left] = chars[right];
                chars[right] = temp;

                left++;
                right--;
            }

            return new string(chars);
        }

        public static string ReverseEachWord(string sentence)
        {
            if (string.IsNullOrEmpty(sentence)) return sentence;

            // 1. Dividir por espacios
            string[] words = sentence.Split(' ');

            // 2. Revertir cada palabra individualmente
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = ReverseString(words[i]); // Reutilizamos el algoritmo anterior
            }

            // 3. Volver a unir con espacios
            return string.Join(" ", words);
        }
    }
}
