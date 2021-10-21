using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Vector3 speed;

    void Update()
    {
        transform.localPosition += speed * Time.deltaTime;
    }
}