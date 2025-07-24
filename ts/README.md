# AZPOS TypeScript Implementation

TypeScript implementation of the azpos library for lexicographic position calculations.

## Installation

```bash
npm install
```

## Usage

```typescript
import { midpoint, validate, needsRebalance, rebalance3, compare } from './src/index';

// Generate a position between two existing positions
const pos = midpoint("a", "z"); // "az"

// Validate a position string
const isValid = validate("abc"); // true

// Check if rebalancing is needed
const needsRebalnce = needsRebalance("a", "aa"); // false

// Generate 3 evenly spaced positions
const [pos1, pos2, pos3] = rebalance3("a", "z"); // ["ah", "ap", "ax"]

// Compare positions
const result = compare("a", "b"); // -1
```

## API

| Function | Description |
| -------- | ----------- |
| `midpoint(a, b)` | Return lexicographic midpoint between `a` and `b`, enforcing `a` < pos < `b` |
| `validate(pos)` | Ensure position string is lowercase a–z and ≤ max length |
| `needsRebalance(a, b)` | True if midpoint would exceed length or conflict |
| `rebalance3(a, b, c)` | Return 3 evenly spaced values between "a" and "z" |
| `compare(a, b)` | Lex sort logic (safe in all langs) |

## Development

```bash
# Install dependencies
npm install

# Run tests
npm test

# Build
npm run build

# Lint
npm run lint
```

## Testing

This implementation uses the shared test vectors from `../test-vectors/positions.json` to ensure consistency with other language implementations.
