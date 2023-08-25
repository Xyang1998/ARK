// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PortaitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"  }
        Pass
        {
          
            Cull Off
            ZTest Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _MainTex_ST;
            float _Radius;
            float E = 2.718282;
            static float PI = 3.1415926;
            //
            

            struct a2v
            {
                float4 vertex: POSITION;
                float4 texcoord: TEXCOORD0;
                float4 maskcoord: TEXCOORD1;
            };

            struct v2f
            {
                float4 pos: SV_POSITION;
                float2 uv:TEXCOORD0;
                float2 maskuv:TEXCOORD1;
            };
            float distance_sqrt(float2 a,float2 b)
            {
                return (a.x-b.x)*(a.x-b.x)+(a.y-b.y)*(a.y-b.y);
            }
            
            v2f vert(a2v v)
            {
                v2f o;
                o.pos=UnityObjectToClipPos(v.vertex);
                o.uv=v.texcoord;
                o.maskuv=v.maskcoord;
                return o;
            }
            fixed4 frag(v2f i) :SV_Target
            {
               
                fixed4 texColor=tex2D(_MainTex,i.uv);
                fixed4 maskColor=tex2D(_MaskTex,i.maskuv);
                fixed alpha=1.0;
                if(maskColor.x<0.99)
                {
                    alpha=0;
                }
                else
                {
                   // float dis=distance_sqrt(i.uv,(0.5,0.5));
                   // if(dis<Radius)
                   // {
                   //     alpha=1.0;
                   // }
                   // else
                   // {
                   //     alpha=1.0-2*(dis-Radius)/(0.5-Radius);
                   //     //alpha=0.1;
                   // }
                    //if(abs(i.uv.x-0.5)>0.1)
                   // {
                        
                        //alpha=1-log((abs(i.uv.x-0.5)-0.1)*((E-1)/(0.4))+1);
                        //alpha=0.9-(abs(i.uv.x-0.5)-0.1)*2.5;
                        //PI = 3.1415926;
                        alpha=pow(sin(PI*i.uv.x),2);
                        //alpha=0.5;
                    //}

                    
                }
                return fixed4(texColor.xyz,alpha);
            }
            

            
            ENDCG

            

        }
    }
}
