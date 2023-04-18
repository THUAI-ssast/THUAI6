import random

import sdk.action as action
import sdk.utils as utils

import time


def init(observation):
    global map_info
    global my_id
    global my_team
    map_info = observation["map"]
    my_id = observation["myId"]
    my_team = observation["myTeam"]


def get_action(observation):
    players = observation["players"]
    bombs = observation["bombs"]
    portals = observation["portalsClassifiedByPattern"]

    time.sleep(1 / 50)

    my_player = players[my_id]
    p = random.random()
    if p < 0.05:
        return action.Idle()
    elif p < 0.25:
        return action.Rotate(action.LeftOrRight.Left)
    elif p < 0.35:
        return action.Rotate(action.LeftOrRight.Right)
    elif p < 0.60:
        return action.Shoot() if my_player["state"]["canShoot"] else action.ChangeBullet()
    else:
        return action.Move(action.ForwardOrBackWard.Forward)
