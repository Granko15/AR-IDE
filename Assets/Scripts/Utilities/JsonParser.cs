using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;

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
    private string jsonFileName = "diagram.json"; // Name of the JSON file
    private string jsonFilePath; // Full path to the JSON file

    public JsonData JsonData => jsonData;

    void Awake()
    {
        // Determine the file path based on the environment
        #if UNITY_EDITOR
            jsonFilePath = Application.dataPath + "/Resources/" + jsonFileName;
            Debug.Log("JSON file path (Editor): " + jsonFilePath);
        #else
            jsonFilePath = Path.Combine(Application.persistentDataPath, jsonFileName);
            Debug.Log("JSON file path (Build): " + jsonFilePath);
        #endif
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
                Debug.Log("Reloaded JSON from file: " + jsonFilePath);
            }
            else
            {
                Debug.LogWarning("JSON file not found at: " + jsonFilePath + ". You might need to create it first.");
                // Optionally, you could create a default empty JSON file here
                // File.WriteAllText(jsonFilePath, "{}");
                // LoadJsonFromFile(); // Try loading again after creating
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load JSON file: " + ex.Message);
        }
    }

    // Public method to save the current jsonData to the file
    public void SaveJsonToFile()
    {
        try
        {
            string jsonText = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonText);
            Debug.Log("JSON saved to: " + jsonFilePath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to save JSON file: " + ex.Message);
        }
    }

    public JsonData GetJsonData()
    {
        return jsonData;
    }
}