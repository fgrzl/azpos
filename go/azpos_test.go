package azpos_test

import (
	"testing"

	azpos "github.com/fgrzl/azpos/go"
)

func TestMidpoint(t *testing.T) {
	tests := []struct {
		a, b     string
		expected string
		hasError bool
	}{
		{"a", "z", "az", false},
		{"az", "bz", "bm", false},
		{"a", "ab", "am", false},
		{"a", "a", "", true}, // equal inputs should error
		{"", "a", "am", false},
	}

	for _, test := range tests {
		result, err := azpos.Midpoint(test.a, test.b)

		if test.hasError {
			if err == nil {
				t.Errorf("Expected error for midpoint(%q, %q), got %q", test.a, test.b, result)
			}
		} else {
			if err != nil {
				t.Errorf("Unexpected error for midpoint(%q, %q): %v", test.a, test.b, err)
				continue
			}
			if result != test.expected {
				t.Errorf("midpoint(%q, %q) = %q, expected %q", test.a, test.b, result, test.expected)
			}
		}
	}
}

func TestValidate(t *testing.T) {
	tests := []struct {
		pos   string
		valid bool
	}{
		{"abc", true},
		{"a", true},
		{"", true},
		{"z", false},   // reserved
		{"ABC", false}, // uppercase
		{"ab1", false}, // invalid chars
	}

	for _, test := range tests {
		err := azpos.Validate(test.pos)
		isValid := err == nil

		if isValid != test.valid {
			t.Errorf("validate(%q) = %v, expected %v", test.pos, isValid, test.valid)
		}
	}
}

func TestCompare(t *testing.T) {
	tests := []struct {
		a, b   string
		result int
	}{
		{"a", "b", -1},
		{"b", "a", 1},
		{"a", "a", 0},
		{"aa", "ab", -1},
	}

	for _, test := range tests {
		result := azpos.Compare(test.a, test.b)
		if result != test.result {
			t.Errorf("compare(%q, %q) = %d, expected %d", test.a, test.b, result, test.result)
		}
	}
}
