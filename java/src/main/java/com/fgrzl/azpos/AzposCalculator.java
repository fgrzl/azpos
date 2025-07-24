package com.fgrzl.azpos;

public class AzposCalculator {
    public static String midpoint(String a, String b) throws AzposException {
        throw new UnsupportedOperationException("Not implemented");
    }

    public static void validate(String pos) throws AzposException {
        throw new UnsupportedOperationException("Not implemented");
    }

    public static int compare(String a, String b) {
        throw new UnsupportedOperationException("Not implemented");
    }

    public static String[] rebalance3(String a, String c) {
        throw new UnsupportedOperationException("Not implemented");
    }
}
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
