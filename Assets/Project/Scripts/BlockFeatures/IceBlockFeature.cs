using TMPro;
using UnityEngine;

public class IceBlockFeature : BaseBlockFeature
{
    [SerializeField] private Material _iceMaterial;
    [SerializeField] private TextMeshProUGUI _revealText;
    [SerializeField] private int _revealCount = 3;
    private int _revealLeft;

    public override void Apply(BlockBehaviour block)
    {
        _block = block;
        _revealLeft = _revealCount;
        var shapeFeature = _block.GetFeature<ShapeFeature>();
        shapeFeature.SetMaterial(_iceMaterial);
    }

    public void Reveal()
    {
        _revealLeft--;
        if (_revealLeft <= 0)
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
