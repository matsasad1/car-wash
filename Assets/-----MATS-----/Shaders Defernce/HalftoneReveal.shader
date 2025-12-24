Shader "UI/HalftoneReveal"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite", 2D) = "white" {}
        _Color      ("Panel Color", Color) = (1,1,1,1)

        _Progress   ("Reveal Progress", Range(0,1)) = 0

        // Dots
        _DotColor   ("Dot Color", Color) = (1,0,0,1)
        _DotDensity ("Dot Density (tiles per unit)", Float) = 12.0
        _DotSize    ("Dot Size", Range(0.01,0.5)) = 0.25
        _DotSoft    ("Dot Edge Softness", Range(0.001,0.5)) = 0.05

        // For aspect ratio correction
        _RectSize   ("Rect Size (set by script)", Vector) = (1,1,0,0)
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
            float _DotDensity, _DotSize, _DotSoft;
            float4 _RectSize; // width, height

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
                float2 localUV : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;

                // Normalize into aspect-correct square UV space
                float2 norm = v.uv * float2(_RectSize.x/_RectSize.y, 1.0);
                o.localUV = norm;

                return o;
            }

            float sdCircle(float2 p, float r) { return length(p) - r; }
            float sdDiamond(float2 p, float r) { return (abs(p.x) + abs(p.y)) - r; }

            fixed4 frag(v2f i) : SV_Target
            {
                // Tile coords in aspect-correct space
                float2 grid = frac(i.localUV * _DotDensity) - 0.5;

                // Pick dot type: circle (halftone style) or diamond (star-like)
                float d = sdCircle(grid, _DotSize * _Progress);
                // float d = sdDiamond(grid, _DotSize * _Progress);

                float mask = 1.0 - smoothstep(0.0, _DotSoft, d);

                // Sample panel sprite
                fixed4 panel = tex2D(_MainTex, i.uv) * i.color;

                // Blend panel + dots
                float3 col = lerp(panel.rgb, _DotColor.rgb, mask);
                float alpha = panel.a * lerp(1.0, _DotColor.a, mask);

                // UI clip rect
                alpha *= UnityGet2DClipping(i.pos.xy, _ClipRect);

                return float4(col, alpha);
            }
            ENDCG
        }
    }
}
