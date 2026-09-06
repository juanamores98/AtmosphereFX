using ICities;
using UnityEngine;

namespace AtmosphereFX.Runtime
{
    /// <summary>
    /// Simulation-tick behaviours owned by v2: sun-matched scatter color and
    /// automatic cubemap fog shutdown at night.
    /// </summary>
    public class PerFrameWatcher : ThreadingExtensionBase
    {
        internal static bool SunMatchedScatter;
        internal static bool OffAtNight;

        private RenderProperties _renderProperties;
        private FogEffect _cubemapFog;
        private int _lookupTick;

        public override void OnCreated(IThreading extension)
        {
            base.OnCreated(extension);
            _renderProperties = null;
            _cubemapFog = null;
            _lookupTick = 0;
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (Config.ModConfig.VanillaMode)
            {
                return; // vanilla mode: the mod touches nothing per frame
            }

            _lookupTick++;

            if (SunMatchedScatter)
            {
                if (_renderProperties == null && (_lookupTick % 60 == 1))
                {
                    _renderProperties = Object.FindObjectOfType<RenderProperties>();
                }

                if (_renderProperties != null && DayNightProperties.instance != null)
                {
                    _renderProperties.m_inscatteringColor = DayNightProperties.instance.currentLightColor;
                }
            }

            if (OffAtNight)
            {
                if (_cubemapFog == null && (_lookupTick % 60 == 1))
                {
                    _cubemapFog = Object.FindObjectOfType<FogEffect>();
                }

                if (_cubemapFog != null && SimulationManager.instance != null)
                {
                    bool shouldBeEnabled = !SimulationManager.instance.m_isNightTime && Config.ModConfig.CubemapFog;
                    if (_cubemapFog.enabled != shouldBeEnabled)
                    {
                        _cubemapFog.enabled = shouldBeEnabled;
                    }
                }
            }
        }

    }
}
