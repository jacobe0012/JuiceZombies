Shader "Night/NightShadowShader"
{
    Properties
	{
		_Color("Color", Color) = (0, 0, 0, 0)
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		_MainTex_ST("MainTex_ST", Vector) = (0, 0, 0, 0)
		_Color2("Color2", Color) = (0, 0, 0, 0)
		_JiYuScaleX("JiYu Scale X", float) = 1
		_JiYuScaleY("JiYu Scale X", float) = 1

		_JiYuSort("JiYu Sort", float) = 0
	}
	SubShader
	{

		Blend SrcAlpha OneMinusSrcAlpha

		Tags
		{
			//"RenderPipeline"="UniversalPipeline"
			"RenderType"="Opaque"
			"Queue"="AlphaTest"
		}
		Pass
		{
			Name "Pass"
			ZTest LEqual
            ZWrite On

			HLSLPROGRAM

			#pragma target 4.5
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

			struct appdata
			{
				float3 positionOS : POSITION;
				float4 uv0 : TEXCOORD0;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};

			struct v2f
			{
				float4 positionCS : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float4 _MainTex_ST;
			float4 _Color2;
			float _JiYuScaleX;
			float _JiYuScaleY;

			float _JiYuSort;
			CBUFFER_END

			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
					UNITY_DOTS_INSTANCED_PROP(float4, _Color)
					UNITY_DOTS_INSTANCED_PROP(float4, _MainTex_ST)
					UNITY_DOTS_INSTANCED_PROP(float4, _Color2)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuScaleX)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuScaleY)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuSort)
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

				#define _Color UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _Color)
				#define _MainTex_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _MainTex_ST)
				#define _Color2 UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _Color2)
				#define _JiYuScaleX UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuScaleX)
				#define _JiYuScaleY UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuScaleY)
				#define _JiYuSort UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuSort)
			#endif

			sampler2D _MainTex;

			v2f vert(appdata v)
			{
				v2f output = (v2f)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, output);

				v.positionOS.x = v.positionOS.x * _JiYuScaleX;
				v.positionOS.y = v.positionOS.y * _JiYuScaleY + _JiYuScaleY - 1;
				float3 positionWS = TransformObjectToWorld(v.positionOS);
				output.positionCS = TransformWorldToHClip(positionWS);
				output.uv0 = v.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				output.positionCS.z = 1 - _JiYuSort;

				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = v.instanceID;
				#endif

				return output;
			}

			half4 frag(v2f i) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(i);
				float4 color = tex2D(_MainTex,i.uv0.xy) * _Color2;
				float dis = distance(i.uv0.xy, float2(0.5, 0.5));
				float helpy1 = step(0.5, i.uv0.y);
				float helpy2 = step(-0.5, -i.uv0.y);
				float helpx = abs(1 - 2 * i.uv0.x);
				float helpxy = helpy1 * dis * 2 +  helpy2 * helpx;

				color.w = 0.95 * (1 - smoothstep(0.5, 1, helpx));
				//color.w = color.w * ((0.95 - 0.25 * i.uv0.y) * step(-0.6, -i.uv0.y) + step(0.6, i.uv0.y) * (1 - smoothstep(0.6, 1, i.uv0.y)) * 0.8);
				color.w = color.w * (1 - smoothstep(0.5, 1, helpxy));
				return color;
			}

			ENDHLSL
		}
	}
}