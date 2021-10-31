using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCrashController : MonoBehaviour
{
    public Transform trainViewBlocker;
    public float viewBlockerMoveSpeed = 2f;

    public Vector3 playerChildPosition;

    public Animator animator;

    public float crashCameraSize = 13.5f;
    public float zoomInCameraSize = 0.1f;
    public float crashCameraFocusSpeed = 5f;

    public float waitTimeToFadeOut;
    public float fadeOutSpeed;
    public Vector2 zoomInCameraPos;

    public SceneReference trainCrashedScene;
    public int trainCrashedSceneEntrance;

    public float timeBeforeCrash = 40f;
    float crashTimer;

    void Awake()
    {
        crashTimer = timeBeforeCrash;
    }

    void Update()
    {
        if (crashTimer > 0f)
        {
            crashTimer -= Time.deltaTime;

            if (crashTimer <= 0f)
            {
                crashTimer = 0f;
                StartCrash();
            }
        }
    }

    public void StartCrash()
    {
        StartCoroutine(StartCrashCoroutine());
    }


    IEnumerator StartCrashCoroutine()
    {
        float initialPos = trainViewBlocker.localPosition.x;
        float time = 0f;

        while (time < 1f)
        {
            time += viewBlockerMoveSpeed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            trainViewBlocker.localPosition = new Vector2(Mathf.Lerp(initialPos, 0f, time), 0f);

            yield return null;
        }

        PlayerMove.instance.DisableRequests++;
        PlayerMove.instance.transform.SetParent(transform);
        PlayerMove.instance.transform.position = playerChildPosition;
        FollowCamera.instance.SetPosition(playerChildPosition);
        FollowCamera.instance.camera.orthographicSize = crashCameraSize;
        FollowCamera.instance.focusSpeed = crashCameraFocusSpeed;

        animator.SetBool("Start", true);

        yield return new WaitForSeconds(waitTimeToFadeOut);

        GameManager.instance.fadeGraphic.transform.SetAsLastSibling();
        Color color = GameManager.instance.fadeGraphic.color;
        Vector2 initialCameraPos = FollowCamera.instance.GetPosition();
        time = 0f;

        while (time < 1f)
        {
            time += fadeOutSpeed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            float t = Pigeon.EaseFunctions.EaseInOutQuartic(time);

            color.a = t;
            GameManager.instance.fadeGraphic.color = color;
            FollowCamera.instance.camera.orthographicSize = Mathf.LerpUnclamped(crashCameraSize, zoomInCameraSize, t);
            FollowCamera.instance.SetPosition(Vector2.LerpUnclamped(initialCameraPos, zoomInCameraPos, t));

            yield return null;
        }

        // Load next scene
        GameManager.chosenEntrance = trainCrashedSceneEntrance;
        GameManager.SaveObjectData();
        GameManager.loadingScene = false;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += GameManager.OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.LoadScene(trainCrashedScene);
    }
}