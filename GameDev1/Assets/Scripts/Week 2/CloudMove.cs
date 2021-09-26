using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMove : MonoBehaviour
{
    public Vector2 minPos;
    public Vector2 maxPos;

    public float moveSpeed;
    float actualMoveSpeed;

    void Start()
    {
        bool leftToRight = Random.value <= 0.5f;
        transform.localPosition = new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), transform.localPosition.z);
        transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0, 5) * 90f);

        actualMoveSpeed = leftToRight ? moveSpeed : -moveSpeed;
    }

    void Update()
    {
        transform.localPosition += new Vector3(actualMoveSpeed * Time.deltaTime, 0f);

        if (transform.localPosition.x > maxPos.x || transform.localPosition.x < minPos.x)
        {
            SetPos();
        }
    }

    void SetPos()
    {
        bool leftToRight = Random.value <= 0.5f;
        transform.localPosition = new Vector3(leftToRight ? minPos.x : maxPos.x, Random.Range(minPos.y, maxPos.y), transform.localPosition.z);
        transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(0, 5) * 90f);

        actualMoveSpeed = leftToRight ? moveSpeed : -moveSpeed;
    }
}