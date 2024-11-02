using System.Runtime.InteropServices;
using UnityEngine;

public class FocusController : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void SendCoroutineComplete(string gameObjectNamePtr, string coroutineNamePtr);

    Camera _camera;

    Machine _machine;

    public float followSpeed = 2f;

    bool _isFollowing = false;

    Transform _cachedCameraTransform;
    Transform _cachedMachineTransform;

    public CameraMovement cameraMovement;

    void Start()
    {
        _camera = FindAnyObjectByType<Camera>();
        _cachedCameraTransform = _camera.transform;

        if (_machine)
        {
            _cachedMachineTransform = _machine.transform;
        }
    }

    void Update()
    {
        if (!_machine)
        {
            _machine = FindAnyObjectByType<Machine>();
            if (_machine)
            {
                _cachedMachineTransform = _machine.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (_isFollowing && _machine && _cachedMachineTransform)
        {
            var targetPosition = new Vector3(_cachedMachineTransform.position.x, _cachedMachineTransform.position.y, _cachedCameraTransform.position.z);
            _cachedCameraTransform.position = Vector3.Lerp(_cachedCameraTransform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    public void StartFollowing()
    {
        if (cameraMovement)
        {
            cameraMovement.lockCamera = true;
        }
        _isFollowing = true;
    }

    public void StopFollowing()
    {
        if (cameraMovement)
        {
            cameraMovement.lockCamera = false;
        }
        _isFollowing = false;
    }
}
