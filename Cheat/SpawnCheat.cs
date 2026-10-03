using BehaviorDesigner.Runtime.Tasks;
using CriWare.CriMana;
using HarmonyLib;
using SceneSettingDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static FMAAProcessVolume;

namespace UnityCheatMainManager.Cheat
{
    internal class SpawnCheat
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ActorManager), nameof(ActorManager.SpawnMonster))]
        public static void SpawnMonster(GameObject monsterRootGameObject, GameDef.MonsterDefine monsterDefine, ActorSetting monsterSetting,
            Il2CppSystem.Action<Monster> onBehaviorTreeLoaded, ref Monster __result)
        {
            if (Spawn.CreateSpawnUI.SpawnMonster.MonsterSet.IsPlayerSpawn)
            {
                Spawn.CreateSpawnUI.SpawnMonster.MonsterSet.IsPlayerSpawn = false;
                Vector3 MonsterSpawnDirection = ProcedureMonoSingleton<CameraManager>.Instance._Owner.transform.forward;
                Vector3 basePosition = ProcedureMonoSingleton<CameraManager>.Instance._Owner.transform.localPosition + MonsterSpawnDirection * Spawn.CreateSpawnUI.SpawnMonster.MonsterSet.SpawnDis;
                Spawn.CreateSpawnUI.SpawnMonster.MonsterSet.SpawnMonsterObject.Add(__result);
                LevelLoadManager.Instance.CurLevelRoot.ActorList.Add(__result);
                float randomX = UnityEngine.Random.Range(0, 3f);
                float randomY = UnityEngine.Random.Range(0, 3f);
                float randomZ = UnityEngine.Random.Range(0, 3f);
                __result.transform.localPosition = new Vector3(
                    basePosition.x,
                    basePosition.y,
                    basePosition.z);

            }
        }
    }
}   

