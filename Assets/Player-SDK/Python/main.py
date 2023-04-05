import json
import random

def agent(observation):
    if random.random() < 0.2:
        return {
            "type": "Rotate",
            "direction": 0,
        }
    else:
        return {
            "type": "Move",
            "direction": 0,
        }

if __name__ == "__main__":
    while True:
        observation = json.loads(input())
        if observation["isGameOver"]:
            break

        action = agent(observation)
        action["frameCount"] = observation["frameCount"]
        print(json.dumps(action), flush=True)