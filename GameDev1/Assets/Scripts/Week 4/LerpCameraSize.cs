using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpCameraSize : MonoBehaviour
{
    public float speed = 0.5f;
    Coroutine animateCameraSizeCoroutine;

    public void ChangeSize(float targetSize)
    {
        if (animateCameraSizeCoroutine != null)
        {
            StopCoroutine(animateCameraSizeCoroutine);
        }

        animateCameraSizeCoroutine = StartCoroutine(AnimateCameraSize(FollowCamera.instance.camera, targetSize));
    }

    IEnumerator AnimateCameraSize(Camera camera, float targetSize)
    {
        float initialSize = camera.orthographicSize;
        float time = 0f;

        while (time < 1f)
        {
            time += speed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            camera.orthographicSize = Mathf.LerpUnclamped(initialSize, targetSize, Pigeon.EaseFunctions.EaseInOutQuartic(time));
            yield return null;
        }

        animateCameraSizeCoroutine = null;
    }
}