# AZPOS Rust Implementation

Rust implementation of the azpos library for azimuthal position calculations.

## Installation

```bash
cargo build
```

## Usage

```rust
use azpos::{calculate_position, PositionInput};

fn main() {
    let input = PositionInput {
        latitude: 40.7128,
        longitude: -74.0060,
        azimuth: 45.0,
        distance: 1000.0,
    };
    
    let result = calculate_position(input);
    println!("Result: {:.6}, {:.6}", result.latitude, result.longitude);
}
```

## Development

```bash
# Build
cargo build

# Run tests
cargo test

# Run tests with output
cargo test -- --nocapture

# Build documentation
cargo doc

# Format code
cargo fmt

# Lint
cargo clippy

# Release build
cargo build --release
```

## Testing

This implementation uses the shared test vectors from `../test-vectors/positions.json` to ensure consistency with other language implementations.

## Crates.io Publication

This library can be published to crates.io for easy distribution and consumption in Rust projects.
