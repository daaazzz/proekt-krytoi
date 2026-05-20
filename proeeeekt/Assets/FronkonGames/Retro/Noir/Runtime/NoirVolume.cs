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
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace FronkonGames.Retro.Noir
{
  /// <summary> Noir Volume. </summary>
  [Serializable, VolumeComponentMenu("Fronkon Games/Retro/Noir")]
  public sealed class NoirVolume : VolumeComponent, IPostProcessComponent
  {
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Common settings.

    /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
    /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Controls the intensity of the effect [0, 1]. Default 1.")]
    public FloatSliderParameterLinear intensity = new(1.0f, 0.0f, 1.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Noir settings.

    /// <summary> The method of the noir effect, default Dither. </summary>
    [EnumDropdown((int)NoirMethod.Dither, "The method of the noir effect, default Dither.")]
    public EnumParameterNoInterpolation<NoirMethod> method = new(NoirMethod.Dither);

    /// <summary> The intensity of the noir effect, default 1.0 [0.0 - 1.0]. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "The intensity of the noir effect, default 1.0 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation noirIntensity = new(1.0f, 0.0f, 1.0f);

    /// <summary> The spread of the dither, default 1.0 [0.0 - 1.0]. </summary>
    /// <remarks> Only available for Bayer method. </remarks>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "The spread of the dither, default 1.0 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation ditherSpread = new(1.0f, 0.0f, 1.0f);

    /// <summary> The density of the dither, default 1.0 [0.0 - 1.0]. </summary>
    /// <remarks> Only available for Bayer method. </remarks>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "The density of the dither, default 1.0 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation ditherDensity = new(1.0f, 0.0f, 1.0f);

    /// <summary> The color blend of the dither, default Solid. </summary>
    /// <remarks> Only available for Bayer method. </remarks>
    [EnumDropdown((int)ColorBlends.Solid, "The color blend of the dither, default Solid.")]
    public EnumParameterNoInterpolation<ColorBlends> ditherColorBlend = new(ColorBlends.Solid);

    /// <summary> The size of the dot screen grid, default 92 [8 - 256]. </summary>
    /// <remarks> Only available for DotScreen method. </remarks>
    [IntSliderWithReset(92, 8, 256, "The size of the dot screen grid, default 92 [8 - 256].")]
    public ClampedIntParameterNoInterpolation dotScreenGridSize = new(92, 8, 256);

    /// <summary> The luminance gain of the dot screen, default 0.3 [0.0 - 1.0]. </summary>
    /// <remarks> Only available for DotScreen method. </remarks>
    [FloatSliderWithReset(0.3f, 0.0f, 1.0f, "The luminance gain of the dot screen, default 0.3 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation dotScreenLuminanceGain = new(0.3f, 0.0f, 1.0f);

    /// <summary> The color blend of the dot screen, default Multiply. </summary>
    /// <remarks> Only available for DotScreen method. </remarks>
    [EnumDropdown((int)ColorBlends.Multiply, "The color blend of the dot screen, default Multiply.")]
    public EnumParameterNoInterpolation<ColorBlends> dotScreenColorBlend = new(ColorBlends.Multiply);

    /// <summary> The size of the halftone, default 2.0 [0.0 - 10.0]. </summary>
    /// <remarks> Only available for Halftone method. </remarks>
    [FloatSliderWithReset(2.0f, 0.0f, 10.0f, "The size of the halftone, default 2.0 [0.0 - 10.0].")]
    public FloatSliderParameterNoInterpolation halftoneSize = new(2.0f, 0.0f, 10.0f);

    /// <summary> The angle of the halftone, default 30.0 [0.0 - 360.0]. </summary>
    /// <remarks> Only available for Halftone method. </remarks>
    [FloatSliderWithReset(30.0f, 0.0f, 360.0f, "The angle of the halftone, default 30.0 [0.0 - 360.0].")]
    public FloatSliderParameterNoInterpolation halftoneAngle = new(30.0f, 0.0f, 360.0f);

    /// <summary> The threshold of the halftone, default 0.75 [0.0 - 1.0]. </summary>
    /// <remarks> Only available for Halftone method. </remarks>
    [FloatSliderWithReset(0.75f, 0.0f, 1.0f, "The threshold of the halftone, default 0.75 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation halftoneThreshold = new(0.75f, 0.0f, 1.0f);

    /// <summary> The color blend of the halftone, default Solid. </summary>
    /// <remarks> Only available for Halftone method. </remarks>
    [EnumDropdown((int)ColorBlends.Solid, "The color blend of the halftone, default Solid.")]
    public EnumParameterNoInterpolation<ColorBlends> halftoneColorBlend = new(ColorBlends.Solid);

    /// <summary> The count of the lines, default 4 [1 - 10]. </summary>
    /// <remarks> Only available for Lines method. </remarks>
    [IntSliderWithReset(4, 1, 10, "The count of the lines, default 4 [1 - 10].")]
    public ClampedIntParameterNoInterpolation linesCount = new(4, 1, 10);

    /// <summary> The granularity of the lines, default 0.25 [0.0 - 1.0]. </summary>
    /// <remarks> Only available for Lines method. </remarks>
    [FloatSliderWithReset(0.25f, 0.0f, 1.0f, "The granularity of the lines, default 0.25 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation linesGranularity = new(0.25f, 0.0f, 1.0f);

    /// <summary> The threshold of the lines, default 0.5 [0.0 - 1.0]. </summary>
    /// <remarks> Only available for Lines method. </remarks>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "The threshold of the lines, default 0.5 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation linesThreshold = new(0.5f, 0.0f, 1.0f);

    /// <summary> The color blend of the lines, default Solid. </summary>
    /// <remarks> Only available for Lines method. </remarks>
    [EnumDropdown((int)ColorBlends.Solid, "The color blend of the lines, default Solid.")]
    public EnumParameterNoInterpolation<ColorBlends> linesColorBlend = new(ColorBlends.Solid);

    /// <summary> The intensity of the duo tone, default 1.0 [0.0 - 1.0]. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "The intensity of the duo tone, default 1.0 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation duoToneIntensity = new(1.0f, 0.0f, 1.0f);

    /// <summary> The color of the brightness, default white. </summary>
    [ColorWithReset(0xFFFFFFFF, "The color of the brightness, default white.")]
    public ColorParameterNoInterpolation duoToneBrightnessColor = new(Color.white);

    /// <summary> The color of the darkness, default black (ish). </summary>
    [ColorWithReset(0x0D0D0DFF, "The color of the darkness, default black (ish).")]
    public ColorParameterNoInterpolation duoToneDarknessColor = new(DefaultDuoToneDarknessColor);

    /// <summary> The threshold between black and white, default 0.5 [0.0 - 1.0]. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "The threshold between black and white, default 0.5 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation duoToneThreshold = new(0.5f, 0.0f, 1.0f);

    /// <summary> The minimum range of the luminance, default 0.0 [0.0 - 1.0]. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "The minimum range of the luminance, default 0.0 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation duoToneLuminanceMinRange = new(0.0f, 0.0f, 1.0f);

    /// <summary> The maximum range of the luminance, default 1.0 [0.0 - 1.0]. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "The maximum range of the luminance, default 1.0 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation duoToneLuminanceMaxRange = new(1.0f, 0.0f, 1.0f);

    /// <summary> Smooth transition between black and white, default 0.5 [0.0 - 1.0]. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "Smooth transition between black and white, default 0.5 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation duoToneSoftness = new(0.5f, 0.0f, 1.0f);

    /// <summary> The amount of light, default 1.0 [0.0 - 5.0]. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 5.0f, "The amount of light, default 1.0 [0.0 - 5.0].")]
    public FloatSliderParameterNoInterpolation duoToneExposure = new(1.0f, 0.0f, 5.0f);

    /// <summary> The intensity of the emboss dark, default 0.05 [0.0 - 1.0]. </summary>
    [FloatSliderWithReset(0.05f, 0.0f, 1.0f, "The intensity of the emboss dark, default 0.05 [0.0 - 1.0].")]
    public FloatSliderParameterNoInterpolation duoToneEmbossDark = new(0.05f, 0.0f, 1.0f);

    /// <summary> The color blend of the duo tone, default Solid. </summary>
    [EnumDropdown((int)ColorBlends.Solid, "The color blend of the duo tone, default Solid.")]
    public EnumParameterNoInterpolation<ColorBlends> duoToneColorBlend = new(ColorBlends.Solid);

    /// <summary> Controls the intensity of the Sepia effect [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Controls the intensity of the Sepia effect [0, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation sepiaIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Controls the intensity of the Chromatic Aberration effect [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Controls the intensity of the Chromatic Aberration effect [0, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation chromaticAberrationIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Controls the intensity of the Blotches effect [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Controls the intensity of the Blotches effect [0, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation blotchesIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Speed of the blotches effect [0, 33]. Default 10. </summary>
    [FloatSliderWithReset(10.0f, 0.0f, 33.0f, "Speed of the blotches effect [0, 33]. Default 10.")]
    public FloatSliderParameterNoInterpolation blotchesSpeed = new(10.0f, 0.0f, 33.0f);

    /// <summary> Intensity of the vignette effect [0, 1]. Default 1.0. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 1.0f, "Intensity of the vignette effect [0, 1]. Default 1.0.")]
    public FloatSliderParameterNoInterpolation vignetteIntensity = new(1.0f, 0.0f, 1.0f);

    /// <summary> Size of the vignette's clear central area [0, 2]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Size of the vignette's clear central area [0, 2]. Default 1.")]
    public FloatSliderParameterNoInterpolation vignetteSize = new(1.0f, 0.0f, 2.0f);

    /// <summary> Color of the vignette. Alpha component of color can influence overall vignette visibility. Default black. </summary>
    [ColorWithReset(0x000000FF, "Color of the vignette. Alpha component of color can influence overall vignette visibility. Default black.")]
    public ColorParameterNoInterpolation vignetteColor = new(Color.black);

    /// <summary> Smoothness of the vignette falloff [0.01, 1]. Default 0.5. </summary>
    [FloatSliderWithReset(0.5f, 0.01f, 1.0f, "Smoothness of the vignette falloff [0.01, 1]. Default 0.5.")]
    public FloatSliderParameterNoInterpolation vignetteSmoothness = new(0.5f, 0.01f, 1.0f);

    /// <summary> Time variation of the vignette effect [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Time variation of the vignette effect [0, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation vignetteTimeVariation = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity of the film grain effect [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity of the film grain effect [0, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation filmGrainIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Speed of the film grain effect [0, 1]. Default 0.5. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "Speed of the film grain effect [0, 1]. Default 0.5.")]
    public FloatSliderParameterNoInterpolation filmGrainSpeed = new(0.5f, 0.0f, 1.0f);

    /// <summary> Intensity of the scratches effect [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "Intensity of the scratches effect [0, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation scratchesIntensity = new(0.0f, 0.0f, 1.0f);

    /// <summary> Speed of the scratches effect [0, 1]. Default 0.5. </summary>
    [FloatSliderWithReset(0.5f, 0.0f, 1.0f, "Speed of the scratches effect [0, 1]. Default 0.5.")]
    public FloatSliderParameterNoInterpolation scratchesSpeed = new(0.5f, 0.0f, 1.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Color settings.

    /// <summary> Brightness [-1, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, -1.0f, 1.0f, "Brightness [-1, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation brightness = new(0.0f, -1.0f, 1.0f);

    /// <summary> Contrast [0, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 10.0f, "Contrast [0, 10]. Default 1.")]
    public FloatSliderParameterNoInterpolation contrast = new(1.0f, 0.0f, 10.0f);

    /// <summary> Gamma [0.1, 10]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.1f, 10.0f, "Gamma [0.1, 10]. Default 1.")]
    public FloatSliderParameterNoInterpolation gamma = new(1.0f, 0.1f, 10.0f);

    /// <summary> The color wheel [0, 1]. Default 0. </summary>
    [FloatSliderWithReset(0.0f, 0.0f, 1.0f, "The color wheel [0, 1]. Default 0.")]
    public FloatSliderParameterNoInterpolation hue = new(0.0f, 0.0f, 1.0f);

    /// <summary> Intensity of a colors [0, 2]. Default 1. </summary>
    [FloatSliderWithReset(1.0f, 0.0f, 2.0f, "Intensity of a colors [0, 2]. Default 1.")]
    public FloatSliderParameterNoInterpolation saturation = new(1.0f, 0.0f, 2.0f);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Advanced settings.

    /// <summary> Does it affect the Scene View? </summary>
    [ToggleWithReset(false, "Does it affect the Scene View?")]
    public BoolParameterNoInterpolation affectSceneView = new(false);

    /// <summary> Use scaled time. </summary>
    [ToggleWithReset(true, "Use scaled time.")]
    public BoolParameterNoInterpolation useScaledTime = new(true);

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public static readonly Color DefaultDuoToneDarknessColor = new(0.05f, 0.05f, 0.05f, 1.0f);

    /// <summary> Reset to default values. </summary>
    public void Reset()
    {
      intensity.value = 1.0f;

      method.value = NoirMethod.Dither;
      noirIntensity.value = 1.0f;
      ditherSpread.value = 1.0f;
      ditherDensity.value = 1.0f;
      ditherColorBlend.value = ColorBlends.Solid;

      dotScreenGridSize.value = 92;
      dotScreenLuminanceGain.value = 0.3f;
      dotScreenColorBlend.value = ColorBlends.Multiply;

      halftoneSize.value = 2.0f;
      halftoneAngle.value = 30.0f;
      halftoneThreshold.value = 0.75f;
      halftoneColorBlend.value = ColorBlends.Solid;

      linesCount.value = 4;
      linesGranularity.value = 0.25f;
      linesThreshold.value = 0.5f;
      linesColorBlend.value = ColorBlends.Solid;

      duoToneIntensity.value = 1.0f;
      duoToneBrightnessColor.value = Color.white;
      duoToneDarknessColor.value = DefaultDuoToneDarknessColor;
      duoToneThreshold.value = 0.5f;
      duoToneLuminanceMinRange.value = 0.0f;
      duoToneLuminanceMaxRange.value = 1.0f;
      duoToneSoftness.value = 0.5f;
      duoToneExposure.value = 1.0f;
      duoToneEmbossDark.value = 0.05f;
      duoToneColorBlend.value = ColorBlends.Solid;

      sepiaIntensity.value = 0.0f;
      chromaticAberrationIntensity.value = 0.0f;

      blotchesIntensity.value = 0.0f;
      blotchesSpeed.value = 10.0f;

      vignetteColor.value = Color.black;
      vignetteIntensity.value = 1.0f;
      vignetteSize.value = 1.0f;
      vignetteSmoothness.value = 0.5f;
      vignetteTimeVariation.value = 0.0f;

      filmGrainIntensity.value = 0.0f;
      filmGrainSpeed.value = 0.5f;

      scratchesIntensity.value = 0.0f;
      scratchesSpeed.value = 0.5f;

      brightness.value = 0.0f;
      contrast.value = 1.0f;
      gamma.value = 1.0f;
      hue.value = 0.0f;
      saturation.value = 1.0f;

      affectSceneView.value = false;
      useScaledTime.value = true;
    }

    /// <summary> Is the effect active? </summary>
    public bool IsActive() => intensity.overrideState == true && intensity.value > 0.0f;

    /// <summary> Is the effect tile compatible? </summary>
    public bool IsTileCompatible() => false;
  }
}
