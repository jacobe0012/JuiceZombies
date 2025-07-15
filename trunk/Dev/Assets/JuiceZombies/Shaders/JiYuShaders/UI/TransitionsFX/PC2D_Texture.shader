// Adapted from a tutorial by https://twitter.com/DanielJMoran

Shader "Hidden/ProCamera2D/TransitionsFX/Texture" 
{
    Properties 
    {
        _MainTex ("Texture", 2D) = "white" {}
		//_MainTex_ST("MainTex_ST", Vector) = (1, 1, 0, 0)
        _Step ("Step", Range(0, 1)) = 0
        _BackgroundColor ("Background Color", Color) = (0, 0, 0, 1)
        _TransitionTex("Transition Texture", 2D) = "white" {}
        _Smoothing ("Smoothing", Range(0, 1)) = .3
		ScreenScale("Screen Scale", Vector) = (1170, 2532, 0, 0)

    }
    SubShader
	{
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
		// No culling or depth
            

		Pass
		{
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
            ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			

            sampler2D _MainTex;
			
            float _Step;
            float4 _BackgroundColor;
            sampler2D _TransitionTex;
            float _Smoothing;
			float4 _MainTex_ST;
			float4 ScreenScale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = v.uv;
				float y=ScreenScale.y /ScreenScale.x;
			
				float4 tilloffset=float4(1,y,0,-y/4);
				o.uv = v.uv.xy * tilloffset.xy + tilloffset.zw;
				//output.uv0 = v.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				return o;
			}
            fixed4 frag (v2f i) : SV_Target
            {
            	if(_Step == 1)
            		return _BackgroundColor;
            	//i.uv = i.uv.xy* _MainTex_ST.xy + _MainTex_ST.zw;

                fixed4 transitTex = tex2D(_TransitionTex, i.uv);


                fixed4 colour = tex2D(_MainTex, i.uv);

				if (_Step >= transitTex.r)
				{
					float alpha = 1;

					if(_Step > 1 - _Smoothing)
						alpha = (_Step - transitTex.r) / (1 - _Step);
					else
						alpha = (_Step - transitTex.r) / _Smoothing;

					alpha = clamp(alpha, 0, 1);
					return lerp(colour, _BackgroundColor, alpha);
				}

				return colour;
            }

            ENDCG
        }
    }
}