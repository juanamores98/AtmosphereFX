using ICities;
using UnityEngine;
using AtmosphereFX.Config;
using AtmosphereFX.Options;
using AtmosphereFX.Runtime;
using AtmosphereFX.UI;

namespace AtmosphereFX
{
    /// <summary>
    /// AtmosphereFX v2 entry point: independent fog and atmosphere tuning for
    /// Cities: Skylines. Original implementation, v2 model.
    /// </summary>
    public class AtmosphereFXMod : LoadingExtensionBase, IUserMod
    {
        private const string HostObjectName = "AtmosphereFX2";

        private GameObject _host;

        public string Name
        {
            get { return ModConfig.ModName; }
        }

        public string Description
        {
            get { return "Independent fog and atmosphere tuning: dynamic fog, cubemap fog, volume scattering."; }
        }

        public void OnEnabled()
        {
            ConfigStore.Load();
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            OptionsPanel.Build(helper);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (ModConfig.VanillaMode)
            {
                SettingsApplier.RestoreGameDefaults();
                return;
            }

            if (ModConfig.ApplyOnLoad)
            {
                SettingsApplier.ApplyAll();
            }

            DestroyHosts();
            _host = new GameObject(HostObjectName);
            _host.AddComponent<AtmosphereEngine>();

            UuiButton.Register(
                "AtmosphereFX v2",
                "Fog and atmosphere tuning (F12)",
                TrayIcon.Make(),
                show => AtmosphereEngine.OpenWindow());
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            UuiButton.Unregister();
            ConfigStore.SaveImmediate();
            SettingsApplier.ClearCache();
            DestroyHosts();
        }

        private void DestroyHosts()
        {
            while (true)
            {
                GameObject leftover = GameObject.Find(HostObjectName);
                if (!leftover)
                {
                    break;
                }

                UnityEngine.Object.DestroyImmediate(leftover);
            }
        }

        public static bool ApplySuiteSection(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return false;
            try
            {
                var doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                var root = doc.DocumentElement;
                if (root == null) return false;
                if (root.Name.Equals("atmospherefx", System.StringComparison.OrdinalIgnoreCase))
                {
                    return ApplySuiteSection(root);
                }
                var node = root.SelectSingleNode("atmospherefx");
                var elem = node as System.Xml.XmlElement;
                if (elem != null)
                {
                    return ApplySuiteSection(elem);
                }
                return false;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return false;
            }
        }

