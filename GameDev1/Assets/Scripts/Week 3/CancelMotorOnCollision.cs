using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelMotorOnCollision : MonoBehaviour
{
    public HingeJoint2D joint;
    public Rigidbody2D nextJoint;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (joint.useMotor)
        {
            joint.useMotor = false;
            nextJoint.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}