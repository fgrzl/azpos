namespace fgrzl.azpos;

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Azpos;
using Xunit;

public class AzposVectorsTests
{
    private static readonly JsonElement Vectors = LoadVectors();

    private static JsonElement LoadVectors()
    {
        var path = FindVectorsFile();
        var json = File.ReadAllText(path);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    private static string FindVectorsFile()
    {
        // 1. Check for .test-vectors/expectations.json in output dir (copied by csproj)
        var outputDir = AppContext.BaseDirectory;
        var outputVectors = Path.Combine(outputDir, ".test-vectors", "expectations.json");
        if (File.Exists(outputVectors))
            return outputVectors;

        // 2. Traverse up to find repo-root .test-vectors/expectations.json
        var dir = outputDir;
        for (var i = 0; i < 8; i++) // go up to 8 levels up
        {
            var candidate = Path.Combine(dir, ".test-vectors", "expectations.json");
            if (File.Exists(candidate))
                return candidate;
            dir = Path.GetDirectoryName(dir.TrimEnd(Path.DirectorySeparatorChar));
            if (string.IsNullOrEmpty(dir)) break;
        }
        throw new FileNotFoundException("Could not find expectations.json from output dir or repo root. Checked from: " + AppContext.BaseDirectory);
    }

    [Theory]
    [MemberData(nameof(GetMidpointPositive))]
    public void Midpoint_Positive(string a, string b, string expected)
    {
        var result = AzposCalculator.Midpoint(a, b);
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> GetMidpointPositive()
    {
        foreach (var item in Vectors.GetProperty("midpoint").GetProperty("positive").EnumerateArray())
        {
            yield return new object[]
            {
                item.GetProperty("a").GetString(),
                item.GetProperty("b").GetString(),
                item.GetProperty("expected").GetString()
            };
        }
    }

    [Theory]
    [MemberData(nameof(GetMidpointNegative))]
    public void Midpoint_Negative(string a, string b, string error)
    {
        var ex = Assert.Throws<AzposException>(() => AzposCalculator.Midpoint(a, b));
        Assert.Contains(error, ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    public static IEnumerable<object[]> GetMidpointNegative()
    {
        foreach (var item in Vectors.GetProperty("midpoint").GetProperty("negative").EnumerateArray())
        {
            yield return new object[]
            {
                item.GetProperty("a").GetString(),
                item.GetProperty("b").GetString(),
                item.GetProperty("error").GetString()
            };
        }
    }

    [Theory]
    [MemberData(nameof(GetValidatePositive))]
    public void Validate_Positive(string pos)
    {
        AzposCalculator.Validate(pos);
    }

    public static IEnumerable<object[]> GetValidatePositive()
    {
        foreach (var item in Vectors.GetProperty("validate").GetProperty("positive").EnumerateArray())
        {
            yield return new object[] { item.GetProperty("pos").GetString() };
        }
    }

    [Theory]
    [MemberData(nameof(GetValidateNegative))]
    public void Validate_Negative(string pos, string error)
    {
        var ex = Assert.Throws<AzposException>(() => AzposCalculator.Validate(pos));
        Assert.Contains(error, ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    public static IEnumerable<object[]> GetValidateNegative()
    {
        foreach (var item in Vectors.GetProperty("validate").GetProperty("negative").EnumerateArray())
        {
            yield return new object[]
            {
                item.GetProperty("pos").GetString(),
                item.GetProperty("error").GetString()
            };
        }
    }

    [Theory]
    [MemberData(nameof(GetCompareVectors))]
    public void Compare(string a, string b, int expected)
    {
        var result = AzposCalculator.Compare(a, b);
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> GetCompareVectors()
    {
        foreach (var item in Vectors.GetProperty("compare").EnumerateArray())
        {
            yield return new object[]
            {
                item.GetProperty("a").GetString(),
                item.GetProperty("b").GetString(),
                item.GetProperty("result").GetInt32()
            };
        }
    }
}
