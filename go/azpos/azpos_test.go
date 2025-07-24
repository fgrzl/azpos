package azpos

import (
	"encoding/json"
	"os"
	"testing"

	"github.com/stretchr/testify/assert"
)

type midpointCase struct {
	A        string `json:"a"`
	B        string `json:"b"`
	Expected string `json:"expected,omitempty"`
	Error    string `json:"error,omitempty"`
}
type validateCase struct {
	Pos   string `json:"pos"`
	Desc  string `json:"desc,omitempty"`
	Error string `json:"error,omitempty"`
}
type needsRebalanceCase struct {
	A string `json:"a"`
	B string `json:"b"`
}
type rebalance3Case struct {
	A        string   `json:"a"`
	C        string   `json:"c"`
	Expected []string `json:"expected"`
}
type compareCase struct {
	A      string `json:"a"`
	B      string `json:"b"`
	Result int    `json:"result"`
}

type expectations struct {
	Midpoint struct {
		Positive []midpointCase `json:"positive"`
		Negative []midpointCase `json:"negative"`
	} `json:"midpoint"`
	Validate struct {
		Positive []validateCase `json:"positive"`
		Negative []validateCase `json:"negative"`
	} `json:"validate"`
	NeedsRebalance struct {
		Positive []needsRebalanceCase `json:"positive"`
		Negative []needsRebalanceCase `json:"negative"`
	} `json:"needsRebalance"`
	Rebalance3 []rebalance3Case `json:"rebalance3"`
	Compare    []compareCase    `json:"compare"`
}

func loadExpectations(t *testing.T) expectations {
	f, err := os.Open("../../test-vectors/expectations.json")
	if err != nil {
		t.Fatalf("failed to open test vectors: %v", err)
	}
	defer f.Close()
	var e expectations
	dec := json.NewDecoder(f)
	if err := dec.Decode(&e); err != nil {
		t.Fatalf("failed to decode test vectors: %v", err)
	}
	return e
}

func TestMidpointVectors(t *testing.T) {
	e := loadExpectations(t)
	for _, tc := range e.Midpoint.Positive {
		out, err := Midpoint(tc.A, tc.B)
		assert.NoError(t, err, "midpoint(%q, %q) unexpected error", tc.A, tc.B)
		assert.Equal(t, tc.Expected, out, "midpoint(%q, %q)", tc.A, tc.B)
	}
	for _, tc := range e.Midpoint.Negative {
		_, err := Midpoint(tc.A, tc.B)
		assert.Error(t, err, "midpoint(%q, %q) should error", tc.A, tc.B)
	}
}

func TestValidateVectors(t *testing.T) {
	e := loadExpectations(t)
	for _, tc := range e.Validate.Positive {
		assert.True(t, Validate(tc.Pos), "validate(%q) should be true", tc.Pos)
	}
	for _, tc := range e.Validate.Negative {
		assert.False(t, Validate(tc.Pos), "validate(%q) should be false", tc.Pos)
	}
}

func TestNeedsRebalanceVectors(t *testing.T) {
	e := loadExpectations(t)
	for _, tc := range e.NeedsRebalance.Positive {
		assert.True(t, NeedsRebalance(tc.A, tc.B), "needsRebalance(%q, %q) should be true", tc.A, tc.B)
	}
	for _, tc := range e.NeedsRebalance.Negative {
		assert.False(t, NeedsRebalance(tc.A, tc.B), "needsRebalance(%q, %q) should be false", tc.A, tc.B)
	}
}

func TestRebalance3Vectors(t *testing.T) {
	e := loadExpectations(t)
	for _, tc := range e.Rebalance3 {
		out, err := Rebalance3(tc.A, tc.C)
		assert.NoError(t, err, "rebalance3(%q, %q) unexpected error", tc.A, tc.C)
		assert.Equal(t, tc.Expected, out[:], "rebalance3(%q, %q)", tc.A, tc.C)
	}
}

func TestCompareVectors(t *testing.T) {
	e := loadExpectations(t)
	for _, tc := range e.Compare {
		assert.Equal(t, tc.Result, Compare(tc.A, tc.B), "compare(%q, %q)", tc.A, tc.B)
	}
}

func BenchmarkMidpoint(b *testing.B) {
	for i := 0; i < b.N; i++ {
		Midpoint("aaaaaaaaaaaaaaaaaaaaaaaay", "aaaaaaaaaaaaaaaaaaaaaaaaz")
	}
}

func BenchmarkValidate(b *testing.B) {
	for i := 0; i < b.N; i++ {
		Validate("aaaaaaaaaaaaaaaaaaaaaaaaym")
	}
}

func BenchmarkNeedsRebalance(b *testing.B) {
	for i := 0; i < b.N; i++ {
		NeedsRebalance("aaaaaaaaaaaaaaaaaaaaaaaay", "aaaaaaaaaaaaaaaaaaaaaaaaz")
	}
}

func BenchmarkRebalance3(b *testing.B) {
	for i := 0; i < b.N; i++ {
		Rebalance3("aaaaaaaaaaaaaaaaaaaaaaaay", "aaaaaaaaaaaaaaaaaaaaaaaaz")
	}
}

func BenchmarkCompare(b *testing.B) {
	for i := 0; i < b.N; i++ {
		Compare("aaaaaaaaaaaaaaaaaaaaaaaay", "aaaaaaaaaaaaaaaaaaaaaaaaz")
	}
}
