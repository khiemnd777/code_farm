using System.Collections;
using UnityEngine;

public class DetectorComponent : MachineComponent
{
    ObstacleSegmentController _obstacleSegmentController;
    LabyrinthFloorController _labyrinthFloorController;

    [SerializeField]
    DotComponent _dotComponent;

    void Start()
    {
        _obstacleSegmentController = FindFirstObjectByType<ObstacleSegmentController>();
        _labyrinthFloorController = FindFirstObjectByType<LabyrinthFloorController>();
    }

    protected virtual IEnumerator Detect()
    {
        if (_labyrinthFloorController)
        {
            if (_labyrinthFloorController.ExitAhead(machine.transform))
            {
                yield return null;
                SendReturnedStringValueCoroutineComplete(this.name, "Detect", "exit");
                yield break;
            }
        }
        if (_obstacleSegmentController)
        {
            if (_obstacleSegmentController.HasObstacleAhead(machine.transform))
            {
                yield return null;
                SendReturnedStringValueCoroutineComplete(this.name, "Detect", "wall");
                yield break;
            }
        }
        if (_dotComponent)
        {
            if (_dotComponent.DotAhead(machine.transform))
            {
                yield return null;
                SendReturnedStringValueCoroutineComplete(this.name, "Detect", "dot");
                yield break;
            }
        }

        yield return null;
        SendReturnedStringValueCoroutineComplete(this.name, "Detect", "plain");
    }
}
