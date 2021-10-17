using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLight : MonoBehaviour
{
    public float rotateSpeed;

    //void Awake()
    //{
    //    StartCoroutine(Rotate());
    //}

    void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    //IEnumerator Rotate()
    //{
    //    while (true)
    //    {
    //        Quaternion initialRotation = transform.localRotation;
    //        Quaternion target = Random.rotation;
    //        float time = 0f;

    //        while (time < 1f)
    //        {
    //            time += rotateSpeed * Time.deltaTime;
    //            if (time > 1f)
    //            {
    //                time = 1f;
    //            }

    //            transform.localRotation = Quaternion.LerpUnclamped(initialRotation, target, time);
    //            yield return null;
    //        }
    //    }
    //}
}