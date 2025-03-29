using UnityEngine;
using UnityEngine.EventSystems;

public class CodeBoxController : MonoBehaviour
{
    [SerializeField] private GameObject polyCodeObject;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material highlightedMaterial;
    private Renderer objectRenderer;
    private static CodeBoxController currentSelectedController;
    private bool isHighlighted = false;

    public string CodeboxName { get; set; } // Add a property to store the name
    public string FilePath { get; set; } // Add a property to store the file path
    public string LineNumber { get; set; } // Add a property to store the line number

    void Start()
    {
        objectRenderer = polyCodeObject.GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("Renderer is null on " + polyCodeObject.name);
        }
    }

    public void ToggleHighlight()
    {
        if (currentSelectedController != null && currentSelectedController != this)
        {
            currentSelectedController.UnHighlightCodebox();
        }

        isHighlighted = !isHighlighted;
        HighlightCodebox(isHighlighted);

        currentSelectedController = isHighlighted ? this : null;

        // Notify CodeboxManager of the change
        if (isHighlighted)
        {
            CodeboxManager.Instance?.SetHighlightedCodebox(this.gameObject); // Assuming CodeboxManager is a singleton
        }
        else
        {
            CodeboxManager.Instance?.SetHighlightedCodebox(null);
        }
    }

    public void RotateObject()
    {
        polyCodeObject.transform.Rotate(0, 51, 0);
    }

    private void HighlightCodebox(bool highlight)
    {
        if (objectRenderer != null)
        {
            Debug.Log("Highlighting codebox: " + highlight);
            objectRenderer.material = highlight ? highlightedMaterial : normalMaterial;
        }
        else
        {
            Debug.LogError("objectRenderer is null in HighlightCodebox!");
        }
    }

    private void UnHighlightCodebox()
    {
        HighlightCodebox(false);
        isHighlighted = false;
    }
}