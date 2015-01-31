using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GridRow : ScriptableObject
{
    public List<Tile> Tiles = new List<Tile>();

    public Tile this[int index]
    {
        get
        {
            return Tiles[index];
        }

        set
        {
            Tiles[index] = value;
        }
    }

    public void Add(Tile newTile)
    {
        Tiles.Add(newTile);
    }
}
