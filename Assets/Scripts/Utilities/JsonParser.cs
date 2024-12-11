using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

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
    public TextAsset jsonFile; // Drag and drop your JSON file here
    private JsonData jsonData; // Parsed JSON data

    // Public getter for JsonData
    public JsonData JsonData => jsonData;

    void Awake()
    {
        if (jsonFile != null)
        {
            ParseJson(jsonFile.text);
        }
        else
        {
            Debug.LogError("JSON file not assigned in the Inspector.");
        }
    }

    void ParseJson(string json)
    {
        try
        {
            // Parse JSON into the JsonData structure
            jsonData = JsonConvert.DeserializeObject<JsonData>(json);

            Debug.Log("JSON parsed successfully!");
            Debug.Log($"Classes Count: {jsonData.classes.Count}");
            foreach (var classEntry in jsonData.classes)
            {
                Debug.Log($"Class Name: {classEntry.Key}");
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse JSON: {ex.Message}");
        }
    }

    public JsonData GetJsonData()
    {
        return jsonData;
    }

}
