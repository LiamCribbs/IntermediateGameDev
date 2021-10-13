using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    public static int chosenEntrance;
    public UnityEvent[] entrances;

    public static void LoadScene(string scene, int entrance)
    {
        chosenEntrance = entrance;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(scene);
    }

    static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        instance.entrances[chosenEntrance].Invoke();
    }

    public void SetPlayerPosition(Vector3 position)
    {
        PlayerMove.instance.transform.position = position;
    }
    public void SetPlayerPosition(Transform position)
    {
        PlayerMove.instance.transform.position = position.position;
    }
}