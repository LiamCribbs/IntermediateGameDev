using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractableBox : MonoBehaviour
{
    public CanvasGroup group;

    public TextMeshProUGUI title;

    const float Padding = 60f;

    public void SetText(string text)
    {
        title.text = text;
        title.ForceMeshUpdate();

        Vector2 bounds = title.textBounds.size;

        bounds.x += Padding;
        bounds.y += Padding;
        ((RectTransform)group.transform).sizeDelta = bounds;
    }
}