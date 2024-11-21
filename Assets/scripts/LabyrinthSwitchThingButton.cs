using UnityEngine;
using UnityEngine.UI;

public class LabyrinthSwitchThingButton : MonoBehaviour
{
    public LabyrinthThings thing;

    [SerializeField]
    LabyrinthThings _switchTo;

    [SerializeField]
    Button thingButton;

    [SerializeField]
    LabyrinthSwitchThingButton _switchToButton;

    void Start()
    {
        if (thingButton)
        {
            thingButton.onClick.AddListener(Switch);
        }
    }

    void Switch()
    {
        LabyrinthSettings.SwitchThing(_switchTo);
        gameObject.SetActive(false);
        if (_switchToButton)
        {
            _switchToButton.gameObject.SetActive(true);
        }
    }
}
