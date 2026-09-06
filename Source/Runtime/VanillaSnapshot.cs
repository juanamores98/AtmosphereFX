using UnityEngine;

namespace AtmosphereFX.Runtime
{
    /// <summary>
    /// Captures the game's untouched render state before the first write and
    /// restores it on demand. This is the true "vanilla" fallback: even if a
    /// user saves an extreme configuration, one call returns every component
    /// to the exact values the game shipped with.
    /// </summary>
    internal static class VanillaSnapshot
    {
        private static bool _captured;

        private static float _colorDecay;
        private static float _fogDensity;
        private static float _noise;
        private static float _windSpeed;
        private static int _fogHeight;
        private static int _horizonHeight;
        private static int _fogStart;
        private static bool _fogEdgeFog;

        private static bool _fogEffectEnabled;
        private static bool _fogEffectEdge;
        private static bool _dayNightFogEnabled;

        private static bool _useVolumeFog;
        private static float _inscatterExponent;
        private static float _inscatterIntensity;
        private static Color _inscatterColor;
        private static Color _volumeColor;
        private static float _volumeStart;

        internal static bool Captured
        {
            get { return _captured; }
        }

        internal static void ResetCapture()
        {
            _captured = false;
        }

        internal static void Capture()
        {
            if (_captured)
            {
                return;
            }

            var fog = Object.FindObjectOfType<FogProperties>();
            if (fog != null)
            {
                _colorDecay = fog.m_ColorDecay;
                _fogDensity = fog.m_FogDensity;
                _noise = fog.m_NoiseContribution;
                _windSpeed = fog.m_WindSpeed;
                _fogHeight = (int)fog.m_FogHeight;
                _horizonHeight = (int)fog.m_HorizonHeight;
                _fogStart = (int)fog.m_FogStart;
                _fogEdgeFog = fog.m_edgeFog;
            }

            var fogEffect = Object.FindObjectOfType<FogEffect>();
            if (fogEffect != null)
            {
                _fogEffectEnabled = fogEffect.enabled;
                _fogEffectEdge = fogEffect.m_edgeFog;
            }

            var dayNightFog = Object.FindObjectOfType<DayNightFogEffect>();
            if (dayNightFog != null)
            {
                _dayNightFogEnabled = dayNightFog.enabled;
            }

            var props = Object.FindObjectOfType<RenderProperties>();
            if (props != null)
            {
                _useVolumeFog = props.m_useVolumeFog;
                _inscatterExponent = props.m_inscatteringExponent;
                _inscatterIntensity = props.m_inscatteringIntensity;
                _inscatterColor = props.m_inscatteringColor;
                _volumeColor = props.m_volumeFogColor;
                _volumeStart = props.m_volumeFogStart;
            }

            _captured = fog != null || props != null || fogEffect != null;
        }

        internal static void Restore()
        {
            if (!_captured)
            {
                return;
            }

            var fog = Object.FindObjectOfType<FogProperties>();
            if (fog != null)
            {
                fog.m_ColorDecay = _colorDecay;
                fog.m_FogDensity = _fogDensity;
                fog.m_NoiseContribution = _noise;
                fog.m_WindSpeed = _windSpeed;
                fog.m_FogHeight = _fogHeight;
                fog.m_HorizonHeight = _horizonHeight;
                fog.m_FogStart = _fogStart;
                fog.m_edgeFog = _fogEdgeFog;
            }

            var fogEffect = Object.FindObjectOfType<FogEffect>();
            if (fogEffect != null)
            {
                fogEffect.enabled = _fogEffectEnabled;
                fogEffect.m_edgeFog = _fogEffectEdge;
            }

            var dayNightFog = Object.FindObjectOfType<DayNightFogEffect>();
            if (dayNightFog != null)
            {
                dayNightFog.enabled = _dayNightFogEnabled;
            }

            var props = Object.FindObjectOfType<RenderProperties>();
            if (props != null)
            {
                props.m_useVolumeFog = _useVolumeFog;
                props.m_inscatteringExponent = _inscatterExponent;
                props.m_inscatteringIntensity = _inscatterIntensity;
                props.m_inscatteringColor = _inscatterColor;
                props.m_volumeFogColor = _volumeColor;
                props.m_volumeFogStart = _volumeStart;
            }
        }
    }
}
