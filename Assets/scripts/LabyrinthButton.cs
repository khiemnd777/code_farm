using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LabyrinthButton : MonoBehaviour
{
    [SerializeField]
    Button labyrinthButton;

    [SerializeField]
    Image buttonBackground;

    [SerializeField]
    LabyrinthSwitchThingButton[] thingButtons;

    Color _normalModeColor = Color.white;
    Color _mazeModeColor = Color.yellow;

    void Start()
    {
        if (labyrinthButton)
        {
            labyrinthButton.onClick.AddListener(ToggleMode);
        }

        ShowOrHideThingButtons();
        UpdateButtonColor();
    }

    void ToggleMode()
    {
        LabyrinthSettings.isMazeMode = !LabyrinthSettings.isMazeMode;
        ResetThing();
        ShowOrHideThingButtons();
        UpdateButtonColor();
    }

    void ResetThing()
    {
        if (!LabyrinthSettings.isMazeMode)
        {
            LabyrinthSettings.SwitchThing(LabyrinthThings.wall);
        }
    }

    void ShowOrHideThingButtons()
    {
        if (thingButtons != null && thingButtons.Length > 0)
        {
            if (!LabyrinthSettings.isMazeMode)
            {
                foreach (var thingButton in thingButtons)
                {
                    if (thingButton)
                    {
                        thingButton.gameObject.SetActive(false);
                    }
                }
                return;
            }
            thingButtons.FirstOrDefault(x => x.thing == LabyrinthSettings.thing)?.gameObject.SetActive(true);
        }
    }

    void UpdateButtonColor()
    {
        if (buttonBackground != null)
        {
            buttonBackground.color = LabyrinthSettings.isMazeMode ? _mazeModeColor : _normalModeColor;
        }
    }
}
