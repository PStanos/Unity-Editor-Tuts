using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Grid : ScriptableObject
{
    public enum ResizeAnchor
    {
        TopLeft,
        Top,
        TopRight,
        Left,
        Middle,
        Right,
        BottomLeft,
        Bottom,
        BottomRight
    }

    [SerializeField]
    private System.Guid _guid;

    public System.Guid Guid
    {
        get
        {
            return _guid;
        }

        set
        {
            _guid = value;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

    [SerializeField]
    private string _name;

    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            if (_name != value)
            {
                _name = value;

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
    }

    [SerializeField]
    private int _width;

    public int Width
    {
        get
        {
            return _width;
        }

        set
        {
            if (_width != value)
            {
                _width = value;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
    }

    [SerializeField]
    private int _height;

    public int Height
    {
        get
        {
            return _height;
        }

        set
        {
            if (_height != value)
            {
                _height = value;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
    }

    [SerializeField]
    private ResizeAnchor _anchor = ResizeAnchor.TopLeft;

    public ResizeAnchor Anchor
    {
        get
        {
            return _anchor;
        }

        set
        {
            if (_anchor != value)
            {
                _anchor = value;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
    }

    [SerializeField]
    private List<GridRow> _gridRows = new List<GridRow>();

    public Tile GetTile(int row, int col)
    {
        return _gridRows[row][col];
    }

    public GridRow GetRow(int row)
    {
        return _gridRows[row];
    }

    public override string ToString()
    {
        return _name;
    }

#if UNITY_EDITOR
    public void Resize(bool keepExistingTiles)
    {
        if (!keepExistingTiles)
        {
            DeleteRows();

            foreach (GridRow row in _gridRows)
            {
                foreach (Tile tile in row.Tiles)
                {
                    DestroyImmediate(tile, true);
                }
            }

            _gridRows = new List<GridRow>();

            for (int row = 0; row < _width; row++)
            {
                GridRow newRow = ScriptableObject.CreateInstance<GridRow>();
                newRow.name = "Row " + (row + 1).ToString();
                UnityEditor.AssetDatabase.AddObjectToAsset(newRow, this);

                _gridRows.Add(newRow);

                for (int col = 0; col < _height; col++)
                {
                    Tile newTile = ScriptableObject.CreateInstance<Tile>();
                    newTile.name = "X" + row.ToString() + "-Y" + col.ToString();
                    _gridRows[row].Add(newTile);

                    UnityEditor.AssetDatabase.AddObjectToAsset(newTile, newRow);
                }
            }

            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.SetDirty(this);

            return;
        }

        /*
        Vector2 anchor = new Vector2(((int)_anchor % 3) / 2.0f, ((int)_anchor / 3) / 2.0f);

        List<List<Tile>> newTiles = new List<List<Tile>>();
        */

        UnityEditor.EditorUtility.SetDirty(this);
    }

    public void DeleteRows()
    {
        foreach(GridRow row in _gridRows)
        {
            DestroyImmediate(row, true);
        }
    }
#endif
}
