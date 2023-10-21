Shader "Hidden/PostProcess/S_TintEff"
{
    HLSLINCLUDE
    #include "Packages/com.yetman.render-pipelines.universal.postprocess/ShaderLibrary/Core.hlsl"

    TEXTURE2D_X(_MainTex);
    float4 _TintColor;
    float _Intensity;
    float2 _TintPosition; // 색조를 적용할 위치

    float4 FullScreenFragment (PostProcessVaryings input) : SV_Target
    {
        // Code for XR support
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);
        
        // Write your shader code here.
        //float4 color = LOAD_TEXTURE2D_X(_MainTex, uv * _ScreenSize.xy);
        //color.rgb = lerp(color.rgb, _TintColor.rgb, _Intensity);
        
        // 색조를 적용할 위치 근처만 처리
        float4 color = LOAD_TEXTURE2D_X(_MainTex, uv * _ScreenSize.xy);
        float2 position = float2(uv.x * _ScreenSize.x, uv.y * _ScreenSize.y);
        float2 positionDiff = abs(position - _TintPosition * _ScreenSize.xy);

        if (positionDiff.x < 10 && positionDiff.y < 10) {
            color.rgb = lerp(color.rgb, _TintColor.rgb, _Intensity);
        }

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
