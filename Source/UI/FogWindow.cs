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

        private Rect _rect = new Rect(ModConfig.WindowX, ModConfig.WindowY, 470f, 500f);
        private Vector2 _scroll;

        /// <summary>
        /// Lo que midio el contenido la ultima vez que se dibujo.
        /// </summary>
        /// <remarks>
        /// El alto del area desplazable estaba fijado a mano. Cuando el contenido crecio por
        /// encima de esa cifra, la barra dejo de llegar al final y el ultimo boton quedo fuera
        /// de alcance: no habia forma de pulsarlo. Medirlo mientras se dibuja y usarlo en el
        /// siguiente fotograma es un fotograma de retraso y ningun numero que mantener.
        /// </remarks>
        private float _contentHeight = 700f;

        internal void Draw(int id)
        {
            if (_rect.x != ModConfig.WindowX || _rect.y != ModConfig.WindowY)
            {
                _rect.x = Mathf.Clamp(ModConfig.WindowX, 0f, Mathf.Max(0f, Screen.width - _rect.width));
                _rect.y = Mathf.Clamp(ModConfig.WindowY, 0f, Mathf.Max(0f, Screen.height - _rect.height));
            }

            Rect oldRect = _rect;
            _rect = GUI.Window(id, _rect, DrawWindow, "AtmosphereFX v2");
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
            GUI.DragWindow(new Rect(0f, 0f, 440f, 22f));
            if (GUI.Button(new Rect(_rect.width - 26f, 4f, 22f, 18f), "x"))
            {
                AtmosphereEngine.CloseWindow();
            }

            _scroll = GUI.BeginScrollView(new Rect(6f, 26f, _rect.width - 12f, _rect.height - 40f), _scroll,
                new Rect(0f, 0f, _rect.width - 30f, _contentHeight));

            float y = 6f;

            // Los dos de un clic, arriba: es lo primero que se busca.
            if (GUI.Button(new Rect(6f, y, 190f, 26f), "Vanilla"))
            {
                QuickPresets.ApplyVanilla();
                Persist();
            }

            if (GUI.Button(new Rect(204f, y, 190f, 26f), "Optimized"))
            {
                QuickPresets.ApplyOptimized();
                Persist();
            }

            y += 34f;

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

            bool edgeFogDynamic = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.EdgeFogDynamic, "Edge fog (dynamic)");
            if (edgeFogDynamic != ModConfig.EdgeFogDynamic)
            {
                ModConfig.EdgeFogDynamic = edgeFogDynamic;
                SettingsApplier.ApplyDynamicFog();
                Persist();
            }
            y += 26f;

            bool edgeFogCubemap = GUI.Toggle(new Rect(6f, y, 380f, 22f), ModConfig.EdgeFogCubemap, "Edge fog (cubemap)");
            if (edgeFogCubemap != ModConfig.EdgeFogCubemap)
            {
                ModConfig.EdgeFogCubemap = edgeFogCubemap;
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
                ModConfig.OffAtNight = offAtNight;
                SettingsApplier.ApplyCubemapFog();
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

            y += 40f;

            // Lo que ocupo de verdad, para que la barra llegue hasta aqui la proxima vez.
            _contentHeight = y;

            GUI.EndScrollView();
        }

        private static void Persist()
        {
            ConfigStore.Save(false);
        }

        private static float Section(string title, float y)
        {
            GUI.Label(new Rect(6f, y, 300f, 24f), "<b><color=#4FC3F7>" + title + "</color></b>");
            return y + 26f;
        }

        /// <summary>
        /// Un deslizador que solo escribe cuando el usuario lo mueve.
        /// </summary>
        /// <remarks>
        /// <b>Que hacia mal.</b> Redondeaba el valor al paso mas cercano y lo comparaba con el
        /// actual; si no coincidian, lo escribia. Como un valor cargado de un preset casi nunca
        /// cae justo en un multiplo del paso, el deslizador reescribia la configuracion sola
        /// nada mas abrir la ventana. Medido: la receta del usuario entraba con densidad
        /// 0.00006, ruido 0.51 y distancia 2852, y la ventana los dejaba en 0.00005, 0.52 y
        /// 2850 sin que nadie tocara nada. Ademas eso disparaba un guardado y una aplicacion
        /// cada vez, lo que se notaba como tirones.
        ///
        /// <b>Como se arregla.</b> IMGUI ya avisa de si un control cambio por accion del
        /// usuario: <c>GUI.changed</c>. Se aisla alrededor del control y solo entonces se
        /// redondea y se escribe. Sin interaccion, el deslizador solo dibuja.
        /// </remarks>
        private static float Slider(string label, float value, float min, float max, float step, float y, Action<float> onChange, string format = "0.00")
        {
            GUI.Label(new Rect(6f, y, 100f, 22f), label);

            bool changedBefore = GUI.changed;
            GUI.changed = false;
            float raw = GUI.HorizontalSlider(new Rect(110f, y + 3f, 250f, 20f), value, min, max);
            bool moved = GUI.changed;
            GUI.changed = changedBefore || moved;

            GUI.Label(new Rect(368f, y, 90f, 22f), (moved ? raw : value).ToString(format));

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
