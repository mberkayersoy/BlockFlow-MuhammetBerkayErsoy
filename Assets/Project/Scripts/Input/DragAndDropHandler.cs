using UnityEngine;
using Sirenix.OdinInspector;

public class DragAndDropHandler : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private LevelGenerator _levelGen;
    [SerializeField] private GridGenerator _gridGen;
    [SerializeField] private Camera _cam;
    [SerializeField] private LayerMask _blockLayer;
    [ReadOnly][SerializeField] private BlockData _selectedBlock;
    [ReadOnly][SerializeField] private MovementFeatureData _moveFeature;
    [ReadOnly][SerializeField] private BlockBehaviour _selectedBehaviour;
    [ReadOnly][SerializeField] private ShapeFeatureData _shapeData;
    [SerializeField] private float _lerpSpeed = 15f;

    private Vector3 _dragOffset;
    private Vector2Int _shapeOffsetInGrid;
    private bool _isDragging;
    private bool _dragInitiated;
    private Vector2 _pressStartPos;
    private float _dragThreshold = 5f;
    private Vector3 _lastAllowedPos;
    private Plane _dragPlane;

    private void Awake()
    {
        _inputManager.OnPressStart += TryDetect;
        _inputManager.OnPressEnd += TryDrop;
        _inputManager.OnPosition += TryDrag;
    }

    // Detect block under cursor on press start
    private void TryDetect(Vector2 screenPos)
    {
        _pressStartPos = screenPos;
        _dragInitiated = false;
        _isDragging = false;

        Ray ray = _cam.ScreenPointToRay(screenPos);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, _blockLayer)) return;

        var blockBehaviour = hit.collider.GetComponentInParent<BlockBehaviour>();
        if (blockBehaviour == null) return;

        _selectedBehaviour = blockBehaviour;
        _selectedBlock = blockBehaviour.BlockData;
        _moveFeature = _selectedBlock.GetFeature<MovementFeatureData>();
        _shapeData = _selectedBlock.GetFeature<ShapeFeatureData>();

        Vector2Int blockOrigin = _moveFeature.Pos;
        Vector2Int clickCell = _gridGen.WorldToGrid(hit.point);
        _shapeOffsetInGrid = clickCell - blockOrigin;

        // If clicked cell is outside shape bounds or false cell, find first true cell as offset
        if (_shapeOffsetInGrid.x < 0 || _shapeOffsetInGrid.x >= _shapeData.Width ||
            _shapeOffsetInGrid.y < 0 || _shapeOffsetInGrid.y >= _shapeData.Height ||
            !_shapeData.GetCell(_shapeOffsetInGrid.x, _shapeOffsetInGrid.y))
        {
            bool found = false;
            for (int y = 0; y < _shapeData.Height && !found; y++)
            {
                for (int x = 0; x < _shapeData.Width && !found; x++)
                {
                    if (_shapeData.GetCell(x, y))
                    {
                        _shapeOffsetInGrid = new Vector2Int(x, y);
                        found = true;
                    }
                }
            }
        }

        Vector3 blockOriginWorld = _gridGen.GridToWorldPosition(blockOrigin.x, blockOrigin.y);
        _dragOffset = blockOriginWorld - hit.point;
        _dragPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Handle dragging motion by delegating to MovementFeatureBehaviour
    private void TryDrag(Vector2 screenPos)
    {
        if (_selectedBehaviour == null || _moveFeature == null) return;
        if (!_dragInitiated)
        {
            if (Vector2.Distance(screenPos, _pressStartPos) < _dragThreshold)
                return;
            _dragInitiated = true;
            _isDragging = true;
        }
        if (!_isDragging) return;

        var movementBehaviour = _selectedBehaviour.GetFeature<MovementFeatureBehaviour>();
        if (movementBehaviour != null)
        {
            _lastAllowedPos = movementBehaviour.TryDrag(
                screenPos,
                _cam,
                _dragPlane,
                _dragOffset,
                _gridGen,
                _levelGen,
                _blockLayer,
                _lerpSpeed
            );
        }
    }

    // Handle drop action by delegating to MovementFeatureBehaviour
    private void TryDrop(Vector2 screenPos)
    {
        if (!_isDragging)
        {
            ResetSelection();
            return;
        }
        if (_selectedBehaviour == null || _shapeData == null)
        {
            ResetSelection();
            return;
        }

        var movementBehaviour = _selectedBehaviour.GetFeature<MovementFeatureBehaviour>();
        if (movementBehaviour != null)
        {
            movementBehaviour.TryDrop(_lastAllowedPos, _gridGen, _levelGen);
        }

        ResetSelection();
    }

    // Reset all selections and dragging state
    private void ResetSelection()
    {
        _isDragging = false;
        _selectedBlock = null;
        _moveFeature = null;
        _selectedBehaviour = null;
        _shapeData = null;
        _dragOffset = Vector3.zero;
    }
}