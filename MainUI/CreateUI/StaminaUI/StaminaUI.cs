using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCheatMainManager.ESP.CreateESPUI;
using UnityCheatMainManager.SaveAndLoad;
using UnityCheatMainManager.UI;
using UnityEngine;
using UniverseLib.UI;
using static UnityCheatMainManager.MainUI.CreateUI.StaminaUI.DrawStaminaNeed;

namespace UnityCheatMainManager.MainUI.CreateUI.StaminaUI
{
    public class Stamina
    {
        public float Current { get; set; }
        public float Max { get; set; }
        public float RestoreRate { get; set; }
        public float RestoreDelay { get; set; }
    }
    // 使用 C# 9.0+ 的 init 语法
 

    public class StaminaUI
    {

        public static bool 模拟体力条 = false;
        public static float CurrentStamina = 100f;
        public static float MaxStamina = 100f;
        public static bool IsDelStamina = false;
        public static StaminaState State = StaminaState.Normal;
        public static float RestoreTimer = 0f;
        public static float RestoreRate = 0.05f;
        public static float RestoreDelay = 4f;
        public static void CreatePlayerStaminaCheat()
        {
            if (模拟体力条)
            {
                DrawStaminaNeed.UpdataStamina();

                DrawStaminaNeed.DrawStaminaBar(CurrentStamina, MaxStamina, 300f, 20f, Color.white,Color.black);

            }

        }
        public static void DrawPlayerStaminaUI(GameObject tabGroup)
        {
            GameObject PlayerStaminaText = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowPlayerStaminaText", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);

            GameObject PlayerStamina = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowPlayerStamina", false, false, true, true, 5, default,
               new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            PlayerStamina.CreateToggle(550, "模拟体力条（测试版，无体力伤害减少90 %）", (state) => { 模拟体力条 = state; });
          
        }
     }
}
