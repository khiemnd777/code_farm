using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;
using System.Text;

public class TabToSpacesHandler : MonoBehaviour
{
    public TMP_InputField inputField;
    private bool completedTransform = false;
    private int originalCaretPosition;
    private int originalStringPosition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && inputField.isFocused)
        {
            HandleTabKey();
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

    private void HandleTabKey()
    {
        // Get the current caret position
        originalCaretPosition = inputField.caretPosition;
        originalStringPosition = inputField.stringPosition;

        var text = inputField.text;
        var strippedText = StripTags(text);
        var textBeforeCaret = text.Substring(0, originalStringPosition);
        var lines = textBeforeCaret.Split('\n');

        if (strippedText[originalCaretPosition - 1] == '\t')
        {
            var indexNearestTab = FindNearestTab(text, originalStringPosition);
            if (indexNearestTab > -1)
            {
                originalStringPosition = indexNearestTab + 1;
            }
            text = text.Remove(originalStringPosition - 1, 1);
            var space = (originalCaretPosition - 1) % 2 == 0 ? "  " : " ";
            text = text.Insert(originalStringPosition - 1, space);
            inputField.text = text;
            originalCaretPosition += space.Length - 1;
        }
        completedTransform = true;
    }

    private string StripTags(string input)
    {
        // Regex to match any tag in the form <tag>...</tag> or <tag ... />
        return Regex.Replace(input, @"<[^>]+>", string.Empty);
    }

    private int FindNearestTab(string originalText, int position)
    {
        // Step 1: Create a mapping of original string to stripped string
        string strippedText;
        int[] originalToStrippedMapping = StripTagsAndMap(originalText, out strippedText);
        
        if (originalToStrippedMapping.Length - 1 < position) return -1;

        // Step 2: Find the nearest tab character in the stripped string
        int strippedPosition = originalToStrippedMapping[position];
        for (int i = strippedPosition - 1; i >= 0; i--)
        {
            if (strippedText[i] == '\t')
            {
                // Find the original position corresponding to the stripped position
                for (int j = 0; j < originalToStrippedMapping.Length; j++)
                {
                    if (originalToStrippedMapping[j] == i)
                    {
                        return j;  // Return the position in the original string
                    }
                }
            }
        }

        return -1;  // Return -1 if no tab character was found to the left
    }

    private int[] StripTagsAndMap(string originalText, out string strippedText)
    {
        // Step 3: Create a mapping array to map original positions to stripped positions
        int[] originalToStrippedMapping = new int[originalText.Length];
        StringBuilder strippedTextBuilder = new StringBuilder();
        int strippedIndex = 0;

        // Regex to match any tag (like <color>)
        Regex tagRegex = new Regex(@"<[^>]+>");
        int originalIndex = 0;

        // Iterate through the original text and strip tags while keeping track of the mapping
        while (originalIndex < originalText.Length)
        {
            Match match = tagRegex.Match(originalText, originalIndex);

            if (match.Success && match.Index == originalIndex)
            {
                // Skip the tag
                for (int i = 0; i < match.Length; i++)
                {
                    originalToStrippedMapping[originalIndex++] = -1;  // Tag part, no mapping
                }
            }
            else
            {
                // Add the character to the stripped text and record its mapping
                strippedTextBuilder.Append(originalText[originalIndex]);
                originalToStrippedMapping[originalIndex] = strippedIndex;
                originalIndex++;
                strippedIndex++;
            }
        }

        strippedText = strippedTextBuilder.ToString();
        return originalToStrippedMapping;
    }
}
