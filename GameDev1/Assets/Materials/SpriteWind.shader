Shader "Sprites/SpriteWind"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_WindIntensity ("Wind Intensity", Vector) = (1, 1, 1, 1)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			uint hashUint(uint state)
            {
                state ^= 2747636419u;
                state *= 2654435769u;
                state ^= state >> 16;
                state *= 2654435769u;
                state ^= state >> 16;
                state *= 2654435769u;
                return state;
            }

            float hash(uint state)
            {
                state ^= 2747636419u;
                state *= 2654435769u;
                state ^= state >> 16;
                state *= 2654435769u;
                state ^= state >> 16;
                state *= 2654435769u;
                return state / 4294967295.0;
            }

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			float4 _WindIntensity;
			float _Step = 0.0001;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 uv = IN.texcoord;
				//uv += (hash((uv + _Time.y) * _WindIntensity.z) - 0.5) * _WindIntensity.xy;
				uv += (sin(_Time.y * _WindIntensity.z) * (hash(uv.x * _Time.y * _WindIntensity.w) - 0.5) * _WindIntensity);
				
				fixed4 c = SampleSpriteTexture (uv) * IN.color;
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}