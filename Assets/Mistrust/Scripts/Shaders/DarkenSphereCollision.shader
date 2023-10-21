Shader "Custom/DarkenSphereCollision"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SphereCenter ("Sphere Center", Vector) = (0, 0, 0, 0)
        _SphereRadius ("Sphere Radius", Range(0, 10)) = 1
        _DarkenIntensity ("Darken Intensity", Range(0, 1)) = 0.5
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
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float4 _SphereCenter;
            float _SphereRadius;
            float _DarkenIntensity;
            sampler2D _MainTex;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                // ���� ��ǥ���� ��ġ ���
                float4 worldPos = mul(unity_ObjectToWorld, i.vertex);
                float3 diff = worldPos.xyz - _SphereCenter.xyz;
                
                // �� ������ �ȼ����� Ȯ��
                if (length(diff) <= _SphereRadius)
                {
                    // �� ������ �ȼ��� ��� ��Ӱ� �����
                    half4 color = tex2D(_MainTex, i.uv);
                    color.rgb -= _DarkenIntensity;
                    return color;
                }
                else
                {
                    // �� ���ΰ� �ƴ� ��� �ؽ�ó �״�� ���
                    return tex2D(_MainTex, i.uv);
                }
            }
            ENDCG
        }
    }
}