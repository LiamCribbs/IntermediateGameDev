using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class ColorChangeGraphic : MonoBehaviour
{
    const int ListCapacity = 64;

    public static Dictionary<int, List<Graphic>> graphics = new Dictionary<int, List<Graphic>>();

    public Graphic graphic;
    public int listIndex;

    void Reset()
    {
        graphic = GetComponent<Graphic>();
    }

    void Awake()
    {
        if (!graphics.ContainsKey(listIndex))
        {
            graphics.Add(listIndex, new List<Graphic>(ListCapacity));
        }

        graphics[listIndex].Add(graphic);
    }

    public static void SetColor(int list, Color color)
    {
        var graphics = ColorChangeGraphic.graphics[list];
        for (int i = 0; i < graphics.Count; i++)
        {
            graphics[i].color = color;
        }
    }

    public static void Clear()
    {
        graphics.Clear();
    }
}