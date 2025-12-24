Shader "UI/OverlayWithHole"
{
   Properties
    {
        _OverlayColor ("Overlay Color", Color) = (0,0,0,0.7)
        _MaskPos ("Hole Position", Vector) = (0.5,0.5,0,0) // normalized screen pos
        _MaskScale ("Hole Scale (X,Y)", Vector) = (0.2,0.1,0,0) // ellipse radii
        _EdgeSoftness ("Edge Softness", Float) = 0.0 // optional fade
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

            fixed4 _OverlayColor;
            float4 _MaskPos;
            float4 _MaskScale;
            float _EdgeSoftness;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Normalize distance relative to ellipse radii
                float2 delta = (uv - _MaskPos.xy) / _MaskScale.xy;
                float dist = dot(delta, delta); // ellipse equation

                // Hard edge vs soft edge
                float alpha = (dist < 1.0) ? 0.0 : _OverlayColor.a;

                if (_EdgeSoftness > 0.0)
                {
                    float t = smoothstep(1.0, 1.0 + _EdgeSoftness, dist);
                    alpha = lerp(0.0, _OverlayColor.a, t);
                }

                return fixed4(_OverlayColor.rgb, alpha);
            }
            ENDCG
        }
    }
}
