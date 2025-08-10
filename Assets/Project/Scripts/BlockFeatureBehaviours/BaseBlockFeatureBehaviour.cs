using UnityEngine;

[ExecuteAlways]
public abstract class BaseBlockFeatureBehaviour : MonoBehaviour
{
    protected BlockBehaviour _block;
    protected BaseBlockFeatureData _data;

    public void SetData(BaseBlockFeatureData data)
    {
        _data = data;
        OnDataAssigned();
    }

    protected virtual void OnDataAssigned() { }

    public abstract void Apply(BlockBehaviour block);
}
