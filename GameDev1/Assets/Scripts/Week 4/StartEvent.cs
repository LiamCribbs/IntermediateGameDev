using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEvent : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent onStart;

    void Start()
    {
        onStart?.Invoke();
    }
}