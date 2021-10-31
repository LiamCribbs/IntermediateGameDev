using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainLeaveStation : MonoBehaviour
{
    public SlidingDoor[] doors;
    public Interactable[] interactablesToDisable;

    public float timeBeforeDoorsOpen = 1f;

    public float initialArrivePos;
    public float finalArrivePos;
    public float finalLeavePos;

    public float arriveSpeed;
    public float leaveSpeed;

    public void ArriveAtStation()
    {
        StartCoroutine(ArriveAtStationCoroutine(initialArrivePos, finalArrivePos, arriveSpeed, Pigeon.EaseFunctions.EaseOutQuadratic));
    }

    IEnumerator ArriveAtStationCoroutine(float initial, float final, float speed, Pigeon.EaseFunctions.EvaluateMode curve)
    {
        yield return new WaitForSeconds(1f);

        Vector3 position = transform.localPosition;
        float time = 0f;

        while (time < 1f)
        {
            time += speed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            position.x = Mathf.LerpUnclamped(initial, final, curve(time));
            transform.localPosition = position;

            yield return null;
        }

        time = 0f;
        while (time < timeBeforeDoorsOpen)
        {
            time += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].OpenDoor(true);
        }

        for (int i = 0; i < interactablesToDisable.Length; i++)
        {
            interactablesToDisable[i].DisableInteractions();
        }
    }

    IEnumerator LeaveStationCoroutine(float initial, float final, float speed, Pigeon.EaseFunctions.EvaluateMode curve)
    {
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].OpenDoor(true);
        }

        float time = 0f;
        while (time < timeBeforeDoorsOpen)
        {
            time += Time.deltaTime;
            yield return null;
        }

        Vector3 position = transform.localPosition;
        time = 0f;

        while (time < 1f)
        {
            time += speed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            position.x = Mathf.LerpUnclamped(initial, final, curve(time));
            transform.localPosition = position;

            yield return null;
        }
    }

    public void EnterTrain()
    {
        PlayerMove.instance.DisableRequests++;
        PlayerMove.instance.transform.SetParent(transform);

        StartCoroutine(LeaveStationCoroutine(finalArrivePos, finalLeavePos, leaveSpeed, Pigeon.EaseFunctions.EaseInQuartic));
    }
}