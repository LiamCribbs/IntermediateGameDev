using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public new Rigidbody2D rigidbody;
    public Rigidbody2D otherRigidbody;
    public Transform other;
    public float otherMass;

    public VertletRope rope;

    // Start is called before the first frame update
    void Awake()
    {
        rope.Setup();
    }

    void LateUpdate()
    {
        rope.startPosition = transform.position;
        rope.endPosition = other.transform.position;
        rope.DrawRope();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 currentVelocity = rigidbody.velocity * Time.fixedDeltaTime;
        Vector3 thisPosition = transform.position;
        Vector3 extrapolatedPosition = thisPosition + currentVelocity;
        Vector3 hookedObjectPosition = other.transform.position;
        float distanceFromHook = Vector3.Distance(extrapolatedPosition, hookedObjectPosition);

        if (distanceFromHook > rope.maxLength)
        {
            Vector3 positionToTest = (extrapolatedPosition - hookedObjectPosition).normalized;
            Vector3 newPosition = (positionToTest * rope.maxLength) + hookedObjectPosition;
            Vector3 newVelocity = newPosition - thisPosition;

            //Calculate tension force
            Vector3 deltaVelocity = newVelocity - currentVelocity;

            Vector3 tensionForce = deltaVelocity / Time.fixedDeltaTime;

            rigidbody.AddForceAtPosition((otherRigidbody ? otherRigidbody.mass : otherMass) * tensionForce, thisPosition);
            if (otherRigidbody)
            {
                otherRigidbody.AddForceAtPosition(-rigidbody.mass * tensionForce, hookedObjectPosition);
            }
        }

        rope.startPosition = transform.position;
        rope.endPosition = other.transform.position;
        rope.Simulate();
    }
}
