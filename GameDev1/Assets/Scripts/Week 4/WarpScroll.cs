using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpScroll : MonoBehaviour
{
    public Vector2 minPos;
    public Vector2 maxPos;
    public Vector3 moveSpeed;

    void Update()
    {
        transform.localPosition += moveSpeed * Time.deltaTime;

        if (transform.localPosition.x < minPos.x)
        {
            Vector3 pos = transform.localPosition;
            pos.x = maxPos.x;
            transform.localPosition = pos;
        }
        else if (transform.localPosition.x > maxPos.x)
        {
            Vector3 pos = transform.localPosition;
            pos.x = minPos.x;
            transform.localPosition = pos;
        }

        if (transform.localPosition.y < minPos.y)
        {
            Vector3 pos = transform.localPosition;
            pos.y = maxPos.y;
            transform.localPosition = pos;
        }
        else if (transform.localPosition.y > maxPos.y)
        {
            Vector3 pos = transform.localPosition;
            pos.y = minPos.y;
            transform.localPosition = pos;
        }
    }
}
