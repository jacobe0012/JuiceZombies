Shader "Unlit/WaterGunShader"
{
    Properties
    {
        _MainTex("_MainTex", 2D) = "white" {}
        _NoiseTex("_NoiseTex", 2D) =  "white" {}
        _MainTex_ST("MainTex_ST", Vector) = (1, 1, 0, 0)
        [HDR]_MiddleColor("Middle Color", Color) = (0.0, 0.0, 0.0, 0)
        [HDR]_BorderColor("Border Color", Color) = (0.0, 0.0, 0.0, 0)
        [HDR]_WhiteColor("White Color", Color) = (0.0, 0.0, 0.0, 0)
        _Alpha("Alpha", Range(0, 1)) = 0.5
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    CBUFFER_START(UnityPerMaterial)
    CBUFFER_END
    ENDHLSL

    SubShader
    {
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Pass
		{
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite On

			Name "Pass"

			HLSLPROGRAM

			#pragma target 4.5
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float2 uv			: TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS		: SV_POSITION;
                float2	uv				: TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			CBUFFER_END

			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
				
					UNITY_DOTS_INSTANCED_PROP(float4, _MainTex_ST)
									
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

				
				#define _MainTex_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _MainTex_ST)
             
			#endif


            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            float4 _MiddleColor;
            float4 _BorderColor;
            float4 _WhiteColor;
            float _Alpha;

            float4x4 offset_matrix(const float2 input, const float2 scale)
            {
                return float4x4(
                    scale.x,0,0,scale.x * -input.x,
                    0,scale.y,0,scale.y * -input.y,
                    0,0,1,0,
                    0,0,0,1
                );
            }

            float MiddleHelp(float x)
            {
                //float PI = 3.1415926; 
                float helpFloat = 0.04 * sin(_Time.y * 20) + 0.04;
                float leftFloat = 0.4 + helpFloat;
                float RightFloat = 0.6 - helpFloat;
                return cos(PI * (x - 0.5) * 1.5)  * (step(leftFloat,x) - step(RightFloat,x))  + smoothstep(0.25, leftFloat, x) * cos(PI * (-0.1) * 1.5) * (step(0, x) - step(leftFloat,x)) + smoothstep(-0.75, -RightFloat, -x) * cos(PI * (-0.1) * 1.5) * (step(RightFloat, x) - step(1,x));
            }



            float2 TilingAndOffset(float2 UV, float2 Tiling, float2 Offset)
            {
                return UV * Tiling + Offset;
            }
            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings varyings = (Varyings)0;


                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_TRANSFER_INSTANCE_ID(attributes, varyings);

             
                // change SV_Position to sort instances on screen without changing theirs matrix depth value
                varyings.positionCS = TransformObjectToHClip(attributes.positionOS);


                // tiling and offset UV
                varyings.uv = TilingAndOffset(attributes.uv, _MainTex_ST.xy, _MainTex_ST.zw);

                return varyings;
            }

            half4 UnlitFragment(Varyings varyings) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(varyings);

                // finally frac UV and locate texture on atlas, now our UV is inside actual texture bounds (repeated)
                varyings.uv = TilingAndOffset(frac(varyings.uv), _MainTex_ST.xy, _MainTex_ST.zw);
                float2 offset = float2(0, _Time.y * 5);
                float4 noiseColor = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, varyings.uv - offset);
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, varyings.uv);
                //texColor = texColor * noiseColor;
                //texColor.a = texColor.a * noiseColor.r;
                //texColor = texColor * (smoothstep(0, 0.3, varyings.uv.x) - smoothstep(0.7, 1, varyings.uv.x));
                texColor.rgb = texColor.rgb * (_MiddleColor.rgb *  MiddleHelp(varyings.uv.x) + _BorderColor * (1 - MiddleHelp(varyings.uv.x)));
                texColor.rgb += step(0.08 * sin(_Time.y * 40) + 0.6, noiseColor.r) * _WhiteColor.rgb;
                //clip(noiseColor.r - sin(_Time.y * 5) - 0.5);
                noiseColor.r = noiseColor.r * (smoothstep(0, 0.3, varyings.uv.x) - smoothstep(0.7, 1, varyings.uv.x));
                noiseColor.r = noiseColor.r * (step(-0.3, -varyings.uv.x) + step(0.7,varyings.uv.x));
                noiseColor.r += 1.0 * (step(0.3,varyings.uv.x) - step(0.7, varyings.uv.x));
                texColor.a = _Alpha;
                //texColor.a = texColor.a * (smoothstep(0, 0.3, varyings.uv.x) - smoothstep(0.7, 1, varyings.uv.x));
                //texColor = texColor * (step(-0.3, -varyings.uv.x) + step(0.7,varyings.uv.x));
                //texColor += float4(1.0, 1.0, 1.0, 1.0) * (step(0.3,varyings.uv.x) - step(0.7, varyings.uv.x));
                //texColor.rgb = texColor.rgb * (_MiddleColor.rgb * cos(3.14159 * (varyings.uv.x - 0.5) * 1.2) + _BorderColor.rgb * (1 - cos(3.14159 * (varyings.uv.x - 0.5) * 1.2)));
                //texColor.rgb = texColor.rgb * (_MiddleColor.rgb * MiddleHelp(varyings.uv.x) + _BorderColor.rgb * (1 - MiddleHelp(varyings.uv.x));
                //texColor.rgb = texColor.rgb * _BorderColor.rgb * (1 - sin(3.14159 * varyings.uv.x));
                clip(noiseColor.r - 0.025 * sin(_Time.y * 10) - 0.05);
                return texColor;
                //return _MiddleColor;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}