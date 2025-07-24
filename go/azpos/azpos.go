// Package azpos provides lexicographic position calculations for fractional indexing.
package azpos

import "errors"

const maxLength = 32

var (
	ErrAzpos         = errors.New("azpos: error")
	ErrEqualInputs   = errors.New("azpos: equal inputs")
	ErrOrder         = errors.New("azpos: a must be less than b")
	ErrNoMidpoint    = errors.New("azpos: no midpoint")
	ErrRebalance3Len = errors.New("azpos: rebalance3: a and c must be same length")
)

func Midpoint(a, b string) (string, error) {
	return midpoint(a, b, true)
}

func midpoint(a, b string, isTopLevel bool) (string, error) {
	if isTopLevel && a == b {
		return "", ErrEqualInputs
	}
	if Compare(a, b) >= 0 {
		return "", ErrOrder
	}
	if a == "" {
		return "am", nil
	}
	if len(a) < len(b) && len(a) > 0 && b[:len(a)] == a {
		restLen := len(b) - len(a)
		if restLen > 0 {
			return a + repeat("a", restLen-1) + "m", nil
		}
	}
	if len(a) == 1 && len(b) == 1 {
		aChar := a[0]
		bChar := b[0]
		diff := int(bChar) - int(aChar)
		if diff > 1 {
			return a + b, nil
		}
		if diff == 1 {
			return a + "m", nil
		}
	}
	if len(a) == len(b) && len(a) > 0 {
		prefix := a[:len(a)-1]
		aLast := a[len(a)-1]
		bLast := b[len(b)-1]
		if a[:len(a)-1] == b[:len(b)-1] && int(bLast)-int(aLast) == 1 {
			return prefix + string(a[len(a)-1]) + "m", nil
		}
	}
	// Find common prefix
	i := 0
	max := min(len(a), len(b))
	for i < max && a[i] == b[i] {
		i++
	}
	prefix := a[:i]
	aSuffix := a[i:]
	bSuffix := b[i:]
	if len(aSuffix) > 0 && len(bSuffix) > 0 {
		aChar := aSuffix[0]
		bChar := bSuffix[0]
		diff := int(bChar) - int(aChar)
		if diff > 1 {
			midChar := byte(int(aChar) + diff/2)
			return prefix + string(midChar) + "m", nil
		}
		if diff == 1 {
			if len(aSuffix) == 1 && len(bSuffix) == 1 {
				return prefix + string(aSuffix[0]) + "m", nil
			}
			if len(aSuffix) > 1 && len(bSuffix) > 1 {
				restA := aSuffix[1:]
				restB := bSuffix[1:]
				if restA == restB {
					return prefix + string(bSuffix[0]) + "m", nil
				}
				rec, err := midpoint(restA, restB, false)
				if err == nil && rec != "" {
					return prefix + string(aSuffix[0]) + rec, nil
				}
				return prefix + string(bSuffix[0]) + "m", nil
			}
			if len(aSuffix) != len(bSuffix) {
				return prefix + string(bSuffix[0]) + "m", nil
			}
		}
		if isTopLevel {
			return "", ErrNoMidpoint
		}
		return "", nil
	}
	if isTopLevel {
		return "", ErrNoMidpoint
	}
	return "", nil
}

func repeat(s string, n int) string {
	if n <= 0 {
		return ""
	}
	if n == 1 {
		return s
	}
	b := make([]byte, n*len(s))
	for i := 0; i < n; i++ {
		copy(b[i*len(s):], s)
	}
	return string(b)
}

func min(a, b int) int {
	if a < b {
		return a
	}
	return b
}

func Validate(pos string) bool {
	l := len(pos)
	if l > maxLength || l == 0 {
		return l == 0 // only allow empty string if that's valid
	}
	if pos == "a" || pos == "z" {
		return false
	}
	for i := 0; i < l; i++ {
		c := pos[i]
		if c < 'a' || c > 'z' {
			return false
		}
	}
	return true
}

func NeedsRebalance(a, b string) bool {
	mid, err := Midpoint(a, b)
	if err != nil {
		return true
	}
	return !Validate(mid) || len(mid) >= maxLength-6
}

func Rebalance3(a, c string) ([3]string, error) {
	if len(a) != len(c) {
		return [3]string{"", "", ""}, ErrRebalance3Len
	}
	base := a
	return [3]string{base + "h", base + "p", base + "x"}, nil
}

func Compare(a, b string) int {
	if a < b {
		return -1
	}
	if a > b {
		return 1
	}
	return 0
}
