using UnityEngine;

public class FieldGrid : MonoBehaviour
{
    [SerializeField]
    ObstacleSegmentController _obstacleSegmentController;

    [SerializeField]
    int _initialWidth;

    [SerializeField]
    int _initialHeight;

    [SerializeField]
    FieldTile _fieldTilePrefab;

    [SerializeField]
    FieldTile _fieldTileLastPrefab;

    [SerializeField]
    FieldTile _fieldTileLastHPrefab;

    [SerializeField]
    FieldTile _fieldTileLastVPrefab;

    [SerializeField]
    FieldTick _tickHPrefab;

    [SerializeField]
    FieldTick _tickVPrefab;

    public int initialWidth
    {
        get => _initialWidth;
    }

    public int initialHeight
    {
        get => _initialHeight;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < _initialWidth; x++)
        {
            var fieldTickH = Instantiate<FieldTick>(_tickHPrefab, new Vector3(x, 1), Quaternion.identity, this.transform);
            fieldTickH.tick = x;

            for (int y = 0; y < _initialHeight; y++)
            {
                if (x == 0)
                {
                    var fieldTickV = Instantiate<FieldTick>(_tickVPrefab, new Vector3(-1, -y), Quaternion.identity, this.transform);
                    fieldTickV.tick = y;
                }

                FieldTile fieldTile;

                if (x == _initialWidth - 1 && y < _initialHeight - 1)
                {
                    fieldTile = Instantiate<FieldTile>(_fieldTileLastHPrefab, new Vector3(x, -y), Quaternion.identity, this.transform);
                }
                else if (x < _initialWidth - 1 && y == _initialHeight - 1)
                {
                    fieldTile = Instantiate<FieldTile>(_fieldTileLastVPrefab, new Vector3(x, -y), Quaternion.identity, this.transform);
                }
                else if (x == _initialWidth - 1 && y == _initialHeight - 1)
                {
                    fieldTile = Instantiate<FieldTile>(_fieldTileLastPrefab, new Vector3(x, -y), Quaternion.identity, this.transform);
                }
                else
                {
                    fieldTile = Instantiate<FieldTile>(_fieldTilePrefab, new Vector3(x, -y), Quaternion.identity, this.transform);
                }
                fieldTile.field = new Field
                {
                    x = x,
                    y = y,
                };
                if (_obstacleSegmentController)
                {
                    foreach (var edgeHighlighter in fieldTile.edgeHighlighters)
                    {
                        edgeHighlighter.obstacleSegmentController = _obstacleSegmentController;
                    }
                }
            }
        }
    }
}
