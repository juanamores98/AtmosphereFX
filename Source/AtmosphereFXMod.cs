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

        public static string ActiveClaims { get { return SettingsApplier.Active && !ModConfig.VanillaMode ? "fog,fogEffect" : string.Empty; } }

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
            }
            else if (ModConfig.ApplyOnLoad)
            {
                SettingsApplier.ApplyAll();
            }

            DestroyHosts();
            _host = new GameObject(HostObjectName);
            _host.AddComponent<AtmosphereEngine>();

            UuiButton.Register(
                "AtmosphereFX v2",
                "Fog and atmosphere tuning (Ctrl+Alt+A)",
                TrayIcon.Make(),
                show => AtmosphereEngine.OpenWindow());
        }

        public void OnDisabled()
        {
            UuiButton.Unregister();
            ConfigStore.SaveImmediate();
            if (!ModConfig.VanillaMode)
            {
                SettingsApplier.RestoreGameDefaults();
            }

            VanillaSnapshot.ResetCapture();
            DestroyHosts();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            UuiButton.Unregister();
            ConfigStore.SaveImmediate();
            SettingsApplier.ClearCache();
            VanillaSnapshot.ResetCapture();
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

        public static void RefreshDerivedState() { if (Runtime.SettingsApplier.Active && !Config.ModConfig.VanillaMode) Runtime.SettingsApplier.ApplyAll(); NotifyStateChanged(); }
        public static bool ReadyForSuite { get { return UnityEngine.Object.FindObjectOfType<FogProperties>() != null && UnityEngine.Object.FindObjectOfType<FogEffect>() != null && UnityEngine.Object.FindObjectOfType<DayNightFogEffect>() != null && UnityEngine.Object.FindObjectOfType<RenderProperties>() != null; } }
        public static string LastApplyError { get; private set; }
        public static string ApplicationStatus { get; private set; }
        public static event System.Action StateChanged;
        public static void NotifyStateChanged()
        {
            var changed = StateChanged;
            if (changed == null) return;
            foreach (System.Action observer in changed.GetInvocationList())
                try { observer(); } catch (System.Exception e) { UnityEngine.Debug.LogException(e); }
        }
        public static bool ValidateSuiteSection(string xml)
        {
            try
            {
                var doc = new System.Xml.XmlDocument { XmlResolver = null }; doc.LoadXml(xml);
                return ParseSection(doc.DocumentElement, false);
            }
            catch (System.Exception e) { LastApplyError = e.Message; return false; }
        }
        public static bool ApplySuiteSection(System.Xml.XmlElement element)
        {
            if (!ParseSection(element, false)) return false;
            if (Infrastructure.FxTransaction.Active) return ParseSection(element, true);
            string previous = ExportSuiteSection();
            Infrastructure.FxTransaction.Begin();
            try
            {
                if (!ParseSection(element, true)) throw new System.InvalidOperationException(LastApplyError);
                Infrastructure.FxTransaction.Commit();
                return true;
            }
            catch (System.Exception failure)
            {
                if (!Infrastructure.FxTransaction.Active) Infrastructure.FxTransaction.Begin();
                var doc = new System.Xml.XmlDocument(); doc.LoadXml(previous);
                bool restored = ParseSection(doc.DocumentElement, true) && ExportSuiteSection() == previous;
                Infrastructure.FxTransaction.Abort();
                LastApplyError = failure.Message;
                ApplicationStatus = (restored && !failure.Message.StartsWith("PARTIAL:") ? "Failed; previous settings restored: " : "PARTIAL; rollback could not be verified: ") + failure.Message;
                NotifyStateChanged();
                return false;
            }
            finally { Infrastructure.FxTransaction.Abort(); }
        }
        private static bool ParseSection(System.Xml.XmlElement element, bool commit)
        {
            if (element == null || !element.Name.Equals("atmospherefx", System.StringComparison.OrdinalIgnoreCase)) return false;
            try
            {
                LastApplyError = string.Empty;
                Infrastructure.FxStorage.LastError = string.Empty;
                string schema = element.GetAttribute("schema");
                if (schema.Length > 0 && schema != "2" && schema != "3") throw new System.ArgumentException("Unsupported preset schema: " + schema);

                var pending = new ConfigFile();
                var culture = System.Globalization.CultureInfo.InvariantCulture;
                foreach (System.Xml.XmlNode node in element.ChildNodes)
                {
                    if (node.NodeType != System.Xml.XmlNodeType.Element) continue;
                    string name = node.Name.ToLowerInvariant();
                    string val = node.InnerText != null ? node.InnerText.Trim() : string.Empty;




                    if (name == "dynamicfog") pending.DynamicFog = bool.Parse(val);
                    else if (name == "applyonload") pending.ApplyOnLoad = bool.Parse(val);
                    else if (name == "staticvolumefog") pending.StaticVolumeFog = int.Parse(val, culture);
                    else if (name == "staticheight") pending.StaticHeight = float.Parse(val, culture);
                    else if (name == "staticstart") pending.StaticStart = float.Parse(val, culture);
                    else if (name == "staticdistance") pending.StaticDistance = float.Parse(val, culture);
                    else if (name == "staticedgedistance") pending.StaticEdgeDistance = float.Parse(val, culture);
                    else if (name == "volumeheight") pending.VolumeHeight = float.Parse(val, culture);
                    else if (name == "volumedensity") pending.VolumeDensity = float.Parse(val, culture);
                    else if (name == "volumedistance") pending.VolumeDistance = float.Parse(val, culture);
                    else if (name == "volumeedgedistance") pending.VolumeEdgeDistance = float.Parse(val, culture);
                    else if (name == "colordecay") pending.ColorDecay = float.Parse(val, culture);
                    else if (name == "density") pending.Density = float.Parse(val, culture);
                    else if (name == "noise") pending.Noise = float.Parse(val, culture);
                    else if (name == "fogheight") pending.FogHeight = float.Parse(val, culture);
                    else if (name == "horizonheight") pending.HorizonHeight = float.Parse(val, culture);
                    else if (name == "startdistance") pending.StartDistance = float.Parse(val, culture);
                    else if (name == "windspeed") pending.WindSpeed = float.Parse(val, culture);
                    else if (name == "edgefog") { pending.EdgeFogDynamic = bool.Parse(val); pending.EdgeFogCubemap = bool.Parse(val); }
                    else if (name == "edgefogdynamic") pending.EdgeFogDynamic = bool.Parse(val);
                    else if (name == "edgefogcubemap") pending.EdgeFogCubemap = bool.Parse(val);
                    else if (name == "vanillamode")
                    {
                        pending.VanillaMode = bool.Parse(val);
                    }
                    else if (name == "cubemapfog") pending.CubemapFog = bool.Parse(val);
                    else if (name == "offatnight") pending.OffAtNight = bool.Parse(val);
                    else if (name == "volumefog") pending.VolumeFog = bool.Parse(val);
                    else if (name == "scatterfalloff") pending.ScatterFalloff = float.Parse(val, culture);
                    else if (name == "scatterstrength") pending.ScatterStrength = float.Parse(val, culture);
                    else if (name == "scattercolormode") pending.ScatterColorMode = int.Parse(val, culture);
                    else if (name == "scatterr") pending.ScatterR = float.Parse(val, culture);
                    else if (name == "scatterg") pending.ScatterG = float.Parse(val, culture);
                    else if (name == "scatterb") pending.ScatterB = float.Parse(val, culture);
                    else if (name == "autovolumecolor") pending.AutoVolumeColor = bool.Parse(val);
                    else if (name == "volumer") pending.VolumeR = float.Parse(val, culture);
                    else if (name == "volumeg") pending.VolumeG = float.Parse(val, culture);
                    else if (name == "volumeb") pending.VolumeB = float.Parse(val, culture);
                    else if (name == "volumestart") pending.VolumeStart = float.Parse(val, culture);
                }

                if (!commit) return true;
                pending.Apply();
                if (ModConfig.VanillaMode) SettingsApplier.RestoreGameDefaults();
                else SettingsApplier.ApplyAll();
                ConfigStore.SaveImmediate();
                if (!string.IsNullOrEmpty(Infrastructure.FxStorage.LastError)) throw new System.IO.IOException(Infrastructure.FxStorage.LastError);
                ApplicationStatus = "Applied to settings; verify appearance in game";
                Infrastructure.FxInterop.RefreshCompanions();
                NotifyStateChanged();
                return true;
            }
            catch (System.Exception e)
            {
                LastApplyError = e.Message;
                ApplicationStatus = "Failed: " + e.Message;

                UnityEngine.Debug.LogException(e);
                return false;
            }
        }

        public static string ExportSuiteSection()
        {
            var source = new System.Xml.XmlDocument();
            using (var writer = new System.IO.StringWriter(System.Globalization.CultureInfo.InvariantCulture))
            {
                new System.Xml.Serialization.XmlSerializer(typeof(ConfigFile)).Serialize(writer, new ConfigFile());
                source.LoadXml(writer.ToString());
            }
            var doc = new System.Xml.XmlDocument();
            var root = doc.CreateElement("atmospherefx"); doc.AppendChild(root);
            foreach (System.Xml.XmlNode node in source.DocumentElement.ChildNodes)
            {
                if (node.Name == "windowX" || node.Name == "windowY") continue;
                var target = doc.CreateElement(node.Name == "scatterMode" ? "scatterColorMode" : node.Name);
                target.InnerText = node.InnerText;
                root.AppendChild(target);
            }
            return root.OuterXml;
        }
    }
}
