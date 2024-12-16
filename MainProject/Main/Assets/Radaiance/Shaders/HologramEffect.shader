Shader "Custom/AdvancedHologram"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _HologramColor ("Hologram Color", Color) = (0.1, 0.7, 1, 1)
        _HologramStrength ("Hologram Strength", Range(0, 1)) = 1
        _NoiseSpeed ("Noise Speed", Range(0.1, 10)) = 2
        _LineFrequency ("Line Frequency", Range(1, 50)) = 10
        _BlendProgress ("Blend Progress", Range(0, 1)) = 0
        _TargetTex ("Target Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float worldY : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _TargetTex;
            float4 _HologramColor;
            float _HologramStrength;
            float _NoiseSpeed;
            float _LineFrequency;
            float _BlendProgress;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldY = v.vertex.y;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float noise = frac(sin(i.worldY * _LineFrequency + _Time.y * _NoiseSpeed) * 43758.5453);
                float lines = step(0.5, noise);

                fixed4 hologramColor = _HologramColor * lines * _HologramStrength;

                fixed4 hologramTex = lerp(tex2D(_MainTex, i.uv), hologramColor, _HologramStrength);

                fixed4 targetTex = tex2D(_TargetTex, i.uv);

                return lerp(hologramTex, targetTex, _BlendProgress);
            }
            ENDCG
        }
    }
}
