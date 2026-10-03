using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityCheatMainManager.Hook
{
    internal class Hook
    {
        public static void HarmonyHook()
        {
            Harmony.CreateAndPatchAll(typeof(Cheat.FightCheatHookManager), null);
            Harmony.CreateAndPatchAll(typeof(Cheat.SpawnCheat), null);
            Harmony.CreateAndPatchAll(typeof(Cheat.StaminaCheat), null);
        }
    }
}
