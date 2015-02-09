using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class GridWindow : EditorWindow
{
    private static readonly string GRID_PATH = "Grids";
    private static readonly float TILE_DIM = 100.0f;

    private int _selectedGridIndex = 0;

    public List<Grid> allGrids;

    private bool _clearSprite = false;

    [MenuItem("Utilities/Grid/" + "Grid Editor")]
    static void Init()
    {
        GridWindow window = (GridWindow)EditorWindow.GetWindow(typeof(GridWindow));
        window.title = "Grid Editor";
        window.allGrids = new List<Grid>();

        foreach (Grid grid in Resources.LoadAll<Grid>(GRID_PATH))
        {
            if (grid.GetType() == typeof(Grid))
            {
               window.allGrids.Add((Grid)grid);
            }
        }
    }

    void OnGUI()
    {
        if (allGrids.Count == 0)
        {
            EditorGUILayout.LabelField("No grids available!");
            return;
        }

        _selectedGridIndex = EditorGUILayout.Popup("Grid to Edit", _selectedGridIndex, allGrids.ToStrings<Grid>());

        Grid selectedGrid = allGrids[_selectedGridIndex];

        Sprite selectedSprite = DrawTileSpriteSelector();

        _clearSprite = EditorGUILayout.ToggleLeft("Clear Tile Mode", _clearSprite);

        DrawGridTiles(selectedGrid, selectedSprite);
    }

    private int _selectedTileTypeIndex = 0;
    private Tile.Orientation _selectedOrientation = Tile.Orientation.Up;

    private Sprite DrawTileSpriteSelector()
    {
        Sprite selectedTileSprite = null;

        TileDirectory tileTypes = (TileDirectory)AssetDatabase.LoadAssetAtPath(TileTypeWindow.FullTileDirectoryPath, typeof(TileDirectory));

        _selectedTileTypeIndex = EditorGUILayout.Popup("Tile to Paint", _selectedTileTypeIndex, tileTypes.TileTypes.ToStrings<TileType>());

        if(tileTypes.TileTypes.Count > 0)
        {
            selectedTileSprite = tileTypes.TileTypes[_selectedTileTypeIndex].TileSprite;
        }

        return selectedTileSprite;
    }

    private void DrawTileSpriteOrientationSelector()
    {
        _selectedOrientation = (Tile.Orientation)EditorGUILayout.EnumPopup("Orientation", _selectedOrientation);
    }

    private Vector2 gridTileScrollPos = new Vector2();

    private void DrawGridTiles(Grid grid, Sprite spriteIfChanged)
    {
        gridTileScrollPos = EditorGUILayout.BeginScrollView(gridTileScrollPos);

        for (int col = 0; col < grid.Width; col++)
        {
            for (int row = 0; row < grid.Height; row++)
            {
                Matrix4x4 origMatrix = GUI.matrix;

                Rect newPos = new Rect(TILE_DIM * col, TILE_DIM * (grid.Height - 1 - row), TILE_DIM, TILE_DIM);

                if (grid.GetTile(row, col).Sprite != null)
                {
                    if (GUI.Button(newPos, grid.GetTile(row, col).Sprite.texture))
                    {
                        if (_clearSprite)
                        {
                            grid.GetTile(row, col).Sprite = null;
                        }
                        else
                        {
                            grid.GetTile(row, col).Sprite = spriteIfChanged;
                            grid.GetTile(row, col).TileOrientation = _selectedOrientation;
                        }

                        EditorUtility.SetDirty(grid.GetTile(row, col));
                        EditorUtility.SetDirty(grid.GetRow(row));
                    }
                }
                else
                {
                    if (GUI.Button(newPos, ""))
                    {
                        if (!_clearSprite)
                        {
                            grid.GetTile(row, col).Sprite = spriteIfChanged;
                            grid.GetTile(row, col).TileOrientation = _selectedOrientation;

                            EditorUtility.SetDirty(grid.GetTile(row, col));
                            EditorUtility.SetDirty(grid.GetRow(row));
                        }
                    }
                }

                GUI.matrix = origMatrix;
            }
        }

        GUILayout.Space(TILE_DIM * grid.Height);

        GUILayoutUtility.EndGroup("Grid");

        EditorGUILayout.EndScrollView();
    }
}
