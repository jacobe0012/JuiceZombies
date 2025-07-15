Shader "JiYuStudioUI/JiYuUIFrameAnim"
{
   Properties
    {
		[NoScaleOffset]
        _MainTex ("Texture", 2D) = "white" {}
		_PlaySpeed("PlaySpeed",float) = 1//�����ٶ� 
        _UCount("_UCount",float) = 1 //����֡ˮƽ����
		_VCount("_VCount",float) = 1 //����֡��ֱ����

		 [Toggle] _AUTOPLAY("AutoPlay",Float) = 0	
    }
    SubShader
    {
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
 
        Pass
        {
            Cull Back
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
			//���������?
			#pragma shader_feature  _AUTOPLAY_ON
 
            struct appdata
            {
                half4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                half4 vertex : SV_POSITION;
            };
 
            sampler2D _MainTex;
			fixed _PlaySpeed;
			fixed  _UCount;
			fixed	_VCount;
	        /*�洢һ�µ�ǰ֡��ID�� ע�����ID�����;��Բ�������fixed���ͣ�
			���Ǹо���PC����û�����⣬����Ϊʲô��
			����ΪPC�˲�����ô�������Ͷ�����floatִ�еģ����ƶ��˾ͻ�ı��fixed���ͣ�
			�����͵����ݷ�Χ��-2��+2֮��*/
            half _FpsID=0;
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
		
				//�ж϶��������Ƿ���
			#ifdef  _AUTOPLAY_ON
             //��ǰID�ۼ� ������ʱ��ÿ����Բ����ٶ�?
				_FpsID += _Time.y*	_PlaySpeed;
	         #endif
            //��IDȡģԼ����ֵ��0~���ͼ��֮��?
			_FpsID = _FpsID % (_UCount*_VCount);
			//ID����ȡ��
			_FpsID = floor(_FpsID);
		    // �ݺ���ID = ID���Ժ������?
			half indexY = floor(_FpsID / _UCount);
			//����ID = ID��ȥ ���������������ID
            half indexX = _FpsID - _UCount * indexY;
			//��СUV,�Ŵ�ͼ��
			fixed2 AnimUV = float2(i.uv.x / _UCount, i.uv.y / _VCount);
			//����ID����������ȡƫ��ֵ�ۼӸ�����λ��
			AnimUV.x += indexX / _UCount ;
			/*ͬ�����򲥷����� 
			AnimUV.y +=indexY / _YSum;*/
			//(�������²��� )  �������IDΪ0  ����-1 - ��ǰID��IDԽ��YԽС ��
			AnimUV.y +=(_VCount-1 - indexY )/ _VCount;
			//����UV��ʾ��ͼ
			fixed4 col = tex2D(_MainTex, AnimUV);
			//͸���޳�,Ӳ�ü�����Լ���ܣ���Ȼ��Ч��Ļ���ȿ�����Ҫ��͸��������
			   clip(col.a - 0.05);           
                UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
            }
            ENDCG
        }
    }
 
}