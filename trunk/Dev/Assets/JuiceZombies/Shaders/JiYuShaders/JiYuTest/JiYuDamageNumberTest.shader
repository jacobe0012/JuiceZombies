Shader "JiYuStudio/JiYuDamageNumberTest"
{
    Properties
    {
        _BgColor("BgColor", Color) = (1, 1, 1, 1)
        _JiYuFrontColor("JiYuFrontColor", Color) = (1, 1, 1, 1)
        _JiYuFrontColor1("JiYuFrontColor1", Color) = (1, 1, 1, 1)
        _JiYuFrontColor2("JiYuFrontColor2", Color) = (1, 1, 1, 1)
        _Dir("Dir",Vector) = (0.4, 0.6, 0, 0)
        //[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        //_MainTex_ST("MainTex_ST", Vector) = (1, 1, 0, 0)
        _ST0("ST0", Vector) = (5, 5, 0, 2)
        _ST1("ST1", Vector) = (5, 5, 1, 2)
        _ST2("ST2", Vector) = (5, 5, 2, 2)
        _ST3("ST3", Vector) = (5, 5, 3, 2)
        _ST4("ST4", Vector) = (5, 5, 4, 2)
        _JiYuFlip("JiYu Flip",Vector) = (0, 0, 0, 0)
        _JiYuSort("JiYu Sort",Vector) = (0, 0, 0, 0)
        _JiYuPivot("JiYu Pivot",Range(0, 1)) = 0.5
        _JiYuStartTime("JiYu StartTime", Float) = 0
        //_NumberInternal("NumberInternal",Range(0, 1)) = 0.369
        _Gap("Gap",Range(0, 1)) = 0.178

        _Duration1("Duration1增大阶段时间", Float) = 0.5
        _Duration2("Duration2保持最大阶段时间", Float) = 0.5
        _Duration3("Duration3缩小并褪色阶段时间", Float) = 0.5
        _InitSize("InitSize初始大小", Range(0, 1)) = 0.2
        _MaxSize("MaxSize最大大小", Range(0, 1)) = 0.8
        _OffsetY("OffsetY向上偏移量", Float) = 0.2
        _JiYuDamageTextureArray("JiYuDamageTextureArray", 2DArray) = "" {}
        _BackgroundTex ("Background Texture", 2D) = "white" {}

        _JiYuTextureArray("JiYuTexture Array", 2DArray) = "" {}

        _OverlayTexEnable("OverlayTex Enable(启用覆盖贴图)", Range(0,1)) = 0
        _OverlayColor("Overlay Color(覆盖颜色)", Color) = (1, 1, 1, 1) //161
        _OverlayBlend("Overlay Blend(覆盖混合)", Range(0, 1)) = 1 // 163
        _OverlayGlow("Overlay Glow(覆盖强度)", Range(0,25)) = 1 // 162
        _OverlayTexSpeedX("OverlayTex SpeedX(覆盖速度x)", Range(-10,10)) = 5
        _OverlayTexSpeedY("OverlayTex SpeedY(覆盖速度y)", Range(-10,10)) = 5

        _OverlayIndex ("Overlay Index(覆盖贴图下标)", Int) = 0
        _OverlayTillingAndOffset("Overlay TillingAndOffset", Vector) = (1, 1, 0, 0)
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent"
        }

        Pass
        {
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off

            Name "Pass"

            HLSLPROGRAM
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma instancing_options renderinglayer
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Assets\JuiceZombies\Shaders\JiYuShaders\JiYuTest\Cginc\JiYuShaderUtility.cginc"

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

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
                float4 _BgColor;
                float4 _JiYuFrontColor;
                float4 _JiYuFrontColor1;
                float4 _JiYuFrontColor2;
                float4 _Dir;

                //float4 _MainTex_ST;
                float4 _ST0;
                float4 _ST1;
                float4 _ST2;
                float4 _ST3;
                float4 _ST4;
                float4 _JiYuDamageTextureArray_ST;
                float4 _BackgroundTex_ST;
                int2 _JiYuFlip;
                int2 _JiYuSort;
                float _JiYuPivot;
                float _JiYuStartTime;
                float _OffsetY;
                //float _NumberInternal;
                float _Gap;
                float4x4 _JiYuNumber;
                float _Duration1;
                float _Duration2;
                float _Duration3;
                float _InitSize;
                float _MaxSize;

                float _OverlayTexEnable;
                float _OverlayTexSpeedX;
                float _OverlayTexSpeedY;

                //float4 _OverlayTex_ST;
                //float4 _OverlayTex_TexelSize;
                float4 _OverlayColor;
                float _OverlayBlend;
                float _OverlayGlow;
                int _OverlayIndex;
                float4 _OverlayTillingAndOffset;

            CBUFFER_END

            #if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
					UNITY_DOTS_INSTANCED_PROP(float4, _BgColor)
					UNITY_DOTS_INSTANCED_PROP(float4, _JiYuFrontColor)
                    UNITY_DOTS_INSTANCED_PROP(float4, _JiYuFrontColor1)
                    UNITY_DOTS_INSTANCED_PROP(float4, _JiYuFrontColor2)
					//UNITY_DOTS_INSTANCED_PROP(float4, _MainTex_ST)
					UNITY_DOTS_INSTANCED_PROP(int2, _JiYuFlip)
					UNITY_DOTS_INSTANCED_PROP(int2, _JiYuSort)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuPivot)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuStartTime)
					UNITY_DOTS_INSTANCED_PROP(float4x4, _JiYuNumber)					
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

				#define _BgColor UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _BgColor)
				#define _JiYuFrontColor UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _JiYuFrontColor)
                #define _JiYuFrontColor1 UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _JiYuFrontColor1)
                #define _JiYuFrontColor2 UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _JiYuFrontColor2)            
				//#define _MainTex_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _MainTex_ST)
                #define _JiYuFlip UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int2, _JiYuFlip)
                #define _JiYuSort UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int2, _JiYuSort)	
                #define _JiYuPivot UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuPivot)	
                #define _JiYuStartTime UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuStartTime)	
                #define _JiYuNumber UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4x4, _JiYuNumber)	
            #endif

            //sampler2D _MainTex;
            sampler2D _BackgroundTex;
            float4 _sortingGlobalData;
            TEXTURE2D_ARRAY(_JiYuDamageTextureArray);
            SAMPLER(sampler_JiYuDamageTextureArray);
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

                output.uv0 = v.uv0.xy * _JiYuDamageTextureArray_ST.xy + _JiYuDamageTextureArray_ST.zw;

                output.uv0.x = _JiYuFlip.x >= 1 ? (1.0 - output.uv0.x) : output.uv0.x;
                output.uv0.y = _JiYuFlip.y >= 1 ? (1.0 - output.uv0.y) : output.uv0.y;


                // 计算当前缩放因子
                float totalDuration = _Duration1 + _Duration2 + _Duration3;
                float sizeFactor;
                float offsetY;
                if (_JiYuStartTime < _Duration1) // 增大阶段
                {
                    sizeFactor = lerp(1.0 / _InitSize, 1.0 / _MaxSize, _JiYuStartTime / _Duration1);
                   
                }
                else if (_JiYuStartTime < _Duration1 + _Duration2) // 最大阶段
                {
                    sizeFactor = 1.0 / _MaxSize; // 保持最大大小
                }
                else // 缩小并褪色阶段
                {
                    float elapsed = _JiYuStartTime - (_Duration1 + _Duration2);
                    sizeFactor = lerp(1.0 / _MaxSize, 1.0 / _InitSize, (elapsed / _Duration3));
                }
                offsetY = lerp(0.001f, _OffsetY, _JiYuStartTime / _Duration1);
                // 计算 Tiling 值
                //float tiling = sizeFactor;

                // 计算 UV 以中心点为中心进行缩放
                // UV 坐标范围是 [0, 1]，将其调整到以中心为轴点
                output.uv0 = (output.uv0 - 0.5) * sizeFactor + 0.5; // 以中心为轴点进行缩放

                output.uv0.y = output.uv0.y - offsetY;
                // if (_JiYuStartTime < _Duration1)
                // {
                //     output.uv0.y = output.uv0.y + _JiYuStartTime * 0.01f;
                // }
                //output.uv0.xy *= _InitSize;

                #if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = v.instanceID;
                #endif

                return output;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset1, out float2 Out)
            {
                Out = UV * Tiling + Offset1;
            }

            // 立方插值函数
            float4 cubicLerp(float4 a, float4 b, float t)
            {
                float t3 = t * t * t;
                float t2 = t * t;
                return a + (b - a) * (6.0 * t2 * t - 15.0 * t2 + 10.0) * t3;
            }

            int calNumCount(float4x4 jiYuNumber)
            {
                int count = 0;
                if ((int)jiYuNumber[0].x != 14)
                {
                    count++;
                }
                if ((int)jiYuNumber[1].x != 14)
                {
                    count++;
                }
                if ((int)jiYuNumber[2].x != 14)
                {
                    count++;
                }
                if ((int)jiYuNumber[3].x != 14)
                {
                    count++;
                }
                if ((int)jiYuNumber[0].y != 14)
                {
                    count++;
                }
                return count;
            }

            float calBgOffsetX(int numCount)
            {
                float offsetX = -0.02;
                if (numCount == 2 || numCount == 4)
                {
                    offsetX = -0.06;
                }
                return offsetX;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(i);
                //float2 uvbg = i.uv0 * _JiYuDamageTextureArray_ST.xy + _JiYuDamageTextureArray_ST.zw; // 对第一个纹理应用平铺和偏移
                UnityTexture2DArray JiYuDamageTextureArray = UnityBuildTexture2DArrayStruct(_JiYuDamageTextureArray);
                float4 frontColor;
                //frontColor= tex2DArray(_JiYuDamageTextureArray, float3(i.uv0,14));
                //frontColor = UNITY_SAMPLE_TEX2DARRAY(_JiYuDamageTextureArray,float3(i.uv0,14 ));
                frontColor = PLATFORM_SAMPLE_TEXTURE2D_ARRAY(JiYuDamageTextureArray.tex,
                                             JiYuDamageTextureArray.samplerstate, i.uv0.xy, 14);

                /*float sizeFactor;

                if (_JiYuStartTime < _Duration1) // 增大阶段
                {
                    sizeFactor = lerp(1.0/_InitSize,1.0/_MaxSize, _JiYuStartTime / _Duration1);
                }
                else if (_JiYuStartTime < _Duration1 + _Duration2) // 最大阶段
                {
                    sizeFactor =  1.0/_MaxSize; // 保持最大大小
                }
                else // 缩小并褪色阶段
                {
                    float elapsed = _JiYuStartTime - (_Duration1 + _Duration2);
                    sizeFactor = lerp(1.0/_MaxSize,   1.0/_InitSize, (elapsed / _Duration3));
                }

                //float tempGap= _Gap * (sizeFactor );*/
                float tempGap = _Gap;
                float tempInternal = _Gap * 2;
                if (((1.0 - 2.0 * tempGap) / 5.0) * 0.0 + tempGap < i.uv0.x && i.uv0.x <= ((1.0 - 2.0 * tempGap) / 5.0)
                    * 1.0 + tempGap) // 第一部分
                {
                    //frontColor= UNITY_SAMPLE_TEX2DARRAY(_JiYuDamageTextureArray,float3(i.uv0.x *_ST0.x -_ST0.z-tempInternal*2,i.uv0.y *_ST0.y -_ST0.w ,(int)_JiYuNumber[0].x )); 
                    //frontColor= tex2DArray(_JiYuDamageTextureArray,float3(i.uv0.x *_ST0.x -_ST0.z-tempInternal*2,i.uv0.y *_ST0.y -_ST0.w ,(int)_JiYuNumber[0].x ));
                    frontColor = PLATFORM_SAMPLE_TEXTURE2D_ARRAY(JiYuDamageTextureArray.tex,
                               JiYuDamageTextureArray.samplerstate,
                               float2(i.uv0.x *_ST0.x -_ST0.z-tempInternal*2,i.
                                   uv0.y *_ST0.y -_ST0.w),
                               (int)_JiYuNumber[0].x);
                }
                else if ((1 - 2 * tempGap) / 5.0 * 1 + tempGap < i.uv0.x && i.uv0.x <= (1 - 2 * tempGap) / 5.0 * 2 +
                    tempGap) // 第二部分
                {
                    //frontColor= UNITY_SAMPLE_TEX2DARRAY(_JiYuDamageTextureArray,float3(i.uv0.x *_ST1.x -_ST1.z-tempInternal,i.uv0.y *_ST1.y -_ST1.w,(int)_JiYuNumber[1].x));
                    //frontColor= tex2DArray(_JiYuDamageTextureArray,float3(i.uv0.x *_ST1.x -_ST1.z-tempInternal,i.uv0.y *_ST1.y -_ST1.w,(int)_JiYuNumber[1].x));
                    frontColor = PLATFORM_SAMPLE_TEXTURE2D_ARRAY(JiYuDamageTextureArray.tex,
           JiYuDamageTextureArray.samplerstate,
           float2(i.uv0.x *_ST1.x -_ST1.z-tempInternal,i.uv0.y *_ST1.y -_ST1.w),
           (int)_JiYuNumber[1].x);
                }
                else if ((1 - 2 * tempGap) / 5.0 * 2 + tempGap < i.uv0.x && i.uv0.x <= (1 - 2 * tempGap) / 5.0 * 3 +
                    tempGap) // 第三部分
                {
                    //frontColor= UNITY_SAMPLE_TEX2DARRAY(_JiYuDamageTextureArray,float3(i.uv0.x *_ST2.x -_ST2.z,i.uv0.y *_ST2.y -_ST2.w,(int)_JiYuNumber[2].x));
                    //frontColor= tex2DArray(_JiYuDamageTextureArray,float3(i.uv0.x *_ST2.x -_ST2.z,i.uv0.y *_ST2.y -_ST2.w,(int)_JiYuNumber[2].x));
                    frontColor = PLATFORM_SAMPLE_TEXTURE2D_ARRAY(JiYuDamageTextureArray.tex,
                                                                      JiYuDamageTextureArray.samplerstate,
                                                                      float2(i.uv0.x *_ST2.x -_ST2.z,i.uv0.y *_ST2.y -
                                                                          _ST2.w),
                                                                      (int)_JiYuNumber[2].x);
                }
                else if ((1 - 2 * tempGap) / 5.0 * 3 + tempGap < i.uv0.x && i.uv0.x <= (1 - 2 * tempGap) / 5.0 * 4 +
                    tempGap) // 第四部分
                {
                    //frontColor= UNITY_SAMPLE_TEX2DARRAY(_JiYuDamageTextureArray,float3(i.uv0.x *_ST3.x -_ST3.z+tempInternal,i.uv0.y *_ST3.y -_ST3.w,(int)_JiYuNumber[3].x));
                    //frontColor= tex2DArray(_JiYuDamageTextureArray,float3(i.uv0.x *_ST3.x -_ST3.z+tempInternal,i.uv0.y *_ST3.y -_ST3.w,(int)_JiYuNumber[3].x));
                    frontColor = PLATFORM_SAMPLE_TEXTURE2D_ARRAY(JiYuDamageTextureArray.tex,
                                                                 JiYuDamageTextureArray.samplerstate,
                                                                 float2(i.uv0.x *_ST3.x -_ST3.z+tempInternal,i.uv0.y *
                                                                     _ST3.
                                                                     y -_ST3.w),
                                                                 (int)_JiYuNumber[3].x);
                }
                else if ((1 - 2 * tempGap) / 5.0 * 4 + tempGap < i.uv0.x && i.uv0.x <= (1 - 2 * tempGap) / 5.0 * 5 +
                    tempGap) // 第五部分
                {
                    //frontColor= UNITY_SAMPLE_TEX2DARRAY(_JiYuDamageTextureArray,float3(i.uv0.x *_ST4.x -_ST4.z+tempInternal*2,i.uv0.y *_ST4.y -_ST4.w,(int)_JiYuNumber[0].y));
                    //frontColor= tex2DArray(_JiYuDamageTextureArray,float3(i.uv0.x *_ST4.x -_ST4.z+tempInternal*2,i.uv0.y *_ST4.y -_ST4.w,(int)_JiYuNumber[0].y));
                    frontColor = PLATFORM_SAMPLE_TEXTURE2D_ARRAY(JiYuDamageTextureArray.tex,
                             JiYuDamageTextureArray.samplerstate,
                             float2(i.uv0.x *_ST4.x -_ST4.z+tempInternal*2,i.uv0.y *_ST4.y -_ST4.w),
                             (int)_JiYuNumber[0].y);
                }

                // 计算渐变颜色

                // float gradientValue = RemapFloat(i.uv0.y, _Dir.x, _Dir.y, 0, 1);
                //
                // gradientValue = smoothstep(0, 1, gradientValue);
                // gradientValue = smoothstep(0, 1, gradientValue);
                //
                // float4 tempfrontcol = lerp(_JiYuFrontColor2, _JiYuFrontColor1, gradientValue);
                
                float4 tempfrontcol = _JiYuFrontColor1;
                frontColor *= tempfrontcol;

                //output.uv0 = v.uv0.xy * _JiYuDamageTextureArray_ST.xy + _JiYuDamageTextureArray_ST.zw

                int count = calNumCount(_JiYuNumber);
                float offsetX = calBgOffsetX(count);
                float2 offsetXY = _BackgroundTex_ST.zw;
                offsetXY.x = offsetX;
                float4 bgColor = tex2D(_BackgroundTex, i.uv0.xy * _BackgroundTex_ST.xy + offsetXY) * _BgColor;
                //float4 iconColor = tex2D(_MainTex, i.uv0.xy) * _Color;
                //float4 finalColor = bgColor + color;
                //float4 finalColor = bgColor * iconColor;
                if (_OverlayTexEnable > 0)
                {
                    float2 overlayUvs = i.uv0.xy;
                    overlayUvs.x += ((_Time.y + 1) * (_OverlayTexSpeedX / 10)) % 1;
                    overlayUvs.y += ((_Time.y + 1) * (_OverlayTexSpeedY / 10)) % 1;

                    float2 arrayTillAndOffset;
                    Unity_TilingAndOffset_float(overlayUvs, _OverlayTillingAndOffset.xy, _OverlayTillingAndOffset.zw,
                        arrayTillAndOffset);

                    //float4 overlayCol = UNITY_SAMPLE_TEX2DARRAY(_JiYuTextureArray,float3(arrayTillAndOffset,_OverlayIndex));

                    float4 overlayCol = PLATFORM_SAMPLE_TEXTURE2D_ARRAY(JiYuDamageTextureArray.tex,
                        JiYuDamageTextureArray.samplerstate, float2(arrayTillAndOffset), _OverlayIndex);
                    //float4 overlayCol = tex2DArray(_JiYuTextureArray,float3(arrayTillAndOffset,_OverlayIndex));
                    //float4 overlayCol = PLATFORM_SAMPLE_TEXTURE2D_ARRAY(JiYuTexArray.tex, JiYuTexArray.samplerstate,
                    //                                                    arrayTillAndOffset, _OverlayIndex);


                    //float4 overlayCol = tex2DArray(_TextureArray, float3(overlayUvs, _Index));
                    //UnityTexture2DArray OverlayTexArray = UnityBuildTexture2DArrayStruct(_TextureArray);
                    //float2 uvOverlayTex = TRANSFORM_TEX(overlayUvs, _TextureArray);
                    //float4 overlayCol=SAMPLE_TEXTURE2D_ARRAY(OverlayTexArray.tex, OverlayTexArray.samplerstate, OverlayTexArray.GetTransformedUV(overlayUvs) );
                    //UnityTexture2D OverlayTex = UnityBuildTexture2DStruct(_OverlayTex);
                    //float2 uvOverlayTex = TRANSFORM_TEX(overlayUvs, _OverlayTex);

                    //float4 overlayCol = SAMPLE_TEXTURE2D(OverlayTex.tex, OverlayTex.samplerstate, OverlayTex.GetTransformedUV(uvOverlayTex) );


                    overlayCol.rgb *= _OverlayColor.rgb * _OverlayGlow;

                    overlayCol.rgb *= overlayCol.a * _OverlayColor.rgb * _OverlayColor.a * _OverlayBlend;
                    frontColor.rgb += overlayCol.rgb;

                    overlayCol.a *= _OverlayColor.a;
                    frontColor.rgb = lerp(frontColor.rgb, frontColor.rgb * overlayCol, _OverlayBlend);
                }

                float4 finalColor = bgColor * (1.0 - frontColor.a) + frontColor * frontColor.a;


                // 计算透明度
                float totalDuration = _Duration1 + _Duration2 + _Duration3;
                float alpha = 1.0;

                if (_JiYuStartTime >= totalDuration) // 完全透明
                {
                    alpha = 0.0;
                }
                else if (_JiYuStartTime >= _Duration1 + _Duration2) // 褪色阶段
                {
                    float elapsed = _JiYuStartTime - (_Duration1 + _Duration2);
                    alpha = 1.0 - (elapsed / _Duration3); // 线性减小透明度
                }

                finalColor.w *= alpha;


                //clip(finalColor.w - 0.05);
                return finalColor;
            }
            ENDHLSL
        }
    }
}