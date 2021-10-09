using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pigeon;
using Pigeon.Math;

public class DialogueEmitter : MonoBehaviour
{
    public Vector3 dialogueBoxOffset;

    public float visibleDistance = 4f;
    public float visibleFalloffDistance = 2f;

    [System.NonSerialized] DialogueBox dialogueBox;
    [System.NonSerialized] public DialogueData currentDialogue;

    [Space(30)]
    public DialogueData[] dialogue;

    public void ShowDialogue(string id)
    {
        for (int i = 0; i < dialogue.Length; i++)
        {
            if (dialogue[i].id == id)
            {
                ShowDialogue(dialogue[i]);
                return;
            }
        }

        throw new System.Exception($"Dialogue not found with id of '{id}'");
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

    public bool IsLeftClickPressed()
    {
        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    void Start()
    {
        ShowDialogue(dialogue[0]);
    }

    void Update()
    {
        SetBoxAlpha();
    }

    void SetBoxAlpha()
    {
        if (!dialogueBox)
        {
            return;
        }

        float distance = (DialogueManager.instance.player.transform.position - transform.position).MagFast();

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