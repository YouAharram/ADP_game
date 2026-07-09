Shader "Custom/WaterWaves"
{
    Properties
    {
        _ColorDeep ("Deep Water Color", Color) = (0.02, 0.25, 0.45, 1)
        _ColorShallow ("Shallow Water Color", Color) = (0.15, 0.55, 0.65, 1)
        _FoamColor ("Foam Color", Color) = (0.9, 0.98, 1, 1)
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveScale ("Wave Scale", Float) = 6.0
        _WaveStrength ("Wave Strength", Range(0,1)) = 0.35
        _Circular ("Circular Pool Mask (0 = off, 1 = on)", Float) = 0
        _Radius ("Pool Radius (UV space, 0-0.5)", Range(0,0.5)) = 0.45
        _EdgeSoftness ("Edge / Foam Softness", Range(0.001,0.3)) = 0.08
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "WaterUnlit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _ColorDeep;
            float4 _ColorShallow;
            float4 _FoamColor;
            float _WaveSpeed;
            float _WaveScale;
            float _WaveStrength;
            float _Circular;
            float _Radius;
            float _EdgeSoftness;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = _Time.y * _WaveSpeed;

                // layered sine waves to fake ripples
                float w1 = sin((uv.x + uv.y) * _WaveScale + t) * 0.5 + 0.5;
                float w2 = sin((uv.x - uv.y) * _WaveScale * 1.7 - t * 1.3) * 0.5 + 0.5;
                float wave = saturate((w1 * 0.6 + w2 * 0.4));

                half4 col = lerp(_ColorDeep, _ColorShallow, wave * _WaveStrength + (1 - _WaveStrength));

                // subtle sparkle foam lines
                float foamLine = smoothstep(0.85, 1.0, w1 * w2);
                col.rgb = lerp(col.rgb, _FoamColor.rgb, foamLine * 0.25);

                float alpha = 1.0;

                if (_Circular > 0.5)
                {
                    float dist = distance(uv, float2(0.5, 0.5));
                    float mask = 1.0 - smoothstep(_Radius - _EdgeSoftness, _Radius, dist);
                    // foam ring right at the pool edge
                    float ring = smoothstep(_Radius - _EdgeSoftness * 1.4, _Radius - _EdgeSoftness, dist)
                               - smoothstep(_Radius - _EdgeSoftness * 0.4, _Radius, dist);
                    col.rgb = lerp(col.rgb, _FoamColor.rgb, saturate(ring) * 0.9);
                    alpha = mask;
                }

                return half4(col.rgb, alpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
