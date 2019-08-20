#include "UnityCG.cginc"

sampler2D	_MainTex;
fixed4		_Color;

struct vertdata {
    float4	vertex : POSITION;
    float4  page : TANGENT;
    fixed4	color : COLOR0;
    half2 	texcoord : TEXCOORD0;
};
struct v2f {
    float4  pos : SV_POSITION;
    fixed4	color : COLOR0;
    fixed4	page : COLOR1;
    half2   uv : TEXCOORD0;
};

v2f vert (vertdata v)
{
    v2f o;
    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.color = v.color * _Color;
	o.color.rgb *= 2.f;
    o.uv = v.texcoord;
	o.page = v.page;
    return o;
}

fixed4 frag (v2f i) : SV_Target
{
    fixed4 tex0 = tex2D (_MainTex, i.uv);
	fixed4 color = i.color;
	fixed a = tex0.r * i.page.r + tex0.g * i.page.g + tex0.b * i.page.b + tex0.a * i.page.a;
	color.a *= a;
    return color;
}
