using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    InteractableBox box;
    Coroutine fadeCoroutine;

    public Vector2 boxOffset;
    public float startOffset = 1f;
    Vector2 currentBoxOffset;

    public KeyCode interactKey = KeyCode.E;

    [System.NonSerialized] public bool inRange;
    public bool disabled;
    public bool disableOnInteract = true;

    [Space(20)]
    public UnityEngine.Events.UnityEvent<Interactable> onInteract;

    void OnEnable()
    {
        currentBoxOffset = boxOffset;
        currentBoxOffset.y -= startOffset;
    }

    public void DisableInteractions()
    {
        disabled = true;
    }

    void Update()
    {
        if (!disabled)
        {
            if (inRange && Input.GetKeyDown(interactKey))
            {
                onInteract?.Invoke(this);

                if (disableOnInteract)
                {
                    disabled = true;
                    Disable();
                }
            }

            if (box != null)
            {
                box.transform.position = DialogueManager.camera.WorldToScreenPoint(transform.position + (Vector3)currentBoxOffset);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!disabled && collision == PlayerMove.instance.collider)
        {
            Enable();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!disabled && collision == PlayerMove.instance.collider)
        {
            Disable();
        }
    }

    public void Enable()
    {
        inRange = true;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        if (box == null)
        {
            box = InteractableManager.GetBox();
        }

        fadeCoroutine = StartCoroutine(FadeBoxIn());
    }

    public void Disable()
    {
        inRange = false;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeBoxOut());
    }

    IEnumerator FadeBoxIn()
    {
        float initialAlpha = box.group.alpha;
        float initialOffset = currentBoxOffset.y;
        float time = 0f;

        while (time < 1f)
        {
            time += 4f * Time.unscaledDeltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            float t = Pigeon.EaseFunctions.EaseInOutQuintic(time);
            box.group.alpha = Mathf.Lerp(initialAlpha, 1f, t);
            currentBoxOffset.y = Mathf.Lerp(initialOffset, boxOffset.y, t);

            yield return null;
        }

        fadeCoroutine = null;
    }

    IEnumerator FadeBoxOut()
    {
        float initialAlpha = box.group.alpha;
        float initialOffset = currentBoxOffset.y;
        float time = 0f;

        while (time < 1f)
        {
            time += 4f * Time.unscaledDeltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            float t = Pigeon.EaseFunctions.EaseInOutQuintic(time);
            box.group.alpha = Mathf.Lerp(initialAlpha, 0f, t);
            currentBoxOffset.y = Mathf.Lerp(initialOffset, boxOffset.y - startOffset, t);

            yield return null;
        }

        InteractableManager.ReturnBox(box);
        box = null;

        fadeCoroutine = null;
    }
}