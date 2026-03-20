Shader "Furby/AlphaMask" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
 _Mask ("Culling Mask", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  SetTexture [_Mask] { combine texture }
  SetTexture [_MainTex] { combine texture, previous alpha }
 }
}
}