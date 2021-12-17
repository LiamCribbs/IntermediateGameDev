using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassportShredder : DialogueEmitter
{
    public static bool hasTripped;
    public Transform passport;

    public float dialogueWaitTime;
    public float shakeTime;
    public float dropPassportTime;
    public float shakeInterval;
    public float shakeIntensity;

    public ParticleSystem particles0, particles1;

    public Transform sprite;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTripped && other.GetComponent<PlayerMove>() == PlayerMove.instance)
        {
            hasTripped = true;
            StartCoroutine(ActivateShredder());
        }
    }

    IEnumerator ActivateShredder()
    {
        PlayerMove.instance.DisableRequests++;

        StartCoroutine(Shake());
        particles0.Play();
        particles1.Play();

        yield return new WaitForSeconds(dialogueWaitTime);

        ShowDialogue(dialogue[0]);

        dialogueBox.GetComponent<UnityEngine.UI.Graphic>().color = new Color(0.2051887f, 0.5992559f, 0.8207547f);
        dialogueBox.transform.GetChild(0).GetComponent<UnityEngine.UI.Graphic>().color = new Color(0.8392157f, 0.8235294f, 0.7098039f);
        dialogueBox.textMesh.color = new Color(0.1019608f, 0.1019608f, 0.1019608f);

        //PlayerMove.instance.DisableRequests--;
    }

    public void ResetDialogueBoxColors()
    {
        dialogueBox.GetComponent<UnityEngine.UI.Graphic>().color = new Color(0.8392157f, 0.8235294f, 0.7098039f);
        dialogueBox.transform.GetChild(0).GetComponent<UnityEngine.UI.Graphic>().color = new Color(0.1019608f, 0.1019608f, 0.1019608f);
        dialogueBox.textMesh.color = new Color(0.8392157f, 0.8235294f, 0.7098039f);
    }

    IEnumerator Shake()
    {
        var wait = new WaitForSeconds(shakeInterval);
        Vector3 pos = sprite.localPosition;
        bool droppedPassport = false;
        float time = 0f;

        while (time < shakeTime)
        {
            sprite.localPosition = pos + (Vector3)(Random.insideUnitCircle * shakeIntensity);

            time += shakeInterval;

            if (!droppedPassport && time > dropPassportTime)
            {
                droppedPassport = true;
                DropPassport();
            }

            yield return wait;
        }

        transform.localPosition = pos;
    }

    void DropPassport()
    {
        passport.gameObject.SetActive(true);
        passport.transform.position = PlayerMove.instance.transform.position;
        passport.SetParent(null);
        Rigidbody2D rigibody = passport.gameObject.AddComponent<Rigidbody2D>();
        rigibody.gravityScale = -2f;
        rigibody.velocity = new Vector2(5f, 0.5f);
    }
}