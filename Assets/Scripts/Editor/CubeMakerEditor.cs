using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CubeMaker))]
public class CubeMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CubeMaker instance = (CubeMaker)target;

        // Instatiate cubes
        if (GUILayout.Button("Create Cube"))
        {
            instance.Create();
        }

        if(GUILayout.Button("Reset Size"))
        {
            instance.ResetSize();
        }
    }

    // Drawing handles
    void OnSceneGUI()
    {
        CubeMaker instance = (CubeMaker)target;

        instance.DrawResizeHandles();
    }
}
