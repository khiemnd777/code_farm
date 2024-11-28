using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[Serializable]
public struct Vector3Struct
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class LabyrinthWall
{
    public Vector3Struct[] positions;
    public Vector3Struct[] rotations;
}

[Serializable]
public class LabyrinthExit
{
    public Vector3Struct[] positions;
}

[Serializable]
public class LabyrinthData
{
    public LabyrinthWall wall;
    public LabyrinthExit exit;
}

public class LabyrinthDataController : OneBehaviour
{
    [SerializeField]
    LabyrinthFloorController _labyrinthFloorController;

    [SerializeField]
    ObstacleSegmentController _obstacleSegmentController;

    IEnumerator SetData(string jsonArguments)
    {
        yield return null;
        if (!string.IsNullOrEmpty(jsonArguments))
        {
            var labyrinthData = JsonUtility.FromJson<LabyrinthData>(jsonArguments);
            if (labyrinthData != null)
            {
                for (var i = 0; i < labyrinthData.wall.positions.Count(); i++)
                {
                    var position = labyrinthData.wall.positions.ElementAt(i);
                    var rotation = labyrinthData.wall.rotations.ElementAt(i);
                    _obstacleSegmentController.GenerateObstacle(
                        new Vector3(position.x, position.y, position.z),
                        Quaternion.Euler(new Vector3(rotation.x, rotation.y, rotation.z))
                    );
                    yield return null;
                }

                for (var i = 0; i < labyrinthData.exit.positions.Count(); i++)
                {
                    var position = labyrinthData.exit.positions.ElementAt(i);
                    _labyrinthFloorController.GenerateFloor(
                        new Vector3(position.x, position.y, position.x),
                        Quaternion.identity
                    );
                    yield return null;
                }
            }
        }
        SendCoroutineComplete(this.name, "SetData");
    }

    IEnumerator GetData()
    {
        yield return null;

        var wallSegments = _obstacleSegmentController.obstacleSegments;
        var walls = wallSegments
            .Select(segment =>
            {
                var position = segment.transform.position;
                var rotation = segment.transform.rotation;
                return new
                {
                    position = new Vector3Struct
                    {
                        x = position.x,
                        y = position.y,
                        z = position.z
                    },
                    rotation = new Vector3Struct
                    {
                        x = rotation.eulerAngles.x,
                        y = rotation.eulerAngles.y,
                        z = rotation.eulerAngles.z
                    }
                };
            })
            .ToList();

        var exitSegments = _labyrinthFloorController.floors;
        var exits = exitSegments
            .Select(segment =>
            {
                var position = segment.transform.position;
                return new
                {
                    position = new Vector3Struct
                    {
                        x = position.x,
                        y = position.y,
                        z = position.z
                    }
                };
            })
            .ToList();

        var labyrinthData = new LabyrinthData
        {
            wall = new LabyrinthWall
            {
                positions = walls.Select(wall => wall.position).ToArray(),
                rotations = walls.Select(wall => wall.rotation).ToArray()
            },
            exit = new LabyrinthExit { positions = exits.Select(exit => exit.position).ToArray() }
        };

        var labyrinthDataAsStr = JsonUtility.ToJson(labyrinthData);

        print(labyrinthDataAsStr);

        SendReturnedStringValueCoroutineComplete(this.name, "GetData", labyrinthDataAsStr);
    }
}
