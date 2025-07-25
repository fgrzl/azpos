"""
test_azpos.py - Tests for azpos.py
"""
import unittest
from azpos import midpoint, validate, needs_rebalance, rebalance3, compare, AzposError

class TestAzpos(unittest.TestCase):
    def test_midpoint_basic(self):
        self.assertEqual(midpoint("", "a"), "am")
        self.assertEqual(midpoint("a", "c"), "ac")
        
    def test_midpoint_equal_inputs(self):
        with self.assertRaises(AzposError):
            midpoint("a", "a")
    
    def test_midpoint_invalid_order(self):
        with self.assertRaises(AzposError):
            midpoint("b", "a")

    def test_validate_basic(self):
        self.assertTrue(validate("abc"))
        self.assertTrue(validate(""))
        self.assertFalse(validate("a"))  # reserved
        self.assertFalse(validate("z"))  # reserved
        self.assertFalse(validate("ABC"))  # uppercase

    def test_needs_rebalance_basic(self):
        # This should not need rebalance for simple cases
        result = needs_rebalance("ab", "ac")
        self.assertIsInstance(result, bool)

    def test_rebalance3_basic(self):
        result = rebalance3("a", "a")
        self.assertEqual(result, ["ah", "ap", "ax"])
        
    def test_rebalance3_different_lengths(self):
        with self.assertRaises(AzposError):
            rebalance3("a", "ab")

    def test_compare_basic(self):
        self.assertEqual(compare("a", "b"), -1)
        self.assertEqual(compare("b", "a"), 1)
        self.assertEqual(compare("a", "a"), 0)

if __name__ == "__main__":
    unittest.main()
