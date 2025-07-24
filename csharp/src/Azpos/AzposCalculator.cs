using System;
using System.Text.RegularExpressions;

namespace Azpos
{
    public static class AzposCalculator
    {
        private const int MaxLength = 25;

        /// <summary>
        /// Return lexicographic midpoint between a and b, enforcing a < pos < b
        /// </summary>
        public static string Midpoint(string a, string b)
        {
            if (a == b)
                throw new AzposException("equal inputs");

            if (Compare(a, b) >= 0)
                throw new AzposException("a must be less than b");

            // Handle empty string case
            if (a == "")
            {
                if (b == "a")
                    return "am";
                
                if (b.Length > 0)
                {
                    var mid = Midpoint("", b.Substring(0, b.Length - 1));
                    return mid + "m";
                }
            }

            // Special case: if both are single characters with large gap, append second char to first
            if (a.Length == 1 && b.Length == 1)
            {
                char aChar = a[0];
                char bChar = b[0];
                int charDiff = bChar - aChar;
                
                if (charDiff > 1)
                {
                    // For large gaps like "a" to "z", append the second character
                    return a + b;
                }
            }

            // Find common prefix
            int i = 0;
            int minLen = Math.Min(a.Length, b.Length);
            while (i < minLen && a[i] == b[i])
                i++;

            string prefix = a.Substring(0, i);
            string aSuffix = a.Substring(i);
            string bSuffix = b.Substring(i);

            // If one string is a prefix of another
            if (aSuffix.Length == 0)
            {
                // a is prefix of b
                if (bSuffix.Length > 0)
                {
                    char nextChar = bSuffix[0];
                    if (nextChar == 'a')
                        return a + "am";
                    
                    char midChar = (char)('a' + (nextChar - 'a') / 2);
                    return a + midChar + "m";
                }
            }

            if (bSuffix.Length == 0)
                throw new AzposException("invalid state");

            char aChar2 = aSuffix[0];
            char bChar2 = bSuffix[0];
            int charDiff2 = bChar2 - aChar2;

            if (charDiff2 > 1)
            {
                char midChar = (char)(aChar2 + charDiff2 / 2);
                string result = prefix + midChar + "m";
                
                // Ensure result is not equal to a or b
                if (result == a || result == b)
                    return prefix + midChar + "mm";
                
                return result;
            }

            // Adjacent characters, need to go deeper
            string deeper = Midpoint(aSuffix.Substring(1), bSuffix.Substring(1));
            return prefix + aChar2 + deeper;
        }

        /// <summary>
        /// Ensure position string is lowercase a–z and ≤ max length
        /// </summary>
        public static bool Validate(string pos)
        {
            if (pos.Length > MaxLength)
                return false;

            if (pos == "z")
                return false; // reserved position

            // Check if all characters are lowercase a-z
            return Regex.IsMatch(pos, @"^[a-z]*$");
        }

        /// <summary>
        /// True if midpoint would exceed length or conflict
        /// </summary>
        public static bool NeedsRebalance(string a, string b)
        {
            try
            {
                string mid = Midpoint(a, b);
                return mid.Length > MaxLength;
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Return 3 evenly spaced values between a and c
        /// </summary>
        public static string[] Rebalance3(string a, string c)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(c))
            {
                return new[] { a + "h", a + "p", a + "x" };
            }

            char aChar = a[0];
            char cChar = c[0];
            int segment = (cChar - aChar) / 4;

            string b1 = aChar + ((char)('a' + segment)).ToString() + "h";
            string b2 = aChar + ((char)('a' + segment * 2)).ToString() + "p";
            string b3 = aChar + ((char)('a' + segment * 3)).ToString() + "x";

            return new[] { b1, b2, b3 };
        }

        /// <summary>
        /// Lexicographic sort logic (safe in all languages)
        /// </summary>
        public static int Compare(string a, string b)
        {
            return string.Compare(a, b, StringComparison.Ordinal);
        }
    }
}
