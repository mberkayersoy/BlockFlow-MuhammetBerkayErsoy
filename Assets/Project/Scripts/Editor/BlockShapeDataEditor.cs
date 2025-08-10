using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeData))]
public class BlockShapeDataEditor : Editor
{
    private ShapeData data;
    private void OnEnable()
    {
        data = (ShapeData)target;
        EnsureShapeIsValid();
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        int newWidth = EditorGUILayout.IntField("Width", data.Width);
        int newHeight = EditorGUILayout.IntField("Height", data.Height);

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
        if (data.shape == null || data.shape.Length != data.Width * data.Height)
        {
            data.Resize(data.Width, data.Height);
        }
    }

    private void DrawGrid()
    {
        for (int y = 0; y < data.Height; y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < data.Width; x++)
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
