using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RelationshipManager : MonoBehaviour
{
    private Dictionary<string, List<LineRenderer>> relationshipLines = new Dictionary<string, List<LineRenderer>>();
    private enum RelationshipType { None, Composition, Usage, Inheritance }
    private RelationshipType currentRelationshipType = RelationshipType.None;
    public TMPro.TextMeshProUGUI relationshipLabel;

    public void SetupRelationships(JsonData jsonData, Dictionary<string, GameObject> codeboxInstances)
    {
        ClearAllRelationships();
        CreateAllRelationships(jsonData, codeboxInstances);
        UpdateAllRelationshipVisibility(); // Nastavíme počiatočnú viditeľnosť
    }

    private void CreateAllRelationships(JsonData jsonData, Dictionary<string, GameObject> codeboxInstances)
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

    public void CycleRelationshipDisplay()
    {
        currentRelationshipType = (RelationshipType)(((int)currentRelationshipType + 1) % 5);
        Debug.Log("Current Relationship Type: " + currentRelationshipType);
        UpdateAllRelationshipVisibility();

        if (relationshipLabel != null)
        {
            switch (currentRelationshipType)
            {
                case RelationshipType.None:
                    relationshipLabel.text = "Display Relationships";
                    break;
                case RelationshipType.Composition:
                    relationshipLabel.text = "Composition";
                    break;
                case RelationshipType.Usage:
                    relationshipLabel.text = "Usage";
                    break;
                case RelationshipType.Inheritance:
                    relationshipLabel.text = "Inheritance";
                    break;
            }
        }
        else
        {
            Debug.LogError("TextMeshPro UI Label 'relationshipLabel' nie je priradený v RelationshipManageri!");
        }
    }

    private void UpdateAllRelationshipVisibility()
    {
        foreach (var lineList in relationshipLines.Values)
        {
            foreach (var line in lineList)
            {
                UpdateLineVisibility(line);
            }
        }
    }

    private void UpdateLineVisibility(LineRenderer line)
    {
        RelationshipLine relationshipLine = line.GetComponent<RelationshipLine>();
        if (relationshipLine != null && relationshipLine.source.activeSelf && relationshipLine.target.activeSelf)
        {
            bool shouldDisplay = false;
            switch (currentRelationshipType)
            {
                case RelationshipType.None:
                    shouldDisplay = false;
                    break;
                case RelationshipType.Composition:
                    shouldDisplay = line.name.Contains("composition");
                    break;
                case RelationshipType.Usage:
                    shouldDisplay = line.name.Contains("usage");
                    break;
                case RelationshipType.Inheritance:
                    shouldDisplay = line.name.Contains("inheritance");
                    break;
            }
            line.gameObject.SetActive(shouldDisplay);
        }
        else
        {
            line.gameObject.SetActive(false); 
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