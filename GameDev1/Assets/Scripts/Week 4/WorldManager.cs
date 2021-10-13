using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu()]
public class WorldManager : ScriptableObject
{
    public SceneReference[] scenes;

    public Dictionary<string, SceneReference> sceneDictionary;

    void OnEnable()
    {
        sceneDictionary = new Dictionary<string, SceneReference>(scenes.Length);
        for (int i = 0; i < scenes.Length; i++)
        {
            sceneDictionary.Add(scenes[i].ScenePath, scenes[i]);
            Debug.Log(scenes[i].ScenePath);
        }
    }
}