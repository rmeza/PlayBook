using System;
using System.Text;

namespace CodeChallenge
{
    public static class StringCompression
    {
        /// <summary>
        /// Run-Length Encoding: comprime grupos de caracteres consecutivos iguales.
        /// Si la versión comprimida no es más corta que la original, devuelve la original.
        /// Complejidad Temporal: O(N) · Espacial: O(1) extra (además de la salida).
        /// </summary>
        public static string Compression(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            StringBuilder sb = new StringBuilder();
            int runLength = 1;

            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] == input[i - 1])
                {
                    runLength++;
                }
                else
                {
                    AppendRun(sb, input[i - 1], runLength);
                    runLength = 1;
                }
            }

            AppendRun(sb, input[^1], runLength);

            return sb.Length < input.Length ? sb.ToString() : input;
        }

        private static void AppendRun(StringBuilder sb, char c, int count)
        {
            sb.Append(c);
            if (count > 1) sb.Append(count);
        }
    }
}