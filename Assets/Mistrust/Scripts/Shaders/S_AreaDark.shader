Shader "Hidden/PostProcess/S_AreaDark"
{
    HLSLINCLUDE
    #include "Packages/com.yetman.render-pipelines.universal.postprocess/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);
    //float4 _TintColor;
    //float _Intensity;

    float2 _DarkenPosition; // 어둡게 만들고 싶은 좌표
    float _DarkenIntensity; // 어둡게 할 강도
    float _Distance;
    float2 _DarkenUV;

    float4 FullScreenFragment (PostProcessVaryings input) : SV_Target
    {
        // Code for XR support
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

                // 현재 픽셀의 위치
        //float2 currentPos = uv;
        //        // 어둡게 만들고 싶은 좌표와 현재 픽셀 간의 거리 계산
        //float distance = length(currentPos - _DarkenPosition);

        float distance = _Distance;

                // 거리가 작을수록 어둡게 만들 강도가 커짐
        float darkenAmount = saturate(1.0 - distance) * _DarkenIntensity;
                // 원본 픽셀 색상
        //half4 color = LOAD_TEXTURE2D_X(_MainTex, uv * _ScreenSize.xy);
        half4 color = LOAD_TEXTURE2D_X(_MainTex, _DarkenUV);

        // 어둡게 만들기
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
