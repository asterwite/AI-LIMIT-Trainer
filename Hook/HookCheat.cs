using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityCheatMainManager.MainUI;
namespace UnityCheatMainManager.Hook
{
    internal class HookCheat
    {
        private static bool GetItemDataBoce = false;
        public static void UpdateCheat()
        {
            var Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
            {
                BasicUI.DrawBasicUICheat();
                MainUI.CreateUI.FightUI.FightWindowUI.FightWindowUICheat(Player);
                Spawn.CreateSpawnUI.Buff.BuffSet.BuffCheat(Player);
                if (!GetItemDataBoce)
                {
                    ItemSpawn.ItemSet.GetBaseItemData();
                    GetItemDataBoce = true;
                }

            }   
        }
    }
}
