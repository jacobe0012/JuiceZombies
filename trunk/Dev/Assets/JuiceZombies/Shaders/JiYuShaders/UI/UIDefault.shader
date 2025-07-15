Shader "Custom/UIDefault"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        //_BlurSize ("Blur Size", Float) = 10
        _JiYuSort("JiYu Sort",Vector) = (0, 1, 0, 0)
        _JiYuPivot("JiYu Pivot",Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On

        // 水平模糊 Pass
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Assets\ApesGang\Shaders\JiYuShaders\JiYuTest\Cginc\JiYuShaderUtility.cginc"
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
            //float _BlurSize;
            int2 _JiYuSort;
            float _JiYuPivot;
            float4 _sortingGlobalData;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
               

                o.vertex.z = JiYuSortFunc(_JiYuPivot, _JiYuSort, _sortingGlobalData,UNITY_MATRIX_MVP);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = 0;
                float2 uv = i.uv;

                // 高斯模糊权重（简单 5 采样点）
                //float weight[3] = { 0.4026, 0.2442, 0.0545 };
                col = tex2D(_MainTex, uv);

                // 水平方向采样
                // for (int j = 1; j < 3; j++)
                // {
                //     col += tex2D(_MainTex, uv + float2(_MainTex_TexelSize.x * j * _BlurSize, 0)) * weight[j];
                //     col += tex2D(_MainTex, uv - float2(_MainTex_TexelSize.x * j * _BlurSize, 0)) * weight[j];
                // }

                return col;
            }
            ENDCG
        }
        
    }
}