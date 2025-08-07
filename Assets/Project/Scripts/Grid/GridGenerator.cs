using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public partial class GridGenerator : MonoBehaviour
{
    [Title("Grid Settings")]
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 5;

    [Title("Prefabs")]
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private GameObject _cornerPrefab;
    [SerializeField] private GameObject _smallGrinderPrefab; // 1x1
    [SerializeField] private GameObject _mediumGrinderPrefab; // 2x1
    [SerializeField] private GameObject _largeGrinderPrefab; // 3x1
    [Title("Placement Settings")]
    [SerializeField] private PlacementSettings _wallLeft;
    [SerializeField] private PlacementSettings _wallRight;
    [SerializeField] private PlacementSettings _wallTop;
    [SerializeField] private PlacementSettings _wallBottom;
    [SerializeField] private PlacementSettings _cornerBottomLeft;
    [SerializeField] private PlacementSettings _cornerBottomRight;
    [SerializeField] private PlacementSettings _cornerTopLeft;
    [SerializeField] private PlacementSettings _cornerTopRight;

    private GridCell[,] _gridCells;
    private GameObject _gridParent;
    private GameObject _wallsParent;

    public int Width { get => _width; }
    public int Height { get => _height; }

    void Awake()
    {
        GenerateGrid();
    }
    public void GenerateGrid()
    {
        ClearGrid();

        _gridParent = new GameObject("GridParent");
        _wallsParent = new GameObject("WallsParent");
        _wallsParent.transform.SetParent(_gridParent.transform);
        _gridParent.transform.localPosition = Vector3.zero;

        _gridCells = new GridCell[_width, _height];

        // Create Cells
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector3 position = new Vector3(
                    x - _width / 2f + 0.5f,
                    0f,
                    y - _height / 2f + 0.5f
                );

                GameObject cell = Instantiate(_cellPrefab, position, _cellPrefab.transform.rotation, _gridParent.transform);
                cell.name = $"Cell ({x},{y})";

                _gridCells[x, y] = new GridCell(x, y)
                {
                    OccupyingBlock = cell
                };
            }
        }

        PlaceWalls();
        PlaceCorners();
    }
    public void ClearGrid()
    {
        if (_gridParent != null)
        {
            DestroyImmediate(_gridParent);
        }
        _gridCells = null;
    }

    private void PlaceWalls()
    {
        float left = -_width / 2f;
        float right = _width / 2f;
        float bottom = -_height / 2f;
        float top = _height / 2f;

        for (int x = 0; x < _width; x++)
        {
            float xPos = x - _width / 2f + 0.5f;

            // Bottom Edge
            Vector3 bottomPos = new Vector3(xPos, 0f, bottom - 0.5f) + _wallBottom.offset;
            Instantiate(_wallPrefab, bottomPos, _wallBottom.Rotation, _wallsParent.transform).name = $"Wall Bottom ({x},-1)";

            // Top Edge
            Vector3 topPos = new Vector3(xPos, 0f, top + 0.5f) + _wallTop.offset;
            Instantiate(_wallPrefab, topPos, _wallTop.Rotation, _wallsParent.transform).name = $"Wall Top ({x},{_height})";
        }

        for (int y = 0; y < _height; y++)
        {
            float yPos = y - _height / 2f + 0.5f;

            // Left Edge
            Vector3 leftPos = new Vector3(left - 0.5f, 0f, yPos) + _wallLeft.offset;
            Instantiate(_wallPrefab, leftPos, _wallLeft.Rotation, _wallsParent.transform).name = $"Wall Left (-1,{y})";

            // Right Edge
            Vector3 rightPos = new Vector3(right + 0.5f, 0f, yPos) + _wallRight.offset;
            Instantiate(_wallPrefab, rightPos, _wallRight.Rotation, _wallsParent.transform).name = $"Wall Right ({_width},{y})";
        }
    }

    private void PlaceCorners()
    {
        float left = -_width / 2f;
        float right = _width / 2f;
        float bottom = -_height / 2f;
        float top = _height / 2f;

        Vector3[] cornerPositions =
        {
        new Vector3(left - 0.5f, 0f, bottom - 0.5f) + _cornerBottomLeft.offset,  // Bottom Left
        new Vector3(right + 0.5f, 0f, bottom - 0.5f) + _cornerBottomRight.offset, // Bottom Right
        new Vector3(left - 0.5f, 0f, top + 0.5f) + _cornerTopLeft.offset,         // Top Left
        new Vector3(right + 0.5f, 0f, top + 0.5f) + _cornerTopRight.offset,       // Top Right
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
            GameObject corner = Instantiate(_cornerPrefab, cornerPositions[i], cornerRotations[i], _wallsParent.transform);
            corner.name = $"Corner {i}";
        }
    }

}