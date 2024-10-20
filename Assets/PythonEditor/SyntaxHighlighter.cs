using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SyntaxHighlighter : MonoBehaviour
{
    public TMP_InputField inputField;
    private bool isUpdating = false;
    private bool completedTransform = false;
    private bool updateFromShiftTab = false;
    private bool updateFromCtrlBackspace = false;
    private bool updateFromEnter = false;
    private bool valueChanged = false;
    private int originalCaretPosition;
    private int originalStringPosition;
    private string tabSpaceStr;

    public GameObject suggestionPanel; // Assign your Suggestion Panel here
    public RectTransform suggestionPanelTransform; // RectTransform of the panel for positioning
    public GameObject suggestionItemPrefab; // Prefab for individual suggestion items
    public Transform suggestionContent; // Parent for suggestion items in the panel

    private List<GameObject> activeSuggestionItems = new List<GameObject>(); // Store active suggestion items
    private List<string> allSuggestions = new List<string>
    {
        "move_forward", "rotate_clockwise", "rotate_counterclockwise"
    };

    private bool suggestionSelected = false;
    private int suggestionSelectedIndex = 0;

    private void Start()
    {
        inputField.onValidateInput = ValidateInput;
        inputField.onValueChanged.AddListener(ValueChanged);
    }

    void Update()
    {
        // Suggestion panel
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Space))
        {
            var strippedText = StripTags(inputField.text);
            var caretPositionIndex = inputField.caretPosition;
            var wordStartPosition = FindCurrentWordStart(strippedText, caretPositionIndex);
            var currentText = strippedText.Substring(wordStartPosition);
            ShowSuggestionPanel(currentText);
        }

        if (suggestionPanel.activeSelf)
        {
            // Navigate suggestions
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                suggestionSelectedIndex = Mathf.Max(0, suggestionSelectedIndex - 1);
                UpdateSuggestionHighlight();
                EventSystem.current.SetSelectedGameObject(null);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                suggestionSelectedIndex = Mathf.Min(activeSuggestionItems.Count - 1, suggestionSelectedIndex + 1);
                UpdateSuggestionHighlight();
                EventSystem.current.SetSelectedGameObject(null);
            }

            // Select suggestion
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Tab))
            {
                inputField.ActivateInputField();
                SelectSuggestion(suggestionSelectedIndex);
            }

            // Hide suggestion panel
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                inputField.ActivateInputField();
                HideSuggestionPanel();
            }
        }

        if (inputField.isFocused)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    HandleShiftTab();
                }
            }

            if (Input.GetKeyDown(KeyCode.Backspace) && Input.GetKey(KeyCode.LeftControl))
            {
                HandleCtrlBackspace();
            }

            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                HandleEnterKey();
            }

            HighlightSyntax();

            try
            {
                if (valueChanged)
                {
                    string word = GetCurrentWord(inputField.text);
                    if (!string.IsNullOrEmpty(word))
                    {
                        if (!suggestionSelected)
                        {
                            ShowSuggestionPanel(word);
                        }
                    }
                    else
                    {
                        HideSuggestionPanel();
                    }
                    suggestionSelected = false;
                }
            }
            catch
            {

            }
            finally
            {
                valueChanged = false;
            }


            if (completedTransform)
            {
                //inputField.caretPosition = originalCaretPosition;
                completedTransform = false;
            }
        }
    }

    private void HandleShiftTab()
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

    private void HandleCtrlBackspace()
    {
        originalCaretPosition = inputField.caretPosition;

        if (originalCaretPosition > 0)
        {
            updateFromCtrlBackspace = true;
            originalStringPosition = GetStringPositionFromCaret(inputField.text, originalCaretPosition);
            int wordBoundaryPosition = FindPreviousWordBoundary(inputField.text, originalStringPosition);
            string newText = inputField.text.Remove(wordBoundaryPosition, originalStringPosition - wordBoundaryPosition);
            originalCaretPosition -= (originalStringPosition - wordBoundaryPosition);
            inputField.text = newText;
            inputField.caretPosition = originalCaretPosition;
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
            updateFromEnter = true;
            var newLineSpace = indexOfTheFirstCharOfLine + 2;
            var spaces = new string(' ', newLineSpace);
            inputField.text = inputField.text.Insert(originalStringPosition, spaces);
            originalCaretPosition += newLineSpace;
            inputField.caretPosition = originalCaretPosition;
        }
    }

    private char ValidateInput(string text, int charIndex, char addedChar)
    {
        // Check if the Tab character is being inserted
        if (addedChar == '\t')
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                Debug.Log("Shift+Tab detected, preventing input.");
                return '\0'; // Prevent Tab character from being added when Shift is held
            }
        }

        if (addedChar == '\\')
        {
            Debug.Log("Esc detected, preventing input.");
            return '\0';
        }

        if (addedChar == ' ')
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                Debug.Log("Ctrl+Tab detected, preventing input.");
                return '\0';
            }
        }

        // Allow the character input normally
        return addedChar;
    }

    private void ValueChanged(string text)
    {
        valueChanged = true;
    }

    private void HighlightSyntax()
    {
        if (!valueChanged) return;

        if (isUpdating) return;

        isUpdating = true;

        // Save the current caret position
        if (!updateFromShiftTab && !updateFromCtrlBackspace && !updateFromEnter)
        {
            originalCaretPosition = inputField.caretPosition;
            originalStringPosition = inputField.stringPosition;
        }
        if (updateFromShiftTab)
        {
            updateFromShiftTab = false;
        }
        if (updateFromCtrlBackspace)
        {
            updateFromCtrlBackspace = false;
        }
        if (updateFromEnter)
        {
            updateFromEnter = false;
        }

        try
        {
            var text = inputField.text;
            // Strip existing <color> tags to avoid nested tags
            string rawText = StripColorTags(text);

            // Replace \t char by two space
            if (originalCaretPosition > 0 && rawText[originalCaretPosition - 1] == '\t')
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

            inputField.text = highlightedText;
            inputField.caretPosition = originalCaretPosition;

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

    private string StripTags(string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
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

    private bool IsRichTextTag(string text, int startIndex, int endIndex)
    {
        string tagContent = text.Substring(startIndex + 1, endIndex - startIndex - 1);

        if (tagContent.StartsWith("/") || tagContent.StartsWith("color") || tagContent.StartsWith("b") || tagContent.StartsWith("i"))
        {
            return true;
        }

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

    // -- Suggestion Panel


    private void ShowSuggestionPanel(string word)
    {
        if (word == "\n")
        {
            word = string.Empty;
        }

        List<string> filteredSuggestions = allSuggestions.FindAll(s => s.StartsWith(word));
        UpdateSuggestionPanel(filteredSuggestions);
        PositionSuggestionPanel();
        suggestionPanel.SetActive(filteredSuggestions.Count > 0);
    }

    private bool IsWordCharacter(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }

    private void PositionSuggestionPanel()
    {
        string strippedText = StripColorTags(inputField.text);
        int caretPositionIndex = originalCaretPosition;
        int wordStartPosition = FindCurrentWordStart(strippedText, caretPositionIndex);

        TMP_TextInfo textInfo = inputField.textComponent.textInfo;

        if (wordStartPosition < 0 || wordStartPosition >= textInfo.characterCount)
        {
            return;
        }

        TMP_CharacterInfo wordCharInfo = textInfo.characterInfo[wordStartPosition];
        Vector2 wordPosition = wordCharInfo.bottomLeft;
        Vector2 worldWordPosition = inputField.textComponent.transform.TransformPoint(wordPosition);
        Vector2 localWordPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            suggestionPanelTransform.parent as RectTransform,
            worldWordPosition,
            inputField.textComponent.canvas.worldCamera,
            out localWordPosition
        );

        suggestionPanelTransform.localPosition = localWordPosition + new Vector2(0, -5);
    }

    private int FindCurrentWordStart(string text, int caretPosition)
    {
        int wordStartPosition = caretPosition;

        while (wordStartPosition > 0 && IsWordCharacter(text[wordStartPosition - 1]))
        {
            wordStartPosition--;
        }

        return wordStartPosition;
    }

    private void UpdateSuggestionPanel(List<string> filteredSuggestions)
    {
        foreach (var item in activeSuggestionItems)
        {
            Destroy(item);
        }
        activeSuggestionItems.Clear();

        for (int i = 0; i < filteredSuggestions.Count; i++)
        {
            GameObject suggestionItem = Instantiate(suggestionItemPrefab, suggestionContent);
            TMP_Text itemText = suggestionItem.GetComponent<TMP_Text>();
            itemText.text = filteredSuggestions[i];
            int index = i;
            suggestionItem.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectSuggestion(index));
            activeSuggestionItems.Add(suggestionItem);
        }

        suggestionSelectedIndex = 0;
        UpdateSuggestionHighlight();
    }

    private void UpdateSuggestionHighlight()
    {
        for (int i = 0; i < activeSuggestionItems.Count; i++)
        {
            TMP_Text itemText = activeSuggestionItems[i].GetComponent<TMP_Text>();
            itemText.color = (i == suggestionSelectedIndex) ? Color.yellow : Color.white;
        }
    }

    private string GetCurrentWord(string text)
    {
        int caretPos = inputField.stringPosition;
        int startPos = caretPos;

        while (startPos > 0 && IsWordCharacter(text[startPos - 1]))
        {
            startPos--;
        }

        return text.Substring(startPos, caretPos - startPos);
    }

    private void SelectSuggestion(int index)
    {
        string suggestion = activeSuggestionItems[index].GetComponent<TMP_Text>().text;
        string currentText = inputField.text;
        int wordStartPos = GetCurrentWordStartPos();
        inputField.text = currentText.Substring(0, wordStartPos) + suggestion + currentText.Substring(inputField.stringPosition);
        inputField.caretPosition = wordStartPos + suggestion.Length;
        HideSuggestionPanel();
        suggestionSelected = true;
    }

    private void HideSuggestionPanel()
    {
        suggestionPanel.SetActive(false);
    }

    private int GetCurrentWordStartPos()
    {
        int caretPos = inputField.stringPosition;
        int startPos = caretPos;

        while (startPos > 0 && IsWordCharacter(inputField.text[startPos - 1]))
        {
            startPos--;
        }

        return startPos;
    }
}
