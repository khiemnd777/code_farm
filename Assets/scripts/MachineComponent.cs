using System.Collections;

public class MachineComponent : OneBehaviour
{
    public Machine machine;

    public virtual IEnumerator Remove()
    {
        yield return null;
    }
}
