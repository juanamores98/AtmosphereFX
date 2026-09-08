using System.IO;
using System.Reflection;

namespace AtmosphereFX.Config
{
    internal static class QuickPresets
    {
        internal static bool ApplyVanilla()
        {
            return AtmosphereFXMod.ApplySuiteSection("<atmospherefx><vanillaMode>true</vanillaMode></atmospherefx>");
        }

        internal static bool ApplyOptimized()
        {
            using (var stream = typeof(QuickPresets).Assembly.GetManifestResourceStream("AtmosphereFX.BuiltIns.Optimized.xml"))
            {
                if (stream == null) return false;
                using (var reader = new StreamReader(stream)) return AtmosphereFXMod.ApplySuiteSection(reader.ReadToEnd());
            }
        }
    }
}
