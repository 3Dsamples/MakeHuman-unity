// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Avatar" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Skin	 ("Skin",Color) = (1,1,1,1)
		_Hair	 ("Hair",Color) = (1,1,1,1)
		_EquipA	 ("EquipA",Color) = (1,1,1,1)
		_EquipB	 ("EquipB",Color) = (1,1,1,1)
		_Color ("Color",Color) = (1,1,1,1)
		_OutlineColor ("輪郭カラー.",Color) = (1,1,1,1)
		_Outline("輪郭の太さ.",float) = 0.02
	}
	SubShader {
		Tags {	"LightMode"="Always"	}
	    Pass {
			Cull		Back
			Blend		Off
			ZWrite		On
			Lighting	Off
			ZTest		Less
CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

sampler2D	_MainTex;
half4		_Skin;
half4		_Hair;
half4		_EquipA;
half4		_EquipB;
half4		_Color;

struct vertdata {
    float4	vertex : POSITION;
    half4  color : COLOR;
    half2 	texcoord : TEXCOORD0;	
};
struct v2f {
    float4  pos : SV_POSITION;
    half4	color : COLOR;
    half2   uv : TEXCOORD0;
};

v2f vert (vertdata v)
{
    v2f o;
    o.pos = UnityObjectToClipPos (v.vertex);
	o.color.xyz = _Skin * v.color.x + _Hair * v.color.y;
	o.color.xyz += (_EquipA * v.color.z + _EquipB * (1.0f - v.color.z)) * (1.f - v.color.x) * (1.f - v.color.y);
	o.color.w = v.color.w;
    o.uv.xy = v.texcoord;
    return o;
}

half4 frag (v2f i) : COLOR
{
    half4 tex0 = tex2D (_MainTex, i.uv);
	half4 color = _Color;

	half a = tex0.a;
	half inva = 1.0f - tex0.a;

	color.rgb	*= i.color.rgb * tex0.rgb * float3(inva,inva,inva) + tex0.rgb * float3(a,a,a);
	color.a 	*= i.color.a;
    return color;
}
ENDCG
	    }
	}
}
