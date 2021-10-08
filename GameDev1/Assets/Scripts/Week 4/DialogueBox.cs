using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogueBox : MonoBehaviour
{
    public new RectTransform transform;
    public TextMeshProUGUI textMesh;

    string text;

    [Space(10)]
    public float padding = 30f;
    public float resizeSpeed = 2.5f;
    Vector2 bounds;

    [Space(10)]
    public float writeDelay = 0.01f;
    Coroutine writeTextCoroutine;

    [Space(10)]
    public AnimationCurve moveEffectIntensityCurve;
    Coroutine moveEffectCoroutine;

    const float ShakeSoftIntensity = 1f;
    const float ShakeHardIntensity = 1f;
    const float FloatSoftIntensity = 1f;
    const float FloatSoftInterval = 1f;
    const float FloatHardIntensity = 1f;
    const float FloatHardInterval = 1f;

    public void Initialze(DialogueData data, System.Action onFinishedWriting)
    {
        SetText(data.text, onFinishedWriting);

        if (data.effect != DialogueData.Effect.None)
        {
            moveEffectCoroutine = StartCoroutine(
                data.effectDuration > 0f ? data.effect switch
                {
                    DialogueData.Effect.ShakeSoft => ShakeText(ShakeSoftIntensity),
                    DialogueData.Effect.ShakeHard => ShakeText(ShakeHardIntensity),
                    DialogueData.Effect.FloatSoft => FloatText(FloatSoftIntensity, FloatSoftInterval),
                    DialogueData.Effect.FloatHard => FloatText(FloatHardIntensity, FloatHardInterval)
                }
                :
                data.effect switch
                {
                    DialogueData.Effect.ShakeSoft => ShakeText(ShakeSoftIntensity, data.effectDuration, moveEffectIntensityCurve),
                    DialogueData.Effect.ShakeHard => ShakeText(ShakeHardIntensity, data.effectDuration, moveEffectIntensityCurve),
                    DialogueData.Effect.FloatSoft => FloatText(FloatSoftIntensity, FloatSoftInterval, data.effectDuration, moveEffectIntensityCurve),
                    DialogueData.Effect.FloatHard => FloatText(FloatHardIntensity, FloatHardInterval, data.effectDuration, moveEffectIntensityCurve)
                });
        }
    }

    public void SetText(string text, System.Action onFinishedWriting)
    {
        this.text = text;

        this.ResetCoroutine(ref moveEffectCoroutine);
        this.RestartCoroutine(ref writeTextCoroutine, WriteText(writeDelay, onFinishedWriting));
    }

    public void ClearText()
    {
        this.RestartCoroutine(ref writeTextCoroutine, ClearText(writeDelay));
    }

    void UpdateBounds()
    {
        textMesh.ForceMeshUpdate();

        Vector2 bounds = textMesh.textBounds.size;
        if (bounds.x < 0f)
        {
            bounds = Vector2.zero;
        }

        bounds.x += padding;
        bounds.y += padding;

        this.bounds = bounds;
    }

    void Update()
    {
        transform.sizeDelta = Vector2.Lerp(transform.sizeDelta, bounds, resizeSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetText("Hello there fellow human", null);
        }
    }

    /// <summary>
    /// Write text over time
    /// </summary>
    IEnumerator WriteText(float waitTime, System.Action onFinishedWriting)
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(waitTime);

        string typedText = "";

        for (int i = 0; i < text.Length; i++)
        {
            typedText += text[i];
            textMesh.text = typedText;
            UpdateBounds();

            yield return wait;
        }

        onFinishedWriting?.Invoke();

        writeTextCoroutine = null;
    }

    /// <summary>
    /// Clear text over time
    /// </summary>
    IEnumerator ClearText(float waitTime)
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(waitTime);

        string typedText = text;

        for (int i = 0; i < text.Length; i++)
        {
            typedText.Remove(typedText.Length - 1);
            textMesh.text = typedText;
            UpdateBounds();

            yield return wait;
        }

        writeTextCoroutine = null;
    }

    /// <summary>
    /// Shake effect
    /// </summary>
    IEnumerator ShakeText(float intensity)
    {
        while (true)
        {
            textMesh.transform.localPosition = Random.insideUnitCircle * intensity;
            yield return null;
        }
    }

    /// <summary>
    /// Shake effect for a duration
    /// </summary>
    IEnumerator ShakeText(float intensity, float duration, AnimationCurve intensityCurve)
    {
        float speed = 1f / duration;
        float time = 0f;

        while (time < 1f)
        {
            time += speed * Time.unscaledDeltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            textMesh.transform.localPosition = Random.insideUnitCircle * (intensity * intensityCurve.Evaluate(time));

            yield return null;
        }

        moveEffectCoroutine = null;
    }

    /// <summary>
    /// Floating effect
    /// </summary>
    IEnumerator FloatText(float intensity, float interval)
    {
        float speed = 1f / interval;

        while (true)
        {
            Vector2 initial = textMesh.transform.localPosition;
            Vector2 target = Random.insideUnitCircle * intensity;
            float time = 0f;

            while (time < 1f)
            {
                time += speed * Time.unscaledDeltaTime;
                if (time > 1f)
                {
                    time = 1f;
                }

                textMesh.transform.localPosition = Vector2.LerpUnclamped(initial, target, Pigeon.EaseFunctions.EaseInOutQuartic(time));
                yield return null;
            }
        }
    }

    /// <summary>
    /// Floating effect for a duration
    /// </summary>
    IEnumerator FloatText(float intensity, float interval, float duration, AnimationCurve intensityCurve)
    {
        float speed = 1f / interval;
        float totalTime = 0f;

        while (true)
        {
            Vector2 initial = textMesh.transform.localPosition;
            Vector2 target = Random.insideUnitCircle * intensity;
            float time = 0f;

            while (time < 1f)
            {
                time += speed * Time.unscaledDeltaTime;
                if (time > 1f)
                {
                    time = 1f;
                }

                totalTime += Time.unscaledDeltaTime;

                textMesh.transform.localPosition = Vector2.LerpUnclamped(initial, target, Pigeon.EaseFunctions.EaseInOutQuartic(time) * intensityCurve.Evaluate(time));

                if (totalTime >= duration)
                {
                    yield break;
                }

                yield return null;
            }
        }

        moveEffectCoroutine = null;
    }
}