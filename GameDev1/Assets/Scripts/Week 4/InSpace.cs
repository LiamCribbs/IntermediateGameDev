using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSpace : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        
    }

    public void OnEntryAnimationComplete()
    {
        Destroy(PlayerMove.instance.GetComponent<Animation>());
        PlayerMove.instance.DisableRequests = 0;
    }
}