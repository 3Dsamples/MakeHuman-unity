Shader "Custom UI/Title" {
	Properties {
		_MainTex	("Base (RGB)", 2D) = "white" {}
		_SubTex 	("SubTex(RGBA)", 2D) = "white" {}
		_UVOffset 	("UV Offset",Vector) = (0,0,0,0)
		_Color		("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
	Tags {"Queue"="Transparent+501" "LightMode"="Always" }
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

		#include "UI2.h"
		ENDCG
    }
}
}
