using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineUtil
{
    public static void RestartCoroutine(this MonoBehaviour mb, ref Coroutine coroutine, IEnumerator method)
    {
        if (coroutine != null)
        {
            mb.StopCoroutine(coroutine);
        }

        coroutine = mb.StartCoroutine(method);
    }

    public static void ResetCoroutine(this MonoBehaviour mb, ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            mb.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}