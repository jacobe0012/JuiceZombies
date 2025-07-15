// Made with Amplify Shader Editor v1.9.3.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "JiYuStudioUI/JiYuUIFlow"
{
	Properties
	{
		_MainTexture("MainTexture", 2D) = "white" {}
		_AddTexture("AddTexture", 2D) = "black" {}
		_NoiseSpeed("NoiseSpeed", Vector) = (0,1.53,0,0)
		_NoiseTexture("NoiseTexture", 2D) = "white" {}
		_NoiseIntensity("NoiseIntensity", Float) = 0.1
		[HDR]_AddColor("AddColor", Color) = (1,1,1,1)
		_GlowSpeed("GlowSpeed", Range( -3 , 3)) = 0.5
		_GlowHeight("GlowHeight", Range( 1 , 2)) = 1.5
		_GlowInterval("GlowInterval", Range( 1 , 10)) = 2
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
			#include "UnityShaderVariables.cginc"


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
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform sampler2D _MainTexture;
			uniform float4 _MainTexture_ST;
			uniform sampler2D _AddTexture;
			uniform float _GlowSpeed;
			uniform float _GlowInterval;
			uniform float _GlowHeight;
			uniform sampler2D _NoiseTexture;
			uniform float2 _NoiseSpeed;
			uniform float _NoiseIntensity;
			uniform float4 _AddColor;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_color = v.color;
				
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
				float2 uv_MainTexture = i.ase_texcoord1.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
				float4 tex2DNode37 = tex2D( _MainTexture, uv_MainTexture );
				float2 texCoord40 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime65 = _Time.y * _GlowSpeed;
				float fmodResult66 = frac(mulTime65/_GlowInterval)*_GlowInterval;
				float4 appendResult74 = (float4(0.0 , ( ( saturate( fmodResult66 ) * 2.0 * _GlowHeight ) - _GlowHeight ) , 0.0 , 0.0));
				float2 texCoord48 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner49 = ( 1.0 * _Time.y * _NoiseSpeed + texCoord48);
				float4 temp_cast_1 = (tex2D( _NoiseTexture, panner49 ).r).xxxx;
				float4 lerpResult44 = lerp( ( float4( texCoord40, 0.0 , 0.0 ) + appendResult74 ) , temp_cast_1 , _NoiseIntensity);
				float4 break60 = ( tex2DNode37 + ( tex2DNode37.a * tex2D( _AddTexture, lerpResult44.xy ) * _AddColor ) );
				float clampResult62 = clamp( break60.a , 0.0 , 1.0 );
				float4 appendResult61 = (float4(break60.r , break60.g , break60.b , clampResult62));
				
				
				finalColor = ( appendResult61 * i.ase_color );
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
Node;AmplifyShaderEditor.RangedFloatNode;64;-1942.025,-641.2625;Inherit;False;Property;_GlowSpeed;GlowSpeed;6;0;Create;True;0;0;0;False;0;False;0.5;0.5;-3;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;65;-1653.522,-650.9725;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-1874.009,-415.6918;Inherit;False;Property;_GlowInterval;GlowInterval;8;0;Create;True;0;0;0;False;0;False;2;2;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimplifiedFModOpNode;66;-1470.44,-542.2556;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;68;-1171.556,-496.9896;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-1317.652,-351.4531;Inherit;False;Constant;_Float2;Float 2;7;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-1383.652,-208.4531;Inherit;False;Property;_GlowHeight;GlowHeight;7;0;Create;True;0;0;0;False;0;False;1.5;1.5;1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-1000.709,-421.175;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;73;-867.6519,-316.4531;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;47;-942.4255,239.6392;Inherit;False;Property;_NoiseSpeed;NoiseSpeed;2;0;Create;True;0;0;0;False;0;False;0,1.53;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;48;-961.5637,52.86407;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;40;-375.6417,-684.2623;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;74;-654.6519,-385.4531;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;49;-579.2563,136.7376;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;45;-180.5662,28.5615;Inherit;True;Property;_NoiseTexture;NoiseTexture;3;0;Create;True;0;0;0;False;0;False;-1;f96aebb9bcd1e3b44bafbb23b1bdac88;319c03c85f68e924e986b617c1ba1b4b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;46;305.6338,193.6614;Inherit;False;Property;_NoiseIntensity;NoiseIntensity;4;0;Create;True;0;0;0;False;0;False;0.1;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;29.14954,-565.5981;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;44;546.4582,-165.2881;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;39;790.5643,-255.7485;Inherit;True;Property;_AddTexture;AddTexture;1;0;Create;True;0;0;0;False;0;False;-1;7aad8c583ef292e48b06af0d1f2fab97;ab9d949350ceda84d9744ca34861e1d1;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;51;925.9171,22.13373;Inherit;False;Property;_AddColor;AddColor;5;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;2,2,2,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;37;939.5934,-662.1848;Inherit;True;Property;_MainTexture;MainTexture;0;0;Create;True;0;0;0;False;0;False;-1;None;922674f5e90859544a2fd87178022f02;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;1201.601,-298.4187;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;38;1346.267,-461.2437;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;60;1480.412,-541.5037;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ClampOpNode;62;1549.001,-271.593;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;53;1518.473,-81.2052;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;61;1690.006,-569.8326;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;1853.943,-228.2241;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;2078.374,-375.1747;Float;False;True;-1;2;ASEMaterialInspector;100;5;JiYuStudioUI/JiYuUIFlow;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;False;;10;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
WireConnection;65;0;64;0
WireConnection;66;0;65;0
WireConnection;66;1;67;0
WireConnection;68;0;66;0
WireConnection;69;0;68;0
WireConnection;69;1;71;0
WireConnection;69;2;72;0
WireConnection;73;0;69;0
WireConnection;73;1;72;0
WireConnection;74;1;73;0
WireConnection;49;0;48;0
WireConnection;49;2;47;0
WireConnection;45;1;49;0
WireConnection;63;0;40;0
WireConnection;63;1;74;0
WireConnection;44;0;63;0
WireConnection;44;1;45;1
WireConnection;44;2;46;0
WireConnection;39;1;44;0
WireConnection;50;0;37;4
WireConnection;50;1;39;0
WireConnection;50;2;51;0
WireConnection;38;0;37;0
WireConnection;38;1;50;0
WireConnection;60;0;38;0
WireConnection;62;0;60;3
WireConnection;61;0;60;0
WireConnection;61;1;60;1
WireConnection;61;2;60;2
WireConnection;61;3;62;0
WireConnection;52;0;61;0
WireConnection;52;1;53;0
WireConnection;0;0;52;0
ASEEND*/
//CHKSM=3F0012E6BF031C054F54F13ABB1B94B7BE70AE93