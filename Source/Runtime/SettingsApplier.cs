using UnityEngine;
using AtmosphereFX.Infrastructure;
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

        private static bool UseClassicFog
        {
            get
            {
                var sim = SimulationManager.instance;
                return sim != null && !sim.m_isNightTime && (!sim.m_enableDayNight || FxInterop.ClassicRequest("fogWithCycle"));
            }
        }

        public static void ClearCache()
        {
            PropertyLedger.Forget();
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

            PropertyLedger.Write(_fogProperties, "m_ColorDecay", ModConfig.ColorDecay);
            PropertyLedger.Write(_fogProperties, "m_FogDensity", ModConfig.Density);
            PropertyLedger.Write(_fogProperties, "m_NoiseContribution", ModConfig.Noise);
            PropertyLedger.Write(_fogProperties, "m_edgeFog", ModConfig.EdgeFogDynamic);
            PropertyLedger.Write(_fogProperties, "m_FogHeight", (int)ModConfig.FogHeight);
            PropertyLedger.Write(_fogProperties, "m_HorizonHeight", (int)ModConfig.HorizonHeight);
            PropertyLedger.Write(_fogProperties, "m_FogStart", (int)ModConfig.StartDistance);
            PropertyLedger.Write(_fogProperties, "m_WindSpeed", ModConfig.WindSpeed);
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

            PropertyLedger.Write(_cubemapFog, "enabled", FxInterop.ClassicRequest("fogMode") ? UseClassicFog : ModConfig.CubemapFog
                && (!ModConfig.OffAtNight || SimulationManager.instance == null || !SimulationManager.instance.m_isNightTime));
            PropertyLedger.Write(_cubemapFog, "m_edgeFog", ModConfig.EdgeFogCubemap);
            if (ModConfig.StaticHeight >= 0) PropertyLedger.Write(_cubemapFog, "m_FogHeight", ModConfig.StaticHeight);
            else PropertyLedger.Release(_cubemapFog, "m_FogHeight");
            if (ModConfig.StaticStart >= 0) PropertyLedger.Write(_cubemapFog, "m_3DFogStart", ModConfig.StaticStart);
            else PropertyLedger.Release(_cubemapFog, "m_3DFogStart");
            if (ModConfig.StaticDistance >= 0) PropertyLedger.Write(_cubemapFog, "m_3DFogDistance", ModConfig.StaticDistance);
            else PropertyLedger.Release(_cubemapFog, "m_3DFogDistance");
            if (ModConfig.StaticEdgeDistance >= 0) PropertyLedger.Write(_cubemapFog, "m_edgeFogDistance", ModConfig.StaticEdgeDistance);
            else PropertyLedger.Release(_cubemapFog, "m_edgeFogDistance");
            if (ModConfig.StaticVolumeFog >= 0) PropertyLedger.Write(_cubemapFog, "m_UseVolumeFog", ModConfig.StaticVolumeFog == 1);
            else PropertyLedger.Release(_cubemapFog, "m_UseVolumeFog");
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

            PropertyLedger.Write(_dynamicEffect, "enabled", FxInterop.ClassicRequest("fogMode") ? !UseClassicFog : ModConfig.DynamicFog);
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

            PropertyLedger.Write(_renderProperties, "m_useVolumeFog", ModConfig.VolumeFog);
            PropertyLedger.Write(_renderProperties, "m_inscatteringExponent", ModConfig.ScatterFalloff);
            PropertyLedger.Write(_renderProperties, "m_inscatteringIntensity", ModConfig.ScatterStrength);
            PropertyLedger.Write(_renderProperties, "m_inscatteringColor", ModConfig.ResolveScatterColor());
            PropertyLedger.Write(_renderProperties, "m_volumeFogColor", ModConfig.ResolveVolumeColor());
            PropertyLedger.Write(_renderProperties, "m_volumeFogStart", ModConfig.VolumeStart);
            if (ModConfig.VolumeHeight >= 0) PropertyLedger.Write(_renderProperties, "m_fogHeight", ModConfig.VolumeHeight);
            else PropertyLedger.Release(_renderProperties, "m_fogHeight");
            if (ModConfig.VolumeDensity >= 0) PropertyLedger.Write(_renderProperties, "m_volumeFogDensity", ModConfig.VolumeDensity);
            else PropertyLedger.Release(_renderProperties, "m_volumeFogDensity");
            if (ModConfig.VolumeDistance >= 0) PropertyLedger.Write(_renderProperties, "m_volumeFogDistance", ModConfig.VolumeDistance);
            else PropertyLedger.Release(_renderProperties, "m_volumeFogDistance");
            if (ModConfig.VolumeEdgeDistance >= 0) PropertyLedger.Write(_renderProperties, "m_edgeFogDistance", ModConfig.VolumeEdgeDistance);
            else PropertyLedger.Release(_renderProperties, "m_edgeFogDistance");
        }

        /// <summary>
        /// Puts every touched component back to the acquired reference where no newer external write exists (captured before the first modification).
        /// </summary>
        internal static void RestoreGameDefaults()
        {
            PropertyLedger.ReleaseAll();
            Active = false;
            VanillaSnapshot.ResetCapture();
        }
    }
}