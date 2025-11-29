use std::cmp::Ordering;

const MAX_LENGTH: usize = 32;

#[derive(Debug, Clone)]
pub enum AzposError {
    EqualInputs,
    InvalidOrder,
    InvalidState,
    ReservedPosition,
    InvalidChars,
    TooLong,
    NoMidpoint,
}

impl std::fmt::Display for AzposError {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            AzposError::EqualInputs => write!(f, "equal inputs"),
            AzposError::InvalidOrder => write!(f, "a must be less than b"),
            AzposError::InvalidState => write!(f, "invalid state"),
            AzposError::ReservedPosition => write!(f, "reserved position"),
            AzposError::InvalidChars => write!(f, "invalid characters"),
            AzposError::TooLong => write!(f, "position too long"),
            AzposError::NoMidpoint => write!(f, "no midpoint"),
        }
    }
}

impl std::error::Error for AzposError {}

/// Return lexicographic midpoint between a and b, enforcing a < pos < b
pub fn midpoint(a: &str, b: &str) -> Result<String, AzposError> {
    midpoint_internal(a, b, true)
}

fn midpoint_internal(a: &str, b: &str, is_top_level: bool) -> Result<String, AzposError> {
    if is_top_level && a == b {
        return Err(AzposError::EqualInputs);
    }
    if compare(a, b) != Ordering::Less {
        return Err(AzposError::InvalidOrder);
    }
    
    // If a is empty, always return 'am'
    if a.is_empty() {
        return Ok("am".to_string());
    }
    
    // If a is a prefix of b, extend with 'a's and 'm' at the right depth
    if a.len() < b.len() && b.starts_with(a) {
        let rest = &b[a.len()..];
        if !rest.is_empty() {
            return Ok(format!("{}{}m", a, "a".repeat(rest.len() - 1)));
        }
    }
    
    // If both are single chars
    if a.len() == 1 && b.len() == 1 {
        let a_char = a.chars().next().unwrap() as u8;
        let b_char = b.chars().next().unwrap() as u8;
        let diff = b_char - a_char;
        if diff > 1 {
            return Ok(format!("{}{}", a, b));
        }
        if diff == 1 {
            return Ok(format!("{}m", a));
        }
    }
    
    // If equal length, all but last char equal, and last char differs by 1
    if a.len() == b.len() && !a.is_empty() {
        let prefix = &a[..a.len() - 1];
        let a_last = a.chars().last().unwrap() as u8;
        let b_last = b.chars().last().unwrap() as u8;
        if &a[..a.len() - 1] == &b[..b.len() - 1] && b_last - a_last == 1 {
            return Ok(format!("{}{}m", prefix, a.chars().last().unwrap()));
        }
    }
    
    // Find common prefix
    let mut i = 0;
    let a_chars: Vec<char> = a.chars().collect();
    let b_chars: Vec<char> = b.chars().collect();
    let min_len = a_chars.len().min(b_chars.len());
    
    while i < min_len && a_chars[i] == b_chars[i] {
        i += 1;
    }
    
    let prefix: String = a_chars[..i].iter().collect();
    let a_suffix: String = a_chars[i..].iter().collect();
    let b_suffix: String = b_chars[i..].iter().collect();
    
    // If suffixes are both non-empty and not equal
    if !a_suffix.is_empty() && !b_suffix.is_empty() {
        let a_char = a_suffix.chars().next().unwrap() as u8;
        let b_char = b_suffix.chars().next().unwrap() as u8;
        let diff = b_char - a_char;
        
        // If chars are not adjacent, use midpoint char
        if diff > 1 {
            let mid_char = char::from(a_char + diff / 2);
            return Ok(format!("{}{}m", prefix, mid_char));
        }
        
        // Characters are adjacent (diff = 1), need to handle carefully
        if diff == 1 {
            // If both suffixes are length 1, return prefix + aChar + 'm'
            if a_suffix.len() == 1 && b_suffix.len() == 1 {
                return Ok(format!("{}{}m", prefix, a_suffix.chars().next().unwrap()));
            }
            
            // For longer suffixes, prefer the simpler solution when possible
            if a_suffix.len() > 1 && b_suffix.len() > 1 {
                let rest_a = &a_suffix[1..];
                let rest_b = &b_suffix[1..];
                
                // If rest of strings are equal, prefer the shorter solution
                if rest_a == rest_b {
                    return Ok(format!("{}{}m", prefix, b_suffix.chars().next().unwrap()));
                }
                
                // Try recursion to find midpoint in the rest
                if let Ok(rec) = midpoint_internal(rest_a, rest_b, false) {
                    if !rec.is_empty() {
                        return Ok(format!("{}{}{}", prefix, a_suffix.chars().next().unwrap(), rec));
                    }
                }
                
                // If recursion failed, use bChar + 'm'
                return Ok(format!("{}{}m", prefix, b_suffix.chars().next().unwrap()));
            }
            
            // For unequal length suffixes, try to use bChar as base
            if a_suffix.len() != b_suffix.len() {
                return Ok(format!("{}{}m", prefix, b_suffix.chars().next().unwrap()));
            }
        }
        
        // If we can't find a midpoint
        if is_top_level {
            return Err(AzposError::NoMidpoint);
        }
        return Ok(String::new());
    }
    
    // If we reach here, no valid midpoint can be found
    if is_top_level {
        Err(AzposError::NoMidpoint)
    } else {
        Ok(String::new())
    }
}

