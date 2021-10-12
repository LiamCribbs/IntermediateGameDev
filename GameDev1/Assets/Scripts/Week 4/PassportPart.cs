using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassportPart : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == PlayerMove.instance.collider)
        {
            GameManager.instance.CollectPart();
            Destroy(gameObject);
        }
    }
}