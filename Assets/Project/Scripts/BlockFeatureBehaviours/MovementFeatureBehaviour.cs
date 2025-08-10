using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteAlways]
public class MovementFeatureBehaviour : BaseBlockFeatureBehaviour
{
    [SerializeField] private List<Sprite> _arrowSprites;
    private SpriteRenderer _spriteRenderer;
    private MovementFeatureData _movementData;
    protected override void OnDataAssigned()
    {
        _movementData = (MovementFeatureData)_data;
    }
    public override void Apply(BlockBehaviour block)
    {
        _block = block;
        _movementData = (MovementFeatureData)_data;
        _movementData.OnPositionChanged += HandlePositionChanged;
    }
    private void HandlePositionChanged(Vector2Int newPos)
    {
    }
    public Vector3 TryDrag(
        Vector2 screenPos,
        Camera cam,
        Plane dragPlane,
        Vector3 dragOffset,
        GridGenerator gridGen,
        LevelGenerator levelGen,
        LayerMask blockLayer,
        float lerpSpeed)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (!dragPlane.Raycast(ray, out float enter)) return _block.transform.position;

        Vector3 hitPoint = ray.GetPoint(enter);
        Vector3 desiredPos = hitPoint + dragOffset;
        var shapeData = _block.BlockData.GetFeature<ShapeFeatureData>();
        // Clamp position to grid boundaries
        desiredPos.x = Mathf.Clamp(desiredPos.x,
            gridGen.GridToWorldPosition(0, 0).x,
            gridGen.GridToWorldPosition(levelGen.LevelData.Width - shapeData.Width, 0).x);
        desiredPos.z = Mathf.Clamp(desiredPos.z,
            gridGen.GridToWorldPosition(0, 0).z,
            gridGen.GridToWorldPosition(0, levelGen.LevelData.Height - shapeData.Height).z);
        desiredPos.y = 0f; // Lock Y to ground

        Vector3 currentPos = _block.transform.position;

        switch (_movementData.MovementAxis)
        {
            case MovementType.Free:
                break;
            case MovementType.Horizontal:
                desiredPos.z = currentPos.z;
                break;
            case MovementType.Vertical:
                desiredPos.x = currentPos.x;
                break;
            case MovementType.Static:
                desiredPos = currentPos;
                break;
        }

        // Movement blocking flags check on X axis
        if ((desiredPos.x > currentPos.x && _block.BlockedPosX) ||
            (desiredPos.x < currentPos.x && _block.BlockedNegX))
            desiredPos.x = currentPos.x;

        // Movement blocking flags check on Z axis
        if ((desiredPos.z > currentPos.z && _block.BlockedPosZ) ||
            (desiredPos.z < currentPos.z && _block.BlockedNegZ))
            desiredPos.z = currentPos.z;

        // Check collisions and get allowed position
        Vector3 allowedPos = GetAllowedMove(desiredPos, blockLayer);

        // Smoothly move block towards allowed position
        _block.transform.position = Vector3.Lerp(currentPos, allowedPos, Time.deltaTime * lerpSpeed);

        return allowedPos;
    }

    public void TryDrop(Vector3 lastAllowedPos, GridGenerator gridGen, LevelGenerator levelGen)
    {
        Vector2Int cell = gridGen.WorldToGrid(lastAllowedPos);
        var shapeData = _block.BlockData.GetFeature<ShapeFeatureData>();
        // Clamp to grid boundaries minus shape size
        cell.x = Mathf.Clamp(cell.x, 0, levelGen.LevelData.Width - shapeData.Width);
        cell.y = Mathf.Clamp(cell.y, 0, levelGen.LevelData.Height - shapeData.Height);

        Vector3 snapPos = gridGen.GridToWorldPosition(cell.x, cell.y);

        // Animate block snapping to grid
        _block.transform.DOMove(snapPos, 0.2f).SetEase(Ease.OutQuad);
        _movementData.Pos = cell;
    }

    private Vector3 GetAllowedMove(Vector3 desiredPos, LayerMask blockLayer)
    {
        var shapeFeature = _block.GetFeature<ShapeFeatureBehaviour>();
        if (shapeFeature == null)
            return desiredPos;

        Vector3 offset = desiredPos - _block.transform.position;
        Vector3 allowedOffset = offset;

        foreach (var col in shapeFeature.CellColliderGos)
        {
            Vector3 dir = offset.normalized;
            float dist = offset.magnitude;
            Vector3 halfExtents = col.bounds.extents;
            Quaternion rot = col.transform.rotation;

            if (Physics.BoxCast(col.bounds.center, halfExtents, dir, out RaycastHit hit, rot, dist, blockLayer))
            {
                if (hit.collider.GetComponentInParent<BlockBehaviour>() != _block)
                {
                    float allowedDist = hit.distance - 0.001f; // Small buffer to avoid clipping
                    if (allowedDist < dist)
                        allowedOffset = dir * Mathf.Min(allowedDist, allowedOffset.magnitude);
                }
            }
        }

        return _block.transform.position + allowedOffset;
    }
}