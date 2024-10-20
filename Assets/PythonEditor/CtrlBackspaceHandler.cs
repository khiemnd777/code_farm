using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            int wordBoundaryPosition = FindPreviousWordBoundary(inputField.text, stringPosition);
            string newText = inputField.text.Remove(wordBoundaryPosition, stringPosition - wordBoundaryPosition);
            inputField.text = newText;
            inputField.caretPosition = wordBoundaryPosition;
        }
    }

    private int FindPreviousWordBoundary(string text, int caretPosition)
    {
        int position = caretPosition - 1;

        while (position >= 0 && text[position] != '\n')
        {
            if (char.IsWhiteSpace(text[position]))
            {
                while (position >= 0 && char.IsWhiteSpace(text[position]) && text[position] != '\n')
                {
                    position--;
                }
                position++;
                break;
            }

            if (!IsWordCharacter(text[position]))
            {
                position++;
                break;
            }

            position--;
        }

        if (position < 0 || (position <= text.Length - 1 && text[position] == '\n'))
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

    private bool IsWordCharacter(char c)
    {
        if (char.IsLetterOrDigit(c) || c == '_')
        {
            return true;
        }
        return false;
    }

    private string StripTags(string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }
}
