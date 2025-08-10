using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[Serializable]
public class BlockData
{
    [SerializeField, LabelText("Block Name")]
    private string _blockName = "New Block";

    [OdinSerialize, SerializeReference, InlineProperty, ListDrawerSettings(ShowFoldout = true, DraggableItems = false)]
    [LabelText("Block Features")]
    private List<BaseBlockFeatureData> _blockFeatures = new();
    public IReadOnlyList<BaseBlockFeatureData> BlockFeatures => _blockFeatures;

    public string BlockName
    {
        get => string.IsNullOrWhiteSpace(_blockName) ? "Unnamed Block" : _blockName;
        set => _blockName = value;
    }
    public void AddFeature(BaseBlockFeatureData feature)
    {
        if (feature == null) return;

        var type = feature.GetType();
        if (_blockFeatures.Any(f => f != null && f.GetType() == type))
        {
            return;
        }
        _blockFeatures.Add(feature);
    }

    public bool HasFeature<T>() where T : BaseBlockFeatureData
    {
        return _blockFeatures.OfType<T>().Any();
    }
    public T GetFeature<T>() where T : BaseBlockFeatureData
    {
        return _blockFeatures.OfType<T>().FirstOrDefault();
    }

}
