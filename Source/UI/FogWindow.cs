using System;
using UnityEngine;
using AtmosphereFX.Config;
using AtmosphereFX.Runtime;

namespace AtmosphereFX.UI
{
    /// <summary>
    /// In-game tuning window mirroring the options panel: every change is
    /// applied live and persisted.
    /// </summary>
    internal sealed class FogWindow
    {
        private Rect _rect = new Rect(200f, 180f, 470f, 470f);
        private Vector2 _scroll;

        internal void Draw(int id)
        {
            _rect = GUI.Window(id, _rect, DrawWindow, "AtmosphereFX v2");
        }

        private void DrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0f, 0f, 440f, 22f));
            if (GUI.Button(new Rect(_rect.width - 26f, 4f, 22f, 18f), "x"))
            {
                AtmosphereEngine.CloseWindow();
            }

            _scroll = GUI.BeginScrollView(new Rect(6f, 26f, _rect.width - 12f, _rect.height - 40f), _scroll,
                new Rect(0f, 0f, _rect.width - 30f, 470f));

            float y = 6f;

            y = Section("Dynamic fog", y);
            ModConfig.DynamicFog = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.DynamicFog, "Enable dynamic fog");
            y += 26f;
            y = Slider("Color decay", ModConfig.ColorDecay, 0f, 1f, 0.01f, y, v => { ModConfig.ColorDecay = v; SettingsApplier.ApplyDynamicFog(); });
            y = Slider("Density", ModConfig.Density, 0f, 0.005f, 0.00005f, y, v => { ModConfig.Density = v; SettingsApplier.ApplyDynamicFog(); }, "0.00000");
            y = Slider("Noise amount", ModConfig.Noise, 0f, 2f, 0.02f, y, v => { ModConfig.Noise = v; SettingsApplier.ApplyDynamicFog(); });
            y = Slider("Fog ceiling", ModConfig.FogHeight, 0f, 5000f, 25f, y, v => { ModConfig.FogHeight = v; SettingsApplier.ApplyDynamicFog(); }, "0");
            y = Slider("Horizon line", ModConfig.HorizonHeight, 0f, 5000f, 25f, y, v => { ModConfig.HorizonHeight = v; SettingsApplier.ApplyDynamicFog(); }, "0");
            y = Slider("Start distance", ModConfig.StartDistance, 0f, 10000f, 25f, y, v => { ModConfig.StartDistance = v; SettingsApplier.ApplyDynamicFog(); }, "0");
            y = Slider("Drift speed", ModConfig.WindSpeed, 0f, 0.05f, 0.001f, y, v => { ModConfig.WindSpeed = v; SettingsApplier.ApplyDynamicFog(); }, "0.000");
            ModConfig.EdgeFog = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.EdgeFog, "Edge fog");
            y += 28f;

            y = Section("Cubemap fog", y);
            ModConfig.CubemapFog = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.CubemapFog, "Enable cubemap fog");
            y += 26f;
            ModConfig.OffAtNight = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.OffAtNight, "Automatic off at night");
            y += 26f;
            ModConfig.VolumeFog = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.VolumeFog, "Volume fog");
            y += 26f;
            y = Slider("Scatter falloff", ModConfig.ScatterFalloff, 0.5f, 10f, 0.05f, y, v => { ModConfig.ScatterFalloff = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Scatter strength", ModConfig.ScatterStrength, 0f, 5f, 0.05f, y, v => { ModConfig.ScatterStrength = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Volume start", ModConfig.VolumeStart, 0f, 4000f, 10f, y, v => { ModConfig.VolumeStart = v; SettingsApplier.ApplyRenderProperties(); }, "0");

            if (GUI.Button(new Rect(6f, y + 6f, 200f, 26f), "Reset to vanilla"))
            {
                ModConfig.RestoreVanilla();
                SettingsApplier.ApplyAll();
            }

            GUI.EndScrollView();

            ConfigStore.Save();
        }

        private static float Section(string title, float y)
        {
            GUI.Label(new Rect(6f, y, 300f, 24f), "<b>" + title + "</b>");
            return y + 26f;
        }

        private static float Slider(string label, float value, float min, float max, float step, float y, Action<float> onChange, string format = "0.00")
        {
            GUI.Label(new Rect(6f, y, 100f, 22f), label);
            float raw = GUI.HorizontalSlider(new Rect(110f, y + 3f, 250f, 20f), value, min, max);
            float snapped = Mathf.Round(raw / step) * step;
            GUI.Label(new Rect(368f, y, 90f, 22f), snapped.ToString(format));
            if (!Mathf.Approximately(snapped, value))
            {
                onChange(snapped);
            }

            return y + 26f;
        }
    }
}
