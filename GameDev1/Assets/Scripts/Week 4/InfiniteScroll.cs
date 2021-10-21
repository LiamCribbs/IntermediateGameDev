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

    public Sprite defaultSprite;
    public Sprite windowSprite;

    System.Action<Transform> onObjectMoved;

    void Awake()
    {
        leadingObject = object1;
    }

    void Update()
    {
        Vector3 move = moveSpeed * Time.deltaTime;
        object1.transform.localPosition += move;
        object2.transform.localPosition += move;

        if (leadingObject.localPosition.x < minPos.x || leadingObject.localPosition.y < minPos.y)
        {
            //leadingObject.localPosition = new Vector3(minPos.x, minPos.y, leadingObject.localPosition.z);
            leadingObject.localPosition += objectSize * 2f;
            onObjectMoved?.Invoke(leadingObject);
            leadingObject = leadingObject == object1 ? object2 : object1;
        }
    }

    public void ActivateWindow(int numWindows)
    {
        StartCoroutine(ActivateWindowsCoroutine(numWindows));
    }

    IEnumerator ActivateWindowsCoroutine(int numWindows)
    {
        bool leaderMoved = false;
        Transform transform = null;
        int i = 0;

        onObjectMoved += (Transform t) =>
        {
            leaderMoved = true;
            transform = t;
            i++;
        };

        while (!leaderMoved)
        {
            yield return null;
        }

        transform.GetComponent<SpriteRenderer>().sprite = windowSprite;
        leaderMoved = false;

        while (!leaderMoved)
        {
            yield return null;
        }

        transform.GetComponent<SpriteRenderer>().sprite = windowSprite;
        i = 0;

        while (i < numWindows)
        {
            yield return null;
        }

        leaderMoved = false;

        while (!leaderMoved)
        {
            yield return null;
        }

        transform.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        leaderMoved = false;

        while (!leaderMoved)
        {
            yield return null;
        }

        transform.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        onObjectMoved = null;
    }
}