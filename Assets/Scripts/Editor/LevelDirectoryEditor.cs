using UnityEngine;
using UnityEditor;
using System.Collections;

public class LevelDirectoryEditor : EditorWindow
{
    public const string DIRECTORY_PATH = "Assets/Scenes/SceneDirectory";
    public const string SCENE_ASSET_PATH = "Assets/Scenes/SceneDirectory/SceneAssets";

    [MenuItem("Utilities/Level Directory")]
    public static void ShowWindow()
    {
        EditorWindow newWindow = EditorWindow.GetWindow(typeof(LevelDirectoryEditor));
        newWindow.title = "Levels";
    }

    void OnGUI()
    {
        SceneDirectory dir = (SceneDirectory)AssetDatabase.LoadAssetAtPath(DIRECTORY_PATH + "/SceneDir.asset", typeof(SceneDirectory));

        if(dir == null)
        {
            EditorGUILayout.LabelField("No scene directory found!");

            if (GUILayout.Button("Create Scene Directory"))
            {
                SceneDirectory newDir = ScriptableObject.CreateInstance<SceneDirectory>();

                AssetDatabase.CreateAsset(newDir, DIRECTORY_PATH + "/SceneDir.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        else
        {
            SceneAsset deleteScene = null;

            for (int itr = 0; itr < dir.Scenes.Count; itr++)
            {
                SceneAsset scene = dir.Scenes[itr];

                if (scene == null)
                    continue;

                CustomEditorUtils.HeaderStart(scene.name);
                scene.sceneName = EditorGUILayout.TextField("Scene Name", scene.sceneName);

                EditorGUILayout.LabelField("Features");
                for (int itrFeature = 0; itrFeature < scene.features.Count; itrFeature++)
                {
                    EditorGUILayout.BeginHorizontal();
                    scene.features[itrFeature] = EditorGUILayout.TextField("Feature #" + (itrFeature + 1).ToString(), scene.features[itrFeature]);

                    if(GUILayout.Button("Delete Feature"))
                    {
                        scene.features.RemoveAt(itrFeature);
                        itrFeature--;
                        continue;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Add New Feature"))
                {
                    scene.features.Add("");
                }

                if (GUILayout.Button("Delete"))
                {
                    deleteScene = scene;
                }

                EditorGUILayout.EndHorizontal();

                CustomEditorUtils.HeaderEnd();
            }

            if(deleteScene != null)
            {
                dir.Scenes.Remove(deleteScene);
                DestroyImmediate(deleteScene, true);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Create Scene Asset"))
            {
                SceneAsset newScene = ScriptableObject.CreateInstance<SceneAsset>();

                if (dir.Scenes.Count > 0)
                {
                    newScene.name = "SceneAsset" + (System.Convert.ToInt32((dir.Scenes[dir.Scenes.Count - 1].name.Split('.')[0][dir.Scenes[dir.Scenes.Count - 1].name.Split('.')[0].Length - 1]).ToString()) + 1).ToString() + ".asset";
                }
                else
                {
                    newScene.name = "SceneAsset1.asset";
                }

                dir.Scenes.Add(newScene);

                AssetDatabase.AddObjectToAsset(newScene, dir);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Save Assets"))
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        // Necessary, otherwise data will be lost if window is open on entering play mode
        EditorUtility.SetDirty(dir);
    }
}
