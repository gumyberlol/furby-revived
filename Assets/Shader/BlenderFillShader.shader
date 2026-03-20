Shader "Furby/BlenderFillShader"
{
    Properties
    {
        _Texture ("Texture (RGBA)", 2D) = "white" {}
        _DebugColour1 ("In #1", Color) = (1,1,1,1)
        _DebugColour2 ("In #2", Color) = (1,1,1,1)
        _DebugColour3 ("In #3", Color) = (1,1,1,1)
        _DebugColourMix ("Mix", Color) = (1,1,1,1)
        _Hue ("Colorize Hue", Float) = 0.5
        _Sat ("Colorize Saturation", Float) = 0.5
        _Val ("Colorize Value", Float) = 0.5
        _SatA ("Amount Saturation", Float) = 0
        _ValA ("Amount Value", Float) = 0
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
            float _Hue;
            float _Sat;
            float _Val;
            float _SatA;
            float _ValA;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP,
                    v.vertex);

                // diffuse lighting!!
                float3 worldNormal = mul(
                    (float3x3)_Object2World,
                    normalize(v.normal));
                float diff = saturate(dot(
                    worldNormal,
                    _WorldSpaceLightPos0.xyz));
                o.color.rgb = _LightColor0.rgb
                    * diff
                    + UNITY_LIGHTMODEL_AMBIENT.rgb;
                o.color.a = 1.0;
                o.uv = TRANSFORM_TEX(
                    v.uv, _Texture);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 c;
                // diffuse * 2!!
                float3 rgb = (tex2D(
                    _Texture, i.uv)
                    * i.color).rgb * 2.0;

                // RGB to HSV!!
                float3 HSV;
                HSV.z = max(rgb.r,
                    max(rgb.g, rgb.b));
                float delta = HSV.z - min(rgb.r,
                    min(rgb.g, rgb.b));

                HSV.xy = float2(0, 0);
                if (delta != 0) {
                    HSV.y = delta / HSV.z;
                    float3 d = (HSV.z - rgb)
                        / delta;
                    d = d - d.zxy;
                    d.xy += float2(2, 4);
                    if (rgb.r >= HSV.z)
                        HSV.x = d.z;
                    else if (rgb.g >= HSV.z)
                        HSV.x = d.x;
                    else
                        HSV.x = d.y;
                    HSV.x = frac(HSV.x / 6.0);
                }

                // colorize!!
                float sat = HSV.y
                    * (1.0 - _SatA)
                    + _Sat * _SatA;
                float val = HSV.z
                    * (1.0 - _ValA)
                    + _Val * _ValA;

                // HSV to RGB!!
                float3 rgb2;
                rgb2.x = abs(_Hue * 6 - 3) - 1;
                rgb2.y = 2 - abs(_Hue * 6 - 2);
                rgb2.z = 2 - abs(_Hue * 6 - 4);
                c.rgb = ((saturate(rgb2) - 1)
                    * sat + 1) * val;
                c.a = 0.0;
                return c;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}