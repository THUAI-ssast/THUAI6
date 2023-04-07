import sdk.action as Action
import random

def agent(game_state, player_id):
    map_info = game_state["map"]
    players = game_state["players"]
    bombs = game_state["bombs"]

    if random.random() < 0.8:
        return Action.Move(Action.MoveDirection.Forward)
    else:
        return Action.Rotate(Action.RotateDirection.Left)
