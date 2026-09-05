using UnityEngine;
using AtmosphereFX.UI;

namespace AtmosphereFX.Runtime
{
    /// <summary>
    /// Scene host for the in-game window (F12) and the tray button state.
    /// </summary>
    public class AtmosphereEngine : MonoBehaviour
    {
        private static bool _open;

        private FogWindow _window;
        private int _windowId;

        internal static void OpenWindow()
        {
            _open = true;
        }

        internal static void CloseWindow()
        {
            _open = false;
        }

        private void Start()
        {
            _windowId = GetInstanceID();
            _window = new FogWindow();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                _open = !_open;
            }
        }

        private void OnGUI()
        {
            if (_open)
            {
                _window.Draw(_windowId);
            }
        }
    }
}
