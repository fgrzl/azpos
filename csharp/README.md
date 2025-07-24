# AZPOS C# Implementation

C# (.NET) implementation of the azpos library for lexicographic position calculations.

## Project Structure

```
csharp/
├── Azpos.sln                    # Solution file
├── src/
│   └── Azpos/                   # Main library project
│       ├── Azpos.csproj
│       ├── AzposCalculator.cs
│       └── AzposException.cs
└── test/
    └── Azpos.Tests/             # Test project
        ├── Azpos.Tests.csproj
        ├── AzposCalculatorTests.cs
        └── TestVectorTests.cs
```

## Installation

```bash
dotnet restore
```

## Usage

```csharp
using Azpos;

// Generate a position between two existing positions
var pos = AzposCalculator.Midpoint("a", "z"); // "az"

// Validate a position string
var isValid = AzposCalculator.Validate("abc"); // true

// Check if rebalancing is needed
var needsRebalance = AzposCalculator.NeedsRebalance("a", "aa"); // false

// Generate 3 evenly spaced positions
var positions = AzposCalculator.Rebalance3("a", "z"); // ["ah", "ap", "ax"]

// Compare positions
var result = AzposCalculator.Compare("a", "b"); // -1
```

## API

| Method | Description |
| ------ | ----------- |
| `Midpoint(a, b)` | Return lexicographic midpoint between `a` and `b`, enforcing `a` < pos < `b` |
| `Validate(pos)` | Ensure position string is lowercase a–z and ≤ max length |
| `NeedsRebalance(a, b)` | True if midpoint would exceed length or conflict |
| `Rebalance3(a, b, c)` | Return 3 evenly spaced values between "a" and "z" |
| `Compare(a, b)` | Lexicographic sort logic (safe in all langs) |

## Development

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Pack NuGet package
dotnet pack
```

## Testing

This implementation includes:
- **Unit tests** for all public methods with comprehensive test cases
- **Test vector validation** using the shared test vectors from `../test-vectors/` to ensure consistency with other language implementations
- **XUnit framework** for modern .NET testing

## NuGet Package

This library can be published as a NuGet package for easy distribution and consumption in .NET projects. The package configuration is defined in `src/Azpos/Azpos.csproj`.
