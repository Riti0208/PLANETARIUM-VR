Shader "Custom/Particle" {

	SubShader{
		Pass {
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjection" = "True" }
		LOD 200
		ZWrite Off

		Blend SrcAlpha One

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 5.0

		struct Particle {
			float3 position;
			float hDeg;
			float sDeg;
			float4 color;
			float4 colorBuffer;
			float size;
		};

		struct PS_INPUT {
			float4 position : SV_POSITION;
			float4 color : COLOR;
			float size : PSIZE;
		};
		// particles' data
		StructuredBuffer<Particle> particleBuffer;

		PS_INPUT vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
		{
			PS_INPUT o = (PS_INPUT)0;

			// Color
			o.color = particleBuffer[instance_id].color;

			o.size = particleBuffer[instance_id].size;
			
			// Position
			o.position = UnityObjectToClipPos(float4(particleBuffer[instance_id].position, 1.0f));

			return o;
		}

		float4 frag(PS_INPUT i) : COLOR
		{
			return i.color;
		}


		ENDCG
		}
	}
		FallBack Off
}