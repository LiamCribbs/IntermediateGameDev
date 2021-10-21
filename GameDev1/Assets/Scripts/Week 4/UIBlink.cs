using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBlink : MonoBehaviour
{
    public UnityEngine.UI.Graphic graphic;
    public float speed;

    Color color;

    void Awake()
    {
        color = graphic.color;
    }

    void Update()
    {
        color.a = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        graphic.color = color;
    }
}