using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;

public class AutoIndentHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    private bool completedTransform = false;
    private int originalStringPosition;
    private int originalCaretPosition;

    void Update()
    {
        // Check if Enter key is pressed
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && inputField.isFocused)
        {
            HandleEnterKey();
        }
    }

    private void LateUpdate()
    {
        if (completedTransform)
        {
            inputField.caretPosition = originalCaretPosition;
            completedTransform = false;
        }
    }

    private void HandleEnterKey()
    {
        // Get the current caret position
        originalStringPosition = inputField.stringPosition;
        originalCaretPosition = inputField.caretPosition;

        var textBeforeCaret = inputField.text.Substring(0, originalStringPosition);

        var (areInCodeBlock, indexOfTheFirstCharOfLine) = AreInCodeBlock(textBeforeCaret);
        if (areInCodeBlock)
        {
            var newLineSpace = indexOfTheFirstCharOfLine + 2;
            var spaces = new string(' ', newLineSpace);
            inputField.text = inputField.text.Insert(originalStringPosition, spaces);
            originalCaretPosition += newLineSpace;
        }

        completedTransform = true;
    }

    private (bool, int) AreInCodeBlock(string textBeforeCaret)
    {
        if (textBeforeCaret == null) return (false, 0);
        textBeforeCaret = StripTags(textBeforeCaret);
        var cumulativeLength = 0;
        var lines = textBeforeCaret.Split('\n');
        var lastLineFirstNonSpaceCharIndex = int.MaxValue;
        var previousLine = lines.ElementAt(lines.Length - 2);

        if (previousLine != null)
        {
            lastLineFirstNonSpaceCharIndex = previousLine.TakeWhile(char.IsWhiteSpace).Count();
        }

        for (int i = lines.Length - 1; i >= 0; i--)
        {
            var currentLine = lines[i];
            var lineLength = currentLine.Length + 1; // +1 for the newline character

            if (IsInCodeBlock(currentLine))
            {
                var firstNonSpaceCharIndex = currentLine.TakeWhile(char.IsWhiteSpace).Count();
                if (i != lines.Length - 2)
                {
                    if (firstNonSpaceCharIndex > 0 && lastLineFirstNonSpaceCharIndex <= firstNonSpaceCharIndex)
                    {
                        continue;
                    }
                }
                return (true, firstNonSpaceCharIndex);
            }
            cumulativeLength += lineLength;
        }
        return (false, 0);
    }

    private bool IsInCodeBlock(string textBeforeCaret)
    {
        // Regex to find if the caret is inside a Python block (e.g., def, if, for, while, try, class, etc.)
        Regex codeBlockRegex = new Regex(@"(def|if|for|while|try|class|with|elif|else|except)\s.*:\s*(\n|\r\n)?(\s+.*)?$", RegexOptions.Multiline);
        return codeBlockRegex.IsMatch(textBeforeCaret);
    }

    private string StripTags(string input)
    {
        // Regex to match any tag in the form <tag>...</tag> or <tag ... />
        return Regex.Replace(input, @"<[^>]+>", string.Empty);
    }
}
