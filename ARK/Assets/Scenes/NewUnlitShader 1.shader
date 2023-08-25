//该shader无法根据Edge的参数来修正图片的UV,需要_UVScale参数手动匹配UV。_Edge参数一变化，_UVScale参数必定要修改
//该shader只适用于UGUI，并且会把图片的分辨率自动缩小一定数值，多出的分辨率用来做泛光处理
//该shader边缘光是根据长宽比例得到，不是根据像素宽度，意思就是说，长度越长，泛光越宽，越短，泛光越短
Shader "Custom/Edge"
{
	Properties
	{
		_Edge ("Edge", Range(0, 0.5)) = 0.1
		_EdgeColor ("EdgeColor", Color) = (1, 1, 1, 1)
		_MainTex ("MainTex", 2D) = "white" {}
		_UVScale ("UVScale", Range(0, 30)) = 0.13
		_Intensity ("Intensity", Range(0, 3)) = 1.86
	}
	SubShader
	{
 
	 Tags  
        {  
            "Queue"="Transparent"  
        }  
		Blend SrcAlpha OneMinusSrcAlpha
 
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
 
			fixed _Edge;
			fixed4 _EdgeColor;
			sampler2D _MainTex;
			float _UVScale;
			float _Intensity;
			float _Test;
 
			struct appdata
			{
				float4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
			};
 
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 objVertex : TEXCOORD0;
				fixed2 uv : TEXCOORD1;
				
			};
 
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.objVertex = v.vertex;
				o.uv = v.uv;
				
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{	
 
			   
				fixed x = i.uv.x ;
				fixed y = i.uv.y;
 
				
 
				//确定泛光的矩形的四个点
 
				float2 leftUp = float2(_Edge,1-_Edge);
 
				float2 leftDown = float2(_Edge,_Edge);
 
				float2 RightUp = float2(1-_Edge,1-_Edge);
 
				float2 RightDown = float2(1-_Edge,_Edge);
 
			
 
			    //确定该像素点距离 四个位置的距离
 
				float leftUpD = distance(leftUp,i.uv); 
 
				float2 leftDownD = distance(leftDown,i.uv); 
 
				float2 RightUpD = distance(RightUp,i.uv); 
 
				float2 RightDownD =  distance(RightDown,i.uv); 
 
				
				float alpha =0;
 
				//像素判断，判断该像素在九宫格的哪个位置，然后做的alpha赋值运算
 
				if(x<_Edge && (1-y)<_Edge)//左上
				    alpha=  pow((_Edge-leftUpD)/_Edge,_Intensity);
				else if(x<_Edge && y<_Edge)//左下
				    alpha=  pow((_Edge-leftDownD)/_Edge,_Intensity);
				else if((1-x)<_Edge && y<_Edge)//右下
				    alpha=  pow((_Edge-RightDownD)/_Edge,_Intensity);
				else if((1-x)<_Edge && (1-y)<_Edge)//左上
				    alpha=  pow((_Edge-RightUpD)/_Edge,_Intensity);
				else if((x < _Edge))//左边
				    alpha = pow(x/_Edge,_Intensity);
				else if(1 - x < _Edge)//右边
				    alpha = pow((1-x)/_Edge,_Intensity);
				else if(1 - y < _Edge)//上边
					alpha = pow((1-y)/_Edge,_Intensity);
				else if(y < _Edge)    //下边 
					alpha =pow(y/_Edge,_Intensity);
				else //中间显示的图形
				{
				      float4 addUV = float4(-_UVScale,-_UVScale,1+_UVScale*2,1+_UVScale*2);
					 fixed4 col = tex2D(_MainTex, i.uv*addUV.zw+addUV.xy);
					 alpha=1;
					 _EdgeColor.xyz =col.xyz;
				}
 
			  return fixed4(_EdgeColor.xyz,alpha);
			}
			ENDCG
		}
	}
}
