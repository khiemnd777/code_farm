using UnityEngine;


public class Utility
{
    public static Vector3 CameraInBound(Camera camera, BoxCollider bound, Vector3 position)
    {
        return CameraInBound(camera, bound.bounds.min, bound.bounds.max, position);
    }

    public static Vector3 CameraInBound(Camera camera, Vector3 center, float width, float height, Vector3 position)
    {
        var min = new Vector3(center.x - width / 2, center.y - height / 2);
        var max = new Vector3(center.x + width / 2, center.y + height / 2);
        return CameraInBound(camera, min, max, position);
    }

    public static Vector3 CameraInBound(Camera camera, Vector3 min, Vector3 max, Vector3 position)
    {
        var halfHeight = camera.orthographicSize;
        var halfWidth = halfHeight * Screen.width / Screen.height;
        var x = Mathf.Clamp(position.x, min.x + halfWidth, max.x - halfWidth);
        var y = Mathf.Clamp(position.y, min.y + halfHeight, max.y - halfHeight);
        return new Vector3(x, y, position.z);
    }
}
