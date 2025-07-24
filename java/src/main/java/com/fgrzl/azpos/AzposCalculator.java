package com.fgrzl.azpos;

import java.util.regex.Pattern;

public class AzposCalculator {
    
    private static final int MAX_LENGTH = 25;
    private static final Pattern VALID_PATTERN = Pattern.compile("^[a-z]*$");
    
    /**
     * Return lexicographic midpoint between a and b, enforcing a < pos < b
     */
    public static String midpoint(String a, String b) throws AzposException {
        if (a.equals(b)) {
            throw new AzposException("equal inputs");
        }
        
        if (compare(a, b) >= 0) {
            throw new AzposException("a must be less than b");
        }
        
        // Handle empty string case
        if (a.isEmpty()) {
            if (b.equals("a")) {
                return "am";
            }
            
            if (b.length() > 0) {
                String mid = midpoint("", b.substring(0, b.length() - 1));
                return mid + "m";
            }
        }
        
        // Find common prefix
        int i = 0;
        int minLen = Math.min(a.length(), b.length());
        while (i < minLen && a.charAt(i) == b.charAt(i)) {
            i++;
        }
        
        String prefix = a.substring(0, i);
        String aSuffix = a.substring(i);
        String bSuffix = b.substring(i);
        
        // If one string is a prefix of another
        if (aSuffix.isEmpty()) {
            // a is prefix of b
            if (!bSuffix.isEmpty()) {
                char nextChar = bSuffix.charAt(0);
                if (nextChar == 'a') {
                    return a + "am";
                }
                
                char midChar = (char)('a' + (nextChar - 'a') / 2);
                return a + midChar + "m";
            }
        }
        
        if (bSuffix.isEmpty()) {
            throw new AzposException("invalid state");
        }
        
        char aChar = aSuffix.charAt(0);
        char bChar = bSuffix.charAt(0);
        int charDiff = bChar - aChar;
        
        if (charDiff > 1) {
            char midChar = (char)(aChar + charDiff / 2);
            String result = prefix + midChar + "m";
            
            // Ensure result is not equal to a or b
            if (result.equals(a) || result.equals(b)) {
                return prefix + midChar + "mm";
            }
            
            return result;
        }
        
        // Adjacent characters, need to go deeper
        String deeper = midpoint(aSuffix.substring(1), bSuffix.substring(1));
        return prefix + aChar + deeper;
    }
    
    /**
     * Ensure position string is lowercase a–z and ≤ max length
     */
    public static boolean validate(String pos) {
        if (pos.length() > MAX_LENGTH) {
            return false;
        }
        
        if (pos.equals("z")) {
            return false; // reserved position
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
            return mid.length() > MAX_LENGTH;
        } catch (AzposException e) {
            return true;
        }
    }
    
    /**
     * Return 3 evenly spaced values between a and c
     */
    public static String[] rebalance3(String a, String c) {
        if (a.isEmpty() || c.isEmpty()) {
            return new String[]{a + "h", a + "p", a + "x"};
        }
        
        char aChar = a.charAt(0);
        char cChar = c.charAt(0);
        int segment = (cChar - aChar) / 4;
        
        String b1 = aChar + String.valueOf((char)('a' + segment)) + "h";
        String b2 = aChar + String.valueOf((char)('a' + segment * 2)) + "p";
        String b3 = aChar + String.valueOf((char)('a' + segment * 3)) + "x";
        
        return new String[]{b1, b2, b3};
    }
    
    /**
     * Lexicographic sort logic (safe in all languages)
     */
    public static int compare(String a, String b) {
        return a.compareTo(b);
    }
}
