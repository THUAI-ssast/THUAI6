import socket
import json

HOST = "127.0.0.1"  # The server's hostname or IP address
PORT = 11111  # The port used by the server

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))

def agent(posX, posY, msg):
    print("Agent: ", posX, posY, msg)
    return {"action": "move", "msg": "up"}

while True:
    data = json.loads(s.recv(1024))
    if data["msg"] == "close":
        break

    response = agent(data["posX"], data["posY"], data["msg"])
    response["frameNo"] = data["frameNo"]
    s.sendall(json.dumps(response).encode("utf-8"))


s.close()