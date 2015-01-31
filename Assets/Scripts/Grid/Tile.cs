using UnityEngine;
using System.Collections;

[System.Serializable]
public class Tile : ScriptableObject
{
    public enum Orientation
    {
        Up,
        Right,
        Down,
        Left
    }

    [SerializeField]
    private Sprite _sprite;

    public Sprite Sprite
    {
        get
        {
            return _sprite;
        }

        set
        {
            _sprite = value;
        }
    }

    [SerializeField]
    private Orientation _orientation;

    public Orientation TileOrientation
    {
        get
        {
            return _orientation;
        }

        set
        {
            _orientation = value;
        }
    }
}
