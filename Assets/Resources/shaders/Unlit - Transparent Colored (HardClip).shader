Shader "Unlit/Transparent Colored (HardClip)"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)",
            2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent"
        "IgnoreProjector"="true"
        "RenderType"="Transparent" }
        LOD 200
        Pass
        {
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 clipUV : TEXCOORD1;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP,
                    v.vertex);
                o.color = v.color;
                // UV from texcoord!!
                o.uv = v.uv.xy;
                // clip from vertex pos!!
                o.clipUV = v.vertex.xy
                    * _MainTex_ST.xy
                    + _MainTex_ST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // HardClip logic!!
                float2 factor = abs(i.clipUV);
                float x = 1.0 - max(
                    factor.x, factor.y);
                if (x < 0.0) discard;
                return tex2D(_MainTex, i.uv)
                    * i.color;
            }
            ENDCG
        }
    }
}