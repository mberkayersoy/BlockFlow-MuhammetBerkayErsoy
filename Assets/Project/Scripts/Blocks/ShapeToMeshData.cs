using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ShapeToMeshData", menuName = "ScriptableObjects/New Shapte To Mesh Data", order = 3)]
public class ShapeToMeshData : SerializedScriptableObject
{
    public SerializedDictionary<ShapeData, Mesh> ShapeToMesh;
}
