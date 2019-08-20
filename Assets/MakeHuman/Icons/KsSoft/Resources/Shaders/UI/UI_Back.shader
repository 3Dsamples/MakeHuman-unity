Shader "Custom UI/Back" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
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

		#include "UI.h"
		ENDCG
    }
}
}
