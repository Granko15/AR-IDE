using System.Collections.Generic;
using UnityEngine;

public class CodeboxManager : MonoBehaviour
{
    public GameObject codeboxPrefab;  // Prefab for the codeboxes
    public JsonParser jsonParser;    // Reference to JSON parser
    public RelationshipManager relationshipManager; // Reference to RelationshipManager

    private float currentXPos = -12f;
    private float currentYPos = 2.5f;
    private float currentZPos = 50f;

    public float spawnDistance = 5000f;

    private Dictionary<string, GameObject> codeboxInstances = new Dictionary<string, GameObject>();

    void Start()
    {
        if (jsonParser != null)
        {
            JsonData data = jsonParser.GetJsonData();
            if (data != null)
            {
                InstantiateCodeboxes(data);

                // Pass the instantiated codeboxes to the RelationshipManager
                if (relationshipManager != null)
                {
                    relationshipManager.SetupRelationships(data, codeboxInstances);
                }
                else
                {
                    Debug.LogError("RelationshipManager is not assigned.");
                }
            }
            else
            {
                Debug.LogError("JSON Data is null.");
            }
        }
        else
        {
            Debug.LogError("JsonParser is not assigned.");
        }
    }

    void InstantiateCodeboxes(JsonData jsonData)
    {
        foreach (var classEntry in jsonData.classes)
        {
            string className = classEntry.Key;
            ClassData classData = classEntry.Value;

            // Instantiate a codebox
            Vector3 position = new Vector3(currentXPos, currentYPos, currentZPos);
            GameObject codebox = Instantiate(codeboxPrefab, position, Quaternion.identity);

            // Store reference
            codeboxInstances[className] = codebox;

            HideCodebox(className);
            // Populate the codebox with class data
            CodeboxPopulator populator = codebox.GetComponent<CodeboxPopulator>();
            if (populator != null)
            {
                populator.PopulateClassData(classData, className);
            }
        }
    }

    public void DisplayCodebox(string className)
    {
        if (codeboxInstances.TryGetValue(className, out GameObject codebox))
        {
            // Get camera position and forward direction
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 forwardDirection = Camera.main.transform.forward;

            // Adjust position to ensure the codebox is placed further away on the Z-axis
            Vector3 newPosition = cameraPosition + forwardDirection * spawnDistance;

            // Keep only the horizontal rotation (you can adjust the degree of rotation if needed)
            Vector3 lookDirection = new Vector3(forwardDirection.x, 0, forwardDirection.z);
            Quaternion horizontalRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(0, 90f, 0);  // Adjust angle as needed

            // Apply position and rotation
            codebox.transform.position = newPosition;
            codebox.transform.rotation = horizontalRotation;

            // Make the codebox visible
            codebox.SetActive(true);

            // Display relationship lines if related codeboxes are displayed
            if (relationshipManager != null)
            {
                relationshipManager.DisplayRelationships(className, codeboxInstances);
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
            // Hide the codebox
            codebox.SetActive(false);

            // Hide relationship lines
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
}
