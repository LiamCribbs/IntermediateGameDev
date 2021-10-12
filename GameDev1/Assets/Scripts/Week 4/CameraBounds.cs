using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
#if UNITY_EDITOR
    public Color DEBUG_COLOR = new Color(0.54f, 0.2f, 1f);
#endif

    public Vector2 size;

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = DEBUG_COLOR;
        Gizmos.DrawWireCube(transform.position, size * 2f);
    }
#endif
}