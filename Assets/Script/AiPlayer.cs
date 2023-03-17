using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using System.IO;

// call AI to decide next action

[Serializable]
public class SendDataType {
    public int frameNo;
    public int posX;
    public int posY;
    public string msg;
}

[Serializable]
public class ReceiveDataType {
    public int frameNo;
    public int action;
    public string msg;
}

public class AiPlayer : MonoBehaviour
{
    // TODO: reaction time
    public float reactionTime = 0.2f; // seconds
    public const int portNumber = 11111;
    public Func<ReceiveDataType, bool> onReceive = null;
    private Socket clientSocket = null;
    private byte[] recvBuffer = new byte[1024];


    // Start is called before the first frame update
    async void Start()
    {
        // Create a local socket server
        IPHostEntry ipHost = Dns.GetHostEntry("127.0.0.1");
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, portNumber);

        Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            Debug.Log("Waiting connection ... ");
            clientSocket = await listener.AcceptAsync();
            Debug.Log("Connected");
        } catch (Exception e) {
            Debug.Log(e.ToString());
        }
    }

    // Should be called once at the beginning of each frame
    public void SendState(SendDataType data) {
        data.frameNo = Time.frameCount;
        string sendStr = JsonUtility.ToJson(data);
        byte[] sendBuffer = Encoding.ASCII.GetBytes(sendStr);
        clientSocket.SendAsync(sendBuffer, SocketFlags.None);
    }

    // Update is called once per frame
    async void Update()
    {
        if (clientSocket == null || !clientSocket.Connected)
            return;

        // FOR DEBUG
        SendDataType sendData = new SendDataType();
        sendData.posX = 1;
        sendData.posY = 2;
        sendData.msg = "Hello";
        SendState(sendData);

        // Peak receive buffer for agent response of last frame
        if (clientSocket.Available == 0)
            return;

        try {
            int recv = await clientSocket.ReceiveAsync(recvBuffer, SocketFlags.None);
            Debug.Assert(recv > 0);

            string recvStr = Encoding.ASCII.GetString(recvBuffer, 0, recv);
            try {
                ReceiveDataType recvData = JsonUtility.FromJson<ReceiveDataType>(recvStr);
                if (recvData.frameNo != Time.frameCount - 1) {
                    Debug.Log("FrameNo = " + Time.frameCount);
                    Debug.Log("RecvFrameNo = " + recvData.frameNo);
                    Debug.Log("Response received in wrong frame");
                    return;
                }
                if (onReceive != null)
                    onReceive(recvData);
            } catch {
                Debug.Log("FrameNo = " + Time.frameCount);
                Debug.Log("Multiple response received in one frame");
                return;
            }
        } catch (Exception e) {
            Debug.Log(e.ToString());
        }
    }
}
