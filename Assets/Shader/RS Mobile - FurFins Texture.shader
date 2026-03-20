Shader "RS Mobile/FurFins/Texture"
{
    Properties
    {
        _Texture ("Texture (RGBA)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Opaque" }
        LOD 80

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"

            sampler2D _Texture;
            float4 _Texture_ST;

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

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c;
                // rgb = black!!
                // alpha only from texture!!
                c.rgb = fixed3(0, 0, 0);
                c.a = tex2D(_Texture, i.uv).a;
                return c;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}