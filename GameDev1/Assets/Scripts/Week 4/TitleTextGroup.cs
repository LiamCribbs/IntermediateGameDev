using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleTextGroup : MonoBehaviour
{
    public static bool deactivated;

    public CanvasGroup group;
    public float fadeSpeed;
    public Pigeon.Button button;

    void Awake()
    {
        if (deactivated)
        {
            gameObject.SetActive(false);
        }
    }

    public void Deactivate()
    {
        if (!deactivated)
        {
            deactivated = true;
            StartCoroutine(DeactivateCoroutine());
        }
    }

    IEnumerator DeactivateCoroutine()
    {
        button.SetHover(true);

        float time = 0f;

        while (time < 1f)
        {
            time += fadeSpeed * Time.deltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            group.alpha = Pigeon.EaseFunctions.EaseInOutQuartic(1f - time);
            yield return null;
        }
    }
}