using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteScroll : MonoBehaviour
{
    public Transform object1, object2;
    Transform leadingObject;

    public Vector3 moveSpeed;
    public Vector2 minPos;
    public Vector3 objectSize;

    // Start is called before the first frame update
    void Awake()
    {
        leadingObject = object1;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = moveSpeed * Time.deltaTime;
        object1.transform.localPosition += move;
        object2.transform.localPosition += move;

        if (leadingObject.localPosition.x < minPos.x || leadingObject.localPosition.y < minPos.y)
        {
            //leadingObject.localPosition = new Vector3(minPos.x, minPos.y, leadingObject.localPosition.z);
            leadingObject.localPosition += objectSize * 2f;
            leadingObject = leadingObject == object1 ? object2 : object1;
        }
    }
}