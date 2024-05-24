using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CodeEditor : MonoBehaviour
{
    public TMP_InputField inputField;

    public TextMeshProUGUI columnText;

    private void Update()
    {
        if (inputField.text.Contains("\n"))
        {
            AddLineNumbers();
        }
    }

    private void AddLineNumbers()
    {
        int clines = inputField.text.Split('\n').Length;
        string[] lines = new string[clines];
        for (int i = 0; i < clines; i++)
            lines[i] = "";

        for (int i = 0; i < clines; i++)
        {
            lines[i] = lines[i].TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ', '\t');
            lines[i] = $"{i + 1:D3}    {lines[i]}";
        }
        columnText.text = string.Join("\n", lines);
    }
}
