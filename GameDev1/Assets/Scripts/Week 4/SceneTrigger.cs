using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    public SceneReference scene;
    public int entrance;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == PlayerMove.instance.collider)
        {
            GameManager.LoadScene(scene, entrance);
        }
    }
}