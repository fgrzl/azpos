"""
azpos.py - Python implementation of lexicographic position calculations
"""
import re

MAX_LENGTH = 32

class AzposError(Exception):
    pass

def midpoint(a: str, b: str, is_top_level: bool = True) -> str:
    """Return lexicographic midpoint between a and b, enforcing a < pos < b"""
    if is_top_level and a == b:
        raise AzposError("equal inputs")
    if compare(a, b) >= 0:
        raise AzposError("a must be less than b")
    
    # If a is empty, always return 'am' for ('', 'a') or ('', 'ab'), etc.
    if a == '':
        return 'am'
    
    # If a is a prefix of b, extend with 'a's and 'm' at the right depth
    if len(a) < len(b) and b.startswith(a):
        rest = b[len(a):]
        return a + 'a' * (len(rest) - 1) + 'm'
    
    # If both are single chars
    if len(a) == 1 and len(b) == 1:
        a_char = ord(a[0])
        b_char = ord(b[0])
        if b_char - a_char > 1:
            return a + b
        if b_char - a_char == 1:
            return a + 'm'
    
    # If equal length, all but last char equal, and last char differs by 1
    if len(a) == len(b) and len(a) > 0:
        prefix = a[:-1]
        a_last = ord(a[-1])
        b_last = ord(b[-1])
        if a[:-1] == b[:-1] and b_last - a_last == 1:
            return prefix + a[-1] + 'm'
    
    # Find common prefix
    i = 0
    while i < min(len(a), len(b)) and a[i] == b[i]:
        i += 1
    prefix = a[:i]
    a_suffix = a[i:]
    b_suffix = b[i:]
    
    # If suffixes are both non-empty and not equal
    if len(a_suffix) > 0 and len(b_suffix) > 0:
        a_char = ord(a_suffix[0])
        b_char = ord(b_suffix[0])
        
        # If chars are not adjacent, use midpoint char
        if b_char - a_char > 1:
            mid_char = chr(a_char + (b_char - a_char) // 2)
            return prefix + mid_char + 'm'
        
        # Characters are adjacent (diff = 1), need to handle carefully
        if b_char - a_char == 1:
            # If both suffixes are length 1, return prefix + aChar + 'm'
            if len(a_suffix) == 1 and len(b_suffix) == 1:
                return prefix + a_suffix[0] + 'm'
            
            # For longer suffixes, prefer the simpler solution when possible
            if len(a_suffix) > 1 and len(b_suffix) > 1:
                rest_a = a_suffix[1:]
                rest_b = b_suffix[1:]
                
                # If rest of strings are equal, prefer the shorter solution
                if rest_a == rest_b:
                    return prefix + b_suffix[0] + 'm'
                
                # Try recursion to find midpoint in the rest
                try:
                    rec = midpoint(rest_a, rest_b, False)
                    if rec != '':
                        return prefix + a_suffix[0] + rec
                except:
                    pass
                
                # If recursion failed, use bChar + 'm'
                return prefix + b_suffix[0] + 'm'
            
            # For unequal length suffixes, try to use bChar as base
            if len(a_suffix) != len(b_suffix):
                return prefix + b_suffix[0] + 'm'
        
        # If we can't find a midpoint
        if is_top_level:
            raise AzposError("no midpoint")
        return ''
    
    # If we reach here, no valid midpoint can be found
    if is_top_level:
        raise AzposError("no midpoint")
    else:
        return ''

def validate(pos: str) -> bool:
    """Ensure position string is lowercase a–z and ≤ max length"""
    if len(pos) > MAX_LENGTH:
        return False
    
    # Reserved bounds: forbid 'a' and 'z'
    if pos == 'a' or pos == 'z':
        return False
    
    # Check if all characters are lowercase a-z
    return bool(re.match(r'^[a-z]*$', pos))

def needs_rebalance(a: str, b: str) -> bool:
    """True if midpoint would exceed length or conflict"""
    try:
        mid = midpoint(a, b)
        # Rebalance if midpoint is not valid or if we're getting close to MAX_LENGTH
        return not validate(mid) or len(mid) >= MAX_LENGTH - 6
    except:
        return True

def rebalance3(a: str, c: str) -> list[str]:
    """Return 3 evenly spaced values between a and c"""
    # For azpos, rebalance3 returns evenly spaced values between a and c, using 'h', 'p', 'x'
    if len(a) != len(c):
        raise AzposError("rebalance3: a and c must be same length")
    base = a
    return [base + 'h', base + 'p', base + 'x']

def compare(a: str, b: str) -> int:
    """Lexicographic sort logic (safe in all languages)"""
    if a < b:
        return -1
    if a > b:
        return 1
    return 0
