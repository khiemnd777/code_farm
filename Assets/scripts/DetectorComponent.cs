using System.Collections;

public class DetectorComponent : MachineComponent
{
    ObstacleSegmentController _obstacleSegmentController;
    LabyrinthFloorController _labyrinthFloorController;

    void Start()
    {
        _obstacleSegmentController = FindObjectOfType<ObstacleSegmentController>();
        _labyrinthFloorController = FindObjectOfType<LabyrinthFloorController>();
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

        yield return null;
        SendReturnedStringValueCoroutineComplete(this.name, "Detect", "plain");
    }
}
