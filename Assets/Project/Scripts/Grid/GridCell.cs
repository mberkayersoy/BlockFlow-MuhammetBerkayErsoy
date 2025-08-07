using UnityEngine;

[System.Serializable]
public class GridCell
{
    public Vector2Int Pos { get; private set; }
    public bool IsEmpty { get; set; }
    public GameObject OccupyingBlock { get; set; }

    public GridCell(int x, int y)
    {
        Pos = new Vector2Int(x, y);
        IsEmpty = true;
        OccupyingBlock = null;
    }
}
