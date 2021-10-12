using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public static new Camera camera;

    static Stack<DialogueBox> pool;

    public GameObject dialogueBoxPrefab;
    public RectTransform canvas;

    public PlayerMove player;

    public new AudioSource audio;
    public AudioClip[] writeClips;

    void Awake()
    {
        instance = this;
        pool = new Stack<DialogueBox>(2);

        camera = Camera.main;
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

        var box = pool.Pop();
        box.gameObject.SetActive(true);

        return box;
    }

    /// <summary>
    /// Return a dialogue box to the pool
    /// </summary>
    public static void ReturnBox(DialogueBox box)
    {
        box.gameObject.SetActive(false);
        pool.Push(box);
    }

    public void PlayWriteSound()
    {
        audio.PlayOneShot(writeClips[Random.Range(2, writeClips.Length)]);
    }

    public void PlayWriteSoundHeavy()
    {
        audio.PlayOneShot(writeClips[Random.Range(0, 2)]);
    }
}