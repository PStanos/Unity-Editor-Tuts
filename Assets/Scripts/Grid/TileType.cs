using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType : ScriptableObject
{
    [SerializeField]
    private Sprite _tileSprite;

    public Sprite TileSprite
    {
        get
        {
            return _tileSprite;
        }

        set
        {
            _tileSprite = value;
        }
    }

    public override string ToString()
    {
        return _tileSprite.name;
    }
}
