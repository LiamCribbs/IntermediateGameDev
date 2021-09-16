using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZSorter : MonoBehaviour
{
    public Transform[] items;

    [ContextMenu("Sort")]
    public void JitterColor()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].transform.localPosition += new Vector3(0f, 0f, i * -0.1f);
        }
    }
}