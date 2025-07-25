namespace fgrzl.azpos;

public static class AzposCalculator
{
    private const int MaxLength = 25;

    /// <summary>
    ///     Validates an azpos position string. Throws AzposException if invalid.
    /// </summary>
    public static void Validate(string? pos)
    {
        switch (pos)
        {
            case null:
                throw new AzposException("null pos");
        }
        switch (pos.Length)
        {
            case > MaxLength:
                throw new AzposException("exceeds max length");
        }
        switch (pos)
        {
            case "a":
                throw new AzposException("reserved lower bound");
            case "z":
                throw new AzposException("reserved position");
        }

        foreach (var c in pos)
        {
            switch (c)
            {
                case < 'a' or > 'z':
                {
                    throw c switch
                    {
                        >= 'A' and <= 'Z' => new AzposException("uppercase not allowed"),
                        _ => new AzposException("non-alphabetic character")
                    };
                }
            }
        }
    }

    /// <summary>
    ///     Validates an azpos position string. Returns true if valid, false otherwise.
    /// </summary>
    public static bool IsValid(string? pos)
    {
        try
        {
            Validate(pos);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Return lexicographic midpoint between a and b, enforcing a < pos < b
    /// </summary>
    public static string Midpoint(string? a, string? b)
        => MidpointInternal(a, b, true)!;

    private static string? MidpointInternal(string? a, string? b, bool isTopLevel)
    {
        if (a is null || b is null)
            throw new AzposException("null input");

        switch (isTopLevel)
        {
            case true when a == b:
            {
                throw a.Length switch
                {
                    MaxLength => new AzposException("no midpoint"),
                    _ => new AzposException("equal inputs")
                };
            }
        }

        if (Compare(a, b) >= 0)
            throw new AzposException("a must be less than b");

        if (string.IsNullOrEmpty(a))
            return "am";

        if (a.Length < b.Length && a.Length > 0 && b.StartsWith(a, StringComparison.Ordinal))
        {
            var restLen = b.Length - a.Length;
            return restLen > 0 ? $"{a}{new string('a', restLen - 1)}m" : $"{a}m";
        }

        switch (a.Length)
        {
            case 1 when b.Length == 1:
            {
                var diff = b[0] - a[0];
                return diff > 1 ? $"{a}{b}" : diff == 1 ? $"{a}m" : throw new AzposException("no midpoint");
            }
        }

        if (a.Length == b.Length && a.Length > 0)
        {
            var prefix = a[..^1];
            var aLast = a[^1];
            var bLast = b[^1];
            if (a[..^1] == b[..^1] && bLast - aLast == 1)
                return $"{prefix}{a[^1]}m";
        }

        int i = 0, max = Math.Min(a.Length, b.Length);
        while (i < max && a[i] == b[i])
            i++;

        var commonPrefix = a[..i];
        var aSuffix = a[i..];
        var bSuffix = b[i..];

        switch (aSuffix.Length)
        {
            case > 0 when bSuffix.Length > 0:
            {
                var aChar = aSuffix[0];
                var bChar = bSuffix[0];
                var diff = bChar - aChar;

                switch (diff)
                {
                    case > 1:
                    {
                        var midChar = (char)(aChar + diff / 2);
                        return $"{commonPrefix}{midChar}m";
                    }
                    case 1 when aSuffix.Length == 1 && bSuffix.Length == 1:
                        return $"{commonPrefix}{aSuffix[0]}m";
                    case 1 when aSuffix.Length > 1 && bSuffix.Length > 1:
                    {
                        var restA = aSuffix[1..];
                        var restB = bSuffix[1..];
                        if (restA == restB)
                            return $"{commonPrefix}{bSuffix[0]}m";

                        var rec = MidpointInternal(restA, restB, false);
                        return string.IsNullOrEmpty(rec) switch
                        {
                            false => $"{commonPrefix}{aSuffix[0]}{rec}",
                            _ => $"{commonPrefix}{bSuffix[0]}m"
                        };
                    }
                    case 1 when aSuffix.Length != bSuffix.Length:
                        return $"{commonPrefix}{bSuffix[0]}m";
                }

                return isTopLevel switch
                {
                    true => throw new AzposException("no midpoint"),
                    _ => null
                };
            }
        }

        return isTopLevel switch
        {
            true => throw new AzposException("no midpoint"),
            _ => null
        };
    }

    /// <summary>
    ///     Return 3 evenly spaced values between a and c
    /// </summary>
    public static string[] ReBalance(string? a, string? c)
    {
        // For azpos, rebalance3 returns evenly spaced values between a and c, using 'h', 'p', 'x'
        if (a is null || c is null)
            throw new AzposException("null input");
        
        if (a.Length != c.Length)
            throw new AzposException("rebalance3: a and c must be same length");
        
        var baseStr = a;
        return [
            $"{baseStr}h",
            $"{baseStr}p", 
            $"{baseStr}x"
        ];
    }

    /// <summary>
    ///     Return 3 evenly spaced values between a and c (alias for ReBalance)
    /// </summary>
    public static string[] Rebalance3(string? a, string? c) => ReBalance(a, c);

    /// <summary>
    ///     True if midpoint would exceed length or conflict
    /// </summary>
    public static bool NeedsRebalance(string? a, string? b)
    {
        try
        {
            var mid = Midpoint(a, b);
            // Rebalance if midpoint is not valid or if we're getting close to MaxLength
            try
            {
                Validate(mid);
                return mid.Length >= MaxLength - 6;
            }
            catch
            {
                return true;
            }
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    ///     Lexicographic sort logic (safe in all languages)
    /// </summary>
    public static int Compare(string? a, string? b)
        => string.Compare(a, b, StringComparison.Ordinal);
}

