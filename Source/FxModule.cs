using System;
using System.IO;
using ColossalFramework.UI;
using UnityEngine;
using AtmosphereFX.UI;

namespace AtmosphereFX
{
    /// <summary>Small public boundary for a standalone or embedded native panel.</summary>
    public static class FxModule
    {
        public const float PreferredWidth = 380f;
        public const float PreferredHeight = 540f;
        private static PanelView _standalone;
        public static string Mode { get { return Config.ModConfig.VanillaMode ? "VANILLA" : (Infrastructure.FxStorage.MatchesOptimized(ReadState(), typeof(AtmosphereFXMod)) ? "OPTIMIZED" : "CUSTOM"); } }
        public static string ReadState() { return AtmosphereFXMod.ExportSuiteSection(); }
        public static bool ApplyState(string xml) { return AtmosphereFXMod.ApplySuiteSection(xml); }
        public static void Release() { if (!Config.QuickPresets.ApplyVanilla()) throw new InvalidOperationException("VANILLA could not be applied."); Flush(); }
        public static void ApplyOptimized()
        {
            Infrastructure.FxStorage.LastNote = string.Empty;
            if (!Config.QuickPresets.ApplyOptimized()) throw new InvalidOperationException(AtmosphereFXMod.LastApplyError ?? "Default could not be applied.");
            Flush();

            // Aplicar sin error no es lo mismo que quedar aplicado: otro mod puede administrar
            // el campo. Si algo no cuajo se nombra aqui, en vez de dejar al usuario pulsando.
            string gap = Infrastructure.FxStorage.OptimizedGap(ReadState(), typeof(AtmosphereFXMod));
            if (gap != null) Infrastructure.FxStorage.LastNote = "Applied, but not in effect: " + gap;
        }
        public static void Flush() { Config.ConfigStore.SaveImmediate(); }
        public static string Status { get { return !string.IsNullOrEmpty(Infrastructure.FxStorage.LastError) ? Infrastructure.FxStorage.LastError : !string.IsNullOrEmpty(Infrastructure.FxStorage.LastNote) ? Infrastructure.FxStorage.LastNote : !string.IsNullOrEmpty(Infrastructure.PropertyLedger.LastWarning) ? Infrastructure.PropertyLedger.LastWarning : "Config: " + UiText.Get(Mode); } }

