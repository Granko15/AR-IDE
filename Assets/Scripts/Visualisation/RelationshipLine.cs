using UnityEngine;

public class RelationshipLine : MonoBehaviour
{
    public GameObject source;
    public GameObject target;
    public LineRenderer lineRenderer;
    public string relationshipType;

    public void UpdatePositions()
    {
        // Update the LineRenderer to follow the positions of the source and target
        lineRenderer.SetPosition(0, source.transform.position);
        lineRenderer.SetPosition(1, target.transform.position);
    }
}