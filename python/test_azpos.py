"""
test_azpos.py - Tests for azpos.py
"""
import unittest
from azpos import midpoint, validate, needs_rebalance, rebalance3, compare, AzposError

class TestAzpos(unittest.TestCase):
    def test_midpoint(self):
        # TODO: add real tests
        self.assertRaises(NotImplementedError, midpoint, "a", "z")

    def test_validate(self):
        self.assertRaises(NotImplementedError, validate, "abc")

    def test_needs_rebalance(self):
        self.assertRaises(NotImplementedError, needs_rebalance, "a", "z")

    def test_rebalance3(self):
        self.assertRaises(NotImplementedError, rebalance3, "a", "z")

    def test_compare(self):
        self.assertRaises(NotImplementedError, compare, "a", "b")

if __name__ == "__main__":
    unittest.main()
