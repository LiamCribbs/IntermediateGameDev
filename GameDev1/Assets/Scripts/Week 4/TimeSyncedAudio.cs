using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSyncedAudio : MonoBehaviour
{
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource.time = Time.time % audioSource.clip.length;
        audioSource.Play();
    }
}