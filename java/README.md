# AZPOS Java Implementation

Java implementation of the azpos library for azimuthal position calculations.

## Installation

```bash
mvn install
```

## Usage

```java
import com.fgrzl.azpos.AzposCalculator;
import com.fgrzl.azpos.PositionInput;
import com.fgrzl.azpos.PositionResult;

public class Example {
    public static void main(String[] args) {
        PositionInput input = new PositionInput(40.7128, -74.0060, 45.0, 1000.0);
        PositionResult result = AzposCalculator.calculatePosition(input);
        
        System.out.printf("Result: %.6f, %.6f%n", 
            result.getLatitude(), result.getLongitude());
    }
}
```

## Development

```bash
# Compile
mvn compile

# Run tests
mvn test

# Package JAR
mvn package

# Install to local repository
mvn install

# Deploy to repository
mvn deploy
```

## Testing

This implementation uses the shared test vectors from `../test-vectors/positions.json` to ensure consistency with other language implementations.

## Maven Artifact

This library can be published to Maven Central for easy distribution and consumption in Java projects.
