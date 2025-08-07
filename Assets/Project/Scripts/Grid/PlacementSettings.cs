using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlacementSettings
{
    public Vector3 offset;
    [SerializeField] private Vector3 _rotation;

    public Quaternion Rotation => Quaternion.Euler(_rotation);
}
