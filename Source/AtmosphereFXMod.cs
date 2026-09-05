using ICities;
using AtmosphereFX.Config;
using AtmosphereFX.Options;
using AtmosphereFX.Runtime;

namespace AtmosphereFX
{
    /// <summary>
    /// AtmosphereFX v2 entry point: independent fog and atmosphere tuning for
    /// Cities: Skylines. Original implementation, v2 model.
    /// </summary>
    public class AtmosphereFXMod : LoadingExtensionBase, IUserMod
    {
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
        }
    }
}
