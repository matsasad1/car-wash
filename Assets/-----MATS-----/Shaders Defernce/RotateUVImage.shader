Shader "UI/RotateUVImage"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Rotation ("Rotation (Degrees)", Range(0,360)) = 0
        _Scroll ("UV Scroll", Vector) = (0, 0, 0, 0)
        _Color ("Tint", Color) = (1,1,1,1)
        _FadeStart ("Fade Start (Y)", Range(0,1)) = 0
        _FadeEnd ("Fade End (Y)", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
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
            float _Rotation;
            float4 _Scroll;
            fixed4 _Color;
            float _FadeStart;
            float _FadeEnd;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv     : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // convert degrees to radians
                float rad = radians(_Rotation);

                // rotation matrix
                float2x2 rot = float2x2(cos(rad), -sin(rad),
                                        sin(rad),  cos(rad));

                // center uv, rotate, re-center
                float2 uv = i.uv - 0.5;
                uv = mul(rot, uv);
                uv += 0.5;

                // add optional scroll
                uv += _Scroll.xy;

                fixed4 col = tex2D(_MainTex, uv) * _Color;

                // vertical fade factor
                float fade = saturate((i.uv.y - _FadeStart) / max(0.0001, (_FadeEnd - _FadeStart)));

                // apply fade to alpha
                col.a *= fade;

                return col;
            }
            ENDCG
        }
    }
}
