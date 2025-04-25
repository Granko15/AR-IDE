using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;

public class CodeBoxController : MonoBehaviour
{
    [SerializeField] private GameObject AttributeScrollButtons;
    [SerializeField] private GameObject ChildrenScrollButtons;
    [SerializeField] private GameObject ParentScrollButtons;
    [SerializeField] private GameObject MetadataScrollButtons;
    [SerializeField] private GameObject UsageScrollButtons;
    [SerializeField] private GameObject RelationshipScrollButtons;
    [SerializeField] private GameObject AIScrollButtons;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material highlightedMaterial;
    [SerializeField] private GameObject CodeBoxBody; // Prefab for the codebox populator
    private Renderer objectRenderer;
    private static CodeBoxController currentSelectedController;
    private bool isHighlighted = false;

    public string CodeboxName { get; set; } // Add a property to store the name
    public string FilePath { get; set; } // Add a property to store the file path
    public string LineNumber { get; set; } // Add a property to store the line number

    void Start()
    {
        objectRenderer = CodeBoxBody.GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogWarning("Renderer is null on " + CodeBoxBody.name);
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
        
        if (isHighlighted)
        {
            CodeboxManager.Instance?.SetHighlightedCodebox(this.gameObject);
        }
        else
        {
            CodeboxManager.Instance?.SetHighlightedCodebox(null);
        }
    }

    public void RotateObject()
    {
        CodeBoxBody.transform.Rotate(0, 51, 0);
        AttributeScrollButtons.transform.Rotate(0, 51, 0);
        ChildrenScrollButtons.transform.Rotate(0, 51, 0);
        ParentScrollButtons.transform.Rotate(0, 51, 0);
        MetadataScrollButtons.transform.Rotate(0, 51, 0);
        UsageScrollButtons.transform.Rotate(0, 51, 0);
        RelationshipScrollButtons.transform.Rotate(0, 51, 0);
        AIScrollButtons.transform.Rotate(0, 51, 0);
    }

    private void HighlightCodebox(bool highlight)
    {
        foreach (Transform child in CodeBoxBody.transform)
        {
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                childRenderer.material = highlight ? highlightedMaterial : normalMaterial;
            }
        }
    }

    private void UnHighlightCodebox()
    {
        HighlightCodebox(false);
        isHighlighted = false;
    }
}