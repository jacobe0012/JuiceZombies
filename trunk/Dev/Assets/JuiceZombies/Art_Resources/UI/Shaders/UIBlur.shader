Shader "Custom/UIBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 5
        _Color ("Tint Color", Color) = (0.2,0.2,0.2,1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        // 水平模糊 Pass
        Pass
        {
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
            float4 _MainTex_TexelSize;
            float _BlurSize;
            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = 0;
                float2 uv = i.uv;

                // 高斯模糊权重（9 采样点，标准差约 2.0）
                float weight[5] = {0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216};
                col += tex2D(_MainTex, uv) * weight[0];

                // 水平方向采样
                for (int j = 1; j < 5; j++)
                {
                    float offset = _MainTex_TexelSize.x * j * _BlurSize;
                    col += tex2D(_MainTex, uv + float2(offset, 0)) * weight[j];
                    col += tex2D(_MainTex, uv - float2(offset, 0)) * weight[j];
                }

                return col * _Color;
            }
            ENDCG
        }

        // 垂直模糊 Pass
        Pass
        {
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
            float4 _MainTex_TexelSize;
            float _BlurSize;
            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = 0;
                float2 uv = i.uv;

                // 高斯模糊权重（9 采样点，标准差约 2.0）
                float weight[5] = {0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216};
                col += tex2D(_MainTex, uv) * weight[0];

                // 垂直方向采样
                for (int j = 1; j < 5; j++)
                {
                    float offset = _MainTex_TexelSize.y * j * _BlurSize;
                    col += tex2D(_MainTex, uv + float2(0, offset)) * weight[j];
                    col += tex2D(_MainTex, uv - float2(0, offset)) * weight[j];
                }

                return col * _Color;
            }
            ENDCG
        }
    }
}