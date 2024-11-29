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

    [SerializeField]
    FieldGrid _fieldGrid;

    void Start()
    {
        if (thingButton)
        {
            thingButton.onClick.AddListener(Switch);
        }
    }

    void Switch()
    {
        LabyrinthSettings.SwitchThing(_switchTo, (thing) =>
        {
            switch (thing)
            {
                case LabyrinthThings.wall:
                    {
                        if (_fieldGrid)
                        {
                            _fieldGrid.fieldTiles.ForEach(tile =>
                            {
                                foreach (var edgeHighlighter in tile.edgeHighlighters)
                                {
                                    edgeHighlighter.boxCollider2D.enabled = true;
                                }
                            });
                        }
                    }
                    break;
                case LabyrinthThings.floor:
                    {
                        if (_fieldGrid)
                        {
                            _fieldGrid.fieldTiles.ForEach(tile =>
                            {
                                foreach (var edgeHighlighter in tile.edgeHighlighters)
                                {
                                    edgeHighlighter.boxCollider2D.enabled = false;
                                }
                            });
                        }
                    }
                    break;
            }
        });
        gameObject.SetActive(false);
        if (_switchToButton)
        {
            _switchToButton.gameObject.SetActive(true);
        }
    }
}
