using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadCaller : MonoBehaviour
{
    public SceneReference scene;
    public int entrance;

    public void LoadScene()
    {
        GameManager.LoadScene(scene, entrance);
    }
}