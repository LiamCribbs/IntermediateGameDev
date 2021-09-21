using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushWiggle : MonoBehaviour
{
    float center;
    public float offset;
    public float speed;

    float timeOffset;

    void Awake()
    {
        center = transform.localEulerAngles.z;
        timeOffset = Random.value * 1000f;
    }

    void Update()
    {
        transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(center - offset, center + offset, (Mathf.Sin((timeOffset + Time.time) * speed) + 1f) / 2f));
    }
}