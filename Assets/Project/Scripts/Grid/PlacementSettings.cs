using UnityEngine;

[System.Serializable]
public struct PlacementSettings
{
    public Vector3 offset;
    [SerializeField] private Vector3 _rotation;

    public readonly Quaternion Rotation => Quaternion.Euler(_rotation);
}