        public static bool ApplySuiteSection(System.Xml.XmlElement element)
        {
            if (element == null) return false;
            try
            {
                var culture = System.Globalization.CultureInfo.InvariantCulture;
                foreach (System.Xml.XmlNode node in element.ChildNodes)
                {
                    if (node.NodeType != System.Xml.XmlNodeType.Element) continue;
                    string name = node.Name.ToLowerInvariant();
                    string val = node.InnerText != null ? node.InnerText.Trim() : string.Empty;
                    bool b;
                    float f;
                    int i;

                    if (name == "dynamicfog" && bool.TryParse(val, out b)) ModConfig.DynamicFog = b;
                    else if (name == "colordecay" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.ColorDecay = f;
                    else if (name == "density" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.Density = f;
                    else if (name == "noise" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.Noise = f;
                    else if (name == "fogheight" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.FogHeight = f;
                    else if (name == "horizonheight" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.HorizonHeight = f;
                    else if (name == "startdistance" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.StartDistance = f;
                    else if (name == "windspeed" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.WindSpeed = f;
                    else if (name == "edgefog" && bool.TryParse(val, out b)) ModConfig.EdgeFog = b;
                    else if (name == "cubemapfog" && bool.TryParse(val, out b)) ModConfig.CubemapFog = b;
                    else if (name == "offatnight" && bool.TryParse(val, out b)) ModConfig.OffAtNight = b;
                    else if (name == "volumefog" && bool.TryParse(val, out b)) ModConfig.VolumeFog = b;
                    else if (name == "scatterfalloff" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.ScatterFalloff = f;
                    else if (name == "scatterstrength" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.ScatterStrength = f;
                    else if (name == "scattercolormode" && int.TryParse(val, out i)) ModConfig.ScatterColorMode = i;
                    else if (name == "scatterr" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.ScatterR = f;
                    else if (name == "scatterg" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.ScatterG = f;
                    else if (name == "scatterb" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.ScatterB = f;
                    else if (name == "autovolumecolor" && bool.TryParse(val, out b)) ModConfig.AutoVolumeColor = b;
                    else if (name == "volumer" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.VolumeR = f;
                    else if (name == "volumeg" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.VolumeG = f;
                    else if (name == "volumeb" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.VolumeB = f;
                    else if (name == "volumestart" && float.TryParse(val, System.Globalization.NumberStyles.Float, culture, out f)) ModConfig.VolumeStart = f;
                }

                SettingsApplier.ApplyAll();
                ConfigStore.SaveImmediate();
                return true;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return false;
            }
        }

        public static string ExportSuiteSection()
        {
            var c = System.Globalization.CultureInfo.InvariantCulture;
            return string.Format(
                "  <atmospherefx>\n" +
                "    <dynamicFog>{0}</dynamicFog>\n" +
                "    <colorDecay>{1}</colorDecay>\n" +
                "    <density>{2}</density>\n" +
                "    <noise>{3}</noise>\n" +
                "    <fogHeight>{4}</fogHeight>\n" +
                "    <horizonHeight>{5}</horizonHeight>\n" +
                "    <startDistance>{6}</startDistance>\n" +
                "    <windSpeed>{7}</windSpeed>\n" +
                "    <edgeFog>{8}</edgeFog>\n" +
                "    <cubemapFog>{9}</cubemapFog>\n" +
                "    <offAtNight>{10}</offAtNight>\n" +
                "    <volumeFog>{11}</volumeFog>\n" +
                "    <scatterFalloff>{12}</scatterFalloff>\n" +
                "    <scatterStrength>{13}</scatterStrength>\n" +
                "    <scatterColorMode>{14}</scatterColorMode>\n" +
                "    <scatterR>{15}</scatterR>\n" +
                "    <scatterG>{16}</scatterG>\n" +
                "    <scatterB>{17}</scatterB>\n" +
                "    <autoVolumeColor>{18}</autoVolumeColor>\n" +
                "    <volumeR>{19}</volumeR>\n" +
                "    <volumeG>{20}</volumeG>\n" +
                "    <volumeB>{21}</volumeB>\n" +
                "    <volumeStart>{22}</volumeStart>\n" +
                "  </atmospherefx>",
                ModConfig.DynamicFog.ToString().ToLowerInvariant(),
                ModConfig.ColorDecay.ToString("0.000", c),
                ModConfig.Density.ToString("0.000000", c),
                ModConfig.Noise.ToString("0.000", c),
                ModConfig.FogHeight.ToString("0.0", c),
                ModConfig.HorizonHeight.ToString("0.0", c),
                ModConfig.StartDistance.ToString("0.0", c),
                ModConfig.WindSpeed.ToString("0.000", c),
                ModConfig.EdgeFog.ToString().ToLowerInvariant(),
                ModConfig.CubemapFog.ToString().ToLowerInvariant(),
                ModConfig.OffAtNight.ToString().ToLowerInvariant(),
                ModConfig.VolumeFog.ToString().ToLowerInvariant(),
                ModConfig.ScatterFalloff.ToString("0.000", c),
                ModConfig.ScatterStrength.ToString("0.000", c),
                ModConfig.ScatterColorMode.ToString(c),
                ModConfig.ScatterR.ToString("0.000", c),
                ModConfig.ScatterG.ToString("0.000", c),
                ModConfig.ScatterB.ToString("0.000", c),
                ModConfig.AutoVolumeColor.ToString().ToLowerInvariant(),
                ModConfig.VolumeR.ToString("0.000", c),
                ModConfig.VolumeG.ToString("0.000", c),
                ModConfig.VolumeB.ToString("0.000", c),
                ModConfig.VolumeStart.ToString("0.0", c));
        }
    }
}
