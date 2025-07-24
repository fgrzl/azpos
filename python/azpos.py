"""
azpos.py - Python implementation of lexicographic position calculations
"""

class AzposError(Exception):
    pass

def midpoint(a: str, b: str, is_top_level: bool = True) -> str:
    raise NotImplementedError("Not implemented")

def validate(pos: str) -> bool:
    raise NotImplementedError("Not implemented")

def needs_rebalance(a: str, b: str) -> bool:
    raise NotImplementedError("Not implemented")

def rebalance3(a: str, c: str):
    raise NotImplementedError("Not implemented")

def compare(a: str, b: str) -> int:
    raise NotImplementedError("Not implemented")
