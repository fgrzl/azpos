namespace Azpos;

using System;

public static class AzposCalculator
{

    /// <summary>
    /// Validates an azpos position string. Throws AzposException if invalid.
    /// </summary>
    public static void Validate(string? pos)
    {
        if (pos == null)
            throw new AzposException("null pos");
        var length = pos.Length;
        if (length > MaxLength)
            throw new AzposException("exceeds max length");
        if (pos == "a")
            throw new AzposException("reserved lower bound");
        if (pos == "z")
            throw new AzposException("reserved position");
        for (var i = 0; i < length; i++)
        {
            var c = pos[i];
            if (c < 'a' || c > 'z')
            {
                if (c >= 'A' && c <= 'Z')
                    throw new AzposException("uppercase not allowed");
                else
                    throw new AzposException("non-alphabetic character");
            }
        }
    }
    const int MaxLength = 25;

    /// <summary>
    /// Return lexicographic midpoint between a and b, enforcing a < pos < b
    /// </summary>
    public static string Midpoint(string? a, string? b) => MidpointInternal(a, b, true);


    static string? MidpointInternal(string? a, string? b, bool isTopLevel)
    {
        if (a == null || b == null)
            throw new AzposException("null input");

        if (isTopLevel && a == b)
        {
            // Special case: if equal strings are at max length, return "no midpoint"
            if (a.Length == MaxLength)
                throw new AzposException("no midpoint");
            else
                throw new AzposException("equal inputs");
        }
        if (Compare(a, b) >= 0)
            throw new AzposException("a must be less than b");

        // If a is empty, always return 'am'
        if (string.IsNullOrEmpty(a))
            return "am";

        // If a is a prefix of b, extend with 'a's and 'm' at the right depth
        if (a.Length < b.Length && a.Length > 0 && b.StartsWith(a))
        {
            var restLen = b.Length - a.Length;
            return restLen > 0 ? a + new string('a', restLen - 1) + "m" : a + "m";
        }

        // If both are single chars
        if (a.Length == 1 && b.Length == 1)
        {
            var diff = b[0] - a[0];
            return diff > 1 ? a + b : diff == 1 ? a + "m" : throw new AzposException("no midpoint");
        }

        // If equal length, all but last char equal, and last char differs by 1
        if (a.Length == b.Length && a.Length > 0)
        {
            var prefix = a[..^1];
            var aLast = a[^1];
            var bLast = b[^1];
            if (a[..^1] == b[..^1] && bLast - aLast == 1)
                return prefix + a[^1] + "m";
        }

        // Find common prefix
        var i = 0;
        var max = Math.Min(a.Length, b.Length);
        while (i < max && a[i] == b[i])
            i++;

        var commonPrefix = a[..i];
        var aSuffix = a[i..];
        var bSuffix = b[i..];

        if (aSuffix.Length > 0 && bSuffix.Length > 0)
        {
            var aChar = aSuffix[0];
            var bChar = bSuffix[0];
            var diff = bChar - aChar;

            if (diff > 1)
            {
                var midChar = (char)(aChar + diff / 2);
                return commonPrefix + midChar + "m";
            }

            if (diff == 1)
            {
                if (aSuffix.Length == 1 && bSuffix.Length == 1)
                    return commonPrefix + aSuffix[0] + "m";

                if (aSuffix.Length > 1 && bSuffix.Length > 1)
                {
                    var restA = aSuffix[1..];
                    var restB = bSuffix[1..];
                    if (restA == restB)
                        return commonPrefix + bSuffix[0] + "m";

                    try
                    {
                        var rec = MidpointInternal(restA, restB, false);
                        if (!string.IsNullOrEmpty(rec))
                            return commonPrefix + aSuffix[0] + rec;
                    }
                    catch
                    {
                        // Fall through to default case
                    }
                    return commonPrefix + bSuffix[0] + "m";
                }

                if (aSuffix.Length != bSuffix.Length)
                    return commonPrefix + bSuffix[0] + "m";
            }

            if (isTopLevel)
                throw new AzposException("no midpoint");
            return null;
        }

        if (isTopLevel)
            throw new AzposException("no midpoint");
        return null;
    }
    /// <summary>
    /// Return 3 evenly spaced values between a and c
    /// </summary>
    public static string[] Rebalance3(string? a, string? c)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(c))
            return [
                (a ?? "") + "h",
                (a ?? "") + "p",
                (a ?? "") + "x"
            ];

        var aChar = a[0];
        var cChar = c[0];
        var segment = (cChar - aChar) / 4;

        return [
            aChar + ((char)('a' + segment)).ToString() + "h",
            aChar + ((char)('a' + segment * 2)).ToString() + "p",
            aChar + ((char)('a' + segment * 3)).ToString() + "x"
        ];
    }

    /// <summary>
    /// Lexicographic sort logic (safe in all languages)
    /// </summary>
    public static int Compare(string? a, string? b) => string.Compare(a, b, StringComparison.Ordinal);
}
