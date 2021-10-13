using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu()]
public class Scene : ScriptableObject
{
    public SceneReference scene;

    public UnityEvent[] entrances;
}