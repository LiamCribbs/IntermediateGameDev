using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pigeon;
using Pigeon.Math;

public class DialogueEmitter : MonoBehaviour
{
    public Vector3 dialogueBoxOffset;
    public Vector2 cameraBoundsCheckPadding;

    public float visibleDistance = 4f;
    public float visibleFalloffDistance = 2f;

    [System.NonSerialized] protected DialogueBox dialogueBox;
    [System.NonSerialized] public DialogueData currentDialogue;

    [Space(30)]
    public Predicates startDialoguePredicate;
    protected bool dialogueStarted;

    [Space(30)]
    public DialogueData[] dialogue;

    const float WaitTimeAfterWritingShort = 0.5f;
    const float WaitTimeAfterWritingMedium = 1.5f;

    public void ShowDialogue(int id)
    {
        ShowDialogue(dialogue[id]);
    }

    public void ShowDialogue(DialogueData data)
    {
        currentDialogue = data;

        if (dialogueBox == null)
        {
            dialogueBox = DialogueManager.GetBox();
        }

        data.onStart?.Invoke(this);
        dialogueBox.Initialze(this, data);
    }

    public void ShowNextDialogue()
    {
        ShowDialogue(dialogue[System.Array.IndexOf(dialogue, currentDialogue) + 1]);
    }

    public void ShowRandomDialogue()
    {
        DialogueData data;
        while (true)
        {
            data = dialogue[UnityEngine.Random.Range(0, dialogue.Length)];
            if (data != currentDialogue)
            {
                break;
            }
        }

        ShowDialogue(data);
    }

    public void ReturnBox()
    {
        DialogueManager.ReturnBox(dialogueBox);
        dialogueBox = null;
    }

    public bool IsLeftClickPressed()
    {
        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    public bool WaitAfterWriting(float waitTime)
    {
        return dialogueBox.lastFinishedWritingTime > 0f && Time.unscaledTime - dialogueBox.lastFinishedWritingTime > waitTime;
    }

    public bool WaitAfterWritingShort()
    {
        return dialogueBox.lastFinishedWritingTime > 0f && Time.unscaledTime - dialogueBox.lastFinishedWritingTime > WaitTimeAfterWritingShort;
    }

    public bool WaitAfterWritingMedium()
    {
        return dialogueBox.lastFinishedWritingTime > 0f && Time.unscaledTime - dialogueBox.lastFinishedWritingTime > WaitTimeAfterWritingMedium;
    }

    public bool IsPlayerWithinRange()
    {
        return ((Vector2)DialogueManager.instance.player.transform.position - (Vector2)transform.position).MagFast() < visibleDistance + visibleFalloffDistance;
    }

    public bool False() => false;
    
    public bool IsWithinCameraBounds()
    {
        float width = (float)Screen.width / Screen.height * FollowCamera.instance.camera.orthographicSize;
        Vector2 camSize = new Vector2(width, width * Screen.height / Screen.width);
        Vector2 camPos = FollowCamera.instance.transform.position;
        Vector2 pos = transform.position;

        return pos.x < camPos.x + camSize.x - cameraBoundsCheckPadding.x && pos.x > camPos.x - camSize.x + cameraBoundsCheckPadding.x &&
            pos.y < camPos.y + camSize.y - cameraBoundsCheckPadding.y && pos.y > camPos.y - camSize.y + cameraBoundsCheckPadding.y;
    }

    void Update()
    {
        if (!dialogueStarted && startDialoguePredicate.Invoke(this))
        {
            dialogueStarted = true;
            ShowDialogue(dialogue[0]);
        }

        SetBoxAlpha();
    }

    void SetBoxAlpha()
    {
        if (!dialogueBox)
        {
            return;
        }

        float distance = ((Vector2)PlayerMove.instance.transform.position - (Vector2)transform.position).MagFast();

        // Set alpha according to distance from player
        if (distance < visibleDistance)
        {
            // Within range
            dialogueBox.group.alpha = 1f;
            if (!dialogueBox.gameObject.activeSelf)
            {
                dialogueBox.gameObject.SetActive(true);
            }
        }
        else if (distance > visibleDistance + visibleFalloffDistance)
        {
            // Out of range
            dialogueBox.group.alpha = 0f;
            if (dialogueBox.gameObject.activeSelf)
            {
                dialogueBox.gameObject.SetActive(false);
            }
        }
        else
        {
            // Fade when close to out of range
            dialogueBox.group.alpha = EaseFunctions.EaseInOutQuartic(1f - Mathf.InverseLerp(visibleDistance, visibleDistance + visibleFalloffDistance, distance));
            if (!dialogueBox.gameObject.activeSelf)
            {
                dialogueBox.gameObject.SetActive(true);
            }
        }
    }
}