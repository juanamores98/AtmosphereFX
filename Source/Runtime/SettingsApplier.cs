using UnityEngine;
using AtmosphereFX.Config;

namespace AtmosphereFX.Runtime
{
    /// <summary>
    /// Pushes the v2 configuration into the game's render components.
    /// All mappings are direct; there are no intermediate transforms.
    /// Cached component references avoid per-invocation FindObjectOfType.
    /// </summary>
    internal static class SettingsApplier
    {
        private static FogProperties _fogProperties;
        private static FogEffect _cubemapFog;
        private static DayNightFogEffect _dynamicEffect;
        private static RenderProperties _renderProperties;

        public static void ClearCache()
        {
            _fogProperties = null;
            _cubemapFog = null;
            _dynamicEffect = null;
            _renderProperties = null;
        }

        internal static void ApplyAll()
        {
            VanillaSnapshot.Capture();
            ApplyDynamicFog();
            ApplyCubemapFog();
            ApplyDynamicFogEffect();
            ApplyRenderProperties();

            PerFrameWatcher.SunMatchedScatter = ModConfig.ScatterColorMode == 1;
            PerFrameWatcher.OffAtNight = ModConfig.OffAtNight;
        }

        internal static void ApplyDynamicFog()
        {
            VanillaSnapshot.Capture();
            if (_fogProperties == null)
            {
                _fogProperties = Object.FindObjectOfType<FogProperties>();
                if (_fogProperties == null)
                {
                    return;
                }
            }

            _fogProperties.m_ColorDecay = ModConfig.ColorDecay;
            _fogProperties.m_FogDensity = ModConfig.Density;
            _fogProperties.m_NoiseContribution = ModConfig.Noise;
            _fogProperties.m_edgeFog = ModConfig.EdgeFog;
            _fogProperties.m_FogHeight = (int)ModConfig.FogHeight;
            _fogProperties.m_HorizonHeight = (int)ModConfig.HorizonHeight;
            _fogProperties.m_FogStart = (int)ModConfig.StartDistance;
            _fogProperties.m_WindSpeed = ModConfig.WindSpeed;
        }

        internal static void ApplyCubemapFog()
        {
            VanillaSnapshot.Capture();
            if (_cubemapFog == null)
            {
                _cubemapFog = Object.FindObjectOfType<FogEffect>();
                if (_cubemapFog == null)
                {
                    return;
                }
            }

            _cubemapFog.enabled = ModConfig.CubemapFog;
            _cubemapFog.m_edgeFog = ModConfig.EdgeFog;
        }

        internal static void ApplyDynamicFogEffect()
        {
            VanillaSnapshot.Capture();
            if (_dynamicEffect == null)
            {
                _dynamicEffect = Object.FindObjectOfType<DayNightFogEffect>();
                if (_dynamicEffect == null)
                {
                    return;
                }
            }

            _dynamicEffect.enabled = ModConfig.DynamicFog;
        }

        internal static void ApplyRenderProperties()
        {
            VanillaSnapshot.Capture();
            if (_renderProperties == null)
            {
                _renderProperties = Object.FindObjectOfType<RenderProperties>();
                if (_renderProperties == null)
                {
                    return;
                }
            }

            _renderProperties.m_useVolumeFog = ModConfig.VolumeFog;
            _renderProperties.m_inscatteringExponent = ModConfig.ScatterFalloff;
            _renderProperties.m_inscatteringIntensity = ModConfig.ScatterStrength;
            _renderProperties.m_inscatteringColor = ModConfig.ResolveScatterColor();
            _renderProperties.m_volumeFogColor = ModConfig.ResolveVolumeColor();
            _renderProperties.m_volumeFogStart = ModConfig.VolumeStart;
        }

        /// <summary>
        /// Puts every touched component back to the exact state the game
        /// shipped with (captured before the first modification).
        /// </summary>
        internal static void RestoreGameDefaults()
        {
            VanillaSnapshot.Restore();
        }
    }
}