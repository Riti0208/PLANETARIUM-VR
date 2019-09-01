Shader "Planetalium/ParticleShader" {

	Properties {
		_MainTex("Tex", 2D) = "White" {}
		_Color("Main Color", COLOR) = (1,1,1,1)
	}

	SubShader{
		Tags {"Queue" = "Transparent" "RenderType"="Transparent" "IgnoreProjection" = "True" }
		LOD 100

		ZWrite Off

		Blend SrcAlpha One
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			sampler2D _MainTex;
			fixed4 _Color;
			float4 _MainTex_ST;

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(0, 0, 0, 1)) + float4(v.vertex.x, v.vertex.y, 0, 0));
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				return col;
			}
			ENDCG
		}
	}
}
