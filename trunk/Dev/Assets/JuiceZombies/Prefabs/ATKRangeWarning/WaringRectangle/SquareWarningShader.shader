Shader "Unlit/SquareWarningShader"
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
        _JiYuScaleXY ("JiYuScaleXY", Vector) = (1, 1, 0, 0)

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
            #include "Assets\ApesGang\Shaders\JiYuShaders\JiYuTest\Cginc\JiYuShaderUtility.cginc"

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
                float4 _JiYuScaleXY;
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
                output.positionCS.xy = output.positionCS.xy * _JiYuScaleXY.xy;
                #if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = v.instanceID;
                #endif

                return output;
            }

            // half4 frag(v2f i) : SV_TARGET
            // {
            //     UNITY_SETUP_INSTANCE_ID(i);
            //     float4 color = tex2D(_MainTex, i.uv0.xy);
            //     color = (1, 1, 1, 1);
            //     color = color * _Color;
            //     float epsilon = 0.0001;
            //     //warningData: x:angle;   y:time;   z:speed;   w:cycle
            //     float angleHelp = (PI - (_WarningData.x / 180) * PI) / 2;
            //     float speedHelp = 1 + (_WarningData.z - 1) * step(epsilon, _WarningData.z);
            //     float speed = speedHelp;
            //     speed = 1. / speed;
            //     float cycletemp = 1. / _WarningData.w;
            //     float cycle = cycletemp;
            //     //float eT = _Time.y - _WarningData.y;
            //     float eT = _JiYuStartTime;
            //
            //     float tHelp = eT * speed;
            //     tHelp = _WarningData.w <= 0 ? 0. : tHelp;
            //     int isZero = step(epsilon, cycle);
            //     tHelp = (isZero == 0) ? tHelp * (1 - step(1, tHelp)) + step(1, tHelp) : tHelp;
            //     cycle = 1 + (cycle - 1) * step(epsilon, cycle);
            //
            //     //计算像素到中心的距离
            //     float YWidth = _MaxWidth / (1.);
            //     YWidth = YWidth * cycle;
            //
            //     float y1 = (i.uv0.y - tHelp) / cycle;
            //     float y2 = y1 - floor(y1);
            //     float y3 = (step((1 - YWidth), y2) - step(1, y2)) * _MaxAlpha + (y2 * (_MiddleAlpha - _MinAlpha) / (1 -
            //         YWidth) + _MinAlpha) * (-step((1 - YWidth), y2) + 1);
            //     float y4 = (step(YWidth, i.uv0.y) - step(1 - YWidth, i.uv0.y)) * y3 + _MaxAlpha * (step(0, i.uv0.y) -
            //         step(YWidth, i.uv0.y) + step(1 - YWidth, i.uv0.y) - step(1, i.uv0.y));
            //     float y5 = step(YWidth, i.uv0.x) - step(1 - YWidth, i.uv0.x);
            //     float y6 = _MaxAlpha * (step(0, i.uv0.x) - step(YWidth, i.uv0.x) + step(1 - YWidth, i.uv0.x) - step(
            //         1, i.uv0.x));
            //
            //     color.w = y4;
            //     color.w = color.w * y5 + y6;
            //     //color.zw.y = 1;
            //     color.w = _JiYuStartTime >= _WarningData.z ? 0.0 : color.w;
            //     clip(color.w - epsilon);
            //
            //     float4 color1 = tex2D(_MainTex, i.uv0.xy);
            //
            //     
            //     if (color1.a > 0.5)
            //     {
            //         color = color * color1;
            //     }
            //
            //     return color;
            // }

            //修改过的

            half4 frag(v2f i) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float4 color = tex2D(_MainTex, i.uv0.xy);
                color = float4(1, 1, 1, 1);
                color = color * _Color;
                float epsilon = 0.0001;
                // warningData: x:angle; y:time; z:duration; w:cycle
                float angleHelp = (PI - (_WarningData.x / 180) * PI) / 2;
                float speedHelp = 1 + (_WarningData.z - 1) * step(epsilon, _WarningData.z);
                float speed = 1. / speedHelp; // Inverse speed for animation scaling
                float cycleCount = max(1.0, _WarningData.w); // Ensure at least 1 cycle
                cycleCount = 1;
                float cycleDuration = 1.0; // Normalized duration of a single cycle
                float eT = _JiYuStartTime;

                // Calculate animation progress, wrapping within cycle count
                float tHelp = eT * speed;
                //tHelp = _WarningData.w <= 0 ? 0. : frac(tHelp * cycleCount); // Normalize within [0, 1] per cycle
                float time = eT;
                float allTime = _WarningData.z / _StartAlphaDurationRatios;
                //tHelp = _WarningData.w <= 0 ? 0. : frac(tHelp * cycleCount); // Normalize within [0, 1] per cycle
                tHelp = min(tHelp, 1);
                // Calculate alpha based on time for fade-in and fade-out
                //float normalizedTime = tHelp; // tHelp is in [0, 1] per cycle
                float alpha = 1.0;
                float alphaMin = 0.3;
                if (time < _StartAlphaDurationRatios * allTime)
                {
                    // Fade-in: 0 to _StartAlphaDurationRatios * _WarningData.z
                    alpha = time / (_StartAlphaDurationRatios * allTime);
                    alpha = max(alphaMin, alpha);
                    //alpha = time / _StartAlphaDurationRatios * allTime; // Linear fade-in
                    //alpha = 0.3;
                    // Optional: alpha = smoothstep(0.0, _StartAlphaDurationRatios, normalizedTime);
                }
                else if (time > allTime * (1 - _EndAlphaDurationRatios))
                {
                    // Fade-out: Linearly decrease alpha from 1 to alphaMin
                    float fadeOutTime = allTime * _EndAlphaDurationRatios; // The duration of the fade-out phase
                    float fadeOutProgress = (time - (allTime * (1 - _EndAlphaDurationRatios))) / fadeOutTime;

                    // Linearly decrease alpha, ensuring it does not go below alphaMin
                    alpha = max(alphaMin, 1.0 - fadeOutProgress); // Decrease alpha from 1 to alphaMin
                }

                // Calculate YWidth
                float YWidth = _MaxWidth; // No cycle scaling

                // Animation calculations
                float y1 = (i.uv0.y - tHelp) / cycleDuration;
                float y2 = y1 - floor(y1);
                float y3 = (step((1 - YWidth), y2) - step(1, y2)) * _MaxAlpha +
                    (y2 * (_MiddleAlpha - _MinAlpha) / (1 - YWidth) + _MinAlpha) *
                    (-step((1 - YWidth), y2) + 1);
                float y4 = (step(YWidth, i.uv0.y) - step(1 - YWidth, i.uv0.y)) * y3 +
                    _MaxAlpha * (step(0, i.uv0.y) - step(YWidth, i.uv0.y) + step(1 - YWidth, i.uv0.y) -
                        step(1, i.uv0.y));
                float y5 = step(YWidth, i.uv0.x) - step(1 - YWidth, i.uv0.x);
                float y6 = _MaxAlpha * (step(0, i.uv0.x) - step(YWidth, i.uv0.x) + step(1 - YWidth, i.uv0.x) -
                    step(1, i.uv0.x));

                color.w = y4;
                color.w = color.w * y5 + y6;
                color.w *= alpha; // Apply fade-in/fade-out alpha
                color.w = _JiYuStartTime >= allTime ? 0.0 : color.w; // Stop after total duration

                clip(color.w - epsilon);

                //float4 color1 = tex2D(_MainTex, i.uv0.xy);
                //if (color1.a > 0.5)
                //{
                //    color = color * color1;
                //}

                return color;
            }
            ENDHLSL
        }
    }
}