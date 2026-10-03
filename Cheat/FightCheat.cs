using CriWare;
using GameDef;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCheatMainManager.Cheat
{
    public class FightCheatHookManager
    {
        public struct FightCheatHook
        {
            public bool 启动;
            public bool 一次;
            public float FloatSet;
           
            public FightCheatHook(bool 启动, bool 一次, float FloatSet)
            {
                this.启动 = 启动;
                this.一次 = 一次;
                this.FloatSet = FloatSet;
            }
        }
        public static FightCheatHook 敌人受伤倍数 = new FightCheatHook(false, false, 1f);
        public static bool 自动弹反 = false;
        public static bool 延长弹反判断时间 = false;
        public static bool 触发了弹反 = false;
        public static float 使用了弹反的时间 = 0;
        public static float 弹反判断时间 = 2f;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MagicSystem), nameof(MagicSystem.RunMagicAction))]
        public static void PlayerUseMagic(GameDef.MagicType __0)
        {
            if (__0 == MagicType.DefenseParry)
            {
                触发了弹反 = true;
                Debug.Log($"Player使用魔法: {__0}");
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Monster), nameof(Monster.DecreaseHp))]
        public static bool MonsterMultipleBeHit(Monster __instance, ref float value)
        {
            if (敌人受伤倍数.启动)
                value = value * 敌人受伤倍数.FloatSet;
            if (MainUI.CreateUI.StaminaUI.StaminaUI.CurrentStamina <= 5f)
                value = value * 0.1f;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HurtUtility), nameof(HurtUtility.HurtActorPostProcessor))]
        public static bool AutoEffectToMonsterWithPlayerBeHit( IActor attacker, BaseGetHit baseGetHit, AttackInfo attackInfo, DamageParamDefine damageParamDefine, ActorDefine.FightType fightType, Vector3 hitPoint, Vector3 attackCheckDir, 
            Vector3 gethitBackDir, float animPauseTime, bool forceSelfConfidenceValue, bool alreadyAttackSomebody, bool writeHaveHurtHandle = false)
        {
            if (自动弹反|| 触发了弹反)
            {
                foreach (var PlayeBeHit in ProcedureMonoSingleton<CameraManager>.Instance._Owner._AttachPoint.GetHits)
                 {
                     if (baseGetHit == PlayeBeHit )
                     {
                         var LoaclMonster = attacker.Cast<Monster>();
                         LoaclMonster.CurrentActionGroup.BeParryWeak(LoaclMonster._AttachPoint.GetHits[0]);
                         return false;
                     }
                 }
            }
            return true;
        }
    }
}
