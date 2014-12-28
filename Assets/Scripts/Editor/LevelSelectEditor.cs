using UnityEngine;
using UnityEditor;
using System.Collections;

public class LevelSelectEditor : EditorWindow
{
    public const string LEVEL_SELECT_SKIN_PATH = "Assets/GUISkins/LevelSelect.guiskin";
    public const string SCENE_PATH = "Assets/Scenes/";
    public const string SCENE_DIR_PATH = "Assets/Scenes/SceneDirectory/SceneDir.asset";
    public const string SCENE_EXTENSION = ".unity";

    [MenuItem("Utilities/Level Select")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelSelectEditor));
    }

    void OnGUI()
    {
        GUISkin skin = (GUISkin)AssetDatabase.LoadAssetAtPath(LEVEL_SELECT_SKIN_PATH, typeof(GUISkin));
        SceneDirectory dir = (SceneDirectory)AssetDatabase.LoadAssetAtPath(SCENE_DIR_PATH, typeof(SceneDirectory));

        for (int itr = 0; itr < dir.Scenes.Count; itr++)
        {
            string levelText = dir.Scenes[itr].sceneName;

            CustomEditorUtils.HeaderStart(levelText);

            if (GUILayout.Button("Load " + levelText, skin.button))
            {
                EditorApplication.OpenScene(SCENE_PATH + levelText + SCENE_EXTENSION);
            }

            EditorGUILayout.LabelField("Details", skin.customStyles[0]);
            foreach(string feature in dir.Scenes[itr].features)
            {
                EditorGUILayout.LabelField(feature);
            }

            CustomEditorUtils.HeaderEnd();
        }
    }
}
