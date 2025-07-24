package azpos

import (
	"errors"
	"math"
	"strings"
)

const MaxLength = 25

var (
	ErrEqualInputs  = errors.New("equal inputs")
	ErrInvalidOrder = errors.New("a must be less than b")
	ErrInvalidState = errors.New("invalid state")
	ErrReservedPos  = errors.New("reserved position")
	ErrInvalidChars = errors.New("invalid characters")
	ErrTooLong      = errors.New("position too long")
)

// Midpoint returns lexicographic midpoint between a and b, enforcing a < pos < b
func Midpoint(a, b string) (string, error) {
	if a == b {
		return "", ErrEqualInputs
	}

	if Compare(a, b) >= 0 {
		return "", ErrInvalidOrder
	}

	// Handle empty string case
	if a == "" {
		if b == "a" {
			return "am", nil
		}
		if len(b) > 0 {
			mid, err := Midpoint("", b[:len(b)-1])
			if err != nil {
				return "", err
			}
			return mid + "m", nil
		}
	}

	// Find common prefix
	i := 0
	minLen := int(math.Min(float64(len(a)), float64(len(b))))
	for i < minLen && a[i] == b[i] {
		i++
	}

	prefix := a[:i]
	aSuffix := a[i:]
	bSuffix := b[i:]

	// If one string is a prefix of another
	if len(aSuffix) == 0 {
		// a is prefix of b
		if len(bSuffix) > 0 {
			nextChar := bSuffix[0]
			if nextChar == 'a' {
				return a + "am", nil
			}
			midChar := byte('a' + (nextChar-'a')/2)
			return a + string(midChar) + "m", nil
		}
	}

	if len(bSuffix) == 0 {
		return "", ErrInvalidState
	}

	aChar := aSuffix[0]
	bChar := bSuffix[0]
	charDiff := bChar - aChar

	if charDiff > 1 {
		midChar := aChar + charDiff/2
		result := prefix + string(midChar) + "m"

		// Ensure result is not equal to a or b
		if result == a || result == b {
			return prefix + string(midChar) + "mm", nil
		}
		return result, nil
	}

	// Adjacent characters, need to go deeper
	deeper, err := Midpoint(aSuffix[1:], bSuffix[1:])
	if err != nil {
		return "", err
	}
	return prefix + string(aChar) + deeper, nil
}

// Validate ensures position string is lowercase a–z and ≤ max length
func Validate(pos string) error {
	if len(pos) > MaxLength {
		return ErrTooLong
	}

	if pos == "z" {
		return ErrReservedPos
	}

	// Check if all characters are lowercase a-z
	for _, r := range pos {
		if r < 'a' || r > 'z' {
			return ErrInvalidChars
		}
	}

	return nil
}

// NeedsRebalance returns true if midpoint would exceed length or conflict
func NeedsRebalance(a, b string) bool {
	mid, err := Midpoint(a, b)
	if err != nil {
		return true
	}
	return len(mid) > MaxLength
}

// Rebalance3 returns 3 evenly spaced values between a and c
func Rebalance3(a, c string) [3]string {
	if len(a) == 0 || len(c) == 0 {
		return [3]string{a + "h", a + "p", a + "x"}
	}

	aChar := a[0]
	cChar := c[0]
	segment := (cChar - aChar) / 4

	b1 := string(aChar) + string('a'+segment) + "h"
	b2 := string(aChar) + string('a'+segment*2) + "p"
	b3 := string(aChar) + string('a'+segment*3) + "x"

	return [3]string{b1, b2, b3}
}

// Compare provides lexicographic sort logic (safe in all languages)
func Compare(a, b string) int {
	return strings.Compare(a, b)
}
