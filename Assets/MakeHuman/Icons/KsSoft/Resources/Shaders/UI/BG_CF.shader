// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom UI/BG_CF" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_SubTex("Sub (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
	Tags {"Queue"="Background" "LightMode"="Always" }
	Fog {	Mode	Off	}
   	Pass {
		Cull		Off
		Blend		Off
		ZWrite		Off
		Lighting	Off
		ZTest		Less

CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
sampler2D	_MainTex;
sampler2D	_SubTex;
fixed4		_Color;

struct vertdata {
	float4	vertex : POSITION;
	fixed4  color : COLOR;
	half2 	texcoord : TEXCOORD0;
};
struct v2f {
	float4  pos : SV_POSITION;
	fixed4	color : COLOR;
	half2   uv : TEXCOORD0;
};

v2f vert(vertdata v)
{
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.color = v.color * _Color;
	o.uv = v.texcoord;
	return o;
}

fixed4 frag(v2f i) : COLOR
{
	fixed4 tex0 = tex2D(_MainTex, i.uv);
	fixed4 tex1 = tex2D(_SubTex, i.uv);
	tex0.rgb = lerp(tex0.rgb, tex1.rgb, i.color.a) * i.color.rgb * 2.f;
	return tex0;
}
ENDCG
    }
}
}
