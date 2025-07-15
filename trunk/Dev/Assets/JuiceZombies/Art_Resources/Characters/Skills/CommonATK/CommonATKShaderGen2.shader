Shader "Unlit/CommonATKShaderGen2"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 0)
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _MainTex_ST("MainTex_ST", Vector) = (1, 1, 0, 0)
        _JiYuScaleX("JiYu Scale X", float) = 1
        _JiYuScaleY("JiYu Scale Y", float) = 1
        _JiYuSort("JiYu Sort", float) = 0
        _JiYuFlip("JiYu Flip", Vector) = (0, 0, 0, 0)

        _MonsterGeneralAttackData("MonsterGeneralAttackData", vector) = (0, 0, 0, 0)
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
            float _JiYuScaleX;
            float _JiYuScaleY;
            float _JiYuSort;
            float2 _JiYuFlip;

            float3 _MonsterGeneralAttackData;
            CBUFFER_END

            #if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
					UNITY_DOTS_INSTANCED_PROP(float4, _Color)
					UNITY_DOTS_INSTANCED_PROP(float4, _MainTex_ST)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuScaleX)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuScaleY)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuSort)
					UNITY_DOTS_INSTANCED_PROP(float2, _JiYuFlip)

                    UNITY_DOTS_INSTANCED_PROP(float3, _MonsterGeneralAttackData)
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

				#define _Color UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _Color)
				#define _MainTex_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _MainTex_ST)
				#define _JiYuScaleX UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuScaleX)
				#define _JiYuScaleY UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuScaleY)
				#define _JiYuSort UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuSort)
                #define _JiYuFlip UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float2, _JiYuFlip)

                #define _WarningData UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float3, _MonsterGeneralAttackData)
            #endif

            sampler2D _MainTex;

            float2 UVTransform(float2 input, float theta, float beta, float r, int leftOrRight, float alpha)
            {
                float2 center = float2(0.5, 0.5);
                float distanceToCenter = length(input - center);
                float inputx = input.x;
                float inputy = input.y;
                float xmiddle = (abs(atan2((inputy - 0.5), leftOrRight * (inputx - 0.5)) - theta)) / beta;
                float xhelp = 1 - xmiddle;
                float yhelp = step(r, distanceToCenter) * (distanceToCenter - r) / (0.5 - r);
                float disLimit = 1 - step(0.5, distanceToCenter);
                xhelp = xhelp * step(-alpha / 2, atan2((inputy - 0.5), leftOrRight * (inputx - 0.5))) * disLimit;
                yhelp = yhelp * step(-alpha / 2, atan2((inputy - 0.5), leftOrRight * (inputx - 0.5))) * disLimit;
                return float2(xhelp, yhelp);
            }

            v2f vert(appdata v)
            {
                v2f output = (v2f)0;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, output);

                v.positionOS.x = v.positionOS.x * _JiYuScaleX;
                v.positionOS.y = v.positionOS.y * _JiYuScaleY;
                //v.positionOS.z = _JiYuSort;
                float3 positionWS = TransformObjectToWorld(v.positionOS);
                output.positionCS = TransformWorldToHClip(positionWS);
                output.positionCS.z = 1 - _JiYuSort;
                output.uv0 = v.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                
                float flipXHelp = step(0.5, _JiYuFlip.x);
                float flipYHelp = step(0.5, _JiYuFlip.y);
                float flipXHelpReversed = 1 - flipXHelp;
                float flipYHelpReversed = 1 - flipYHelp;

                output.uv0.x = flipXHelp * (1 - output.uv0.x) + flipXHelpReversed * output.uv0.x;
                output.uv0.y = flipYHelp * (1 - output.uv0.y) + flipYHelpReversed * output.uv0.y;


                #if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = v.instanceID;
                #endif

                return output;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float alpha = _MonsterGeneralAttackData.z;
                alpha = PI * alpha / 180;
                float cycleT = _MonsterGeneralAttackData.x;
                float initTime = _MonsterGeneralAttackData.y;
                float nowTime = _Time.y - initTime;

                float theta = alpha / 2 - nowTime * alpha / cycleT;
                float beta = alpha * nowTime / (1.28 * cycleT);

                float r = 0.3;

                i.uv0.xy = UVTransform(i.uv0.xy, theta, beta, r, 1, alpha);

                float4 color = tex2D(_MainTex, i.uv0.xy);
                color = color * step(0, nowTime);
                color = color * (1 - step(cycleT, nowTime));
                if(color.zw.y < 0.01) discard;
                return color;
            }
            ENDHLSL
        }
    }
}