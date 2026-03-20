Shader "Furby/Incubator-Ice"
{
    Properties
    {
        _SpecColor ("Specular Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 300

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 worldNormalScaled : TEXCOORD2;
                float3 shLight : TEXCOORD3;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

                // world normal!!
                float3 worldNormal = mul(
                    (float3x3)_Object2World,
                    normalize(v.normal));
                o.worldNormal = worldNormal;

                // scaled world normal!!
                float3 scaledNormal = mul(
                    (float3x3)_Object2World,
                    normalize(v.normal) * unity_Scale.w);
                o.worldNormalScaled = scaledNormal;

                // view direction!!
                float3 worldPos = mul(
                    _Object2World,
                    v.vertex).xyz;
                o.viewDir = _WorldSpaceCameraPos
                    - worldPos;

                // spherical harmonics!!
                o.shLight = ShadeSH9(
                    float4(scaledNormal, 1.0));

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = i.worldNormal;
                float3 viewDir = normalize(i.viewDir);

                // fresnel = rim effect!!
                float fresnel = 1.0 - saturate(
                    dot(viewDir, normal));

                // specular!!
                float3 halfDir = normalize(
                    _WorldSpaceLightPos0.xyz + viewDir);
                float nh = max(0.0,
                    dot(i.worldNormalScaled,
                    halfDir));
                float spec = pow(nh, 128.0);

                fixed4 c;
                // specular colour!!
                c.xyz = (_LightColor0.xyz
                    * _SpecColor.xyz
                    * spec * 2.0)
                    + float3(1,1,1);

                // alpha = fresnel + specular!!
                c.w = fresnel
                    + (_LightColor0.w
                    * _SpecColor.w * spec);

                return c;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}