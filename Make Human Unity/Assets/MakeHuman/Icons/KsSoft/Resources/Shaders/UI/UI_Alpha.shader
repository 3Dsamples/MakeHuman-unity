Shader "Custom UI/Alpha" {
	Properties {
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
	Tags {"Queue"="Transparent+502" "LightMode"="Always" }
	Fog {	Mode	Off	}
   	Pass {
		Cull		Off
		Blend		SrcAlpha OneMinusSrcAlpha
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
