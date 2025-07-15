// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "JiYuStudioUI/JiYuUISweep"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_TransWidth("TransWidth", Float) = 0.17
		_Tilling("Tilling", Vector) = (25,25,0,0)
		[HDR]_FXColor("FXColor", Color) = (1.26732,1.716981,0.0566928,1)
		_ClipDir("ClipDir", Vector) = (0,1,0,0)
		_PosOffset("PosOffset", Float) = 0
		_GlowWidth("GlowWidth", Float) = -1.33
		_Vector1("Vector 1", Vector) = (1,1,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _FXColor;
			uniform float _TransWidth;
			uniform float2 _Tilling;
			uniform float2 _ClipDir;
			uniform float _GlowWidth;
			uniform float2 _Vector1;
			uniform float _PosOffset;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode11 = tex2D( _MainTex, uv_MainTex );
				float4 appendResult18 = (float4(_FXColor.r , _FXColor.g , _FXColor.b , tex2DNode11.a));
				float2 texCoord2 = i.ase_texcoord1.xy * _Tilling + float2( 0,0 );
				float2 texCoord5 = i.ase_texcoord1.xy * ( float2( 1,1 ) * _GlowWidth ) + ( _Vector1 * _PosOffset );
				float dotResult32 = dot( _ClipDir , texCoord5 );
				float temp_output_13_0 = ( distance( frac( texCoord2 ) , float2( 0.5,0.5 ) ) + dotResult32 );
				float temp_output_50_0 = ( 1.0 - temp_output_13_0 );
				float4 lerpResult21 = lerp( tex2DNode11 , appendResult18 , step( _TransWidth , ( temp_output_13_0 * temp_output_50_0 ) ));
				
				
				finalColor = lerpResult21;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19302
Node;AmplifyShaderEditor.Vector2Node;49;-2200.411,172.9182;Inherit;False;Constant;_Vector3;Vector 3;6;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;58;-2246.969,350.1508;Inherit;False;Property;_GlowWidth;GlowWidth;7;0;Create;True;0;0;0;False;0;False;-1.33;-1.33;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-2299.537,674.7227;Inherit;False;Property;_PosOffset;PosOffset;6;0;Create;True;0;0;0;False;0;False;0;1.79064;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;48;-2340.245,518.8208;Inherit;False;Property;_Vector1;Vector 1;8;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;1;-2159.01,-114.8336;Inherit;False;Property;_Tilling;Tilling;3;0;Create;True;0;0;0;False;0;False;25,25;25,25;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1920.054,-112.0395;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-2004.537,575.7227;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-1824.969,299.1508;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;7;-1667.01,-95.83359;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;6;-1740.816,171.624;Inherit;False;Constant;_Vector2;Vector 2;4;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;20;-1512.838,-171.3917;Inherit;False;Property;_ClipDir;ClipDir;5;0;Create;True;0;0;0;False;0;False;0,1;-1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1442.01,372.5388;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceOpNode;9;-1526.539,2.347402;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;32;-1277.47,177.5872;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-1133.651,68.32365;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-95.26709,-339.9892;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;-1;519dafacd208bb64c83d5124be03b09d;36168894f43df5447b794a65464669c8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;50;-945.4111,330.9182;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-585.0471,176.5505;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;15;291.4312,-437.4263;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ColorNode;16;42.502,-664.8461;Inherit;False;Property;_FXColor;FXColor;4;1;[HDR];Create;True;0;0;0;False;0;False;1.26732,1.716981,0.0566928,1;0.6313726,0.8666667,0.003921569,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;12;-876.4589,-429.1478;Inherit;False;Property;_TransWidth;TransWidth;2;0;Create;True;0;0;0;False;0;False;0.17;0.17;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;17;-98.45007,-14.46101;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;18;531.4468,-589.8644;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;412.4577,-1224.426;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;21;808.2065,-196.715;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2786.734,1177.724;Inherit;False;Property;_RotateDegrees;RotateDegrees;0;0;Create;True;0;0;0;False;0;False;-4.18;2.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;3;-2802.363,898.8944;Inherit;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;53;-767.1755,376.1191;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;14;-729.7737,-276.2819;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1904.625,-379.1844;Float;False;True;-1;2;ASEMaterialInspector;100;5;JiYuStudioUI/JiYuUISweep;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;2;0;1;0
WireConnection;55;0;48;0
WireConnection;55;1;56;0
WireConnection;57;0;49;0
WireConnection;57;1;58;0
WireConnection;7;0;2;0
WireConnection;5;0;57;0
WireConnection;5;1;55;0
WireConnection;9;0;7;0
WireConnection;9;1;6;0
WireConnection;32;0;20;0
WireConnection;32;1;5;0
WireConnection;13;0;9;0
WireConnection;13;1;32;0
WireConnection;50;0;13;0
WireConnection;43;0;13;0
WireConnection;43;1;50;0
WireConnection;15;0;11;0
WireConnection;17;0;12;0
WireConnection;17;1;43;0
WireConnection;18;0;16;1
WireConnection;18;1;16;2
WireConnection;18;2;16;3
WireConnection;18;3;15;3
WireConnection;21;0;11;0
WireConnection;21;1;18;0
WireConnection;21;2;17;0
WireConnection;53;0;50;0
WireConnection;14;0;12;0
WireConnection;0;0;21;0
ASEEND*/
//CHKSM=F8502FF7F75101DF554B7F296FC357675903C563