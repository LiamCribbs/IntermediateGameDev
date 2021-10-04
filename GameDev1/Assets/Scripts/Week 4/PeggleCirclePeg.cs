using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeggleCirclePeg : MonoBehaviour
{
    public Color hitColor;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = hitColor;
    }
}