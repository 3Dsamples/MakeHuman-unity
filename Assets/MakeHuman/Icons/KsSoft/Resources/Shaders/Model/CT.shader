Shader "Custom/CT" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color",Color) = (1,1,1,1)
//		_FogColor	("Fog",Color) = (1,1,1,0.1)
	}
	SubShader {
	Tags {	"LightMode"="Always"	}
    Pass {
		Cull		Back
		Blend		Off
		ZWrite		On
		AlphaTest	Off
		Lighting	Off
		ZTest		Less

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "CT.h"
		ENDCG
    }
}
}
