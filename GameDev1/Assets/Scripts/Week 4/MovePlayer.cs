using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public Vector3 speed;

    void Update()
    {
        PlayerMove.instance.transform.localPosition += speed * Time.deltaTime;
    }
}