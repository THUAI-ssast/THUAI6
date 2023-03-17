import socket
import json

HOST = "127.0.0.1"  # The server's hostname or IP address
PORT = 11111  # The port used by the server

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))

while True:
    data = {"action": "move", "msg": "up"}
    s.sendall(json.dumps(data).encode("utf-8"))

    data = json.loads(s.recv(1024))
    print(f"Received.")
    print(f"field1: {data['field1']}")
    print(f"field2: {data['field2']}")
    if data["field1"] == "close":
        break


s.close()