using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageChute : MonoBehaviour
{
    Vector3 position;
    public float shakeIntensity = 2f;

    public AnimationClip suckAnimation;
    static bool suckedPlayer;

    void Start()
    {
        position = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = position + (Vector3)(Random.insideUnitCircle * shakeIntensity);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!suckedPlayer && collision == PlayerMove.instance.collider)
        {
            suckedPlayer = true;
            PlayerMove.instance.DisableRequests++;
            FollowCamera.instance.focusSpeed = 4.25f;

            var animation = PlayerMove.instance.gameObject.AddComponent<Animation>();
            animation.clip = suckAnimation;
            animation.AddClip(suckAnimation, "suck");
            animation.Play("suck");
        }
    }
}