using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CtrlBackspaceHandler : MonoBehaviour
{
    public TMP_InputField inputField;

    void Update()
    {
        // Check if the Ctrl and Backspace keys are pressed together and the input field is focused
        if (Input.GetKeyDown(KeyCode.Backspace) && Input.GetKey(KeyCode.LeftControl) && inputField.isFocused)
        {
            HandleCtrlBackspace();
        }
    }

    private void HandleCtrlBackspace()
    {
        // Get the caret position in the text
        int caretPosition = inputField.caretPosition;

        if (caretPosition > 0)
        {
            int stringPosition = GetStringPositionFromCaret(inputField.text, caretPosition);

            // Find the position of the previous word boundary
            int wordBoundaryPosition = FindPreviousWordBoundary(inputField.text, stringPosition);

            // Remove text from word boundary to caret position
            string newText = inputField.text.Remove(wordBoundaryPosition, stringPosition - wordBoundaryPosition);

            // Update the text in the input field
            inputField.text = newText;

            // Move the caret to the word boundary
            inputField.caretPosition = wordBoundaryPosition;
        }
    }

    private int FindPreviousWordBoundary(string text, int caretPosition)
    {
        // Start searching backward from the current caret position
        int position = caretPosition - 1;

        // Stop at the beginning of the line or text
        while (position >= 0 && text[position] != '\n')
        {
            // If we encounter whitespace, remove all consecutive whitespace
            if (char.IsWhiteSpace(text[position]))
            {
                while (position >= 0 && char.IsWhiteSpace(text[position]) && text[position] != '\n')
                {
                    position--;
                }
                // After finding all whitespace, adjust the position to one step forward
                position++;
                break;
            }

            // If we encounter a non-alphanumeric character, stop
            if (!char.IsLetterOrDigit(text[position]))
            {
                position++;
                break;
            }

            position--;
        }

        // If we're at a newline or start of the text, adjust position
        if (position < 0 || text[position] == '\n')
        {
            position++;
        }

        return position;
    }

    private int GetStringPositionFromCaret(string text, int caretPosition)
    {
        int visibleCharCount = 0;
        bool insideTag = false;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '<' && !insideTag)
            {
                int closingTagIndex = text.IndexOf('>', i);

                if (closingTagIndex != -1 && IsRichTextTag(text, i, closingTagIndex))
                {
                    insideTag = true;
                    i = closingTagIndex - 1;
                    continue;
                }
            }
            
            if (!insideTag)
            {
                visibleCharCount++;

                if (visibleCharCount == caretPosition)
                {
                    return i + 1;
                }
            }

            if (insideTag && text[i] == '>')
            {
                insideTag = false;
            }
        }

        return -1;
    }

    private bool IsRichTextTag(string text, int startIndex, int endIndex)
    {
        string tagContent = text.Substring(startIndex + 1, endIndex - startIndex - 1);
        if (tagContent.StartsWith("/") || tagContent.StartsWith("color") || tagContent.StartsWith("b") || tagContent.StartsWith("i"))
        {
            return true;
        }

        return false;
    }
}
