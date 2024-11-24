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
        if (edgeTransform && _obstacleSegmentPrefab)
        {
            var obstaclePosition = new Vector3(edgeTransform.position.x, edgeTransform.position.y, -1f);
            var obstacleSegment = Instantiate<ObstacleSegment>(_obstacleSegmentPrefab, obstaclePosition, edgeTransform.rotation);
            obstacleSegment.obstacleSegmentController = this;
            obstacleSegments.Add(obstacleSegment);
        }
    }

    public void RemoveObstacle(ObstacleSegment obstacleSegment)
    {
        obstacleSegments.Remove(obstacleSegment);
        Destroy(obstacleSegment.gameObject);
    }

    public bool HasObstacleAhead(Transform machineTransform)
    {
        var position = machineTransform.position + machineTransform.right * .5f;
        return obstacleSegments.Any(segment =>
        {
            return Utility.ArePositionsEqual(position, segment.transform.position);
        });
    }
}
