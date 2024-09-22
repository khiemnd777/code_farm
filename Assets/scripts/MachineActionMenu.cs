using UnityEngine;

public class MachineActionMenu : MonoBehaviour
{
    public Machine machine;

    void Update()
    {
        if (machine)
        {
            transform.position = machine.transform.position;
            //transform.position = Vector3.MoveTowards(transform.position, machine.transform.position, .03f);
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
        if (machine)
        {
            machine.Run();
        }
    }

    public void Stop()
    {
        if (machine)
        {
            machine.Stop();
        }
    }

    public void Close()
    {
        if (machine)
        {
            Destroy(transform.gameObject);
        }
    }
}
