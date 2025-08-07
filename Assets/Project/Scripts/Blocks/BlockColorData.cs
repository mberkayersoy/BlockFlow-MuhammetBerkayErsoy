using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockColorData", menuName = "ScriptableObjects/New Block Color Data", order = 1)]
public class BlockColorData : ScriptableObject
{
    public List<ColorData> BlockColors;
}
[System.Serializable]
public class ColorData
{
    public BlockColorType ColorType;
    public Color BlockColor;
    public Material BlockMaterial;
}


