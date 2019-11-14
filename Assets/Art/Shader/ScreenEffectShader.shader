Shader "Custom/ScreenEffectShader"
{
	Properties
	{
		_MainTex ("Captured Texture", 2D) = "white" {}
		[PerRendererData] _Depth("Depth", float) = 0
		[HideInInspector][PreRendererData] _DepthStrenght("Depth Strenght", float) = 1

		[HideInInspector][PreRendererData] _BlurPower("Blur Power", float) = 0
		[HideInInspector][PreRendererData] _BlurRange("Blur Range", int) = 0
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
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			
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
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;

				return OUT;
			}

			sampler2D _MainTex;
			float _Depth;
			float _DepthStrenght;

			float _BlurPower;
			int _BlurRange;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = IN.color;
				float2 nUV = 0.5 - abs(IN.texcoord - 0.5);

				float horizontalA = 1 - min(1, nUV.x / _Depth);
				float verticalA = 1 - min(1, nUV.y / _Depth);

				float hvA = max(horizontalA + verticalA, 0.001);

				c.a = min(1, lerp(horizontalA, verticalA, verticalA / hvA) * _DepthStrenght) * IN.color.a;

				half3 bluredColor = half3(0,0,0);
				int totalBlurSteps = _BlurRange * 2 + 1;
				totalBlurSteps *= totalBlurSteps;

				for(int x = - _BlurRange; x <= _BlurRange; x++)
				{
					for(int y = - _BlurRange; y <= _BlurRange; y++)
					{
						bluredColor += tex2D(_MainTex, IN.texcoord + float2(x * 0.0015, y * 0.0015)).rgb;
					}
				}

				bluredColor /= totalBlurSteps;

				float totalAlpha = _BlurPower + c.a;
				fixed3 finalColor = lerp(bluredColor, c.rgb, c.a / totalAlpha);
				fixed finalAlpha = max(_BlurPower, c.a);

				return fixed4(finalColor * finalAlpha, finalAlpha);
			}
		ENDCG
		}
	}
}
