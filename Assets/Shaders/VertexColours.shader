Shader "Custom/VertexColours" {
	Properties {
		_Shininess ("Shininess", Range(0.03, 1)) = 0.078125
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert
		half _Shininess;
		struct Input {
			half2 uv_MainTex;
			fixed3 vertColours;
		};

		void vert (inout appdata_full v, out Input o) {
			o.vertColours = v.color.rgb;
			o.uv_MainTex = v.texcoord;
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.vertColours.rgb;
			o.Specular = 1;
			o.Gloss = _Shininess;
		}
		
		ENDCG
	} 
	FallBack "Specular"
}
