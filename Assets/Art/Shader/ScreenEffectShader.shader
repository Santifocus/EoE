Shader "Custom/ScreenEffectShader"
{
	Properties
	{
		_MainTex ("Captured Texture", 2D) = "white" {}

		_Color("Border Color", Color) = (1,1,1,1)
		_BorderDepth("Border Depth", float) = 0

		_TintColor("Tint Color", Color) = (1,1,1,1)

		_BlurPower("Blur Power", Range(0, 1)) = 0
		_BlurRange("Blur Range", int) = 0
		_BlurPixelDistance("Blur Pixel Distance", float) = 2
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
			float4 _MainTex_TexelSize;

			fixed4 _Color;
			fixed4 _TintColor;

			float _BorderDepth;

			float _BlurPower;
			int _BlurRange;
			float _BlurPixelDistance;

			fixed4 frag(v2f IN) : SV_Target
			{
				//Base col
				fixed4 col = tex2D(_MainTex, IN.texcoord);

				//Start with blur
				float xyScale = _MainTex_TexelSize.w / _MainTex_TexelSize.z;
				float xSteps = _BlurPixelDistance / _MainTex_TexelSize.z * xyScale;
				float ySteps = _BlurPixelDistance / _MainTex_TexelSize.z;

				fixed3 bluredColor = fixed3(0,0,0);
				float sampledCount = _BlurRange * 2 +1;
				sampledCount *= sampledCount;

				for(int x = -_BlurRange; x <= _BlurRange; x++)
				{
					for(int y = -_BlurRange; y <= _BlurRange; y++)
					{
						bluredColor += tex2D(_MainTex, IN.texcoord + float2(x * xSteps, y * ySteps));
					}
				}
				bluredColor /= sampledCount;
				col.rgb = lerp(col, bluredColor, _BlurPower);

				//Now add the overlays
				fixed4 overlayColor;

				//Border
				float2 nUV = (0.5 - abs(IN.texcoord - 0.5)) * 2; //On the sides == 0, middle = 1.0
				float xIntense = 1 - min(1, nUV.x / (_BorderDepth * xyScale));
				float yIntense = 1 - min(1, nUV.y / _BorderDepth);
				overlayColor = _Color * max(xIntense, yIntense);
				overlayColor.rgb *= overlayColor.a;
				
				//Tint
				float lerpPoint = _TintColor.a > 0.0 ? (_TintColor.a / (_TintColor.a + overlayColor.a)) : 0;
				overlayColor.rgb = lerp(overlayColor.rgb, _TintColor.rgb * _TintColor.a, lerpPoint);
				overlayColor.a = min(overlayColor.a + _TintColor.a, 1);

				//Lerp both colors
				col = lerp(col, overlayColor, overlayColor.a);
				col.a = 1;
				return col;
			}
		ENDCG
		}
	}
}
