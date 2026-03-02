Shader "UI/RotatableGradientWithAlpha"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (0,0,0,1)
        _Rotation ("Rotation (Degrees)", Float) = 0
        _AlphaThreshold ("Alpha Threshold", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color1;
            fixed4 _Color2;
            float _Rotation;
            float _AlphaThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture's color and alpha
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Only color the non-transparent parts
                if (texColor.a <= _AlphaThreshold)
                {
                    discard; // Skip coloring transparent parts
                }

                // Convert rotation to radians
                float angle = radians(_Rotation);
                float cosA = cos(angle);
                float sinA = sin(angle);

                // Center UV around (0.5, 0.5)
                float2 centeredUV = i.uv - 0.5;

                // Rotate UV
                float2 rotatedUV;
                rotatedUV.x = centeredUV.x * cosA - centeredUV.y * sinA;
                rotatedUV.y = centeredUV.x * sinA + centeredUV.y * cosA;

                // Use the rotated UV's x component for gradient
                float t = saturate(rotatedUV.x + 0.5); // shift to 0..1

                // Interpolate between the two colors
                fixed4 gradientColor = lerp(_Color1, _Color2, t);

                // Preserve the original texture alpha
                gradientColor.a *= texColor.a;

                return gradientColor;
            }
            ENDCG
        }
    }
}