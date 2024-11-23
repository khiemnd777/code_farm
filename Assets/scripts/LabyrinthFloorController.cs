using System;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthFloorController : MonoBehaviour
{
    [NonSerialized]
    public List<LabyrinthFloor> floors = new List<LabyrinthFloor>();

    [SerializeField]
    LabyrinthFloor _floorPrefab;

    public void GenerateFloor(Transform floorHighlightTransform)
    {
        if (floorHighlightTransform && _floorPrefab)
        {
            var floorPosition = new Vector3(floorHighlightTransform.position.x, floorHighlightTransform.position.y, .5f);
            var floorIns = Instantiate<LabyrinthFloor>(_floorPrefab, floorPosition, floorHighlightTransform.rotation);
            floorIns.floorController = this;
            floors.Add(floorIns);
        }
    }

    public void RemoveFloor(LabyrinthFloor floor)
    {
        floors.Remove(floor);
        Destroy(floor.gameObject);
    }
}
