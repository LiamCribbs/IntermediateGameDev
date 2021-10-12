using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager instance;
    public static new Camera camera;

    static Stack<InteractableBox> pool;

    public GameObject boxPrefab;
    public RectTransform canvas;

    void Awake()
    {
        instance = this;
        pool = new Stack<InteractableBox>(2);

        camera = Camera.main;
    }

    /// <summary>
    /// Request a box from the pool
    /// </summary>
    public static InteractableBox GetBox()
    {
        if (pool.Count == 0)
        {
            pool.Push(Instantiate(instance.boxPrefab, instance.canvas).GetComponent<InteractableBox>());
        }

        var box = pool.Pop();
        box.gameObject.SetActive(true);

        return box;
    }

    /// <summary>
    /// Return a box to the pool
    /// </summary>
    public static void ReturnBox(InteractableBox box)
    {
        box.gameObject.SetActive(false);
        pool.Push(box);
    }
}