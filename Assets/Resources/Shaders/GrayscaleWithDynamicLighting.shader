Shader "Custom/GrayscaleWithDynamicLighting"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GrayscaleAmount ("Grayscale Amount", Range(0, 1)) = 1
        _LightIntensity ("Light Intensity", Range(0, 8)) = 1
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1) // Base color to show when grayscale is 0
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
            #pragma multi_compile_fog

#include "UnityCG.cginc"

sampler2D _MainTex;
float _GrayscaleAmount;
float _LightIntensity;
float4 _BaseColor; // Base color for objects when grayscale is 0

            // Declare shadow map texture and light matrix
sampler2D _ShadowMap;
float4x4 _LightMatrix;

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
    float4 shadowCoord : TEXCOORD1; // Shadow coordinates for projection
};

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.normal = normalize(mul((float3x3) unity_WorldToObject, v.normal)); // Transform normal to world space

                // Calculate shadow coordinates using the light matrix
    o.shadowCoord = mul(_LightMatrix, v.vertex); // Transform vertex to shadow map space
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
                // Sample the texture color (if there is one)
    fixed4 col = tex2D(_MainTex, i.uv);

                // If no texture is present, use the base color
    if (col.a == 0) // Check if the texture is fully transparent
    {
        col = _BaseColor; // Use the base color if the texture is missing or fully transparent
    }

                // Calculate lighting
    fixed3 worldNormal = normalize(i.normal);
    fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz); // Get the light direction from Unity's lighting system
    float diff = max(dot(worldNormal, lightDir), 0.0);

                // If grayscale is set to 0, return the color with lighting applied
    if (_GrayscaleAmount == 0)
    {
        col.rgb *= (diff * _LightIntensity + 0.1); // Apply lighting to the original color
        return col;
    }

                // Convert to grayscale using luminance values
    float gray = dot(col.rgb, fixed3(0.299, 0.587, 0.114));
    fixed3 grayscaleColor = fixed3(gray, gray, gray);

                // Lerp between the original color and grayscale color based on the amount
    col.rgb = lerp(col.rgb, grayscaleColor, _GrayscaleAmount);

                // Combine the lighting with the color
    col.rgb *= (diff * _LightIntensity + 0.1); // Multiply by light intensity

                // Shadow handling (correct depth comparison)
                // Perform shadow comparison by sampling the shadow map with the correct texture coordinates
    float shadow = tex2Dproj(_ShadowMap, i.shadowCoord).r; // Sample the shadow map using projected coordinates
    shadow = 1.0 - shadow; // Invert the shadow value (dark = shadow, light = no shadow)

                // Apply shadow to the color
    col.rgb *= shadow;

    return col;
}
            ENDCG
        }
    }
FallBack"Diffuse"
}
