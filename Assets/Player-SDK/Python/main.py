import json

def agent(observation):
    return {
        "type": "Shoot",
    }

if __name__ == "__main__":
    while True:
        observation = json.loads(input())
        if observation["isGameOver"]:
            break

        action = agent(observation)
        action["frameCount"] = observation["frameCount"]
        print(json.dumps(action), flush=True)
