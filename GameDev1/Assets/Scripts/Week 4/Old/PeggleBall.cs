using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeggleBall : MonoBehaviour
{
    public new Rigidbody2D rigidbody;
    public float force = 1f;

    void Start()
    {
        rigidbody.simulated = false;
    }

    void Update()
    {
        MouseControls();
    }

    void MouseControls()
    {
        if (!rigidbody.simulated && Input.GetKeyDown(KeyCode.Mouse0))
        {
            rigidbody.simulated = true;
            rigidbody.transform.parent = null;
            rigidbody.AddForce(transform.up * -force, ForceMode2D.Impulse);
        }
    }
}