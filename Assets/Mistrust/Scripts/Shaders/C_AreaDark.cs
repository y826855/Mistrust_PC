using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.PostProcessing;

namespace CustomPostProcessing {
    
    [System.Serializable, VolumeComponentMenu("Custom Post-processing/C_AreaDark")]
    public class C_AreaDark : VolumeComponent
    {
        public Vector2Parameter DarkPosition = new Vector2Parameter(Vector2.zero);
        public FloatParameter DarkIntensity = new FloatParameter(0);
        public FloatParameter Distance = new FloatParameter(0);
        public Vector2Parameter DarkenUV = new Vector2Parameter(Vector2.zero);
    }
    
    // Defining renderers for custom post-processing effects
    [CustomPostProcess("C_AreaDark", CustomPostProcessInjectionPoint.AfterPostProcess)]
    public class C_AreaDarkRenderer : CustomPostProcessRenderer
    {
        private C_AreaDark _volumeComponent;

        private Material _material;

        private const string ShaderPath = "Hidden/PostProcess/S_AreaDark" ;
        
        // Cache Shader Property ID.
        private static class ShaderIDs {
            internal static readonly int ScreenColor = Shader.PropertyToID("_MainTex");
            internal static readonly int DarkPosition = Shader.PropertyToID("_DarkenPosition");
            internal static readonly int DarkIntensity = Shader.PropertyToID("_DarkenIntensity");
            internal static readonly int Distance = Shader.PropertyToID("_Distance");
            internal static readonly int DarkenUV = Shader.PropertyToID("_DarkenUV");
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
            _volumeComponent = stack.GetComponent<C_AreaDark>();
            
            return _volumeComponent.DarkIntensity.value > 0;
        }

        // The actual rendering run is done here.
        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source,
            RenderTargetIdentifier destination, ref RenderingData renderingData,
            CustomPostProcessInjectionPoint injectionPoint)
        {
            using (new ProfilingScope(cmd, new ProfilingSampler("C_AreaDark")))
            {
               // Source texture settings
               cmd.SetGlobalTexture(ShaderIDs.ScreenColor, source);
                            
                // Set material properties
                if (_material != null)
                {
                    _material.SetVector(ShaderIDs.DarkPosition, _volumeComponent.DarkPosition.value);
                    _material.SetFloat(ShaderIDs.DarkIntensity, _volumeComponent.DarkIntensity.value);
                    _material.SetFloat(ShaderIDs.Distance, _volumeComponent.Distance.value);
                    _material.SetVector(ShaderIDs.DarkenUV, _volumeComponent.DarkenUV.value);
                }

                // Choose one of the two.
                //cmd.Blit(source, destination, _material, 0); //Shader Graph
                //CoreUtils.DrawFullScreen(cmd, _material, destination); // Shader Code
            }
        }
    }
}