Shader "Custom/OutlineEffect"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Float) = 0.02
        _MainTex ("Main Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
LOD 200

        Pass
        {
ZWrite On

ZTest LEqual

Cull Back // Change this to Cull Back or Cull Off

Blend SrcAlpha
OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

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
    float4 color : COLOR;
};

fixed4 _OutlineColor;
float _OutlineThickness;

v2f vert(appdata v)
{
    v2f o;
    float3 norm = normalize(v.normal);
    v.vertex.xyz += norm * _OutlineThickness * 0.5; // Adjust offset
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    return _OutlineColor; // Output the outline color
}
            ENDCG
        }
    }
FallBack"Diffuse"
}
