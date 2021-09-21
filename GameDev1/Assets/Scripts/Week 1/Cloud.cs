using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float minPosition, maxPosition;

    public float moveSpeed;

    public float upSpeed;
    public float upMagnitude;
    float upTimeOffset;
    float initialY;

    void Awake()
    {
        upTimeOffset = Random.value * 1000f;
        initialY = transform.localPosition.y;
    }

    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x + moveSpeed * Time.deltaTime, initialY + (Mathf.Sin((upTimeOffset + Time.time) * upSpeed) * upMagnitude), 0f);
        if (transform.localPosition.x > maxPosition)
        {
            transform.localPosition -= new Vector3(maxPosition - minPosition, 0f, 0f);
        }

        if (transform.localPosition.x < minPosition)
        {
            transform.localPosition -= new Vector3(minPosition - maxPosition, 0f, 0f);
        }
    }
}