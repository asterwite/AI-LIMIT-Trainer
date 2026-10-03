using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCheatMainManager.MainUI.CreateUI.StaminaUI;
namespace UnityCheatMainManager.Cheat
{
    internal class StaminaCheat
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(InputManager), nameof(InputManager.PlayerOrdinaryAttackDownInput))]
        public static bool PlayerInputNoHeavy()
        {
            var Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
            if (StaminaUI.模拟体力条)
                DrawStaminaNeed.ConsumeStamina(4f);
            return true;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(InputManager), nameof(InputManager.PlayerPowerfulAttackDownInput))]
        public static bool PlayerInputHeavy()
        {
            if (StaminaUI.模拟体力条)
            {
                var Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                if (StaminaUI.模拟体力条)
                    DrawStaminaNeed.ConsumeStamina(8f);
            }
            return true;
        }
    }
}
