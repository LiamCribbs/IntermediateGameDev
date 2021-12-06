using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pigeon.Math;

public class CircleFollow : MonoBehaviour
{
    public Transform target;

    public Vector3 center;
    public float radius;
    public float speed;

    void Update()
    {
        Vector3 position = Vector2.Lerp(transform.localPosition, center + (target.localPosition - transform.position).NormalizedFast() * radius, speed * Time.deltaTime);
        position.z = center.z;
        transform.localPosition = position;
    }
}