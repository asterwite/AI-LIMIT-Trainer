using BepInEx;
using CommandTerminal;
using CriWare.CriMana;
using GameDef;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using PalmPioneer;
using UnityEngine;
using static BackPackManager;

namespace UnityCheatMainManager.ESP.ESPNeed
{
    public class ESPMain : MonoBehaviour
    {
        public static CommandTerminal.Terminal ConsoleTerminal ;
        private static Harmony harmony;
        private static bool Bonce = false;
        private static List<EffectLightController> TheLocalLight = new List<EffectLightController>();
        private void Start()
        {
            CommandTerminal.Terminal.Init();
            GameObject terminalObj = new GameObject("AI-LIMIT-ConsoleTerminal");
            var Consol = terminalObj.AddComponent<CommandTerminal.Terminal>();
            DontDestroyOnLoad(terminalObj);
            Consol.SetupWindow();
            ConsoleTerminal = Consol;

            //var MonsterDrop = GlobalConfig.ConfigData._MonsterDropByID[0];
            //MonsterDrop.Money = 100;
            //var DropInfo = MonsterDrop.DropItemList;
            //DropInfo[0].ID = 2;
            //DropInfo[0].Count = 1;
            //BackPackManager.PickUpInfo DropItemInfo = new BackPackManager.PickUpInfo();
            //DropKickItemManager.DropKickInfo DropKickItemInfo = new DropKickItemManager.DropKickInfo();

        }

        private void OnGUI()
        {
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
            {
                UnityCheatMainManager.ESP.CreateESPUI.ESPUI.DrawESPCheat();
                UnityCheatMainManager.MainUI.CreateUI.StaminaUI.StaminaUI.CreatePlayerStaminaCheat();
            }
        }
    }
}