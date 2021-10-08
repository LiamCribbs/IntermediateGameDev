using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEmitter : MonoBehaviour
{
    public DialogueBox dialogueBox;
    public DialogueData currentDialogue;

    public DialogueData[] dialogue;

    public void ShowDialogue(string id)
    {

    }

    public void ShowDialogue(DialogueData data)
    {
        currentDialogue = data;

        dialogueBox = DialogueManager.GetBox();
        data.onStart?.Invoke(this);
        dialogueBox.Initialze(data, OnFinishedWriting);
    }

    void OnFinishedWriting()
    {
        currentDialogue.onFinishedWriting?.Invoke(this);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}