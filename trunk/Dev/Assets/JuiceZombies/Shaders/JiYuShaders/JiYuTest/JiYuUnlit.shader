Shader "UnicornStudio/JiYuUnlit"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque"  }
		
		Pass
		{
			HLSLPROGRAM

			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON
			#pragma vertex vert
			#pragma fragment frag
            #include "UnityCG.cginc"

			struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : INSTANCEID_SEMANTIC;
				#endif
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				#if UNITY_ANY_INSTANCING_ENABLED
				uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
            };

			CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
			CBUFFER_END
			
			/*
			#if defined(UNITY_DOTS_INSTANCING_ENABLED)
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
					UNITY_DOTS_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
				#define _MainTex_ST UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4, _MainTex_ST)
			#endif	
			*/

            sampler2D _MainTex;


		    v2f vert (appdata v)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

				#if UNITY_ANY_INSTANCING_ENABLED
				o.instanceID = v.instanceID;
				#endif
                return o;
            }
			
            fixed4 frag (v2f i) : SV_Target
            {
				UNITY_SETUP_INSTANCE_ID(i);
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
			ENDHLSL
		}
	}
}

