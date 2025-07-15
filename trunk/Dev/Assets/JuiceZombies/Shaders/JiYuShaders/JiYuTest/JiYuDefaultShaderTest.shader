Shader "JiYuStudio/JiYuDefaultShaderTest"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _MainTex_ST("MainTex_ST", Vector) = (1, 1, 0, 0)
        _JiYuFlip("JiYu Flip",Vector) = (0, 0, 0, 0)
        _JiYuSort("JiYu Sort",Vector) = (0, 0, 0, 0)
        _JiYuPivot("JiYu Pivot",Range(0, 1)) = 0.5
        
        _JiYuAlphaOffsetY("JiYu AlphaOffsetY",Range(0, 1)) = 0
        _AlphaCutOff("JiYu Alpha CutOff",Range(0, 1)) = 0.01
        
        _DissolveMap ("Dissolve Map", 2D) = "white" {}
        _JiYuDissolveThreshold ("JiYuDissolveThreshold", Range(0,1)) = 0
        
        _JiYuTextureArray("JiYuTexture Array", 2DArray) = "" {}
        
        _OverlayTexEnable("OverlayTex Enable(启用覆盖贴图)", Range(0,1)) = 0
        _OverlayColor("Overlay Color(覆盖颜色)", Color) = (1, 1, 1, 1) //161
        _OverlayBlend("Overlay Blend(覆盖混合)", Range(0, 1)) = 1 // 163
        _OverlayGlow("Overlay Glow(覆盖强度)", Range(0,25)) = 1 // 162
        _OverlayTexSpeedX("OverlayTex SpeedX(覆盖速度x)", Range(-10,10)) = 0
        _OverlayTexSpeedY("OverlayTex SpeedY(覆盖速度y)", Range(-10,10)) = 0

        _OverlayIndex ("Overlay Index(覆盖贴图下标)", Int) = 0
        _OverlayTillingAndOffset("Overlay TillingAndOffset", Vector) = (1, 1, 0, 0)
        
        _JiYuScaleXY ("JiYuScaleXY", Vector) = (1, 1, 0, 0)

    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry"
        }

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
                float _JiYuAlphaOffsetY;
                float _AlphaCutOff;
                float4 _JiYuScaleXY;
                float4 _DissolveMap_ST;
                float _JiYuDissolveThreshold;
                int _OverlayTexEnable;
                float _OverlayTexSpeedX;
                float _OverlayTexSpeedY;
                float4 _OverlayColor;
                float _OverlayBlend;
                float _OverlayGlow;
                int _OverlayIndex;
                float4 _OverlayTillingAndOffset;
            CBUFFER_END

            #if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
					UNITY_DOTS_INSTANCED_PROP(float4, _Color)
					UNITY_DOTS_INSTANCED_PROP(float4, _MainTex_ST)
					UNITY_DOTS_INSTANCED_PROP(int2, _JiYuFlip)
					UNITY_DOTS_INSTANCED_PROP(int2, _JiYuSort)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuPivot)
					UNITY_DOTS_INSTANCED_PROP(float4, _JiYuScaleXY)
	                UNITY_DOTS_INSTANCED_PROP(float4, _OverlayColor)
                    UNITY_DOTS_INSTANCED_PROP(int, _OverlayTexEnable)
                    UNITY_DOTS_INSTANCED_PROP(int, _OverlayIndex)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuDissolveThreshold)
            
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

				#define _Color UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _Color)
				#define _MainTex_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _MainTex_ST)
                #define _JiYuFlip UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int2, _JiYuFlip)
                #define _JiYuSort UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int2, _JiYuSort)	
                #define _JiYuPivot UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuPivot)
                #define _JiYuDissolveThreshold UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuDissolveThreshold)	
                #define _JiYuScaleXY UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _JiYuScaleXY)
                #define _OverlayTexEnable UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int, _OverlayTexEnable)
                #define _OverlayIndex UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int, _OverlayIndex)
            #define _OverlayColor UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _OverlayColor)
            #endif

            sampler2D _MainTex;
            float4 _sortingGlobalData;
            sampler2D _DissolveMap;
            TEXTURE2D_ARRAY(_JiYuTextureArray);
            SAMPLER(sampler_JiYuTextureArray);

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

            half4 frag(v2f i) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(i);
                //float dissolveValue = tex2D(_DissolveMap, i.uv0.xy).r;
                //clip(dissolveValue - _DissolveThreshold);
                float4 color;
                if (_JiYuAlphaOffsetY > 0)
                {
                    if (i.uv0.y > _JiYuAlphaOffsetY)
                    {
                        color = tex2D(_MainTex, i.uv0.xy) * _Color;
                    }
                    else
                    {
                        color = tex2D(_MainTex, i.uv0.xy);
                    }
                }
                else
                {
                    color = tex2D(_MainTex, i.uv0.xy) * _Color;
                }
                if (_OverlayTexEnable > 0)
                {
                    float randomSeed = 1515;
                    float2 overlayUvs = i.uv0.xy;
                    overlayUvs.x += ((_Time.y + randomSeed) * (_OverlayTexSpeedX / 10)) % 1;
                    overlayUvs.y += ((_Time.y + randomSeed) * (_OverlayTexSpeedY / 10)) % 1;

                    float2 arrayTillAndOffset =overlayUvs*_OverlayTillingAndOffset.xy+_OverlayTillingAndOffset.zw;

                    
                    //float4 overlayCol = tex2DArray(_TextureArray, float3(overlayUvs, _Index));
                    //UnityTexture2DArray OverlayTexArray = UnityBuildTexture2DArrayStruct(_TextureArray);
                    //float2 uvOverlayTex = TRANSFORM_TEX(overlayUvs, _TextureArray);
                    //float4 overlayCol=SAMPLE_TEXTURE2D_ARRAY(OverlayTexArray.tex, OverlayTexArray.samplerstate, OverlayTexArray.GetTransformedUV(overlayUvs) );
                    //UnityTexture2D OverlayTex = UnityBuildTexture2DStruct(_OverlayTex);
                    //float2 uvOverlayTex = TRANSFORM_TEX(overlayUvs, _OverlayTex);
                    color = tex2D(_MainTex, i.uv0.xy) * _Color;
                    float4 overlayCol = SAMPLE_TEXTURE2D_ARRAY(_JiYuTextureArray, sampler_JiYuTextureArray, arrayTillAndOffset, _OverlayIndex);


                    overlayCol.rgb *= _OverlayColor.rgb * _OverlayGlow;

                    overlayCol.rgb *= overlayCol.a * _OverlayColor.rgb * _OverlayColor.a * _OverlayBlend;
                    color.rgb += overlayCol.rgb;

                    overlayCol.a *= _OverlayColor.a;
                    color.rgb = lerp(color.rgb, color.rgb * overlayCol, _OverlayBlend);
                }


                
                float4 dissolveSamp = tex2D(_DissolveMap, i.uv0.xy*_DissolveMap_ST.xy+_DissolveMap_ST.zw);
                float dissolveValue = dissolveSamp.r;
                clip(dissolveValue - _JiYuDissolveThreshold);
                clip(color.w - _AlphaCutOff);
                return color;
            }
            ENDHLSL
        }
    }
}