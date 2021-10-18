using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class DialogueBox : MonoBehaviour
{
    public new RectTransform transform;
    public CanvasGroup group;
    public TextMeshProUGUI textMesh;

    DialogueEmitter emitter;
    Coroutine checkCompleteConditionCoroutine;
    bool canComplete;

    string text;

    [Space(10)]
    public float padding = 30f;
    public float resizeSpeed = 2.5f;
    Vector2 bounds;

    [Space(10)]
    public float writeDelay = 0.01f;
    Coroutine writeTextCoroutine;
    const int MaxClearIterations = 12;

    [Space(10)]
    public AnimationCurve moveEffectIntensityCurve;
    Coroutine moveEffectCoroutine;

    const float ShakeSoftIntensity = 2f;
    const float ShakeHardIntensity = 8f;
    const float ShakeWaitTime = 0.05f;
    static readonly WaitForSecondsRealtime shakeWaitForSeconds = new WaitForSecondsRealtime(ShakeWaitTime);
    const float FloatSoftIntensity = 15f;
    const float FloatSoftInterval = 0.8f;
    const float FloatHardIntensity = 15f;
    const float FloatHardInterval = 0.2f;

    [System.NonSerialized] public float lastFinishedWritingTime;
    static readonly WaitForSecondsRealtime pauseCharacterWaitTime = new WaitForSecondsRealtime(0.5f);

    const string AlienScriptStartTag = "<i><color=#ffc949>";
    const string AlienScriptEndTag = "</i></color>";

    public RectTransform answer1Transform;
    public CanvasGroup answer1Group;
    public TextMeshProUGUI answer1Text;
    public Pigeon.RectSizeButton answer1Button;

    public RectTransform answer2Transform;
    public CanvasGroup answer2Group;
    public TextMeshProUGUI answer2Text;
    public Pigeon.RectSizeButton answer2Button;

    public bool Active
    {
        get => gameObject.activeInHierarchy;
    }

    public void RestartCoroutine(ref Coroutine coroutine, IEnumerator method)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(method);
    }

    public void ResetCoroutine(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    new Coroutine StartCoroutine(IEnumerator routine)
    {
        return DialogueManager.instance.StartCoroutine(routine);
    }

    new void StopCoroutine(Coroutine routine)
    {
        DialogueManager.instance.StopCoroutine(routine);
    }

    public void Initialze(DialogueEmitter emitter, DialogueData data)
    {
        this.emitter = emitter;

        answer1Transform.gameObject.SetActive(false);
        answer2Transform.gameObject.SetActive(false);

        textMesh.transform.localPosition = Vector3.zero;
        canComplete = false;
        ResetCoroutine(ref checkCompleteConditionCoroutine);

        SetText(data.text);

        if (data.effect != DialogueData.Effect.None)
        {
            moveEffectCoroutine = StartCoroutine(
                data.effectDuration > 0f ? data.effect switch
                {
                    DialogueData.Effect.ShakeSoft => ShakeText(ShakeSoftIntensity, data.effectDuration, moveEffectIntensityCurve),
                    DialogueData.Effect.ShakeHard => ShakeText(ShakeHardIntensity, data.effectDuration, moveEffectIntensityCurve),
                    DialogueData.Effect.FloatSoft => FloatText(FloatSoftIntensity, FloatSoftInterval, data.effectDuration, moveEffectIntensityCurve),
                    DialogueData.Effect.FloatHard => FloatText(FloatHardIntensity, FloatHardInterval, data.effectDuration, moveEffectIntensityCurve)
                }
                :
                data.effect switch
                {
                    DialogueData.Effect.ShakeSoft => ShakeText(ShakeSoftIntensity),
                    DialogueData.Effect.ShakeHard => ShakeText(ShakeHardIntensity),
                    DialogueData.Effect.FloatSoft => FloatText(FloatSoftIntensity, FloatSoftInterval),
                    DialogueData.Effect.FloatHard => FloatText(FloatHardIntensity, FloatHardInterval)

                });
        }
    }

    public void SetText(string text)
    {
        this.text = text;
        lastFinishedWritingTime = -1f;

        ResetCoroutine(ref moveEffectCoroutine);
        RestartCoroutine(ref writeTextCoroutine, WriteText(writeDelay));
    }

    public void ClearText()
    {
        RestartCoroutine(ref writeTextCoroutine, ClearText(writeDelay));
    }

    Vector2 UpdateBounds(TextMeshProUGUI textMesh)
    {
        textMesh.ForceMeshUpdate();

        Vector2 bounds = textMesh.textBounds.size;
        if (bounds.x < 0f)
        {
            bounds = Vector2.zero;
        }

        bounds.x += padding;
        bounds.y += padding;

        return bounds;
    }

    void Update()
    {
        transform.sizeDelta = Vector2.Lerp(transform.sizeDelta, bounds, resizeSpeed * Time.deltaTime);

        transform.position = DialogueManager.camera.WorldToScreenPoint(emitter.transform.position + emitter.dialogueBoxOffset);

        if (canComplete && writeTextCoroutine ==  null)
        {
            canComplete = false;
            ClearText();
        }
    }

    IEnumerator CheckCompleteCondition()
    {
        while (true)
        {
            while (!Active)
            {
                yield return null;
            }

            if (emitter.currentDialogue.completeCondition.Invoke(emitter))
            {
                emitter.currentDialogue.onCompleteEvent.Invoke(emitter);
                checkCompleteConditionCoroutine = null;

                canComplete = true;

                yield break;
            }

            yield return null;
        }
    }

    public void Response1Pressed()
    {
        emitter.currentDialogue.onResponse1Chosen.Invoke(emitter);
        canComplete = true;
    }

    public void Response2Pressed()
    {
        emitter.currentDialogue.onResponse2Chosen.Invoke(emitter);
        canComplete = true;
    }

    public void QueueDialogue(DialogueData dialogue)
    {
        queuedData = dialogue;
        emitter.currentDialogue.onClearedEvent.action.AddListener(InitializeQueuedDialogue);
    }

    DialogueData queuedData;

    void InitializeQueuedDialogue(DialogueEmitter emitter)
    {
        emitter.currentDialogue.onClearedEvent.action.RemoveListener(InitializeQueuedDialogue);
        emitter.ShowDialogue(queuedData);
    }

    /// <summary>
    /// Write text over time
    /// </summary>
    IEnumerator WriteText(float waitTime)
    {
        // Start checking completion condition
        if (emitter.currentDialogue.alwaysCheckForCompletion)
        {
            checkCompleteConditionCoroutine = StartCoroutine(CheckCompleteCondition());
        }

        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(waitTime);

        string typedText = "";

        // Write text one char at a time
        for (int i = 0; i < text.Length; i++)
        {
            while (!Active)
            {
                yield return null;
            }

            if (i == 0)
            {
                DialogueManager.instance.PlayWriteSoundHeavy();
            }
            else
            {
                DialogueManager.instance.PlayWriteSound();
            }

            char letter = text[i];
            if (letter == '\\' && text[i + 1] == 'w')
            {
                yield return pauseCharacterWaitTime;

                i++;
                continue;
            }

            if (letter == '<')
            {
                int j;
                for (j = i + 1; j < text.Length; j++)
                {
                    if (text[j] == '>')
                    {
                        break;
                    }
                }

                string tag = text.Substring(i, j - i + 1);
                if (tag == "<i>")
                {
                    tag = AlienScriptStartTag;
                }
                else if (tag == "</i>")
                {
                    tag = AlienScriptEndTag;
                }

                typedText += tag;

                i = j;
            }
            else
            {
                typedText += text[i];
            }

            //typedText += text[i];
            textMesh.text = typedText;
            bounds = UpdateBounds(textMesh);

            yield return wait;
        }

        lastFinishedWritingTime = Time.unscaledTime;

        // On finished event
        emitter.currentDialogue.onFinishedWriting?.Invoke(emitter);

        // Start checking completion condition
        if (!emitter.currentDialogue.alwaysCheckForCompletion)
        {
            checkCompleteConditionCoroutine = StartCoroutine(CheckCompleteCondition());
        }

        if (!string.IsNullOrEmpty(emitter.currentDialogue.response1))
        {
            answer1Text.text = emitter.currentDialogue.response1;
            answer2Text.text = emitter.currentDialogue.response2;
            answer1Transform.gameObject.SetActive(true);
            answer2Transform.gameObject.SetActive(true);
            answer1Button.clickSize = new Vector2(UpdateBounds(answer1Text).x, 95f);
            answer2Button.clickSize = new Vector2(UpdateBounds(answer2Text).x, 95f);
            answer1Button.SetClick(true);
            answer2Button.SetClick(true);
        }

        writeTextCoroutine = null;
    }

    /// <summary>
    /// Clear text over time
    /// </summary>
    IEnumerator ClearText(float waitTime)
    {
        ResetCoroutine(ref checkCompleteConditionCoroutine);

        if (answer1Button.clicking)
        {
            answer1Button.SetClick(false);
            answer2Button.SetClick(false);
        }

        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(waitTime);

        text = textMesh.text;
        string typedText = text;

        int charsPerIteration = text.Length > MaxClearIterations ? text.Length / MaxClearIterations : 1;

        // Delete text one char at a time
        for (int i = 0; i < text.Length; i += charsPerIteration)
        {
            while (!Active)
            {
                yield return null;
            }

            int startIndex = typedText.Length - charsPerIteration;
            if (startIndex < 0)
            {
                charsPerIteration = typedText.Length;
                startIndex = 0;
            }

            typedText = typedText.Remove(startIndex, charsPerIteration);
            textMesh.text = typedText;
            bounds = UpdateBounds(textMesh);

            DialogueManager.instance.PlayWriteSound();

            yield return wait;
        }

        emitter.currentDialogue.onClearedEvent.Invoke(emitter);

        writeTextCoroutine = null;
    }

    /// <summary>
    /// Shake effect
    /// </summary>
    IEnumerator ShakeText(float intensity)
    {
        while (true)
        {
            while (!Active)
            {
                yield return null;
            }

            textMesh.transform.localPosition = Random.insideUnitCircle * intensity;
            yield return shakeWaitForSeconds;
        }
    }

    /// <summary>
    /// Shake effect for a duration
    /// </summary>
    IEnumerator ShakeText(float intensity, float duration, AnimationCurve intensityCurve)
    {
        float speed = 1f / duration;
        float time = 0f;
        float shakeWaitTime = 0f;

        while (time < 1f)
        {
            while (!Active)
            {
                yield return null;
            }

            shakeWaitTime += Time.unscaledDeltaTime;

            time += speed * Time.unscaledDeltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            if (shakeWaitTime > ShakeWaitTime)
            {
                shakeWaitTime = 0f;
                textMesh.transform.localPosition = Random.insideUnitCircle * (intensity * intensityCurve.Evaluate(time));
            }

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
                while (!Active)
                {
                    yield return null;
                }

                time += speed * Time.unscaledDeltaTime;
                if (time > 1f)
                {
                    time = 1f;
                }

                textMesh.transform.localPosition = Vector2.LerpUnclamped(initial, target, Pigeon.EaseFunctions.EaseInOutSin(time));
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
            while (!Active)
            {
                yield return null;
            }

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
                    moveEffectCoroutine = null;
                    yield break;
                }

                yield return null;
            }
        }
    }
}