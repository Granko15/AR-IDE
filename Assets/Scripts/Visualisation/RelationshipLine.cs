using UnityEngine;

public class RelationshipLine
{
    private GameObject source;
    private GameObject target;
    private LineRenderer lineRenderer;

    public RelationshipLine(GameObject source, GameObject target, LineRenderer lineRenderer)
    {
        this.source = source;
        this.target = target;
        this.lineRenderer = lineRenderer;
    }

    public void UpdatePositions()
    {
        // Update the LineRenderer to follow the positions of the source and target
        lineRenderer.SetPosition(0, source.transform.position);
        lineRenderer.SetPosition(1, target.transform.position);
    }
}
