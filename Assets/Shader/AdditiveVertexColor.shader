Shader "tk2d/AdditiveVertexColor"
{
    Properties
    {
        _MainTex ("Base (RGB) Trans (A)",
            2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent"
        "IgnoreProjector"="true"
        "RenderType"="Transparent" }
        LOD 110

        Pass
        {
            ZWrite Off
            Cull Off
            Blend SrcAlpha One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

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
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // texture * vertex color!!
                // additive blend!!
                return tex2D(_MainTex, i.uv)
                    * i.color;
            }
            ENDCG
        }
    }
}