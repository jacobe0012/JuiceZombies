Shader "UnicornStudio/JiYuGPUAnimationTest"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        [Normal][NoScaleOffset]_NormalMap("NormalMap", 2D) = "bump" {}
        [NoScaleOffset]_Metallic("Metallic", 2D) = "black" {}
        [ToggleUI]Boolean_4fa18106922e427e8f2e6cec7d7fbe02("Emission", Float) = 0
        [NoScaleOffset]Texture2D_6225545d970340259adeca6137569ef7("EmissionMap", 2D) = "white" {}
        Color_e3411054af87437e8a76014ef1aa41dd("EmissionColor", Color) = (1, 1, 1, 0)
        ClipThreshold("ClipThreshold", Range(0, 1)) = 0.01
        [HDR]_ClipEmissionColor("ClipEmissionColor", Color) = (0.9811321, 0, 0, 0)
        _ClipWidth("ClipWidth", Float) = 0.02
        _JiYuPivot("JiYuPivot", Range(0, 1)) = 0.5
        [NoScaleOffset]_AnimatedBoneMatrices("AnimatedBoneMatrices", 2D) = "white" {}
        _EnableAnimation("EnableAnimation", Float) = 1
        _Color("Color", Color) = (1, 1, 1, 1)
        _LightColor("LightColor", Color) = (0, 0, 0, 0)
        _ShadowColor("ShadowColor", Color) = (0, 0, 0, 0)
        _LightPosition("LightPosition", Float) = 0
        _LightStartState("LightStartState", Float) = 0
        _JiYuFlip("JiYuFlip", Vector) = (0, 0, 0, 0)
        _JiYuSort("JiYuSort", Vector) = (0, 0, 0, 0)
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque" 
            "UniversalMaterialType" = "Unlit"
            "Queue"="Geometry"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Off
        //Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
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
        

        
        Varyings UnpackVaryings (PackedVaryings input)
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
        float4 _NormalMap_TexelSize;
        float4 _Metallic_TexelSize;
        float Boolean_4fa18106922e427e8f2e6cec7d7fbe02;
        float4 Texture2D_6225545d970340259adeca6137569ef7_TexelSize;
        float4 Color_e3411054af87437e8a76014ef1aa41dd;
        float ClipThreshold;
        float4 _ClipEmissionColor;
        float _JiYuPivot;
        float _ClipWidth;
        float4 _AnimatedBoneMatrices_TexelSize;
        float4x4 _AnimationState;
        float _EnableAnimation;
        float4 _Color;
        float4 _LightColor;
        float4 _ShadowColor;
        float _LightPosition;
        float _LightStartState;
        int2 _JiYuSort;
        int2 _JiYuFlip;
        CBUFFER_END
        
        #if defined(DOTS_INSTANCING_ON)
        // DOTS instancing definitions
        UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, ClipThreshold)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, _JiYuPivot)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float4x4, _AnimationState)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, _EnableAnimation)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float4, _Color)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, _LightPosition)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(float, _LightStartState)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(int2, _JiYuSort)
            UNITY_DOTS_INSTANCED_PROP_OVERRIDE_SUPPORTED(int2, _JiYuFlip)
        UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
        // DOTS instancing usage macros
        #define UNITY_ACCESS_HYBRID_INSTANCED_PROP(var, type) UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(type, var)
        #elif defined(UNITY_INSTANCING_ENABLED)
        // Unity instancing definitions
        UNITY_INSTANCING_BUFFER_START(SGPerInstanceData)
            UNITY_DEFINE_INSTANCED_PROP(float, ClipThreshold)
            UNITY_DEFINE_INSTANCED_PROP(float, _JiYuPivot)
            UNITY_DEFINE_INSTANCED_PROP(float4x4, _AnimationState)
            UNITY_DEFINE_INSTANCED_PROP(float, _EnableAnimation)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_DEFINE_INSTANCED_PROP(float, _LightPosition)
            UNITY_DEFINE_INSTANCED_PROP(float, _LightStartState)
            UNITY_DEFINE_INSTANCED_PROP(int2, _JiYuSort)
            UNITY_DEFINE_INSTANCED_PROP(int2, _JiYuFlip)
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
        TEXTURE2D(_NormalMap);
        SAMPLER(sampler_NormalMap);
        TEXTURE2D(_Metallic);
        SAMPLER(sampler_Metallic);
        TEXTURE2D(Texture2D_6225545d970340259adeca6137569ef7);
        SAMPLER(samplerTexture2D_6225545d970340259adeca6137569ef7);
        TEXTURE2D(_AnimatedBoneMatrices);
        SAMPLER(sampler_AnimatedBoneMatrices);
		float4 _sortingGlobalData;
        // Graph Includes
        #include "Assets/ThirdPartyPackages/GPUECSAnimationBaker/Engine/Shader/GpuEcsAnimator.cginc"
        
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
        float RemapInternal(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }
        // Graph Functions
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            //output.positionCS = input.positionCS;


            float _jiYuPivot = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_JiYuPivot, float);
            int2 _jiYuSort = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_JiYuSort, int2);

            float4 mvp3 = mul(UNITY_MATRIX_MVP, float4(0,RemapInternal(_jiYuPivot, 0, 1, -1, 1),0,1));
            float2 screenClipSpacePos = mvp3.xy / mvp3.w;
            output.positionCS = input.positionCS;
            output.positionCS.z =
                 _jiYuSort.x * _sortingGlobalData.x                                                                                // layer offset
                + _jiYuSort.y * _sortingGlobalData.y                                                                              // sorting index offset
                + _sortingGlobalData.y * saturate(RemapInternal(screenClipSpacePos.y, -1, 1, 0, 1));  // screen y pos offset
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
        
        void Unity_Comparison_NotEqual_half(half A, half B, out half Out)
        {
            Out = A != B ? 1 : 0;
        }
        
        void Unity_MatrixConstruction_Row_half (half4 M0, half4 M1, half4 M2, half4 M3, out half4x4 Out4x4, out half3x3 Out3x3, out half2x2 Out2x2)
        {
        Out4x4 = half4x4(M0.x, M0.y, M0.z, M0.w, M1.x, M1.y, M1.z, M1.w, M2.x, M2.y, M2.z, M2.w, M3.x, M3.y, M3.z, M3.w);
        Out3x3 = half3x3(M0.x, M0.y, M0.z, M1.x, M1.y, M1.z, M2.x, M2.y, M2.z);
        Out2x2 = half2x2(M0.x, M0.y, M1.x, M1.y);
        }
        
        void Unity_Branch_float3(float Predicate, float3 True, float3 False, out float3 Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_Branch_half3(half Predicate, half3 True, half3 False, out half3 Out)
        {
            Out = Predicate ? True : False;
        }
        
        struct Bindings_GpuEcsAnimator_7ab285417ae253147953173b02b67332_half
        {
        float3 ObjectSpaceNormal;
        float3 ObjectSpaceTangent;
        float3 ObjectSpacePosition;
        half4 uv1;
        half4 uv2;
        half4 uv3;
        };
        
        void SG_GpuEcsAnimator_7ab285417ae253147953173b02b67332_half(half _EnableAnimation, UnityTexture2D _AnimatedBoneMatrices, half4x4 _AnimationState, Bindings_GpuEcsAnimator_7ab285417ae253147953173b02b67332_half IN, out float3 position_1, out float3 normal_2, out float3 tangent_3)
        {
        half _Property_3b80de15f2d4440d911ce87c42c058ef_Out_0_Float = _EnableAnimation;
        half _Comparison_7a05610797d949a3875c21ce458d0b4f_Out_2_Boolean;
        Unity_Comparison_NotEqual_half(_Property_3b80de15f2d4440d911ce87c42c058ef_Out_0_Float, half(0), _Comparison_7a05610797d949a3875c21ce458d0b4f_Out_2_Boolean);
        half4 _UV_1be6761a5b46434d9450a201247cecab_Out_0_Vector4 = IN.uv1;
        half4 _UV_9d11b56d03964ee7949150f0411ee35d_Out_0_Vector4 = IN.uv2;
        half4 _UV_9a1c6fa68d794e9d8e976f5e3ca926c7_Out_0_Vector4 = IN.uv3;
        half4x4 _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var4x4_4_Matrix4;
        half3x3 _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var3x3_5_Matrix3;
        half2x2 _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var2x2_6_Matrix2;
        Unity_MatrixConstruction_Row_half(_UV_1be6761a5b46434d9450a201247cecab_Out_0_Vector4, _UV_9d11b56d03964ee7949150f0411ee35d_Out_0_Vector4, _UV_9a1c6fa68d794e9d8e976f5e3ca926c7_Out_0_Vector4, half4 (0, 0, 0, 0), _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var4x4_4_Matrix4, _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var3x3_5_Matrix3, _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var2x2_6_Matrix2);
        UnityTexture2D _Property_954fd2a177f34fd2b3433d2b94426dce_Out_0_Texture2D = _AnimatedBoneMatrices;
        half4x4 _Property_1b920faf4fa848428db0791ec7c51422_Out_0_Matrix4 = _AnimationState;
        half3 _AnimateBlendCustomFunction_536a9facb4c94737affba23c19d4719f_positionOut_2_Vector3;
        half3 _AnimateBlendCustomFunction_536a9facb4c94737affba23c19d4719f_normalOut_6_Vector3;
        half3 _AnimateBlendCustomFunction_536a9facb4c94737affba23c19d4719f_tangentOut_7_Vector3;
        AnimateBlend_half(IN.ObjectSpacePosition, IN.ObjectSpaceNormal, IN.ObjectSpaceTangent, _MatrixConstruction_4d1bef8cb8dd4c1ca20668d01e596095_var4x4_4_Matrix4, _Property_954fd2a177f34fd2b3433d2b94426dce_Out_0_Texture2D.tex, _Property_1b920faf4fa848428db0791ec7c51422_Out_0_Matrix4, _AnimateBlendCustomFunction_536a9facb4c94737affba23c19d4719f_positionOut_2_Vector3, _AnimateBlendCustomFunction_536a9facb4c94737affba23c19d4719f_normalOut_6_Vector3, _AnimateBlendCustomFunction_536a9facb4c94737affba23c19d4719f_tangentOut_7_Vector3);
        float3 _Branch_2278d61e908143df8a0dc1d91ed70ee5_Out_3_Vector3;
        Unity_Branch_float3(_Comparison_7a05610797d949a3875c21ce458d0b4f_Out_2_Boolean, _AnimateBlendCustomFunction_536a9facb4c94737affba23c19d4719f_positionOut_2_Vector3, IN.ObjectSpacePosition, _Branch_2278d61e908143df8a0dc1d91ed70ee5_Out_3_Vector3);
        half3 _Branch_0793612be3df44d195d7be200190f9bd_Out_3_Vector3;
        Unity_Branch_half3(_Comparison_7a05610797d949a3875c21ce458d0b4f_Out_2_Boolean, _AnimateBlendCustomFunction_536a9facb4c94737affba23c19d4719f_normalOut_6_Vector3, IN.ObjectSpaceNormal, _Branch_0793612be3df44d195d7be200190f9bd_Out_3_Vector3);
        half3 _Branch_ea2a094034f7410482cd294b3f50abfc_Out_3_Vector3;
        Unity_Branch_half3(_Comparison_7a05610797d949a3875c21ce458d0b4f_Out_2_Boolean, _AnimateBlendCustomFunction_536a9facb4c94737affba23c19d4719f_tangentOut_7_Vector3, IN.ObjectSpaceTangent, _Branch_ea2a094034f7410482cd294b3f50abfc_Out_3_Vector3);
        position_1 = _Branch_2278d61e908143df8a0dc1d91ed70ee5_Out_3_Vector3;
        normal_2 = _Branch_0793612be3df44d195d7be200190f9bd_Out_3_Vector3;
        tangent_3 = _Branch_ea2a094034f7410482cd294b3f50abfc_Out_3_Vector3;
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
            int2 _Property_551fb6da021d4753b8e9fbe4857cbfdf_Out_0_Vector2 = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_JiYuFlip, int2);
            float _Split_cfa101a3cf324e3c8dc09dfe49232c27_R_1_Float = (float)_Property_551fb6da021d4753b8e9fbe4857cbfdf_Out_0_Vector2[0];
            float _Split_cfa101a3cf324e3c8dc09dfe49232c27_G_2_Float = (float)_Property_551fb6da021d4753b8e9fbe4857cbfdf_Out_0_Vector2[1];
            float _Split_cfa101a3cf324e3c8dc09dfe49232c27_B_3_Float = 0;
            float _Split_cfa101a3cf324e3c8dc09dfe49232c27_A_4_Float = 0;
            float _Step_eddd1bc0ab8244468236e9e9a3b19c43_Out_2_Float;
            Unity_Step_float(float(0.5), _Split_cfa101a3cf324e3c8dc09dfe49232c27_R_1_Float, _Step_eddd1bc0ab8244468236e9e9a3b19c43_Out_2_Float);
            float _Multiply_07dc1b2aa8ce41a8ba2fdf1cbd3ab7df_Out_2_Float;
            Unity_Multiply_float_float(_Step_eddd1bc0ab8244468236e9e9a3b19c43_Out_2_Float, -2, _Multiply_07dc1b2aa8ce41a8ba2fdf1cbd3ab7df_Out_2_Float);
            float _Add_9e398982ebc94ae39e1684c6dae740b8_Out_2_Float;
            Unity_Add_float(_Multiply_07dc1b2aa8ce41a8ba2fdf1cbd3ab7df_Out_2_Float, float(1), _Add_9e398982ebc94ae39e1684c6dae740b8_Out_2_Float);
            float _Property_26b74e64fcf9483d8de21b41b74429d7_Out_0_Float = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_EnableAnimation, float);
            UnityTexture2D _Property_596d299abef8414ab417ac4c6e34581d_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_AnimatedBoneMatrices);
            float4x4 _Property_c6e8e2d379a94ca1afc58ae69a691d3c_Out_0_Matrix4 = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_AnimationState, float4x4);
            Bindings_GpuEcsAnimator_7ab285417ae253147953173b02b67332_half _GpuEcsAnimator_872425d9f4694a7098630df8919fc655;
            _GpuEcsAnimator_872425d9f4694a7098630df8919fc655.ObjectSpaceNormal = IN.ObjectSpaceNormal;
            _GpuEcsAnimator_872425d9f4694a7098630df8919fc655.ObjectSpaceTangent = IN.ObjectSpaceTangent;
            _GpuEcsAnimator_872425d9f4694a7098630df8919fc655.ObjectSpacePosition = IN.ObjectSpacePosition;
            _GpuEcsAnimator_872425d9f4694a7098630df8919fc655.uv1 = IN.uv1;
            _GpuEcsAnimator_872425d9f4694a7098630df8919fc655.uv2 = IN.uv2;
            _GpuEcsAnimator_872425d9f4694a7098630df8919fc655.uv3 = IN.uv3;
            float3 _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_position_1_Vector3;
            float3 _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_normal_2_Vector3;
            float3 _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_tangent_3_Vector3;
            SG_GpuEcsAnimator_7ab285417ae253147953173b02b67332_half(_Property_26b74e64fcf9483d8de21b41b74429d7_Out_0_Float, _Property_596d299abef8414ab417ac4c6e34581d_Out_0_Texture2D, _Property_c6e8e2d379a94ca1afc58ae69a691d3c_Out_0_Matrix4, _GpuEcsAnimator_872425d9f4694a7098630df8919fc655, _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_position_1_Vector3, _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_normal_2_Vector3, _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_tangent_3_Vector3);
            half _Split_25a39bfe4cb04ded8715c1365a37d951_R_1_Float = _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_position_1_Vector3[0];
            half _Split_25a39bfe4cb04ded8715c1365a37d951_G_2_Float = _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_position_1_Vector3[1];
            half _Split_25a39bfe4cb04ded8715c1365a37d951_B_3_Float = _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_position_1_Vector3[2];
            half _Split_25a39bfe4cb04ded8715c1365a37d951_A_4_Float = 0;
            float _Multiply_938e5f2c76134d9a816d4f5a69e4359f_Out_2_Float;
            Unity_Multiply_float_float(_Add_9e398982ebc94ae39e1684c6dae740b8_Out_2_Float, _Split_25a39bfe4cb04ded8715c1365a37d951_R_1_Float, _Multiply_938e5f2c76134d9a816d4f5a69e4359f_Out_2_Float);
            float _Step_a1d209e0ead84e8d84fef5f9ee3cd6df_Out_2_Float;
            Unity_Step_float(float(0.5), _Split_cfa101a3cf324e3c8dc09dfe49232c27_G_2_Float, _Step_a1d209e0ead84e8d84fef5f9ee3cd6df_Out_2_Float);
            float _Multiply_15cc14504d064f3fbd72c140e3e644d1_Out_2_Float;
            Unity_Multiply_float_float(_Step_a1d209e0ead84e8d84fef5f9ee3cd6df_Out_2_Float, -2, _Multiply_15cc14504d064f3fbd72c140e3e644d1_Out_2_Float);
            float _Add_d972882d354c44e9b9b7449cb467af75_Out_2_Float;
            Unity_Add_float(_Multiply_15cc14504d064f3fbd72c140e3e644d1_Out_2_Float, float(1), _Add_d972882d354c44e9b9b7449cb467af75_Out_2_Float);
            float _Multiply_cf1c687d46fe42b3b9727313ccc0f119_Out_2_Float;
            Unity_Multiply_float_float(_Add_d972882d354c44e9b9b7449cb467af75_Out_2_Float, _Split_25a39bfe4cb04ded8715c1365a37d951_G_2_Float, _Multiply_cf1c687d46fe42b3b9727313ccc0f119_Out_2_Float);
            half _Split_fd49aeed15584d61934e964d16c03262_R_1_Float = _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_position_1_Vector3[0];
            half _Split_fd49aeed15584d61934e964d16c03262_G_2_Float = _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_position_1_Vector3[1];
            half _Split_fd49aeed15584d61934e964d16c03262_B_3_Float = _GpuEcsAnimator_872425d9f4694a7098630df8919fc655_position_1_Vector3[2];
            half _Split_fd49aeed15584d61934e964d16c03262_A_4_Float = 0;
            float4 _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RGBA_4_Vector4;
            float3 _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RGB_5_Vector3;
            float2 _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RG_6_Vector2;
            Unity_Combine_float(_Multiply_938e5f2c76134d9a816d4f5a69e4359f_Out_2_Float, _Multiply_cf1c687d46fe42b3b9727313ccc0f119_Out_2_Float, _Split_fd49aeed15584d61934e964d16c03262_B_3_Float, float(0), _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RGBA_4_Vector4, _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RGB_5_Vector3, _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RG_6_Vector2);
            description.Position = _Combine_fda62c3bfbb147b3bdd10c64fda6fe45_RGB_5_Vector3;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
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
            float _Float_86c84528c6c148418e04770d3b1d3dee_Out_0_Float = float(0.96);
            float4 _Property_0de32fdc388f4ac9af4ced5661009c60_Out_0_Vector4 = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_Color, float4);
            UnityTexture2D _Property_b39d57107c819b888a3cd145b01b0cd1_Out_0_Texture2D = UnityBuildTexture2DStruct(_MainTex);
            float4 _SampleTexture2D_fc1f87869368e881acd54e038ee273f3_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_b39d57107c819b888a3cd145b01b0cd1_Out_0_Texture2D.tex, _Property_b39d57107c819b888a3cd145b01b0cd1_Out_0_Texture2D.samplerstate, _Property_b39d57107c819b888a3cd145b01b0cd1_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_fc1f87869368e881acd54e038ee273f3_R_4_Float = _SampleTexture2D_fc1f87869368e881acd54e038ee273f3_RGBA_0_Vector4.r;
            float _SampleTexture2D_fc1f87869368e881acd54e038ee273f3_G_5_Float = _SampleTexture2D_fc1f87869368e881acd54e038ee273f3_RGBA_0_Vector4.g;
            float _SampleTexture2D_fc1f87869368e881acd54e038ee273f3_B_6_Float = _SampleTexture2D_fc1f87869368e881acd54e038ee273f3_RGBA_0_Vector4.b;
            float _SampleTexture2D_fc1f87869368e881acd54e038ee273f3_A_7_Float = _SampleTexture2D_fc1f87869368e881acd54e038ee273f3_RGBA_0_Vector4.a;
            float4 _Multiply_2f27e03e5a394fc8ac642922c2929feb_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_0de32fdc388f4ac9af4ced5661009c60_Out_0_Vector4, _SampleTexture2D_fc1f87869368e881acd54e038ee273f3_RGBA_0_Vector4, _Multiply_2f27e03e5a394fc8ac642922c2929feb_Out_2_Vector4);
            float4 _Multiply_56fe66f801bb4d01a6a650f6afbd51d6_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Float_86c84528c6c148418e04770d3b1d3dee_Out_0_Float.xxxx), _Multiply_2f27e03e5a394fc8ac642922c2929feb_Out_2_Vector4, _Multiply_56fe66f801bb4d01a6a650f6afbd51d6_Out_2_Vector4);
            float4 _Property_a2bc9d24c8d14325a25ca524af50918a_Out_0_Vector4 = _LightColor;
            float _Float_d349555000994f7ba50fddd9487905b2_Out_0_Float = float(0.04);
            float4 _Multiply_64497a5a65c140a39d57469e1d6fa969_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_a2bc9d24c8d14325a25ca524af50918a_Out_0_Vector4, (_Float_d349555000994f7ba50fddd9487905b2_Out_0_Float.xxxx), _Multiply_64497a5a65c140a39d57469e1d6fa969_Out_2_Vector4);
            float4 _Add_b098ead9cf2a4546b33b33480a155f1e_Out_2_Vector4;
            Unity_Add_float4(_Multiply_56fe66f801bb4d01a6a650f6afbd51d6_Out_2_Vector4, _Multiply_64497a5a65c140a39d57469e1d6fa969_Out_2_Vector4, _Add_b098ead9cf2a4546b33b33480a155f1e_Out_2_Vector4);
            float _Property_5e75926366fe4990abfa745da3b3c808_Out_0_Float = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_LightPosition, float);
            float _Multiply_915a6d31b7fe445d8759569cfee48d63_Out_2_Float;
            Unity_Multiply_float_float(_Property_5e75926366fe4990abfa745da3b3c808_Out_0_Float, -1, _Multiply_915a6d31b7fe445d8759569cfee48d63_Out_2_Float);
            float _Step_1f63133ca6254b538e742921afd88d15_Out_2_Float;
            Unity_Step_float(float(-1), _Multiply_915a6d31b7fe445d8759569cfee48d63_Out_2_Float, _Step_1f63133ca6254b538e742921afd88d15_Out_2_Float);
            float4 _Multiply_ab3c6cfb63424490a2ae6f29dbedcbc9_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Add_b098ead9cf2a4546b33b33480a155f1e_Out_2_Vector4, (_Step_1f63133ca6254b538e742921afd88d15_Out_2_Float.xxxx), _Multiply_ab3c6cfb63424490a2ae6f29dbedcbc9_Out_2_Vector4);
            float4 _Property_8ffc845dcfd74865868616e17899fa49_Out_0_Vector4 = _ShadowColor;
            float _Float_d770b7cff2764b549da5a83d92105e1a_Out_0_Float = float(0.96);
            float4 _Multiply_e9d6b2f8a95e40f8b2bded3727a50a69_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_8ffc845dcfd74865868616e17899fa49_Out_0_Vector4, (_Float_d770b7cff2764b549da5a83d92105e1a_Out_0_Float.xxxx), _Multiply_e9d6b2f8a95e40f8b2bded3727a50a69_Out_2_Vector4);
            float4 _Add_d138ab2d4f7c48d98e257846df6c478c_Out_2_Vector4;
            Unity_Add_float4(_Multiply_e9d6b2f8a95e40f8b2bded3727a50a69_Out_2_Vector4, _Multiply_64497a5a65c140a39d57469e1d6fa969_Out_2_Vector4, _Add_d138ab2d4f7c48d98e257846df6c478c_Out_2_Vector4);
            float4 _Multiply_1b5121a4b51645228082e53d30619de3_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_2f27e03e5a394fc8ac642922c2929feb_Out_2_Vector4, _Add_d138ab2d4f7c48d98e257846df6c478c_Out_2_Vector4, _Multiply_1b5121a4b51645228082e53d30619de3_Out_2_Vector4);
            float _Step_9c2c14541e994b61b06a7c27088e6ca3_Out_2_Float;
            Unity_Step_float(float(1), _Property_5e75926366fe4990abfa745da3b3c808_Out_0_Float, _Step_9c2c14541e994b61b06a7c27088e6ca3_Out_2_Float);
            float4 _Multiply_9b89636440da46eda21bef2390f83c31_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_1b5121a4b51645228082e53d30619de3_Out_2_Vector4, (_Step_9c2c14541e994b61b06a7c27088e6ca3_Out_2_Float.xxxx), _Multiply_9b89636440da46eda21bef2390f83c31_Out_2_Vector4);
            float4 _Add_d81b056ea89a491796244a96eec6ff3e_Out_2_Vector4;
            Unity_Add_float4(_Multiply_ab3c6cfb63424490a2ae6f29dbedcbc9_Out_2_Vector4, _Multiply_9b89636440da46eda21bef2390f83c31_Out_2_Vector4, _Add_d81b056ea89a491796244a96eec6ff3e_Out_2_Vector4);
            float _Property_dc4d94d0c09f4569882896ec5e731019_Out_0_Float = UNITY_ACCESS_HYBRID_INSTANCED_PROP(_LightStartState, float);
            float _Step_b7403629e39d4ce5a0ec2743b4fcb30b_Out_2_Float;
            Unity_Step_float(float(1), _Property_dc4d94d0c09f4569882896ec5e731019_Out_0_Float, _Step_b7403629e39d4ce5a0ec2743b4fcb30b_Out_2_Float);
            float4 _Multiply_78fae37d9e854198a71fcbddbe5f2803_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Add_d81b056ea89a491796244a96eec6ff3e_Out_2_Vector4, (_Step_b7403629e39d4ce5a0ec2743b4fcb30b_Out_2_Float.xxxx), _Multiply_78fae37d9e854198a71fcbddbe5f2803_Out_2_Vector4);
            float _Multiply_850d66dcb3a042388d5623c8e25b3266_Out_2_Float;
            Unity_Multiply_float_float(_Property_dc4d94d0c09f4569882896ec5e731019_Out_0_Float, -1, _Multiply_850d66dcb3a042388d5623c8e25b3266_Out_2_Float);
            float _Step_04d26d2c90674c66a2aca35718ab67ae_Out_2_Float;
            Unity_Step_float(float(-1), _Multiply_850d66dcb3a042388d5623c8e25b3266_Out_2_Float, _Step_04d26d2c90674c66a2aca35718ab67ae_Out_2_Float);
            float4 _Multiply_2f24646de53a4693a0a8fa02db5ed018_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_56fe66f801bb4d01a6a650f6afbd51d6_Out_2_Vector4, (_Step_04d26d2c90674c66a2aca35718ab67ae_Out_2_Float.xxxx), _Multiply_2f24646de53a4693a0a8fa02db5ed018_Out_2_Vector4);
            float4 _Add_30d45449dc9149459e9a5a595b36935c_Out_2_Vector4;
            Unity_Add_float4(_Multiply_78fae37d9e854198a71fcbddbe5f2803_Out_2_Vector4, _Multiply_2f24646de53a4693a0a8fa02db5ed018_Out_2_Vector4, _Add_30d45449dc9149459e9a5a595b36935c_Out_2_Vector4);
            float _Split_0b89e4c8c4fc429381e79c48e004407d_R_1_Float = _Multiply_2f27e03e5a394fc8ac642922c2929feb_Out_2_Vector4[0];
            float _Split_0b89e4c8c4fc429381e79c48e004407d_G_2_Float = _Multiply_2f27e03e5a394fc8ac642922c2929feb_Out_2_Vector4[1];
            float _Split_0b89e4c8c4fc429381e79c48e004407d_B_3_Float = _Multiply_2f27e03e5a394fc8ac642922c2929feb_Out_2_Vector4[2];
            float _Split_0b89e4c8c4fc429381e79c48e004407d_A_4_Float = _Multiply_2f27e03e5a394fc8ac642922c2929feb_Out_2_Vector4[3];
            float _Property_3ae0775973644a4a9c4faad94eb3c2c3_Out_0_Float = UNITY_ACCESS_HYBRID_INSTANCED_PROP(ClipThreshold, float);
            surface.BaseColor = (_Add_30d45449dc9149459e9a5a595b36935c_Out_2_Vector4.xyz);
            surface.Alpha = _Split_0b89e4c8c4fc429381e79c48e004407d_A_4_Float;
            surface.AlphaClipThreshold = _Property_3ae0775973644a4a9c4faad94eb3c2c3_Out_0_Float;
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
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
            output.uv1 =                                        input.uv1;
            output.uv2 =                                        input.uv2;
            output.uv3 =                                        input.uv3;
        
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