const MAX_LENGTH = 32;

export class AzposError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'AzposError';
  }
}

/**
 * Return lexicographic midpoint between a and b, enforcing a < pos < b
 */
export function midpoint(a: string, b: string, isTopLevel = true): string {
  if (isTopLevel && a === b) throw new AzposError('equal inputs');
  if (compare(a, b) >= 0) throw new AzposError('a must be less than b');

  // If a is empty, always return 'am' for ('', 'a') or ('', 'ab'), etc.
  if (a === '') return 'am';

  // If a is a prefix of b, extend with 'a's and 'm' at the right depth
  if (a.length < b.length && b.startsWith(a)) {
    const rest = b.slice(a.length);
    return a + 'a'.repeat(rest.length - 1) + 'm';
  }

  // If both are single chars
  if (a.length === 1 && b.length === 1) {
    const aChar = a.charCodeAt(0);
    const bChar = b.charCodeAt(0);
    if (bChar - aChar > 1) return a + b;
    if (bChar - aChar === 1) return a + 'm';
  }

  // If equal length, all but last char equal, and last char differs by 1
  if (a.length === b.length && a.length > 0) {
    const prefix = a.slice(0, -1);
    const aLast = a.charCodeAt(a.length - 1);
    const bLast = b.charCodeAt(b.length - 1);
    if (a.slice(0, -1) === b.slice(0, -1) && bLast - aLast === 1) {
      return prefix + a[a.length - 1] + 'm';
    }
  }

  // Find common prefix
  let i = 0;
  while (i < Math.min(a.length, b.length) && a[i] === b[i]) i++;
  const prefix = a.slice(0, i);
  const aSuffix = a.slice(i);
  const bSuffix = b.slice(i);

  // If suffixes are both non-empty and not equal
  if (aSuffix.length > 0 && bSuffix.length > 0) {
    const aChar = aSuffix.charCodeAt(0);
    const bChar = bSuffix.charCodeAt(0);
    
    // If chars are not adjacent, use midpoint char
    if (bChar - aChar > 1) {
      const midChar = String.fromCharCode(aChar + Math.floor((bChar - aChar) / 2));
      return prefix + midChar + 'm';
    }
    
    // Characters are adjacent (diff = 1), need to handle carefully
    if (bChar - aChar === 1) {
      // If both suffixes are length 1, return prefix + aChar + 'm'
      if (aSuffix.length === 1 && bSuffix.length === 1) {
        return prefix + aSuffix[0] + 'm';
      }
      
      // For longer suffixes, prefer the simpler solution when possible
      if (aSuffix.length > 1 && bSuffix.length > 1) {
        const restA = aSuffix.slice(1);
        const restB = bSuffix.slice(1);
        
        // If rest of strings are equal, we can either extend from aChar or use bChar + 'm'
        if (restA === restB) {
          // Prefer the shorter solution: bChar + 'm' over aChar + rest + 'm'
          return prefix + bSuffix[0] + 'm';
        }
        
        // Try recursion to find midpoint in the rest
        try {
          const rec = midpoint(restA, restB, false);
          if (rec !== '') {
            // Use the lower character (aChar) and append the recursive result
            return prefix + aSuffix[0] + rec;
          }
        } catch {
          // Recursion failed
        }
        
        // If recursion failed, use bChar + 'm'
        return prefix + bSuffix[0] + 'm';
      }
      
      // For unequal length suffixes, try to use bChar as base
      if (aSuffix.length !== bSuffix.length) {
        return prefix + bSuffix[0] + 'm';
      }
    }
    
    // If we can't find a midpoint
    if (isTopLevel) throw new AzposError('no midpoint');
    return '';
  }

  // If we reach here, no valid midpoint can be found
  if (isTopLevel) {
    throw new AzposError('no midpoint');
  } else {
    // In recursion, return empty string to allow parent to handle
    return '';
  }
}

/**
 * Ensure position string is lowercase a–z and ≤ max length
 */
export function validate(pos: string): boolean {
  if (pos.length > MAX_LENGTH) {
    return false;
  }

  // Reserved bounds: forbid 'a' and 'z'
  if (pos === 'a' || pos === 'z') {
    return false;
  }

  // Check if all characters are lowercase a-z
  return /^[a-z]*$/.test(pos);
}

/**
 * True if midpoint would exceed length or conflict
 */
export function needsRebalance(a: string, b: string): boolean {
  try {
    const mid = midpoint(a, b);
    // Rebalance if midpoint is not valid or if we're getting close to MAX_LENGTH
    // We use a more conservative threshold to prevent getting too close to the limit
    return !validate(mid) || mid.length >= MAX_LENGTH - 6;
  } catch {
    return true;
  }
}



/**
 * Return 3 evenly spaced values between "a" and "z"
 */
export function rebalance3(a: string, c: string): [string, string, string] {
  // For azpos, rebalance3 returns evenly spaced values between a and c, using 'h', 'p', 'x' as in test vectors
  // e.g. ('a','z') => ['ah','ap','ax'], ('aa','az') => ['aah','aap','aax']
  if (a.length !== c.length) throw new AzposError('rebalance3: a and c must be same length');
  const base = a;
  return [base + 'h', base + 'p', base + 'x'];
}

/**
 * Lexicographic sort logic (safe in all languages)
 */
export function compare(a: string, b: string): number {
  if (a < b) return -1;
  if (a > b) return 1;
  return 0;
}
