Shader "RS Mobile/Metal/Texture/Opaque"
{
    Properties
    {
        _Texture ("Texture (RGBA)", 2D) = "white" {}
        _CubeTex ("Reflection Cubemap", CUBE) = "black" {}
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
            samplerCUBE _CubeTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 reflDir : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Texture);

                // world normal!!
                float3 worldNormal = normalize(mul(
                    (float3x3)_Object2World,
                    normalize(v.normal)));

                // reflection vector!!
                float3 worldPos = mul(
                    _Object2World, v.vertex).xyz;
                float3 incident = worldPos
                    - _WorldSpaceCameraPos;
                float3 refl = incident - 2.0
                    * dot(worldNormal, incident)
                    * worldNormal;

                // flip x for cubemap!!
                o.reflDir = float3(
                    -refl.x, refl.y, refl.z);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_Texture, i.uv);
                fixed3 cube = texCUBE(
                    _CubeTex, i.reflDir).rgb;

                fixed4 c;
                // cubemap * alpha * texture!!
                c.rgb = cube * tex.a * tex.rgb;
                c.a = 0.0;
                return c;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}