using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<BlockShapeData> _blockShapeDatas;
    [SerializeField] private BlockColorType _selectedColor;
    [SerializeField] private BlockShapeData _selectedShape;

    public List<BlockShapeData> BlockShapeDatas { get => _blockShapeDatas; set => _blockShapeDatas = value; }
    public BlockColorType SelectedColor { get => _selectedColor; private set => _selectedColor = value; }
    public BlockShapeData SelectedShape { get => _selectedShape; private set => _selectedShape = value; }
}
