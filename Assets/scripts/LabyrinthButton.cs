using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabyrinthButton : MonoBehaviour
{
    [SerializeField]
    Button labyrinthButton;

    [SerializeField]
    Image buttonBackground;

    Color normalModeColor = Color.white;
    Color mazeModeColor = Color.yellow;


    void Start()
    {
        if (labyrinthButton != null)
        {
            labyrinthButton.onClick.AddListener(ToggleMode);
        }

        UpdateButtonColor();
    }

    void ToggleMode()
    {
        LabyrinthSettings.isMazeMode = !LabyrinthSettings.isMazeMode;
        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        if (buttonBackground != null)
        {
            buttonBackground.color = LabyrinthSettings.isMazeMode ? mazeModeColor : normalModeColor;
        }
    }
}
