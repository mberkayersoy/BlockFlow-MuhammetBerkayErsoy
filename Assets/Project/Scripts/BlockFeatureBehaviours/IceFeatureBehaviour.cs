using TMPro;
using UnityEngine;
[ExecuteAlways]
public class IceFeatureBehaviour : BaseBlockFeatureBehaviour
{
    [SerializeField] private Material _iceMaterial;
    [SerializeField] private TextMeshProUGUI _revealText;
    [SerializeField] private Transform _canvas;

    private IceFeatureData _iceData;
    public override void Apply(BlockBehaviour block)
    {
        _block = block;
        _iceData = (IceFeatureData)_data;
        var shapeFeature = _block.GetFeature<ShapeFeatureBehaviour>();
        shapeFeature.SetMaterial(_iceMaterial);
        _revealText.text = _iceData.RevealCount.ToString();

        Vector3 pivotOffset = shapeFeature.GetPivotOffsetFromShape();
        pivotOffset.y = 1.1f;
        _canvas.localPosition = pivotOffset;
    }
    protected override void OnDataAssigned()
    {
        _iceData = (IceFeatureData)_data;
    }

    public void Reveal()
    {
        _iceData.RevealCount--;
        if (_iceData.RevealLeft <= 0)
        {
            AnimateIceBreaking();
        }
    }

    public void AnimateIceBreaking()
    {
        // To do: Ice Breaking particles
    }

    public void AnimateText()
    {
        // To do: Do Tween scale animation
    }
}
