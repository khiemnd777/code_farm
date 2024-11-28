using UnityEngine;
using System.Runtime.InteropServices;

public class OneBehaviour : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void SendCoroutineComplete(
        string gameObjectNamePtr,
        string coroutineNamePtr
    );

    [DllImport("__Internal")]
    public static extern void SendReturnedStringValueCoroutineComplete(
        string gameObjectNamePtr,
        string coroutineNamePtr,
        string stringValuePtr
    );
}
