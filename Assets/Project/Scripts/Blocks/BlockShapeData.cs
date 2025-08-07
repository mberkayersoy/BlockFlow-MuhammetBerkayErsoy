using UnityEngine;

[CreateAssetMenu(fileName = "BlockShapeData", menuName = "ScriptableObjects/New Block Shape Data", order = 0)]
public class BlockShapeData : ScriptableObject
{
    public Mesh BlockMesh;
    public int width = 3;
    public int height = 3;
    public bool[] shape;

    public bool GetCell(int x, int y)
    {
        if (shape == null || shape.Length != width * height)
            Resize(width, height);

        return shape[y * width + x];
    }

    public void SetCell(int x, int y, bool value)
    {
        if (shape == null || shape.Length != width * height)
            Resize(width, height);

        shape[y * width + x] = value;
    }

    public void Resize(int newWidth, int newHeight)
    {
        bool[] newShape = new bool[newWidth * newHeight];

        if (shape != null)
        {
            for (int y = 0; y < Mathf.Min(newHeight, height); y++)
            {
                for (int x = 0; x < Mathf.Min(newWidth, width); x++)
                {
                    int oldIndex = y * width + x;
                    int newIndex = y * newWidth + x;

                    if (oldIndex < shape.Length && newIndex < newShape.Length)
                    {
                        newShape[newIndex] = shape[oldIndex];
                    }
                }
            }
        }

        width = newWidth;
        height = newHeight;
        shape = newShape;
    }

}
