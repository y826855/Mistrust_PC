Shader "Custom/DarkenInsideSphere"
{
    Properties
    {
        _SphereCenter("Sphere Center", Vector) = (0, 0, 0, 1)
        _SphereRadius("Sphere Radius", Range(0, 10)) = 1
        _DarkenColor("Darken Color", Color) = (0, 0, 0, 1)
    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            struct appdata_t
            {
                float4 vertex : POSITION;
            };
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };
 
            float4 _SphereCenter;
            float _SphereRadius;
            float4 _DarkenColor;
 
            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
 
            half4 frag(v2f i) : SV_Target
            {
                float3 toSphereCenter = i.worldPos - _SphereCenter.xyz;
                float distanceToCenter = length(toSphereCenter);
 
                if (distanceToCenter <= _SphereRadius)
                {
                    return _DarkenColor;
                }
                else
                {
                    return half4(1, 1, 1, 1);
                }
            }
            ENDCG
        }
    }
}