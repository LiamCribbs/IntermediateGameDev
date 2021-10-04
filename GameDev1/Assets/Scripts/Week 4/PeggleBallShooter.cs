using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeggleBallShooter : MonoBehaviour
{
    public Rigidbody2D ball;
    Vector3 ballStartPosition;

    void Start()
    {
        ballStartPosition = ball.transform.localPosition;
    }

    /// <summary>
    /// Resets ball position
    /// </summary>
    public void ResetBall()
    {
        ball.velocity = Vector2.zero;
        ball.angularVelocity = 0f;
        ball.simulated = false;

        ball.transform.SetParent(transform);
        ball.transform.localPosition = ballStartPosition;
        ball.transform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        
    }
}