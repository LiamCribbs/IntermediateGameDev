using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    public int sceneIndex;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == PlayerMove.instance.collider)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        }
    }
}