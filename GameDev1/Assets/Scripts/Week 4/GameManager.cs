using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UnityEngine.UI.Graphic fadeGraphic;
    Coroutine fadeCoroutine;
    const float FadeSpeed = 2.25f;
    public static bool loadingScene;

    public static int chosenEntrance;
    public UnityEvent[] entrances;

    [Space(20)]
    public List<MonoBehaviour> saveables;
    public static SceneSaveData[] sceneData;

    void Awake()
    {
        instance = this;

        if (sceneData == null)
        {
            sceneData = new SceneSaveData[SceneManager.sceneCountInBuildSettings];
        }
    }

    public static void LoadScene(string scene, int entrance)
    {
        if (loadingScene)
        {
            return;
        }

        loadingScene = true;

        chosenEntrance = entrance;
        SaveObjectData();

        if (instance.fadeCoroutine != null)
        {
            instance.StopCoroutine(instance.fadeCoroutine);
        }

        instance.fadeCoroutine = instance.StartCoroutine(FadeSceneOut(scene));
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        instance.entrances[chosenEntrance].Invoke();

        FollowCamera.instance.SetPosition(PlayerMove.instance.transform.position);

        LoadObjectData();

        instance.fadeCoroutine = instance.StartCoroutine(FadeSceneIn());
    }

    static IEnumerator FadeSceneOut(string scene)
    {
        PlayerMove.instance.DisableRequests++;

        instance.fadeGraphic.transform.SetAsLastSibling();

        Color color = instance.fadeGraphic.color;
        float time = 0f;

        while (time < 1f)
        {
            time += FadeSpeed * Time.deltaTime;

            color.a = Pigeon.EaseFunctions.EaseInOutQuartic(time);
            instance.fadeGraphic.color = color;

            yield return null;
        }

        PlayerMove.instance.DisableRequests--;

        instance.fadeCoroutine = null;

        loadingScene = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(scene);
    }

    static IEnumerator FadeSceneIn()
    {
        instance.fadeGraphic.transform.SetAsLastSibling();

        Color color = instance.fadeGraphic.color;
        float time = 0f;

        while (time < 1f)
        {
            time += FadeSpeed * Time.deltaTime;

            color.a = 1f - Pigeon.EaseFunctions.EaseInOutQuartic(time);
            instance.fadeGraphic.color = color;

            yield return null;
        }

        instance.fadeCoroutine = null;
    }

    public void SetPlayerPosition(Vector3 position)
    {
        position.z = PlayerMove.instance.transform.position.z;
        PlayerMove.instance.transform.position = position;
    }

    public void SetPlayerPosition(Transform position)
    {
        Vector3 p = position.position;
        p.z = PlayerMove.instance.transform.position.z;
        PlayerMove.instance.transform.position = p;
    }

    static void SaveObjectData()
    {
        if (instance.saveables == null || instance.saveables.Count == 0)
        {
            return;
        }

        SceneSaveData saveData = new SceneSaveData()
        {
            saveData = new List<SaveData>(instance.saveables.Count)
        };

        for (int i = 0; i < instance.saveables.Count; i++)
        {
            saveData.saveData.Add(instance.saveables[i].GetComponent<ISaveable>().Save());
        }

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        sceneData[sceneIndex] = saveData;
    }

    static void LoadObjectData()
    {
        if (instance.saveables == null || instance.saveables.Count == 0)
        {
            return;
        }

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneSaveData saveData = sceneData[sceneIndex];

        if (saveData != null)
        {
            for (int i = 0; i < saveData.saveData.Count; i++)
            {
                instance.saveables[i].GetComponent<ISaveable>().Load(saveData.saveData[i]);
            }
        }
    }
}