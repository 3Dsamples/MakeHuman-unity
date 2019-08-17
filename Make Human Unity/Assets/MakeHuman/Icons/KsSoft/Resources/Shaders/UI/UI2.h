#include "UnityCG.cginc"
#define	WIDTH_H 240.f
#define	HEIGHT_H 160.f
sampler2D	_MainTex;
sampler2D	_SubTex;
fixed4		_Color;
fixed2		_UVOffset;

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

v2f vert (vertdata v)
{
    v2f o;
     o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.color = v.color * _Color;
    o.uv = v.texcoord;
    return o;
}

fixed4 frag (v2f i) : SV_Target
{
    fixed4 tex0 = tex2D (_MainTex, i.uv);
	fixed2	uv = i.uv;
	uv.x += _UVOffset.x + sin((uv.x + _UVOffset.y) * 3.14) * 0.1;
	uv.y += _UVOffset.y * uv.y * 0.25;
    fixed4 tex1 = tex2D (_SubTex, uv);
	tex1.rgb = tex1.rgb * i.color.rgb * i.color.a;
	tex0.rgb += tex1.rgb;
    return tex0;
}
