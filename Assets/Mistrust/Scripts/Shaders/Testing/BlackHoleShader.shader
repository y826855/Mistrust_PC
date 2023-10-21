Shader "Custom/BlackHoleShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _AbsorptionRate ("Absorption Rate", Range(0, 1)) = 0.5
        _Transparency ("Transparency", Range(0, 1)) = 0.5
    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _AbsorptionRate;
            float _Transparency;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy * 0.5 + 0.5; // Normalize UV coordinates.
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                half4 texColor = tex2D(_MainTex, i.uv);
                half4 absorptionColor = texColor * _AbsorptionRate;
                half4 finalColor = lerp(texColor, absorptionColor, _AbsorptionRate);
                finalColor.a *= _Transparency;
                return finalColor * _Color;
            }
            ENDCG
        }
    }
}
