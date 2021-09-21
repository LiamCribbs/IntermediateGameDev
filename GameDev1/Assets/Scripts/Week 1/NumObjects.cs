using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumObjects : MonoBehaviour
{
    void Start()
    {
        print(FindObjectsOfType<GameObject>().Length);
    }
}
