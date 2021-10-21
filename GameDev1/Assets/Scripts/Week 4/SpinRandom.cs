using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinRandom : MonoBehaviour
{
    public float spinSpeed;
    public Vector2 randSpeed;

    void Awake()
    {
        if (spinSpeed == 0f)
        {
            spinSpeed = Random.Range(randSpeed.x, randSpeed.y);
        }
    }

    void Update()
    {
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }
}
