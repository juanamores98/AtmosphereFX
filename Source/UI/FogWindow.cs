using System;
using UnityEngine;
using AtmosphereFX.Config;
using AtmosphereFX.Runtime;

namespace AtmosphereFX.UI
{
    /// <summary>
    /// In-game tuning window mirroring the options panel: every change is
    /// applied live and persisted (state saves are throttled).
    /// </summary>
    internal sealed class FogWindow
    {
        private static readonly string[] ScatterModes = { "Auto (vanilla)", "Sun matched", "Custom" };

        private Rect _rect = new Rect(40f, 60f, 470f, 500f);
        private Vector2 _scroll;
        private float _lastSave = -10f;

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
                new Rect(0f, 0f, _rect.width - 30f, 700f));

            float y = 6f;

            // ---- Dynamic fog ----
            y = Section("Dynamic fog", y);
            bool dynamicFog = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.DynamicFog, "Enable dynamic fog");
            if (dynamicFog != ModConfig.DynamicFog)
            {
                ModConfig.DynamicFog = dynamicFog;
                SettingsApplier.ApplyDynamicFogEffect();
                Persist();
            }
            y += 26f;

            y = Slider("Color decay", ModConfig.ColorDecay, 0f, 1f, 0.01f, y, v => { ModConfig.ColorDecay = v; SettingsApplier.ApplyDynamicFog(); });
            y = Slider("Density", ModConfig.Density, 0f, 0.005f, 0.00005f, y, v => { ModConfig.Density = v; SettingsApplier.ApplyDynamicFog(); }, "0.00000");
            y = Slider("Noise amount", ModConfig.Noise, 0f, 2f, 0.02f, y, v => { ModConfig.Noise = v; SettingsApplier.ApplyDynamicFog(); });
            y = Slider("Fog ceiling", ModConfig.FogHeight, 0f, 5000f, 25f, y, v => { ModConfig.FogHeight = v; SettingsApplier.ApplyDynamicFog(); }, "0");
            y = Slider("Horizon line", ModConfig.HorizonHeight, 0f, 5000f, 25f, y, v => { ModConfig.HorizonHeight = v; SettingsApplier.ApplyDynamicFog(); }, "0");
            y = Slider("Start distance", ModConfig.StartDistance, 0f, 10000f, 25f, y, v => { ModConfig.StartDistance = v; SettingsApplier.ApplyDynamicFog(); }, "0");
            y = Slider("Drift speed", ModConfig.WindSpeed, 0f, 0.05f, 0.001f, y, v => { ModConfig.WindSpeed = v; SettingsApplier.ApplyDynamicFog(); }, "0.000");

            bool edgeFog = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.EdgeFog, "Edge fog");
            if (edgeFog != ModConfig.EdgeFog)
            {
                ModConfig.EdgeFog = edgeFog;
                SettingsApplier.ApplyDynamicFog();
                SettingsApplier.ApplyCubemapFog();
                Persist();
            }
            y += 28f;

            // ---- Cubemap fog ----
            y = Section("Cubemap fog", y);
            bool cubemapFog = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.CubemapFog, "Enable cubemap fog");
            if (cubemapFog != ModConfig.CubemapFog)
            {
                ModConfig.CubemapFog = cubemapFog;
                SettingsApplier.ApplyCubemapFog();
                Persist();
            }
            y += 26f;

            bool offAtNight = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.OffAtNight, "Automatic off at night");
            if (offAtNight != ModConfig.OffAtNight)
            {
                ModConfig.OffAtNight = offAtNight; // picked up by the per-frame watcher
                Persist();
            }
            y += 26f;

            bool volumeFog = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.VolumeFog, "Volume fog");
            if (volumeFog != ModConfig.VolumeFog)
            {
                ModConfig.VolumeFog = volumeFog;
                SettingsApplier.ApplyRenderProperties();
                Persist();
            }
            y += 26f;

            y = Slider("Scatter falloff", ModConfig.ScatterFalloff, 0.5f, 10f, 0.05f, y, v => { ModConfig.ScatterFalloff = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Scatter strength", ModConfig.ScatterStrength, 0f, 5f, 0.05f, y, v => { ModConfig.ScatterStrength = v; SettingsApplier.ApplyRenderProperties(); });

            int scatterMode = GUI.SelectionGrid(new Rect(6f, y, 400f, 24f), ModConfig.ScatterColorMode, ScatterModes, 3);
            if (scatterMode != ModConfig.ScatterColorMode)
            {
                ModConfig.ScatterColorMode = scatterMode;
                SettingsApplier.ApplyRenderProperties();
                Persist();
            }
            y += 32f;

            y = Slider("Scatter red", ModConfig.ScatterR, 0f, 1f, 0.01f, y, v => { ModConfig.ScatterR = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Scatter green", ModConfig.ScatterG, 0f, 1f, 0.01f, y, v => { ModConfig.ScatterG = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Scatter blue", ModConfig.ScatterB, 0f, 1f, 0.01f, y, v => { ModConfig.ScatterB = v; SettingsApplier.ApplyRenderProperties(); });

            bool autoVolume = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.AutoVolumeColor, "Automatic volume color");
            if (autoVolume != ModConfig.AutoVolumeColor)
            {
                ModConfig.AutoVolumeColor = autoVolume;
                SettingsApplier.ApplyRenderProperties();
                Persist();
            }
            y += 26f;

            y = Slider("Volume red", ModConfig.VolumeR, 0f, 1f, 0.01f, y, v => { ModConfig.VolumeR = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Volume green", ModConfig.VolumeG, 0f, 1f, 0.01f, y, v => { ModConfig.VolumeG = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Volume blue", ModConfig.VolumeB, 0f, 1f, 0.01f, y, v => { ModConfig.VolumeB = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Volume start", ModConfig.VolumeStart, 0f, 4000f, 10f, y, v => { ModConfig.VolumeStart = v; SettingsApplier.ApplyRenderProperties(); }, "0");

            if (GUI.Button(new Rect(6f, y + 6f, 200f, 26f), "Reset to vanilla"))
            {
                ModConfig.RestoreVanilla();
                SettingsApplier.ApplyAll();
                Persist();
            }

            GUI.EndScrollView();
        }

        private static void Persist()
        {
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
                ConfigStore.Save();
            }

            return y + 26f;
        }
    }
}
