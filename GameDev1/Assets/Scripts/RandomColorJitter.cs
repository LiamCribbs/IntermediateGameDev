using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColorJitter : MonoBehaviour
{
    public float jitter;

    [ContextMenu("Jitter")]
    public void JitterColor()
    {
        GetComponent<SpriteRenderer>().color += new Color(Random.Range(-jitter, jitter), Random.Range(-jitter, jitter), Random.Range(-jitter, jitter));
    }

    [ColorUsage(true, true)] public Color color;

    [ContextMenu("SetColor")]
    public void SetColor()
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}