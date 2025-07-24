use serde::{Deserialize, Serialize};
use std::cmp::Ordering;

const MAX_LENGTH: usize = 25;

#[derive(Debug, Clone)]
pub enum AzposError {
    EqualInputs,
    InvalidOrder,
    InvalidState,
    ReservedPosition,
    InvalidChars,
    TooLong,
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
        }
    }
}

impl std::error::Error for AzposError {}

/// Not implemented stub for midpoint
pub fn midpoint(_a: &str, _b: &str) -> Result<String, AzposError> {
    Err(AzposError::InvalidState)
}

    // Handle empty string case
    if a.is_empty() {
        if b == "a" {
            return Ok("am".to_string());
        }
        
        if !b.is_empty() {
            let mid = midpoint("", &b[..b.len()-1])?;
            return Ok(mid + "m");
        }
    }

    // Find common prefix
    let mut i = 0;
    let min_len = a.len().min(b.len());
    let a_chars: Vec<char> = a.chars().collect();
    let b_chars: Vec<char> = b.chars().collect();
    
    while i < min_len && a_chars[i] == b_chars[i] {
        i += 1;
    }

    let prefix: String = a_chars[..i].iter().collect();
    let a_suffix: String = a_chars[i..].iter().collect();
    let b_suffix: String = b_chars[i..].iter().collect();

    // If one string is a prefix of another
    if a_suffix.is_empty() {
        // a is prefix of b
        if !b_suffix.is_empty() {
            let next_char = b_suffix.chars().next().unwrap();
            if next_char == 'a' {
                return Ok(a.to_string() + "am");
            }
            
            let mid_char = char::from(b'a' + (next_char as u8 - b'a') / 2);
            return Ok(a.to_string() + &mid_char.to_string() + "m");
        }
    }

    if b_suffix.is_empty() {
        return Err(AzposError::InvalidState);
    }

    let a_char = a_suffix.chars().next().unwrap();
    let b_char = b_suffix.chars().next().unwrap();
    let char_diff = b_char as u8 - a_char as u8;

    if char_diff > 1 {
        let mid_char = char::from(a_char as u8 + char_diff / 2);
        let result = prefix + &mid_char.to_string() + "m";
        
        // Ensure result is not equal to a or b
        if result == a || result == b {
            return Ok(prefix + &mid_char.to_string() + "mm");
        }
        
        return Ok(result);
    }

    // Adjacent characters, need to go deeper
    let a_rest: String = a_suffix.chars().skip(1).collect();
    let b_rest: String = b_suffix.chars().skip(1).collect();
    let deeper = midpoint(&a_rest, &b_rest)?;
    Ok(prefix + &a_char.to_string() + &deeper)
}

/// Ensure position string is lowercase a–z and ≤ max length
pub fn validate(pos: &str) -> Result<(), AzposError> {
    if pos.len() > MAX_LENGTH {
        return Err(AzposError::TooLong);
    }

    if pos == "z" {
        return Err(AzposError::ReservedPosition);
    }

    // Check if all characters are lowercase a-z
    for c in pos.chars() {
        if !c.is_ascii_lowercase() || c < 'a' || c > 'z' {
            return Err(AzposError::InvalidChars);
        }
    }

    Ok(())
}

/// True if midpoint would exceed length or conflict
pub fn needs_rebalance(a: &str, b: &str) -> bool {
    match midpoint(a, b) {
        Ok(mid) => mid.len() > MAX_LENGTH,
        Err(_) => true,
    }
}

/// Return 3 evenly spaced values between a and c
pub fn rebalance3(a: &str, c: &str) -> [String; 3] {
    if a.is_empty() || c.is_empty() {
        return [
            a.to_string() + "h",
            a.to_string() + "p", 
            a.to_string() + "x"
        ];
    }

    let a_char = a.chars().next().unwrap() as u8;
    let c_char = c.chars().next().unwrap() as u8;
    let segment = (c_char - a_char) / 4;

    let b1 = format!("{}{}{}", char::from(a_char), char::from(b'a' + segment), "h");
    let b2 = format!("{}{}{}", char::from(a_char), char::from(b'a' + segment * 2), "p");
    let b3 = format!("{}{}{}", char::from(a_char), char::from(b'a' + segment * 3), "x");

    [b1, b2, b3]
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
        assert_eq!(midpoint("a", "z").unwrap(), "az");
        assert_eq!(midpoint("az", "bz").unwrap(), "bm");
    }

    #[test]
    fn test_midpoint_equal_inputs() {
        assert!(matches!(midpoint("a", "a"), Err(AzposError::EqualInputs)));
    }

    #[test]
    fn test_validate_basic() {
        assert!(validate("abc").is_ok());
        assert!(validate("a").is_ok());
        assert!(validate("").is_ok());
    }

    #[test]
    fn test_validate_reserved() {
        assert!(matches!(validate("z"), Err(AzposError::ReservedPosition)));
    }

    #[test]
    fn test_validate_invalid_chars() {
        assert!(matches!(validate("ABC"), Err(AzposError::InvalidChars)));
        assert!(matches!(validate("ab1"), Err(AzposError::InvalidChars)));
    }

    #[test]
    fn test_compare() {
        assert_eq!(compare("a", "b"), Ordering::Less);
        assert_eq!(compare("b", "a"), Ordering::Greater);
        assert_eq!(compare("a", "a"), Ordering::Equal);
    }
}
