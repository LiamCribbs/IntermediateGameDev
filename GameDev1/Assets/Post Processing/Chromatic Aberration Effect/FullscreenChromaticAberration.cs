using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(FullscreenChromaticAberrationRenderer), PostProcessEvent.AfterStack, "Custom/Fullscreen Chromatic Aberration")]
public sealed class FullscreenChromaticAberration : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Intensity")]
    public FloatParameter intensity = new FloatParameter { value = 0.01f };
}

public sealed class FullscreenChromaticAberrationRenderer : PostProcessEffectRenderer<FullscreenChromaticAberration>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/FullscreenChromaticAberration"));
        sheet.properties.SetFloat("_Intensity", settings.intensity * 0.1f);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}