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

        if (generator.GridData != null && generator.GridData.Cells != null)
        {
            GridCell[,] gridCells = generator.GridData.Cells;

            GUIStyle cellStyle = new GUIStyle(EditorStyles.label)
            {
                fixedWidth = 60,
                fixedHeight = 60,
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(0, 0, 0, 0),
                fontSize = 10
            };

            int width = generator.GridData.Width;
            int height = generator.GridData.Height;

            int drawWidth = width + 2; // for edges
            int drawHeight = height + 2; // for edges

            EditorGUILayout.BeginVertical();

            for (int y = drawHeight - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < drawWidth; x++)
                {
                    Rect cellRect = GUILayoutUtility.GetRect(60, 60, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

                    bool isCorner =
                        (x == 0 && y == 0) ||
                        (x == 0 && y == drawHeight - 1) ||
                        (x == drawWidth - 1 && y == 0) ||
                        (x == drawWidth - 1 && y == drawHeight - 1);

                    bool isWall = !isCorner && (
                        x == 0 || x == drawWidth - 1 ||
                        y == 0 || y == drawHeight - 1
                    );

                    if (isCorner)
                    {
                        GUI.Label(cellRect, "", cellStyle);
                    }
                    else if (isWall)
                    {
                        EditorGUI.DrawRect(
                            new Rect(cellRect.x - 1, cellRect.y - 1, cellRect.width + 2, cellRect.height + 2),
                            Color.black
                        );
                        GUI.Label(cellRect, "WALL", cellStyle);
                    }
                    else
                    {
                        int gridX = x - 1;
                        int gridY = y - 1;
                        var cell = gridCells[gridX, gridY];
                        EditorGUI.DrawRect(
                            new Rect(cellRect.x - 1, cellRect.y - 1, cellRect.width + 2, cellRect.height + 2),
                            Color.gray
                        );
                        GUI.Label(cellRect, $"({cell.Pos.x},{cell.Pos.y})", cellStyle);
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
}
