using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    public SceneReference scene;
    public int entrance;
    bool enable = false;

    IEnumerator Start()
    {
        yield return null;
        enable = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (enable && other == PlayerMove.instance.collider)
        {
            GameManager.LoadScene(scene, entrance);
        }
    }
}