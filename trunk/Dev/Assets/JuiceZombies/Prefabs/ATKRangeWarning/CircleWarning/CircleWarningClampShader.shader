Shader "Unlit/CircleWarningClampShader"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _MainTex_ST("MainTex_ST", Vector) = (1, 1, 0, 0)
        _JiYuFlip("JiYu Flip",Vector) = (0, 0, 0, 0)
        _JiYuSort("JiYu Sort",Vector) = (0, 0, 0, 0)
        _JiYuPivot("JiYu Pivot",Range(0, 1)) = 0.5
        _JiYuStartTime("JiYu StartTime", Float) = 0
        _WarningData("Warning Data", Vector) = (0, 0, 0, 1)
        _MinAlpha("Min Alpha", float) = 1.0
        _MiddleAlpha("Middle Alpha", float) = 1.0
        _MaxAlpha("Max Alpha", float) = 1.0
        _MaxWidth("Max Width", Range(0, 1)) = 0.1
        _StartAlphaDurationRatios("Start AlphaDurationRatios", Range(0, 1)) = 0.4
        _EndAlphaDurationRatios("End AlphaDurationRatios", Range(0, 1)) = 0.4
    }
    SubShader
    {

        Tags
        {
            "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent"
        }

        Pass
        {
            //Cull Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #pragma target 4.5
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Assets\JuiceZombies\Shaders\JiYuShaders\JiYuTest\Cginc\JiYuShaderUtility.cginc"

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
                int2 _JiYuFlip;
                int2 _JiYuSort;
                float _JiYuPivot;
                float _JiYuStartTime;
                float4 _WarningData;
                float _MinAlpha;
                float _MiddleAlpha;
                float _MaxAlpha;
                float _MaxWidth;
                float _StartAlphaDurationRatios;
                float _EndAlphaDurationRatios;
            CBUFFER_END

            #if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
					UNITY_DOTS_INSTANCED_PROP(float4, _Color)
					UNITY_DOTS_INSTANCED_PROP(float4, _MainTex_ST)
					UNITY_DOTS_INSTANCED_PROP(int2, _JiYuFlip)
					UNITY_DOTS_INSTANCED_PROP(int2, _JiYuSort)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuPivot)	
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuStartTime)
                    UNITY_DOTS_INSTANCED_PROP(float4, _WarningData)
                    UNITY_DOTS_INSTANCED_PROP(float, _MinAlpha)
                    UNITY_DOTS_INSTANCED_PROP(float, _MiddleAlpha)
                    UNITY_DOTS_INSTANCED_PROP(float, _MaxAlpha)
                    UNITY_DOTS_INSTANCED_PROP(float, _MaxWidth)

				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

				#define _Color UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _Color)
				#define _MainTex_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _MainTex_ST)
                #define _JiYuFlip UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int2, _JiYuFlip)
                #define _JiYuSort UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int2, _JiYuSort)	
                #define _JiYuPivot UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuPivot)	
                #define _JiYuStartTime UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuStartTime)
                #define _WarningData UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _WarningData)
                #define _MinAlpha UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _MinAlpha)
                #define _MiddleAlpha UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _MiddleAlpha)
                #define _MaxAlpha UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _MaxAlpha)
                #define _MaxWidth UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _MaxWidth)
            #endif

            sampler2D _MainTex;
            float4 _sortingGlobalData;

            v2f vert(appdata v)
            {
                v2f output = (v2f)0;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, output);


                float3 positionWS = TransformObjectToWorld(v.positionOS);
                output.positionCS = TransformWorldToHClip(positionWS);

                output.positionCS.z = JiYuSortFunc(_JiYuPivot, _JiYuSort, _sortingGlobalData,UNITY_MATRIX_MVP);

                output.uv0 = v.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;

                output.uv0.x = _JiYuFlip.x >= 1 ? (1.0 - output.uv0.x) : output.uv0.x;
                output.uv0.y = _JiYuFlip.y >= 1 ? (1.0 - output.uv0.y) : output.uv0.y;

                #if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = v.instanceID;
                #endif

                return output;
            }


            half4 frag(v2f i) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float4 color = tex2D(_MainTex, i.uv0.xy);
                float epsilon = 0.0001;
                // warningData: x:angle; y:time; z:duration; w:cycle
                float angleHelp = (PI - (_WarningData.x / 180) * PI) / 2;
                float speedHelp = 1 + (_WarningData.z - 1) * step(epsilon, _WarningData.z);
                float speed = 1.0 / speedHelp; // Inverse speed for animation scaling
                float cycleCount = max(1.0, _WarningData.w); // Ensure at least 1 cycle
                cycleCount = 1;
                float cycleDuration = 1.0; // Normalized duration of a single cycle
                float eT = _JiYuStartTime;

                // Calculate animation progress
                float tHelp = eT * speed;
                float time = eT;
                float allTime = _WarningData.z / _StartAlphaDurationRatios;
                tHelp = min(tHelp, 1.0); // Normalize to [0, 1]

                // Calculate alpha based on time for fade-in and fade-out
                float alpha = 1.0;
                float alphaMin = 0.3;
                if (time < _StartAlphaDurationRatios * allTime)
                {
                    // Fade-in: 0 to _StartAlphaDurationRatios * allTime
                    alpha = time / (_StartAlphaDurationRatios * allTime);
                    alpha = max(alphaMin, alpha);
                }
                else if (time > allTime * (1 - _EndAlphaDurationRatios))
                {
                    // Fade-out: Linearly decrease alpha from 1 to alphaMin
                    float fadeOutTime = allTime * _EndAlphaDurationRatios;
                    float fadeOutProgress = (time - (allTime * (1 - _EndAlphaDurationRatios))) / fadeOutTime;
                    alpha = max(alphaMin, 1.0 - fadeOutProgress);
                }

                // Calculate pixel distance to center
                float2 hexagonCenter = float2(0.5, 0.5); // Center in UV space
                float2 relativePos = i.uv0.xy; // UV coordinates [0, 1]
                float distanceToCenter = length(relativePos - hexagonCenter);

                // Circle expansion animation
                float maxRadius = 0.5; // Maximum radius (covers half the texture)
                float currentRadius = tHelp * maxRadius; // Linearly grow radius from 0 to maxRadius

                // Radial gradient: outer edge is red, center is more transparent
                float gradient = smoothstep(currentRadius * 0.5, currentRadius, distanceToCenter);
                // Adjusted for lighter center
                float fillAlpha = step(distanceToCenter, currentRadius); // 1 inside circle, 0 outside

                // Outline: slightly darker ring at the edge
                float outlineThickness = _MaxWidth * 0.5; // Outline thickness
                float outlineRange = abs(distanceToCenter - currentRadius);
                float outlineAlpha = smoothstep(outlineThickness, 0.0, outlineRange); // Smooth outline

                // Combine texture color with adjusted red gradient
                color = color * _Color;
                // Base red gradient: softer, more transparent center
                float3 baseColor = lerp(float3(1.0, 0.4, 0.4) * 0.5, float3(1.0, 0.4, 0.4), gradient);
                // Softer red, fading to transparent center
                // Outline color: slightly darker but still soft
                float3 outlineColor = float3(0.8, 0.2, 0.2); // Softer darker red for outline
                // Blend base color and outline
                float3 finalColor = lerp(baseColor, outlineColor, outlineAlpha * 0.3); // Blend outline with base
                color.rgb = lerp(color.rgb, finalColor, _MaxAlpha * 0.6); // Apply to texture with reduced intensity

                // Combine alphas
                color.w = fillAlpha * alpha * _MaxAlpha * 0.5; // Reduced base alpha for transparency
                color.w = max(color.w, outlineAlpha * alpha * _MaxAlpha * 0.6); // Ensure outline is visible

                // Stop after total duration
                color.w = _JiYuStartTime >= allTime ? 0.0 : color.w;
                color.w = min(1.0, color.w);
                clip(color.w - epsilon);
                return color;
            }
            ENDHLSL
        }
    }
}