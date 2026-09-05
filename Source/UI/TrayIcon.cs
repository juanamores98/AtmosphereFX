using UnityEngine;

namespace AtmosphereFX.UI
{
    /// <summary>
    /// Procedurally drawn tray icon: layered fog bands on a slate disc.
    /// </summary>
    internal static class TrayIcon
    {
        internal static Texture2D Make()
        {
            const int size = 48;
            var tex = new Texture2D(size, size, TextureFormat.ARGB32, false)
            {
                name = "AtmosphereFX.tray"
            };

            var center = new Vector2((size - 1) / 2f, (size - 1) / 2f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    Color c = new Color(0f, 0f, 0f, 0f);
                    if (distance < 22f)
                    {
                        c = new Color32(52, 66, 84, 255); // slate disc
                    }

                    if (distance < 22f)
                    {
                        // Three soft fog bands.
                        float band = Mathf.Sin((y - size * 0.5f) * 0.55f);
                        float fog = Mathf.Clamp01(band * 1.4f + 0.5f);
                        float edge = 1f - Mathf.Abs(x - center.x) / 22f;
                        float coverage = Mathf.Clamp01(fog * edge * 1.2f);
                        c = Color.Lerp(c, new Color(0.86f, 0.92f, 1f, 1f), coverage * 0.85f);
                    }

                    tex.SetPixel(x, y, c);
                }
            }

            tex.Apply(false, true);
            return tex;
        }
    }
}
