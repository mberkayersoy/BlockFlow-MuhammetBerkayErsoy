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


<img width="748" height="119" alt="Screenshot 2025-08-10 at 21 44 15" src="https://github.com/user-attachments/assets/f2f78cbc-f27b-407a-b5f8-c2cb23b290df" />

<img width="465" height="461" alt="Screenshot 2025-08-10 at 21 44 01" src="https://github.com/user-attachments/assets/75cd9364-4786-4ed5-8c57-ebb50ff2ca71" />

<img width="303" height="486" alt="Screenshot 2025-08-10 at 21 43 48" src="https://github.com/user-attachments/assets/01706e0d-44b4-40c8-b77c-fb9bc6d125d7" />

<img width="753" height="303" alt="Screenshot 2025-08-10 at 21 43 36" src="https://github.com/user-attachments/assets/0929d6e5-45bd-46e4-8dd2-7296bbc411e8" />

<img width="753" height="586" alt="Screenshot 2025-08-10 at 21 43 25" src="https://github.com/user-attachments/assets/fba0c5f9-5484-4b45-9ed2-eb213ec70c17" />

<img width="753" height="80" alt="Screenshot 2025-08-10 at 21 43 16" src="https://github.com/user-attachments/assets/e80d63a1-5790-4012-bc66-bfe9c6e6d2d2" />











