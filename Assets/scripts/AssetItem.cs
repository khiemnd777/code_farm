using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssetItem : MonoBehaviour
{
    public Image image;

    public Machine machine;

    public TMP_Text nameText;

    [SerializeField]
    PropertiesCanvas _propertiesCanvasPrefab;

    // Update is called once per frame
    void Update()
    {
        nameText.text = machine.name;
    }

    public void OpenPropertiesPanel()
    {
        if (PropertiesCanvasUtils.propertiesCanvas)
        {
            Destroy(PropertiesCanvasUtils.propertiesCanvas.gameObject);
        }

        PropertiesCanvasUtils.propertiesCanvas = Instantiate<PropertiesCanvas>(_propertiesCanvasPrefab);
        PropertiesCanvasUtils.propertiesCanvas.propertiesPanel.machine = machine;
    }
}
