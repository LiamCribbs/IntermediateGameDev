using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripEvent : DialogueEmitter
{
    public bool hasTripped;

    public AnimationCurve fallRotateCurve;

    public Transform passport;

    void Start()
    {
        passport.SetParent(PlayerMove.instance.sprite);
        passport.localPosition = new Vector3(0f, 0f, -0.05f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTripped && other.GetComponent<PlayerMove>() == PlayerMove.instance)
        {
            hasTripped = true;
            StartCoroutine(Trip());
        }
    }

    IEnumerator Trip()
    {
        PlayTripAnimation();

        yield return new WaitForSeconds(0.1f);

        DropPassport();

        yield return new WaitForSeconds(1f);

        ShowDialogue(dialogue[0]);
    }

    void PlayTripAnimation()
    {
        PlayerMove.instance.DisableRequests++;
        StartCoroutine(RotatePlayer());
    }

    IEnumerator RotatePlayer()
    {
        float time = 0f;

        while (time < 1f)
        {
            time += 2f * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            PlayerMove.instance.sprite.localRotation = Quaternion.Euler(0f, 0f, -90f * fallRotateCurve.Evaluate(time));
            yield return null;
        }
    }

    IEnumerator RotatePlayerUp()
    {
        float time = 0f;

        while (time < 1f)
        {
            time += 0.4f * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            PlayerMove.instance.sprite.localRotation = Quaternion.Euler(0f, 0f, -90f * (1f - Pigeon.EaseFunctions.EaseInOutQuintic(time)));
            yield return null;
        }

        PlayerMove.instance.DisableRequests--;
    }

    public void ReturnToNormalState()
    {
        StartCoroutine(RotatePlayerUp());
        ReturnBox();
    }

    void DropPassport()
    {
        passport.gameObject.SetActive(true);
        passport.SetParent(null);
        passport.gameObject.AddComponent<Rigidbody2D>();
    }
}