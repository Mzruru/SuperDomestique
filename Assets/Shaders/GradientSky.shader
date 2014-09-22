Shader "Nature/Sky/GradientSky" {

	Properties {
		[HideInInspector] _C0U("C0U", Color) = (1,1,1,1)
		[HideInInspector] _C0L("C0L", Color) = (1,1,1,1)
		[HideInInspector] _C1U("C1U", Color) = (1,1,1,1)
		[HideInInspector] _C1L("C1L", Color) = (1,1,1,1)
		[HideInInspector] _C2U("C2U", Color) = (1,1,1,1)
		[HideInInspector] _C2L("C2L", Color) = (1,1,1,1)
		[HideInInspector] _C3U("C3U", Color) = (1,1,1,1)
		[HideInInspector] _C3L("C3L", Color) = (1,1,1,1)
		[HideInInspector] _C4U("C4U", Color) = (1,1,1,1)
		[HideInInspector] _C4L("C4L", Color) = (1,1,1,1)
		[HideInInspector] _C5U("C5U", Color) = (1,1,1,1)
		[HideInInspector] _C5L("C5L", Color) = (1,1,1,1)
		[HideInInspector] _C6U("C6U", Color) = (1,1,1,1)
		[HideInInspector] _C6L("C6L", Color) = (1,1,1,1)
		[HideInInspector] _C7U("C7U", Color) = (1,1,1,1)
		[HideInInspector] _C7L("C7L", Color) = (1,1,1,1)
	
		[HideInInspector] _DawnTime("Dawn Time (mins from midnight)", float) = 360.0
		[HideInInspector] _DuskTime("Dusk Time (mins from midnight)", float) = 1140.0
		[HideInInspector] _DawnLength("Dawn Length (mins)", float) = 60.0
		[HideInInspector] _DuskLength("Dusk Length (mins)", float) = 60.0
		[HideInInspector] _CurrentTime("Current Time (mins from midnight)", float) = 360.0
		[HideInInspector] _OvercastAmount("Overcast sky (0 - 1)", float) = 0.0
		[HideInInspector] _OvercastDarken("Overcast darken (0 - 1)", float) = 0.0
	}
	
	SubShader {
		Pass {
		Fog {Mode Off}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			struct vertexInput {
				float4 vertex : POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			struct fragmentInput{
				float4 position : SV_POSITION;
				float4 texcoord0 : TEXCOORD0;
			};

			uniform float4 _C0U;
			uniform float4 _C0L;
			uniform float4 _C1U;
			uniform float4 _C1L;
			uniform float4 _C2U;
			uniform float4 _C2L;
			uniform float4 _C3U;
			uniform float4 _C3L;
			uniform float4 _C4U;
			uniform float4 _C4L;
			uniform float4 _C5U;
			uniform float4 _C5L;
			uniform float4 _C6U;
			uniform float4 _C6L;
			uniform float4 _C7U;
			uniform float4 _C7L;
			
			uniform float _DawnTime;
			uniform float _DuskTime;
			uniform float _DawnLength;
			uniform float _DuskLength;
			uniform float _CurrentTime;
			uniform float _OvercastAmount;
			uniform float _OvercastDarken;
			
			fragmentInput vert(vertexInput i){
				fragmentInput o;
				o.position = mul (UNITY_MATRIX_MVP, i.vertex);
				o.texcoord0 = i.texcoord0;
				return o;
			}
			float4 frag(fragmentInput i) : COLOR {
				
				float4 top = (1,1,1,1);
				float4 bot = (1,1,1,1);
				float ratio = 0;
				
				if (_CurrentTime < _DawnTime) {
					ratio = _CurrentTime / _DawnTime;
					top = lerp(_C0U, _C1U, ratio);
					bot = lerp(_C0L, _C1L, ratio);
				} else if (_CurrentTime < _DawnTime + (_DawnLength / 2)) {
					ratio = (_CurrentTime - _DawnTime) / (_DawnLength / 2);
					top = lerp(_C1U, _C2U, ratio);
					bot = lerp(_C1L, _C2L, ratio);
				} else if (_CurrentTime < _DawnTime + _DawnLength) {
					ratio = (_CurrentTime - (_DawnTime + (_DawnLength / 2))) / (_DawnLength / 2);
					top = lerp(_C2U, _C3U, ratio);
					bot = lerp(_C2L, _C3L, ratio);
				} else if (_CurrentTime < 720) {
					ratio = (_CurrentTime - (_DawnTime + _DawnLength)) / (720 - (_DawnTime + _DawnLength));
					top = lerp(_C3U, _C4U, ratio);
					bot = lerp(_C3L, _C4L, ratio);
				} else if (_CurrentTime < _DuskTime) {
					ratio = (_CurrentTime - 720) / (_DuskTime - 720);
					top = lerp(_C4U, _C5U, ratio);
					bot = lerp(_C4L, _C5U, ratio);
				} else if (_CurrentTime < _DuskTime + (_DuskLength / 2)) {
					ratio = (_CurrentTime - _DuskTime) / (_DuskLength / 2);
					top = lerp(_C5U, _C6U, ratio);
					bot = lerp(_C5L, _C6L, ratio);
				} else if (_CurrentTime < _DuskTime + _DuskLength) {
					ratio = (_CurrentTime - (_DuskTime + (_DuskLength / 2))) / (_DuskLength / 2);
					top = lerp(_C6U, _C7U, ratio);
					bot = lerp(_C6L, _C7L, ratio);
				} else {
					ratio = (_CurrentTime - (_DuskTime + _DuskLength)) / (1440 - (_DuskTime + _DuskLength));
					top = lerp(_C7U, _C0U, ratio);
					bot = lerp(_C7L, _C0L, ratio);
				}
				
				float topValue = 1 - i.texcoord0.y;
				float bottomValue = i.texcoord0.y;
				float3 mixedRGB = (top.rgb *topValue) + (bot.rgb * bottomValue);
				
				if (_OvercastAmount > 0)
				{
					float overcastValue = ((mixedRGB[0] + mixedRGB[1] + mixedRGB[2]) / 3) * (1 - _OvercastDarken);
					float3 overcastColour = (overcastValue, overcastValue, overcastValue); 
					mixedRGB = lerp (mixedRGB, overcastColour, _OvercastAmount);
				}
				
				return float4(mixedRGB,1.0);
			}
			
			ENDCG
		}
	}
}