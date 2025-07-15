Shader "Custom/UIBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 10
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = 0;
                float2 uv = i.uv;

                // 高斯模糊权重（简单 5 采样点）
                float weight[3] = { 0.4026, 0.2442, 0.0545 };
                col += tex2D(_MainTex, uv) * weight[0];

                // 水平方向采样
                for (int j = 1; j < 3; j++)
                {
                    col += tex2D(_MainTex, uv + float2(_MainTex_TexelSize.x * j * _BlurSize, 0)) * weight[j];
                    col += tex2D(_MainTex, uv - float2(_MainTex_TexelSize.x * j * _BlurSize, 0)) * weight[j];
                }

                return col;
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = 0;
                float2 uv = i.uv;

                // 增加采样点到 7，增强模糊效果
                float weight[4] = { 0.2941, 0.2353, 0.1176, 0.0588 }; // 调整高斯权重
                col += tex2D(_MainTex, uv) * weight[0];

                // 垂直方向采样
                for (int j = 1; j < 4; j++)
                {
                    float offset = _MainTex_TexelSize.y * j * _BlurSize;
                    col += tex2D(_MainTex, uv + float2(0, offset)) * weight[j];
                    col += tex2D(_MainTex, uv - float2(0, offset)) * weight[j];
                }
                return col;
            }
            ENDCG
        }
    }
}