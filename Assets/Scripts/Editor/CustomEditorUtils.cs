using UnityEngine;
using System.Collections;
using UnityEditor;

[System.Serializable]
public class CustomEditorUtils
{
    public const string HEADER_SKIN_PATH = "Assets/GUISkins/Header.guiskin";

	public static void HeaderStart(string title)
    {
        GUISkin headerSkin = (GUISkin)AssetDatabase.LoadAssetAtPath(HEADER_SKIN_PATH, typeof(GUISkin));

        EditorGUILayout.BeginVertical();
        GUILayout.Box(title, headerSkin.box);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(headerSkin.customStyles[0]);
    }

    public static void HeaderEnd()
    {
        EditorGUILayout.EndVertical();
    }
}
