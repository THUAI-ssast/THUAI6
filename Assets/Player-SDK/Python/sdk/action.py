import enum

class MoveDirection(enum.Enum):
    Forward = 0
    Backward = 1

def Move(direction: MoveDirection):
    return {
        "type": "Move",
        "direction": direction.value
    }

class RotateDirection(enum.Enum):
    Left = 0
    Right = 1

def Rotate(direction: RotateDirection):
    return {
        "type": "Rotate",
        "direction": direction.value
    }
