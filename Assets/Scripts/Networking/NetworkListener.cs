using System;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System.Globalization;
using UnityEngine.XR;

public class NetworkListener : MonoBehaviour
{
    private WebSocket ws;
    public CodeboxManager codeboxManager; // Assign in Unity Editor
    public UnityMainThreadDispatcher dispatcher; // Assign in Unity Editor
    public RelationshipManager relationshipManager; // Assign in Unity Editor
    public JsonParser jsonParser; // Assign in Unity Editor

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
            string content = json["content"]?.ToString();

            if (command == "display_code_box" && !string.IsNullOrEmpty(file))
            {
                dispatcher.Enqueue(() => DisplayCodeboxOnMainThread(className));
            }
            else if (command == "hide_code_box")
            {
                dispatcher.Enqueue(() => HideCodeboxOnMainThread(className));
            }
            else if (command == "send_diagram" && !string.IsNullOrEmpty(content))
            {
                HandleSendDiagram(content);
            }
            else if(command == "OpenAIResponse"){
                HandleOpenAIResponse(json);
            }
            else if (command == "LoadAIHistory"){
                HandleLoadAIHistory(json);
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

    private void HandleSendDiagram(string content)
    {
        try
        {
            // Save the JSON content to a file
            SaveDiagramToFile(content);

            // Reload the JSON data from the file
            jsonParser.LoadJsonFromFile(); // Reload JSON from file

            // Update codeboxes and relationships
            dispatcher.Enqueue(() =>
            {
                codeboxManager.UpdateCodeboxes();
            });
            Debug.Log("Diagram updated.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to handle send_diagram: " + ex.Message);
        }
    }

    private void HandleLoadAIHistory(JObject json)
    {
        if (json.TryGetValue("data", out JToken dataToken))
        {
            JObject wrapper = new JObject();
            wrapper["classes"] = dataToken; // Assign the "data" array to the "classes" key
            string path = Application.dataPath + "/Resources/AIChatHistory.json";
            System.IO.File.WriteAllText(path, wrapper.ToString());
            Debug.Log("AI history saved to: " + path);
        }
        else
        {
            Debug.LogError("Data field not found in JSON.");
        }
    }

    private void SaveDiagramToFile(string content)
    {
        string path = Application.dataPath + "/Resources/diagram.json";
        System.IO.File.WriteAllText(path, content);
    }

    public void SendJumpToClass(string file, int line)
    {
        if (ws != null && ws.IsAlive)
        {
            JObject json = new JObject();
            json["command"] = "JumpToClass";
            json["file"] = file;
            json["line"] = line;

            ws.Send(json.ToString());
            Debug.Log("Sent JumpToClass command: " + json.ToString());
        }
        else
        {
            Debug.LogError("WebSocket connection is not open.");
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
    
    public void SwitchToThisCodeboxInAIAssistant(string className, string filePath)
    {
        if (ws != null && ws.IsAlive)
        {
            JObject json = new JObject();
            json["command"] = "SwitchToThisCodeboxInAIAssistant";
            json["className"] = className;
            json["filePath"] = filePath;

            ws.Send(json.ToString());
            Debug.Log("Sent OpenAIRequest command: " + json.ToString());
        }
        else
        {
            Debug.LogError("WebSocket connection is not open.");
        }
    }
    private void HandleOpenAIResponse(JObject json)
    {   
        string prompt = json["prompt"]?.ToString();
        string response = json["response"]?.ToString();
        Debug.Log("For prompt: " + prompt);
        Debug.Log("Response: " + response);
    }
}
