# AZPOS Go Implementation

Go implementation of the azpos library for azimuthal position calculations.

## Installation

```bash
go mod download
```

## Usage

```go
package main

import (
    "fmt"
    "github.com/fgrzl/azpos/go/azpos"
)

func main() {
    result := azpos.CalculatePosition(azpos.Input{
        Latitude:  40.7128,
        Longitude: -74.0060,
        Azimuth:   45.0,
        Distance:  1000.0,
    })
    
    fmt.Printf("Result: %+v\n", result)
}
```

## Development

```bash
# Run tests
go test ./...

# Run tests with coverage
go test -cover ./...

# Build
go build ./...

# Format code
go fmt ./...

# Lint (requires golangci-lint)
golangci-lint run
```

## Testing

This implementation uses the shared test vectors from `../test-vectors/positions.json` to ensure consistency with other language implementations.
