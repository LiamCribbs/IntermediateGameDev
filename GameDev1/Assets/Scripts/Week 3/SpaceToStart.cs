using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceToStart : MonoBehaviour
{
    public Pigeon.Button button;

    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            enabled = false;
            button.SetHover(true);
            Time.timeScale = 1f;
        }
    }
}