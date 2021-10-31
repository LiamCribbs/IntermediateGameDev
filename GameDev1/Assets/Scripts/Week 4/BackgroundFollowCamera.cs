using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFollowCamera : MonoBehaviour
{
    public Vector2 offset;

    void Update()
    {
        Vector3 position = FollowCamera.instance.transform.position;
        position.z = transform.position.z;
        position.x += offset.x;
        position.y += offset.y;
        transform.position = position;
    }
}