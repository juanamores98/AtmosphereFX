using UnityEngine;

namespace AtmosphereFX.Config
{
    /// <summary>
    /// AtmosphereFX v2 tuning model. Every value here is owned by this mod:
    /// ranges, defaults and mappings were designed for v2 and are applied
    /// directly to the game's render components without intermediate tricks.
    /// </summary>
    internal static class ModConfig
    {
        internal const string ModName = "AtmosphereFX v2";

        // Reference values of an unmodified game, used by the reset action.
        internal const float VanillaColorDecay = 0.2f;
        internal const float VanillaDensity = 0.00223f;
        internal const float VanillaNoise = 1f;
        internal const float VanillaWindSpeed = 0.001f;
        internal const float VanillaFogHeight = 1000f;
        internal const float VanillaHorizonHeight = 800f;
        internal const float VanillaStartDistance = 194f;
        internal const float VanillaScatterFalloff = 1.7f;
        internal const float VanillaScatterStrength = 1.7f;

        internal static readonly Color VanillaScatterColor = new Color(0.5647f, 0.9255f, 1f, 1f);
        internal static readonly Color DefaultVolumeColor = new Color(0.651f, 0.8863f, 1f, 1f);

        // ---- Dynamic fog ----
        internal static bool DynamicFog = true;
        internal static float ColorDecay = VanillaColorDecay;
        internal static float Density = VanillaDensity;
        internal static float Noise = VanillaNoise;
        internal static float FogHeight = VanillaFogHeight;
        internal static float HorizonHeight = VanillaHorizonHeight;
        internal static float StartDistance = VanillaStartDistance;
        internal static float WindSpeed = VanillaWindSpeed;
        internal static bool EdgeFog = true;

        // ---- Classic fog ----
        internal static bool CubemapFog;
        internal static bool OffAtNight;
        internal static bool VolumeFog = true;
        internal static float ScatterFalloff = VanillaScatterFalloff; // direct game exponent
        internal static float ScatterStrength = VanillaScatterStrength;
        internal static int ScatterColorMode; // 0 auto, 1 sun-matched, 2 custom
        internal static float ScatterR = 0.2f;
        internal static float ScatterG = 0.4f;
        internal static float ScatterB = 0.8f;
        internal static bool AutoVolumeColor = true;
        internal static float VolumeR = 0.7f;
        internal static float VolumeG = 0.8f;
        internal static float VolumeB = 1f;
        internal static float VolumeStart;

        // ---- Behavior ----
        internal static bool ApplyOnLoad = true;

        internal static void RestoreVanilla()
        {
            DynamicFog = true;
            ColorDecay = VanillaColorDecay;
            Density = VanillaDensity;
            Noise = VanillaNoise;
            FogHeight = VanillaFogHeight;
            HorizonHeight = VanillaHorizonHeight;
            StartDistance = VanillaStartDistance;
            WindSpeed = VanillaWindSpeed;
            EdgeFog = true;
            CubemapFog = false;
            OffAtNight = false;
            VolumeFog = true;
            ScatterFalloff = VanillaScatterFalloff;
            ScatterStrength = VanillaScatterStrength;
            ScatterColorMode = 0;
            AutoVolumeColor = true;
            VolumeStart = 0f;
            ApplyOnLoad = true;
        }

        internal static Color ResolveScatterColor(int mode)
        {
            switch (mode)
            {
                case 2:
                    return new Color(ScatterR, ScatterG, ScatterB, 1f);
                default:
                    return VanillaScatterColor;
            }
        }

        internal static Color ResolveScatterColor()
        {
            return ResolveScatterColor(ScatterColorMode);
        }

        internal static Color ResolveVolumeColor()
        {
            return AutoVolumeColor
                ? DefaultVolumeColor
                : new Color(VolumeR, VolumeG, VolumeB, 1f);
        }
    }
}
