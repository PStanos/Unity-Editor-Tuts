﻿using UnityEngine;
using UnityEditor;
using System.Collections;

public class ErrorTest : EditorWindow
{
    [MenuItem("Utilities/Error Test")]
    static void Init()
    {
        ErrorTest window = (ErrorTest)EditorWindow.GetWindow(typeof(ErrorTest));
        window.title = "Errors";
    }

    void OnGUI()
    {
        try
        {
            throw new System.Exception("Testing");
        }
        catch(System.Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }
    }
}