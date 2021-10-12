using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float speed = 1f;
    public Vector2 targetPos;
    public Vector3 playerPos = new Vector3(0f, 0f, -0.05f);

    public void MoveUp()
    {
        PlayerMove.instance.disable = true;
        PlayerMove.instance.rigidbody.simulated = false;
        PlayerMove.instance.transform.SetParent(transform);
        PlayerMove.instance.transform.localPosition = playerPos;
        StartCoroutine(MoveUpCoroutine(targetPos));
    }

    IEnumerator MoveUpCoroutine(Vector3 targetPos)
    {
        Vector3 initialPos = transform.localPosition;
        targetPos.z = initialPos.z;
        float time = 0f;

        while (time < 1f)
        {
            time += speed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            transform.localPosition = Vector3.LerpUnclamped(initialPos, targetPos, Pigeon.EaseFunctions.EaseInOutQuartic(time));

            yield return null;
        }

        PlayerMove.instance.transform.SetParent(null);
        PlayerMove.instance.disable = false;
        PlayerMove.instance.rigidbody.simulated = true;
    }
}