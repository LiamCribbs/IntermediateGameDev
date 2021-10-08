using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    float followInterpolateAmount;
    public float focusCentering = 0.98f;
    public float followDistance = 0.5f;

    public Vector2 offset = new Vector2(0f, 0.09f);

    Vector2 focusPosition;

    void Update()
    {
        Vector2 targetPosition = (Vector2)target.position + offset;

        // Camera follow
        float focusPositionDelta = Vector2.Distance(targetPosition, focusPosition);

        //Lerp position to lag behind player movement
        if (focusPositionDelta > 0.01f)
        {
            followInterpolateAmount = Mathf.Pow(1f - focusCentering, Time.deltaTime);
        }
        if (focusPositionDelta > followDistance)
        {
            followInterpolateAmount = Mathf.Min(followInterpolateAmount, followDistance / focusPositionDelta);
        }

        focusPosition = Vector2.Lerp(targetPosition, focusPosition, followInterpolateAmount);

        transform.position = new Vector3(focusPosition.x, focusPosition.y, -10f);
    }
}
