Shader "Custom UI/NotexAlpha" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
	Tags {"Queue"="Transparent+500" "LightMode"="Always" }
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

		#include "UI_NotexAlpha.h"
		ENDCG
    }
}
}
