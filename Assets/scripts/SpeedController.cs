using UnityEngine;

public class SpeedController : OneBehaviour
{
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
