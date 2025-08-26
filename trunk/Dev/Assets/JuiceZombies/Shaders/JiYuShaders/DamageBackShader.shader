Shader "UnicornStudio/DamageBackShader"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 0)
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _MainTex_ST("MainTex_ST", Vector) = (1, 1, 0, 0)
        _JiYuScaleX("JiYu Scale X", float) = 1
        _JiYuScaleY("JiYu Scale X", float) = 1
        _JiYuSort("JiYu Sort", float) = 0
        _JiYuFlip("JiYu Flip", Vector) = (0, 0, 0, 0)
        _ToMaxTime("To Max Time", float) = 0
        _HoldMaxTime("Hold Max Time", float) = 0
        _MaxToDisappearTime("Max To Disappear", float) = 1
        _InitSize("Init Size", float) = 1
        _MaxSize("Max Size", float) = 1
        _DisappearSize("Disappear Size", float) = 1
        _InitAlpha("Inint Alpha", float) = 1
        _MaxAlpha("Max Alpha", float) = 1
        _DisappearAlpha("Disappear Alpha", float) = 1
        _StartTime("Start time", float) = 1
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

            float _ToMaxTime;
            float _HoldMaxTime;
            float _MaxToDisappearTime;
            float _InitSize;
            float _MaxSize;
            float _DisappearSize;
            float _InitAlpha;
            float _MaxAlpha;
            float _DisappearAlpha;
            float _StartTime;
            CBUFFER_END

            #if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
					UNITY_DOTS_INSTANCED_PROP(float4, _Color)
					UNITY_DOTS_INSTANCED_PROP(float4, _MainTex_ST)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuScaleX)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuScaleY)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuSort)
					UNITY_DOTS_INSTANCED_PROP(float2, _JiYuFlip)

                    UNITY_DOTS_INSTANCED_PROP(float, _ToMaxTime)
                    UNITY_DOTS_INSTANCED_PROP(float, _HoldMaxTime)
                    UNITY_DOTS_INSTANCED_PROP(float, _MaxToDisappearTime)
                    UNITY_DOTS_INSTANCED_PROP(float, _InitSize)
                    UNITY_DOTS_INSTANCED_PROP(float, _MaxSize)
                    UNITY_DOTS_INSTANCED_PROP(float, _DisappearSize)
                    UNITY_DOTS_INSTANCED_PROP(float, _InitAlpha)
                    UNITY_DOTS_INSTANCED_PROP(float, _MaxAlpha)
                    UNITY_DOTS_INSTANCED_PROP(float, _DisappearAlpha)
                    UNITY_DOTS_INSTANCED_PROP(float, _StartTime)
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

				#define _Color UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _Color)
				#define _MainTex_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _MainTex_ST)
				#define _JiYuScaleX UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuScaleX)
				#define _JiYuScaleY UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuScaleY)
				#define _JiYuSort UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuSort)
                #define _JiYuFlip UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float2, _JiYuFlip)

                #define _ToMaxTime UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _ToMaxTime)
                #define _HoldMaxTime UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _HoldMaxTime)
                #define _MaxToDisappearTime UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _MaxToDisappearTime)
                #define _InitSize UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _InitSize)
                #define _MaxSize UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _MaxSize)
                #define _DisappearSize UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _DisappearSize)
                #define _InitAlpha UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _InitAlpha)
                #define _MaxAlpha UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _MaxAlpha)
                #define _DisappearAlpha UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _DisappearAlpha)
                #define _StartTime UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _StartTime)
            #endif

            sampler2D _MainTex;

            float ScaleChange(float toMaxTime, float holdMaxTime, float maxToDisappearTime, float initSize, float maxSize, float disappearSize, float existenceTime)
            {
                float time1 = toMaxTime;
                float time2 = toMaxTime + holdMaxTime;
                float time3 = toMaxTime + holdMaxTime + maxToDisappearTime;

                return (smoothstep(0, time1, existenceTime) - smoothstep(time2, time3, existenceTime)) * (maxSize - initSize)
                        + initSize * (1 - step(time2, existenceTime)) + disappearSize * step(time2, existenceTime) 
                        + (step(time2, existenceTime) - step(time3, existenceTime)) * (existenceTime - time3) * (disappearSize - initSize) / maxToDisappearTime;
            }

            v2f vert(appdata v)
            {
                v2f output = (v2f)0;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, output);

                float eT = _Time.y - _StartTime;
                float scaleHelp = ScaleChange(_ToMaxTime, _HoldMaxTime, _MaxToDisappearTime, _InitSize, _MaxSize, _DisappearSize, eT);

                v.positionOS.y = v.positionOS.y + 0.3;

                v.positionOS.x = v.positionOS.x * _JiYuScaleX * scaleHelp;
                v.positionOS.y = v.positionOS.y * _JiYuScaleY * scaleHelp;
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
                float4 color = tex2D(_MainTex, i.uv0.xy);

                float eT = _Time.y - _StartTime;
                float aHelp = ScaleChange(_ToMaxTime, _HoldMaxTime, _MaxToDisappearTime, _InitAlpha, _MaxAlpha, _DisappearAlpha, eT);
                aHelp = aHelp - _DisappearAlpha * step(_ToMaxTime + _HoldMaxTime + _MaxToDisappearTime, eT);
                color.zw.y = color.zw.y * aHelp;

                if(color.zw.y < 0.01) discard;
                return color;
            }
            ENDHLSL
        }
    }
}