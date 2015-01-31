using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SceneAsset : ScriptableObject
{
    public string sceneName;

    public List<string> features = new List<string>();
}
