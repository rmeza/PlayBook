using System;
using System.Collections.Generic;

namespace CodeChallenge
{
    public static class LongestSubstringNoRepeat
    {
        /// <summary>
        /// Longitud de la subcadena más larga sin caracteres repetidos.
        /// Usa sliding window con Dictionary para recordar la última posición de cada carácter.
        /// Complejidad Temporal: O(N) · Espacial: O(min(N, charset)).
        /// </summary>
        public static int LongestSubstringLength(string input)
        {
            if (string.IsNullOrEmpty(input))
                return 0;

            Dictionary<char, int> lastSeen = new Dictionary<char, int>();
            int maxLength = 0;
            int windowStart = 0;

            for (int windowEnd = 0; windowEnd < input.Length; windowEnd++)
            {
                char c = input[windowEnd];

                if (lastSeen.TryGetValue(c, out int prevIndex) && prevIndex >= windowStart)
                {
                    windowStart = prevIndex + 1;
                }

                lastSeen[c] = windowEnd;
                maxLength = Math.Max(maxLength, windowEnd - windowStart + 1);
            }

            return maxLength;
        }
    }
}