Shader "RS Mobile/BRDF/BRDFAtlas CubeMapSpecular" {
Properties {
 _MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
 _Ramp2D ("BRDF Ramp", 2D) = "gray" {}
 _NumAtlasesX ("X Atlases", Float) = 1
 _CubeTex ("Reflection Cubemap", CUBE) = "black" {}
 _SpecularHighlightColor ("Specular Color", Color) = (1,1,1,1)
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutput o)
		{
			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}