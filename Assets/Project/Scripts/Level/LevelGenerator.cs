using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Rendering;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BlockBehaviour _blockPrefab;
    [SerializeField] private GridGenerator _gridGen;
    [SerializeField] private List<FeaturePrefabMapping> _featurePrefabList;
    [SerializeField] private SerializedDictionary<BlockColorType, Color> _editorColors;

    [Header("Level Info")]
    [SerializeField] private string _levelName = "Level_01";

    [Header("Grid Size")]
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 5;

    [Header("Block Palette")]
    [SerializeField] private List<BlockData> _blockPalette = new();

    [Header("Grinder Palette")]
    [SerializeField] private List<GrinderData> _grinderPalette = new();

    [System.NonSerialized] public int SelectedBlockPalette = -1;
    [System.NonSerialized] public int SelectedGrinderPalette = -1;
    [System.NonSerialized] private LevelData _levelData;

    [Header("Shape Assets")]
    [SerializeField] private List<ShapeData> _availableShapes = new();
    public List<ShapeData> AvailableShapes => _availableShapes;

#if UNITY_EDITOR
    [HideInInspector] public ShapeData EditorSelectedShapeAsset;
#endif

    private Dictionary<BlockData, BlockBehaviour> _map = new();
    private GameObject _levelParent;
    private GameObject _blockParent;

    public LevelData LevelData => _levelData;
    public IReadOnlyList<BlockData> BlockPalette => _blockPalette ??= new();

    #region GridData Access
    public bool HasGridData => _levelData?.Cells != null;
    public GridCell[,] GetGridCells() => _levelData?.Cells;
    public int GetGridWidth() => _levelData?.Width ?? 0;
    public int GetGridHeight() => _levelData?.Height ?? 0;
    #endregion

    #region Properties
    public int Width { get => _width; set => _width = value; }
    public int Height { get => _height; set => _height = value; }
    #endregion

    #region Palette Management
    public BlockData AddNewBlockTemplate()
    {
        var blockData = new BlockData();
        _blockPalette.Add(blockData);
        return blockData;
    }
    public void RemoveBlockTemplateAt(int index)
    {
        if (index < 0 || index >= _blockPalette.Count) return;
        _blockPalette.RemoveAt(index);
    }
    #endregion

    #region Editor Level Helpers

    private void CreateLevelParents()
    {
        _levelParent = new GameObject("LevelParent");
        _blockParent = new GameObject("BlocksParent");
        _blockParent.transform.SetParent(_levelParent.transform);
    }

    public void CreateEmptyLevel(int width, int height)
    {
        CreateLevelParents();
        _levelData = new LevelData(width, height);
        _gridGen.GenerateGrid(_levelData).transform.SetParent(_levelParent.transform);
    }

    public void DeleteLevel()
    {
        _levelData = null;
        _gridGen.ClearGrid();
        _map.Clear();

        if (_levelParent != null)
            DestroyImmediate(_levelParent);
    }
    #endregion

    #region Block Placement
    public void PlaceBlockAt(int gridX, int gridY, int paletteIndex)
    {
        if (!CanPlaceBlock(paletteIndex, out var blockTemplate)) return;

        var newBlockInstance = CloneBlock(blockTemplate);
        OverrideShapeIfEditorSelected(newBlockInstance);

        var shapeFeature = newBlockInstance.GetFeature<ShapeFeatureData>();
        if (shapeFeature == null || !CanFitShape(shapeFeature, gridX, gridY)) return;

        var moveFeature = newBlockInstance.GetFeature<MovementFeatureData>();
        if (moveFeature != null)
            moveFeature.Pos = new Vector2Int(gridX, gridY);

        _levelData.BlockDatas.Add(newBlockInstance);
        InstantiateBlockVisual(newBlockInstance, gridX, gridY);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private void OverrideShapeIfEditorSelected(BlockData block)
    {
#if UNITY_EDITOR
        if (EditorSelectedShapeAsset == null) return;

        var shapeFeature = block.GetFeature<ShapeFeatureData>();
        if (shapeFeature == null) return;

        shapeFeature.SyncFromShapeData(
            EditorSelectedShapeAsset.shape,
            EditorSelectedShapeAsset.Width,
            EditorSelectedShapeAsset.Height);
#endif
    }

    private void InstantiateBlockVisual(BlockData blockData, int gridX, int gridY)
    {
        if (_blockPrefab == null || _gridGen == null) return;

        Vector3 desiredModelWorldPos = _gridGen.GridToWorldPosition(gridX, gridY);

        var blockObj = Instantiate(_blockPrefab, _blockParent.transform);
        blockObj.transform.localRotation = Quaternion.identity;
        blockObj.transform.localScale = Vector3.one;
        blockObj.transform.position = desiredModelWorldPos;

        RegisterBlock(blockData, blockObj);
        blockObj.InitializeBlockData(blockData, _featurePrefabList);
    }

    public void RemoveBlockAt(int gridX, int gridY)
    {
        if (!HasGridData) return;

        var targetData = _levelData.GetBlockAtCell(gridX, gridY);
        if (targetData == null) return;

        var blockObj = _blockParent.GetComponentsInChildren<BlockBehaviour>()
            .FirstOrDefault(b => ReferenceEquals(b.BlockData, targetData));

        if (blockObj != null)
            DestroyBlockObject(blockObj.gameObject);

        _levelData.BlockDatas.Remove(targetData);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private void DestroyBlockObject(GameObject blockObj)
    {
#if UNITY_EDITOR
        DestroyImmediate(blockObj);
#else
        Destroy(blockObj);
#endif
    }

    public void RegisterBlock(BlockData data, BlockBehaviour behaviour)
    {
        _map[data] = behaviour;
    }

    private bool CanPlaceBlock(int paletteIndex, out BlockData blockTemplate)
    {
        blockTemplate = null;
        if (!HasGridData) return false;
        if (paletteIndex < 0 || paletteIndex >= BlockPalette.Count) return false;

        blockTemplate = BlockPalette[paletteIndex];
        return blockTemplate != null;
    }

    private bool CanFitShape(ShapeFeatureData shapeFeature, int gridX, int gridY)
    {
        for (int y = 0; y < shapeFeature.Height; y++)
        {
            for (int x = 0; x < shapeFeature.Width; x++)
            {
                if (!shapeFeature.GetCell(x, y)) continue;

                int targetX = gridX + x;
                int targetY = gridY + y;

                if (targetX < 0 || targetX >= GetGridWidth() ||
                    targetY < 0 || targetY >= GetGridHeight() ||
                    _levelData.IsCellOccupied(targetX, targetY))
                    return false;
            }
        }
        return true;
    }
    #endregion

    #region Utilities
    private BlockData CloneBlock(BlockData original)
    {
        var bytes = SerializationUtility.SerializeValue(original, DataFormat.Binary);
        return SerializationUtility.DeserializeValue<BlockData>(bytes, DataFormat.Binary);
    }

    public Color GetColor(BlockColorType colorType)
    {
        return _editorColors.TryGetValue(colorType, out var color) ? color : Color.white;
    }
    #endregion

    #region Save & Load

    private JsonSerializerSettings JsonSettings => new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto
    };

    public void SaveLevel()
    {
        if (!HasGridData)
        {
            Debug.LogError("No Level Data!");
            return;
        }

        string folderPath = Path.Combine(Application.dataPath, "Levels");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, $"{_levelName}.json");

        string json = JsonConvert.SerializeObject(_levelData, JsonSettings);
        File.WriteAllText(filePath, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        Debug.Log($"Level saved: {filePath}");
    }

    public void LoadLevel(string levelName)
    {
        string folderPath = Path.Combine(Application.dataPath, "Levels");
        string filePath = Path.Combine(folderPath, $"{levelName}.json");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"Level file not found: {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        _levelData = JsonConvert.DeserializeObject<LevelData>(json, JsonSettings);

        if (_levelData == null)
        {
            Debug.LogError("Level data cant load!");
            return;
        }

        GenerateVisualFromGridData();
    }

    public void LoadLevelInScene(LevelData gridData)
    {
        CreateLevelParents();
        _levelData = gridData;
        _gridGen.GenerateGrid(_levelData).transform.SetParent(_levelParent.transform);
    }

    private void GenerateVisualFromGridData()
    {
        LoadLevelInScene(_levelData);

        foreach (var blockData in _levelData.BlockDatas)
        {
            var moveFeature = blockData.GetFeature<MovementFeatureData>();
            if (moveFeature == null)
                continue;

            InstantiateBlockVisual(blockData, moveFeature.Pos.x, moveFeature.Pos.y);
        }
    }

    #endregion
}

[System.Serializable]
public class FeaturePrefabMapping
{
    public string FeatureTypeName;
    public BaseBlockFeatureBehaviour Prefab;
}
