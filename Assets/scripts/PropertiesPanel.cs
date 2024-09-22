using TMPro;
using UnityEngine;

public class PropertiesPanel : MonoBehaviour
{
    public Canvas canvas;
    public Machine machine;
    public TMP_InputField nameField;
    public TMP_InputField fileNameField;
    public TMP_InputField typeField;

    void Start()
    {
        if(machine)
        {
            nameField.text = machine.name;
            fileNameField.text = machine.pyExecutedFilePath;
            typeField.text = machine.typeName;
        }
    }

    public void OpenIDE()
    {
        if (machine)
        {
            machine.OpenIDE();
        }
    }

    public void Run()
    {
        if (machine && !machine.isRunning)
        {
            machine.Run();
        }
    }

    public void Stop()
    {
        if (machine && !machine.isReseting)
        {
            machine.Stop();
        }
    }

    public void ClosePropertiesPanel()
    {
        Destroy(canvas.transform.gameObject);
    }
}
