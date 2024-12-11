using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkListener : MonoBehaviour
{
    private Socket server;
    private IPAddress localAddr;
    private int port;

    private void Start()
    {
        localAddr = IPAddress.Parse("127.0.0.1");
        port = 7777;
        IPEndPoint localEndPoint = new IPEndPoint(localAddr, port);

        server = new Socket(localAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            server.Bind(localEndPoint);
            server.Listen(10);

            Debug.Log($"Server started on {localAddr}:{port}");

            server.BeginAccept(new AsyncCallback(AcceptCallback), server);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void AcceptCallback(IAsyncResult ar)
    {
        Socket listener = (Socket) ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        Debug.Log("Connected!");

        StateObject state = new StateObject();
        state.workSocket = handler;
        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
    }

    private void ReadCallback(IAsyncResult ar)
    {
        StateObject state = (StateObject) ar.AsyncState;
        Socket handler = state.workSocket;

        int bytesRead = handler.EndReceive(ar);

        if (bytesRead > 0)
        {
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }
        else
        {
            if (state.sb.Length > 1)
            {
                string content = state.sb.ToString();
                Debug.Log($"Read {content.Length} bytes from socket.\n Data : {content}");
            }
            handler.Close();
        }
    }

    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }

    private void OnApplicationQuit()
    {
        if (server != null){
            server.Close();
        }
    }
}