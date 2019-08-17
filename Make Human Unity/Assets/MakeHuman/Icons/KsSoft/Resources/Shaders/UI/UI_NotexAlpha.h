#include "UnityCG.cginc"

fixed4		_Color;

struct vertdata {
    float4	vertex : POSITION;
    fixed4  color : COLOR;
};
struct v2f {
    float4  pos : SV_POSITION;
    fixed4	color : COLOR;
};

v2f vert (vertdata v)
{
    v2f o;
     o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.color = v.color * _Color;
    return o;
}

fixed4 frag (v2f i) : SV_Target
{
	i.color.rgb *= 2.f;
    return i.color;
}
