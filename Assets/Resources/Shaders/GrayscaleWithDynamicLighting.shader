Shader "Custom/GrayscaleWithDynamicLighting"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GrayscaleAmount ("Grayscale Amount", Range(0, 1)) = 1
        _LightIntensity ("Light Intensity", Range(0, 8)) = 1 // Add a property for light intensity
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

sampler2D _MainTex;
float _GrayscaleAmount;
float _LightIntensity; // Declare the light intensity variable

struct appdata_t
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float3 normal : NORMAL;
    float4 color : COLOR;
};

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.normal = normalize(mul((float3x3) unity_WorldToObject, v.normal)); // Transform normal to world space
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
                // Sample the texture color
    fixed4 col = tex2D(_MainTex, i.uv);

                // Calculate lighting
    fixed3 worldNormal = normalize(i.normal);
    fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz); // Get the light direction from Unity's lighting system
    float diff = max(dot(worldNormal, lightDir), 0.0);

                // Convert to grayscale using luminance values
    float gray = dot(col.rgb, fixed3(0.299, 0.587, 0.114));
    fixed3 grayscaleColor = fixed3(gray, gray, gray);

                // Lerp between the original color and grayscale color based on the amount
    col.rgb = lerp(col.rgb, grayscaleColor, _GrayscaleAmount);

                // Combine the lighting with the color
    col.rgb *= (diff * _LightIntensity + 0.1); // Multiply by light intensity

    return col;
}
            ENDCG
        }
    }
FallBack"Diffuse"
}
