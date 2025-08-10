using UnityEngine;

[System.Serializable]
public class GridCell
{
    public Vector2Int Pos;

    public GridCell(int x, int y)
    {
        Pos = new Vector2Int(x, y);
    }
}

