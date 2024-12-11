using UnityEngine;
using TMPro;

public class CodeboxPopulator : MonoBehaviour
{
    public TMP_Text Side1Text; // Class name, attributes, methods
    public TMP_Text Side2Text; // Composition, uses
    public TMP_Text Side3Text; // Children
    public TMP_Text Side4Text; // Parents
    public TMP_Text Side5Text; // Usage
    public TMP_Text Side6Text; // File path, package, line number
    public TMP_Text Side7Text; // ChatOpenAI history of prompts and responses

    public void PopulateClassData(ClassData classData, string className)
    {
        if (Side1Text != null)
        {
            Side1Text.text = $"Class: {className}\n" +
                             $"Attributes:\n\t{string.Join("\n\t", classData.attributes)}\n\n" +
                             $"Methods:\n\t{string.Join("\n\t", GetMethodNames(classData.methods))}";
        }

        if (Side2Text != null)
        {
            Side2Text.text = $"Composition:\n\t{string.Join("\n\t", classData.composition)}\n" +
                             $"Uses:\n\t{string.Join("\n\t", classData.uses)}";
        }

        if (Side3Text != null)
        {
            Side3Text.text = $"Children:\n\t{string.Join("\n\t", GetChildrenNames(className))}";
        }

        if (Side4Text != null)
        {
            Side4Text.text = $"Parents:\n\t{string.Join("\n\t", classData.base_classes)}";
        }

        if (Side5Text != null)
        {
            Side5Text.text = $"Usage:\n\t{string.Join("\n\t", classData.uses)}";
        }

        if (Side6Text != null)
        {
            Side6Text.text = $"File Path:\t{classData.file_path}\n" +
                             $"Package:\t{classData.package}\n" +
                             $"Line Number:\t{classData.line_number}";
        }
    }

    private string[] GetMethodNames(Method[] methods)
    {
        if (methods == null || methods.Length == 0)
            return new string[] { "None" };

        string[] methodNames = new string[methods.Length];
        for (int i = 0; i < methods.Length; i++)
        {
            methodNames[i] = $"{methods[i].name} (Line: {methods[i].line_number})";
        }
        return methodNames;
    }

    private string[] GetChildrenNames(string className)
    {
        return new string[] { "Child1", "Child2" };
    }
}
