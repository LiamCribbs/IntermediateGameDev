using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMagnet : CameraBounds
{
    public enum DistanceTestMode
    {
        Horizontal, Vertical
    }

    public DistanceTestMode distanceTestMode;

    public AnimationCurve pullStrength = AnimationCurve.Linear(0f, 0f, 1f, 1f);

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Color c = DEBUG_COLOR;
        c.a = 0.5f;
        Gizmos.color = c;
        Gizmos.DrawCube(transform.position, new Vector3(0.25f, 0.25f));
    }

    void Reset()
    {
        DEBUG_COLOR = new Color(0.2f, 0.58f, 1f);
    }
#endif
}