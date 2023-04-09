import json
import sys
import os

if __name__ == "__main__":
    # Import contestant code
    sys.path.append(os.path.join(os.path.dirname(__file__), "../"))
    import contestant_code

    start_observation = json.loads(input())
    contestant_code.init(start_observation)
    while True:
        observation = json.loads(input())

        action = contestant_code.get_action(observation)
        action["frame"] = observation["frame"]
        print(json.dumps(action), flush=True)
