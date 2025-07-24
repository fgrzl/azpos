using System.Text.Json;
using Xunit;
using Azpos;

namespace Azpos.Tests
{
    public class TestVectorTests
    {
        public class MidpointTestCase
        {
            public string a { get; set; } = "";
            public string b { get; set; } = "";
            public string? expected { get; set; }
            public string? error { get; set; }
            public string? note { get; set; }
        }

        [Fact]
        public void Midpoint_PositiveTestVectors_ProduceExpectedResults()
        {
            // Load positive test vectors
            var positiveJson = File.ReadAllText("../../../../test-vectors/positive.json");
            var positiveTests = JsonSerializer.Deserialize<MidpointTestCase[]>(positiveJson);

            Assert.NotNull(positiveTests);

            foreach (var test in positiveTests)
            {
                if (!string.IsNullOrEmpty(test.expected))
                {
                    var result = AzposCalculator.Midpoint(test.a, test.b);
                    Assert.Equal(test.expected, result);
                }
            }
        }

        [Fact]
        public void Midpoint_NegativeTestVectors_HandleErrorsCorrectly()
        {
            // Load negative test vectors
            var negativeJson = File.ReadAllText("../../../../test-vectors/negative.json");
            var negativeTests = JsonSerializer.Deserialize<MidpointTestCase[]>(negativeJson);

            Assert.NotNull(negativeTests);

            foreach (var test in negativeTests)
            {
                if (!string.IsNullOrEmpty(test.error))
                {
                    if (test.error == "equal inputs")
                    {
                        Assert.Throws<AzposException>(() => AzposCalculator.Midpoint(test.a, test.b));
                    }
                    else if (test.error.Contains("reserved position"))
                    {
                        // This would be handled by validate function
                        if (test.a == "a" || test.a == "z")
                        {
                            Assert.False(AzposCalculator.Validate(test.a));
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(test.expected))
                {
                    // Some negative tests still have expected results with notes
                    var result = AzposCalculator.Midpoint(test.a, test.b);
                    Assert.Equal(test.expected, result);
                }
            }
        }
    }
}
