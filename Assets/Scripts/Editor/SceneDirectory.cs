using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SceneDirectory : ScriptableObject
{
    [SerializeField]
    private List<SceneAsset> scenes = new List<SceneAsset>();

    public List<SceneAsset> Scenes
    {
        get
        {
            return scenes;
        }

        set
        {
            scenes = value;
        }
    }
}
