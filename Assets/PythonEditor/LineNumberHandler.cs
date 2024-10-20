using UnityEngine;
using TMPro;

public class LineNumberHandler : MonoBehaviour
{
    public TMP_InputField inputField; // The TMP_InputField for code input
    public TMP_InputField lineNumberText;   // The TMP_Text to show the line numbers

    void Start()
    {
        // Add a listener to detect changes in the input field
        inputField.onValueChanged.AddListener(UpdateLineNumbers);

        // Initialize the line numbers when the editor starts
        UpdateLineNumbers(inputField.text);
    }

    private void UpdateLineNumbers(string text)
    {
        // Get the current number of lines
        int lineCount = inputField.text.Split('\n').Length;

        // Build the line numbers string
        string lineNumbers = "";
        for (int i = 1; i <= lineCount; i++)
        {
            lineNumbers += i + "\n";
        }

        // Update the line numbers text
        lineNumberText.text = lineNumbers;
    }
}
