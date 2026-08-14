using System;
using System.Collections.Generic;

namespace CodeChallenge
{
    public static class Basic
    {
        public static char? FindFirstNonRepeatingChar(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            //count frequency using Dictionary
            Dictionary<char, int> charCounts = new Dictionary<char, int>();
            foreach (char c in input)
            {
                if (charCounts.ContainsKey(c))
                    charCounts[c]++;
                else
                    charCounts[c] = 1;
            }

            //Step 2: Find the first character with a count of 1
            foreach(char c in input)
            {
                if(charCounts[c]==1)
                return c;
            }

            return null;

        }

    }
}
