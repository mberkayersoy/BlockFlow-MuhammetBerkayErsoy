# Block Flow
Color Block Jam style puzzle game template built in Unity.

## What's in the Project?

- **Dynamic Grid** – The board size is fully configurable. You can set any width and height for the grid, and the game will generate the cells accordingly.
- **Drag And Drop System** – A custom block placement handler that snaps pieces to the grid using shape data instead of just raycasts, ensuring accurate placement even for asymmetrical blocks.
- **Shape System** – Shapes are defined using boolean arrays inside `ShapeData` assets. Each shape can have its own size, pivot point, and color.
- **Placement Controls** – Built-in rules prevent blocks from overlapping or going outside the grid boundaries.
- **Custom Level Editor** – An in-Inspector tool (powered by Odin Inspector) for creating, editing, and testing levels directly inside Unity without external tools.
- **Save and Load** – Levels are stored as `.json` files so you can easily save your work and load it later for editing or testing.

---

## How the Editor Works

**Create a Level**
- Set your desired board size in the Inspector in the LevelGenerator.
- Click "Create New Level" to create the grid.

**Load Level**
- There are currently five levels saved in the project. Setting the desired level in the LevelGenerator Inspector and clicking the Load Level button will load the saved level.

**Create Your Block Palette**
- Add a new block template.
- Assign a shape and color.

**Place Block**
- Left-click a cell to place the selected block.
- Right-click to remove one.

**Save / Load**
- Save your design in the `Assets/Levels/` folder.
- Reload later to continue working.
