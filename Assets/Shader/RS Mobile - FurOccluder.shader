Shader "RS Mobile/FurOccluder"
{
    SubShader
    {
        Tags { "Queue"="Geometry-10" "RenderType"="Opaque" }
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

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // always white!!
                return fixed4(1, 1, 1, 1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}