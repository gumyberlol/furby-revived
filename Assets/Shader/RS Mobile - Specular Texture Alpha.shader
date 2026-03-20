Shader "RS Mobile/Specular/Texture/Alpha"
{
    Properties
    {
        _Texture ("Texture (RGBA)", 2D) = "white" {}
        _CubeTex ("Reflection Cubemap", CUBE) = "black" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 80

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

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
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 reflDir : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

                // world normal!!
                float3 worldNormal = normalize(mul(
                    (float3x3)_Object2World,
                    normalize(v.normal)));

                // diffuse lighting!!
                float diff = saturate(dot(
                    worldNormal,
                    _WorldSpaceLightPos0.xyz));

                o.color.rgb = _LightColor0.rgb
                    * diff
                    + UNITY_LIGHTMODEL_AMBIENT.rgb;
                o.color.a = 1.0;

                // reflection vector!!
                float3 worldPos = mul(
                    _Object2World, v.vertex).xyz;
                float3 incident = worldPos
                    - _WorldSpaceCameraPos;
                float3 refl = incident - 2.0
                    * dot(worldNormal, incident)
                    * worldNormal;

                // flip x!!
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
                c.rgb += texCUBE(
                    _CubeTex, i.reflDir).rgb
                    * tex.a;
                c.a = tex.a;
                return c;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}