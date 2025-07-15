Shader "DGY/UV_Move_Add/UV_Move_Add"
{
    Properties
    {
        _Texture ("Texture", 2D) = "white" {}
        _Texture_ST("Texture_ST", Vector) = (1, 1, 0, 0)
        [HDR]_Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Mask ("Mask", 2D) = "white" {}
        _Mask_ST("Mask_ST", Vector) = (1, 1, 0, 0)
        _U_Move ("U_Move", Float ) = 0
        _V_Move ("V_Move", Float ) = 0


    }
    SubShader
    {

        Blend One One
        Cull Off
        ZWrite Off

        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
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
            float4 _Texture_ST;
            float4 _Mask_ST;
            float _U_Move;
            float _V_Move;
            CBUFFER_END

            #if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
					UNITY_DOTS_INSTANCED_PROP(float4, _Color)
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

				#define _Color UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _Color)
            #endif

            sampler2D _Texture;
            sampler2D _Mask;
            v2f vert(appdata v)
            {
                v2f output = (v2f)0;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, output);

                float3 positionWS = TransformObjectToWorld(v.positionOS);
                output.positionCS = TransformWorldToHClip(positionWS);
                output.uv0 = v.uv0.xy;
                
                #if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = v.instanceID;
                #endif

                return output;
            }

            half4 frag(v2f i, float facing : VFACE) : SV_TARGET
            {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 node_1390 = _Time;
                float2 node_7639 = (float2((_U_Move*node_1390.g),(node_1390.g*_V_Move))+i.uv0);
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_7639, _Texture));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                //float3 emissive = (_Color.rgb*_Texture_var.rgb*_Mask_var.rgb*i.vertexColor.rgb);
                float3 emissive = (_Color.rgb*_Texture_var.rgb*_Mask_var.rgb);
                float3 finalColor = emissive;
                half4 finalRGBA = half4(finalColor,1);
                return finalRGBA;
            }
            ENDHLSL
        }
    }
}