using System.Collections.Generic;
using UnityEngine;

public class RelationshipManager : MonoBehaviour
{
    private List<RelationshipLine> relationshipLines = new List<RelationshipLine>();

    public void SetupRelationships(JsonData jsonData, Dictionary<string, GameObject> codeboxInstances)
    {
        foreach (var classEntry in jsonData.classes)
        {
            string className = classEntry.Key;
            ClassData classData = classEntry.Value;

            if (codeboxInstances.TryGetValue(className, out GameObject sourceCodebox))
            {
                // Create composition relationships
                foreach (string relatedClass in classData.composition)
                {
                    if (codeboxInstances.TryGetValue(relatedClass, out GameObject targetCodebox))
                    {
                        CreateLineBetween(sourceCodebox, targetCodebox, Color.green);
                    }
                }

                // Create usage relationships
                foreach (string relatedClass in classData.uses)
                {
                    if (codeboxInstances.TryGetValue(relatedClass, out GameObject targetCodebox))
                    {
                        CreateLineBetween(sourceCodebox, targetCodebox, Color.blue);
                    }
                }

                // Create inheritance relationships
                foreach (string baseClass in classData.base_classes)
                {
                    if (codeboxInstances.TryGetValue(baseClass, out GameObject targetCodebox))
                    {
                        CreateLineBetween(sourceCodebox, targetCodebox, Color.red);
                    }
                }
            }
        }
    }

    void CreateLineBetween(GameObject source, GameObject target, Color color)
    {
        // Create a new GameObject to hold the LineRenderer
        GameObject lineObject = new GameObject("RelationshipLine");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Configure LineRenderer properties
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        // Set initial positions
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, source.transform.position);
        lineRenderer.SetPosition(1, target.transform.position);

        // Add to the list of relationship lines for updating
        relationshipLines.Add(new RelationshipLine(source, target, lineRenderer));
    }

    void Update()
    {
        // Update the positions of all relationship lines
        foreach (var line in relationshipLines)
        {
            line.UpdatePositions();
        }
    }
}
