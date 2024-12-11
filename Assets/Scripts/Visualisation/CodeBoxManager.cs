using System.Collections.Generic;
using UnityEngine;

public class CodeboxManager : MonoBehaviour
{
    public GameObject codeboxPrefab;  // Prefab for the codeboxes
    public JsonParser jsonParser;    // Reference to JSON parser
    public RelationshipManager relationshipManager; // Reference to RelationshipManager

    public float xSpacing = 1f; // Spacing for arranging codeboxes
    public float ySpacing = 1f;
    public float zSpacing = 2f;

    private float currentXPos = -12f;
    private float currentYPos = 2.5f;
    private float currentZPos = 2.2f;

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

            // Rotate the codebox
            codebox.transform.Rotate(0, 90f, 0);

            // Update position for the next codebox
            currentXPos += xSpacing;

            // Store reference
            codeboxInstances[className] = codebox;

            // Populate the codebox with class data
            CodeboxPopulator populator = codebox.GetComponent<CodeboxPopulator>();
            if (populator != null)
            {
                populator.PopulateClassData(classData, className);
            }
        }
    }
}
