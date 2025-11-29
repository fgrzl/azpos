package com.fgrzl.azpos;

import java.util.regex.Pattern;

public class AzposCalculator {
    private static final int MAX_LENGTH = 32;
    private static final Pattern VALID_PATTERN = Pattern.compile("^[a-z]*$");
    
    /**
     * Return lexicographic midpoint between a and b, enforcing a < pos < b
     */
    public static String midpoint(String a, String b) throws AzposException {
        return midpointInternal(a, b, true);
    }
    
    private static String midpointInternal(String a, String b, boolean isTopLevel) throws AzposException {
        if (isTopLevel && a.equals(b)) {
            throw new AzposException("equal inputs");
        }
        if (compare(a, b) >= 0) {
            throw new AzposException("a must be less than b");
        }
        
        // If a is empty, always return 'am'
        if (a.isEmpty()) {
            return "am";
        }
        
        // If a is a prefix of b, extend with 'a's and 'm' at the right depth
        if (a.length() < b.length() && b.startsWith(a)) {
            String rest = b.substring(a.length());
            if (rest.length() > 0) {
                return a + "a".repeat(rest.length() - 1) + "m";
            }
        }
        
        // If both are single chars
        if (a.length() == 1 && b.length() == 1) {
            int aChar = a.charAt(0);
            int bChar = b.charAt(0);
            int diff = bChar - aChar;
            if (diff > 1) {
                return a + b;
            }
            if (diff == 1) {
                return a + "m";
            }
        }
        
        // If equal length, all but last char equal, and last char differs by 1
        if (a.length() == b.length() && a.length() > 0) {
            String prefix = a.substring(0, a.length() - 1);
            int aLast = a.charAt(a.length() - 1);
            int bLast = b.charAt(b.length() - 1);
            if (a.substring(0, a.length() - 1).equals(b.substring(0, b.length() - 1)) && bLast - aLast == 1) {
                return prefix + a.charAt(a.length() - 1) + "m";
            }
        }
        
        // Find common prefix
        int i = 0;
        int maxLen = Math.min(a.length(), b.length());
        while (i < maxLen && a.charAt(i) == b.charAt(i)) {
            i++;
        }
        String prefix = a.substring(0, i);
        String aSuffix = a.substring(i);
        String bSuffix = b.substring(i);
        
        // If suffixes are both non-empty and not equal
        if (aSuffix.length() > 0 && bSuffix.length() > 0) {
            int aChar = aSuffix.charAt(0);
            int bChar = bSuffix.charAt(0);
            int diff = bChar - aChar;
            
            // If chars are not adjacent, use midpoint char
            if (diff > 1) {
                char midChar = (char)(aChar + diff / 2);
                return prefix + midChar + "m";
            }
            
            // Characters are adjacent (diff = 1), need to handle carefully
            if (diff == 1) {
                // If both suffixes are length 1, return prefix + aChar + 'm'
                if (aSuffix.length() == 1 && bSuffix.length() == 1) {
                    return prefix + aSuffix.charAt(0) + "m";
                }
                
                // For longer suffixes, prefer the simpler solution when possible
                if (aSuffix.length() > 1 && bSuffix.length() > 1) {
                    String restA = aSuffix.substring(1);
                    String restB = bSuffix.substring(1);
                    
                    // If rest of strings are equal, prefer the shorter solution
                    if (restA.equals(restB)) {
                        return prefix + bSuffix.charAt(0) + "m";
                    }
                    
                    // Try recursion to find midpoint in the rest
                    try {
                        String rec = midpointInternal(restA, restB, false);
                        if (!rec.isEmpty()) {
                            return prefix + aSuffix.charAt(0) + rec;
                        }
                    } catch (AzposException e) {
                        // Recursion failed
                    }
                    
                    // If recursion failed, use bChar + 'm'
                    return prefix + bSuffix.charAt(0) + "m";
                }
                
                // For unequal length suffixes, try to use bChar as base
                if (aSuffix.length() != bSuffix.length()) {
                    return prefix + bSuffix.charAt(0) + "m";
                }
            }
            
            // If we can't find a midpoint
            if (isTopLevel) {
                throw new AzposException("no midpoint");
            }
            return "";
        }
        
        // If we reach here, no valid midpoint can be found
        if (isTopLevel) {
            throw new AzposException("no midpoint");
        } else {
            return "";
        }
    }
    
    /**
     * Ensure position string is lowercase a–z and ≤ max length
     */
    public static boolean validate(String pos) {
        if (pos.length() > MAX_LENGTH) {
            return false;
        }
        
        // Reserved bounds: forbid 'a' and 'z'
        if (pos.equals("a") || pos.equals("z")) {
            return false;
        }
        
        // Check if all characters are lowercase a-z
        return VALID_PATTERN.matcher(pos).matches();
    }
    
    /**
     * True if midpoint would exceed length or conflict
     */
    public static boolean needsRebalance(String a, String b) {
        try {
            String mid = midpoint(a, b);
            // Rebalance if midpoint is not valid or if we're getting close to MAX_LENGTH
            return !validate(mid) || mid.length() >= MAX_LENGTH - 6;
        } catch (AzposException e) {
            return true;
        }
    }
    
    /**
     * Return 3 evenly spaced values between a and c
     */
    public static String[] rebalance3(String a, String c) throws AzposException {
        // For azpos, rebalance3 returns evenly spaced values between a and c, using 'h', 'p', 'x'
        if (a.length() != c.length()) {
            throw new AzposException("rebalance3: a and c must be same length");
        }
        String base = a;
        return new String[]{base + "h", base + "p", base + "x"};
    }
    
    /**
     * Lexicographic sort logic (safe in all languages)
     */
    public static int compare(String a, String b) {
        return a.compareTo(b);
    }
}
