// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "JiYuStudio/JiYuUITransTest"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_RotateDegrees("RotateDegrees", Float) = 0
		_TransWidth("TransWidth", Range( 0 , 1)) = 0.9173574
		_Tilling("Tilling", Vector) = (16,9,0,0)
		_FXColor("FXColor", Color) = (1,1,1,1)
		_ClipWidth("ClipWidth", Range( -2 , 2)) = 0
		_ClipDir("ClipDir", Vector) = (1,1,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		[HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
		ENDHLSL

		
		Pass
		{
			Name "Sprite Unlit"
			Tags { "LightMode"="Universal2D" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM


			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			

			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _MainTex_ST;
			float4 _FXColor;
			float2 _ClipDir;
			float2 _Tilling;
			float _ClipWidth;
			float _TransWidth;
			float _RotateDegrees;
			CBUFFER_END


			struct VertexInput
			{
				float3 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_SKINNED_VERTEX_INPUTS
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};



			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;


				v.positionOS = UnityFlipSprite( v.positionOS, unity_SpriteProps.xy );

				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.positionOS );

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.positionCS = vertexInput.positionCS;
				o.positionWS = vertexInput.positionWS;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				half4 Color=0;	
				float4 positionCS = IN.positionCS;
				float3 positionWS = IN.positionWS;

				float2 texCoord49 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float dotResult47 = dot( texCoord49 , _ClipDir );
				float4 color55 =  float4(0,0,0,0);
				float2 uv_MainTex = IN.texCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode5 = tex2D( _MainTex, uv_MainTex );
				float4 appendResult44 = (float4(_FXColor.r , _FXColor.g , _FXColor.b , tex2DNode5.a));
				float2 texCoord19 = IN.texCoord0.xy * _Tilling + float2( 0,0 );
				float2 texCoord15 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float cos23 = cos( _RotateDegrees );
				float sin23 = sin( _RotateDegrees );
				float2 rotator23 = mul( texCoord15 - float2( 0.5,0.5 ) , float2x2( cos23 , -sin23 , sin23 , cos23 )) + float2( 0.5,0.5 );
				float4 lerpResult8 = lerp( tex2DNode5 , appendResult44 , step( (-0.1 + (_TransWidth - 0.0) * (2.0 - -0.1) / (1.0 - 0.0)) , ( distance( frac( texCoord19 ) , float2( 0.5,0.5 ) ) + rotator23.x ) ));
				float4 ifLocalVar52 = 0;
				if( dotResult47 >= _ClipWidth )
				ifLocalVar52 = color55;
				else
				ifLocalVar52 = lerpResult8;
				
				Color *= IN.color * unity_SpriteColor;
				return Color;
			}

			ENDHLSL
		}
	
		
	}

	
}
