using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHDRColor : MonoBehaviour
{
    static MaterialPropertyBlock properties;
    static readonly int IntensityID = Shader.PropertyToID("_Intensity");

    public SpriteRenderer sprite;
    public float intensity;

    public void SetIntensity(float intensity)
    {
        this.intensity = intensity;
        properties.SetFloat(IntensityID, intensity);
        sprite.SetPropertyBlock(properties);
    }

    void Awake()
    {
        properties = new MaterialPropertyBlock();
        properties.SetFloat(IntensityID, intensity);
        sprite.SetPropertyBlock(properties);
    }

    void OnValidate()
    {
        properties = new MaterialPropertyBlock();
        properties.SetFloat(IntensityID, intensity);
        sprite.SetPropertyBlock(properties);
    }

    void Reset()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
}