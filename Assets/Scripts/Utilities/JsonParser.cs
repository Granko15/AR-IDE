using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

[System.Serializable]
public class Method
{
    public string name;
    public int line_number;
}

[System.Serializable]
public class ClassData
{
    public string file_path;
    public string package;
    public int line_number;
    public Method[] methods;
    public string[] attributes;
    public string[] base_classes;
    public string[] composition;
    public string[] uses;
    public string[] thread_id;
}

[System.Serializable]
public class JsonData
{
    public Dictionary<string, ClassData> classes;
    public string[] functions;
    public string[] imports;
}

public class JsonParser : MonoBehaviour
{
    private JsonData jsonData; // Holds parsed JSON data
    private string jsonFilePath = "Assets/Resources/diagram.json"; // Path to JSON file

    public JsonData JsonData => jsonData;

    void Awake()
    {
        // Automatically load the JSON from the file path
        LoadJsonFromFile();
    }

    void ParseJson(string json)
    {
        try
        {
            jsonData = JsonConvert.DeserializeObject<JsonData>(json);
            Debug.Log("JSON parsed successfully!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse JSON: {ex.Message}");
        }
    }

    public void LoadJsonFromFile()
    {
        try
        {
            if (File.Exists(jsonFilePath))
            {
                string jsonText = File.ReadAllText(jsonFilePath);
                ParseJson(jsonText);
                Debug.Log("Reloaded JSON from file.");
            }
            else
            {
                Debug.LogWarning("JSON file not found: " + jsonFilePath);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load JSON file: " + ex.Message);
        }
    }

    public JsonData GetJsonData()
    {
        return jsonData;
    }
}
