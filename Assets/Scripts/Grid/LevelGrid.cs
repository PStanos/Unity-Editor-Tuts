using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class LevelGrid : MonoBehaviour
{
#if UNITY_EDITOR
    public static readonly string GRID_ASSET_PATH = "Assets/Resources/Grids";
#endif

    [SerializeField]
    [HideInInspector]
    private Grid _grid;

    public Grid Grid
    {
        get { return _grid; }
        set { _grid = value; }
    }

    public GameObject tilePrefab;

    public float tileWidth = 1.0f;

    public float tileHeight = 1.0f;

    public void Rebuild()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile prefab is null!");
            return;
        }

        for (int itr = 0; itr < this.transform.childCount; itr++)
        {
            if (Application.isPlaying)
            {
                Destroy(this.transform.GetChild(itr).gameObject);
            }
            else
            {
                DestroyImmediate(this.transform.GetChild(itr).gameObject);
                itr--;
            }
        }

        for (int col = 0; col < _grid.Width; col++)
        {
            for (int row = _grid.Height - 1; row > 0; row--)
            {
                GameObject newTile = (GameObject)GameObject.Instantiate(tilePrefab,
                    new Vector3((col - (int)(_grid.Width / 2)) * tileWidth, (row - (int)(_grid.Height / 2)) * tileHeight, 0.0f),
                    Quaternion.Euler(new Vector3(0, 0, ((int)_grid.GetTile(row, col).TileOrientation) * 90.0f)));

                SpriteRenderer sr = newTile.GetComponent<SpriteRenderer>();
                sr.sprite = _grid.GetTile(row, col).Sprite;
                sr.transform.localScale = new Vector3(tileWidth, tileHeight, 1);

                newTile.transform.SetParent(this.transform, false);
            }
        }
    }

#if UNITY_EDITOR
    public void NewGrid()
    {
        _grid = ScriptableObject.CreateInstance<Grid>();
        _grid.Guid = Guid.NewGuid();

        UnityEditor.AssetDatabase.CreateAsset(_grid, GRID_ASSET_PATH + "/grid-" + _grid.Guid + ".asset");
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    public void DeleteGrid()
    {
        if (_grid == null)
            return;

        bool delete = UnityEditor.EditorUtility.DisplayDialog("Confirm Grid Deletion", "Continuing will permanently delete the level grid. Continue?", "Yes", "No");

        if (delete)
        {
            UnityEditor.AssetDatabase.DeleteAsset(GRID_ASSET_PATH + "/grid-" + _grid.Guid + ".asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            _grid = null;
        }
    }

    public void RedrawGrid()
    {
        for (int row = 0; row < _grid.Width; row++)
        {
            for (int col = 0; col < _grid.Height; col++)
            {
                _grid.GetTile(row, col);
            }
        }
    }
#endif
}
