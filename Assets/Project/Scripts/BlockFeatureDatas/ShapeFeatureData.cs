using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class ShapeFeatureData : BaseBlockFeatureData
{
    public BlockColorType ColorType;
    [ReadOnly] public bool[] Shape;
    [HideInInspector] public int Width;
    [HideInInspector] public int Height;

    public void SyncFromShapeData(bool[] shape, int width, int height)
    {
        Shape = shape;
        Width = width;
        Height = height;
    }

    public bool GetCell(int x, int y)
    {
        if (Shape == null || Shape.Length != Width * Height)
            return false;
        return Shape[y * Width + x];
    }
}
