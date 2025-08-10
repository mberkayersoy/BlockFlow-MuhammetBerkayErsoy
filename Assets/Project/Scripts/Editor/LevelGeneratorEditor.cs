using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : OdinEditor
{
    private LevelGenerator levelGen;
    private int paletteSelected = -1;
    protected override void OnEnable()
    {
        base.OnEnable();
        levelGen = (LevelGenerator)target;
        paletteSelected = levelGen?.SelectedBlockPalette ?? -1;

        if (levelGen != null && (paletteSelected < 0 || paletteSelected >= levelGen.BlockPalette.Count))
            paletteSelected = -1;
    }

    public override void OnInspectorGUI()
    {
        try
        {
            base.OnInspectorGUI();
            if (levelGen == null)
                levelGen = (LevelGenerator)target;
            if (levelGen == null)
                return;
            DrawPaletteControls();
            DrawPaletteButtons();
            DrawSelectedPaletteDetails();
            DrawShapeBlockCreator();

            DrawGridControls();
            DrawGridVisualizer();


            if (GUILayout.Button("Save Level"))
            {
                levelGen.SaveLevel();
            }
            if (GUILayout.Button("Load Level"))
            {
                levelGen.LoadLevel("Level_01");
            }

            if (levelGen.SelectedBlockPalette != paletteSelected)
                levelGen.SelectedBlockPalette = paletteSelected;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"LevelGeneratorEditor.OnInspectorGUI error: {ex}");
        }
    }

    private void DrawShapeBlockCreator()
    {
        SirenixEditorGUI.Title("Select Shape Asset", null, TextAlignment.Left, true);

        if (levelGen.AvailableShapes == null || levelGen.AvailableShapes.Count == 0)
        {
            GUILayout.Label("There is no shape.", EditorStyles.helpBox);
            return;
        }

        int columns = 2;
        int count = 0;

        EditorGUILayout.BeginHorizontal();
        foreach (var shapeData in levelGen.AvailableShapes)
        {
            if (shapeData == null) continue;

            bool isSelected = levelGen.EditorSelectedShapeAsset == shapeData;

            Color originalColor = GUI.backgroundColor;
            if (isSelected)
                GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);

            GUILayout.BeginVertical("box", GUILayout.Width(150));

            GUILayout.Label(shapeData.name, EditorStyles.boldLabel);

            DrawShapePreview(shapeData);

            GUILayout.Space(4);

            if (GUILayout.Button(isSelected ? "Selected" : "Select", GUILayout.Width(60)))
            {
                Undo.RecordObject(levelGen, "Change Selected Shape Asset");
                levelGen.EditorSelectedShapeAsset = shapeData;
                EditorUtility.SetDirty(levelGen);
            }

            GUILayout.EndVertical();
            GUI.backgroundColor = originalColor;

            count++;
            if (count % columns == 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawShapePreview(ShapeData shapeData)
    {
        if (shapeData == null) return;

        int cellSize = 15;
        for (int y = 0; y < shapeData.Height; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < shapeData.Width; x++)
            {
                bool filled = shapeData.GetCell(x, y);
                Color cellColor = filled ? Color.black : Color.gray;

                Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.ExpandWidth(false));
                EditorGUI.DrawRect(rect, cellColor);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    #region UI Draw Methods

    private void DrawPaletteControls()
    {
        GUILayout.Space(8);
        GUILayout.Label("Palette Controls", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add New Block Template"))
        {
            Undo.RecordObject(levelGen, "Add Block Template");
            levelGen.AddNewBlockTemplate();
            EditorUtility.SetDirty(levelGen);
            paletteSelected = levelGen.BlockPalette.Count - 1;
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);
    }

    private void DrawPaletteButtons()
    {
        var palette = levelGen.BlockPalette;
        if (palette == null || palette.Count == 0)
        {
            EditorGUILayout.HelpBox("Pallete is empty. Add new palette temolate.", MessageType.Info);
            return;
        }

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < palette.Count; i++)
        {
            var pd = palette[i];
            string title = pd != null && !string.IsNullOrWhiteSpace(pd.BlockName)
                ? pd.BlockName
                : $"Block {i}";

            string featSummary = "";

            bool toggled = GUILayout.Toggle(
                paletteSelected == i,
                $"{title}\n{featSummary}",
                "Button",
                GUILayout.Width(100), GUILayout.Height(48)
            );

            if (toggled && paletteSelected != i)
            {
                paletteSelected = i;
                levelGen.SelectedBlockPalette = paletteSelected;
                EditorUtility.SetDirty(levelGen);
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(6);
    }


    private void DrawSelectedPaletteDetails()
    {
        var palette = levelGen.BlockPalette;
        if (paletteSelected < 0 || paletteSelected >= palette.Count)
            return;

        EditorGUILayout.BeginVertical("box");
        GUILayout.Label($"Selected: #{levelGen.BlockPalette[paletteSelected].BlockName}", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove"))
        {
            if (EditorUtility.DisplayDialog("Confirm", $"Are you sure you want to delete #{paletteSelected}?", "Yes", "No"))
            {
                Undo.RecordObject(levelGen, "Remove Block Template");
                levelGen.RemoveBlockTemplateAt(paletteSelected);
                paletteSelected = -1;
                levelGen.SelectedBlockPalette = -1;
                EditorUtility.SetDirty(levelGen);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(6);
        EditorGUILayout.HelpBox(
            "Edit the block details in the 'Block Palette' field.",
            MessageType.Info
        );
        EditorGUILayout.EndVertical();
    }

    private void DrawGridControls()
    {
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create New Level"))
        {
            Undo.RecordObject(levelGen, "Create New Level");
            levelGen.CreateEmptyLevel(levelGen.Width, levelGen.Height);
            EditorUtility.SetDirty(levelGen);
        }
        if (GUILayout.Button("Delete Level Temporary Level"))
        {
            Undo.RecordObject(levelGen, "Delete Level Temporary Level");
            levelGen.DeleteLevel();
            EditorUtility.SetDirty(levelGen);
        }
        EditorGUILayout.EndHorizontal();
    }

    #endregion

    #region Helper Methods

    private void DrawGridVisualizer()
    {
        GUILayout.Space(10);
        GUILayout.Label("Grid Preview (Click to place block)", EditorStyles.boldLabel);

        if (!levelGen.HasGridData)
        {
            GUILayout.Label("Grid is empty. Create a grid first.", EditorStyles.helpBox);
            return;
        }

        var cells = levelGen.GetGridCells();
        int width = levelGen.GetGridWidth();
        int height = levelGen.GetGridHeight();

        GUIStyle cellStyle = new GUIStyle(EditorStyles.label)
        {
            fixedWidth = 40,
            fixedHeight = 40,
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0),
            fontSize = 9
        };

        int drawWidth = width + 2;
        int drawHeight = height + 2;

        EditorGUILayout.BeginVertical();
        for (int y = drawHeight - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < drawWidth; x++)
            {
                Rect cellRect = GUILayoutUtility.GetRect(40, 40, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

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
                    EditorGUI.DrawRect(new Rect(cellRect.x - 1, cellRect.y - 1, cellRect.width + 2, cellRect.height + 2), Color.black);
                    GUI.Label(cellRect, "WALL", cellStyle);
                }
                else
                {
                    int gridX = x - 1;
                    int gridY = y - 1;

                    var cell = cells[gridX, gridY];

                    Color cellColor;
                    var gd = levelGen.LevelData;

                    if (gd.IsCellOccupied(gridX, gridY))
                    {
                        var block = gd.GetBlockAtCell(gridX, gridY);
                        var shapeFeature = block?.GetFeature<ShapeFeatureData>();
                        if (shapeFeature != null)
                            cellColor = levelGen.GetColor(shapeFeature.ColorType);
                        else
                            cellColor = Color.white;
                    }
                    else
                    {
                        cellColor = Color.gray;
                    }

                    EditorGUI.DrawRect(new Rect(cellRect.x - 1, cellRect.y - 1, cellRect.width + 2, cellRect.height + 2), cellColor);


                    GUI.Label(cellRect, $"({cell.Pos.x},{cell.Pos.y})", cellStyle);


                    if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.button == 0)
                        {
                            if (paletteSelected >= 0 && paletteSelected < levelGen.BlockPalette.Count)
                            {
                                Undo.RecordObject(levelGen, "Place Block");

                                levelGen.PlaceBlockAt(cell.Pos.x, cell.Pos.y, paletteSelected);
                                EditorUtility.SetDirty(levelGen);
                            }
                        }
                        else if (Event.current.button == 1)
                        {
                            Undo.RecordObject(levelGen, "Remove Block");
                            levelGen.RemoveBlockAt(cell.Pos.x, cell.Pos.y);
                            EditorUtility.SetDirty(levelGen);
                        }
                        Event.current.Use();
                    }
                }

            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
    #endregion
}
