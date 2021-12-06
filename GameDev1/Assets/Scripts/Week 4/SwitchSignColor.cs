using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchSignColor : MonoBehaviour
{
    public float duration = 1f;

    public TMPro.TMP_Text text;
    public Color[] textColors;

    public new Light light;
    public Color[] lightColors;

    void Start()
    {
        StartCoroutine(SwitchColors());
    }

    IEnumerator SwitchColors()
    {
        WaitForSeconds wait = new WaitForSeconds(duration);
        int i = -1;

        while (true)
        {
            i = (i + 1) % textColors.Length;

            text.color = textColors[i];
            light.color = lightColors[i];

            yield return wait;
        }
    }
}