Shader "Sprites/Desaturated"
{
    Properties
    {
        _Saturation("Saturation", Range(0, 1)) = 1.0
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }
        
        LOD 200
        CGPROGRAM

        #pragma surface surf Standard finalcolor:Final
        #pragma target 3.0

            struct Input {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        fixed4 _RGB;
        uniform float _Saturation;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        #include "ColorFunctions.cginc"

        // FinalPass shader to desaturate
        void Final(Input IN, SurfaceOutputStandard o, inout fixed4 c)
        {
            // Convert to HSL
            float3 HSL = RGBtoHSL(c.rgb);

            // Set aturation based on shader input 
            HSL.g = _Saturation;

            // COnvert back to rgb 
            c.rgb = HSLtoRGB(HSL);
        }

        // Generic surface shader
        void surf(Input IN, inout SurfaceOutputStandard o) {

                fixed4 albedoColor = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = albedoColor.rgb;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}