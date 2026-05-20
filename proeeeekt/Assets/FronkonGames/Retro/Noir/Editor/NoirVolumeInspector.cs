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
using UnityEditor;
using UnityEditor.Rendering;

namespace FronkonGames.Retro.Noir.Editor
{
  /// <summary> Noir Volume inspector. </summary>
  [CustomEditor(typeof(NoirVolume))]
  public class NoirVolumeInspector : Inspector
  {
    protected override void InspectorGUI()
    {
      NoirVolume volume = (NoirVolume)target;

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      DrawFloatSliderWithReset("intensity");

      /////////////////////////////////////////////////
      // Noir.
      /////////////////////////////////////////////////
      Separator();

      DrawEnumDropdownWithReset<NoirMethod>("method");
      IndentLevel++;
      DrawFloatSliderWithReset("noirIntensity", "Intensity");
      switch (volume.method.value)
      {
        case NoirMethod.Dither:
          DrawFloatSliderWithReset("ditherSpread", "Spread");
          DrawFloatSliderWithReset("ditherDensity", "Density");
          DrawEnumDropdownWithReset("ditherColorBlend", "Color blend", ColorBlends.Solid);
          break;
        case NoirMethod.DotScreen:
          DrawIntSliderWithReset("dotScreenGridSize", "Grid size");
          DrawFloatSliderWithReset("dotScreenLuminanceGain", "Luminance gain");
          DrawEnumDropdownWithReset("dotScreenColorBlend", "Color blend", ColorBlends.Multiply);
          break;
        case NoirMethod.Halftone:
          DrawFloatSliderWithReset("halftoneSize", "Size");
          DrawFloatSliderWithReset("halftoneAngle", "Angle");
          DrawFloatSliderWithReset("halftoneThreshold", "Threshold");
          DrawEnumDropdownWithReset("halftoneColorBlend", "Color blend", ColorBlends.Solid);
          break;
        case NoirMethod.Lines:
          DrawIntSliderWithReset("linesCount", "Count");
          DrawFloatSliderWithReset("linesGranularity", "Granularity");
          DrawFloatSliderWithReset("linesThreshold", "Threshold");
          DrawEnumDropdownWithReset("linesColorBlend", "Color blend", ColorBlends.Solid);
          break;
      }
      IndentLevel--;

      DrawFloatSliderWithReset("duoToneIntensity", "Duo tone");
      if (volume.duoToneIntensity.value > 0.0f)
      {
        IndentLevel++;
        DrawColorWithReset("duoToneBrightnessColor", "Brightness color", Color.white);
        DrawColorWithReset("duoToneDarknessColor", "Darkness color", NoirVolume.DefaultDuoToneDarknessColor);
        DrawFloatSliderWithReset("duoToneThreshold", "Threshold");
        
        SerializedDataParameter luminanceMinRange = UnpackParameter("duoToneLuminanceMinRange");
        SerializedDataParameter luminanceMaxRange = UnpackParameter("duoToneLuminanceMaxRange");
        float min = luminanceMinRange.value.floatValue;
        float max = luminanceMaxRange.value.floatValue;
        MinMaxSlider("Luminance range", "Luminance range used to change colors.", ref min, ref max, 0.0f, 1.0f, 0.0f, 1.0f);
        luminanceMinRange.value.floatValue = min;
        luminanceMaxRange.value.floatValue = max;

        DrawFloatSliderWithReset("duoToneSoftness", "Softness");
        DrawFloatSliderWithReset("duoToneExposure", "Exposure");
        DrawFloatSliderWithReset("duoToneEmbossDark", "Emboss dark");
        DrawEnumDropdownWithReset("duoToneColorBlend", "Color blend", ColorBlends.Solid);
        IndentLevel--;
      }

      DrawFloatSliderWithReset("sepiaIntensity", "Sepia");

      DrawFloatSliderWithReset("chromaticAberrationIntensity", "Chromatic aberration");

      DrawFloatSliderWithReset("blotchesIntensity", "Blotches");
      if (volume.blotchesIntensity.value > 0.0f)
      {
        IndentLevel++;
        DrawFloatSliderWithReset("blotchesSpeed", "Speed");
        IndentLevel--;
      }

      DrawFloatSliderWithReset("vignetteIntensity", "Vignette");
      if (volume.vignetteIntensity.value > 0.0f)
      {
        IndentLevel++;
        DrawColorWithReset("vignetteColor", "Color", Color.black);
        DrawFloatSliderWithReset("vignetteSize", "Size");
        DrawFloatSliderWithReset("vignetteSmoothness", "Smoothness");
        DrawFloatSliderWithReset("vignetteTimeVariation", "Time variation");
        IndentLevel--;
      }

      DrawFloatSliderWithReset("filmGrainIntensity", "Film grain");
      if (volume.filmGrainIntensity.value > 0.0f)
      {
        IndentLevel++;
        DrawFloatSliderWithReset("filmGrainSpeed", "Speed");
        IndentLevel--;
      }

      DrawFloatSliderWithReset("scratchesIntensity", "Scratches");
      if (volume.scratchesIntensity.value > 0.0f)
      {
        IndentLevel++;
        DrawFloatSliderWithReset("scratchesSpeed", "Speed");
        IndentLevel--;
      }
    }

    protected override void ResetValues() => ((NoirVolume)target).Reset();

    protected override void CheckForErrors()
    {
      if (Noir.IsInAnyRenderFeatures() == false)
      {
        Separator();
        EditorGUILayout.HelpBox($"Renderer Feature '{Constants.Asset.Name}' not found. You must add it as a Render Feature.", MessageType.Error);
      }
      else
      {
        Noir[] effects = Noir.Instances;
        bool anyEnabled = false;
        for (int i = 0; i < effects.Length; i++)
        {
          if (effects[i].isActive == true)
          {
            anyEnabled = true;
            break;
          }
        }

        if (anyEnabled == false)
        {
          Separator();
          EditorGUILayout.HelpBox($"No Renderer Feature '{Constants.Asset.Name}' is active. You must activate it in the Render Features.", MessageType.Warning);
        }
      }
    }
  }
}
