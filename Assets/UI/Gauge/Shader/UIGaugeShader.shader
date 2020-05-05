Shader "Unlit/UIGaugeShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "defaulttexture" {}
        _EmptyColor("Empty color", Color) = (1,1,1,1)
        _FillColor("Fill color", Color) = (1,1,1,1)
        _FillRate("Fill rate", Float) = 1.0
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"}

        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Always
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            CBUFFER_START(UnityPerMaterial)
            half4 _EmptyColor;
            half4 _FillColor;
            half _FillRate;
            CBUFFER_END

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (i.uv.x <= _FillRate) {
                    return _FillColor;
                }
                else {
                    return _EmptyColor;
                }
            }
            ENDCG
        }
    }
}
