Shader "Furby/Eyes"
{
    Properties
    {
        _Texture ("Texture (RGBA)", 2D) = "white" {}
        _NumLines ("Num Lines", Float) = 150
        _LineWidth ("Line Width", Range(0,1)) = 0.25
        _GridColour ("Grid Colour", Color) = (0.15,0.15,0.15,1)
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 80

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"

            sampler2D _Texture;
            float4 _Texture_ST;
            float _NumLines;
            float _LineWidth;
            float3 _GridColour;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Texture);
                return o;
            }

            float GridLine(float coord)
			{
				float t = abs(frac(coord * _NumLines) - 0.5);
				float s = t / _LineWidth;
				float smooth = 1.0 - (s * s * (3.0 - 2.0 * s));
				// no if statement!!
				// use step instead!!
				return smooth * step(t, _LineWidth);
			}

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_Texture, i.uv);
                float gridX = GridLine(i.uv.x);
                float gridY = GridLine(i.uv.y);
                c.rgb += (gridX + gridY) * _GridColour;
                c.a = 0.0;
                return c;
            }
            ENDCG
        }
    }
    FallBack "Mobile/Diffuse"
}