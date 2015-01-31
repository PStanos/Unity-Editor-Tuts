using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TileDirectory : ScriptableObject
{
    [SerializeField]
    private List<TileType> _tileTypes;

    public List<TileType> TileTypes
    {
        get
        {
            return _tileTypes;
        }

        set
        {
            _tileTypes = value;
        }
    }

#if UNITY_EDITOR
    public void AddTileType()
    {
        TileType tileType = ScriptableObject.CreateInstance<TileType>();
        _tileTypes.Add(tileType);

        UnityEditor.AssetDatabase.AddObjectToAsset(tileType, this);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    public void DeleteTileType(TileType tileType)
    {
        _tileTypes.Remove(tileType);
        DestroyImmediate(tileType, true);

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }
#endif
}
