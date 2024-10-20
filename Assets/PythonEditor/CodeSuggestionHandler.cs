using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CodeSuggestionHandler : MonoBehaviour
{
    public TMP_InputField inputField; // Assign your TMP Input Field here
    public GameObject suggestionPanel; // Assign your Suggestion Panel here
    public RectTransform suggestionPanelTransform; // RectTransform of the panel for positioning
    public GameObject suggestionItemPrefab; // Prefab for individual suggestion items
    public Transform suggestionContent; // Parent for suggestion items in the panel

    private List<GameObject> activeSuggestionItems = new List<GameObject>(); // Store active suggestion items
    private List<string> allSuggestions = new List<string>
    {
        "move_forward", "rotate_clockwise", "rotate_counterclockwise"
    };

    private int selectedIndex = 0;

    void Start()
    {
        inputField.onValueChanged.AddListener(OnTextChanged);
    }

    void Update()
    {
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
                selectedIndex = Mathf.Max(0, selectedIndex - 1);
                UpdateSuggestionHighlight();
                EventSystem.current.SetSelectedGameObject(null);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex = Mathf.Min(activeSuggestionItems.Count - 1, selectedIndex + 1);
                UpdateSuggestionHighlight();
                EventSystem.current.SetSelectedGameObject(null);
            }

            // Select suggestion
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Tab))
            {
                inputField.ActivateInputField();
                SelectSuggestion(selectedIndex);
            }

            // Hide suggestion panel
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                inputField.ActivateInputField();
                HideSuggestionPanel();
            }
        }
    }

    private void OnTextChanged(string text)
    {
        string word = GetCurrentWord(text);
        if (!string.IsNullOrEmpty(word))
        {
            ShowSuggestionPanel(word);
        }
        else
        {
            HideSuggestionPanel();
        }
    }

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
        string strippedText = StripTags(inputField.text);
        int caretPositionIndex = inputField.caretPosition;
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

        selectedIndex = 0;
        UpdateSuggestionHighlight();
    }

    private string StripTags(string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }

    private void UpdateSuggestionHighlight()
    {
        for (int i = 0; i < activeSuggestionItems.Count; i++)
        {
            TMP_Text itemText = activeSuggestionItems[i].GetComponent<TMP_Text>();
            itemText.color = (i == selectedIndex) ? Color.yellow : Color.white;
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
