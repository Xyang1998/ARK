Shader "UGUI/NPBar_new"
{
	Properties
	{
     
		 _MainTex ("Texture", 2D) = "white" {}
		 _BeginColor ("BeginColor",color)=(0,0,0,0)
        _EndColor ("EndColor",color)=(0,0,0,0)
        //_DeColor ("血量减少时的颜色",color)=(0,0,0,0)
        _InColor ("血量增加时的颜色",color)=(0,0,0,0)
        //_Threshold ("血量低于多少时变色",Range(0,1))=0.5
        //_ThresholdColor ("血量低于多少时变什么色",color)=(0,0,0,0)
        _TargetPer ("目标百分比",Range(0,1))=1
        _CurPer ("当前百分比",Range(0,1))=0.2
		[Header(Stencil)]
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
                [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
	        Cull Off   //关闭剔除
		Lighting Off
		ZWrite Off  //关闭深度写入
		ZTest [unity_GUIZTestMode]
		Blend One OneMinusSrcAlpha
		//ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

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
            float4 _MainTex_ST;
            fixed4 _BeginColor;
            fixed4 _EndColor;
            //float _Threshold;
            //fixed4 _ThresholdColor;
            float _TargetPer;
            //fixed4 _DeColor;
            fixed4 _InColor;
            float _CurPer;

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
			}

			fixed4 frag(v2f o) : SV_Target
			{
				 fixed4 col=(0,0,0,0);
                //float temp=o.uv.x+_Time.y*0.3;
                col=lerp(_BeginColor,_EndColor,o.uv.x);
                if(_CurPer<_TargetPer) //减血
                {
                    if(o.uv.x>_CurPer && o.uv.x<_TargetPer)
                    {
                        col=_InColor;
                    }
                }
                if(o.uv.x>max(_CurPer,_TargetPer))
                {
                    col=(0,0,0,0);
                }
                

                return col;
			}
		ENDCG
		}
	}
}