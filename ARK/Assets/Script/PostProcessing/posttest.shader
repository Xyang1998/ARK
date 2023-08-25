Shader "Unlit/posttest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness ("Brightness",float)=1
        _Saturation ("Saturation",float)=1
        _Contrast ("Contrast",float)=1
    }
    SubShader
    {


        Pass
        {ZTest Always
        Cull off
        Zwrite off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            half _Brightness;
            half _Saturation;
            half _Contrast; 
            float4 _MainTex_ST;

            #include "UnityCG.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            
            v2f vert (appdata_img v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv);
                fixed3 finalColor=col.rgb*_Brightness;
                fixed lu=0.2125*col.r+0.7154*col.g+0.0721*col.b;
                fixed3 luColor=fixed3(lu,lu,lu);
                finalColor=lerp(luColor,finalColor,_Saturation);
                fixed3 avgColor=fixed3(0.5,0.5,0.5);
                finalColor=lerp(avgColor,finalColor,_Contrast);
                

                UNITY_APPLY_FOG(i.fogCoord, col);
                return fixed4(finalColor,col.a);
            }
            ENDCG
        }
    }
}
