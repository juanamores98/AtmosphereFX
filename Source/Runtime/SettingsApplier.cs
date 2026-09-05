using UnityEngine;
using AtmosphereFX.Config;

namespace AtmosphereFX.Runtime
{
    /// <summary>
    /// Pushes the v2 configuration into the game's render components.
    /// All mappings are direct; there are no intermediate transforms.
    /// </summary>
    internal static class SettingsApplier
    {
        internal static void ApplyAll()
        {
            ApplyDynamicFog();
            ApplyCubemapFog();
            ApplyDynamicFogEffect();
            ApplyRenderProperties();

            PerFrameWatcher.SunMatchedScatter = ModConfig.ScatterColorMode == 1;
            PerFrameWatcher.OffAtNight = ModConfig.OffAtNight;
        }

        internal static void ApplyDynamicFog()
        {
            var fog = Object.FindObjectOfType<FogProperties>();
            if (fog == null)
            {
                return;
            }

            fog.m_ColorDecay = ModConfig.ColorDecay;
            fog.m_FogDensity = ModConfig.Density;
            fog.m_NoiseContribution = ModConfig.Noise;
            fog.m_edgeFog = ModConfig.EdgeFog;
            fog.m_FogHeight = (int)ModConfig.FogHeight;
            fog.m_HorizonHeight = (int)ModConfig.HorizonHeight;
            fog.m_FogStart = (int)ModConfig.StartDistance;
            fog.m_WindSpeed = ModConfig.WindSpeed;
        }

        internal static void ApplyCubemapFog()
        {
            var fog = Object.FindObjectOfType<FogEffect>();
            if (fog == null)
            {
                return;
            }

            fog.enabled = ModConfig.CubemapFog;
            fog.m_edgeFog = ModConfig.EdgeFog;
        }

        internal static void ApplyDynamicFogEffect()
        {
            var effect = Object.FindObjectOfType<DayNightFogEffect>();
            if (effect != null)
            {
                effect.enabled = ModConfig.DynamicFog;
            }
        }

        internal static void ApplyRenderProperties()
        {
            var props = Object.FindObjectOfType<RenderProperties>();
            if (props == null)
            {
                return;
            }

            props.m_useVolumeFog = ModConfig.VolumeFog;
            props.m_inscatteringExponent = ModConfig.ScatterFalloff;
            props.m_inscatteringIntensity = ModConfig.ScatterStrength;
            props.m_inscatteringColor = ModConfig.ResolveScatterColor();
            props.m_volumeFogColor = ModConfig.ResolveVolumeColor();
            props.m_volumeFogStart = ModConfig.VolumeStart;
        }
    }
}
