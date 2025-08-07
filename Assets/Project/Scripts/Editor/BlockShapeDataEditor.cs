using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockShapeData))]
public class BlockShapeDataEditor : Editor
{
    private BlockShapeData data;

    SerializedProperty blockMeshProp;

    private void OnEnable()
    {
        data = (BlockShapeData)target;
        blockMeshProp = serializedObject.FindProperty("BlockMesh");

        EnsureShapeIsValid();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(blockMeshProp);

        EditorGUI.BeginChangeCheck();

        int newWidth = EditorGUILayout.IntField("Width", data.width);
        int newHeight = EditorGUILayout.IntField("Height", data.height);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(data, "Resize Shape");
            data.Resize(newWidth, newHeight);
        }

        EditorGUILayout.Space();

        EnsureShapeIsValid();
        DrawGrid();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);
        }

        serializedObject.ApplyModifiedProperties();
    }


    private void EnsureShapeIsValid()
    {
        if (data.shape == null || data.shape.Length != data.width * data.height)
        {
            data.Resize(data.width, data.height);
        }
    }

    private void DrawGrid()
    {
        for (int y = 0; y < data.height; y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < data.width; x++)
            {
                bool current = data.GetCell(x, y);
                GUI.backgroundColor = current ? Color.green : Color.red;

                if (GUILayout.Button("", GUILayout.Width(25), GUILayout.Height(25)))
                {
                    Undo.RecordObject(data, "Toggle Cell");
                    data.SetCell(x, y, !current);
                }
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }
    }
}
