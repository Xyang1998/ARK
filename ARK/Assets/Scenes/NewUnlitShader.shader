Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HAmount ("HAmount",int)=7
        _VAmount ("VAmount",int)=6
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"  }

        Pass
        {
             Tags {  "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            float _HAmount;
            float _VAmount;

            v2f vert (a2v v)
            {
                v2f o;
                float speed=0.5;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex)+frac(float2(speed,0)*_Time.y);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float speed=1;
                float time=floor(_Time.y*speed);
                float row=floor(time/_HAmount);
                float col=time-row*_HAmount;
                //half2 uv = float2(i.uv.x * offsetX, i.uv.y*offsetY);
 
 //将所显示的图片缩放至应有的大小 （即一个关键帧图像的大小）
 //half2 uv = float2(i.uv.x /_HAmount, i.uv.y / _VAmount); // 等价于上面3句
 
 //下面方法虽然不能和序列帧动画一一对应，但仍符合序列帧动画的执行顺序
// uv.x += col / _HAmount; // 更换序列帧
// uv.y -= row / _VAmount;  //等价于uv.y += 1.0 - row / _VerticalAmount; 
               // row=modf(row,_HAmount);
                //half2 uv=i.uv+half2(col,-row);
                //uv.x/=_HAmount;
                //uv.y/=_VAmount;

                
                fixed4 color = tex2D(_MainTex, i.uv);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return color;
            }
            ENDCG
        }
    }
}
