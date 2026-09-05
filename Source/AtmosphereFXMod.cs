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
    }
}
