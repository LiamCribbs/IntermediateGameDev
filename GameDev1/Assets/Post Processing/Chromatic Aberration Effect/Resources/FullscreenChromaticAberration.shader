Shader "Hidden/Custom/FullscreenChromaticAberration"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float _Intensity;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float2 uv = i.texcoord;
            float colR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord - _Intensity).r;
            float colG = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).g;
            float colB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + _Intensity).b;
            return float4(colR, colG, colB, 1);
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}