        public static PanelView CreatePanel(UIComponent parent, float width = PreferredWidth, float height = PreferredHeight)
        {
            var view = new PanelView("AtmosphereFX", parent, width, height, Release, ApplyOptimized, () => Status, () => Mode);
            var page0 = view.AddPage("Fog");
            view.Check(page0, "Dynamic fog", () => Config.ModConfig.DynamicFog, v => Edit(() => Config.ModConfig.DynamicFog = v));
            view.Number(page0, "Fog start (m)", () => Config.ModConfig.StartDistance, v => Edit(() => Config.ModConfig.StartDistance = v), 0f, 10000f, 1f);
            view.Number(page0, "Horizon (m)", () => Config.ModConfig.HorizonHeight, v => Edit(() => Config.ModConfig.HorizonHeight = v), 0f, 5000f, 1f);
            view.Number(page0, "Fog ceiling (m)", () => Config.ModConfig.FogHeight, v => Edit(() => Config.ModConfig.FogHeight = v), 0f, 5000f, 1f);
            view.Number(page0, "Density", () => Config.ModConfig.Density, v => Edit(() => Config.ModConfig.Density = v), 0f, 0.005f, 1e-05f);
            view.Number(page0, "Colour decay", () => Config.ModConfig.ColorDecay, v => Edit(() => Config.ModConfig.ColorDecay = v), 0f, 1f, 0.001f);
            view.Number(page0, "Noise", () => Config.ModConfig.Noise, v => Edit(() => Config.ModConfig.Noise = v), 0f, 2f, 0.01f);
            view.Number(page0, "Wind drift", () => Config.ModConfig.WindSpeed, v => Edit(() => Config.ModConfig.WindSpeed = v), 0f, 0.05f, 0.0001f);
            view.Check(page0, "Dynamic edge fog", () => Config.ModConfig.EdgeFogDynamic, v => Edit(() => Config.ModConfig.EdgeFogDynamic = v));
            view.Check(page0, "Classic edge fog", () => Config.ModConfig.EdgeFogCubemap, v => Edit(() => Config.ModConfig.EdgeFogCubemap = v));
            view.Check(page0, "Apply settings when a city loads", () => Config.ModConfig.ApplyOnLoad, v => { Config.ModConfig.ApplyOnLoad = v; Config.ConfigStore.Save(); });
            view.Info(page0, () => AtmosphereFXMod.ApplicationStatus ?? "Settings ready; appearance not yet verified in game");
            var page1 = view.AddPage("Volume");
            view.Action(page1, "Anti-Blue Haze", () => Edit(() => {
                Config.ModConfig.ScatterStrength = 0f;
                Config.ModConfig.ScatterColorMode = 2;
                Config.ModConfig.ScatterR = 0.5f;
                Config.ModConfig.ScatterG = 0.5f;
                Config.ModConfig.ScatterB = 0.5f;
            }));
            view.Check(page1, "Classic fog", () => Config.ModConfig.CubemapFog, v => Edit(() => Config.ModConfig.CubemapFog = v));
            view.Check(page1, "Disable classic fog at night", () => Config.ModConfig.OffAtNight, v => Edit(() => Config.ModConfig.OffAtNight = v));
            view.Check(page1, "Volumetric fog", () => Config.ModConfig.VolumeFog, v => Edit(() => Config.ModConfig.VolumeFog = v));
            view.Choice(page1, "Scatter colour", () => new[] { "Automatic", "Sun matched", "Custom RGB" }, () => Config.ModConfig.ScatterColorMode, v => Edit(() => Config.ModConfig.ScatterColorMode = v));
            view.Number(page1, "Scatter intensity", () => Config.ModConfig.ScatterStrength, v => Edit(() => Config.ModConfig.ScatterStrength = v), 0f, 100f, 0.01f);
            view.Number(page1, "Scatter exponent", () => Config.ModConfig.ScatterFalloff, v => Edit(() => Config.ModConfig.ScatterFalloff = v), 0.5f, 100000f, 0.1f, log: true);
            view.Number(page1, "Scatter red", () => Config.ModConfig.ScatterR, v => Edit(() => Config.ModConfig.ScatterR = v), 0f, 1f, 0.001f, enabled: () => Config.ModConfig.ScatterColorMode == 2);
            view.Number(page1, "Scatter green", () => Config.ModConfig.ScatterG, v => Edit(() => Config.ModConfig.ScatterG = v), 0f, 1f, 0.001f, enabled: () => Config.ModConfig.ScatterColorMode == 2);
            view.Number(page1, "Scatter blue", () => Config.ModConfig.ScatterB, v => Edit(() => Config.ModConfig.ScatterB = v), 0f, 1f, 0.001f, enabled: () => Config.ModConfig.ScatterColorMode == 2);
            view.Check(page1, "Automatic volume colour", () => Config.ModConfig.AutoVolumeColor, v => Edit(() => Config.ModConfig.AutoVolumeColor = v));
            view.Number(page1, "Volume red", () => Config.ModConfig.VolumeR, v => Edit(() => Config.ModConfig.VolumeR = v), 0f, 1f, 0.001f, enabled: () => !Config.ModConfig.AutoVolumeColor);
            view.Number(page1, "Volume green", () => Config.ModConfig.VolumeG, v => Edit(() => Config.ModConfig.VolumeG = v), 0f, 1f, 0.001f, enabled: () => !Config.ModConfig.AutoVolumeColor);
            view.Number(page1, "Volume blue", () => Config.ModConfig.VolumeB, v => Edit(() => Config.ModConfig.VolumeB = v), 0f, 1f, 0.001f, enabled: () => !Config.ModConfig.AutoVolumeColor);
            view.Number(page1, "Volume start (m)", () => Config.ModConfig.VolumeStart, v => Edit(() => Config.ModConfig.VolumeStart = v), 0f, 10000f, 1f);
            var advanced = view.AddPage("Depth");
            view.Choice(advanced, "Classic volume fog", () => new[] { "Game", "Off", "On" }, () => Config.ModConfig.StaticVolumeFog + 1, v => Edit(() => Config.ModConfig.StaticVolumeFog = v - 1));
            view.OptionalNumber(advanced, "Static Height", () => Config.ModConfig.StaticHeight, v => Edit(() => Config.ModConfig.StaticHeight = v), 0f, 10000f, 1f);
            view.OptionalNumber(advanced, "Static Start", () => Config.ModConfig.StaticStart, v => Edit(() => Config.ModConfig.StaticStart = v), 0f, 10000f, 1f);
            view.OptionalNumber(advanced, "Static Distance", () => Config.ModConfig.StaticDistance, v => Edit(() => Config.ModConfig.StaticDistance = v), 0f, 20000f, 1f);
            view.OptionalNumber(advanced, "Static Edge Distance", () => Config.ModConfig.StaticEdgeDistance, v => Edit(() => Config.ModConfig.StaticEdgeDistance = v), 0f, 20000f, 1f);
            view.OptionalNumber(advanced, "Volume Height", () => Config.ModConfig.VolumeHeight, v => Edit(() => Config.ModConfig.VolumeHeight = v), 0f, 10000f, 1f);
            view.OptionalNumber(advanced, "Volume Density", () => Config.ModConfig.VolumeDensity, v => Edit(() => Config.ModConfig.VolumeDensity = v), 0f, 0.01f, 1e-05f, log: true);
            view.OptionalNumber(advanced, "Volume Distance", () => Config.ModConfig.VolumeDistance, v => Edit(() => Config.ModConfig.VolumeDistance = v), 0f, 20000f, 1f);
            view.OptionalNumber(advanced, "Volume Edge Distance", () => Config.ModConfig.VolumeEdgeDistance, v => Edit(() => Config.ModConfig.VolumeEdgeDistance = v), 0f, 20000f, 1f);
            view.Refresh();
            return view;
        }

