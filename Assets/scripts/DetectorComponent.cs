using System.Collections;
using UnityEngine;

public class DetectorComponent : MachineComponent
{
    ObstacleSegmentController _obstacleSegmentController;

    void Start()
    {
        _obstacleSegmentController = FindObjectOfType<ObstacleSegmentController>();
    }

    protected virtual IEnumerator Detect()
    {
        if (_obstacleSegmentController)
        {
            if (_obstacleSegmentController.HasObstacleAhead(machine.transform))
            {
                yield return null;
                SendReturnedStringValueCoroutineComplete(this.name, "Detect", "wall");
                yield break;
            }
        }

        yield return null;
        SendReturnedStringValueCoroutineComplete(this.name, "Detect", "plain");
    }
}
