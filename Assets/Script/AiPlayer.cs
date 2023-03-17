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
// TODO: to be implemented

[Serializable]
public class SendDataType {
    public int field1;
    public string field2;
}

[Serializable]
public class ReceiveDataType {
    public int action;
    public string msg;
}

public class AiPlayer : MonoBehaviour
{
    // TODO: to be decided
    // original reaction time
    public float reactionTime = 0.2f; // seconds
    public const int portNumber = 11111;
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

    // Update is called once per frame
    async void Update()
    {
        if (clientSocket == null || !clientSocket.Connected)
            return;

        int recv = await clientSocket.ReceiveAsync(recvBuffer, SocketFlags.None);
        if (recv == 0)
            return;

        string recvStr = Encoding.ASCII.GetString(recvBuffer, 0, recv);
        ReceiveDataType recvData = JsonUtility.FromJson<ReceiveDataType>(recvStr);

        Debug.Log("Received: " + recvStr);
        Debug.Log("Action: " + recvData.action);
        Debug.Log("Msg: " + recvData.msg);

        SendDataType sendData = new SendDataType();
        sendData.field1 = 1;
        sendData.field2 = "Hello";
        string sendStr = JsonUtility.ToJson(sendData);
        byte[] sendBuffer = Encoding.ASCII.GetBytes(sendStr);
        await clientSocket.SendAsync(sendBuffer, SocketFlags.None);
    }
}
