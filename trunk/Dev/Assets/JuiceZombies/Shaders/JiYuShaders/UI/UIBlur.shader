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

        // ˮƽģ�� Pass
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

                // ��˹ģ��Ȩ�أ��� 5 �����㣩
                float weight[3] = { 0.4026, 0.2442, 0.0545 };
                col += tex2D(_MainTex, uv) * weight[0];

                // ˮƽ�������
                for (int j = 1; j < 3; j++)
                {
                    col += tex2D(_MainTex, uv + float2(_MainTex_TexelSize.x * j * _BlurSize, 0)) * weight[j];
                    col += tex2D(_MainTex, uv - float2(_MainTex_TexelSize.x * j * _BlurSize, 0)) * weight[j];
                }

                return col;
            }
            ENDCG
        }

        // ��ֱģ�� Pass
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

                // ���Ӳ����㵽 7����ǿģ��Ч��
                float weight[4] = { 0.2941, 0.2353, 0.1176, 0.0588 }; // ������˹Ȩ��
                col += tex2D(_MainTex, uv) * weight[0];

                // ��ֱ�������
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