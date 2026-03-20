Shader "Furby/AlphaMask"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Mask ("Culling Mask", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }

        Pass
        {
            Tags { "Queue"="Transparent" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _Mask;
            float4 _MainTex_ST;
            float4 _Mask_ST;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP,
                    v.vertex);
                o.uv = TRANSFORM_TEX(
                    v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // mask alpha!!
                // main texture rgb!!
                fixed4 main = tex2D(
                    _MainTex, i.uv);
                fixed mask = tex2D(
                    _Mask, i.uv).a;
                return fixed4(
                    main.rgb, mask);
            }
            ENDCG
        }
    }
}