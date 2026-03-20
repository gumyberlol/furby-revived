Shader "Unlit/Transparent Colored FONT (SoftClip)"
{
    Properties
    {
        _MainTex ("Alpha (A)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent+10"
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
            float2 _ClipSharpness;

            struct appdata {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
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
                o.uv = v.uv;
                // clip from vertex pos!!
                o.clipUV = v.vertex.xy
                    * _MainTex_ST.xy
                    + _MainTex_ST.zw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // SoftClip logic!!
                float2 factor =
                    (1.0 - abs(i.clipUV))
                    * _ClipSharpness;
                float softClip = saturate(
                    min(factor.x, factor.y));

                // font = alpha only!!
                // rgb = white * color!!
                fixed4 col = i.color;
                col.a *= tex2D(
                    _MainTex, i.uv).a
                    * softClip;
                return col;
            }
            ENDCG
        }
    }
}
// shader so bad the bytecode had an error