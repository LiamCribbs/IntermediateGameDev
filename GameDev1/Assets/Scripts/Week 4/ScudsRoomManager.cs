using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScudsRoomManager : MonoBehaviour, ISaveable
{
    public DialogueEmitter fallGuy;
    public Animator fallGuyAnimator;
    public GameObject fallGuyTrigger;

    public bool HasFallGuyFallen { get; set; }

    public void PlayFallAnimation()
    {
        fallGuyAnimator.SetBool("Fall", true);
        HasFallGuyFallen = true;
        fallGuyTrigger.SetActive(false);
    }

    public SaveData Save()
    {
        return new ScudsRoomManagerSaveData()
        {
            hasFallGuyFallen = HasFallGuyFallen
        };
    }

    public void Load(SaveData baseSaveData)
    {
        ScudsRoomManagerSaveData saveData = (ScudsRoomManagerSaveData)baseSaveData;
        HasFallGuyFallen = saveData.hasFallGuyFallen;

        if (HasFallGuyFallen)
        {
            fallGuyAnimator.Play("FallOffBanister", 0, 1f);
            fallGuyTrigger.SetActive(false);
        }
    }
}