using UnityEngine;
using UnityEngine.InputSystem;
using MixedReality.Toolkit.UX;
using System;

public class PrefabButtonHandler : MonoBehaviour
{
    [Serializable]
    public class ButtonAction
    {
        public PressableButton button; // Reference to the button
        public string actionName; // Name of the action (e.g., "Rotate", "Hide")
    }

    public CodeBoxController codeBoxController; // Reference to the CodeBoxController
    public ButtonAction[] buttonActions; // Array of buttons and their actions

    void Start()
    {
        foreach (var buttonAction in buttonActions)
        {
            if (buttonAction.button != null)
            {
                buttonAction.button.OnClicked.AddListener(() => OnButtonClick(buttonAction.actionName));
            }
            else
            {
                Debug.LogError("Button component is not assigned for an action.");
            }
        }
    }

    void OnButtonClick(string actionName)
    {
        if (CodeboxManager.Instance == null)
        {
            Debug.LogError("CodeboxManager instance not found in scene.");
            return;
        }

        switch (actionName)
        {
            case "Rotate":
                codeBoxController.RotateObject();
                break;

            case "Hide":
                CodeboxManager.Instance.HideCodebox(codeBoxController.CodeboxName);
                break;

            case "JumpToCurrent":
                CodeboxManager.Instance.JumpToCurrentCodeBox(codeBoxController.CodeboxName);
                break;
            
            case "Highlight":
                codeBoxController.ToggleHighlight();
                break;
            case "SwitchToThisCodebox":
                CodeboxManager.Instance.SendSwitchCommandInAIAssistant(codeBoxController.CodeboxName, codeBoxController.FilePath);
                break;
            default:
                Debug.LogWarning($"Action '{actionName}' is not recognized.");
                break;
        }
    }
}