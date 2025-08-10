using System;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
    public int Width;
    public int Height;
    public GridCell[,] Cells;
    public List<BlockData> BlockDatas = new List<BlockData>();
    public List<GrinderData> GrinderDatas = new List<GrinderData>();

    public LevelData(int width, int height)
    {
        Width = width;
        Height = height;
        Cells = new GridCell[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                Cells[x, y] = new GridCell(x, y);
    }
    public bool IsCellOccupied(int gridX, int gridY, BlockData ignoreBlock = null)
    {
        foreach (var block in BlockDatas)
        {
            if (block == ignoreBlock)
                continue;

            var shapeFeature = block.GetFeature<ShapeFeatureData>();
            var moveFeature = block.GetFeature<MovementFeatureData>();

            if (shapeFeature == null || moveFeature == null)
                continue;

            for (int y = 0; y < shapeFeature.Height; y++)
            {
                for (int x = 0; x < shapeFeature.Width; x++)
                {
                    if (!shapeFeature.GetCell(x, y))
                        continue;

                    int worldX = moveFeature.Pos.x + x;
                    int worldY = moveFeature.Pos.y + (shapeFeature.Height - 1 - y);

                    if (worldX == gridX && worldY == gridY)
                        return true;
                }
            }
        }
        return false;
    }


    public BlockData GetBlockAtCell(int gridX, int gridY)
    {
        foreach (var block in BlockDatas)
        {
            var shapeFeature = block.GetFeature<ShapeFeatureData>();
            var moveFeature = block.GetFeature<MovementFeatureData>();

            if (shapeFeature == null || moveFeature == null)
                continue;

            for (int y = 0; y < shapeFeature.Height; y++)
            {
                for (int x = 0; x < shapeFeature.Width; x++)
                {
                    if (!shapeFeature.GetCell(x, y))
                        continue;

                    int worldX = moveFeature.Pos.x + x;
                    int worldY = moveFeature.Pos.y + (shapeFeature.Height - 1 - y);

                    if (worldX == gridX && worldY == gridY)
                        return block;
                }
            }

        }
        return null;
    }
}
