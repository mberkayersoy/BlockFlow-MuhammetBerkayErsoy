using UnityEngine;

[CreateAssetMenu(fileName = "ShapeData", menuName = "ScriptableObjects/New Shape Data", order = 0)]
public class ShapeData : ScriptableObject
{
    public int Width = 3;
    public int Height = 3;
    public bool[] shape;

    public bool GetCell(int x, int y)
    {
        if (shape == null || shape.Length != Width * Height)
            Resize(Width, Height);

        int index = y * Width + x;
        return shape[index];
    }
    public void SetCell(int x, int y, bool value)
    {
        if (shape == null || shape.Length != Width * Height)
            Resize(Width, Height);

        int index = y * Width + x;
        shape[index] = value;
    }
    public void Resize(int newWidth, int newHeight)
    {
        bool[] newShape = new bool[newWidth * newHeight];

        if (shape != null)
        {
            for (int y = 0; y < Mathf.Min(newHeight, Height); y++)
            {
                for (int x = 0; x < Mathf.Min(newWidth, Width); x++)
                {
                    int oldIndex = y * Width + x;
                    int newIndex = y * newWidth + x;

                    if (oldIndex < shape.Length && newIndex < newShape.Length)
                    {
                        newShape[newIndex] = shape[oldIndex];
                    }
                }
            }
        }

        Width = newWidth;
        Height = newHeight;
        shape = newShape;
    }

}
