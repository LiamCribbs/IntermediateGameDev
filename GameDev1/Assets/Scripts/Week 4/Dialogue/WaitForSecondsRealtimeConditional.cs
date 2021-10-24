using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForSecondsRealtimeConditional : CustomYieldInstruction
{
    readonly float startTime;
    readonly float waitTime;

    public override bool keepWaiting
    {
        get
        {
            return Time.unscaledTime - startTime < waitTime && !Input.GetKeyDown(KeyCode.Mouse0);
        }
    }

    public WaitForSecondsRealtimeConditional(float waitTime)
    {
        startTime = Time.unscaledTime;
        this.waitTime = waitTime;
    }
}