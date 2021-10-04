using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetZone : MonoBehaviour
{
    public PeggleBallShooter shooter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PeggleBall ball))
        {
            shooter.ResetBall();
        }
    }
}