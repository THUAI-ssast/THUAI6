from enum import Enum


class ForwardOrBackWard(Enum):
    Forward = 0
    Backward = 1


class LeftOrRight(Enum):
    Left = 0
    Right = 1


def Move(direction: ForwardOrBackWard):
    return {"type": "Move", "direction": direction.value}


def Rotate(direction: LeftOrRight):
    return {"type": "Rotate", "direction": direction.value}


def Shoot():
    return {"type": "Shoot"}


def ChangeBullet():
    return {"type": "ChangeBullet"}


def PlaceBomb(x: int, y: int):
    return {"type": "PlaceBomb", "position": [x, y]}


# TODO: actions left


def Idle():
    return {"type": "Idle"}
