using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public new Camera camera;

    public Transform followTarget;

    Coroutine moveCameraCoroutine;
    Coroutine changeSizeCoroutine;

    public float moveSpeed;
    public float followSpeed;
    public float changeSizeSpeed;

    void Update()
    {
        if (followTarget)
        {
            Vector3 pos = Vector3.Lerp(transform.position, followTarget.position, followSpeed * Time.deltaTime);
            pos.z = -10f;
            transform.position = pos;
        }
    }

    public void EnableFollow(Transform target)
    {
        followTarget = target;

        if (moveCameraCoroutine != null)
        {
            StopCoroutine(moveCameraCoroutine);
            moveCameraCoroutine = null;
        }
    }

    public void DisableFollow()
    {
        followTarget = null;
    }

    public void StartMoveCamera(Transform target)
    {
        if (moveCameraCoroutine != null)
        {
            StopCoroutine(moveCameraCoroutine);
        }

        moveCameraCoroutine = StartCoroutine(MoveCamera(target.position));
    }

    IEnumerator MoveCamera(Vector3 target)
    {
        Vector3 initial = transform.position;
        float time = 0f;

        while (time < 1f)
        {
            time += moveSpeed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            transform.position = Vector3.LerpUnclamped(initial, target, Pigeon.EaseFunctions.EaseInOutQuartic(time));

            yield return null;
        }

        moveCameraCoroutine = null;
    }

    public void StartChangeSize(float size)
    {
        if (changeSizeCoroutine != null)
        {
            StopCoroutine(changeSizeCoroutine);
        }

        changeSizeCoroutine = StartCoroutine(ChangeSize(size));
    }

    IEnumerator ChangeSize(float target)
    {
        float initial = camera.orthographicSize;
        float time = 0f;

        while (time < 1f)
        {
            time += changeSizeSpeed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            camera.orthographicSize = Mathf.LerpUnclamped(initial, target, Pigeon.EaseFunctions.EaseInOutQuartic(time));

            yield return null;
        }

        changeSizeCoroutine = null;
    }
}