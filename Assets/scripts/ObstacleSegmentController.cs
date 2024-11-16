using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleSegmentController : MonoBehaviour
{
    [NonSerialized]
    public List<ObstacleSegment> obstacleSegments = new List<ObstacleSegment>();

    [SerializeField]
    ObstacleSegment _obstacleSegmentPrefab;

    public void GenerateObstacle(Transform edgeTransform)
    {
        if (edgeTransform != null && _obstacleSegmentPrefab != null)
        {
            var edgePosition = edgeTransform.position;
            edgePosition.z = -1f;
            var obstacleSegment = Instantiate<ObstacleSegment>(_obstacleSegmentPrefab, edgePosition, edgeTransform.rotation);
            obstacleSegment.obstacleSegmentController = this;
            obstacleSegments.Add(obstacleSegment);
        }
    }

    public void RemoveObstacle(ObstacleSegment obstacleSegment)
    {
        obstacleSegments.Remove(obstacleSegment);
        Destroy(obstacleSegment.gameObject);
    }

    public bool HasObstacle(Transform machineTransform)
    {
        var position = machineTransform.position + machineTransform.rotation * (Vector3.right / 2);
        var field = FieldUtils.ToField(position);
        return obstacleSegments.Any(segment =>
        {
            var obstacleField = FieldUtils.ToField(segment.transform.position);
            return field.x == obstacleField.y && field.y == obstacleField.y;
        });
    }
}