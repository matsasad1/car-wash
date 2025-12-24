Shader "UI/RectangularOverlay"
{
    Properties
    {
        _MainTex ("Dummy Main Tex", 2D) = "white" {} // <- Add this line
        _OverlayColor ("Overlay Color", Color) = (0,0,0,0.7)
        _MaskPos ("Hole Center (UV)", Vector) = (0.5,0.5,0,0)
        _MaskSize ("Hole Size (X,Y)", Vector) = (0.2,0.2,0,0)
        _CornerRadius ("Corner Radius (Rect only)", Range(0,0.5)) = 0.0
        _EdgeSoftness ("Edge Softness", Range(0,0.1)) = 0.01
        _MaskShape ("Mask Shape (0=Rect, 1=Circle)", Int) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
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
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            sampler2D _MainTex; // <- Add this line

            fixed4 _OverlayColor;
            float4 _MaskPos;
            float4 _MaskSize;
            float _CornerRadius;
            float _EdgeSoftness;
            int _MaskShape;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float RectSDF(float2 uv, float2 center, float2 halfSize, float radius)
            {
                float2 d = abs(uv - center) - (halfSize - radius);
                return length(max(d, 0.0)) - radius;
            }

            float CircleSDF(float2 uv, float2 center, float radius, float2 aspect)
            {
                float2 uvScaled = (uv - center) * aspect;
                return length(uvScaled) - radius;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 halfSize = _MaskSize.xy * 0.5;

                float dist;

                if (_MaskShape == 0)
                {
                    dist = RectSDF(uv, _MaskPos.xy, halfSize, _CornerRadius);
                }
                else
                {
                    float2 aspect = float2(_ScreenParams.x / _ScreenParams.y, 1.0);
                    float radius = min(halfSize.x, halfSize.y);
                    dist = CircleSDF(uv, _MaskPos.xy, radius, aspect);
                }

                float alpha;
                if (_EdgeSoftness > 0.0)
                {
                    float t = smoothstep(0.0, _EdgeSoftness, dist);
                    alpha = lerp(0.0, _OverlayColor.a, t);
                }
                else
                {
                    alpha = (dist <= 0.0) ? 0.0 : _OverlayColor.a;
                }

                return fixed4(_OverlayColor.rgb, alpha);
            }
            ENDCG
        }
    }
}
