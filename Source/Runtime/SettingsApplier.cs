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
        internal static bool Active;

        private static FogProperties _fogProperties;
        private static FogEffect _cubemapFog;
        private static DayNightFogEffect _dynamicEffect;
        private static RenderProperties _renderProperties;

        public static void ClearCache()
        {
            Active = false;
            _fogProperties = null;
            _cubemapFog = null;
            _dynamicEffect = null;
            _renderProperties = null;
        }

        internal static void ApplyAll()
        {
            if (ModConfig.VanillaMode) return;
            Active = true;
            VanillaSnapshot.Capture();
            ApplyDynamicFog();
            ApplyCubemapFog();
            ApplyDynamicFogEffect();
            ApplyRenderProperties();
        }

        internal static void ApplyDynamicFog()
        {
            if (ModConfig.VanillaMode) return;
            Active = true;
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
            _fogProperties.m_edgeFog = ModConfig.EdgeFogDynamic;
            _fogProperties.m_FogHeight = (int)ModConfig.FogHeight;
            _fogProperties.m_HorizonHeight = (int)ModConfig.HorizonHeight;
            _fogProperties.m_FogStart = (int)ModConfig.StartDistance;
            _fogProperties.m_WindSpeed = ModConfig.WindSpeed;
        }

        internal static void ApplyCubemapFog()
        {
            if (ModConfig.VanillaMode) return;
            Active = true;
            VanillaSnapshot.Capture();
            if (_cubemapFog == null)
            {
                _cubemapFog = Object.FindObjectOfType<FogEffect>();
                if (_cubemapFog == null)
                {
                    return;
                }
            }

            _cubemapFog.enabled = ModConfig.CubemapFog
                && (!ModConfig.OffAtNight || SimulationManager.instance == null || !SimulationManager.instance.m_isNightTime);
            _cubemapFog.m_edgeFog = ModConfig.EdgeFogCubemap;
            if (ModConfig.StaticHeight >= 0f) _cubemapFog.m_FogHeight = ModConfig.StaticHeight;
            if (ModConfig.StaticStart >= 0f) _cubemapFog.m_3DFogStart = ModConfig.StaticStart;
            if (ModConfig.StaticDistance >= 0f) _cubemapFog.m_3DFogDistance = ModConfig.StaticDistance;
            if (ModConfig.StaticEdgeDistance >= 0f) _cubemapFog.m_edgeFogDistance = ModConfig.StaticEdgeDistance;
            if (ModConfig.StaticVolumeFog >= 0) _cubemapFog.m_UseVolumeFog = ModConfig.StaticVolumeFog == 1;
        }

        internal static void ApplyDynamicFogEffect()
        {
            if (ModConfig.VanillaMode) return;
            Active = true;
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
            if (ModConfig.VanillaMode) return;
            Active = true;
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
            if (ModConfig.VolumeHeight >= 0f) _renderProperties.m_fogHeight = ModConfig.VolumeHeight;
            if (ModConfig.VolumeDensity >= 0f) _renderProperties.m_volumeFogDensity = ModConfig.VolumeDensity;
            if (ModConfig.VolumeDistance >= 0f) _renderProperties.m_volumeFogDistance = ModConfig.VolumeDistance;
            if (ModConfig.VolumeEdgeDistance >= 0f) _renderProperties.m_edgeFogDistance = ModConfig.VolumeEdgeDistance;
        }

        /// <summary>
        /// Puts every touched component back to the exact state the game
        /// shipped with (captured before the first modification).
        /// </summary>
        internal static void RestoreGameDefaults()
        {
            if (Active) VanillaSnapshot.Restore();
            Active = false;
            VanillaSnapshot.ResetCapture();
        }
    }
}