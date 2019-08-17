#include "UnityCG.cginc"

sampler2D	_MainTex;
half4		_Color;
//half4		_FogColor;

struct vertdata {
    float4	vertex : POSITION;
    fixed4  color : COLOR;
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
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    o.color = v.color * _Color;
    o.uv.xy = v.texcoord;
    return o;
}

half4 frag (v2f i) : COLOR
{
    half4 tex0 = tex2D (_MainTex, i.uv);
    tex0 = tex0 * i.color;
	return tex0;
}
