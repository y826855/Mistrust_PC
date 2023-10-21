Shader "Custom/BlackHoleShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Center ("Center", Vector) = (0.5, 0.5, 0.5, 0.5)
        _Radius ("Radius", Range(0, 10)) = 0.2
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
            float4 _Center;
            float _Radius;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy * 0.5 + 0.5; // Normalize UV coordinates.
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                // Calculate distance to the center.
                float2 uvCenter = _Center.xy * 0.5 + 0.5; // Normalize center coordinates.
                float distanceToCenter = distance(i.uv, uvCenter);
                // Apply the absorption effect based on distance and radius.
                float absorptionRate = 1.0 - distanceToCenter / _Radius;
                absorptionRate = max(absorptionRate, 0); // Clamp to zero.
                half4 texColor = tex2D(_MainTex, i.uv);
                half4 finalColor = texColor * absorptionRate;
                return finalColor * _Color;
            }
            ENDCG
        }
    }
}
