using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DialogueSet : ScriptableObject
{
    [TextArea] public List<string> dialogue;

    public void SetTextRandom(DialogueEmitter emitter)
    {
        emitter.currentDialogue.text = dialogue[Random.Range(0, dialogue.Count)];
    }

    public void ReloadThisDialogueAfterTime(DialogueEmitter emitter)
    {
        emitter.StartCoroutine(emitter.ReloadDialogueAfterRandTime(1f, 10f));
    }
}