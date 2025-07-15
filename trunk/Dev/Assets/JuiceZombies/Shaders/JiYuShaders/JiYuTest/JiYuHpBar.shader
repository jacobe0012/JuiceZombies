Shader "JiYuStudio/JiYuHpBar"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		_MainTex_ST("MainTex_ST", Vector) = (1, 1, 0, 0)
        _JiYuHealth ("JiYu Health", Range(0, 1)) = 1
        _BorderSize ("BorderSize", Range(0, 1)) = 0.25
		_JiYuSort("JiYu Sort",Vector) = (0, 0, 0, 0)
		_JiYuPivot("JiYu Pivot",Range(0, 1)) = 0.5
    }
    SubShader
    {
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		Pass
		{
			Cull Back
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
            float _JiYuHealth;
            float _BorderSize;
            int2 _JiYuSort;
			float _JiYuPivot;
            CBUFFER_END

            #if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
					UNITY_DOTS_INSTANCED_PROP(float4, _Color)
					UNITY_DOTS_INSTANCED_PROP(float4, _MainTex_ST)
                    UNITY_DOTS_INSTANCED_PROP(float, _JiYuHealth)
                    UNITY_DOTS_INSTANCED_PROP(float, _BorderSize)
					UNITY_DOTS_INSTANCED_PROP(int2, _JiYuSort)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuPivot)
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

				#define _Color UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _Color)
				#define _MainTex_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _MainTex_ST)
                #define _JiYuHealth UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuHealth)   
                #define _BorderSize UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _BorderSize)
				#define _JiYuSort UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int2, _JiYuSort)
				#define _JiYuPivot UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuPivot)      
            #endif

            sampler2D _MainTex;
            float4 _sortingGlobalData;
            float RemapInternal(float value, float fromMin, float fromMax, float toMin, float toMax)
            {
                return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
            }
            v2f vert(appdata v)
            {
                v2f output = (v2f)0;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, output);

                float3 positionWS = TransformObjectToWorld(v.positionOS);
                output.positionCS = TransformWorldToHClip(positionWS);
                

                output.positionCS.z = JiYuSortFunc(_JiYuPivot,_JiYuSort,_sortingGlobalData,UNITY_MATRIX_MVP);


                output.uv0 = v.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                #if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = v.instanceID;
                #endif

                return output;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                // UNITY_SETUP_INSTANCE_ID(i);
                // float4 color = tex2D(_MainTex,i.uv0.xy) * _Color;
                // return color;

                //round corner clipping
                float2 coords = i.uv0;
                coords.x *= 8; //Uniform坐标
                float2 pointOnLineSeg = float2(clamp(coords.x, 0.5, 7.5), 0.5); //取的线段
                float sdf = distance(coords, pointOnLineSeg) * 2 - 1; //计算线段的sdf，并减去径向值1获取内部形状
                clip(-sdf); //裁掉外边的像素

                //border
                float borderSdf = sdf + _BorderSize; //以一个新的径向距离计算一个新的sdf
                float pd = fwidth(borderSdf);
                float borderMask = saturate(-borderSdf / pd); //计算边框遮罩
                //float borderMask = step(0, -borderSdf); //计算边框遮罩

                // sample the texture
                float3 healthbarColor = tex2D(_MainTex, float2(_JiYuHealth, i.uv0.y)); //用自己定义的坐标采样血条颜色贴图

                float healthbarMask = _JiYuHealth > i.uv0.x; //血条遮罩
                if (_JiYuHealth < 0.2) // 低血量闪烁效果
                {
                    float flash = cos(_Time.y * 4) * 0.4 + 1;
                    healthbarColor *= flash;
                }

                return float4(healthbarColor * healthbarMask * borderMask, 1);
            }
            ENDHLSL
        }
    }
}