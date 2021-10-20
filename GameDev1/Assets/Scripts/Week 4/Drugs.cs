using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Pigeon;

public class Drugs : MonoBehaviour
{
    Coroutine doDrugsCoroutine;

    public PostProcessProfile postProcess;
    Bloom bloom;
    Pixelate pixelate;

    public Texture2D bloomTex;

    [Space(10)]
    public float bloomIntensity;
    public float pixelateIntensity;
    public AnimationCurve pixelateCurve;
    public float startSpeed;

    public void StartDoDrugs()
    {
        bloom = postProcess.GetSetting<Bloom>();
        pixelate = postProcess.GetSetting<Pixelate>();

        DontDestroyOnLoad(gameObject);
        doDrugsCoroutine = StartCoroutine(DoDrugs());
    }

    IEnumerator DoDrugs()
    {
        bloom.dirtTexture.overrideState = true;
        bloom.dirtIntensity.overrideState = true;
        bloom.dirtTexture.value = bloomTex;

        float initBloom = bloom.dirtIntensity.value;
        float initPixelate = pixelate.intensity.value.x;
        float time = 0f;

        while (time < 1f)
        {
            time += startSpeed * Time.unscaledDeltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            bloom.dirtIntensity.value = Mathf.LerpUnclamped(initBloom, bloomIntensity, EaseFunctions.EaseInExponential(time));
            float pixelateValue = Mathf.LerpUnclamped(initPixelate, pixelateIntensity, pixelateCurve.Evaluate(time));
            pixelate.intensity.value = new Vector4(pixelateValue, pixelateValue, pixelate.intensity.value.z, pixelate.intensity.value.w);

            yield return null;
        }

        yield return new WaitForSecondsRealtime(30f);

        time = 0f;

        while (time < 1f)
        {
            time += startSpeed * Time.unscaledDeltaTime;
            if (time > 1f)
            {
                time = 1f;
            }

            bloom.dirtIntensity.value = Mathf.LerpUnclamped(bloomIntensity, initBloom, EaseFunctions.EaseInExponential(time));
            float pixelateValue = Mathf.LerpUnclamped(pixelateIntensity, initPixelate, EaseFunctions.EaseInOutQuartic(time));
            pixelate.intensity.value = new Vector4(pixelateValue, pixelateValue, pixelate.intensity.value.z, pixelate.intensity.value.w);

            yield return null;
        }

        doDrugsCoroutine = null;
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (bloom != null)
        {
            bloom.dirtTexture.value = null;
            bloom.dirtTexture.overrideState = false;
            bloom.dirtIntensity.overrideState = false;

            bloom.dirtIntensity.value = 0f;
            pixelate.intensity.value = new Vector4(0.007f, 0.007f, 1.777778f, 9f);
        }
    }
}