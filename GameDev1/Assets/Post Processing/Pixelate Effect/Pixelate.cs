using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PixelateRenderer), PostProcessEvent.BeforeStack, "Custom/Pixelate")]
public sealed class Pixelate : PostProcessEffectSettings
{
    [Range(0f, 1f)]
    public Vector4Parameter intensity = new Vector4Parameter();
}

public sealed class PixelateRenderer : PostProcessEffectRenderer<Pixelate>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Pixelate"));
        sheet.properties.SetVector("_Intensity", settings.intensity);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}