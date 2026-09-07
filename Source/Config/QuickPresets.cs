using UnityEngine;

namespace AtmosphereFX.Config
{
    /// <summary>
    /// Los dos ajustes de un clic: el del juego sin tocar, y la receta del usuario.
    /// </summary>
    /// <remarks>
    /// <b>Que son.</b> <c>Vanilla</c> deja este mod sin imponer nada: el juego tal cual.
    /// <c>Optimized</c> es la receta calibrada del usuario, la misma que llevaba el preset
    /// «Default» de Render It!+, derivada en su dia de como tenia configurados los mods
    /// clasicos que esta suite sustituye.
    ///
    /// <b>Por que pasan por ApplySuiteSection.</b> Es el mismo camino que recorre un perfil de
    /// suite guardado, con sus validaciones y sus efectos inmediatos. Un atajo que escribiera
    /// los campos por su cuenta se desincronizaria del resto en cuanto alguien anadiera un
    /// ajuste nuevo.
    /// </remarks>
    internal static class QuickPresets
    {
        private const string VanillaXml =
            "<atmospherefx>" +
            "<vanillaMode>true</vanillaMode>" +
            "</atmospherefx>";

        private const string OptimizedXml =
            "<atmospherefx>" +
            "<vanillaMode>false</vanillaMode>" +
            "<dynamicFog>true</dynamicFog>" +
            "<colorDecay>0.21</colorDecay>" +
            "<density>0.00006</density>" +
            "<noise>0.51</noise>" +
            "<fogHeight>500</fogHeight>" +
            "<horizonHeight>1300</horizonHeight>" +
            "<startDistance>2852</startDistance>" +
            "<windSpeed>0</windSpeed>" +
            "<edgeFog>false</edgeFog>" +
            "<cubemapFog>true</cubemapFog>" +
            "<offAtNight>true</offAtNight>" +
            "<volumeFog>true</volumeFog>" +
            "<scatterFalloff>1.7</scatterFalloff>" +
            "<scatterStrength>1.72</scatterStrength>" +
            "<scatterColorMode>0</scatterColorMode>" +
            "<scatterR>0.565</scatterR>" +
            "<scatterG>0.925</scatterG>" +
            "<scatterB>1</scatterB>" +
            "<autoVolumeColor>false</autoVolumeColor>" +
            "<volumeR>0.651</volumeR>" +
            "<volumeG>0.886</volumeG>" +
            "<volumeB>1</volumeB>" +
            "<volumeStart>0</volumeStart>" +
            "</atmospherefx>";

        internal static bool ApplyVanilla()
        {
            return Apply(VanillaXml, "Vanilla");
        }

        internal static bool ApplyOptimized()
        {
            return Apply(OptimizedXml, "Optimized");
        }

        private static bool Apply(string xml, string name)
        {
            bool ok = AtmosphereFX.AtmosphereFXMod.ApplySuiteSection(xml);
            Debug.Log("[AtmosphereFX] preset " + name + (ok ? " aplicado" : " RECHAZADO"));
            return ok;
        }
    }
}
