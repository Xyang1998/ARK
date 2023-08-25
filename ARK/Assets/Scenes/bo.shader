Shader "Chapter/Billboard"
//广告牌效果的作用，就是无轮视角发生怎样的变化，总让物体显示朝向你想要的(固定的方向)
{
    Properties
    {
        _MainTex ("_Main Tex", 2D) = "white" {}
        _Color ("_Color Tint", Color) = (1,1,1,1)
        _VecticalBillborading ("Vertical Restraints",Range(0,1)) = 1 //用于调整是固定法线方向，还是固定向上的方向，即垂直约束
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnorePrjectot" = "True" "DisableBatching" = "True" }
 
        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off //关闭剔除功能，保证背面的水能显示
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
 
            struct a2v
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed _VecticalBillborading;
 
            v2f vert (a2v v)
            {
                v2f o;
                //使用模型空间的原点作为锚点
                float3 center = float3(0,0,0);
                //获取模型空间下的相机的位置
                float3 viewer = mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1));
                //计算三个正交矢量
                //获取法线方向
                //TODO 这里有个疑问，为什么法线方向是这样获取的？ 
                // 初步理解，这里应该不是取法线，而是按照我们想要的规则，定义一个我们想要的法线方向。
                // 或者我们换个角度思考。法线方向，其实应该是我们90度垂直看向一个物体的某个点时的方向，如果是平面的话，所有的法线是平行的，如果是个其他物体，则垂直于切线方向。
                // 那么我们在这里找的法线方向，其实是，我们垂直正视物体的方向。而我们用广告牌技术要实现的效果，就是我们无论从任何角度去看这个物体，都是垂直正视的，所以法线就是相机到物体的方向。
                // 如果这么想的话，之前的凹凸材质也比较清晰了，之所以有凹凸效果，正因为垂直正视的方向被法线贴图扰乱到了其他角度，巴拉不拉巴拉比拉bualbual(不知道怎么组织语言了)
                float3 normalDir = viewer - center;
                //当_VecticalBillborading为1时，意味着法线方向一直朝向相机
                //当_VecticalBillborading为0时，意味着法线方向的Y轴为0，即分布在XZ的平面上。此时向上方向理论上与法线垂直，所以向上方向为(0,1,0) 
                normalDir.y = normalDir.y * _VecticalBillborading;
                //
                normalDir = normalize(normalDir);
                //要注意，向上方向是模型空间下的up方向(类似transform.up),而不是切线方向，不要搞混
                //如果法线方向已经是(0,1,0),那我们需要矫正upDir为前方向防止他们平行
                float3 upDir = abs(normalDir.y) > 0.999 ? float3(0,0,1) : float3(0,1,0);
                float3 rightDir = normalize(cross(upDir,normalDir));
                //TODO 看书时，书上说明，前面的upDir是不准确的，使用upDir和normalDir叉乘得到rightDir方向，
                //TODO 然后再用rightDir和normalDir的叉乘得到准确的upDir,但是，既然upDir是不准确的，怎么能得到正确的rightDir呢？
                upDir = normalize(cross(normalDir,rightDir));
                //因为我们强行转动了物体在相机中出现的视角，所以渲染时用于插值的顶点坐标应该校正一下。
                float3 centerOffs = v.vertex.xyz - center;
                float3 localPos = center + rightDir * centerOffs.x + upDir * centerOffs.y + normalDir * centerOffs.z;
                o.pos = UnityObjectToClipPos(float4(localPos,1));
				o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex,i.uv);
                c.rgb *= _Color.rgb;
                return c;
            }
            ENDCG
        }
    }
 
    Fallback "Transparent/VertexLit"
}