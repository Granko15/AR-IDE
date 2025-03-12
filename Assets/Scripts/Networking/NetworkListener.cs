using System;
using UnityEngine;
using WebSocketSharp;

public class NetworkListener : MonoBehaviour
{
    private WebSocket ws;

    private void Start()
    {
        // WebSocket server address to connect to (Python server)
        string url = "ws://127.0.0.1:7777";

        // Create the WebSocket client
        ws = new WebSocket(url);

        // Define WebSocket event handlers
        ws.OnOpen += (sender, e) => 
        {
            Debug.Log("Connected to WebSocket server");
            ws.Send("Hello from Unity!");
        };

        ws.OnMessage += (sender, e) => 
        {
            Debug.Log("Received message: " + e.Data);
            // Handle messages from the Python server here
            // For example, you can parse the response or trigger actions based on it
        };

        ws.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket error: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket connection closed");
        };

        // Connect to the WebSocket server
        ws.Connect();
    }

    // Explicitly use 'new' keyword to hide inherited method
    new private void SendMessage(string message)
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Send(message);
            Debug.Log($"Sent message: {message}");
        }
    }

    private void OnApplicationQuit()
    {
        if (ws != null)
        {
            ws.Close();
            Debug.Log("WebSocket connection closed.");
        }
    }
}
