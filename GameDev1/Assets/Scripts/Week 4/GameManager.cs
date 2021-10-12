using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static int partsCollected;
    public TMPro.TextMeshProUGUI partsCollectedText;
    public TMPro.TextMeshProUGUI partsCollectedTextAlien;
    public GameObject winText;

    void Awake()
    {
        instance = this;
    }

    public void CollectPart()
    {
        partsCollected++;
        partsCollectedText.text = partsCollected.ToString();
        partsCollectedTextAlien.text = partsCollectedText.text;

        if (partsCollected == 2)
        {
            winText.SetActive(true);
        }
    }
}