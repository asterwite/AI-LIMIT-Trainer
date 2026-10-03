using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UniverseLib.Input;
using UniverseLib;

namespace UnityCheatMainManager.UI
{
    public static class DisplayManager
    {
        public static int ActiveDisplayIndex { get; private set; }
        public static Display ActiveDisplay => Display.displays[ActiveDisplayIndex];

        public static int Width => ActiveDisplay.renderingWidth;
        public static int Height => ActiveDisplay.renderingHeight;

        public static Vector3 MousePosition => Application.isEditor
            ? UniverseLib.Input.InputManager.MousePosition
            : Display.RelativeMouseAt(UniverseLib.Input.InputManager.MousePosition);

        public static bool MouseInTargetDisplay => MousePosition.z == ActiveDisplayIndex;

        private static Camera canvasCamera;

        internal static void Init()
        {
        }

        public static void SetDisplay(int display)
        {
            if (ActiveDisplayIndex == display)
                return;

            if (Display.displays.Length <= display)
            {
                CheatMain.WriteLog($"Cannot set display index to {display} as there are not enough monitors connected!");

                //if (ConfigManager.Target_Display.Value == display)
                //    ConfigManager.Target_Display.Value = 0;

                return;
            }

            ActiveDisplayIndex = display;
            ActiveDisplay.Activate();

            ManagerUI.UICanvas.targetDisplay = display;

            // ensure a camera is targeting the display
            if (!Camera.main || Camera.main.targetDisplay != display)
            {
                if (!canvasCamera)
                {
                    canvasCamera = new GameObject("UnityExplorer_CanvasCamera").AddComponent<Camera>();
                    GameObject.DontDestroyOnLoad(canvasCamera.gameObject);
                    canvasCamera.hideFlags = HideFlags.HideAndDontSave;
                }
                canvasCamera.targetDisplay = display;
            }

            RuntimeHelper.StartCoroutine(FixPanels());
        }

        private static IEnumerator FixPanels()
        {
            yield return null;
            yield return null;

            foreach (Panels.CheatMainPanel panel in ManagerUI.UIPanels.Values)
            {
                panel.EnsureValidSize();
                panel.EnsureValidPosition();
                panel.Dragger.OnEndResize();
            }
        }
    }
}
