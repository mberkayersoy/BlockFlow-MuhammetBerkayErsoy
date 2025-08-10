using System;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class GridGenerator : MonoBehaviour
{
    [Title("Grid Settings")]
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 5;


    [Title("Prefabs")]
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GameObject _straightWall;
    [SerializeField] private GameObject _cornerPrefab;

    [Title("Placement Settings")]
    [SerializeField] private PlacementSettings _wallLeft;
    [SerializeField] private PlacementSettings _wallRight;
    [SerializeField] private PlacementSettings _wallTop;
    [SerializeField] private PlacementSettings _wallBottom;
    [SerializeField] private PlacementSettings _cornerBottomLeft;
    [SerializeField] private PlacementSettings _cornerBottomRight;
    [SerializeField] private PlacementSettings _cornerTopLeft;
    [SerializeField] private PlacementSettings _cornerTopRight;

    private LevelData _levelData;
    private GameObject _gridParent;
    private GameObject _wallsParent;
    private float _cellSize = 1f;
    public LevelData GridData => _levelData;
    public Action<LevelData> GridDataChanged;

    // Grid Cell (0,0) is origin.
    public Vector3 GridToWorldPosition(int x, int y)
    {
        float localX = x * _cellSize + _cellSize * 0.5f;
        float localZ = y * _cellSize + _cellSize * 0.5f;

        Vector3 localPos = new Vector3(localX, 0f, localZ);

        if (_gridParent != null)
            return _gridParent.transform.TransformPoint(localPos);

        return transform.TransformPoint(localPos);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 local = (_gridParent != null)
            ? _gridParent.transform.InverseTransformPoint(worldPos)
            : transform.InverseTransformPoint(worldPos);

        int ix = Mathf.FloorToInt(local.x / _cellSize);
        int iy = Mathf.FloorToInt(local.z / _cellSize);

        return new Vector2Int(ix, iy);
    }

    public void GenerateGrid(int width = -1, int height = -1)
    {
        if (width < 0 || height < 0)
        {
            width = _width;
            height = _height;
        }
        ClearGrid();

        _gridParent = new GameObject("GridParent");
        _gridParent.transform.SetParent(this.transform);
        _gridParent.transform.localPosition = Vector3.zero;


        _wallsParent = new GameObject("WallsParent");
        _wallsParent.transform.SetParent(_gridParent.transform);

        _levelData = new LevelData(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 posLocal = new Vector3(
                    x * _cellSize + _cellSize * 0.5f,
                    0f,
                    y * _cellSize + _cellSize * 0.5f
                );


                GameObject cell = Instantiate(_cellPrefab, _gridParent.transform.TransformPoint(posLocal), _cellPrefab.transform.rotation, _gridParent.transform);
                cell.name = $"Cell ({x},{y})";
            }
        }

        PlaceWalls(width, height);
        PlaceCorners(width, height);

        GridDataChanged?.Invoke(_levelData);
    }

    public GameObject GenerateGrid(LevelData gridData)
    {
        int width = gridData.Width;
        int height = gridData.Height;

        ClearGrid();

        _gridParent = new GameObject("GridParent");
        _gridParent.transform.SetParent(this.transform);
        _gridParent.transform.localPosition = Vector3.zero;

        _wallsParent = new GameObject("WallsParent");
        _wallsParent.transform.SetParent(_gridParent.transform);

        _levelData = gridData;

        for (int x = 0; x < gridData.Width; x++)
        {
            for (int y = 0; y < gridData.Height; y++)
            {
                Vector3 posLocal = new Vector3(
                    x * _cellSize + _cellSize * 0.5f,
                    0f,
                    y * _cellSize + _cellSize * 0.5f
                );


                GameObject cell = Instantiate(_cellPrefab, _gridParent.transform.TransformPoint(posLocal), _cellPrefab.transform.rotation, _gridParent.transform);
                cell.name = $"Cell ({x},{y})";
            }
        }

        PlaceWalls(width, height);
        PlaceCorners(width, height);

        GridDataChanged?.Invoke(_levelData);
        return _gridParent;
    }

    public void ClearGrid()
    {
        if (_gridParent != null)
            DestroyImmediate(_gridParent);
    }

    private void PlaceWalls(int width, int height)
    {
        // Bottom & Top walls
        for (int x = 0; x < width; x++)
        {
            float xPos = x + 0.5f;

            Vector3 bottomPos = new Vector3(xPos, 0f, -0.5f) + _wallBottom.offset;
            Instantiate(_straightWall, bottomPos * _cellSize, _wallBottom.Rotation, _wallsParent.transform)
                .name = $"Wall Bottom ({x},-1)";


            Vector3 topPos = new Vector3(xPos, 0f, height + 0.5f) + _wallTop.offset;
            Instantiate(_straightWall, topPos * _cellSize, _wallTop.Rotation, _wallsParent.transform)
                .name = $"Wall Top ({x},{height})";
        }

        // Left & Right walls
        for (int y = 0; y < height; y++)
        {
            float yPos = y + 0.5f;

            Vector3 leftPos = new Vector3(-0.5f, 0f, yPos) + _wallLeft.offset;
            Instantiate(_straightWall, leftPos * _cellSize, _wallLeft.Rotation, _wallsParent.transform)
                .name = $"Wall Left (-1,{y})";

            // Sağ duvar
            Vector3 rightPos = new Vector3(width + 0.5f, 0f, yPos) + _wallRight.offset;
            Instantiate(_straightWall, rightPos * _cellSize, _wallRight.Rotation, _wallsParent.transform)
                .name = $"Wall Right ({width},{y})";
        }
    }

    private void PlaceCorners(int width, int height)
    {
        Vector3[] cornerPositions =
        {
        (new Vector3(-0.5f, 0f, -0.5f) + _cornerBottomLeft.offset) * _cellSize,                // Bottom Left
        (new Vector3(width + 0.5f, 0f, -0.5f) + _cornerBottomRight.offset) * _cellSize,       // Bottom Right
        (new Vector3(-0.5f, 0f, height + 0.5f) + _cornerTopLeft.offset) * _cellSize,          // Top Left
        (new Vector3(width + 0.5f, 0f, height + 0.5f) + _cornerTopRight.offset) * _cellSize   // Top Right
    };

        Quaternion[] cornerRotations =
        {
        _cornerBottomLeft.Rotation,
        _cornerBottomRight.Rotation,
        _cornerTopLeft.Rotation,
        _cornerTopRight.Rotation
    };

        for (int i = 0; i < cornerPositions.Length; i++)
        {
            Instantiate(_cornerPrefab, _gridParent.transform.TransformPoint(cornerPositions[i]),
                cornerRotations[i], _wallsParent.transform)
                .name = $"Corner {i}";
        }
    }
}
