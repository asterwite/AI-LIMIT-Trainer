using System.Reflection;
using UnityEngine;
using UniverseLib.Input;
using UniverseLib;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;
using Il2CppInterop.Runtime.Injection;
using UnityCheatMainManager.ESP.ESPNeed;
using SenseGame;

namespace UnityCheatMainManager
{
    public class CheatBehaviour : MonoBehaviour
    {   
        internal static CheatBehaviour Instance { get; private set; }
        public static Dictionary<string, Action> Actions { get; set; } = new Dictionary<string, Action>();
        public CheatBehaviour(System.IntPtr ptr) : base(ptr) { }
        internal static void Setup()
        {
            try
            {
                if (!ClassInjector.IsTypeRegisteredInIl2Cpp<CheatBehaviour>())
                    ClassInjector.RegisterTypeInIl2Cpp<CheatBehaviour>();
                GameObject BaseGameObject = new GameObject("UnityCheatMainManagerBehaviour");
                BaseGameObject.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(BaseGameObject);
                Instance = BaseGameObject.AddComponent<CheatBehaviour>();
                GameObject EspGameObject = new GameObject("UnityCheatMainManagerESPUpdataBehaviour");
                EspGameObject.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(EspGameObject);
                if (!ClassInjector.IsTypeRegisteredInIl2Cpp<ESPMain>())
                {
                    ClassInjector.RegisterTypeInIl2Cpp<ESPMain>();
                }
                EspGameObject.AddComponent<ESPMain>();
            }
            catch (Exception e)
            {
                CheatMain.Instance.ILog($"Setup 失败：{e.Message}", LogType.Error);
                CheatMain.Instance.ILog($"堆栈跟踪：{e.StackTrace}", LogType.Error);
            }
        }
        internal void Update()
        {
            if (UniverseLib.Input.InputManager.GetKeyDown(KeyCode.F1) && MainUI.BasicUI.ShowDev)
                GameObject.FindAnyObjectByType<SenseGame.DevelopUIInputHandle>().Show();



            if (UnityCheatMainManager.UI.Panels.OptionWindow.ShowKey == CheatMain.ShowCounter.Value || UnityCheatMainManager.UI.Panels.OptionWindow.RestShowWindow && !UnityCheatMainManager.UI.Panels.OptionWindow.Bonce)
            {
                UnityCheatMainManager.UI.ManagerUI.OnShowCounterValueChanged(UnityCheatMainManager.UI.ManagerUI.MycloseBtn, UnityCheatMainManager.UI.ManagerUI.navbarPanelObject);
                UnityCheatMainManager.UI.Panels.OptionWindow.Bonce = true;
                UnityCheatMainManager.UI.Panels.OptionWindow.RestShowWindow = false;
            }

            Hook.HookCheat.UpdateCheat();
            if (UniverseLib.Input.InputManager.GetKeyDown(CheatMain.ShowCounter.Value))
            {
                UnityCheatMainManager.UI.ManagerUI.ShowMenu = !UnityCheatMainManager.UI.ManagerUI.ShowMenu;
                UniverseLib.Config.ConfigManager.Force_Unlock_Mouse = !UniverseLib.Config.ConfigManager.Force_Unlock_Mouse;
            }
            foreach (var action in Actions)
            {
                action.Value.Invoke();
            }      
        }
        internal void OnDestroy()
        {
            OnApplicationQuit();
        }

        internal void DelUpdateAction(string actionName)
        {
            if(Actions.ContainsKey(actionName)) 
            {
                Actions.Remove(actionName);
            }
        }
        internal void AddUpdateAction(string actionName, Action action)
        {
            if (!Actions.ContainsKey(actionName))
            {
                Actions.Add(actionName, action);
            }
        }
        internal bool quitting;
        internal void OnApplicationQuit()
        {
            if (quitting) return;
            quitting = true;

            TryDestroy(UnityCheatMainManager.UI.ManagerUI.UIRoot?.transform.root.gameObject);

            TryDestroy((typeof(Universe).Assembly.GetType("UniverseLib.UniversalBehaviour")
                .GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null, null)
                as Component).gameObject);

            TryDestroy(this.gameObject);
        }
        internal void TryDestroy(GameObject obj)
        {
            try
            {
                if (obj)
                    Destroy(obj);
            }
            catch { }
        }      
    }
}
