using System.Collections.Generic;
using UnityEngine;

public class RelationshipManager : MonoBehaviour
{
    private Dictionary<string, List<LineRenderer>> relationshipLines = new Dictionary<string, List<LineRenderer>>();

    public void SetupRelationships(JsonData jsonData, Dictionary<string, GameObject> codeboxInstances)
    {
        // Remove all existing relationships before creating new ones
        ClearAllRelationships();

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
                        CreateLineBetween(sourceCodebox, targetCodebox, Color.green, "composition");
                    }
                }

                // Create usage relationships
                foreach (string relatedClass in classData.uses)
                {
                    if (codeboxInstances.TryGetValue(relatedClass, out GameObject targetCodebox))
                    {
                        CreateLineBetween(sourceCodebox, targetCodebox, Color.blue, "usage");
                    }
                }

                // Create inheritance relationships
                foreach (string baseClass in classData.base_classes)
                {
                    if (codeboxInstances.TryGetValue(baseClass, out GameObject targetCodebox))
                    {
                        CreateLineBetween(sourceCodebox, targetCodebox, Color.red, "inheritance");
                    }
                }
            }
        }
    }

    public void DisplayRelationships(string className)
    {
        if (relationshipLines.TryGetValue(className, out List<LineRenderer> lines))
        {
            foreach (var line in lines)
            {
                RelationshipLine relationshipLine = line.GetComponent<RelationshipLine>();
                if (relationshipLine != null && relationshipLine.source.activeSelf && relationshipLine.target.activeSelf)
                {
                    line.gameObject.SetActive(true);
                }
            }
        }
    }

    public void HideRelationships(string className)
    {
        if (relationshipLines.TryGetValue(className, out List<LineRenderer> lines))
        {
            foreach (var line in lines)
            {
                line.gameObject.SetActive(false);
            }
        }
    }

    private void CreateLineBetween(GameObject source, GameObject target, Color color, string relationshipType)
    {
        // Create a new GameObject to hold the LineRenderer
        GameObject lineObject = new GameObject($"RelationshipLine_{source.name}_{target.name}_{relationshipType}");
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

        // Add RelationshipLine component
        RelationshipLine relationshipLine = lineObject.AddComponent<RelationshipLine>();
        relationshipLine.source = source;
        relationshipLine.target = target;
        relationshipLine.lineRenderer = lineRenderer;

        // Add to the list of relationship lines for updating
        if (!relationshipLines.ContainsKey(source.name))
        {
            relationshipLines[source.name] = new List<LineRenderer>();
        }
        relationshipLines[source.name].Add(lineRenderer);

        if (!relationshipLines.ContainsKey(target.name))
        {
            relationshipLines[target.name] = new List<LineRenderer>();
        }
        relationshipLines[target.name].Add(lineRenderer);
    }

    private void Update()
    {
        // Update the positions of all relationship lines
        foreach (var lineList in relationshipLines.Values)
        {
            foreach (var line in lineList)
            {
                RelationshipLine relationshipLine = line.GetComponent<RelationshipLine>();
                if (relationshipLine != null)
                {
                    relationshipLine.UpdatePositions();
                    // Update visibility based on the active status of source and target
                    line.gameObject.SetActive(relationshipLine.source.activeSelf && relationshipLine.target.activeSelf);
                }
            }
        }
    }

    public void RemoveRelationships(string className)
    {
        if (relationshipLines.TryGetValue(className, out List<LineRenderer> lines))
        {
            foreach (var line in lines)
            {
                Destroy(line.gameObject); // Destroy the line object
            }
            relationshipLines.Remove(className);
        }
    }

    public void ClearAllRelationships()
    {
        // Remove all existing relationships before adding new ones
        foreach (var key in new List<string>(relationshipLines.Keys))
        {
            RemoveRelationships(key);
        }
        relationshipLines.Clear();
    }
}
