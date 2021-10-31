using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public Transform doorLeft;
    public Transform doorRight;
    public float speed = 0.25f;
    public float closedScale = 54f;
    Coroutine openDoorCoroutine;

    public Interactable interactable;

    public void OpenDoor(bool open)
    {
        if (openDoorCoroutine != null)
        {
            StopCoroutine(openDoorCoroutine);
        }

        openDoorCoroutine = StartCoroutine(OpenDoorCoroutine(open));
    }

    IEnumerator OpenDoorCoroutine(bool open)
    {
        float initialScale = doorLeft.localScale.x;
        float targetScale = open ? 0f : closedScale;
        Vector3 scale = doorLeft.localScale;
        float time = 0f;

        while (time < 1f)
        {
            time += speed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            scale.x = Mathf.LerpUnclamped(initialScale, targetScale, Pigeon.EaseFunctions.EaseOutBounce(time));
            doorLeft.localScale = scale;
            scale.x = -scale.x;
            doorRight.localScale = scale;

            yield return null;
        }

        if (interactable != null)
        {
            if (open)
            {
                interactable.EnableInteractions();
                if (GetComponent<Collider2D>().OverlapPoint(PlayerMove.instance.transform.position))
                {
                    interactable.Enable();
                }
            }
            else
            {
                interactable.DisableInteractions();
            }
        }

        openDoorCoroutine = null;
    }

    void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Contains("Left"))
            {
                doorLeft = transform.GetChild(i);
            }
            else if (transform.GetChild(i).name.Contains("Right"))
            {
                doorRight = transform.GetChild(i);
            }
        }
    }
}
