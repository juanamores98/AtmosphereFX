using UnityEngine;
namespace AtmosphereFX.Runtime
{
    public class AtmosphereEngine : MonoBehaviour
    {
        internal static void OpenWindow() { FxModule.OpenStandalone(); }
        internal static void CloseWindow() { FxModule.CloseStandalone(); }
        public static void ToggleWindow() { FxModule.OpenStandalone(true); }
        private void Update() { if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.A)) ToggleWindow(); Config.ConfigStore.CheckPendingSave(); }
        private void OnDestroy() { Config.ConfigStore.SaveImmediate(); FxModule.CloseStandalone(); }
    }
}
