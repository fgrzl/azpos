# AZPOS - Multi-Language Lexicographic Position Library

A multi-language library for handling lexicographic position calculations, providing consistent implementations across TypeScript, Go, C#, Java, and Rust. Perfect for implementing fractional indexing in ordered lists and trees.

## Features

- **`midpoint(a, b)`** - Return lexicographic midpoint between `a` and `b`, enforcing `a` < pos < `b`
- **`validate(pos)`** - Ensure position string is lowercase a–z and ≤ max length
- **`needsRebalance(a, b)`** - True if midpoint would exceed length or conflict
- **`rebalance3(a, b, c)`** - Return 3 evenly spaced values between "a" and "z"
- **`compare(a, b)`** - Lexicographic sort logic (safe in all langs)

## Structure

- `test-vectors/` - Shared golden test cases for consistency across all implementations
- `ts/` - TypeScript implementation
- `go/` - Go implementation  
- `csharp/` - C# (.NET) implementation
- `java/` - Java implementation
- `rust/` - Rust implementation
- `scripts/` - Build and test automation scripts
- `.github/workflows/` - CI/CD workflows for all languages

## Testing

All implementations use the same test vectors from `test-vectors/positions.json` to ensure mathematical consistency across languages.

## Getting Started

Each language implementation has its own README with specific setup instructions:

- [TypeScript](./ts/README.md)
- [Go](./go/README.md)
- [C#](./csharp/README.md)
- [Java](./java/README.md)
- [Rust](./rust/README.md)

## Contributing

Please ensure all implementations pass the shared test vectors before submitting changes.
