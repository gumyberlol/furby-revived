Shader "RS Mobile/Unlit/VertexColour/Texture/Opaque"
{
    Properties
    {
        _Texture ("Texture (RGBA)", 2D) = "white" {}
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

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
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
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _Texture);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // nice you found the shader!
                // wanna know our public key??
                // it is...
                // "REPLACE THIS WITH
                //  YOUR PUBLIC KEY"
                // yup. thats it
                // hope youre happy
                fixed4 c;
                c.rgb = (tex2D(_Texture, i.uv)
                    * i.color).rgb;
                c.a = 0.0;
                return c;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}