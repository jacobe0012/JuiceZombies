Shader "Unlit/Number011"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _Point("Point", Range(0, 2)) = 2
        _ID1Color("ID 1 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID2Color("ID 2 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID3Color("ID 3 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID4Color("ID 4 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID5Color("ID 5 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID6Color("ID 6 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
        _ID10Color("ID 10 Color", COLOR) = (1.0, 1.0, 1.0, 1.0)
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
            ZTest LEqual    //Default
            // ZTest Less | Greater | GEqual | Equal | NotEqual | Always
            ZWrite On       //Default
            Cull Off
            //Blend [_SrcBlend] [_DstBlend]

            HLSLPROGRAM
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup

            //float _damageTime;
            //loat _tN;

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
            int _Point;
            float4 _ID1Color;
            float4 _ID2Color;
            float4 _ID3Color;
            float4 _ID4Color;
            float4 _ID5Color;
            float4 _ID6Color;
            float4 _ID10Color;

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

            float2 switchNumber(float2 input, float2 uvHelp, int targetNumber, int inputlength, int isK, int isM)
            {
                float help = 0.8 / inputlength;
                
                int i = 1;

                /*while(i < inputlength + 1)
                {
                    targetNumber = (int)targetNumber * (1 - 0.9 * isK - 0.9999 * isM);
                    int displayNumber = targetNumber % 10;
                    if(i == 1 || (isK || isM))
                    {
                        displayNumber = 11 * isK + 12 * isM;
                    }

                    if(i == 4 || (isK || isM))
                    {
                        displayNumber = 13;
                    }

                    int displayNX = displayNumber % 5;
                    int displayNY = (int)displayNumber * 0.2;



                    if(uvHelp.x >= (inputlength - i) * help + 0.1 && uvHelp.x < (inputlength - i + 1) * help + 0.1)
                    {
                        input.x = displayNX * 0.2 + 0.06 + 0.08 * (input.x - ((inputlength - i) * help) -0.1) / help - (displayNumber == 1) * 0.004;
                        input.y = input.y * (1.0 / 3.0) + displayNY * (1.0 / 3.0) ;
                    }

                    int isN = (i == 1 || (isK || isM)) || (i == 4 || (isK || isM));
                    targetNumber = (int)targetNumber * (1 - 0.9 * step(-0.5, -isN));
                    
                    i++;
                }*/

                while(i < inputlength + 1)
                {
                    int displayNumber = targetNumber % 10;
                    int displayNX = displayNumber % 5;
                    int displayNY = (int)displayNumber * 0.2;

                    if(uvHelp.x >= (inputlength - i) * help + 0.1 && uvHelp.x < (inputlength - i + 1) * help + 0.1)
                    {
                        input.x = displayNX * 0.2 + 0.06 + 0.08 * (input.x - ((inputlength - i) * help) - 0.1) / help - (displayNumber == 1) * 0.004;
                        input.y = input.y * (1.0 / 3.0) + displayNY * 1.0 / 3.0;
                    }
                    i++;
                    targetNumber = (int)targetNumber * 0.1;
                }

                input.y = input.y * (step(0.1, uvHelp) - step(0.9, uvHelp));
                //input.y = input.y * inputlength;

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
                scale.x = scale.x * (intLength((int)damageNumberData[1][0]) + (int)damageNumberData[0][1] - (int)damageNumberData[1][1]);
                scale.x = scale.x * 1.05 * sin(damageTime * 3.1415f) * 0.4;
                scale.y = scale.y * 1.05 * sin(damageTime * 3.1415f) * 2;
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
                //float2x4 damageNumberData = _DamageNumberBuffer[propertyIndex];
#else
                float4 uvTilingAndOffset = float4(1, 1, 0, 0);
                float sortingValue = 0;
                int2 flipValue = int2(0, 0);
                //float2x4 damageNumberData = float2x4(float2(0, 0), float2(0, 0), float2(0, 0), float2(0, 0));
#endif

                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_TRANSFER_INSTANCE_ID(attributes, varyings);

                // flip x/y UVs for mirroring texture
                attributes.uv.x = flipValue.x >= 0 ? attributes.uv.x : (1.0 - attributes.uv.x);
                attributes.uv.y = flipValue.y >= 0 ? attributes.uv.y : (1.0 - attributes.uv.y);


                int isK = 1;

                // change SV_Position to sort instances on screen without changing theirs matrix depth value
                varyings.positionCS = TransformObjectToHClip(attributes.positionOS);

                //float damageTime = _Time.y - damageNumberData[1][2];
                //varyings.positionCS.y -= 0.02 * sin(damageTime * 3.1415f); 
            
                //varyings.positionCS.x = varyings.positionCS.x * (intLength((int)damageNumberData[0][2]) + (int)damageNumberData[0][1]);
                varyings.positionCS.z = sortingValue;
                //varyings.positionCS.y += 0.03 * sin(damageTime * 3.1415f) * varyings.positionCS.y;
                //varyings.positionCS.x += 0.1 * sin(damageTime * 3.1415f) * varyings.positionCS.x;

                // tiling and offset UV
                varyings.uv = TilingAndOffset(attributes.uv, uvTilingAndOffset.xy, uvTilingAndOffset.zw);
                //varyings.uv = varyings.uv * float2(1.1 * sin(damageTime * 3.1415f), 1.1 * sin(damageTime * 3.1415f));
                
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
                
                //varyings.uv = TilingAndOffset(frac(varyings.uv), float2(1, 0.5), float2(0, 0));

                float2 uvHelp = varyings.uv;

                //int targetNumber = _tN + isK * 9 * _tN;
                int targetNumber = (int)damageNumberData[1][0] * (1 + (int)damageNumberData[0][1] * 9);
                int inputlength = intLength(targetNumber);
                inputlength = inputlength + damageNumberData[0][1] - 2 * damageNumberData[1][1];
                varyings.uv = switchNumber(varyings.uv, uvHelp, targetNumber, inputlength, (int)damageNumberData[0][1], (int)damageNumberData[1][1]);
                

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

                float damageTime = _Time.y - damageNumberData[1][2];

                texColor.w = texColor.w * smoothstep(0, 0.4, 1 - damageTime);
                clip(texColor.w - 0.01);
                return texColor ;//* smoothstep(0, 0.5, _damageTime);
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
