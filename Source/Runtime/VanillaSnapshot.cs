using UnityEngine;

namespace AtmosphereFX.Runtime
{
    /// <summary>
    /// Captures the game's untouched render state before the first write and
    /// restores it on demand. Each component is tracked independently so a
    /// component that was never captured is never restored as zero.
    /// </summary>
    internal static class VanillaSnapshot
    {
        private static bool _fogCaptured;
        private static bool _fogEffectCaptured;
        private static bool _dayNightFogCaptured;
        private static bool _renderPropsCaptured;

        private static float _colorDecay;
        private static float _fogDensity;
        private static float _noise;
        private static float _windSpeed;
        private static float _fogHeight;
        private static float _horizonHeight;
        private static float _fogStart;
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
        private static bool _staticVolume;
        private static float _originalStaticHeight;
        private static float _originalStaticStart;
        private static float _originalStaticDistance;
        private static float _originalStaticEdgeDistance;
        private static float _originalVolumeHeight;
        private static float _originalVolumeDensity;
        private static float _originalVolumeDistance;
        private static float _originalVolumeEdgeDistance;


        internal static bool Captured
        {
            get { return _fogCaptured || _fogEffectCaptured || _dayNightFogCaptured || _renderPropsCaptured; }
        }

        internal static void Capture()
        {
            var fog = Object.FindObjectOfType<FogProperties>();
            if (fog != null && !_fogCaptured)
            {
                _colorDecay = fog.m_ColorDecay;
                _fogDensity = fog.m_FogDensity;
                _noise = fog.m_NoiseContribution;
                _windSpeed = fog.m_WindSpeed;
                _fogHeight = fog.m_FogHeight;
                _horizonHeight = fog.m_HorizonHeight;
                _fogStart = fog.m_FogStart;
                _fogEdgeFog = fog.m_edgeFog;
                _fogCaptured = true;
            }

            var fogEffect = Object.FindObjectOfType<FogEffect>();
            if (fogEffect != null && !_fogEffectCaptured)
            {
                _fogEffectEnabled = fogEffect.enabled;
                _fogEffectEdge = fogEffect.m_edgeFog;
                _staticVolume = fogEffect.m_UseVolumeFog;
                _originalStaticHeight = fogEffect.m_FogHeight;
                _originalStaticStart = fogEffect.m_3DFogStart;
                _originalStaticDistance = fogEffect.m_3DFogDistance;
                _originalStaticEdgeDistance = fogEffect.m_edgeFogDistance;

                _fogEffectCaptured = true;
            }

            var dayNightFog = Object.FindObjectOfType<DayNightFogEffect>();
            if (dayNightFog != null && !_dayNightFogCaptured)
            {
                _dayNightFogEnabled = dayNightFog.enabled;
                _dayNightFogCaptured = true;
            }

            var props = Object.FindObjectOfType<RenderProperties>();
            if (props != null && !_renderPropsCaptured)
            {
                _useVolumeFog = props.m_useVolumeFog;
                _inscatterExponent = props.m_inscatteringExponent;
                _inscatterIntensity = props.m_inscatteringIntensity;
                _inscatterColor = props.m_inscatteringColor;
                _volumeColor = props.m_volumeFogColor;
                _volumeStart = props.m_volumeFogStart;
                _originalVolumeHeight = props.m_fogHeight;
                _originalVolumeDensity = props.m_volumeFogDensity;
                _originalVolumeDistance = props.m_volumeFogDistance;
                _originalVolumeEdgeDistance = props.m_edgeFogDistance;

                _renderPropsCaptured = true;
            }
        }

        internal static void ResetCapture()
        {
            _fogCaptured = false;
            _fogEffectCaptured = false;
            _dayNightFogCaptured = false;
            _renderPropsCaptured = false;
        }

        internal static void Restore()
        {
            Infrastructure.PropertyLedger.ReleaseAll();
        }
    }
}
