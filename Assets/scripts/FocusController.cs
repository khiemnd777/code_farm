using System.Runtime.InteropServices;
using UnityEngine;

public class FocusController : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void SendCoroutineComplete(string gameObjectNamePtr, string coroutineNamePtr);

    Camera _camera;

    Machine _machine;

    public float followSpeed = 2.0f;

    bool _isFollowing = false;

    void Start()
    {
        _camera = FindAnyObjectByType<Camera>();
    }

    void Update()
    {
        if (!_machine)
        {
            _machine = FindAnyObjectByType<Machine>();
        }
        if (_isFollowing && _machine)
        {
            var targetPosition = new Vector3(_machine.transform.position.x, _machine.transform.position.y, _camera.transform.position.z);
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    public void StartFollowing()
    {
        _isFollowing = true;
    }

    public void StopFollowing()
    {
        _isFollowing = false;
    }
}
