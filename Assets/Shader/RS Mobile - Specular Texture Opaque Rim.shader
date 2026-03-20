Shader "RS Mobile/Specular/Texture/Opaque/Rim"
{
    Properties
    {
        _Texture ("Texture (RGBA)", 2D) = "white" {}
        _CubeTex ("Reflection Cubemap", CUBE) = "black" {}
        _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0)
        _RimPower ("Rim Power", Range(0.5,8)) = 3
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
            #include "Lighting.cginc"

            sampler2D _Texture;
            float4 _Texture_ST;
            samplerCUBE _CubeTex;
            float4 _RimColor;
            float _RimPower;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 reflDir : TEXCOORD1;
                float3 rimColor : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

                // world normal!!
                float3 worldNormal = normalize(mul(
                    (float3x3)_Object2World,
                    normalize(v.normal)));

                // view direction!!
                float3 worldPos = mul(
                    _Object2World, v.vertex).xyz;
                float3 viewDir = _WorldSpaceCameraPos
                    - worldPos;

                // rim effect!!
                float rim = 1.0 - saturate(dot(
                    normalize(viewDir),
                    worldNormal));
                o.rimColor = _RimColor.rgb
                    * pow(rim, _RimPower);

                // diffuse!!
                float diff = saturate(dot(
                    worldNormal,
                    _WorldSpaceLightPos0.xyz));
                o.color.rgb = _LightColor0.rgb
                    * diff
                    + UNITY_LIGHTMODEL_AMBIENT.rgb;
                o.color.a = 1.0;

                // reflection!!
                float3 incident = -viewDir;
                float3 refl = incident - 2.0
                    * dot(worldNormal, incident)
                    * worldNormal;
                o.reflDir = float3(
                    -refl.x, refl.y, refl.z);

                o.uv = TRANSFORM_TEX(v.uv, _Texture);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_Texture, i.uv)
                    * i.color;
                fixed4 c;
                c.rgb = tex.rgb * 2.0;
                // add cubemap!!
                c.rgb += texCUBE(
                    _CubeTex, i.reflDir).rgb
                    * tex.a;
                // add rim!!
                c.rgb += i.rimColor;
                c.a = 0.0;
                return c;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}