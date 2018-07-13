Shader "Custom/Unlit Transparent (Alpha Gamma Corrected)"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AlphaGamma ("Alpha Gamma", Range(1.0, 2.8)) = 2.1
	}
	SubShader
	{
		Tags
		{
		    "RenderType" = "Transparent"
		    "Queue" = "Transparent"
		    "IgnoreProjector" = "True"
        }

		ZWrite off
		Cull back
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _AlphaGamma;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);
				col.a = pow(col.a, _AlphaGamma);

				UNITY_APPLY_FOG(i.fogCoord, col);

				return col;
			}
			ENDCG
		}
	}
}
