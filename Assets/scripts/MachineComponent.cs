using System.Runtime.InteropServices;
using UnityEngine;

public class MachineComponent : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void SendCoroutineComplete(string gameObjectNamePtr, string coroutineNamePtr);

    public Machine machine;
}
