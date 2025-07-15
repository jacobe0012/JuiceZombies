Shader "Unlit/DoubleDNumber"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _BackTex("BackTex", 2D) = "white" {}
        _ID1Color("ID 1 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID2Color("ID 2 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID3Color("ID 3 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID4Color("ID 4 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID5Color("ID 5 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID6Color("ID 6 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID10Color("ID 10 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _InitialSize("Initial Size", float) = 1.0
        _MaxSize("Max Size", float) = 1.0
        _DisappearSize("Disappear Size", float) = 1.0
        _ToMaxTime("To Max Time", float) = 1.0
        _HoldMaxTime("Hold Max Time", float) = 1.0
        _ToDisappearTime("To Disappear Time", float) = 1.0
        _AppearAlpha("Appear Alpha", float) = 1.0
        _MaxAlpha("Max Alpha", float) = 1.0
        _DisappearAlpha("Disappear Alpha", float) = 1.0
        _Radius("Radius", Range(0, 1000)) = 1.0
        _XScale("Scale of X", float) = 1.0
        _YScale("Scale of Y", float) = 1.0
        _CharacterProportion("Character Proportion", Range(0, 1000)) = 450
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    CBUFFER_START(UnityPerMaterial)
    CBUFFER_END
    ENDHLSL

    SubShader
    {
        Tags {"Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue" = "AlphaTest" "RenderType" = "TransparentCutout"}
            ZTest LEqual    
            ZWrite On       
            Cull Off

            HLSLPROGRAM
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup

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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_BackTex);
            SAMPLER(sampler_BackTex);
            float4 _ID1Color;
            float4 _ID2Color;
            float4 _ID3Color;
            float4 _ID4Color;
            float4 _ID5Color;
            float4 _ID6Color;
            float4 _ID10Color;
            float _InitialSize;
            float _MaxSize;
            float _DisappearSize;
            float _ToMaxTime;
            float _HoldMaxTime;
            float _ToDisappearTime;
            float _AppearAlpha;
            float _MaxAlpha;
            float _DisappearAlpha;
            float _Radius;
            float _XScale;
            float _YScale;
            float _CharacterProportion;

#if defined(UNITY_INSTANCING_ENABLED) || defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) || defined(UNITY_STEREO_INSTANCING_ENABLED)
            StructuredBuffer<int> _propertyPointers;
            StructuredBuffer<float4> _uvTilingAndOffsetBuffer;
            StructuredBuffer<float4> _uvAtlasBuffer;
            StructuredBuffer<float> _sortingValueBuffer;
            StructuredBuffer<float4x4> _positionBuffer;
            StructuredBuffer<float2> _pivotBuffer;
            StructuredBuffer<float2> _heightWidthBuffer;
            StructuredBuffer<int2> _flipBuffer;
            StructuredBuffer<float2x4> _DamageNumberBuffer;
#endif

            float4x4 offset_matrix(const float2 input, const float2 scale)
            {
                return float4x4(
                    scale.x,0,0,scale.x * -input.x,
                    0,scale.y,0,scale.y * -input.y,
                    0,0,1,0,
                    0,0,0,1
                );
            }

            float sizeChange(float InitialSize, float MaxSize, float DisappearSize, float ToMaxTime, 
                                float HoldMaxTime, float ToDisappearTime, float damageTime)
            {
                float time1 = ToMaxTime;
                float time2 = ToMaxTime + HoldMaxTime;
                float time3 = ToMaxTime + HoldMaxTime + ToDisappearTime;

                return (smoothstep(0, time1, damageTime) - smoothstep(time2, time3, damageTime)) * (MaxSize - InitialSize)
                        + InitialSize * (1 - step(time2, damageTime)) + DisappearSize * step(time2, damageTime) 
                        + (step(time2, damageTime) - step(time3, damageTime)) * (damageTime - time3) * (DisappearSize - InitialSize) / ToDisappearTime;
            }

            int intLength(int input)
            {
                int i = 1;

                while(step(10, input))
                {
                    input = input * 0.1;
                    i++;
                }
                return i;
            }

            int CutWithout0Length(int input)
            {
                return 3 * step(4.5, input) + step(6.5, input) + step(7.5, input) + step(8.5, input);
            }

            int Judgment5(int input)
            {
                //判断第五位是否为0
                input = (int) input * 0.0001; 
                input = input % 10;
                return 1 - step(0.5, input);
            }

            int Judgment6(int input)
            {
                //判断第六位是否为0
                input = (int) input * 0.00001; 
                input = input % 10;
                return 1 - step(0.5, input);
            }

            float personalFract(float value)
            {
                return value - floor(value);
            }

            float2 switchNumber(float2 input, float2 uvHelp, int targetNumber, int inputlength, int cut1length, int cut2length, int renderlength)
            {
                float help = 0.8 / renderlength;
                float helpChangeForPoint = help / 2;
                int i = 1;
                targetNumber = targetNumber * pow(0.1, (cut1length + cut2length));
                targetNumber = targetNumber * (1 + step(4.5, inputlength) * 9);
                int pointPosition = (-1 * (inputlength + cut2length) + 11) * (
                                        (step(6.5, inputlength) - step(7.5, inputlength)) * (step(-0.1, cut2length) - step(1.1, cut2length)) + 
                                        (step(7.5, inputlength) - step(8.5, inputlength)) * (step(-0.1, cut2length) - step(0.1, cut2length)));
                int havePoint = step(0.5, pointPosition);
                int pointHadBeenRendered = 0;
                while(i < renderlength + 1)
                {
                    int displayNumber = targetNumber % 10;
                    displayNumber = (i + displayNumber == 1) ? (step(4.5, inputlength) * 11 + step(6.5, inputlength)) : displayNumber;
                    int pointHelp = (i == pointPosition) ? 1 : 0;
                    targetNumber = targetNumber * (1 + 9 * pointHelp);
                    displayNumber = displayNumber + (13 - displayNumber) * pointHelp;
                    int displayNX = displayNumber % 5;
                    int displayNY = (int)displayNumber * 0.2;
                    if(uvHelp.x >= (renderlength - i) * help + 0.1 - havePoint * helpChangeForPoint / 2 + (pointHelp + pointHadBeenRendered) * helpChangeForPoint
                        && uvHelp.x < (renderlength - i + 1) * help + 0.1 - havePoint * helpChangeForPoint / 2 + pointHadBeenRendered * helpChangeForPoint)
                    {
                        input.x = displayNX * 0.2 + 0.1 * (1 -_CharacterProportion / 1000) + 
                                    0.2 * (_CharacterProportion / 1000) * (input.x - ((renderlength - i) * help) - 0.1 + havePoint * helpChangeForPoint / 2 - pointHadBeenRendered * helpChangeForPoint) / help 
                                    - 0 * (displayNumber == 1) * 0.004 - pointHelp * helpChangeForPoint / 4;
                        input.y = input.y * (1.0 / 3.0) + displayNY * 1.0 / 3.0;
                    }
                    pointHadBeenRendered += pointHelp;
                    input.y = input.y * (step(0.1 + havePoint * helpChangeForPoint / 2, uvHelp) 
                                - step(0.9 - havePoint * helpChangeForPoint / 2, uvHelp));
                    i++;
                    targetNumber = (int)targetNumber * 0.1;
                }
                return input;
            }
            
            void setup()
            {
#if defined(UNITY_INSTANCING_ENABLED) || defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) || defined(UNITY_STEREO_INSTANCING_ENABLED)
                int propertyIndex = _propertyPointers[unity_InstanceID];
                float4x4 transform = _positionBuffer[propertyIndex];
                float2 pivot = _pivotBuffer[propertyIndex];
                float2x4 damageNumberData = _DamageNumberBuffer[propertyIndex];
                float2 scale = _heightWidthBuffer[propertyIndex];
                float damageTime = _Time.y - damageNumberData[1][2];
                int targetNumber = (int)damageNumberData[1][0] * (1 + (int)damageNumberData[0][1] * 9);
                int inputlength = intLength(targetNumber);
                int cut1length = CutWithout0Length(inputlength);
                int cut2length = (step(6.5, inputlength) - step(7.5, inputlength)) * Judgment5(targetNumber) * (1 + Judgment6(targetNumber))
                                    + (step(7.5, inputlength) - step(8.5, inputlength)) * Judgment6(targetNumber);
                int renderlength = inputlength - cut1length - cut2length + step(4.5, inputlength)
                                    + (step(6.5, inputlength) - step(7.5, inputlength)) * (-1 * step(1.5, cut2length) + 1)
                                    + (step(7.5, inputlength) - step(8.5, inputlength)) * (-1 * step(0.5, cut2length) + 1);
                scale.x = scale.x * renderlength * 0.8 * _XScale;
                scale.y = scale.y * 2 * 1.5 * _YScale;
                scale = scale * sizeChange(_InitialSize, _MaxSize, _DisappearSize, _ToMaxTime, _HoldMaxTime, _ToDisappearTime, damageTime);
                unity_ObjectToWorld = mul(transform, offset_matrix(pivot, scale));
#endif
            }

            float2 TilingAndOffset(float2 UV, float2 Tiling, float2 Offset)
            {
                return UV * Tiling + Offset;
            }
            Varyings UnlitVertex(Attributes attributes, uint instanceID : SV_InstanceID)
            {
                Varyings varyings = (Varyings)0;

#if defined(UNITY_INSTANCING_ENABLED) || defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) || defined(UNITY_STEREO_INSTANCING_ENABLED)
                int propertyIndex = _propertyPointers[instanceID];
                float4 uvTilingAndOffset = _uvTilingAndOffsetBuffer[propertyIndex];
                float sortingValue = _sortingValueBuffer[propertyIndex];
                int2 flipValue = _flipBuffer[propertyIndex];
                float2x4 damageNumberData = _DamageNumberBuffer[propertyIndex];
#else
                float4 uvTilingAndOffset = float4(1, 1, 0, 0);
                float sortingValue = 0;
                int2 flipValue = int2(0, 0);
                float2x4 damageNumberData = float2x4(float2(0, 0), float2(0, 0), float2(0, 0), float2(0, 0));
#endif

                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_TRANSFER_INSTANCE_ID(attributes, varyings);

                // flip x/y UVs for mirroring texture
                attributes.uv.x = flipValue.x >= 0 ? attributes.uv.x : (1.0 - attributes.uv.x);
                attributes.uv.y = flipValue.y >= 0 ? attributes.uv.y : (1.0 - attributes.uv.y);


                // change SV_Position to sort instances on screen without changing theirs matrix depth value
                varyings.positionCS = TransformObjectToHClip(attributes.positionOS);
                float randomValue1 = 2 * PI * personalFract(sin(dot(float2(1, 1), float2(12.9898, 78.233))) * damageNumberData[1][2]);
                float randomValue2 = personalFract(cos(dot(float2(1, 1), float2(34.5678, 56.789))) * damageNumberData[1][2] * 10);
                varyings.positionCS.x += _Radius * randomValue2 * cos(randomValue1) / 1170;
                varyings.positionCS.y += _Radius * randomValue2 * sin(randomValue1) / 2532;
                
                varyings.positionCS.z = sortingValue;
                

                // tiling and offset UV
                varyings.uv = TilingAndOffset(attributes.uv, uvTilingAndOffset.xy, uvTilingAndOffset.zw);
                
                return varyings;
            }

            float4 UnlitFragment(Varyings varyings, uint instanceID : SV_InstanceID) : SV_Target
            {
#if defined(UNITY_INSTANCING_ENABLED) || defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) || defined(UNITY_STEREO_INSTANCING_ENABLED)
                int propertyIndex = _propertyPointers[instanceID];
                float4 uvAtlas = _uvAtlasBuffer[propertyIndex];
                float2x4 damageNumberData = _DamageNumberBuffer[propertyIndex];
#else
                float4 uvAtlas = float4(1, 1, 0, 0);
                float2x4 damageNumberData = float2x4(float2(0, 0), float2(0, 0), float2(0, 0), float2(0, 0));
#endif

                // finally frac UV and locate texture on atlas, now our UV is inside actual texture bounds (repeated)
                varyings.uv = TilingAndOffset(frac(varyings.uv), uvAtlas.xy, uvAtlas.zw);

                float2 uvHelp = varyings.uv;

                int targetNumber = (int)damageNumberData[1][0] * (1 + (int)damageNumberData[0][1] * 9);
                int inputlength = intLength(targetNumber);
                int cut1length = CutWithout0Length(inputlength);
                int cut2length = (step(6.5, inputlength) - step(7.5, inputlength)) * Judgment5(targetNumber) * (1 + Judgment6(targetNumber))
                                    + (step(7.5, inputlength) - step(8.5, inputlength)) * Judgment6(targetNumber);
                int renderlength = inputlength - cut1length - cut2length + step(4.5, inputlength)
                                    + (step(6.5, inputlength) - step(7.5, inputlength)) * (-1 * step(1.5, cut2length) + 1)
                                    + (step(7.5, inputlength) - step(8.5, inputlength)) * (-1 * step(0.5, cut2length) + 1);
                varyings.uv = switchNumber(varyings.uv, uvHelp, targetNumber, inputlength, cut1length, cut2length, renderlength);

                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, varyings.uv);
                texColor.xyz = texColor.xyz * (
                    (damageNumberData[0][0] == 1) * _ID1Color +
                    (damageNumberData[0][0] == 2) * _ID2Color +
                    (damageNumberData[0][0] == 3) * _ID3Color +
                    (damageNumberData[0][0] == 4) * _ID4Color +
                    (damageNumberData[0][0] == 5) * _ID5Color +
                    (damageNumberData[0][0] == 6) * _ID6Color +
                    (damageNumberData[0][0] == 10) * _ID10Color
                    );

                float4 backColor = SAMPLE_TEXTURE2D(_BackTex, sampler_BackTex, uvHelp);
                texColor = texColor * texColor.w + backColor * (1 - texColor.w);
                float damageTime = _Time.y - damageNumberData[1][2];
                float aHelp = sizeChange(_AppearAlpha, _MaxAlpha, _DisappearAlpha, _ToMaxTime, _HoldMaxTime, _ToDisappearTime, damageTime);
                aHelp = aHelp - _DisappearAlpha * step(_ToMaxTime + _HoldMaxTime + _ToDisappearTime, damageTime);
                texColor.a = texColor.a * aHelp;
                clip(texColor.w - 0.01);
                return texColor ;
                //return backColor;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
