using System;
using System.Collections.Generic;
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

    /*void LateUpdate()
    {
        if (!instantiated)
        {
            instantiated = true; // Set the flag to true after instantiation
            if (jsonParser != null)
            {
                JsonData projectData = jsonParser.GetJsonData();
                if (projectData != null)
                {
                    InstantiateCodeboxes(projectData);
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
    }*/

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

    public void PrintHighlightedCodebox()
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
}