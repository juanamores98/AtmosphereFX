using ColossalFramework.IO;
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

        private bool _DynamicFog = ModConfig.DynamicFog;
        [XmlElement("dynamicFog")] public bool DynamicFog { get => _DynamicFog; set => _DynamicFog = value; }
        private float _ColorDecay = ModConfig.ColorDecay;
        [XmlElement("colorDecay")] public float ColorDecay { get => _ColorDecay; set => _ColorDecay = Clamp(value, 0f, 1f); }
        private float _Density = ModConfig.Density;
        [XmlElement("density")] public float Density { get => _Density; set => _Density = Clamp(value, 0f, 0.005f); }
        private float _Noise = ModConfig.Noise;
        [XmlElement("noise")] public float Noise { get => _Noise; set => _Noise = Clamp(value, 0f, 2f); }
        private float _FogHeight = ModConfig.FogHeight;
        [XmlElement("fogHeight")] public float FogHeight { get => _FogHeight; set => _FogHeight = Clamp(value, 0f, 5000f); }
        private float _HorizonHeight = ModConfig.HorizonHeight;
        [XmlElement("horizonHeight")] public float HorizonHeight { get => _HorizonHeight; set => _HorizonHeight = Clamp(value, 0f, 5000f); }
        private float _StartDistance = ModConfig.StartDistance;
        [XmlElement("startDistance")] public float StartDistance { get => _StartDistance; set => _StartDistance = Clamp(value, 0f, 10000f); }
        private float _WindSpeed = ModConfig.WindSpeed;
        [XmlElement("windSpeed")] public float WindSpeed { get => _WindSpeed; set => _WindSpeed = Clamp(value, 0f, 0.05f); }
        [XmlElement("edgeFog")] public bool EdgeFogLegacy { get => EdgeFogDynamic; set { EdgeFogDynamic = value; EdgeFogCubemap = value; } }
        public bool ShouldSerializeEdgeFogLegacy() { return false; }
        private bool _EdgeFogDynamic = ModConfig.EdgeFogDynamic;
        [XmlElement("edgeFogDynamic")] public bool EdgeFogDynamic { get => _EdgeFogDynamic; set => _EdgeFogDynamic = value; }
        private bool _EdgeFogCubemap = ModConfig.EdgeFogCubemap;
        [XmlElement("edgeFogCubemap")] public bool EdgeFogCubemap { get => _EdgeFogCubemap; set => _EdgeFogCubemap = value; }

        private bool _CubemapFog = ModConfig.CubemapFog;
        [XmlElement("cubemapFog")] public bool CubemapFog { get => _CubemapFog; set => _CubemapFog = value; }
        private bool _OffAtNight = ModConfig.OffAtNight;
        [XmlElement("offAtNight")] public bool OffAtNight { get => _OffAtNight; set => _OffAtNight = value; }
        private bool _VolumeFog = ModConfig.VolumeFog;
        [XmlElement("volumeFog")] public bool VolumeFog { get => _VolumeFog; set => _VolumeFog = value; }
        private float _ScatterFalloff = ModConfig.ScatterFalloff;
        [XmlElement("scatterFalloff")] public float ScatterFalloff { get => _ScatterFalloff; set => _ScatterFalloff = Clamp(value, 0.5f, 100000f); }
        private float _ScatterStrength = ModConfig.ScatterStrength;
        [XmlElement("scatterStrength")] public float ScatterStrength { get => _ScatterStrength; set => _ScatterStrength = Clamp(value, 0f, 100f); }
        private int _ScatterColorMode = ModConfig.ScatterColorMode;
        [XmlElement("scatterMode")] public int ScatterColorMode { get => _ScatterColorMode; set => _ScatterColorMode = ClampInt(value, 0, 2); }
        private float _ScatterR = ModConfig.ScatterR;
        [XmlElement("scatterR")] public float ScatterR { get => _ScatterR; set => _ScatterR = Clamp(value, 0f, 1f); }
        private float _ScatterG = ModConfig.ScatterG;
        [XmlElement("scatterG")] public float ScatterG { get => _ScatterG; set => _ScatterG = Clamp(value, 0f, 1f); }
        private float _ScatterB = ModConfig.ScatterB;
        [XmlElement("scatterB")] public float ScatterB { get => _ScatterB; set => _ScatterB = Clamp(value, 0f, 1f); }
        private bool _AutoVolumeColor = ModConfig.AutoVolumeColor;
        [XmlElement("autoVolumeColor")] public bool AutoVolumeColor { get => _AutoVolumeColor; set => _AutoVolumeColor = value; }
        private float _VolumeR = ModConfig.VolumeR;
        [XmlElement("volumeR")] public float VolumeR { get => _VolumeR; set => _VolumeR = Clamp(value, 0f, 1f); }
        private float _VolumeG = ModConfig.VolumeG;
        [XmlElement("volumeG")] public float VolumeG { get => _VolumeG; set => _VolumeG = Clamp(value, 0f, 1f); }
        private float _VolumeB = ModConfig.VolumeB;
        [XmlElement("volumeB")] public float VolumeB { get => _VolumeB; set => _VolumeB = Clamp(value, 0f, 1f); }
        private float _VolumeStart = ModConfig.VolumeStart;
        [XmlElement("volumeStart")] public float VolumeStart { get => _VolumeStart; set => _VolumeStart = Clamp(value, 0f, 10000f); }

        private float _WindowX = ModConfig.WindowX;
        [XmlElement("windowX")] public float WindowX { get => _WindowX; set => _WindowX = Infrastructure.FxStorage.Clamp(value, -100000f, 100000f); }
        private float _WindowY = ModConfig.WindowY;
        [XmlElement("windowY")] public float WindowY { get => _WindowY; set => _WindowY = Infrastructure.FxStorage.Clamp(value, -100000f, 100000f); }

        private bool _ApplyOnLoad = ModConfig.ApplyOnLoad;
        [XmlElement("applyOnLoad")] public bool ApplyOnLoad { get => _ApplyOnLoad; set => _ApplyOnLoad = value; }

        private bool _VanillaMode = ModConfig.VanillaMode;
        [XmlElement("vanillaMode")] public bool VanillaMode { get => _VanillaMode; set => _VanillaMode = value; }


        private int _StaticVolumeFog = ModConfig.StaticVolumeFog;
        [XmlElement("staticVolumeFog")] public int StaticVolumeFog { get => _StaticVolumeFog; set => _StaticVolumeFog = ClampInt(value, -1, 1); }
        private float _StaticHeight = ModConfig.StaticHeight;
        [XmlElement("staticHeight")] public float StaticHeight { get => _StaticHeight; set => _StaticHeight = Clamp(value, -1f, 10000f); }
        private float _StaticStart = ModConfig.StaticStart;
        [XmlElement("staticStart")] public float StaticStart { get => _StaticStart; set => _StaticStart = Clamp(value, -1f, 10000f); }
        private float _StaticDistance = ModConfig.StaticDistance;
        [XmlElement("staticDistance")] public float StaticDistance { get => _StaticDistance; set => _StaticDistance = Clamp(value, -1f, 20000f); }
        private float _StaticEdgeDistance = ModConfig.StaticEdgeDistance;
        [XmlElement("staticEdgeDistance")] public float StaticEdgeDistance { get => _StaticEdgeDistance; set => _StaticEdgeDistance = Clamp(value, -1f, 20000f); }
        private float _VolumeHeight = ModConfig.VolumeHeight;
        [XmlElement("volumeHeight")] public float VolumeHeight { get => _VolumeHeight; set => _VolumeHeight = Clamp(value, -1f, 10000f); }
        private float _VolumeDensity = ModConfig.VolumeDensity;
        [XmlElement("volumeDensity")] public float VolumeDensity { get => _VolumeDensity; set => _VolumeDensity = Clamp(value, -1f, 0.01f); }
        private float _VolumeDistance = ModConfig.VolumeDistance;
        [XmlElement("volumeDistance")] public float VolumeDistance { get => _VolumeDistance; set => _VolumeDistance = Clamp(value, -1f, 20000f); }
        private float _VolumeEdgeDistance = ModConfig.VolumeEdgeDistance;
        [XmlElement("volumeEdgeDistance")] public float VolumeEdgeDistance { get => _VolumeEdgeDistance; set => _VolumeEdgeDistance = Clamp(value, -1f, 20000f); }

        internal void Apply()
        {
            ModConfig.DynamicFog = DynamicFog;
            ModConfig.ColorDecay = ColorDecay;
            ModConfig.Density = Density;
            ModConfig.Noise = Noise;
            ModConfig.FogHeight = FogHeight;
            ModConfig.HorizonHeight = HorizonHeight;
            ModConfig.StartDistance = StartDistance;
            ModConfig.WindSpeed = WindSpeed;
            ModConfig.EdgeFogDynamic = EdgeFogDynamic;
            ModConfig.EdgeFogCubemap = EdgeFogCubemap;
            ModConfig.CubemapFog = CubemapFog;
            ModConfig.OffAtNight = OffAtNight;
            ModConfig.VolumeFog = VolumeFog;
            ModConfig.ScatterFalloff = ScatterFalloff;
            ModConfig.ScatterStrength = ScatterStrength;
            ModConfig.ScatterColorMode = ScatterColorMode;
            ModConfig.ScatterR = ScatterR;
            ModConfig.ScatterG = ScatterG;
            ModConfig.ScatterB = ScatterB;
            ModConfig.AutoVolumeColor = AutoVolumeColor;
            ModConfig.VolumeR = VolumeR;
            ModConfig.VolumeG = VolumeG;
            ModConfig.VolumeB = VolumeB;
            ModConfig.VolumeStart = VolumeStart;
            ModConfig.WindowX = WindowX;
            ModConfig.WindowY = WindowY;
            ModConfig.ApplyOnLoad = ApplyOnLoad;
            ModConfig.VanillaMode = VanillaMode;
            ModConfig.StaticVolumeFog = StaticVolumeFog;
            ModConfig.StaticHeight = StaticHeight;
            ModConfig.StaticStart = StaticStart;
            ModConfig.StaticDistance = StaticDistance;
            ModConfig.StaticEdgeDistance = StaticEdgeDistance;
            ModConfig.VolumeHeight = VolumeHeight;
            ModConfig.VolumeDensity = VolumeDensity;
            ModConfig.VolumeDistance = VolumeDistance;
            ModConfig.VolumeEdgeDistance = VolumeEdgeDistance;

        }

        private static float Clamp(float v, float min, float max)
        {
            return Infrastructure.FxStorage.Clamp(v, min, max);
        }

        private static int ClampInt(int v, int min, int max)
        {
            return v < min ? min : (v > max ? max : v);
        }
    }

    /// <summary>
    /// Reads and writes the settings document next to the game executable.
    /// Writes are throttled so slider drags stay cheap without losing the final value.
    /// </summary>
    internal static class ConfigStore
    {
                /// <remarks>
        /// <b>Ruta completa, no relativa.</b> Un nombre suelto lo resuelve .NET contra el
        /// directorio de trabajo del proceso, que en Cities: Skylines es la carpeta de
        /// instalacion del juego. Ahi acababan estos XML: dentro de Archivos de Programa, donde
        /// escribir suele requerir permisos y donde una verificacion de Steam puede borrarlos.
        /// Se midio en partida —los cuatro archivos aparecieron en la carpeta del juego— y solo
        /// LumenFX lo hacia bien.
        ///
        /// <b>La migracion.</b> Si queda un archivo en el sitio antiguo y todavia no hay uno en
        /// el nuevo, se lee el antiguo: nadie pierde su configuracion por arreglar esto.
        /// </remarks>
        private const string FileName = "AtmosphereFX2.xml";

        private static string ConfigPath
        {
            get { return Path.Combine(DataLocation.localApplicationData, "AtmosphereFX2.xml"); }
        }

        /// <summary>El sitio antiguo: la carpeta de trabajo del proceso.</summary>
        private static string ConfigPathLegacy
        {
            get { return "AtmosphereFX2.xml"; }
        }

        /// <summary>De donde leer: el sitio nuevo si existe, y si no el antiguo.</summary>
        private static string ConfigPathToRead
        {
            get
            {
                return File.Exists(ConfigPath) || !File.Exists(ConfigPathLegacy)
                    ? ConfigPath
                    : ConfigPathLegacy;
            }
        }
        private static float _lastWrite = -10f;
        private static bool _dirty;

        internal static void Load()
        {
            try
            {
                if (!File.Exists(ConfigPathToRead))
                {
                    return;
                }

                using (var reader = new StreamReader(ConfigPathToRead))
                {
                    var serializer = new XmlSerializer(typeof(ConfigFile));
                    if (serializer.Deserialize(reader) is ConfigFile document)
                    {
                        document.Apply();
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

        internal static void Save(bool immediate = false)
        {
            _dirty = true;
            float now = Time.realtimeSinceStartup;
            if (immediate || now - _lastWrite >= 1.0f)
            {
                SaveImmediate();
            }
        }

        internal static void CheckPendingSave()
        {
            if (_dirty && Time.realtimeSinceStartup - _lastWrite >= 1.0f)
            {
                SaveImmediate();
            }
        }

        internal static void SaveImmediate()
        {
            _dirty = true;
            _lastWrite = Time.realtimeSinceStartup;
            try
            {
                Infrastructure.FxStorage.WriteXml(ConfigPath, new ConfigFile());
                _dirty = false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}

