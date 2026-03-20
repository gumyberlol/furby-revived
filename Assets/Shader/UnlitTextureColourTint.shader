Shader "Unlit/TextureColourTint" {
Properties {
 _Color ("Tint Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 LOD 100
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  SetTexture [_MainTex] { ConstantColor [_Color] combine texture * constant }
 }
}
}