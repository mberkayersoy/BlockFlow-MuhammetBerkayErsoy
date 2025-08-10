using UnityEngine;
using System;
using Sirenix.OdinInspector;

[Serializable]
public class MovementFeatureData : BaseBlockFeatureData
{
    [ReadOnly][SerializeField] private Vector2Int _pos;
    public MovementType MovementAxis;
    public event Action<Vector2Int> OnPositionChanged;
    public Vector2Int Pos
    {
        get => _pos;
        set
        {
            if (_pos != value)
            {
                _pos = value;
                OnPositionChanged?.Invoke(_pos);
            }
        }
    }
}
