using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public Color matchColor;
    public UnityEngine.Events.UnityEvent onTriggerEnter;
    public UnityEngine.Events.UnityEvent<Transform> onTriggerEnterObject;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
        {
            return;
        }

        if (collision.gameObject.TryGetComponent(out SpriteRenderer renderer) && renderer.color == matchColor)
        {
            onTriggerEnter?.Invoke();
            onTriggerEnterObject?.Invoke(collision.transform);

            enabled = false;
        }
    }
}