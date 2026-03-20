Shader "Kaijudo/Particles/Additive Always In Front"
{
    Properties
    {
        _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _InvFade ("Soft Particles Factor",
            Range(0.01,3)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent+500"
        "IgnoreProjector"="true"
        "RenderType"="Transparent" }

        Pass
        {
            ZTest Off
            ZWrite Off
            Cull Off
            Blend SrcAlpha One
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TintColor;

            struct appdata {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP,
                    v.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(
                    v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // 2 * color * tint * texture!!
                return 2.0 * i.color
                    * _TintColor
                    * tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}