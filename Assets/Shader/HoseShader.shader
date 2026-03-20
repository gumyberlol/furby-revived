Shader "Furby/Hose Shader (A-B mix)"
{
    Properties
    {
        _TextureA ("Texture A (RGBA)", 2D) = "white" {}
        _TextureB ("Texture B (RGBA)", 2D) = "white" {}
        _TextureMix ("Texture Mix", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent"
        "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _TextureA;
            sampler2D _TextureB;
            float4 _TextureA_ST;
            float4 _TextureB_ST;
            float _TextureMix;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uvA : TEXCOORD0;
                float2 uvB : TEXCOORD1;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP,
                    v.vertex);
                o.uvA = TRANSFORM_TEX(
                    v.uv, _TextureA);
                o.uvB = TRANSFORM_TEX(
                    v.uv, _TextureB);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 a = tex2D(
                    _TextureA, i.uvA);
                fixed4 b = tex2D(
                    _TextureB, i.uvB);
                // lerp A and B!!
                return a * _TextureMix
                    + b * (1.0 - _TextureMix);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}