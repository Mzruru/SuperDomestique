Shader "Nature/Sky/GradientSky" {

	Properties {
		_UpperMidnightColour("Upper Midnight Colour", Color) = (0,0,0.208,1)
		_LowerMidnightColour("Lower Midnight Colour", Color) = (0,0.137,0.404,1)
		_UpperDawnStartColour("Upper Dawn Start Colour", Color) = (0.043, 0, 0.345,1)
		_LowerDawnStartColour("Lower Dawn Start Colour", Color) = (0, 0.165, 0.529,1)
		_UpperDawnMidColour("Upper Dawn Mid Colour", Color) = (0.870, 0.917, 0,1)
		_LowerDawnMidColour("Lower Dawn Mid Colour", Color) = (1, 0.302, 0.302,1)
		_UpperDawnEndColour("Upper Dawn End Colour", Color) = (0.470, 0.914, 1,1)
		_LowerDawnEndColour("Lower Dawn End Colour", Color) = (0.856, 0.914, 1,1)
		_UpperMiddayColour("Upper Midday Colour", Color) = (0.447, 0.976, 1,1)
		_LowerMiddayColour("Lower Midday Colour", Color) = (0.706, 0.706, 1,1)
		_UpperDuskStartColour("Upper Dusk Start Colour", Color) = (0.357, 0.639, 0.808,1)
		_LowerDuskStartColour("Lower Dusk Start Colour", Color) = (0.216, 0.216, 0.965,1)
		_UpperDuskMidColour("Upper Dusk Mid Colour", Color) = (0.196, 0.255, 0.839,1)
		_LowerDuskMidColour("Lower Dusk Mid Colour", Color) = (0.745, 0.216, 1,1)
		_UpperDuskEndColour("Upper Dusk End Colour", Color) = (0, 0.051, 0.573,1)
		_LowerDuskEndColour("Lower Dusk End Colour", Color) = (0, 0, 0.278,1)
	
		_DawnTime("Dawn Time (mins from midnight)", float) = 360.0
		_DuskTime("Dusk Time (mins from midnight)", float) = 1140.0
		_DawnLength("Dawn Length (mins)", float) = 60.0
		_DuskLength("Dusk Length (mins)", float) = 60.0
		[HideInInspector] _CurrentTime("Current Time (mins from midnight)", float) = 360.0
	}
	
	SubShader {
		Pass {
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

			uniform float4 _UpperMidnightColour;
			uniform float4 _LowerMidnightColour;
			uniform float4 _UpperDawnStartColour;
			uniform float4 _LowerDawnStartColour;
			uniform float4 _UpperDawnMidColour;
			uniform float4 _LowerDawnMidColour;
			uniform float4 _UpperDawnEndColour;
			uniform float4 _LowerDawnEndColour;
			uniform float4 _UpperMiddayColour;
			uniform float4 _LowerMiddayColour;
			uniform float4 _UpperDuskStartColour;
			uniform float4 _LowerDuskStartColour;
			uniform float4 _UpperDuskMidColour;
			uniform float4 _LowerDuskMidColour;
			uniform float4 _UpperDuskEndColour;
			uniform float4 _LowerDuskEndColour;
			
			uniform float _DawnTime;
			uniform float _DuskTime;
			uniform float _DawnLength;
			uniform float _DuskLength;
			uniform float _CurrentTime;
			
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
					top = lerp(_UpperMidnightColour, _UpperDawnStartColour, ratio);
					bot = lerp(_LowerMidnightColour, _LowerDawnStartColour, ratio);
				} else if (_CurrentTime < _DawnTime + (_DawnLength / 2)) {
					ratio = (_CurrentTime - _DawnTime) / (_DawnLength / 2);
					top = lerp(_UpperDawnStartColour, _UpperDawnMidColour, ratio);
					bot = lerp(_LowerDawnStartColour, _LowerDawnMidColour, ratio);
				} else if (_CurrentTime < _DawnTime + _DawnLength) {
					ratio = (_CurrentTime - (_DawnTime + (_DawnLength / 2))) / (_DawnLength / 2);
					top = lerp(_UpperDawnMidColour, _UpperDawnEndColour, ratio);
					bot = lerp(_LowerDawnMidColour, _LowerDawnEndColour, ratio);
				} else if (_CurrentTime < 720) {
					ratio = (_CurrentTime - (_DawnTime + _DawnLength)) / (720 - (_DawnTime + _DawnLength));
					top = lerp(_UpperDawnEndColour, _UpperMiddayColour, ratio);
					bot = lerp(_LowerDawnEndColour, _LowerMiddayColour, ratio);
				} else if (_CurrentTime < _DuskTime) {
					ratio = (_CurrentTime - 720) / (_DuskTime - 720);
					top = lerp(_UpperMiddayColour, _UpperDuskStartColour, ratio);
					bot = lerp(_LowerMiddayColour, _LowerDuskStartColour, ratio);
				} else if (_CurrentTime < _DuskTime + (_DuskLength / 2)) {
					ratio = (_CurrentTime - _DuskTime) / (_DuskLength / 2);
					top = lerp(_UpperDuskStartColour, _UpperDuskMidColour, ratio);
					bot = lerp(_LowerDuskStartColour, _LowerDuskMidColour, ratio);
				} else if (_CurrentTime < _DuskTime + _DuskLength) {
					ratio = (_CurrentTime - (_DuskTime + (_DuskLength / 2))) / (_DuskLength / 2);
					top = lerp(_UpperDuskMidColour, _UpperDuskEndColour, ratio);
					bot = lerp(_LowerDuskMidColour, _LowerDuskEndColour, ratio);
				} else {
					ratio = (_CurrentTime - (_DuskTime + _DuskLength)) / (1440 - (_DuskTime + _DuskLength));
					top = lerp(_UpperDuskEndColour, _UpperMidnightColour, ratio);
					bot = lerp(_LowerDuskEndColour, _LowerMidnightColour, ratio);
				}
				
				float topValue = 1 - i.texcoord0.y;
				float bottomValue = i.texcoord0.y;
				float3 mixedRGB = (top.rgb *topValue) + (bot.rgb * bottomValue);
				return float4(mixedRGB,1.0);
			}
			
			ENDCG
		}
	}
}