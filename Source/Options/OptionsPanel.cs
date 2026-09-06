using System;
using UnityEngine;
using ColossalFramework.UI;
using ICities;
using AtmosphereFX.Config;
using AtmosphereFX.Runtime;

namespace AtmosphereFX.Options
{
    /// <summary>
    /// v2 options panel. Ranges and grouping are specific to this version:
    /// every control writes directly into the v2 configuration model.
    /// </summary>
    internal static class OptionsPanel
    {
        private static readonly string[] ScatterModes = { "Auto (vanilla)", "Sun matched", "Custom" };

        internal static void Build(UIHelperBase helper)
        {
            BuildDynamicGroup(helper);
            BuildCubemapGroup(helper);
            BuildGeneralGroup(helper);
        }

        private static void BuildDynamicGroup(UIHelperBase helper)
        {
            var group = helper.AddGroup("Dynamic Fog");

            group.AddCheckbox("Enable dynamic fog", ModConfig.DynamicFog, sel =>
            {
                ModConfig.DynamicFog = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyDynamicFogEffect();
            });

            OptionWidgets.AddValueSlider(group, "Color decay", 0f, 1f, 0.01f, ModConfig.ColorDecay, true, sel =>
            {
                ModConfig.ColorDecay = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyDynamicFog();
            });

            OptionWidgets.AddValueSlider(group, "Density", 0f, 0.005f, 0.00005f, ModConfig.Density, true, sel =>
            {
                ModConfig.Density = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyDynamicFog();
            });

            OptionWidgets.AddValueSlider(group, "Noise amount", 0f, 2f, 0.02f, ModConfig.Noise, true, sel =>
            {
                ModConfig.Noise = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyDynamicFog();
            });

            OptionWidgets.AddValueSlider(group, "Fog ceiling", 0f, 5000f, 25f, ModConfig.FogHeight, true, sel =>
            {
                ModConfig.FogHeight = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyDynamicFog();
            });

            OptionWidgets.AddValueSlider(group, "Horizon line", 0f, 5000f, 25f, ModConfig.HorizonHeight, true, sel =>
            {
                ModConfig.HorizonHeight = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyDynamicFog();
            });

            OptionWidgets.AddValueSlider(group, "Start distance", 0f, 10000f, 25f, ModConfig.StartDistance, true, sel =>
            {
                ModConfig.StartDistance = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyDynamicFog();
            });

            OptionWidgets.AddValueSlider(group, "Drift speed", 0f, 0.05f, 0.001f, ModConfig.WindSpeed, true, sel =>
            {
                ModConfig.WindSpeed = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyDynamicFog();
            });

            group.AddCheckbox("Edge fog", ModConfig.EdgeFog, sel =>
            {
                ModConfig.EdgeFog = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyDynamicFog();
                SettingsApplier.ApplyCubemapFog();
            });
        }

        private static void BuildCubemapGroup(UIHelperBase helper)
        {
            var group = helper.AddGroup("Cubemap Fog");

            group.AddCheckbox("Enable cubemap fog", ModConfig.CubemapFog, sel =>
            {
                ModConfig.CubemapFog = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyCubemapFog();
            });

            group.AddCheckbox("Automatic off at night", ModConfig.OffAtNight, sel =>
            {
                ModConfig.OffAtNight = sel;
                ConfigStore.Save();
            });

            group.AddCheckbox("Volume fog", ModConfig.VolumeFog, sel =>
            {
                ModConfig.VolumeFog = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });

            OptionWidgets.AddValueSlider(group, "Scatter falloff", 0.5f, 10f, 0.05f, ModConfig.ScatterFalloff, true, sel =>
            {
                ModConfig.ScatterFalloff = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });

            OptionWidgets.AddValueSlider(group, "Scatter strength", 0f, 5f, 0.05f, ModConfig.ScatterStrength, true, sel =>
            {
                ModConfig.ScatterStrength = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });

            group.AddDropdown("Scatter color", ScatterModes, ModConfig.ScatterColorMode, sel =>
            {
                ModConfig.ScatterColorMode = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });

            AddChannelSlider(group, "Scatter red", ModConfig.ScatterR, sel =>
            {
                ModConfig.ScatterR = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });

            AddChannelSlider(group, "Scatter green", ModConfig.ScatterG, sel =>
            {
                ModConfig.ScatterG = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });

            AddChannelSlider(group, "Scatter blue", ModConfig.ScatterB, sel =>
            {
                ModConfig.ScatterB = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });

            group.AddCheckbox("Automatic volume color", ModConfig.AutoVolumeColor, sel =>
            {
                ModConfig.AutoVolumeColor = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });

            AddChannelSlider(group, "Volume red", ModConfig.VolumeR, sel =>
            {
                ModConfig.VolumeR = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });

            AddChannelSlider(group, "Volume green", ModConfig.VolumeG, sel =>
            {
                ModConfig.VolumeG = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });

            AddChannelSlider(group, "Volume blue", ModConfig.VolumeB, sel =>
            {
                ModConfig.VolumeB = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });

            OptionWidgets.AddValueSlider(group, "Volume start", 0f, 4000f, 10f, ModConfig.VolumeStart, true, sel =>
            {
                ModConfig.VolumeStart = sel;
                ConfigStore.Save();
                SettingsApplier.ApplyRenderProperties();
            });
        }

        private static void BuildGeneralGroup(UIHelperBase helper)
        {
            var general = helper.AddGroup("General");

            general.AddCheckbox("Vanilla mode (suspend the mod)", ModConfig.VanillaMode, sel =>
            {
                ModConfig.VanillaMode = sel;
                ConfigStore.Save();
                if (sel)
                {
                    SettingsApplier.RestoreGameDefaults();
                }
                else
                {
                    SettingsApplier.ApplyAll();
                }
            });

            general.AddCheckbox("Apply when a map loads", ModConfig.ApplyOnLoad, sel =>
            {
                ModConfig.ApplyOnLoad = sel;
                ConfigStore.Save();
            });

            general.AddButton("Reset to vanilla", () =>
            {
                ModConfig.RestoreVanilla();
                ConfigStore.Save();
                SettingsApplier.ApplyAll();
            });
        }

        private static void AddChannelSlider(UIHelperBase group, string label, float current, Action<float> onChange)
        {
            OptionWidgets.AddValueSlider(group, label, 0f, 1f, 0.01f, current, false, sel =>
            {
                onChange(sel);
                SettingsApplier.ApplyRenderProperties();
            });
        }
    }
}
