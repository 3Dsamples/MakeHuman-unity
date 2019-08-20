#include "UnityCG.cginc"
sampler2D	_MainTex;
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

v2f vert (vertdata v)
{
    v2f o;
	float4		vertex = v.vertex;
     o.pos = mul (UNITY_MATRIX_MVP, vertex);
	o.color = v.color * _Color;
    o.uv = v.texcoord;
    return o;
}

fixed4 frag (v2f i) : SV_Target
{
    fixed4 tex0 = tex2D (_MainTex, i.uv);
	tex0.rgb *= i.color.rgb * 2.f;
	tex0.a   *= i.color.a;
	return tex0;
}
