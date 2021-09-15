using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float min, max;
    public float speed;

    void Update()
    {
        transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(min, max, (Mathf.Sin(Time.time * speed) + 1f) / 2f));
    }
}