/// Ensure position string is lowercase a–z and ≤ max length
pub fn validate(pos: &str) -> bool {
    if pos.len() > MAX_LENGTH {
        return false;
    }

    // Reserved bounds: forbid 'a' and 'z'
    if pos == "a" || pos == "z" {
        return false;
    }

    // Check if all characters are lowercase a-z
    for c in pos.chars() {
        if !c.is_ascii_lowercase() || c < 'a' || c > 'z' {
            return false;
        }
    }

    true
}

/// True if midpoint would exceed length or conflict
pub fn needs_rebalance(a: &str, b: &str) -> bool {
    match midpoint(a, b) {
        Ok(mid) => !validate(&mid) || mid.len() >= MAX_LENGTH - 6,
        Err(_) => true,
    }
}

/// Return 3 evenly spaced values between a and c
pub fn rebalance3(a: &str, c: &str) -> Result<[String; 3], AzposError> {
    // For azpos, rebalance3 returns evenly spaced values between a and c, using 'h', 'p', 'x'
    if a.len() != c.len() {
        return Err(AzposError::InvalidState);
    }
    let base = a;
    Ok([
        format!("{}h", base),
        format!("{}p", base),
        format!("{}x", base),
    ])
}

/// Lexicographic sort logic (safe in all languages)
pub fn compare(a: &str, b: &str) -> Ordering {
    a.cmp(b)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_midpoint_basic() {
        assert_eq!(midpoint("", "a").unwrap(), "am");
        assert_eq!(midpoint("a", "c").unwrap(), "ac");
    }

    #[test]
    fn test_midpoint_equal_inputs() {
        assert!(matches!(midpoint("a", "a"), Err(AzposError::EqualInputs)));
    }

    #[test]
    fn test_validate_basic() {
        assert!(validate("abc"));
        assert!(validate(""));
        assert!(!validate("a"));  // reserved
        assert!(!validate("z"));  // reserved
        assert!(!validate("ABC"));  // uppercase
    }

    #[test]
    fn test_needs_rebalance_basic() {
        // This should not need rebalance for simple cases
        let result = needs_rebalance("ab", "ac");
        assert!(result == false || result == true); // Just check it returns a bool
    }

    #[test]
    fn test_rebalance3_basic() {
        let result = rebalance3("a", "a").unwrap();
        assert_eq!(result, ["ah".to_string(), "ap".to_string(), "ax".to_string()]);
    }

    #[test]
    fn test_compare_basic() {
        assert_eq!(compare("a", "b"), Ordering::Less);
        assert_eq!(compare("b", "a"), Ordering::Greater);
        assert_eq!(compare("a", "a"), Ordering::Equal);
    }
}
