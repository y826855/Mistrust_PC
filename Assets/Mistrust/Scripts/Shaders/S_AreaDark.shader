Shader "Hidden/PostProcess/S_AreaDark"
{
    HLSLINCLUDE
    #include "Packages/com.yetman.render-pipelines.universal.postprocess/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);
    //float4 _TintColor;
    //float _Intensity;

    float2 _DarkenPosition; // ��Ӱ� ����� ���� ��ǥ
    float _DarkenIntensity; // ��Ӱ� �� ����
    float _Distance;
    float2 _DarkenUV;

    float4 FullScreenFragment (PostProcessVaryings input) : SV_Target
    {
        // Code for XR support
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

                // ���� �ȼ��� ��ġ
        //float2 currentPos = uv;
        //        // ��Ӱ� ����� ���� ��ǥ�� ���� �ȼ� ���� �Ÿ� ���
        //float distance = length(currentPos - _DarkenPosition);

        float distance = _Distance;

                // �Ÿ��� �������� ��Ӱ� ���� ������ Ŀ��
        float darkenAmount = saturate(1.0 - distance) * _DarkenIntensity;
                // ���� �ȼ� ����
        //half4 color = LOAD_TEXTURE2D_X(_MainTex, uv * _ScreenSize.xy);
        half4 color = LOAD_TEXTURE2D_X(_MainTex, _DarkenUV);

        // ��Ӱ� �����
        color.rgb -= darkenAmount;

        return color;

        // Write your shader code here.
        //float4 color = LOAD_TEXTURE2D_X(_MainTex, uv * _ScreenSize.xy);
        //color.rgb = lerp(color.rgb, _TintColor.rgb, _Intensity);
        
        return color;
    }
    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex FullScreenTrianglePostProcessVertexProgram
            #pragma fragment FullScreenFragment
            ENDHLSL
        }
    }
    Fallback Off
}
