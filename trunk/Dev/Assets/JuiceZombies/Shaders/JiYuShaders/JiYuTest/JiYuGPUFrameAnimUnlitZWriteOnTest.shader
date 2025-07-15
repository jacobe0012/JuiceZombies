
Shader "JiYuStudio/JiYuGPUFrameAnimUnlitZWriteOnTest"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _TimeScale("TimeScale", Float) = 10.0
		_MainTex_ST("MainTex_ST", Vector) = (1, 1, 0, 0)
		[HDR]_Color("Color", Color) = (1, 1, 1, 1)
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		_JiYuStartTime("JiYu StartTime", Float) = 0
		_JiYuFrameAnimSpeed("JiYuFrameAnimSpeed(改为播放一次所需时间/s)", Float) = 1
		_FrameRow("FrameRow", Int) = 4
		_FrameCol("FrameCol", Int) = 4
		_JiYuFlip("JiYu Flip",Vector) = (0, 0, 0, 0)
		_JiYuSort("JiYu Sort",Vector) = (0, 0, 0, 0)
		_JiYuPivot("JiYu Pivot",Range(0, 1)) = 0.5
		_JiYuAnimIndex("JiYu AnimIndex",Int) = 0
		_JiYuFrameAnimLoop("JiYu FrameAnimLoop",Int) = 0
		_Offset ("Offset", Vector) = (0, 0, 0, 0)
		//_Scale ("Scale", Vector) = (1, 1, 0, 0)
		//_BlendMode1("Blend Mode1",Int) = 3
		//_BlendMode2("Blend Mode2",Int) = 10
		//_BlendMode3("Blend Mode3",Int) = 1
		//_BlendMode4("Blend Mode4",Int) = 10
	}

	SubShader
	{
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		
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
			
			struct VertexInput
			{
				float4 positionOS : POSITION;
				float4 uv0 : TEXCOORD0;

				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float2 texCoord0 : TEXCOORD0;
				
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
			};

			CBUFFER_START( UnityPerMaterial )
			int _FrameCol;
			int _FrameRow;
			float _JiYuFrameAnimSpeed;
			float _TimeScale;
			float _JiYuStartTime;
			float4 _Color;
			float4 _MainTex_ST;
			float4 _Scale;
			int2 _JiYuFlip;
			int2 _JiYuSort;
			float _JiYuPivot;
			int _JiYuAnimIndex;
			int _JiYuFrameAnimLoop;
			float4 _Offset;
			CBUFFER_END

			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
					UNITY_DOTS_INSTANCED_PROP(float4, _Color)
					UNITY_DOTS_INSTANCED_PROP(float4, _MainTex_ST)
					UNITY_DOTS_INSTANCED_PROP(int2, _JiYuFlip)
					UNITY_DOTS_INSTANCED_PROP(int2, _JiYuSort)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuPivot)
					UNITY_DOTS_INSTANCED_PROP(int, _JiYuAnimIndex)
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuFrameAnimSpeed)
					UNITY_DOTS_INSTANCED_PROP(int, _JiYuFrameAnimLoop)	
					UNITY_DOTS_INSTANCED_PROP(float, _JiYuStartTime)	
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

				#define _Color UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _Color)
				#define _MainTex_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _MainTex_ST)
                #define _JiYuFlip UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int2, _JiYuFlip)
                #define _JiYuSort UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int2, _JiYuSort)
                #define _JiYuPivot UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuPivot)
                #define _JiYuAnimIndex UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int, _JiYuAnimIndex)
                #define _JiYuFrameAnimSpeed UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuFrameAnimSpeed)
                #define _JiYuFrameAnimLoop UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(int, _JiYuFrameAnimLoop)
                #define _JiYuStartTime UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float, _JiYuStartTime)
			#endif


			sampler2D _MainTex;
			float4 _sortingGlobalData;
            float RemapInternal(float value, float fromMin, float fromMax, float toMin, float toMax)
            {
                return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
            }

			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				 

				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );


				float3 defaultVertexValue = float3( _Offset.xyz);

				float3 vertexValue = defaultVertexValue;
				vertexValue.z = 0;
				v.positionOS.xyz += vertexValue;


				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.positionOS.xyz );

				o.positionCS = vertexInput.positionCS;
				
                o.positionCS.z = JiYuSortFunc(_JiYuPivot,_JiYuSort,_sortingGlobalData,UNITY_MATRIX_MVP);

				int offsetIndex = clamp(_JiYuAnimIndex, 1,_FrameRow * _FrameCol);

				int rowIndex = (offsetIndex-1) / _FrameCol; 
				int colIndex = (offsetIndex-1) % _FrameCol;

				float2 offset =_JiYuAnimIndex>0? float2(_MainTex_ST.z+colIndex,_MainTex_ST.w -rowIndex):0;

				o.texCoord0 = v.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw +offset;

                //o.texCoord0 = (o.texCoord0 - 0.5) * _Scale.x + 0.5; // 以中心为轴点进行缩放

                o.texCoord0.x = _JiYuFlip.x >= 1 ? (1.0 - o.texCoord0.x): o.texCoord0.x;
                o.texCoord0.y = _JiYuFlip.y >= 1 ? (1.0 - o.texCoord0.y): o.texCoord0.y;

				#if UNITY_ANY_INSTANCING_ENABLED
				o.instanceID = v.instanceID;
				#endif

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				//float mulTime13_g12 = (_TimeParameters.x-_JiYuStartTime) * _TimeScale;


				//_TimeParameters.x - _JiYuStartTime
				// *** BEGIN Flipbook UV Animation vars ***
				// Total tiles of Flipbook Texture
				float fbtotaltiles2_g12 = (float)_FrameCol * (float)_FrameRow;
				//float offsetTime =_Time.y - _JiYuStartTime;

				// 设置误差范围
				//float errorRange = 0.05;

				// 如果偏差在误差范围内，则映射为目标值
				//offsetTime = offsetTime * step(offsetTime, errorRange);
				//offsetTime = RemapInternal(offsetTime, offsetTime, duration, 0, duration);
				float mulTime13_g12 = _JiYuStartTime * _TimeScale;
				if (_JiYuFrameAnimLoop>=2 && _JiYuAnimIndex==0 ) {

					mulTime13_g12 = (_TimeParameters.x) * _TimeScale;
				}

				float tempscale=10.7;
				float animSpeed=((float)_FrameCol*(float)_FrameRow) /(_JiYuFrameAnimSpeed*tempscale);
				// Offsets for cols and rows of Flipbook Texture
				float fbcolsoffset2_g12 = 1.0f / (float)_FrameCol;
				float fbrowsoffset2_g12 = 1.0f / (float)_FrameRow;
				// Speed of animation
				float speed=_JiYuAnimIndex>0 ? 0 : animSpeed;
				float fbspeed2_g12 = (mulTime13_g12) * speed;
				// UV Tiling (col and row offset)
				float2 fbtiling2_g12 = float2(fbcolsoffset2_g12, fbrowsoffset2_g12);
				// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
				// Calculate current tile linear index

				float fbcurrenttileindex2_g121 = round(fmod(fbspeed2_g12, fbtotaltiles2_g12));
				float fbcurrenttileindex2_g12 = round(fmod(fbspeed2_g12, fbtotaltiles2_g12));


				fbcurrenttileindex2_g12 += ( fbcurrenttileindex2_g12 < 0) ? fbtotaltiles2_g12 : 0;
				// Obtain Offset X coordinate from current tile linear index
				float fblinearindextox2_g12 = round (fmod ( fbcurrenttileindex2_g12, (float)_FrameCol ) );
				// Multiply Offset X by coloffset
				float fboffsetx2_g12 = fblinearindextox2_g12 * fbcolsoffset2_g12;
				// Obtain Offset Y coordinate from current tile linear index
				float fblinearindextoy2_g12 = round(fmod( ( fbcurrenttileindex2_g12 - fblinearindextox2_g12 ) / (float)_FrameCol, (float)_FrameRow) );
				// Reverse Y to get tiles from Top to Bottom
				fblinearindextoy2_g12 = (int)((float)_FrameRow-1) - fblinearindextoy2_g12;
				// Multiply Offset Y by rowoffset
				float fboffsety2_g12 = fblinearindextoy2_g12 * fbrowsoffset2_g12;
				// UV Offset
				float2 fboffset2_g12 = float2(fboffsetx2_g12, fboffsety2_g12);
				// Flipbook UV
				half2 fbuv2_g12 = IN.texCoord0.xy * fbtiling2_g12 + fboffset2_g12;
				// *** END Flipbook UV Animation vars ***
				float4 color = tex2D( _MainTex, fbuv2_g12 )* _Color;

				if (_JiYuFrameAnimLoop<=0 && _JiYuAnimIndex<=0 ) {
					
					//float duration =(float)_FrameCol * (float)_FrameRow * (1.0 /(_TimeScale *  animSpeed ));
					//_JiYuCurTime =_TimeParameters.x - _JiYuStartTime;
					//duration = duration - _JiYuCurTime;
					color.w = _JiYuStartTime >= _JiYuFrameAnimSpeed ? 0.0 : color.w;
				}
                //clip(color.w - 0.5);

				return color;
			}

			ENDHLSL

		}
		

	}

	
}