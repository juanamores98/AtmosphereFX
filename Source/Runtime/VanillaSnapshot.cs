namespace AtmosphereFX.Runtime
{
    internal static class VanillaSnapshot
    {
        // Compatibility boundary. Each scalar/flag is captured at acquisition by
        // PropertyLedger, including components created after the level callback.
        internal static void Capture() { }
        internal static void ResetCapture() { }
        internal static void Restore() { Infrastructure.PropertyLedger.ReleaseAll(); }
    }
}
