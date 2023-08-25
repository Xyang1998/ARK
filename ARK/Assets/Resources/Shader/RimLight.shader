Shader "Custom/Test0"
{
    Properties
    {
        _MainTex("渲染纹理",2D)="white"{}
        _EdgeColor("边缘颜色",Color)=(1,1,1,1)
        _EdgeArea("边缘范围",float)=1
    }
    SubShader
    {
        Pass
        {  
            Blend SrcAlpha OneMinusSrcAlpha
                     
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
		    
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _EdgeColor;
            fixed _EdgeArea;
            
 
            struct a2v
            {
                float4 vertex:POSITION;
                float2 texcoord:TEXCOORD0;
            };
 
            struct v2f
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
            };
 
            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
      
                return o;
            }
 
            fixed4 frag(v2f i):SV_Target
            {
                fixed4 texResult=tex2D(_MainTex,i.uv);
                //通过采样周围点的Alpha,判断边缘                
                float2 uv_up = i.uv + _MainTex_TexelSize.xy * float2(0,1) * _EdgeArea;
				float2 uv_down = i.uv + _MainTex_TexelSize.xy * float2(0,-1) * _EdgeArea;
				float2 uv_left = i.uv + _MainTex_TexelSize.xy * float2(-1,0) * _EdgeArea;
				float2 uv_right = i.uv + _MainTex_TexelSize.xy * float2(1,0) * _EdgeArea;
                
                //周围的Alpha
                fixed aroundAlpha=tex2D(_MainTex,uv_up).a+tex2D(_MainTex,uv_down).a+
                                    tex2D(_MainTex,uv_left).a+tex2D(_MainTex,uv_right).a;
                aroundAlpha=saturate(aroundAlpha);
                //判定颜色
                fixed3 color=lerp(_EdgeColor,texResult,aroundAlpha*texResult.a);
                
                return fixed4(color,aroundAlpha); 
            }
            ENDCG
        }
    }
}