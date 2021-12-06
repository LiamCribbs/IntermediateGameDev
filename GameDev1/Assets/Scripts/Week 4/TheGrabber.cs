using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheGrabber : MonoBehaviour
{
    public float grabSpeed = 1.5f;

    public void StartGrab()
    {
        StartCoroutine(Grab());
    }

    IEnumerator Grab()
    {
        PlayerMove.instance.jumpEnabled = false;

        Vector3 pos = transform.position;
        float topY = pos.y;
        float time = 0f;

        while (time < 1f)
        {
            time += grabSpeed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            float t = Pigeon.EaseFunctions.EaseOutQuadratic(time);

            Vector3 playerPos = PlayerMove.instance.transform.position;
            pos.x = playerPos.x;
            pos.y = Mathf.LerpUnclamped(topY, playerPos.y, t);
            transform.position = pos;

            yield return null;
        }

        PlayerMove.instance.DisableRequests++;
        PlayerMove.instance.transform.SetParent(transform);

        float bottomY = pos.y;
        time = 0f;

        while (time < 1f)
        {
            time += grabSpeed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            float t = Pigeon.EaseFunctions.EaseInQuartic(time);

            pos.x = PlayerMove.instance.transform.position.x;
            pos.y = Mathf.LerpUnclamped(bottomY, topY, t);
            transform.position = pos;

            yield return null;
        }
    }
}