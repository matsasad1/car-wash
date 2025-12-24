Shader "UI/WaveStarReveal"
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

        // Stars
        _StarColor  ("Star Color", Color) = (1,1,1,1)
        _StarScale  ("Star Density (tiles)", Float) = 8.0
        _StarSize   ("Star Size", Range(0.01,0.5)) = 0.2
        _StarSoft   ("Star Softness", Range(0.001,0.5)) = 0.1
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
            fixed4 _StarColor;

            float _Progress;
            float _WaveAmp, _WaveFreq, _WaveSpeed, _WaveSoft;
            float _StarScale, _StarSize, _StarSoft;

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

            // Signed distance for a simple 4-point star (diamond)
            float sdDiamond(float2 p, float r)
            {
                return (abs(p.x) + abs(p.y)) - r;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t = _Time.y * _WaveSpeed;

                // ==== Wave Reveal ====
                // Horizontal wipe with sine wave
                float wave = i.localPos.y + sin(i.localPos.x * _WaveFreq + t) * _WaveAmp;
                // Compare with _Progress to reveal
                float mask = smoothstep(_Progress - _WaveSoft, _Progress + _WaveSoft, wave);

                // ==== Stars ====
                // Tile into star field
                float2 tile = frac(i.localPos * _StarScale) - 0.5;
                float starDist = sdDiamond(tile, _StarSize * _Progress); // scale with progress
                float starMask = 1.0 - smoothstep(0.0, _StarSoft, starDist);

                // Combine star mask with wave mask
                float sparkle = starMask * mask;

                // Sample panel texture
                fixed4 panel = tex2D(_MainTex, i.uv) * i.color;

                // Mix stars into panel
                float3 col = lerp(panel.rgb, _StarColor.rgb, sparkle);
                float alpha = panel.a * max(mask, sparkle * _StarColor.a);

                // Clip for UI rect
                alpha *= UnityGet2DClipping(i.pos.xy, _ClipRect);

                return float4(col, alpha);
            }
            ENDCG
        }
    }
}
