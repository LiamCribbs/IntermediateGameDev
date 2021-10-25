using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    public float intensity = 0.05f;
    public float waitTime;

    Vector3 position;

    void Awake()
    {
        position = transform.localPosition;
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        while (true)
        {
            transform.localPosition = position + (Vector3)(Random.insideUnitCircle * intensity);

            yield return waitTime > 0 ? new WaitForSeconds(waitTime) : null;
        }
    }
}