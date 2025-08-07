using UnityEngine;

public class ColorFeature : BaseBlockFeature
{
    private MeshRenderer _meshRenderer;
    public override void Apply(BlockBehaviour block)
    {
        _meshRenderer = block.Model.GetComponent<MeshRenderer>();
    }

    public void SetMaterial(Material material)
    {
        _meshRenderer.material = material;
    }
}
