Shader "JiYuStudio/JiYuGPUAnimationOutlineTest"
{
    Properties
    {
        [MainTexture]_MainTex("MainTex", 2D) = "white" {}
        ClipThreshold("ClipThreshold", Range(0, 1)) = 0.01
        [NoScaleOffset]_AnimatedBoneMatrices("AnimatedBoneMatrices", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _JiYuSort("JiYuSort", Vector) = (0, 0, 0, 0)
        _JiYuFlip("JiYuFlip", Vector) = (0, 0, 0, 0)
        _JiYuPivot("JiYuPivot", Range(0, 1)) = 0.5
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

        _JiYuTextureArray("JiYuTexture Array", 2DArray) = "" {}

        _OutlineEnable("Outline Enable(启用描边)", Range(0,1)) = 0
        _OutlineWidth("Outline Base Width(描边宽度)", Range(0,5)) = 2
        _OutlineAlpha("Outline Base Alpha(描边透明度)", Range(0,1)) = 1
        _OutlineColor("Outline Color(描边颜色)", Color) = (1, 1, 1, 1)
        _OutlineGlow("Outline Glow(描边颜色发光强度)", Range(1,100)) = 1

        _OutlineDistortEnable("OutlineDistort Enable(启用描边变形)", Range(0,1)) = 0
        //_OutlineDistortTex("OutlineDistort Tex", 2D) = "white" {}
        _OutlineDistortAmount("OutlineDistort Amount(变形幅度)", Range(0,2)) = 1

        _OutlineDistortSpeedX("OutlineDistort SpeedX(变形速度x)", Range(-10,10)) = 5
        _OutlineDistortSpeedY("OutlineDistort SpeedY(变形速度y)", Range(-10,10)) = 5

        _OutlineDistortIndex("OutlineDistort Index(变形贴图下标)", Int) = 0
        _OutlineDistortTillingAndOffset("OutlineDistort TillingAndOffset", Vector) = (1, 1, 0, 0)

        _OutlineTexEnable("OutlineTex Enable(启用描边贴图)", Range(0,1)) = 0
        //_OutlineTex("Outline Tex", 2D) = "white" {}
        _OutlineSpeedX("OutlineTex SpeedX(描边贴图速度x)", Range(-10,10)) = 5
        _OutlineSpeedY("OutlineTex SpeedY(描边贴图速度y)", Range(-10,10)) = 5

        _OutlineTexIndex("OutlineTex Index(描边贴图下标)", Int) = 0
        _OutlineTexTillingAndOffset("OutlineTex TillingAndOffset", Vector) = (1, 1, 0, 0)

        _OverlayTexEnable("OverlayTex Enable(启用覆盖贴图)", Range(0,1)) = 0
        _OverlayColor("Overlay Color(覆盖颜色)", Color) = (1, 1, 1, 1) //161
        _OverlayBlend("Overlay Blend(覆盖混合)", Range(0, 1)) = 0.124 // 163
        _OverlayGlow("Overlay Glow(覆盖强度)", Range(0,25)) = 16.5 // 162
        _OverlayTexSpeedX("OverlayTex SpeedX(覆盖速度x)", Range(-10,10)) = 0.3
        _OverlayTexSpeedY("OverlayTex SpeedY(覆盖速度y)", Range(-10,10)) = -0.3

        _OverlayIndex ("Overlay Index(覆盖贴图下标)", Int) = 0
        _OverlayTillingAndOffset("Overlay TillingAndOffset", Vector) = (5, 5, 0, 0)
        //残影
        _ChromAberrEnable("ChromAberr Enable Enable(启用残影)", Range(0,1)) = 0
		_ChromAberrAmount("ChromAberr Amount(残影偏移)", Range(0, 1)) = 0.11 //78
		_ChromAberrAlpha("ChromAberr Alpha(残影透明度)", Range(0, 1)) = 0.25 //79
        
        _DissolveMap ("Dissolve Map", 2D) = "white" {}
        _JiYuDissolveThreshold ("JiYuDissolveThreshold", Range(0,1)) = 0
        _JiYuScaleXY ("JiYuScaleXY", Vector) = (1, 1, 0, 0)
    }
    SubShader
    {

        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="TransparentCutout"
            "UniversalMaterialType" = "Unlit"
            "Queue"="AlphaTest"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite On

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM
            // Pragmas
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma instancing_options renderinglayer
            #pragma vertex vert
            #pragma fragment frag

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma shader_feature _ _SAMPLE_GI
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            // GraphKeywords: <None>

            // Defines

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_TEXCOORD1
            #define ATTRIBUTES_NEED_TEXCOORD2
            #define ATTRIBUTES_NEED_TEXCOORD3
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
            #define _FOG_FRAGMENT 1
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define _ALPHATEST_ON 1


            // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

            // Includes
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            // custom interpolators pre packing
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                float4 uv3 : TEXCOORD3;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float3 normalWS;
                float4 texCoord0;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };

            struct SurfaceDescriptionInputs
            {
                float4 uv0;
            };

            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
                float4 uv1;
                float4 uv2;
                float4 uv3;
            };

            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0 : INTERP0;
                float3 positionWS : INTERP1;
                float3 normalWS : INTERP2;
                #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };


            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output;
                output.positionCS = input.positionCS;
                output.texCoord0 = input.texCoord0.xyzw;
                output.positionWS = input.positionWS.xyz;
                output.normalWS = input.normalWS.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif
                return output;
            }


            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float4 _MainTex_ST;
                float ClipThreshold;
                float _JiYuPivot;
                float4 _AnimatedBoneMatrices_TexelSize;
                float4x4 _AnimationState;
                float4 _Color;
                int2 _JiYuSort;
                int2 _JiYuFlip;
                float4 _JiYuScaleXY;

                float _OutlineWidth;
                float _OutlineAlpha;
                float4 _OutlineColor;
                float _OutlineGlow;
                float _OutlineSpeedX;
                float _OutlineSpeedY;
                float _OutlineDistortAmount;
                float4 _OutlineDistortTex_ST;
                float4 _OutlineDistortTex_TexelSize;
                float _OutlineDistortEnable;
                float _OutlineDistortSpeedX;
                float _OutlineDistortSpeedY;

                float _OutlineEnable;
                float _OutlineTexEnable;
                float4 _OutlineTex_ST;
                float4 _OutlineTex_TexelSize;

                int _OverlayTexEnable;
                float _OverlayTexSpeedX;
                float _OverlayTexSpeedY;

                //float4 _OverlayTex_ST;
                //float4 _OverlayTex_TexelSize;
                float4 _OverlayColor;
                float _OverlayBlend;
                float _OverlayGlow;
                int _OverlayIndex;
                float4 _OverlayTillingAndOffset;

                int _OutlineDistortIndex;
                float4 _OutlineDistortTillingAndOffset;

                int _OutlineTexIndex;
                float4 _OutlineTexTillingAndOffset;
                float _JiYuDissolveThreshold;
                float4 _DissolveMap_TexelSize;
                float4 _DissolveMap_ST;
            
                int _ChromAberrEnable;
                float _ChromAberrAmount;
                float _ChromAberrAlpha;
            CBUFFER_END

            #if defined(DOTS_INSTANCING_ON)
        // DOTS instancing definitions
        UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, ClipThreshold)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, _JiYuPivot)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float4x4, _AnimationState)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float4, _Color)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float4, _OverlayColor)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float4, _JiYuScaleXY)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(int2, _JiYuSort)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(int2, _JiYuFlip)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, _JiYuDissolveThreshold)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(int, _OverlayTexEnable)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(int, _OverlayIndex)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(int, _ChromAberrEnable)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(int, _OutlineEnable)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, _ChromAberrAmount)
            
        UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
        // DOTS instancing usage macros
        #define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(type, var)
            #elif defined(UNITY_INSTANCING_ENABLED)
        // Unity instancing definitions
        UNITY_INSTANCING_BUFFER_START(SGPerInstanceData)
            UNITY_DEFINE_INSTANCED_PROP(float, ClipThreshold)
            UNITY_DEFINE_INSTANCED_PROP(float, _JiYuPivot)
            UNITY_DEFINE_INSTANCED_PROP(float4x4, _AnimationState)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_DEFINE_INSTANCED_PROP(float4, _OverlayColor)
            UNITY_DEFINE_INSTANCED_PROP(float4, _JiYuScaleXY)
            UNITY_DEFINE_INSTANCED_PROP(int2, _JiYuSort)
            UNITY_DEFINE_INSTANCED_PROP(int2, _JiYuFlip)
            UNITY_DEFINE_INSTANCED_PROP(float, _JiYuDissolveThreshold)
            UNITY_DEFINE_INSTANCED_PROP(int, _OverlayTexEnable)
            UNITY_DEFINE_INSTANCED_PROP(int, _OverlayIndex)
            UNITY_DEFINE_INSTANCED_PROP(int, _ChromAberrEnable)
            UNITY_DEFINE_INSTANCED_PROP(int, _OutlineEnable)
            UNITY_DEFINE_INSTANCED_PROP(float, _ChromAberrAmount)
        UNITY_INSTANCING_BUFFER_END(SGPerInstanceData)
        // Unity instancing usage macros
        #define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) UNITY_ACCESS_INSTANCED_PROP(SGPerInstanceData, var)
            #else
            #define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) var
            #endif

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_AnimatedBoneMatrices);
            SAMPLER(sampler_AnimatedBoneMatrices);
            TEXTURE2D(_OutlineDistortTex);
            SAMPLER(sampler_OutlineDistortTex);
            TEXTURE2D(_OutlineTex);
            SAMPLER(sampler_OutlineTex);
            TEXTURE2D(_DissolveMap);
            SAMPLER(sampler_DissolveMap);
            
            //TEXTURE2D(_OverlayTex);
            //SAMPLER(sampler_OverlayTex);

            TEXTURE2D_ARRAY(_JiYuTextureArray);
            SAMPLER(sampler_JiYuTextureArray);
            // Graph Includes
            #include "Assets/JuiceZombies/Shaders/JiYuShaders/JiYuTest/Cginc/JiYuGpuEcsAnimator.cginc"
            #include "Assets\JuiceZombies\Shaders\JiYuShaders\JiYuTest\Cginc\JiYuShaderUtility.cginc"
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
            #endif

            // Graph Functions
            float4 _sortingGlobalData;

            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);


                //Add
                float _jiYuPivot = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_JiYuPivot, float);
                int2 _jiYuSort = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_JiYuSort, int2);

                output.positionCS = input.positionCS;
                output.positionCS.z = JiYuSortFunc(_jiYuPivot, _jiYuSort, _sortingGlobalData,UNITY_MATRIX_MVP);
                
                //output.positionCS = (output.positionCS - 0.5) * 1.0 + 0.5; // 以中心为轴点进行缩放
                //output.positionCS = input.positionCS;
                output.texCoord0.xyzw = input.texCoord0;
                output.positionWS.xyz = input.positionWS;
                output.normalWS.xyz = input.normalWS;
                #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
                #endif
                return output;
            }

            void Unity_Step_float(float Edge, float In, out float Out)
            {
                Out = step(Edge, In);
            }

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            void Unity_MatrixConstruction_Row_half(half4 M0, half4 M1, half4 M2, half4 M3, out half4x4 Out4x4,
                                               out half3x3 Out3x3, out half2x2 Out2x2)
            {
                Out4x4 = half4x4(M0.x, M0.y, M0.z, M0.w, M1.x, M1.y, M1.z, M1.w, M2.x, M2.y, M2.z, M2.w, M3.x, M3.y,
    M3.z, M3.w);
                Out3x3 = half3x3(M0.x, M0.y, M0.z, M1.x, M1.y, M1.z, M2.x, M2.y, M2.z);
                Out2x2 = half2x2(M0.x, M0.y, M1.x, M1.y);
            }

            struct Bindings_GpuEcsAnimatorSimple_29945fd86e6b030489a032c16eb38957_half
            {
                float3 ObjectSpacePosition;
                half4 uv1;
                half4 uv2;
                half4 uv3;
            };

            void SG_GpuEcsAnimatorSimple_29945fd86e6b030489a032c16eb38957_half(
                UnityTexture2D _AnimatedBoneMatrices, half4x4 _AnimationState,
                Bindings_GpuEcsAnimatorSimple_29945fd86e6b030489a032c16eb38957_half IN, out half3 position_1)
            {
                half4 _UV_1be6761a5b46434d9450a201247cecab_Out_0_Vector4 = IN.uv1;
                half4 _UV_9d11b56d03964ee7949150f0411ee35d_Out_0_Vector4 = IN.uv2;
                half4 _UV_9a1c6fa68d794e9d8e976f5e3ca926c7_Out_0_Vector4 = IN.uv3;
                half4x4 _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var4x4_4_Matrix4;
                half3x3 _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var3x3_5_Matrix3;
                half2x2 _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var2x2_6_Matrix2;
                Unity_MatrixConstruction_Row_half(_UV_1be6761a5b46434d9450a201247cecab_Out_0_Vector4,
        _UV_9d11b56d03964ee7949150f0411ee35d_Out_0_Vector4,
        _UV_9a1c6fa68d794e9d8e976f5e3ca926c7_Out_0_Vector4,
        half4(0, 0, 0, 0),
        _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var4x4_4_Matrix4,
        _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var3x3_5_Matrix3,
        _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var2x2_6_Matrix2);
                UnityTexture2D _Property_954fd2a177f34fd2b3433d2b94426dce_Out_0_Texture2D = _AnimatedBoneMatrices;
                half4x4 _Property_1b920faf4fa848428db0791ec7c51422_Out_0_Matrix4 = _AnimationState;
                half3 _AnimateBlendPosCustomFunction_536a9facb4c94737affba23c19d4719f_positionOut_2_Vector3;
                AnimateBlendPos_half(IN.ObjectSpacePosition,
         _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var4x4_4_Matrix4,
         _Property_954fd2a177f34fd2b3433d2b94426dce_Out_0_Texture2D.tex,
         _Property_1b920faf4fa848428db0791ec7c51422_Out_0_Matrix4,
         _AnimateBlendPosCustomFunction_536a9facb4c94737affba23c19d4719f_positionOut_2_Vector3);
                position_1 = _AnimateBlendPosCustomFunction_536a9facb4c94737affba23c19d4719f_positionOut_2_Vector3;
            }

            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }

            void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A * B;
            }

            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                float4 _jiyuScaleXY = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_JiYuScaleXY, float4);
                int2 _Property_551fb6da021d4753b8e9fbe4857cbfdf_Out_0_Vector2 = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                    _JiYuFlip, int2);
                float _Split_cfa101a3cf324e3c8dc09dfe49232c27_R_1_Float =
                    _Property_551fb6da021d4753b8e9fbe4857cbfdf_Out_0_Vector2[0];
                float _Split_cfa101a3cf324e3c8dc09dfe49232c27_G_2_Float =
                    _Property_551fb6da021d4753b8e9fbe4857cbfdf_Out_0_Vector2[1];
                float _Split_cfa101a3cf324e3c8dc09dfe49232c27_B_3_Float = 0;
                float _Split_cfa101a3cf324e3c8dc09dfe49232c27_A_4_Float = 0;
                float _Step_eddd1bc0ab8244468236e9e9a3b19c43_Out_2_Float;
                Unity_Step_float(0.5, _Split_cfa101a3cf324e3c8dc09dfe49232c27_R_1_Float,
               _Step_eddd1bc0ab8244468236e9e9a3b19c43_Out_2_Float);
                float _Multiply_07dc1b2aa8ce41a8ba2fdf1cbd3ab7df_Out_2_Float;
                Unity_Multiply_float_float(_Step_eddd1bc0ab8244468236e9e9a3b19c43_Out_2_Float, -2,
                    _Multiply_07dc1b2aa8ce41a8ba2fdf1cbd3ab7df_Out_2_Float);
                float _Add_9e398982ebc94ae39e1684c6dae740b8_Out_2_Float;
                Unity_Add_float(_Multiply_07dc1b2aa8ce41a8ba2fdf1cbd3ab7df_Out_2_Float, 1,
                           _Add_9e398982ebc94ae39e1684c6dae740b8_Out_2_Float);
                UnityTexture2D _Property_596d299abef8414ab417ac4c6e34581d_Out_0_Texture2D =
                    UnityBuildTexture2DStructNoScale(_AnimatedBoneMatrices);
                float4x4 _Property_c6e8e2d379a94ca1afc58ae69a691d3c_Out_0_Matrix4 = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                    _AnimationState, float4x4);
                Bindings_GpuEcsAnimatorSimple_29945fd86e6b030489a032c16eb38957_half
                    _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5;
                _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5.ObjectSpacePosition = IN.ObjectSpacePosition;
                _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5.uv1 = IN.uv1;
                _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5.uv2 = IN.uv2;
                _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5.uv3 = IN.uv3;
                half3 _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5_position_1_Vector3;
                SG_GpuEcsAnimatorSimple_29945fd86e6b030489a032c16eb38957_half(
                    _Property_596d299abef8414ab417ac4c6e34581d_Out_0_Texture2D,
                    _Property_c6e8e2d379a94ca1afc58ae69a691d3c_Out_0_Matrix4,
                    _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5,
                    _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5_position_1_Vector3);
                half _Split_25a39bfe4cb04ded8715c1365a37d951_R_1_Float =
                    _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5_position_1_Vector3[0];
                half _Split_25a39bfe4cb04ded8715c1365a37d951_G_2_Float =
                    _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5_position_1_Vector3[1];
                half _Split_25a39bfe4cb04ded8715c1365a37d951_B_3_Float =
                    _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5_position_1_Vector3[2];
                half _Split_25a39bfe4cb04ded8715c1365a37d951_A_4_Float = 0;
                float _Multiply_938e5f2c76134d9a816d4f5a69e4359f_Out_2_Float;
                Unity_Multiply_float_float(_Add_9e398982ebc94ae39e1684c6dae740b8_Out_2_Float,
                                _Split_25a39bfe4cb04ded8715c1365a37d951_R_1_Float,
                                _Multiply_938e5f2c76134d9a816d4f5a69e4359f_Out_2_Float);
                float _Step_a1d209e0ead84e8d84fef5f9ee3cd6df_Out_2_Float;
                Unity_Step_float(0.5, _Split_cfa101a3cf324e3c8dc09dfe49232c27_G_2_Float,
                    _Step_a1d209e0ead84e8d84fef5f9ee3cd6df_Out_2_Float);
                float _Multiply_15cc14504d064f3fbd72c140e3e644d1_Out_2_Float;
                Unity_Multiply_float_float(_Step_a1d209e0ead84e8d84fef5f9ee3cd6df_Out_2_Float, -2,
                    _Multiply_15cc14504d064f3fbd72c140e3e644d1_Out_2_Float);
                float _Add_d972882d354c44e9b9b7449cb467af75_Out_2_Float;
                Unity_Add_float(_Multiply_15cc14504d064f3fbd72c140e3e644d1_Out_2_Float, 1,
                                                                    _Add_d972882d354c44e9b9b7449cb467af75_Out_2_Float);
                float _Multiply_cf1c687d46fe42b3b9727313ccc0f119_Out_2_Float;
                Unity_Multiply_float_float(_Add_d972882d354c44e9b9b7449cb467af75_Out_2_Float,
                        _Split_25a39bfe4cb04ded8715c1365a37d951_G_2_Float,
                        _Multiply_cf1c687d46fe42b3b9727313ccc0f119_Out_2_Float);
                half _Split_fd49aeed15584d61934e964d16c03262_R_1_Float =
                    _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5_position_1_Vector3[0];
                half _Split_fd49aeed15584d61934e964d16c03262_G_2_Float =
                    _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5_position_1_Vector3[1];
                half _Split_fd49aeed15584d61934e964d16c03262_B_3_Float =
                    _GpuEcsAnimatorSimple_8fef306f7e4f467099d4d1e084e7dca5_position_1_Vector3[2];
                half _Split_fd49aeed15584d61934e964d16c03262_A_4_Float = 0;
                float4 _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RGBA_4_Vector4;
                float3 _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RGB_5_Vector3;
                float2 _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RG_6_Vector2;
                Unity_Combine_float(_Multiply_938e5f2c76134d9a816d4f5a69e4359f_Out_2_Float,
                    _Multiply_cf1c687d46fe42b3b9727313ccc0f119_Out_2_Float,
                    _Split_fd49aeed15584d61934e964d16c03262_B_3_Float, 0,
                    _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RGBA_4_Vector4,
                    _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RGB_5_Vector3,
                    _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RG_6_Vector2);
                description.Position = _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RGB_5_Vector3;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;

                description.Position.xy =description.Position.xy * _jiyuScaleXY.xy;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
                return output;
            }

            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float Alpha;
                float AlphaClipThreshold;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                
                float ChromAberrAmount = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                    _ChromAberrAmount, float);
                int OutlineEnable = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                    _OutlineEnable, int);
                int ChromAberrEnable = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                    _ChromAberrEnable, int);
                // float ChromAberrAmount = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                //     _ChromAberrAmount, float);
                // float ChromAberrAlpha = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                //     _ChromAberrAlpha, float);
                float4 OverlayColor = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                    _OverlayColor, float4);
                int OverlayTexEnable = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                    _OverlayTexEnable, int);
                int OverlayIndex = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                    _OverlayIndex, int);
                float jiYuDissolveThreshold = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                    _JiYuDissolveThreshold, float);
                float4 _Property_eb4e1ba82b1f42bd9015fe56185a8fd4_Out_0_Vector4 = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                    _Color, float4);

                UnityTexture2D _DissolveMap_Texture2D = UnityBuildTexture2DStruct(
                    _DissolveMap);
                float4 dissolveSamp = SAMPLE_TEXTURE2D(
                    _DissolveMap_Texture2D.tex,
                    _DissolveMap_Texture2D.samplerstate,
                    _DissolveMap_Texture2D.GetTransformedUV(IN.uv0.xy));


                UnityTexture2D _Property_886bab260a0342e7a7802e84b57c639d_Out_0_Texture2D = UnityBuildTexture2DStruct(
                    _MainTex);
                float4 _SampleTexture2D_77e31d07f71f45258e4996190aad0f69_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(
                    _Property_886bab260a0342e7a7802e84b57c639d_Out_0_Texture2D.tex,
                    _Property_886bab260a0342e7a7802e84b57c639d_Out_0_Texture2D.samplerstate,
                    _Property_886bab260a0342e7a7802e84b57c639d_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_77e31d07f71f45258e4996190aad0f69_R_4_Float =
                    _SampleTexture2D_77e31d07f71f45258e4996190aad0f69_RGBA_0_Vector4.r;
                float _SampleTexture2D_77e31d07f71f45258e4996190aad0f69_G_5_Float =
                    _SampleTexture2D_77e31d07f71f45258e4996190aad0f69_RGBA_0_Vector4.g;
                float _SampleTexture2D_77e31d07f71f45258e4996190aad0f69_B_6_Float =
                    _SampleTexture2D_77e31d07f71f45258e4996190aad0f69_RGBA_0_Vector4.b;
                float _SampleTexture2D_77e31d07f71f45258e4996190aad0f69_A_7_Float =
                    _SampleTexture2D_77e31d07f71f45258e4996190aad0f69_RGBA_0_Vector4.a;
                float4 _Multiply_8e94b6b0a2a347c991af0c084ed17ad6_Out_2_Vector4;
                Unity_Multiply_float4_float4(_Property_eb4e1ba82b1f42bd9015fe56185a8fd4_Out_0_Vector4,
                     _SampleTexture2D_77e31d07f71f45258e4996190aad0f69_RGBA_0_Vector4,
                     _Multiply_8e94b6b0a2a347c991af0c084ed17ad6_Out_2_Vector4);
                float _Split_9984097e44554fbea2620cd324b728c3_R_1_Float =
                    _Multiply_8e94b6b0a2a347c991af0c084ed17ad6_Out_2_Vector4[0];
                float _Split_9984097e44554fbea2620cd324b728c3_G_2_Float =
                    _Multiply_8e94b6b0a2a347c991af0c084ed17ad6_Out_2_Vector4[1];
                float _Split_9984097e44554fbea2620cd324b728c3_B_3_Float =
                    _Multiply_8e94b6b0a2a347c991af0c084ed17ad6_Out_2_Vector4[2];
                float _Split_9984097e44554fbea2620cd324b728c3_A_4_Float =
                    _Multiply_8e94b6b0a2a347c991af0c084ed17ad6_Out_2_Vector4[3];
                float4 _Combine_2cd83cd387eb4a2194c7ac2703b14770_RGBA_4_Vector4;
                float3 _Combine_2cd83cd387eb4a2194c7ac2703b14770_RGB_5_Vector3;
                float2 _Combine_2cd83cd387eb4a2194c7ac2703b14770_RG_6_Vector2;
                Unity_Combine_float(_Split_9984097e44554fbea2620cd324b728c3_R_1_Float,
                    _Split_9984097e44554fbea2620cd324b728c3_G_2_Float,
                    _Split_9984097e44554fbea2620cd324b728c3_B_3_Float,
                    _Split_9984097e44554fbea2620cd324b728c3_A_4_Float,
                    _Combine_2cd83cd387eb4a2194c7ac2703b14770_RGBA_4_Vector4,
                    _Combine_2cd83cd387eb4a2194c7ac2703b14770_RGB_5_Vector3,
                    _Combine_2cd83cd387eb4a2194c7ac2703b14770_RG_6_Vector2);
                float4 _Add_64c3281a28444661a3f096720cfb0795_Out_2_Vector4;
                Unity_Add_float4(_Combine_2cd83cd387eb4a2194c7ac2703b14770_RGBA_4_Vector4, float4(0, 0, 0, 0),
                                                              _Add_64c3281a28444661a3f096720cfb0795_Out_2_Vector4);
                float _Split_5f093b3df0924520b8ea63f5245bd967_R_1_Float =
                    _Add_64c3281a28444661a3f096720cfb0795_Out_2_Vector4[0];
                float _Split_5f093b3df0924520b8ea63f5245bd967_G_2_Float =
                    _Add_64c3281a28444661a3f096720cfb0795_Out_2_Vector4[1];
                float _Split_5f093b3df0924520b8ea63f5245bd967_B_3_Float =
                    _Add_64c3281a28444661a3f096720cfb0795_Out_2_Vector4[2];
                float _Split_5f093b3df0924520b8ea63f5245bd967_A_4_Float =
                    _Add_64c3281a28444661a3f096720cfb0795_Out_2_Vector4[3];
                float _Property_3ae0775973644a4a9c4faad94eb3c2c3_Out_0_Float = UNITY_ACCESS_HYBRID_INSTANCED_PROP(
                    ClipThreshold, float);


                surface.BaseColor = (_Add_64c3281a28444661a3f096720cfb0795_Out_2_Vector4.xyz);
                surface.Alpha = _Split_5f093b3df0924520b8ea63f5245bd967_A_4_Float;
                surface.AlphaClipThreshold = _Property_3ae0775973644a4a9c4faad94eb3c2c3_Out_0_Float;
                float randomSeed = 1535;
                UnityTexture2DArray JiYuTexArray = UnityBuildTexture2DArrayStruct(_JiYuTextureArray);
                if (OutlineEnable > 0)
                {
                    UnityTexture2D tex = UnityBuildTexture2DStruct(_MainTex);
                    float4 tex2 = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate, tex.GetTransformedUV(IN.uv0.xy));
                    half2 destUv = half2(_OutlineWidth * _MainTex_TexelSize.x * 200,_OutlineWidth * _MainTex_TexelSize.y * 200);

                    // OUTDIST_ON
                    if (_OutlineDistortEnable > 0)
                    {
                        float2 uvOutDistTex = IN.uv0.xy;
                        //


                        float _OutlineDistortTexXSpeed = _OutlineDistortSpeedX;
                        float _OutlineDistortTexYSpeed = _OutlineDistortSpeedY;
                        uvOutDistTex.x += ((_Time.x + randomSeed) * _OutlineDistortTexXSpeed) % 1;
                        uvOutDistTex.y += ((_Time.x + randomSeed) * _OutlineDistortTexYSpeed) % 1;

                        //UnityTexture2D OutDistTex = UnityBuildTexture2DStruct(_OutlineDistortTex);
                        //float4 uvOutDistTex1 = SAMPLE_TEXTURE2D(OutDistTex.tex, OutDistTex.samplerstate, OutDistTex.GetTransformedUV(uvOutDistTex) );

                        float2 OutDistTillAndOffset;
                        Unity_TilingAndOffset_float(uvOutDistTex, _OutlineDistortTillingAndOffset.xy,
                     _OutlineDistortTillingAndOffset.zw, OutDistTillAndOffset);
                        float4 uvOutDistTex1 = PLATFORM_SAMPLE_TEXTURE2D_ARRAY(
                            JiYuTexArray.tex, JiYuTexArray.samplerstate, OutDistTillAndOffset, _OutlineDistortIndex);

                        half outDistortAmnt = (uvOutDistTex1.r - 0.5) * 0.2 * _OutlineDistortAmount;
                        destUv.x += outDistortAmnt;
                        destUv.y += outDistortAmnt;
                        //
                    }
                    float4 spriteLeft1 = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate,
                            tex.GetTransformedUV(IN.uv0.xy + half2(destUv.x, 0)));
                    float4 spriteLeft2 = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate,
                                          tex.GetTransformedUV(IN.uv0.xy - half2(destUv.x, 0)));
                    float4 spriteLeft3 = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate,
                        tex.GetTransformedUV(IN.uv0.xy + half2(0, destUv.y)));
                    float4 spriteLeft4 = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate,
                        tex.GetTransformedUV(IN.uv0.xy - half2(0,
                            destUv.y)));


                    half spriteLeft = spriteLeft1.a;
                    half spriteRight = spriteLeft2.a;
                    half spriteBottom = spriteLeft3.a;
                    half spriteTop = spriteLeft4.a;

                    //half spriteLeft = tex2D(_MainTex, IN + half2(destUv.x, 0)).a;
                    //half spriteRight = tex2D(_MainTex, IN - half2(destUv.x, 0)).a;
                    //half spriteBottom = tex2D(_MainTex, IN + half2(0, destUv.y)).a;
                    //half spriteTop = tex2D(_MainTex, IN - half2(0, destUv.y)).a;
                    half result = spriteLeft + spriteRight + spriteBottom + spriteTop;

                    // OUTBASE8DIR_ON
                    float4 spriteTopLeft = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate,
                                                                tex.GetTransformedUV(IN.uv0.xy + half2(destUv.x, destUv.
                                                                    y)));
                    float4 spriteTopRight = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate,tex.GetTransformedUV(IN.uv0.xy + half2(-destUv.x, destUv.y)));
                    float4 spriteBotLeft = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate,
                          tex.GetTransformedUV(IN.uv0.xy + half2(destUv.x, -destUv.y)));
                    float4 spriteBotRight = SAMPLE_TEXTURE2D(tex.tex, tex.samplerstate,
                                    tex.GetTransformedUV(IN.uv0.xy + half2(-destUv.x, -destUv.y)));

                    result = result + spriteTopLeft.a + spriteTopRight.a + spriteBotLeft.a + spriteBotRight.a;

                    result = step(0.05, saturate(result));

                    // OUTTEX_ON
                    if (_OutlineTexEnable > 0)
                    {
                        float _OutlineTexXSpeed = _OutlineSpeedX;
                        float _OutlineTexYSpeed = _OutlineSpeedY;
                        float2 uvOutlineTex = IN.uv0.xy;

                        uvOutlineTex.x += ((_Time.x + randomSeed) * _OutlineTexXSpeed) % 1;
                        uvOutlineTex.y += ((_Time.x + randomSeed) * _OutlineTexYSpeed) % 1;

                        //UnityTexture2D OutlineTex = UnityBuildTexture2DStruct(_OutlineTex);
                        //float4 tempOutColor = SAMPLE_TEXTURE2D(OutlineTex.tex, OutlineTex.samplerstate, OutlineTex.GetTransformedUV(uvOutlineTex) );

                        float2 OutlineTexTillAndOffset;
                        Unity_TilingAndOffset_float(uvOutlineTex, _OutlineTexTillingAndOffset.xy,
                                                          _OutlineTexTillingAndOffset.zw, OutlineTexTillAndOffset);
                        float4 tempOutColor = PLATFORM_SAMPLE_TEXTURE2D_ARRAY(
                            JiYuTexArray.tex, JiYuTexArray.samplerstate, OutlineTexTillAndOffset, _OutlineTexIndex);

                        //half4 tempOutColor = tex2D(_OutlineTex, i.uvOutTex);
                        tempOutColor *= _OutlineColor;
                        _OutlineColor = tempOutColor;
                    }

                    //

                    result *= (1 - surface.Alpha) * _OutlineAlpha;

                    //half4 outline = _OutlineColor * _Property_eb4e1ba82b1f42bd9015fe56185a8fd4_Out_0_Vector4.a;
                    half4 outline = _OutlineColor * 1;
                    outline.rgb *= _OutlineGlow;
                    outline.a = result;
                    half4 col;
                    col = outline;


                    col = lerp(float4(surface.BaseColor, surface.Alpha), outline, result);

                    surface.BaseColor = col.rgb;
                    surface.Alpha = col.a;
                }
                if (OverlayTexEnable > 0)
                {
                    float2 overlayUvs = IN.uv0.xy;
                    overlayUvs.x += ((_Time.y + randomSeed) * (_OverlayTexSpeedX / 10)) % 1;
                    overlayUvs.y += ((_Time.y + randomSeed) * (_OverlayTexSpeedY / 10)) % 1;

                    float2 arrayTillAndOffset;
                    Unity_TilingAndOffset_float(overlayUvs, _OverlayTillingAndOffset.xy, _OverlayTillingAndOffset.zw,
                                                arrayTillAndOffset);
                    float4 overlayCol = PLATFORM_SAMPLE_TEXTURE2D_ARRAY(JiYuTexArray.tex, JiYuTexArray.samplerstate,
                                                                        arrayTillAndOffset, OverlayIndex);


                    //float4 overlayCol = tex2DArray(_TextureArray, float3(overlayUvs, _Index));
                    //UnityTexture2DArray OverlayTexArray = UnityBuildTexture2DArrayStruct(_TextureArray);
                    //float2 uvOverlayTex = TRANSFORM_TEX(overlayUvs, _TextureArray);
                    //float4 overlayCol=SAMPLE_TEXTURE2D_ARRAY(OverlayTexArray.tex, OverlayTexArray.samplerstate, OverlayTexArray.GetTransformedUV(overlayUvs) );
                    //UnityTexture2D OverlayTex = UnityBuildTexture2DStruct(_OverlayTex);
                    //float2 uvOverlayTex = TRANSFORM_TEX(overlayUvs, _OverlayTex);

                    //float4 overlayCol = SAMPLE_TEXTURE2D(OverlayTex.tex, OverlayTex.samplerstate, OverlayTex.GetTransformedUV(uvOverlayTex) );


                    overlayCol.rgb *= OverlayColor.rgb * _OverlayGlow;

                    overlayCol.rgb *= overlayCol.a * OverlayColor.rgb * OverlayColor.a * _OverlayBlend;
                    surface.BaseColor += overlayCol.rgb;

                    overlayCol.a *= OverlayColor.a;
                    surface.BaseColor = lerp(surface.BaseColor, surface.BaseColor * overlayCol, _OverlayBlend);
                }
                if (ChromAberrEnable > 0)
                {
                    UnityTexture2D rt = UnityBuildTexture2DStruct(_MainTex);
                    float4 r = SAMPLE_TEXTURE2D(
                    rt.tex,rt.samplerstate,rt.GetTransformedUV(IN.uv0.xy+half2(ChromAberrAmount/10, 0)));
                    float4 b = SAMPLE_TEXTURE2D(
                    rt.tex,rt.samplerstate,rt.GetTransformedUV(IN.uv0.xy+half2(-ChromAberrAmount/10, 0)));

                    surface.BaseColor =float3(r.r * r.a, surface.BaseColor.g, b.b * b.a);
                    //surface.BaseColor =surface.BaseColor * r.rgb * b.rgb;
                    surface.Alpha =  max(max(r.a, b.a) * _ChromAberrAlpha, surface.Alpha);
                    //half4 r = tex2D(_MainTex, i.uv + half2(_ChromAberrAmount/10, 0)) * i.color;
				    //half4 b = tex2D(_MainTex, i.uv + half2(-_ChromAberrAmount/10, 0)) * i.color;
				    //col = half4(r.r * r.a, col.g, b.b * b.a, max(max(r.a, b.a) * _ChromAberrAlpha, col.a));
                    
                }
                
                float dissolveValue = dissolveSamp.r;
                clip(dissolveValue - jiYuDissolveThreshold);
                


                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;
                output.uv1 = input.uv1;
                output.uv2 = input.uv2;
                output.uv3 = input.uv3;

                return output;
            }

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

                #ifdef HAVE_VFX_MODIFICATION
                #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
                #endif
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

                #endif


                #if UNITY_UV_STARTS_AT_TOP
                #else
                #endif


                output.uv0 = input.texCoord0;
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}