Shader "Custom/Sprite/ColliderReveal2D"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        _RevealCenter("Reveal Center (World XY)", Vector) = (0,0,0,0)
        _RevealRadius("Reveal Radius", Float) = 0.0
        _Feather("Feather Width", Float) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off Lighting Off Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 worldPos : TEXCOORD1; // store world-space XY
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float4 _RevealCenter;
            float _RevealRadius;
            float _Feather;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;

                // convert local vertex position to world position
                float4 world = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = world.xy;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv) * _Color * i.color;

                // ✅ Euclidean distance (circle!)
                float dist = distance(i.worldPos, _RevealCenter.xy);

                // smooth circular edge
                float mask = smoothstep(_RevealRadius, _RevealRadius - _Feather, dist);

                texColor.a *= mask;
                return texColor;
            }
            ENDCG
        }
    }
}
