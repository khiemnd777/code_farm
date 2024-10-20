using System.Runtime.InteropServices;
using UnityEngine;

public class UnityJSLib : MonoBehaviour
{
    // Import the JavaScript function from the .jslib file
    [DllImport("__Internal")]
    private static extern void sendToUnity(string gameObjectName, string methodName, string argument);

}
