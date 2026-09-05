using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace AtmosphereFX.Config
{
    /// <summary>
    /// v2 settings schema. Property names define the XML document; they are
    /// owned by this version of the mod.
    /// </summary>
    [XmlRoot(ElementName = "atmosphereFx", Namespace = "", IsNullable = false)]
    public class ConfigFile
    {
        [XmlAttribute("schema")]
        public int Schema = 2;

        [XmlElement("dynamicFog")] public bool DynamicFog { get => ModConfig.DynamicFog; set => ModConfig.DynamicFog = value; }
        [XmlElement("colorDecay")] public float ColorDecay { get => ModConfig.ColorDecay; set => ModConfig.ColorDecay = Math.Max(0f, Math.Min(1f, value)); }
        [XmlElement("density")] public float Density { get => ModConfig.Density; set => ModConfig.Density = Clamp(value, 0f, 0.005f); }
        [XmlElement("noise")] public float Noise { get => ModConfig.Noise; set => ModConfig.Noise = Clamp(value, 0f, 2f); }
        [XmlElement("fogHeight")] public float FogHeight { get => ModConfig.FogHeight; set => ModConfig.FogHeight = Clamp(value, 0f, 5000f); }
        [XmlElement("horizonHeight")] public float HorizonHeight { get => ModConfig.HorizonHeight; set => ModConfig.HorizonHeight = Clamp(value, 0f, 5000f); }
        [XmlElement("startDistance")] public float StartDistance { get => ModConfig.StartDistance; set => ModConfig.StartDistance = Clamp(value, 0f, 10000f); }
        [XmlElement("windSpeed")] public float WindSpeed { get => ModConfig.WindSpeed; set => ModConfig.WindSpeed = Clamp(value, 0f, 0.05f); }
        [XmlElement("edgeFog")] public bool EdgeFog { get => ModConfig.EdgeFog; set => ModConfig.EdgeFog = value; }

        [XmlElement("cubemapFog")] public bool CubemapFog { get => ModConfig.CubemapFog; set => ModConfig.CubemapFog = value; }
        [XmlElement("offAtNight")] public bool OffAtNight { get => ModConfig.OffAtNight; set => ModConfig.OffAtNight = value; }
        [XmlElement("volumeFog")] public bool VolumeFog { get => ModConfig.VolumeFog; set => ModConfig.VolumeFog = value; }
        [XmlElement("scatterFalloff")] public float ScatterFalloff { get => ModConfig.ScatterFalloff; set => ModConfig.ScatterFalloff = Clamp(value, 0.5f, 10f); }
        [XmlElement("scatterStrength")] public float ScatterStrength { get => ModConfig.ScatterStrength; set => ModConfig.ScatterStrength = Clamp(value, 0f, 5f); }
        [XmlElement("scatterMode")] public int ScatterColorMode { get => ModConfig.ScatterColorMode; set => ModConfig.ScatterColorMode = ClampInt(value, 0, 2); }
        [XmlElement("scatterR")] public float ScatterR { get => ModConfig.ScatterR; set => ModConfig.ScatterR = Clamp(value, 0f, 1f); }
        [XmlElement("scatterG")] public float ScatterG { get => ModConfig.ScatterG; set => ModConfig.ScatterG = Clamp(value, 0f, 1f); }
        [XmlElement("scatterB")] public float ScatterB { get => ModConfig.ScatterB; set => ModConfig.ScatterB = Clamp(value, 0f, 1f); }
        [XmlElement("autoVolumeColor")] public bool AutoVolumeColor { get => ModConfig.AutoVolumeColor; set => ModConfig.AutoVolumeColor = value; }
        [XmlElement("volumeR")] public float VolumeR { get => ModConfig.VolumeR; set => ModConfig.VolumeR = Clamp(value, 0f, 1f); }
        [XmlElement("volumeG")] public float VolumeG { get => ModConfig.VolumeG; set => ModConfig.VolumeG = Clamp(value, 0f, 1f); }
        [XmlElement("volumeB")] public float VolumeB { get => ModConfig.VolumeB; set => ModConfig.VolumeB = Clamp(value, 0f, 1f); }
        [XmlElement("volumeStart")] public float VolumeStart { get => ModConfig.VolumeStart; set => ModConfig.VolumeStart = Clamp(value, 0f, 4000f); }

        [XmlElement("applyOnLoad")] public bool ApplyOnLoad { get => ModConfig.ApplyOnLoad; set => ModConfig.ApplyOnLoad = value; }

        private static float Clamp(float v, float min, float max)
        {
            return v < min ? min : (v > max ? max : v);
        }

        private static int ClampInt(int v, int min, int max)
        {
            return v < min ? min : (v > max ? max : v);
        }
    }

    /// <summary>
    /// Reads and writes the settings document next to the game executable.
    /// </summary>
    internal static class ConfigStore
    {
        private const string FileName = "AtmosphereFX2.xml";

        internal static void Load()
        {
            try
            {
                if (!File.Exists(FileName))
                {
                    return;
                }

                using (var reader = new StreamReader(FileName))
                {
                    var serializer = new XmlSerializer(typeof(ConfigFile));
                    if (serializer.Deserialize(reader) is ConfigFile)
                    {
                        return;
                    }
                }

                Debug.Log("[AtmosphereFX v2] settings file could not be deserialized");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal static void Save()
        {
            try
            {
                using (var writer = new StreamWriter(FileName))
                {
                    new XmlSerializer(typeof(ConfigFile)).Serialize(writer, new ConfigFile());
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
