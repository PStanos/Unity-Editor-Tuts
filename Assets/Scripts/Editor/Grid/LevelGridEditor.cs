using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LevelGrid))]
public class LevelGridEditor : Editor
{
    LevelGrid level;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        level = (LevelGrid)target;

        if (level.Grid == null)
        {
            if (GUILayout.Button("New Grid"))
            {
                level.NewGrid();
            }

            return;
        }

        level.Grid.Name = EditorGUILayout.TextField("Name", level.Grid.Name);
        level.Grid.Width = EditorGUILayout.IntField("Width", level.Grid.Width);
        level.Grid.Height = EditorGUILayout.IntField("Height", level.Grid.Height);

        if (GUILayout.Button("Rebuild Level"))
        {
            level.Rebuild();
        }

        if (GUILayout.Button("Resize Grid"))
        {
            level.Grid.Resize(false);
            level.RedrawGrid();
        }

        GridToolEditor.DrawTools();

        if (GUILayout.Button("Delete Grid"))
        {
            level.DeleteGrid();
        }
    }
}
