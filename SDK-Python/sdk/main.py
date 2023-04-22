import json
import sys
import os

import threading
import time

class Consumer(threading.Thread):

    def __init__(self, *args, timeout=1, **kwargs):
        super().__init__(*args, **kwargs)
        self.lines = []
        self.last_length = 0
        self.timeout = timeout

    def run(self):
        for line in sys.stdin:
            self.lines.append(line)

    def get_init(self):
        while len(self.lines) == 0:
            pass
        return self.lines[0]

    def get_observation(self):
        begin_time = time.time()
        while len(self.lines) == self.last_length:
            if time.time() - begin_time > self.timeout:
                sys.exit(0) # when not receiving from main game
        self.last_length = len(self.lines)
        return self.lines[-1]

consumer = Consumer()
consumer.daemon = True
consumer.start()

if __name__ == "__main__":
    # Import contestant code
    sys.path.append(os.path.join(os.path.dirname(__file__), "../"))
    import contestant_code

    start_observation = json.loads(consumer.get_init())
    contestant_code.init(start_observation)

    while True:
        observation = json.loads(consumer.get_observation())
        action = contestant_code.get_action(observation)
        action["frame"] = observation["frame"]
        print(json.dumps(action), flush=True)
