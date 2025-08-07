using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBlockFeature : MonoBehaviour
{
    protected BlockBehaviour _block;
    public abstract void Apply(BlockBehaviour block);
}
