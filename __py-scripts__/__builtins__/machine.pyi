from typing import Coroutine

def move_forward() -> Coroutine: """Move forward"""
def rotate_counterclockwise() -> Coroutine: """Rotate counterclockwise"""
def rotate_clockwise() -> Coroutine: """Rotate clockwise"""
def log(message: any) -> None: """Write log"""
def move_to(x: int, y: int) -> Coroutine: """Move to specific position"""

class Field:
  x: float
  y: float

def get_field() -> Field: """Get field"""