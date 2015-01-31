using UnityEngine;
using UnityEditor;
using System.Collections;

public class GridToolEditor
{
    private TileType _selectedTileType;

    public TileType SelectedTileType
    {
        get
        {
            return _selectedTileType;
        }

        set
        {
            _selectedTileType = value;
        }
    }

    public static void DrawTools()
    {

    }
}
