using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    static Stack<DialogueBox> pool;

    public GameObject dialogueBoxPrefab;
    public RectTransform canvas;

    void Awake()
    {
        instance = this;
        pool = new Stack<DialogueBox>(2);
    }

    /// <summary>
    /// Request a dialogue box from the pool
    /// </summary>
    public static DialogueBox GetBox()
    {
        if (pool.Count == 0)
        {
            pool.Push(Instantiate(instance.dialogueBoxPrefab, instance.canvas).GetComponent<DialogueBox>());
        }

        return pool.Pop();
    }
}