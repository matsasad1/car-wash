Shader "UI/BenDayDots_AspectCorrect"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _DotColor("Dot Color", Color) = (1,1,1,1)
        _BackgroundColor("Background Color", Color) = (0,0,0,0)
        _DotScale("Dot Scale", Float) = 40.0
        _GradientTex("Gradient Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _DotColor;
            fixed4 _BackgroundColor;
            float _DotScale;
            sampler2D _GradientTex;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Halftone dot pattern
                float2 grid = frac(i.uv * _DotScale);
                float2 center = abs(grid - 0.5);
                float dist = length(center * 2.0);

                // Regular uniform dot threshold
                float dotShape = smoothstep(0.5, 0.45, dist);

                // Sample gradient (assume white=visible, black=transparent)
                fixed4 grad = tex2D(_GradientTex, i.uv);

                // Apply gradient as opacity mask
                float finalAlpha = dotShape * grad.a;

                // Output
                fixed4 col = lerp(_BackgroundColor, _DotColor, finalAlpha);
                col.a = finalAlpha;
                return col;
            }
            ENDCG
        }
    }
}
