using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class ShapeFeatureBehaviour : BaseBlockFeatureBehaviour
{
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private BlockColorType _colorType;
    private ShapeFeatureData _shapeData;
    [SerializeField] private LayerMask _blockLayer;

    [SerializeField] private BlockColorData blockColorData;
    [SerializeField] private ShapeToMeshData shapeToMeshData;
    private float _cellSize = 1f;
    private List<BoxCollider> _cellColliderGos = new List<BoxCollider>();

    public List<BoxCollider> CellColliderGos { get => _cellColliderGos; private set => _cellColliderGos = value; }
    public BlockColorType ColorType { get => _colorType; }

    protected override void OnDataAssigned()
    {
        base.OnDataAssigned();
        _shapeData = (ShapeFeatureData)_data;
    }
    public override void Apply(BlockBehaviour block)
    {
        _block = block;
        _colorType = _shapeData.ColorType;

        if (block.Model == null)
            return;

        _meshRenderer = block.Model.GetComponent<MeshRenderer>();
        _meshFilter = block.Model.GetComponent<MeshFilter>();

        if (blockColorData != null && _meshRenderer != null)
        {
            foreach (var colorData in blockColorData.BlockColors)
            {
                if (_colorType == colorData.ColorType)
                {
                    SetMaterial(colorData.BlockMaterial);
                    break;
                }
            }
        }

        if (shapeToMeshData != null && _meshFilter != null)
        {
            foreach (var kv in shapeToMeshData.ShapeToMesh)
            {
                if (kv.Key.Width == _shapeData.Width &&
                    kv.Key.Height == _shapeData.Height &&
                    kv.Key.shape != null &&
                    _shapeData.Shape != null &&
                    kv.Key.shape.SequenceEqual(_shapeData.Shape))
                {
                    SetMesh(kv.Value);
                    break;
                }
            }
        }

        block.Model.transform.localPosition = GetPivotOffsetFromShape();

        CreateCellColliders();
    }
    public Vector3 GetPivotOffsetFromShape()
    {
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        for (int y = 0; y < _shapeData.Height; y++)
        {
            for (int x = 0; x < _shapeData.Width; x++)
            {
                if (!_shapeData.GetCell(x, y)) continue;

                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
            }
        }

        if (minX == int.MaxValue)
            return Vector3.zero;

        int shapeWidth = maxX - minX + 1;
        int shapeHeight = maxY - minY + 1;

        float offsetX = (shapeWidth - 1) * 0.5f * _cellSize;
        float offsetZ = (shapeHeight - 1) * 0.5f * _cellSize;

        return new Vector3(offsetX, 0f, offsetZ);
    }

    private void CreateCellColliders()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
#else
        Destroy(transform.GetChild(i).gameObject);
#endif
        }

        _cellColliderGos.Clear();

        if (_block == null || _shapeData == null) return;

        int blockLayerIndex = Mathf.RoundToInt(Mathf.Log(_blockLayer.value, 2));
        float shrink = 0.02f;

        for (int y = 0; y < _shapeData.Height; y++)
        {
            for (int x = 0; x < _shapeData.Width; x++)
            {
                if (!_shapeData.GetCell(x, y))
                    continue;

                float centerX = x * _cellSize;
                float centerZ = (_shapeData.Height - 1 - y) * _cellSize;
                float centerY = _cellSize * 0.5f;
                Vector3 localCenter = new Vector3(centerX, centerY, centerZ);

                GameObject newCollider = new GameObject($"CellCollider_{x}_{y}");
                newCollider.transform.SetParent(_block.transform);
                newCollider.transform.localPosition = Vector3.zero;
                newCollider.layer = blockLayerIndex;

                var box = newCollider.AddComponent<BoxCollider>();
                box.center = localCenter;

                box.size = new Vector3(_cellSize - shrink, _cellSize, _cellSize - shrink);

                _cellColliderGos.Add(box);
            }
        }

        for (int i = 0; i < _cellColliderGos.Count; i++)
        {
            for (int j = i + 1; j < _cellColliderGos.Count; j++)
            {
                Physics.IgnoreCollision(_cellColliderGos[i], _cellColliderGos[j]);
            }
        }
    }
    public void SetMaterial(Material material)
    {
        if (_meshRenderer != null) _meshRenderer.material = material;
    }
    public void SetMesh(Mesh mesh)
    {
        if (_meshFilter != null) _meshFilter.sharedMesh = mesh;
    }
}
