using System;
using UnityEngine;
using AtmosphereFX.Config;
using AtmosphereFX.Runtime;

namespace AtmosphereFX.UI
{
    /// <summary>
    /// In-game tuning window for AtmosphereFX v2: 2 clean ergonomic tabs,
    /// persistent quick presets, and 1-click Anti-Blue Haze button.
    /// Every change is applied live and persisted with throttle.
    /// </summary>
    internal sealed class FogWindow
    {
        private static readonly string[] Tabs = { "Dynamic fog & horizon", "Volumetrics & scattering" };
        private static readonly string[] ScatterModes = { "Auto (vanilla)", "Sun matched", "Custom" };

        private Rect _rect = new Rect(ModConfig.WindowX, ModConfig.WindowY, 490f, 490f);
        private int _tab;
        private Vector2 _scrollTab0;
        private Vector2 _scrollTab1;

        internal void Draw(int id)
        {
            if (_rect.x != ModConfig.WindowX || _rect.y != ModConfig.WindowY)
            {
                _rect.x = Mathf.Clamp(ModConfig.WindowX, 0f, Mathf.Max(0f, Screen.width - _rect.width));
                _rect.y = Mathf.Clamp(ModConfig.WindowY, 0f, Mathf.Max(0f, Screen.height - _rect.height));
            }

            Rect oldRect = _rect;
            _rect = GUI.Window(id, _rect, DrawWindow, "AtmosphereFX Studio v2");
            if (_rect.x != oldRect.x || _rect.y != oldRect.y)
            {
                _rect.x = Mathf.Clamp(_rect.x, 0f, Mathf.Max(0f, Screen.width - _rect.width));
                _rect.y = Mathf.Clamp(_rect.y, 0f, Mathf.Max(0f, Screen.height - _rect.height));
                ModConfig.WindowX = _rect.x;
                ModConfig.WindowY = _rect.y;
                ConfigStore.Save(false);
            }
        }

