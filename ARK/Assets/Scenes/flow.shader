Shader "Unlit/flow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "DisableBatching"="True" "IgnorePrjector" = "True"}

        Pass
        {
            Tags{"LightMode"="ForwardBase"}
            //ZWrite Off
            //Blend SrcAlpha OneMinusSrcAlpha
            //Blend one zero
            Cull off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (a2v v)
            {
                v2f o;
                float3 center=float3(0,0,0);
                float3 viewer=mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1));
                float3 normaldir=viewer-center;
                normaldir.y=normaldir.y*1.0;
                normaldir=normalize(normaldir);
                float3 updir=abs(normaldir.y)>0.999? float3(0,0,1):float3(0,1,0);
                float3 rightdir=normalize(cross(updir,normaldir));
                updir=normalize((cross(normaldir,rightdir)));
                float3 offset=v.vertex.xyz-center;
                float3 localpos=center+rightdir*offset.x+updir*offset.y+normaldir*offset.z;
                o.vertex=UnityObjectToClipPos(float4(localpos,1));
                //o.vertex=mul(unity_MatrixMVP,localpos);
                o.uv=v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
