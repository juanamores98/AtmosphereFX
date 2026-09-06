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

        public void OnEnabled()
        {
            Config.ConfigStore.Load();
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (Config.ModConfig.VanillaMode)
            {
                return; // vanilla mode: the mod touches nothing per frame
            }

            if (SunMatchedScatter)
            {
                if (_renderProperties == null)
                {
                    _renderProperties = Object.FindObjectOfType<RenderProperties>();
                }

                if (_renderProperties != null)
                {
                    _renderProperties.m_inscatteringColor = DayNightProperties.instance.currentLightColor;
                }
            }

            if (OffAtNight)
            {
                if (_cubemapFog == null)
                {
                    _cubemapFog = Object.FindObjectOfType<FogEffect>();
                }

                if (_cubemapFog != null)
                {
                    _cubemapFog.enabled = !SimulationManager.instance.m_isNightTime && Config.ModConfig.CubemapFog;
                }
            }
        }
    }
}
