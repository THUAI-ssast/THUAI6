import json
import sys
import os

sys.path.append(os.path.join(os.path.dirname(__file__), "../"))
import contestant_code

if __name__ == "__main__":
    while True:
        observation = json.loads(input())
        if observation["isGameOver"]:
            break

        game_state = observation["map"]["model"]
        player_id = observation["playerId"]
        action = contestant_code.agent(game_state, player_id)

        action["frameCount"] = observation["frameCount"]
        print(json.dumps(action), flush=True)
