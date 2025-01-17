using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.PostProcessing;

namespace CustomPostProcessing {
    
    [System.Serializable, VolumeComponentMenu("Custom Post-processing/C_TintEff")]
    public class C_TintEff : VolumeComponent
    {
        public ColorParameter TintColor = new ColorParameter(Color.white);
        public FloatParameter Intensity = new FloatParameter(0);
        public Vector2Parameter TintPosition = new Vector2Parameter(Vector2.zero);
    }
    
    // Defining renderers for custom post-processing effects
    [CustomPostProcess("C_TintEff", CustomPostProcessInjectionPoint.AfterPostProcess)]
    public class C_TintEffRenderer : CustomPostProcessRenderer
    {
        private C_TintEff _volumeComponent;

        private Material _material;

        private const string ShaderPath = "Hidden/PostProcess/S_TintEff";

        // Cache Shader Property ID.
        private static class ShaderIDs {
            internal static readonly int ScreenColor = Shader.PropertyToID("_MainTex");
            internal static readonly int TintColor = Shader.PropertyToID("_TintColor");
            internal static readonly int Intensity = Shader.PropertyToID("_Intensity");
            internal static readonly int TintPosition = Shader.PropertyToID("_TintPosition");
        }
        
        public override bool visibleInSceneView => true;
        
        // Initialized is called only once before the first render call.
        // So we use it to create our material
        public override void Initialize()
        {
            // You should have your shaders inside the 'Resources' folder.
            _material = CoreUtils.CreateEngineMaterial(ShaderPath);
        }

        // Called for each pair of camera injection points in each frame. Return true if the effect should be rendered for this camera.
        public override bool Setup(ref RenderingData renderingData, CustomPostProcessInjectionPoint injectionPoint)
        {
            VolumeStack stack = VolumeManager.instance.stack;   
            _volumeComponent = stack.GetComponent<C_TintEff>();
            
            return _volumeComponent.Intensity.value > 0;
        }

        // The actual rendering run is done here.
        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source,
            RenderTargetIdentifier destination, ref RenderingData renderingData,
            CustomPostProcessInjectionPoint injectionPoint)
        {
            using (new ProfilingScope(cmd, new ProfilingSampler("C_TintEff")))
            {
               // Source texture settings
               cmd.SetGlobalTexture(ShaderIDs.ScreenColor, source);
                            
                // Set material properties
                if (_material != null)
                {
                    _material.SetColor(ShaderIDs.TintColor, _volumeComponent.TintColor.value);
                    _material.SetFloat(ShaderIDs.Intensity, _volumeComponent.Intensity.value);
                    _material.SetVector(ShaderIDs.TintPosition, _volumeComponent.TintPosition.value);
                }

                // Choose one of the two.
                //cmd.Blit(source, destination, _material, 0); //Shader Graph
                CoreUtils.DrawFullScreen(cmd, _material, destination); // Shader Code
            }
        }
    }
}