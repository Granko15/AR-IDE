using UnityEngine;
using UnityEngine.InputSystem;
using MixedReality.Toolkit.UX;
public class PrefabButtonHandler : MonoBehaviour
{
    private PressableButton button;
    public CodeBoxController codeBoxController; // Reference to the CodeBoxController

    void Start()
    {
        button = GetComponent<PressableButton>();
        if (button != null)
        {
            button.OnClicked.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Button component not found on " + codeBoxController.CodeboxName);
        }
    }

    void OnButtonClick()
    {
        if (CodeboxManager.Instance != null)
        {
            CodeboxManager.Instance.HideCodebox(codeBoxController.CodeboxName);
        }
        else
        {
            Debug.LogError("GameManager instance not found in scene.");
        }
    }
}