Shader "UnicornStudio/JiYuSecKillDNumberTest"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		_MainTex_ST("MainTex_ST", Vector) = (1, 1, 0, 0)
		_JiYuSort("JiYu Sort",Vector) = (0, 0, 0, 0)
		_JiYuPivot("JiYu Pivot",Range(0, 1)) = 0.5
		_JiYuDamageNumber("JiYu DamageNumber", Vector) =(0, 0, 0, 0)
		_JiYuStartTime("JiYu StartTime", Float) = 0

	}
	SubShader
	{
        Tags {"RenderPipeline" = "UniversalPipeline" "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
		
		Pass
		{
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			#pragma target 4.5
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON
			#pragma vertex vert
			#pragma fragment frag
            //#include "UnityCG.cginc"
            #include "Assets\JuiceZombies\Shaders\JiYuShaders\JiYuTest\Cginc\JiYuShaderUtility.cginc"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : INSTANCEID_SEMANTIC;
				#endif
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
            };

			CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
			int2 _JiYuDamageNumber;
			float _JiYuStartTime;
			int2 _JiYuSort;
			float _JiYuPivot;
			CBUFFER_END
			
			
			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
					UNITY_DOTS_INSTANCED_PROP(float4, _MainTex_ST)
					UNITY_DOTS_INSTANCED_PROP(int2, _JiYuDamageNumber)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuStartTime)
					UNITY_DOTS_INSTANCED_PROP(int2, _JiYuSort)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuPivot)	
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
				#define _MainTex_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _MainTex_ST)
				#define _JiYuDamageNumber UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int2, _JiYuDamageNumber)
                #define _JiYuStartTime UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuStartTime)
                #define _JiYuSort UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int2, _JiYuSort)	
                #define _JiYuPivot UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuPivot)	
			#endif	
			

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
			float4 _sortingGlobalData;
			
            float4x4 offset_matrix(const float2 input, const float2 scale)
            {
                return float4x4(
                    scale.x,0,0,scale.x * -input.x,
                    0,scale.y,0,scale.y * -input.y,
                    0,0,1,0,
                    0,0,0,1
                );
            }
            float2 TilingAndOffset(float2 UV, float2 Tiling, float2 Offset)
            {
                return UV * Tiling + Offset;
            }
		    v2f vert (appdata v)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);


                
                float damageTime = _JiYuStartTime;
				float2 scale = (1.0,1.0);
                scale.x = scale.x * 1.05 * sin(damageTime * 3.1415f) * 0.4;
                scale.y = scale.y * 1.05 * sin(damageTime * 3.1415f) * 2;
				float3 positionWS = TransformObjectToWorld(v.vertex);
                //unity_ObjectToWorld = mul(positionWS, offset_matrix(float2(.5,.5), scale));


                // change SV_Position to sort instances on screen without changing theirs matrix depth value
                o.vertex = TransformObjectToHClip(v.vertex);



                o.vertex.z = JiYuSortFunc(_JiYuPivot,_JiYuSort,_sortingGlobalData,UNITY_MATRIX_MVP);

                // tiling and offset UV
                o.uv = TilingAndOffset(v.uv, _MainTex_ST.xy, _MainTex_ST.zw);

                
                //UNITY_TRANSFER_FOG(o,o.vertex);

				#if UNITY_ANY_INSTANCING_ENABLED
				o.instanceID = v.instanceID;
				#endif
                return o;
            }
			
            half4 frag (v2f i) : SV_Target
            {
				UNITY_SETUP_INSTANCE_ID(i);
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float damageTime = _JiYuStartTime;

                texColor.w = texColor.w * smoothstep(0, 0.4, 1 - damageTime);
                clip(texColor.w - 0.01);
                return texColor;
            }
			ENDHLSL
		}
	}
}

