using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonActions : MonoBehaviour
{
    public UltEvents.UltEvent<Interactable> action;

    public void Invoke(Interactable interactable)
    {
        action.Invoke(interactable);
    }

    public void Invoke()
    {
        action.Invoke(null);
    }

    public void SetSpriteColor(SpriteRenderer renderer, Color color)
    {
        renderer.color = color;
    }

    public void ActivateDialogue(DialogueEmitter emitter, int index)
    {
        emitter.QueueDialogueOverwrite(index);
    }
}