using UnityEngine;
using TMPro;

public class CodeboxPopulator : MonoBehaviour
{
    public TMP_Text ClassNameText; // Class name, attributes, methods
    public TMP_Text RelationshipText; // Composition, uses
    public TMP_Text InheritanceText; // Children
    public TMP_Text ParentsText; // Parents
    public TMP_Text UsageText; // Usage
    public TMP_Text MetaDataText; // File path, package, line number
    public TMP_Text AIAssistantText; // ChatOpenAI history of prompts and responses

    public void PopulateClassData(ClassData classData, string className)
    {
        if (ClassNameText != null)
        {
            ClassNameText.text = $"Class: {className}\n" +
                             $"Attributes:\n\t{string.Join("\n\t", classData.attributes)}\n\n" +
                             $"Methods:\n\t{string.Join("\n\t", GetMethodNames(classData.methods))}";
        }

        if (RelationshipText != null)
        {
            RelationshipText.text = $"Composition:\n\t{string.Join("\n\t", classData.composition)}\n" +
                             $"Uses:\n\t{string.Join("\n\t", classData.uses)}";
        }

        if (InheritanceText != null)
        {
            InheritanceText.text = $"Children:\n\t{string.Join("\n\t", GetChildrenNames(className))}";
        }

        if (ParentsText != null)
        {
            ParentsText.text = $"Parents:\n\t{string.Join("\n\t", classData.base_classes)}";
        }

        if (UsageText != null)
        {
            UsageText.text = $"Usage:\n\t{string.Join("\n\t", classData.uses)}";
        }

        if (MetaDataText != null)
        {
            MetaDataText.text = $"File Path:\t{classData.file_path}\n" +
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
