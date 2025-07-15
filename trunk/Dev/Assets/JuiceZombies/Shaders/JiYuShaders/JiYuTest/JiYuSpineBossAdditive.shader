// Spine/Skeleton PMA Screen
// - single color multiply tint
// - unlit
// - Premultiplied alpha Multiply blending
// - No depth, no backface culling, no fog.
// - ShadowCaster pass

Shader "Spine/Blend Modes/Skeleton PMA AdditiveTest" {
	Properties {
		[HDR]_Color ("Tint Color", Color) = (1,1,1,1)
		[HDR]_Color1 ("Tint Color1", Color) = (0,0,0,0)
		[NoScaleOffset] _MainTex ("MainTex", 2D) = "black" {}
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		[HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Set to Always as default
		_JiYuFlip("JiYu Flip",Vector) = (0, 0, 0, 0)
		_JiYuSort("JiYu Sort",Vector) = (3, 6, 0, 0)
		_JiYuPivot("JiYu Pivot",Range(0, 1)) = 0.5
		// Outline properties are drawn via custom editor.
		[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
		[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
		[HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
		[HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
		[HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
		[HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
		[HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One One
		Lighting Off

		Stencil {
			Ref[_StencilRef]
			Comp[_StencilComp]
			Pass Keep
		}

		Pass {
			Name "Normal"

			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Assets\ApesGang\Shaders\JiYuShaders\JiYuTest\Cginc\JiYuShaderUtility.cginc"
			uniform sampler2D _MainTex;
			float2 _JiYuFlip;
			float2 _JiYuSort;
			float _JiYuPivot;
			float4 _sortingGlobalData;
			uniform float4 _Color;
			uniform float4 _Color1;
			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			VertexOutput vert (VertexInput v) {
				VertexOutput o;
				//o.pos = UnityObjectToClipPos(v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.pos.z = JiYuSortFunc(_JiYuPivot,_JiYuSort,_sortingGlobalData,UNITY_MATRIX_MVP);
				o.uv = v.uv;
				o.vertexColor = v.vertexColor * float4((_Color1.rgb+_Color.rgb) * _Color.a, _Color.a); // Combine a PMA version of _Color with vertexColor.
				return o;
			}

			float4 frag (VertexOutput i) : SV_Target {
				float4 texColor = tex2D(_MainTex, i.uv);

				#if defined(_STRAIGHT_ALPHA_INPUT)
				texColor.rgb *= texColor.a;
				#endif

				return (texColor * i.vertexColor);
			}
			ENDCG
		}

		Pass {
			Name "Caster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1

			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			struct v2f {
				V2F_SHADOW_CASTER;
				float4 uvAndAlpha : TEXCOORD1;
			};

			uniform float4 _MainTex_ST;

			v2f vert (appdata_base v, float4 vertexColor : COLOR) {
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				o.uvAndAlpha.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uvAndAlpha.z = 0;
				o.uvAndAlpha.a = vertexColor.a;
				return o;
			}

			uniform sampler2D _MainTex;
			uniform fixed _Cutoff;

			float4 frag (v2f i) : SV_Target {
				fixed4 texcol = tex2D(_MainTex, i.uvAndAlpha.xy);
				clip(texcol.a * i.uvAndAlpha.a - _Cutoff);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	CustomEditor "SpineShaderWithOutlineGUI"
}
