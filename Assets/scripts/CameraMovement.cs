using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    Camera _camera;

    [SerializeField]
    float _speed = 12f;

    [SerializeField]
    float _zoomSpeed = 3f;

    [SerializeField]
    float _zoomMin = 1f;

    [SerializeField]
    float _zoomMax = 20f;

    float cameraX = 0f;
    float cameraY = 0f;

    [SerializeField]
    Boundary _bound;

    BoundingBox _limitedBoundingBox;

    [SerializeField]
    float _dragSpeed = 2;

    Vector3 _dragOrigin;
    Vector3 _differenceMousePosition;

    public bool lockCamera;

    Transform _cacheCameraTransform;

    void Start()
    {
        if (_camera)
        {
            _camera.farClipPlane = 50f;
            _cacheCameraTransform = _camera.transform;
        }
    }
    // Update is called once per frame
    void Update()
    {

        if (_camera)
        {
            if (_limitedBoundingBox.size == Vector3.zero)
            {
                SetLimitedBoundingBox(_bound.boundary);
            }

            if (_limitedBoundingBox.centerTarget)
            {
                // Moving by WASD
                cameraX = Input.GetAxis("Horizontal") * _speed * Time.deltaTime;
                cameraY = Input.GetAxis("Vertical") * _speed * Time.deltaTime;
            }


            // Zooming by mouse scroll
            var orthographicSize = _camera.orthographicSize;
            orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -_zoomSpeed;
            orthographicSize = Mathf.Clamp(orthographicSize, _zoomMin, _zoomMax);
            _camera.orthographicSize = orthographicSize;

            // Drag moving
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                _dragOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
                return;
            }

            if (!Input.GetMouseButton(1) && !Input.GetMouseButton(2)) return;

            Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _differenceMousePosition = mousePosition - new Vector3(cameraX, cameraY);
            cameraX = (_dragOrigin.x - _differenceMousePosition.x) * _dragSpeed * Time.deltaTime;
            cameraY = (_dragOrigin.y - _differenceMousePosition.y) * _dragSpeed * Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        if (!lockCamera)
        {
            _cacheCameraTransform.Translate(new Vector3(cameraX, cameraY), Space.Self);
            var cameraPosition = Utility.CameraInBound(_camera, _limitedBoundingBox.centerTarget.position, _limitedBoundingBox.size.x, _limitedBoundingBox.size.y, _cacheCameraTransform.position);
            _cacheCameraTransform.position = cameraPosition;
        }
    }

    void SetLimitedBoundingBox(BoundingBox limitedBoundingBox)
    {
        _limitedBoundingBox = limitedBoundingBox;
    }

    void SetDefaultLimitedBoundingBox()
    {
        _limitedBoundingBox = _bound.boundary;
    }
}
