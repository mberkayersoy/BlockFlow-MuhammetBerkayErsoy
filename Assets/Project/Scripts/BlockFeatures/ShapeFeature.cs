using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeFeature : BaseBlockFeature
{
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    public override void Apply(BlockBehaviour block)
    {
        _meshRenderer = block.Model.GetComponent<MeshRenderer>();
        _meshFilter = block.Model.GetComponent<MeshFilter>();
    }
    public void SetMaterial(Material material)
    {
        _meshRenderer.material = material;
    }
    public void SetMesh(Mesh mesh)
    {
        _meshFilter.sharedMesh = mesh;
    }
}
