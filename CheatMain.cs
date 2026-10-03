using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UniverseLib;
using UnityCheatMainManager.UI;
using UnityCheatMainManager.UI.Models;
using System.Text;
using UnityCheatMainManager.MainUI;
using UnityCheatMainManager.ESP.ESPNeed;
using static UnityCheatMainManager.WindowsMessageBoxPlugin;
using static UnityCheatMainManager.CMPDLL;
namespace UnityCheatMainManager
{
    public static class BepInExShow
    {
        public const string PLUGIN_GUID = "AI-LIMITCheatManager";

        public const string PLUGIN_NAME = "无限机兵";

        public const string PLUGIN_VERSION = "1.2";

        public const string For_Name = "By：无云五蕴";
    }


    [BepInPlugin(BepInExShow.PLUGIN_GUID, BepInExShow.PLUGIN_NAME, BepInExShow.PLUGIN_VERSION)]
    public class CheatMain :  BasePlugin
    {
        public static CheatMain Instance;
        public ManualLogSource LogSource
            => Log;
        public string UnhollowedModulesFolder => Path.Combine(Paths.BepInExRootPath);

        public static int i = 0;

        public static ConfigEntry<KeyCode> ShowCounter { get; set; }
        public static ConfigEntry<int> FontSize { get; set; }
        public static ConfigEntry<string> PanelPosition { get; set; }

        public static ConfigEntry<float>  CurrentStamina { get; set; }
        public static ConfigEntry<float> MaxStamina { get; set; }
        public static ConfigEntry<float> RestoreRate { get; set; }
        public static ConfigEntry<float> RestoreDelay { get; set; }

        public static void WriteLog(object message, LogType logType = LogType.Log)
        {
            CheatMain.Instance.ILog(message, logType);
        }
        public void ILog(object message, LogType logType = LogType.Log)
        {
            string text = (message?.ToString()) ?? "";
            switch (logType)
            {
                case LogType.Error:
                case LogType.Exception:
                    LogSource.LogError(text);
                    break;
                case LogType.Assert:
                case LogType.Log:
                    LogSource.LogMessage(text);
                    break;
                case LogType.Warning:
                    LogSource.LogWarning(text);
                    break;
            }
        }

        private void Init()
        {
            Instance = this;


            Hook.Hook.HarmonyHook();

            ShowCounter = Config.Bind("界面呼出", "快捷键", KeyCode.Insert);
            FontSize = Config.Bind("字体设置", "字体大小", 17);
            PanelPosition = Config.Bind("界面位置", "界面位置", "");

            CurrentStamina = Config.Bind("模拟体力", "当前体力", 100f);
            MaxStamina = Config.Bind("模拟体力", "最大体力", 100f);
            RestoreRate = Config.Bind("模拟体力", "每帧恢复多少体力", 0.05f);
            RestoreDelay = Config.Bind("模拟体力", "恢复需要等待多少秒", 4f);

            MainUI.CreateUI.FightUI.QuickUseMagic.UseMagicShowKey = Config.Bind("快速魔法", "使用魔法快捷键", KeyCode.E);

            MainUI.CreateUI.StaminaUI.StaminaUI.CurrentStamina = CurrentStamina.Value;
            MainUI.CreateUI.StaminaUI.StaminaUI.MaxStamina = MaxStamina.Value;
            MainUI.CreateUI.StaminaUI.StaminaUI.RestoreDelay = RestoreDelay.Value;
            MainUI.CreateUI.StaminaUI.StaminaUI.RestoreRate = RestoreRate.Value;
            Console.OutputEncoding = Encoding.UTF8;


            //CMPDLL.CheckIntegrity();

            Universe.Init(1.0f, new Action(LateInit), new Action<string, LogType>(ILog), new UniverseLib.Config.UniverseLibConfig()
            {
                Force_Unlock_Mouse = false,
                Unhollowed_Modules_Folder = UnhollowedModulesFolder,
                Disable_EventSystem_Override = false,
                Allow_UI_Selection_Outside_UIBase = true,
            });

            ILog($"正在加载修改...");
            CheatBehaviour.Setup();
        }
     
        private void LateInit()
        {
            ILog($"正在创建UI界面...");

            UnityCheatMainManager.UI.ManagerUI.InitUI();
            ILog($"OJBK！");
        }
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<TooltipGUI>();
            Init();
        }
    }
}
