Shader "RS Mobile/Diffuse/Texture/Alpha"
{
    Properties
    {
        _Texture ("Texture (RGBA)", 2D) = "white" {}
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
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

                // world normal!!
                float3 worldNormal = mul(
                    (float3x3)_Object2World,
                    normalize(v.normal));

                // diffuse lighting!!
                float diff = saturate(dot(
                    worldNormal,
                    _WorldSpaceLightPos0.xyz));

                o.color.rgb = _LightColor0.rgb
                    * diff
                    + UNITY_LIGHTMODEL_AMBIENT.rgb;
                o.color.a = 1.0;
                o.uv = TRANSFORM_TEX(v.uv, _Texture);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_Texture, i.uv)
                    * i.color;
                fixed4 c;
                // rgb * 2 = double brightness!!
                c.rgb = tex.rgb * 2.0;
                c.a = tex.a;
                return c;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}