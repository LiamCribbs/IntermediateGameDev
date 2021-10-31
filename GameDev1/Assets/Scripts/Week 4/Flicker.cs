using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour
{
    public Behaviour behvaiour;
    public Vector2 blinkWaitRange;
    public Vector2 blinkOnRange;

    void Awake()
    {
        behvaiour.enabled = false;
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(blinkWaitRange.x, blinkWaitRange.y));
            behvaiour.enabled = true;
            yield return new WaitForSeconds(Random.Range(blinkOnRange.x, blinkOnRange.y));
            behvaiour.enabled = false;
        }
    }
}