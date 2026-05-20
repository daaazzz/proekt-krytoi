////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace FronkonGames.Retro.Noir
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Noir
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private NoirVolume volume;

      private TextureHandle renderTextureHandle0;
      private TextureHandle renderTextureHandle1;

      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");
        public static readonly int EffectTime = Shader.PropertyToID("_EffectTime");

        public static readonly int NoirIntensity = Shader.PropertyToID("_NoirIntensity");

        public static readonly int DitherSpread = Shader.PropertyToID("_DitherSpread");
        public static readonly int DitherDensity = Shader.PropertyToID("_DitherDensity");
        public static readonly int DitherColorBlend = Shader.PropertyToID("_DitherColorBlend");

        public static readonly int DotScreenGridSize = Shader.PropertyToID("_DotScreenGridSize");
        public static readonly int DotScreenColorBlend = Shader.PropertyToID("_DotScreenColorBlend");
        public static readonly int DotScreenLuminanceGain = Shader.PropertyToID("_DotScreenLuminanceGain");

        public static readonly int HalftoneSize = Shader.PropertyToID("_HalftoneSize");
        public static readonly int HalftoneAngle = Shader.PropertyToID("_HalftoneAngle");
        public static readonly int HalftoneThreshold = Shader.PropertyToID("_HalftoneThreshold");
        public static readonly int HalftoneColorBlend = Shader.PropertyToID("_HalftoneColorBlend");

        public static readonly int LinesCount = Shader.PropertyToID("_LinesCount");
        public static readonly int LinesGranularity = Shader.PropertyToID("_LinesGranularity");
        public static readonly int LinesThreshold = Shader.PropertyToID("_LinesThreshold");
        public static readonly int LinesColorBlend = Shader.PropertyToID("_LinesColorBlend");

        public static readonly int DuoToneIntensity = Shader.PropertyToID("_DuoToneIntensity");
        public static readonly int DuoToneThreshold = Shader.PropertyToID("_DuoToneThreshold");
        public static readonly int DuoToneBrightnessColor = Shader.PropertyToID("_DuoToneBrightnessColor");
        public static readonly int DuoToneDarknessColor = Shader.PropertyToID("_DuoToneDarknessColor");
        public static readonly int DuoToneLuminanceMinRange = Shader.PropertyToID("_DuoToneLuminanceMinRange");
        public static readonly int DuoToneLuminanceMaxRange = Shader.PropertyToID("_DuoToneLuminanceMaxRange");
        public static readonly int DuoToneSoftness = Shader.PropertyToID("_DuoToneSoftness");
        public static readonly int DuoToneExposure = Shader.PropertyToID("_DuoToneExposure");
        public static readonly int DuoToneColorBlend = Shader.PropertyToID("_DuoToneColorBlend");
        public static readonly int DuoToneEmbossDark = Shader.PropertyToID("_DuoToneEmbossDark");

        public static readonly int SepiaIntensity = Shader.PropertyToID("_SepiaIntensity");

        public static readonly int ChromaticAberrationIntensity = Shader.PropertyToID("_ChromaticAberrationIntensity");

        public static readonly int FilmGrainIntensity = Shader.PropertyToID("_FilmGrainIntensity");
        public static readonly int FilmGrainSpeed = Shader.PropertyToID("_FilmGrainSpeed");

        public static readonly int BlotchesIntensity = Shader.PropertyToID("_BlotchesIntensity");
        public static readonly int BlotchesSpeed = Shader.PropertyToID("_BlotchesSpeed");

        public static readonly int VignetteColor = Shader.PropertyToID("_VignetteColor");
        public static readonly int VignetteIntensity = Shader.PropertyToID("_VignetteIntensity");
        public static readonly int VignetteSize = Shader.PropertyToID("_VignetteSize");
        public static readonly int VignetteSmoothness = Shader.PropertyToID("_VignetteSmoothness");
        public static readonly int VignetteTimeVariation = Shader.PropertyToID("_VignetteTimeVariation");

        public static readonly int ScratchesIntensity = Shader.PropertyToID("_ScratchesIntensity");
        public static readonly int ScratchesSpeed = Shader.PropertyToID("_ScratchesSpeed");

        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");      
      }

      private static class Keywords
      {
        public static readonly string Dither    = "DITHER_ON";
        public static readonly string DotScreen = "DOTSCREEN_ON";
        public static readonly string Halftone  = "HALFTONE_ON";
        public static readonly string Lines     = "LINES_ON";

        public static readonly string EmbossDark = "EMBOSS_DARK_ON";

        public static readonly string Sepia = "SEPIA_ON";

        public static readonly string ChromaticAberration = "CHROMATIC_ABERRATION_ON";

        public static readonly string FilmGrain = "FILMGRAIN_ON";

        public static readonly string Blotches = "BLOTCHES_ON";

        public static readonly string Vignette = "VIGNETTE_ON";

        public static readonly string Scratches = "SCRATCHES_ON";
      }
      
      /// <summary> Render pass constructor. </summary>
      public RenderPass() : base()
      {
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
      }

      private void UpdateMaterial()
      {
        if (material != null && volume != null)
        {
          material.shaderKeywords = null;
          material.SetFloat(ShaderIDs.Intensity, volume.intensity.value);

          float time = volume.useScaledTime.value == true ? Time.time : Time.unscaledTime;
          material.SetVector(ShaderIDs.EffectTime, new Vector4(time / 20.0f, time, time * 2.0f, time * 3.0f));

          material.SetFloat(ShaderIDs.NoirIntensity, volume.noirIntensity.value);

          if (volume.noirIntensity.value > 0.0f)
          {
            switch (volume.method.value)
            {
              case NoirMethod.Dither:
                CoreUtils.SetKeyword(material, Keywords.Dither, true);
                material.SetInt(ShaderIDs.DitherColorBlend, (int)volume.ditherColorBlend.value);
                material.SetFloat(ShaderIDs.DitherSpread, 0.9f + 0.1f * volume.ditherSpread.value);
                material.SetFloat(ShaderIDs.DitherDensity, Mathf.Max(volume.ditherDensity.value, 0.05f));
                break;
              case NoirMethod.DotScreen:
                CoreUtils.SetKeyword(material, Keywords.DotScreen, true);
                material.SetInt(ShaderIDs.DotScreenGridSize, volume.dotScreenGridSize.value);
                material.SetInt(ShaderIDs.DotScreenColorBlend, (int)volume.dotScreenColorBlend.value);
                material.SetFloat(ShaderIDs.DotScreenLuminanceGain, volume.dotScreenLuminanceGain.value);
                break;
              case NoirMethod.Halftone:
                CoreUtils.SetKeyword(material, Keywords.Halftone, true);
                material.SetFloat(ShaderIDs.HalftoneSize, volume.halftoneSize.value);
                material.SetFloat(ShaderIDs.HalftoneAngle, volume.halftoneAngle.value * Mathf.Deg2Rad);
                material.SetFloat(ShaderIDs.HalftoneThreshold, volume.halftoneThreshold.value * 0.9f);
                material.SetInt(ShaderIDs.HalftoneColorBlend, (int)volume.halftoneColorBlend.value);
                break;
              case NoirMethod.Lines:
                CoreUtils.SetKeyword(material, Keywords.Lines, true);
                material.SetInt(ShaderIDs.LinesCount, volume.linesCount.value);
                material.SetFloat(ShaderIDs.LinesGranularity, volume.linesGranularity.value);
                material.SetFloat(ShaderIDs.LinesThreshold, volume.linesThreshold.value);
                material.SetInt(ShaderIDs.LinesColorBlend, (int)volume.linesColorBlend.value);
                break;
            }
          }

          if (volume.duoToneIntensity.value > 0.0f)
          {
            material.SetFloat(ShaderIDs.DuoToneIntensity, volume.duoToneIntensity.value);
            material.SetFloat(ShaderIDs.DuoToneThreshold, volume.duoToneThreshold.value);
            material.SetColor(ShaderIDs.DuoToneBrightnessColor, volume.duoToneBrightnessColor.value);
            material.SetColor(ShaderIDs.DuoToneDarknessColor, volume.duoToneDarknessColor.value);
            material.SetFloat(ShaderIDs.DuoToneLuminanceMinRange, volume.duoToneLuminanceMinRange.value);
            material.SetFloat(ShaderIDs.DuoToneLuminanceMaxRange, volume.duoToneLuminanceMaxRange.value);
            material.SetFloat(ShaderIDs.DuoToneSoftness, volume.duoToneSoftness.value);
            material.SetFloat(ShaderIDs.DuoToneExposure, volume.duoToneExposure.value);
            material.SetInt(ShaderIDs.DuoToneColorBlend, (int)volume.duoToneColorBlend.value);

            if (volume.duoToneEmbossDark.value > 0.0f)
            {
              CoreUtils.SetKeyword(material, Keywords.EmbossDark, true);
              material.SetFloat(ShaderIDs.DuoToneEmbossDark, Mathf.Min(volume.duoToneEmbossDark.value, 0.99f));
            }
          }

          if (volume.sepiaIntensity.value > 0.0f)
          {
            CoreUtils.SetKeyword(material, Keywords.Sepia, true);
            material.SetFloat(ShaderIDs.SepiaIntensity, volume.sepiaIntensity.value);
          }

          if (volume.blotchesIntensity.value > 0.0f)
          {
            CoreUtils.SetKeyword(material, Keywords.Blotches, true);
            material.SetFloat(ShaderIDs.BlotchesIntensity, volume.blotchesIntensity.value);
            material.SetFloat(ShaderIDs.BlotchesSpeed, volume.blotchesSpeed.value);
          }

          if (volume.chromaticAberrationIntensity.value > 0.0f)
          {
            CoreUtils.SetKeyword(material, Keywords.ChromaticAberration, true);
            material.SetFloat(ShaderIDs.ChromaticAberrationIntensity, volume.chromaticAberrationIntensity.value * 20.0f);
          }

          if (volume.filmGrainIntensity.value > 0.0f)
          {
            CoreUtils.SetKeyword(material, Keywords.FilmGrain, true);
            material.SetFloat(ShaderIDs.FilmGrainIntensity, volume.filmGrainIntensity.value);
            material.SetFloat(ShaderIDs.FilmGrainSpeed, volume.filmGrainSpeed.value);
          }

          if (volume.scratchesIntensity.value > 0.0f)
          {
            CoreUtils.SetKeyword(material, Keywords.Scratches, true);
            material.SetFloat(ShaderIDs.ScratchesIntensity, volume.scratchesIntensity.value);
            material.SetFloat(ShaderIDs.ScratchesSpeed, volume.scratchesSpeed.value * 10.0f);
          }

          CoreUtils.SetKeyword(material, Keywords.Vignette, volume.vignetteIntensity.value > 0.0f);
          material.SetVector(ShaderIDs.VignetteColor, volume.vignetteColor.value);
          material.SetFloat(ShaderIDs.VignetteIntensity, volume.vignetteIntensity.value);
          material.SetFloat(ShaderIDs.VignetteSmoothness, volume.vignetteSmoothness.value);
          material.SetFloat(ShaderIDs.VignetteSize, volume.vignetteSize.value);
          material.SetFloat(ShaderIDs.VignetteTimeVariation, volume.vignetteTimeVariation.value * 20.0f);

          material.SetFloat(ShaderIDs.Brightness, volume.brightness.value);
          material.SetFloat(ShaderIDs.Contrast, volume.contrast.value);
          material.SetFloat(ShaderIDs.Gamma, 1.0f / volume.gamma.value);
          material.SetFloat(ShaderIDs.Hue, volume.hue.value);
          material.SetFloat(ShaderIDs.Saturation, volume.saturation.value);
        }
      }

      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        volume = VolumeManager.instance.stack.GetComponent<NoirVolume>();
        if (material == null || volume == null || volume.IsActive() == false)
          return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer == true)
          return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (cameraData.camera.cameraType == CameraType.SceneView && volume.affectSceneView.value == false || cameraData.postProcessEnabled == false)
          return;

        TextureHandle source = resourceData.activeColorTexture;
        TextureDesc sourceDesc = source.GetDescriptor(renderGraph);

        UpdateMaterial();

        if (volume.chromaticAberrationIntensity.value > 0.0f)
        {
          renderTextureHandle0 = renderGraph.CreateTexture(sourceDesc);
          renderTextureHandle1 = renderGraph.CreateTexture(sourceDesc);

          renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(source, renderTextureHandle0, material, 0), $"{Constants.Asset.AssemblyName}.Pass0");
          renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(renderTextureHandle0, renderTextureHandle1, material, 1), $"{Constants.Asset.AssemblyName}.Pass1");

          resourceData.cameraColor = renderTextureHandle1;
        }
        else
        {
          TextureHandle destination = renderGraph.CreateTexture(sourceDesc);

          RenderGraphUtils.BlitMaterialParameters pass = new(source, destination, material, 0);
          renderGraph.AddBlitPass(pass, $"{Constants.Asset.AssemblyName}.Pass");

          resourceData.cameraColor = destination;
        }

      }
    }
  }
}
