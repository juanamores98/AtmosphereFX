using System;
using UnityEngine;
using ColossalFramework.UI;
using ICities;

namespace AtmosphereFX.Options
{
    /// <summary>
    /// Factory helpers for the option widgets, including the value/min/max
    /// labels shown next to sliders.
    /// </summary>
    internal static class OptionWidgets
    {
        internal static UISlider AddValueSlider(
            UIHelperBase group,
            string text,
            float min,
            float max,
            float step,
            float value,
            bool showValueLabel,
            Action<float> onChange)
        {
            UISlider slider = null;
            slider = (UISlider)group.AddSlider(text, min, max, step, value, sel =>
            {
                if (showValueLabel)
                {
                    RefreshValueLabel(slider, sel);
                }

                onChange(sel);
            });

            if (showValueLabel)
            {
                AttachValueLabels(slider, value.ToString());
            }

            return slider;
        }

        internal static void RefreshValueLabel(UISlider slider, float value)
        {
            var labels = slider.GetComponentsInChildren<UILabel>();
            labels[0].text = value.ToString();
        }

        private static void AttachValueLabels(UISlider slider, string valueText)
        {
            slider.size = new Vector2(400f, slider.size.y);

            var valueLabel = slider.AddUIComponent<UILabel>();
            valueLabel.text = valueText;
            valueLabel.position = new Vector3(slider.size.x + 20f, 0f, 0f);

            var minLabel = slider.AddUIComponent<UILabel>();
            var maxLabel = slider.AddUIComponent<UILabel>();

            minLabel.text = slider.minValue.ToString();
            maxLabel.text = slider.maxValue.ToString();

            minLabel.textScale = 0.8f;
            maxLabel.textScale = 0.8f;

            minLabel.color = new Color32(minLabel.color.r, minLabel.color.g, minLabel.color.b, 50);
            maxLabel.color = new Color32(maxLabel.color.r, maxLabel.color.g, maxLabel.color.b, 50);

            minLabel.position = new Vector3(slider.position.x, -slider.size.y, 0f);
            maxLabel.position = new Vector3(slider.size.x - maxLabel.size.x, -slider.size.y, 0f);

            slider.parent.size = new Vector2(slider.parent.size.x, slider.parent.size.y + 10f);
        }
    }
}