        private void DrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0f, 0f, _rect.width - 30f, 22f));
            if (GUI.Button(new Rect(_rect.width - 26f, 4f, 22f, 18f), "x"))
            {
                AtmosphereEngine.CloseWindow();
            }

            float y = 26f;

            // -----------------------------------------------------------------
            // Top Persistent Bar: 1-Click Actions & Anti-Blue Haze
            // -----------------------------------------------------------------
            if (GUI.Button(new Rect(8f, y, 90f, 24f), "Vanilla"))
            {
                QuickPresets.ApplyVanilla();
                Persist();
            }

            if (GUI.Button(new Rect(104f, y, 110f, 24f), "Optimized"))
            {
                QuickPresets.ApplyOptimized();
                Persist();
            }

            if (GUI.Button(new Rect(220f, y, 260f, 24f), "Clear the blue haze"))
            {
                ApplyAntiBlueHaze();
            }

            y += 30f;

            // -----------------------------------------------------------------
            // 2 Ergonomic Tabs
            // -----------------------------------------------------------------
            _tab = GUI.Toolbar(new Rect(8f, y, _rect.width - 16f, 26f), _tab, Tabs);
            y += 32f;

            float contentHeight = _rect.height - y - 10f;
            if (_tab == 0)
            {
                DrawDynamicFogTab(new Rect(8f, y, _rect.width - 16f, contentHeight));
            }
            else
            {
                DrawVolumetricTab(new Rect(8f, y, _rect.width - 16f, contentHeight));
            }
        }

        private void DrawDynamicFogTab(Rect area)
        {
            _scrollTab0 = GUI.BeginScrollView(area, _scrollTab0, new Rect(0f, 0f, area.width - 20f, 410f));
            float y = 4f;

            y = Section("DYNAMIC FOG & HORIZON", y);
            bool dynamicFog = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.DynamicFog, " Dynamic day/night fog");
            if (dynamicFog != ModConfig.DynamicFog)
            {
                ModConfig.DynamicFog = dynamicFog;
                SettingsApplier.ApplyDynamicFogEffect();
                Persist();
            }
            y += 26f;

            y = Slider("Distancia inicio", ModConfig.StartDistance, 0f, 10000f, 25f, y, v => { ModConfig.StartDistance = v; SettingsApplier.ApplyDynamicFog(); }, "0 m");
            y = Slider("Horizon line", ModConfig.HorizonHeight, 0f, 5000f, 25f, y, v => { ModConfig.HorizonHeight = v; SettingsApplier.ApplyDynamicFog(); }, "0 m");
            y = Slider("Techo de niebla", ModConfig.FogHeight, 0f, 5000f, 25f, y, v => { ModConfig.FogHeight = v; SettingsApplier.ApplyDynamicFog(); }, "0 m");
            y = Slider("Densidad", ModConfig.Density, 0f, 0.005f, 0.00005f, y, v => { ModConfig.Density = v; SettingsApplier.ApplyDynamicFog(); }, "0.00000");

            y += 6f;
            y = Section("APARIENCIA & MOVIMIENTO", y);
            y = Slider("Decadencia color", ModConfig.ColorDecay, 0f, 1f, 0.01f, y, v => { ModConfig.ColorDecay = v; SettingsApplier.ApplyDynamicFog(); });
            y = Slider("Cantidad ruido", ModConfig.Noise, 0f, 2f, 0.02f, y, v => { ModConfig.Noise = v; SettingsApplier.ApplyDynamicFog(); });
            y = Slider("Deriva de viento", ModConfig.WindSpeed, 0f, 0.05f, 0.001f, y, v => { ModConfig.WindSpeed = v; SettingsApplier.ApplyDynamicFog(); }, "0.000");

            bool edgeFogDynamic = GUI.Toggle(new Rect(6f, y, 220f, 22f), ModConfig.EdgeFogDynamic, " Edge fog (dynamic)");
            if (edgeFogDynamic != ModConfig.EdgeFogDynamic)
            {
                ModConfig.EdgeFogDynamic = edgeFogDynamic;
                SettingsApplier.ApplyDynamicFog();
                Persist();
            }

            bool edgeFogCubemap = GUI.Toggle(new Rect(230f, y, 220f, 22f), ModConfig.EdgeFogCubemap, " Niebla de borde (cubemap)");
            if (edgeFogCubemap != ModConfig.EdgeFogCubemap)
            {
                ModConfig.EdgeFogCubemap = edgeFogCubemap;
                SettingsApplier.ApplyCubemapFog();
                Persist();
            }
            y += 30f;

            if (GUI.Button(new Rect(6f, y, 220f, 24f), "Reset this tab to vanilla"))
            {
                ModConfig.RestoreVanilla();
                SettingsApplier.ApplyAll();
                Persist();
            }

            GUI.EndScrollView();
        }

        private void DrawVolumetricTab(Rect area)
        {
            _scrollTab1 = GUI.BeginScrollView(area, _scrollTab1, new Rect(0f, 0f, area.width - 20f, 430f));
            float y = 4f;

            y = Section("CUBEMAP & VOLUMEN DE PROFUNDIDAD", y);
            bool cubemapFog = GUI.Toggle(new Rect(6f, y, 220f, 22f), ModConfig.CubemapFog, " Activar cubemap fog");
            if (cubemapFog != ModConfig.CubemapFog)
            {
                ModConfig.CubemapFog = cubemapFog;
                SettingsApplier.ApplyCubemapFog();
                Persist();
            }

            bool offAtNight = GUI.Toggle(new Rect(230f, y, 220f, 22f), ModConfig.OffAtNight, " Apagar de noche");
            if (offAtNight != ModConfig.OffAtNight)
            {
                ModConfig.OffAtNight = offAtNight;
                SettingsApplier.ApplyCubemapFog();
                Persist();
            }
            y += 24f;

            bool volumeFog = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.VolumeFog, " Volumetric fog");
            if (volumeFog != ModConfig.VolumeFog)
            {
                ModConfig.VolumeFog = volumeFog;
                SettingsApplier.ApplyRenderProperties();
                Persist();
            }
            y += 28f;

            y = Section("INSCATTERING & SCATTERING", y);
            y = Slider("Scatter strength", ModConfig.ScatterStrength, 0f, 5f, 0.05f, y, v => { ModConfig.ScatterStrength = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Scatter falloff", ModConfig.ScatterFalloff, 0.5f, 10f, 0.05f, y, v => { ModConfig.ScatterFalloff = v; SettingsApplier.ApplyRenderProperties(); });

            GUI.Label(new Rect(6f, y, 120f, 20f), "Modo de color:");
            int scatterMode = GUI.SelectionGrid(new Rect(125f, y, 320f, 22f), ModConfig.ScatterColorMode, ScatterModes, 3);
            if (scatterMode != ModConfig.ScatterColorMode)
            {
                ModConfig.ScatterColorMode = scatterMode;
                SettingsApplier.ApplyRenderProperties();
                Persist();
            }
            y += 28f;

            y = Slider("Scatter red", ModConfig.ScatterR, 0f, 1f, 0.01f, y, v => { ModConfig.ScatterR = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Scatter green", ModConfig.ScatterG, 0f, 1f, 0.01f, y, v => { ModConfig.ScatterG = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Scatter blue", ModConfig.ScatterB, 0f, 1f, 0.01f, y, v => { ModConfig.ScatterB = v; SettingsApplier.ApplyRenderProperties(); });

            y += 6f;
            y = Section("VOLUMETRIC COLOUR & START", y);
            bool autoVolume = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.AutoVolumeColor, " Automatic volumetric colour");
            if (autoVolume != ModConfig.AutoVolumeColor)
            {
                ModConfig.AutoVolumeColor = autoVolume;
                SettingsApplier.ApplyRenderProperties();
                Persist();
            }
            y += 26f;

            y = Slider("Volumen Rojo", ModConfig.VolumeR, 0f, 1f, 0.01f, y, v => { ModConfig.VolumeR = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Volumen Verde", ModConfig.VolumeG, 0f, 1f, 0.01f, y, v => { ModConfig.VolumeG = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Volumen Azul", ModConfig.VolumeB, 0f, 1f, 0.01f, y, v => { ModConfig.VolumeB = v; SettingsApplier.ApplyRenderProperties(); });
            y = Slider("Inicio volumen", ModConfig.VolumeStart, 0f, 4000f, 10f, y, v => { ModConfig.VolumeStart = v; SettingsApplier.ApplyRenderProperties(); }, "0 m");

            GUI.EndScrollView();
        }

        private static void ApplyAntiBlueHaze()
        {
            ModConfig.ScatterStrength = 0f;
            ModConfig.ScatterColorMode = 2; // Custom
            ModConfig.ScatterR = 0.5f;
            ModConfig.ScatterG = 0.5f;
            ModConfig.ScatterB = 0.5f;
            SettingsApplier.ApplyRenderProperties();
            Persist();
        }

        private static void Persist()
        {
            ConfigStore.Save(false);
        }

        private static float Section(string title, float y)
        {
            GUI.Label(new Rect(6f, y, 380f, 22f), "<b><color=#4FC3F7>" + title + "</color></b>");
            return y + 24f;
        }

        private static float Slider(string label, float value, float min, float max, float step, float y, Action<float> onChange, string format = "0.00")
        {
            GUI.Label(new Rect(6f, y, 120f, 22f), label);

            bool changedBefore = GUI.changed;
            GUI.changed = false;
            float raw = GUI.HorizontalSlider(new Rect(130f, y + 3f, 240f, 20f), value, min, max);
            bool moved = GUI.changed;
            GUI.changed = changedBefore || moved;

            string displayStr;
            if (format.EndsWith("m"))
            {
                displayStr = (moved ? Mathf.RoundToInt(raw) : Mathf.RoundToInt(value)).ToString() + " m";
            }
            else
            {
                displayStr = (moved ? raw : value).ToString(format);
            }
            GUI.Label(new Rect(376f, y, 85f, 22f), displayStr);

            if (moved)
            {
                float snapped = Mathf.Round(raw / step) * step;
                if (!Mathf.Approximately(snapped, value))
                {
                    onChange(snapped);
                    ConfigStore.Save(false);
                }
            }

            return y + 26f;
        }
    }
}
