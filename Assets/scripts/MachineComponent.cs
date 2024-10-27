using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class MachineComponent : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void SendCoroutineComplete(string gameObjectNamePtr, string coroutineNamePtr);

    public Machine machine;

    public virtual IEnumerator Remove()
    {
        yield return null;
    }
}
