using UnityEngine;

public class GeneralSettings : MonoBehaviour
{
    public float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        // disable WebGLInput.captureAllKeyboardInput so elements in web page can handle keyboard inputs
        WebGLInput.captureAllKeyboardInput = false;
#endif
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}
