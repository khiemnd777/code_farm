using System.Runtime.InteropServices;
using UnityEngine;

public class SpeedController : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void SendCoroutineComplete(string gameObjectNamePtr, string coroutineNamePtr);
    
    public float initialSpeed = 1.0f;

    void Start()
    {
        SetSpeed(initialSpeed);
    }

    void SetSpeed(float scale)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
