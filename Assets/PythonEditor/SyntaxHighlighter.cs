using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Text;

public class SyntaxHighlighter : MonoBehaviour
{
    public TMP_InputField inputField;
    private bool isUpdating = false;
    private bool completedTransform = false;
    private bool updateFromShiftTab = false;
    private int originalCaretPosition;
    private int originalStringPosition;
    private string tabSpaceStr;

    private void Start()
    {
        inputField.onValidateInput = ValidateInput;
        inputField.onValueChanged.AddListener(HighlightSyntax);
    }

    void Update()
    {
        if (inputField.isFocused)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    originalCaretPosition = inputField.caretPosition;
                    var text = inputField.text;
                    var stringPosition = GetStringPositionFromCaret(text, originalCaretPosition);
                    if (text[stringPosition - 1] == ' ')
                    {
                        updateFromShiftTab = true;
                        string rawText = StripColorTags(text);
                        var spaceIndex = FindSpaceIndexFromCaret(rawText, originalCaretPosition);
                        tabSpaceStr = (spaceIndex) % 2 == 0 ? "  " : " ";
                        for (int i = 0; i < tabSpaceStr.Length; i++)
                        {
                            if (text[stringPosition - 1] == ' ')
                            {
                                text = text.Remove(stringPosition - 1, 1);
                                stringPosition--;
                                originalCaretPosition--;
                            }
                        }
                        inputField.text = text;
                        inputField.caretPosition = originalCaretPosition;
                    }
                }
            }
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

    private char ValidateInput(string text, int charIndex, char addedChar)
    {
        // Check if the Tab character is being inserted
        if (addedChar == '\t')
        {
            // Detect if Shift is held down
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                Debug.Log("Shift+Tab detected, preventing input.");
                return '\0'; // Prevent Tab character from being added when Shift is held
            }
        }

        // Allow the character input normally
        return addedChar;
    }

    private void HighlightSyntax(string text)
    {
        if (isUpdating) return;

        isUpdating = true;

        // Save the current caret position
        if (!updateFromShiftTab)
        {
            originalCaretPosition = inputField.caretPosition;
            originalStringPosition = inputField.stringPosition;
        }

        updateFromShiftTab = false;

        try
        {
            // Strip existing <color> tags to avoid nested tags
            string rawText = StripColorTags(text);

            // Replace \t char by two space
            if (rawText[originalCaretPosition - 1] == '\t')
            {
                rawText = rawText.Remove(originalCaretPosition - 1, 1);
                var spaceIndex = FindSpaceIndexFromCaret(rawText, originalCaretPosition - 1);
                tabSpaceStr = (spaceIndex) % 2 == 0 ? "  " : " ";
                rawText = rawText.Insert(originalCaretPosition - 1, tabSpaceStr);
                originalCaretPosition += tabSpaceStr.Length - 1;
            }

            // Apply syntax highlighting
            string highlightedText = rawText;

            // Keywords
            highlightedText = Regex.Replace(highlightedText, @"(\b(False|None|True|and|as|assert|async|await|break|class|continue|def|del|elif|else|except|finally|for|from|global|if|import|in|is|lambda|nonlocal|not|or|pass|raise|return|try|while|with|yield)\b)",
                                       "<color=__hashtag__FFCC00>$1</color>");

            // Built-in functions
            highlightedText = Regex.Replace(highlightedText, @"(\b(abs|dict|help|min|setattr|all|dir|hex|next|slice|any|divmod|id|object|sorted|ascii|enumerate|input|oct|staticmethod|bin|eval|int|open|str|bool|exec|isinstance|ord|sum|bytearray|filter|issubclass|pow|super|bytes|float|iter|print|tuple|callable|format|len|property|type|chr|frozenset|list|range|vars|classmethod|getattr|locals|repr|zip|compile|globals|map|reversed|__import__|complex|hasattr|max|round|delattr|hash|memoryview|set)\b)",
                                            "<color=__hashtag__00CCCC>$1</color>");

            // Decorators
            highlightedText = Regex.Replace(highlightedText, @"(@\w+)",
                                            "<color=__hashtag__FF88FF>$1</color>");

            // Comments
            highlightedText = Regex.Replace(highlightedText, @"(#.*)",
                                            "<color=__hashtag__676767>$1</color>");

            // Strings
            highlightedText = HighlightStrings(highlightedText);

            // Numbers
            highlightedText = Regex.Replace(highlightedText, @"(\b\d+(\.\d+)?\b)",
                                            "<color=__hashtag__FF6666>$1</color>");

            highlightedText = ReplaceColorTagsWithPlaceholders(highlightedText);

            // Temporarily disable event listener to prevent loop
            inputField.onValueChanged.RemoveListener(HighlightSyntax);
            inputField.text = highlightedText;

            inputField.onValueChanged.AddListener(HighlightSyntax);

            isUpdating = false;
            completedTransform = true;
        }
        catch
        {
            isUpdating = false;
        }
    }

    private string ReplaceColorTagsWithPlaceholders(string input)
    {
        return Regex.Replace(input, @"__hashtag__", "#");
    }

    private string HighlightStrings(string text)
    {
        // Handle triple-quoted strings first to avoid confusion with single and double quotes
        text = Regex.Replace(text, @"(?<!<[^>]*)(("""".*?""""|'''.*?'''))(?![^<]*>)",
                             "<color=#FF8800>$1</color>");

        // Handle single- and double-quoted strings
        text = Regex.Replace(text, @"(?<!<[^>]*)(\""[^""\\]*(?:\\.[^""\\]*)*\"")(?![^<]*>)",
                             "<color=#FF8800>$1</color>");
        text = Regex.Replace(text, @"(?<!<[^>]*)(\'[^'\\]*(?:\\.[^'\\]*)*\')(?![^<]*>)",
                             "<color=#FF8800>$1</color>");

        return text;
    }

    private string StripColorTags(string text)
    {
        // This method removes any existing <color> tags
        return Regex.Replace(text, @"<color=[^>]+>|</color>", string.Empty);
    }

    private int FindNearestSpace(string originalText, int position)
    {
        // Step 1: Create a mapping of original string to stripped string
        string strippedText;
        int[] originalToStrippedMapping = StripTagsAndMap(originalText, out strippedText);

        if (originalToStrippedMapping.Length - 1 < position) return -1;

        // Step 2: Find the nearest tab character in the stripped string
        int strippedPosition = originalToStrippedMapping[position];
        for (int i = strippedPosition - 1; i >= 0; i--)
        {
            if (strippedText[i] == ' ')
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

    private int GetStringPositionFromCaret(string text, int caretPosition)
    {
        int visibleCharCount = 0;  // Tracks the number of visible characters
        bool insideTag = false;    // Tracks if we are inside a rich text tag

        for (int i = 0; i < text.Length; i++)
        {
            // Check if we are at the start of a potential rich text tag
            if (text[i] == '<' && !insideTag)
            {
                // Check if this is actually a valid tag by finding the closing '>'
                int closingTagIndex = text.IndexOf('>', i);

                // If a '>' exists and there is a valid closing tag, treat it as a rich text tag
                if (closingTagIndex != -1 && IsRichTextTag(text, i, closingTagIndex))
                {
                    insideTag = true;
                    i = closingTagIndex - 1;  // Skip the entire tag
                    continue;  // Move to the next character after the tag
                }
            }

            // If not inside a tag, treat it as a visible character
            if (!insideTag)
            {
                visibleCharCount++;

                // If the visible character count matches the caret position, return the index
                if (visibleCharCount == caretPosition)
                {
                    return i + 1;
                }
            }

            // End of tag processing
            if (insideTag && text[i] == '>')
            {
                insideTag = false;  // Exiting the tag
            }
        }

        return -1;  // Return -1 if no match is found
    }

    // Helper function to determine if a sequence between '<' and '>' is a valid rich text tag
    private bool IsRichTextTag(string text, int startIndex, int endIndex)
    {
        // Extract the content inside the '<' and '>'
        string tagContent = text.Substring(startIndex + 1, endIndex - startIndex - 1);

        // Check if this is a valid rich text tag (you can add more valid tags if needed)
        if (tagContent.StartsWith("/") || tagContent.StartsWith("color") || tagContent.StartsWith("b") || tagContent.StartsWith("i"))
        {
            return true;  // This is a valid rich text tag
        }

        // Otherwise, treat it as literal text (e.g., "<" or ">")
        return false;
    }

    private int FindSpaceIndexFromCaret(string rawText, int caretPosition)
    {
        var textBeforeCaret = rawText.Substring(0, caretPosition);
        var lines = textBeforeCaret.Split('\n');
        var remainCaretPosition = caretPosition - 1;
        for (int i = 0; i < lines.Length; i++)
        {
            var missingEnterSignIndex = i == lines.Length - 1 ? 0 : 1;
            remainCaretPosition -= (lines[i].Length - missingEnterSignIndex);
        }
        return Mathf.Max(remainCaretPosition - 1, 0);
    }
}