        internal static void OpenStandalone(bool toggle = false)
        {
            if (_standalone == null || _standalone.Root == null)
            {
                _standalone = CreatePanel(null, PreferredWidth, Mathf.Min(PreferredHeight, UIView.GetAView().fixedHeight - 24f));
                _standalone.Root.relativePosition = new Vector3(Mathf.Clamp(WindowX, 0f, Mathf.Max(0f, UIView.GetAView().fixedWidth - PreferredWidth)), Mathf.Clamp(WindowY, 0f, Mathf.Max(0f, UIView.GetAView().fixedHeight - _standalone.Root.height)));
                _standalone.Root.eventPositionChanged += (c, value) => { WindowX = value.x; WindowY = value.y; SavePosition(); };
            }
            else _standalone.Root.isVisible = toggle ? !_standalone.Root.isVisible : true;
            var screen = UIView.GetAView();
            _standalone.SetSize(PreferredWidth, Mathf.Min(PreferredHeight, screen.fixedHeight - 24f));
            var pos = _standalone.Root.relativePosition;
            _standalone.Root.relativePosition = new Vector3(Mathf.Clamp(pos.x, 0f, Mathf.Max(0f, screen.fixedWidth - _standalone.Root.width)), Mathf.Clamp(pos.y, 0f, Mathf.Max(0f, screen.fixedHeight - _standalone.Root.height)));
            _standalone.Refresh();
        }

        internal static void CloseStandalone()
        {
            if (_standalone != null) _standalone.Dispose();
            _standalone = null;
        }
        private static void Edit(Action edit)
        {
            edit(); Config.ModConfig.VanillaMode = false;
            Runtime.SettingsApplier.ApplyAll(); Config.ConfigStore.Save(); Infrastructure.FxInterop.RefreshCompanions(); AtmosphereFXMod.NotifyStateChanged();
        }
        private static float WindowX { get { return Config.ModConfig.WindowX; } set { Config.ModConfig.WindowX = value; } }
        private static float WindowY { get { return Config.ModConfig.WindowY; } set { Config.ModConfig.WindowY = value; } }
        private static void SavePosition() { Config.ConfigStore.Save(); }
    }
}
