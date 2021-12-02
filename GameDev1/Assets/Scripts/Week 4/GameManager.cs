using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool interactedThisFrame;

    public UnityEngine.UI.Graphic fadeGraphic;
    Coroutine fadeCoroutine;
    const float FadeSpeed = 2.25f;
    public static bool loadingScene;

    public static int chosenEntrance;
    public UnityEvent[] entrances;

    [Space(20)]
    public List<MonoBehaviour> saveables;
    public static SceneSaveData[] sceneData;

    [Space(20)]
    public TMPro.TextMeshProUGUI doongesText;
    public static int doonges;
    Coroutine bulgeDoongesTextCoroutine;
    const float DoongesTextBulgeSpeed1 = 5.5f;
    const float DoongesTextBulgeSpeed2 = 1f;
    const float DoongesTextBulgeScale = 2.25f;

    void Awake()
    {
        instance = this;

        if (sceneData == null)
        {
            sceneData = new SceneSaveData[SceneManager.sceneCountInBuildSettings];
        }

        if (doongesText != null)
        {
            doongesText.text = string.Format("{0:n0}", doonges);
        }
    }

    void LateUpdate()
    {
        interactedThisFrame = false;
    }

    public void SetDoonges(int numDoonges)
    {
        SetDoongeCounter(doonges + numDoonges);
    }

    public static void SetDoongeCounter(int amount)
    {
        int prevDoonges = doonges;
        doonges = amount;
        if (instance.doongesText != null)
        {
            instance.doongesText.text = string.Format("{0:n0}", doonges);

            if (doonges != prevDoonges)
            {
                if (instance.bulgeDoongesTextCoroutine != null)
                {
                    instance.StopCoroutine(instance.bulgeDoongesTextCoroutine);
                }
                instance.bulgeDoongesTextCoroutine = instance.StartCoroutine(BulgeDoongesText());
            }
        }
    }

    static IEnumerator BulgeDoongesText()
    {
        float initialScale = instance.doongesText.transform.localScale.x;
        float time = 0f;

        while (time < 1f)
        {
            time += DoongesTextBulgeSpeed1 * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            float scale = Mathf.LerpUnclamped(initialScale, DoongesTextBulgeScale, Pigeon.EaseFunctions.EaseOutQuartic(time));
            instance.doongesText.transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        initialScale = instance.doongesText.transform.localScale.x;
        time = 0f;

        while (time < 1f)
        {
            time += DoongesTextBulgeSpeed2 * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            float scale = Mathf.LerpUnclamped(initialScale, 1f, Pigeon.EaseFunctions.EaseOutElastic(time));
            instance.doongesText.transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        instance.bulgeDoongesTextCoroutine = null;
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

    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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

    public static void SaveObjectData()
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