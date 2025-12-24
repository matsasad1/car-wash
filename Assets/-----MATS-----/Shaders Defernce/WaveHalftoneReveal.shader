Shader "UI/WaveHalftoneReveal"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite", 2D) = "white" {}
        _Color      ("Panel Color", Color) = (1,1,1,1)

        _Progress   ("Reveal Progress", Range(0,1)) = 0

        // Wave controls
        _WaveAmp    ("Wave Amplitude", Float) = 0.05
        _WaveFreq   ("Wave Frequency", Float) = 10.0
        _WaveSpeed  ("Wave Speed", Float) = 1.0
        _WaveSoft   ("Wave Softness", Range(0.001,0.5)) = 0.05

        // Ben Day Dots
        _DotColor   ("Dot Color", Color) = (1,0,0,1)
        _DotDensity ("Dot Density (tiles)", Float) = 12.0
        _DotSize    ("Dot Size", Range(0.01,0.5)) = 0.25
        _DotSoft    ("Dot Edge Softness", Range(0.001,0.5)) = 0.05
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }

        Cull Off ZWrite Off Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;
            fixed4 _DotColor;

            float _Progress;
            float _WaveAmp, _WaveFreq, _WaveSpeed, _WaveSoft;
            float _DotDensity, _DotSize, _DotSoft;

            float4 _ClipRect;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float4 color : COLOR;
                float2 localPos : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                o.localPos = v.uv; // keep 0..1 local space
                return o;
            }

            // Circle SDF
            float sdCircle(float2 p, float r)
            {
                return length(p) - r;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t = _Time.y * _WaveSpeed;

                // ==== Wave Reveal ====
                float wave = i.localPos.y + sin(i.localPos.x * _WaveFreq + t) * _WaveAmp;
                float mask = smoothstep(_Progress - _WaveSoft, _Progress + _WaveSoft, wave);

                // ==== Ben Day Dots ====
                // Tile coordinates
                float2 grid = frac(i.localPos * _DotDensity) - 0.5;
                float d = sdCircle(grid, _DotSize * _Progress);
                float dotMask = 1.0 - smoothstep(0.0, _DotSoft, d);

                float dotInfluence = dotMask * mask;

                // Sample panel texture
                fixed4 panel = tex2D(_MainTex, i.uv) * i.color;

                // Mix dots into panel
                float3 col = lerp(panel.rgb, _DotColor.rgb, dotInfluence);
                float alpha = panel.a * max(mask, dotInfluence * _DotColor.a);

                // UI clipping
                alpha *= UnityGet2DClipping(i.pos.xy, _ClipRect);

                return float4(col, alpha);
            }
            ENDCG
        }
    }
}
