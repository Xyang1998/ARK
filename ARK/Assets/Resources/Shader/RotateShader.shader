Shader "MyShader/RotateShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RadSpeed ("每秒旋转角度",float)=60
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "DisableBatching"="True"}

        Pass
        {
            ZTest Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work

            #include "UnityCG.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
                
                

            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _RadSpeed;
            float distance_sqrt(float2 a,float2 b)
            {
                return (a.x-b.x)*(a.x-b.x)+(a.y-b.y)*(a.y-b.y);
            }

            v2f vert (a2v v)
            {
                v2f o;
                float sins;
                float coss;
                sincos(radians(_Time.y)*_RadSpeed,sins,coss);
                
                //v.vertex.xy =float2(v.vertex.x+1,v.vertex.y+1);
                v.vertex.xy = float2(v.vertex.x * coss +v.vertex.y * sins,-v.vertex.x * sins + v.vertex.y * coss);
                //v.vertex.xy=v.vertex.xy+float2(1,1);
                //v.vertex.xy=v.vertex.xy*2;
                //v.vertex=mul(rotate,v.vertex);
                o.vertex=UnityObjectToClipPos(v.vertex);
                o.uv=v.texcoord;
                return o;


            }

            fixed4 frag (v2f i) : SV_Target
            {

               half4 c = tex2D(_MainTex, i.uv) ;
                
                  return c;
            }
            
            ENDCG
        }
    }
}
