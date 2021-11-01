using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour, ISaveable
{
    public Interactable interactable;

    public void PickupDoonge(int numDoonges)
    {
        GameManager.SetDoongeCounter(GameManager.doonges + numDoonges);
        InteractableManager.ReturnBox(interactable.box);
        gameObject.SetActive(false);
    }

    public void Load(SaveData saveData)
    {
        if (((CollectableSaveData)saveData).collected)
        {
            gameObject.SetActive(false);
        }
    }

    public SaveData Save()
    {
        return new CollectableSaveData()
        {
            collected = !gameObject.activeSelf
        };
    }
}