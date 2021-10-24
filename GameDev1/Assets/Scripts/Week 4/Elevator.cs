using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Elevator : MonoBehaviour, ISaveable
{
    [System.Serializable]
    public struct PositionSet
    {
        public Vector2 startPosition;
        public Vector2 endPosition;
    }

    public float speed = 1f;
    public PositionSet[] positions;
    public Vector2 currentTargetPosition;
    public Vector3 playerPos = new Vector3(0f, 0f, -0.01f);

    public UnityEvent onArrived;

    Coroutine moveCoroutine;

    void Awake()
    {
        currentTargetPosition = transform.localPosition;
    }

    public void MoveUp()
    {
        float originalZ = PlayerMove.instance.transform.position.z;

        PlayerMove.instance.DisableRequests++;
        PlayerMove.instance.transform.SetParent(transform);
        PlayerMove.instance.transform.localPosition = playerPos;
        moveCoroutine = StartCoroutine(MoveUpCoroutine(positions[0].endPosition, originalZ));
    }

    public void MoveUp(int positionIndex)
    {
        PositionSet position = positions[positionIndex];
        transform.localPosition = new Vector3(position.startPosition.x, position.startPosition.y, transform.localPosition.z);

        float originalZ = PlayerMove.instance.transform.position.z;

        PlayerMove.instance.DisableRequests++;
        PlayerMove.instance.transform.SetParent(transform);
        PlayerMove.instance.transform.localPosition = playerPos;
        moveCoroutine = StartCoroutine(MoveUpCoroutine(position.endPosition, originalZ));
    }

    public void Move(Vector2 startPosition, Vector2 endPosition)
    {
        transform.localPosition = new Vector3(startPosition.x, startPosition.y, transform.localPosition.z);

        float originalZ = PlayerMove.instance.transform.position.z;

        PlayerMove.instance.DisableRequests++;
        PlayerMove.instance.transform.SetParent(transform);
        PlayerMove.instance.transform.localPosition = playerPos;
        moveCoroutine = StartCoroutine(MoveUpCoroutine(endPosition, originalZ));
    }

    public void MoveToggle()
    {
        if (Mathf.Approximately(transform.localPosition.x, positions[0].startPosition.x) && Mathf.Approximately(transform.localPosition.y, positions[0].startPosition.y))
        {
            Move(positions[0].startPosition, positions[0].endPosition);
        }
        else
        {
            Move(positions[0].endPosition, positions[0].startPosition);
        }
    }

    IEnumerator MoveUpCoroutine(Vector3 targetPos, float originalZ)
    {
        currentTargetPosition = targetPos;

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
        Vector3 pos = PlayerMove.instance.transform.localPosition;
        pos.z = originalZ;
        PlayerMove.instance.transform.localPosition = pos;
        PlayerMove.instance.DisableRequests--;
        PlayerMove.instance.velocity = Vector2.zero;
        onArrived.Invoke();

        if (TryGetComponent(out Interactable interactable))
        {
            interactable.Enable();
        }

        moveCoroutine = null;
    }

    SaveData ISaveable.Save()
    {
        return new ElevatorSaveData()
        {
            position = moveCoroutine == null ? (Vector2)transform.localPosition : currentTargetPosition
        };
    }

    public void Load(SaveData baseSaveData)
    {
        var saveData = (ElevatorSaveData)baseSaveData;
        currentTargetPosition = saveData.position;
        transform.localPosition = new Vector3(saveData.position.x, saveData.position.y, transform.localPosition.z);
    }
}