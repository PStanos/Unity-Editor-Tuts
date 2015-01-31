using UnityEngine;
using System.Collections;
using UnityEditor;

public class TileTypeWindow : EditorWindow
{
    public static readonly string TILE_DIRECTORY_PATH = "Assets/Resources/TileTypes";
    public static readonly string TILE_DIRECTORY_NAME = "TileDirectory";

    public const float TILE_TYPE_PREVIEW_WIDTH = 50.0f;
    public const float TILE_TYPE_PREVIEW_HEIGHT = 50.0f;
    public const float TILE_TYPE_PREVIEW_MARGIN = 10.0f;

    private Vector2 scrollViewPos = new Vector2();

    public static string FullTileDirectoryPath
    {
        get
        {
            return TILE_DIRECTORY_PATH + "/" + TILE_DIRECTORY_NAME + ".asset";
        }
    }

    private TileDirectory _directory;

    [MenuItem("Utilities/Grid/Tile Types")]
    public static void ShowWindow()
    {
        EditorWindow newWindow = EditorWindow.GetWindow(typeof(TileTypeWindow));
        newWindow.title = "Tile Types";
    }

    void OnGUI()
    {
        scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);

        _directory = (TileDirectory)AssetDatabase.LoadAssetAtPath(FullTileDirectoryPath, typeof(TileDirectory));

        if (_directory == null)
        {
            if (GUILayout.Button("New Level Directory"))
            {
                _directory = ScriptableObject.CreateInstance<TileDirectory>();
                _directory.name = TILE_DIRECTORY_NAME;

                AssetDatabase.CreateAsset(_directory, FullTileDirectoryPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return;
        }

        int tileNum = 0;
        TileType typeToDelete = null;
        Rect prevRect = new Rect();

        foreach (TileType tileType in _directory.TileTypes)
        {
            GUILayout.Space(TILE_TYPE_PREVIEW_MARGIN);

            Rect newRect = EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(TILE_TYPE_PREVIEW_HEIGHT - EditorGUIUtility.standardVerticalSpacing * 2));

            if (tileType.TileSprite != null)
            {
                EditorGUI.DrawTextureTransparent(new Rect(TILE_TYPE_PREVIEW_MARGIN, tileNum * TILE_TYPE_PREVIEW_HEIGHT + (tileNum + 1) * TILE_TYPE_PREVIEW_MARGIN, TILE_TYPE_PREVIEW_WIDTH, TILE_TYPE_PREVIEW_HEIGHT), tileType.TileSprite.texture);
            }

            GUILayout.Space(TILE_TYPE_PREVIEW_HEIGHT + TILE_TYPE_PREVIEW_MARGIN * 2);

            EditorGUILayout.BeginVertical();

            EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent(tileType.TileSprite == null ? "Tile Sprite" : tileType.TileSprite.name)).x + 5.0f;
            tileType.TileSprite = (Sprite)EditorGUILayout.ObjectField(tileType.TileSprite == null ? "Tile Sprite" : tileType.TileSprite.name, tileType.TileSprite, typeof(Sprite));
            EditorGUIUtility.labelWidth = 0.0f;

            if (GUILayout.Button("Delete Tile Type"))
            {
                typeToDelete = tileType;
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            //GUILayout.Space(TILE_TYPE_PREVIEW_HEIGHT);

            tileNum++;
        }

        if (typeToDelete != null)
        {
            _directory.DeleteTileType(typeToDelete);
        }

        GUILayout.Space(TILE_TYPE_PREVIEW_MARGIN);

        if (GUILayout.Button("Add Tile Type"))
        {
            _directory.AddTileType();
        }

        EditorUtility.SetDirty(_directory);

        foreach (TileType tileType in _directory.TileTypes)
        {
            EditorUtility.SetDirty(tileType);
        }

        EditorGUILayout.EndScrollView();
    }
}
