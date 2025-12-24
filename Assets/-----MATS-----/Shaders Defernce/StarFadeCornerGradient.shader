Shader "UI/StarFadeCornerGradient"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite", 2D) = "white" {}
        _Color      ("Panel Color (multiply)", Color) = (1,1,1,1)
        _StarColor  ("Star Color", Color) = (1,1,1,1)

        // Core control (0 = fully stars, 1 = solid panel)
        _Value      ("Value (Solid->Stars)", Range(0,1)) = 1

        // Directional gradient controls
        _AngleDeg   ("Gradient Angle (deg)", Range(0,360)) = 45
        _CornerUV   ("Corner (UV 0..1)", Vector) = (0,0,0,0) // e.g. (0,0)=bottom-left, (1,1)=top-right
        _GradWidth  ("Gradient Width", Float) = 0.75        // how far the sweep travels across the rect
        _GradSoft   ("Gradient Softness", Range(0.0001,0.5)) = 0.1

        // Star field controls
        _StarScale  ("Star Tile Scale (bigger = more stars)", Float) = 6.0
        _StarSize   ("Star Size (0..1 in cell)", Range(0.0,1.0)) = 0.45
        _StarSoft   ("Star Edge Softness", Range(0.0001,0.5)) = 0.08

        // Standard UI stuff
        [HideInInspector]_StencilComp ("Stencil Comp", Float) = 8
        [HideInInspector]_Stencil     ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp   ("Stencil Op", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask  ("Stencil Read Mask", Float)  = 255
        [HideInInspector]_ColorMask   ("Color Mask", Float) = 15
        [HideInInspector]_UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        [PerRendererData]_MainTex_ST ("", Vector) = (1,1,0,0)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _StarColor;

            float _Value;

            float _AngleDeg;
            float4 _CornerUV; // use xy
            float _GradWidth;
            float _GradSoft;

            float _StarScale;
            float _StarSize;
            float _StarSoft;

            // UI support
            float4 _ClipRect;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float4 color : COLOR;
                float2 screenPos : TEXCOORD1; // for UI clip
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                o.screenPos = v.vertex.xy;
                return o;
            }

            // Smooth min/max helpers for pleasant blends
            float smin(float a, float b, float k)
            {
                float h = saturate(0.5 + 0.5*(b - a)/k);
                return lerp(b, a, h) - k*h*(1.0 - h);
            }

            // Signed distance to a 4-point "star" (diamond) centered at origin, cell space [-0.5..0.5]
            // r in [0..0.5]
            float sdDiamond(float2 p, float r)
            {
                // L1 norm diamond: |x| + |y| - r
                return (abs(p.x) + abs(p.y)) - r;
            }

            // Tile uv into cell space centered at 0 (so each cell runs ~[-0.5..0.5])
            void tileCoords(in float2 uv, in float scale, out float2 cellP)
            {
                float2 t = uv * scale;
                float2 g = frac(t) - 0.5; // center each tile
                cellP = g;
            }

            // Directional gradient (0..1) starting from _CornerUV in direction angle
            float directionalRamp(float2 uv01, float2 corner, float angleDeg, float width, float soft)
            {
                float ang = radians(angleDeg);
                float2 dir = float2(cos(ang), sin(ang));

                float proj = dot(uv01 - corner, dir);
                // Normalize by width so proj in [0..1] across the rect
                float r = saturate(proj / max(width, 1e-4));

                // Soft edges
                float a0 = saturate((r - 0.0) / max(soft, 1e-4));
                float a1 = 1.0 - saturate((r - (1.0 - soft)) / max(soft, 1e-4));
                return saturate(a0 * a1); // bell-ish window inside [0..1] with soft edges
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample sprite (keeps sprite alpha/shape & atlasing)
                fixed4 baseCol = tex2D(_MainTex, i.uv) * i.color;

                // Normalized UV 0..1 (no tiling) for procedural fields
                // Assumes the UI Image fills the sprite rect; good enough for UI masks/effects
                float2 uv01 = i.uv;

                // Star field mask (0 outside star, 1 inside), repeated
                float2 cellP; tileCoords(uv01, _StarScale, cellP);
                float r = saturate(_StarSize) * 0.5; // keep within cell
                float d = sdDiamond(cellP, r);

                // Soft edge to produce antialiased stars
                float starMask = 1.0 - smoothstep(0.0, max(_StarSoft, 1e-4), d);

                // Directional gradient mask 0..1
                float grad = directionalRamp(uv01, _CornerUV.xy, _AngleDeg, _GradWidth, _GradSoft);

                // Blend logic:
                // _Value=1 -> solid panel; _Value=0 -> stars dominate within the gradient sweep.
                // We let gradient choose where transition happens, and _Value scales its strength.
                // panelMask is stronger where grad is high; starMask appears where grad is high as _Value drops.
                float panelMask = 1.0;
                float starInfluence = (1.0 - _Value) * grad; // only appears along the sweep

                // Compose colors: mix panel color and star color by starInfluence * starMask
                // Keep sprite's original alpha as a ceiling.
                float starBlend = saturate(starInfluence) * starMask;

                // Final color (premultiplied-like blend between panel and stars)
                float3 col = lerp(baseCol.rgb, _StarColor.rgb, starBlend);

                // Alpha: base sprite alpha, but reduced where stars “cut” it (stars can also colorize)
                float alpha = baseCol.a * lerp(1.0, _StarColor.a, starBlend);

                // UI Rect clipping
                #ifdef UNITY_UI_CLIP_RECT
                alpha *= UnityGet2DClipping(i.screenPos, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(alpha - 0.001);
                #endif

                return float4(col, alpha);
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}
