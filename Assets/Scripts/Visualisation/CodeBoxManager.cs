using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CodeboxManager : MonoBehaviour
{
    public static CodeboxManager Instance { get; private set; } // Singleton instance

    public GameObject codeboxPrefab;
    public JsonParser jsonParser;
    public RelationshipManager relationshipManager;
    public NetworkListener networkListener; // Assign in Unity Editor
    private float currentXPos = -12f;
    private float currentYPos = 2.5f;
    private float currentZPos = 50f;

    public float spawnDistance = 50f;

    private Dictionary<string, GameObject> codeboxInstances = new Dictionary<string, GameObject>();
    private GameObject highlightedCodebox = null; // Store the highlighted codebox instance
    private bool instantiated = false; // Flag to check if the codebox is instantiated

    // Pridaj verejnú udalosť, ktorá sa zavolá po vytvorení Codeboxov
    public delegate void CodeboxesCreatedDelegate(Dictionary<string, GameObject> codeboxes);
    public static event CodeboxesCreatedDelegate OnCodeboxesCreated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (jsonParser != null)
        {
            JsonData projectData = jsonParser.GetJsonData();
            if (projectData != null)
            {
                InstantiateCodeboxes(projectData);
                // Po vytvorení Codeboxov zavolaj udalosť
                OnCodeboxesCreated?.Invoke(codeboxInstances);
            }
            else
            {
                Debug.LogError("JSON projectData is null.");
            }
        }
        else
        {
            Debug.LogError("JsonParser is not assigned.");
        }
    }

    public void InstantiateCodeboxes(JsonData projectData)
    {
        foreach (var classEntry in projectData.classes)
        {
            string className = classEntry.Key;
            ClassData classData = classEntry.Value;

            Vector3 position = new Vector3(currentXPos, currentYPos, currentZPos);
            GameObject codebox = Instantiate(codeboxPrefab, position, Quaternion.identity);

            codeboxInstances[className] = codebox;

            HideCodebox(className);

            CodeboxPopulator populator = codebox.GetComponent<CodeboxPopulator>();
            if (populator != null)
            {
                populator.PopulateClassData(classData, className);
            }

            // Set the CodeboxName in CodeBoxController
            CodeBoxController controller = codebox.GetComponent<CodeBoxController>();
            if (controller != null)
            {
                controller.CodeboxName = className;
                controller.FilePath = classData.file_path; // Assuming ClassData has a file_path property
                controller.LineNumber = classData.line_number.ToString(); // Assuming ClassData has a line_number property
            }
        }
        relationshipManager.SetupRelationships(projectData, codeboxInstances);
        LoadAndAssignChatHistory(); // Load and assign chat history after codeboxes are instantiated
    }
    
    private void LoadAndAssignChatHistory()
    {
        string filePath = Application.dataPath + "/Resources/AIChatHistory.json";
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            ChatHistoryWrapper chatHistoryWrapper = JsonUtility.FromJson<ChatHistoryWrapper>(jsonString);

            if (chatHistoryWrapper == null || chatHistoryWrapper.classes == null)
            {
                Debug.LogError("Failed to parse AIChatHistory.json.");
                return;
            }

            foreach (var classData in chatHistoryWrapper.classes)
            {
                foreach (var codebox in codeboxInstances.Values)
                {
                    CodeBoxController controller = codebox.GetComponent<CodeBoxController>();
                    if (controller != null && controller.FilePath == classData.filePath)
                    {
                        CodeboxPopulator populator = codebox.GetComponent<CodeboxPopulator>();
                        if (populator != null && populator.AIAssistantText != null)
                        {
                            populator.AIAssistantText.text = FormatChatHistoryReversed(classData.chatHistory);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("AIChatHistory.json not found at path: " + filePath);
        }
    }

    private string FormatChatHistoryReversed(ChatMessage[] chatHistory)
    {
        if (chatHistory == null || chatHistory.Length == 0)
            return "No chat history available.";

        string formattedHistory = "";
        for (int i = chatHistory.Length - 1; i >= 0; i--)
        {
            formattedHistory += $"{chatHistory[i].role}: {chatHistory[i].text}\n";
        }
        return formattedHistory;
    }

    public void DisplayCodebox(string className)
    {
        if (codeboxInstances.TryGetValue(className, out GameObject codebox))
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 forwardDirection = Camera.main.transform.forward;

            Vector3 newPosition = cameraPosition + forwardDirection * spawnDistance;

            Vector3 lookDirection = new Vector3(forwardDirection.x, 0, forwardDirection.z);
            Quaternion horizontalRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 90f, 0);

            codebox.transform.position = newPosition;
            codebox.transform.rotation = horizontalRotation;

            codebox.SetActive(true);

            if (relationshipManager != null)
            {
                relationshipManager.DisplayRelationships(className);
            }
        }
        else
        {
            Debug.LogError($"Codebox for class {className} not found.");
        }
    }

    public void HideCodebox(string className)
    {
        if (codeboxInstances.TryGetValue(className, out GameObject codebox))
        {
            codebox.SetActive(false);

            if (relationshipManager != null)
            {
                relationshipManager.HideRelationships(className);
            }
        }
        else
        {
            Debug.LogError($"Codebox for class {className} not found.");
        }
    }

    public void UpdateCodeboxes()
    {
        foreach (var codebox in codeboxInstances.Values)
        {
            Destroy(codebox);
        }
        codeboxInstances.Clear();

        JsonData projectData = jsonParser.GetJsonData();
        if (projectData != null)
        {
            InstantiateCodeboxes(projectData);
        }
    }

    public void SendSwitchCommandInAIAssistant(string className, string filePath)
    {
        if (networkListener != null)
        {
            networkListener.SwitchToThisCodeboxInAIAssistant(className, filePath);
        }
        else
        {
            Debug.LogError("NetworkListener is not assigned.");
        }
    }

    public Dictionary<string, GameObject> GetCodeboxInstances()
    {
        return codeboxInstances;
    }

    public void SetHighlightedCodebox(GameObject codebox)
    {
        highlightedCodebox = codebox;
    }

    public GameObject GetHighlightedCodebox()
    {
        return highlightedCodebox;
    }

    public void JumpToHighlightedCodeBox()
    {
        if (highlightedCodebox != null)
        {
            Debug.Log("Highlighted Codebox: " + highlightedCodebox.GetComponent<CodeBoxController>().CodeboxName+
                      "\nFile Path: " + highlightedCodebox.GetComponent<CodeBoxController>().FilePath +
                      "\nLine Number: " + highlightedCodebox.GetComponent<CodeBoxController>().LineNumber);
            networkListener.SendJumpToClass(highlightedCodebox.GetComponent<CodeBoxController>().FilePath,
                int.Parse(highlightedCodebox.GetComponent<CodeBoxController>().LineNumber));

        }
        else
        {
            Debug.Log("No codebox is currently highlighted.");
        }
    }

    private GameObject GetCodeboxByName(string className)
    {
        if (codeboxInstances.TryGetValue(className, out GameObject codebox))
        {
            return codebox;
        }
        else
        {
            Debug.LogError($"Codebox for class {className} not found.");
            return null;
        }
    }

    public void JumpToCurrentCodeBox(String className)
    {
        GameObject codebox = GetCodeboxByName(className);
        CodeBoxController codeBoxController = codebox.GetComponent<CodeBoxController>();
        String filePath = codeBoxController.FilePath;
        String lineNumber = codeBoxController.LineNumber;
        
        networkListener.SendJumpToClass(filePath, int.Parse(lineNumber));

    }
}
[System.Serializable]
public class ChatHistoryWrapper
{
    public ChatHistoryData[] classes;
}

[System.Serializable]
public class ChatHistoryData
{
    public string className;
    public string filePath;
    public ChatMessage[] chatHistory;
}

[System.Serializable]
public class ChatMessage
{
    public string role;
    public string text;
}