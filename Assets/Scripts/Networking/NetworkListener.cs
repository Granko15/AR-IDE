using System;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System.Globalization;

public class NetworkListener : MonoBehaviour
{
    private WebSocket ws;
    public CodeboxManager codeboxManager; // Assign in Unity Editor
    public UnityMainThreadDispatcher dispatcher; // Assign in Unity Editor

    private void Start()
    {
        string url = "ws://127.0.0.1:7777";
        ws = new WebSocket(url);

        ws.OnOpen += (sender, e) => Debug.Log("Connected to WebSocket server");
        ws.OnMessage += (sender, e) => HandleMessage(e.Data);
        ws.OnError += (sender, e) => Debug.LogError("WebSocket error: " + e.Message);
        ws.OnClose += (sender, e) => Debug.Log("WebSocket connection closed");

        ws.Connect();
    }

    private void HandleMessage(string message)
    {
        try
        {
            var json = JObject.Parse(message);
            string command = json["command"]?.ToString();
            string className = json["className"]?.ToString();
            string file = json["file"]?.ToString();

            if (command == "display_code_box" && !string.IsNullOrEmpty(file))
            {
                dispatcher.Enqueue(() => DisplayCodeboxOnMainThread(className));
            }
            else if (command == "hide_code_box")
            {
                dispatcher.Enqueue(() => HideCodeboxOnMainThread(className));
            }
            else
            {
                Debug.Log("Unknown command: " + json.ToString());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to handle message: " + ex.Message);
        }
    }

    private void DisplayCodeboxOnMainThread(string className)
    {
        codeboxManager.DisplayCodebox(className);
    }
    private void HideCodeboxOnMainThread(string className)
    {
        codeboxManager.HideCodebox(className);
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
