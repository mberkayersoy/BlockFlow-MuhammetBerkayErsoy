using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : OdinEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridGenerator generator = (GridGenerator)target;

        GUILayout.Space(10);
        GUILayout.Label("Grid Controls", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate Grid"))
            generator.GenerateGrid();

        if (GUILayout.Button("Clear Grid"))
            generator.ClearGrid();

        GUILayout.Space(10);
        GUILayout.Label("Grid Cell Editor", EditorStyles.boldLabel);

        var gridField = generator.GetType().GetField("_gridCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (gridField?.GetValue(generator) is GridCell[,] gridCells && gridCells != null)
        {
            GUIStyle cellStyle = new GUIStyle(EditorStyles.popup)
            {
                fixedWidth = 60,
                fixedHeight = 60,
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                fontSize = 10
            };

            int width = generator.Width;
            int height = generator.Height;

            EditorGUILayout.BeginVertical();

            for (int y = height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < width; x++)
                {
                    bool isEdge = x == 0 || x == width - 1 || y == 0 || y == height - 1;

                    if (!isEdge)
                    {
                        Rect cellRect = GUILayoutUtility.GetRect(60, 60, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                        EditorGUI.DrawRect(new Rect(cellRect.x - 1, cellRect.y - 1, cellRect.width + 2, cellRect.height + 2), Color.red);
                        GUIStyle noMargin = new GUIStyle(cellStyle);
                        EditorGUI.BeginChangeCheck();
                        BlockColorType newColorType = (BlockColorType)EditorGUI.EnumPopup(cellRect, gridCells[x, y].ColorType, noMargin);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(generator, "Change Grid Cell Color");
                            gridCells[x, y].ColorType = newColorType;
                            EditorUtility.SetDirty(generator);
                        }
                    }
                    else
                    {
                        DrawCell(gridCells[x, y], cellStyle, generator);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
        else
        {
            GUILayout.Label("Generate the grid to edit cells.", EditorStyles.helpBox);
        }
    }

    private void DrawCell(GridCell cell, GUIStyle cellStyle, GridGenerator generator)
    {
        EditorGUI.BeginChangeCheck();
        BlockColorType newColorType = (BlockColorType)EditorGUILayout.EnumPopup(cell.ColorType, cellStyle, GUILayout.Width(60), GUILayout.Height(60));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(generator, "Change Grid Cell Color");
            cell.ColorType = newColorType;
            EditorUtility.SetDirty(generator);
        }
    }
}
