using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class GridCell
{
    public Vector2Int Pos { get; private set; }
    public bool IsEmpty { get => ColorType == BlockColorType.None; }
    public GameObject OccupyingBlock { get; set; }
    public bool IsInteractable { get => ColorType != BlockColorType.None; }
    [EnumToggleButtons]
    public BlockColorType ColorType { get; set; }

    public GridCell(int x, int y)
    {
        Pos = new Vector2Int(x, y);
        OccupyingBlock = null;
        ColorType = BlockColorType.None;
    }
}
