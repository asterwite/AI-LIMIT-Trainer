using Bullets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniverseLib.UI;
using UnityCheatMainManager.UI;
using static UnityCheatMainManager.Cheat.FightCheatHookManager;
using GameDef;
using UniverseLib.UI.Widgets;
using Il2CppSystem;

namespace UnityCheatMainManager.MainUI.CreateUI.FightUI
{
    internal class FightWindowUI
    {
        public struct PlayerFightCheatSet
        {
            public bool 启动;
            public bool 一次;
            public float 等待时间;
            public float 等待需要时间;
            public PlayerFightCheatSet(bool 启动, bool 一次, float 等待时间, float 等待需要时间)
            {
                this.启动 = 启动;
                this.一次 = 一次;
                this.等待时间 = 等待时间;
                this.等待需要时间 = 等待需要时间;
            }
        }
        public static float GetLocalMonsterDistance = 10f;
        public static float MonsterGetDamage = 10f;
        public static PlayerFightCheatSet 无敌 = new PlayerFightCheatSet(false, false,0,0);
        public static bool 无限生命 = false;
        public static bool 无限同步率 = false;
        public static bool 杀戮光环 = false;
        public static PlayerFightCheatSet 天降雷罚 = new PlayerFightCheatSet(false, false, 0, 1f);
        public static PlayerFightCheatSet 厉炎燃烧 = new PlayerFightCheatSet(false, false, 0, 1f);
        public static PlayerFightCheatSet 黑洞 = new PlayerFightCheatSet(false, false, 0, 10f);
        public static PlayerFightCheatSet 震地 = new PlayerFightCheatSet(false, false, 0, 1f);
        private static PlayerFightUseWindow OldPlayerFightUseWindow;
        public static void FightWindowUICheat(Player Player)
        {

            if (无敌.启动 && !Player.IsInvincible())
            {
                无敌.一次 = true;
                Player.SetBuffInvincible(true);
            }
            if (!无敌.启动 && Player.IsInvincible() && 无敌.一次)
            {
                无敌.一次 = false;
                Player.SetBuffInvincible(false);
            }

            if (无限生命 && Player.GetHp() < Player.GetHPMax())
                Player.IncreaseHp(Player.GetHPMax() - Player.GetHp());
            if (无限同步率 && Player.GetConfidence() < Player.GetConfidenceMax())
                Player.ChangeConfidence(Player.GetConfidenceMax() - Player.GetConfidence(), ActorDefine.ConfidenceChangeType.GetHit, true);

            QuickUseMagic.InputUseQuickMagic();
            #region[敌人遍历]
            var cameraManager = ProcedureMonoSingleton<CameraManager>.Instance;
            var camera = cameraManager.GetRealCameraScript();
            var playerTransform = ProcedureMonoSingleton<CameraManager>.Instance._Owner.transform;

            // 1. 先收集所有符合条件的怪物（在范围内、存活）
            List<Monster> InDisMonsters = new List<Monster>(); // 请替换为实际怪物类型
            foreach (var localMonster in LevelLoadManager.Instance.CurLevelRoot.ActorList)
            {
                if (localMonster == null || localMonster.GetHp() <= 0)
                    continue;

                float monsterDistance = Vector3.Distance(playerTransform.position, localMonster.transform.position);
                if (monsterDistance >= GetLocalMonsterDistance)
                    continue;

                InDisMonsters.Add(localMonster);
            }

            if (杀戮光环 && InDisMonsters.Count > 0)
            {
                foreach (var monster in InDisMonsters)
                {
                    monster.DecreaseHp(monster.GetHPMax(), specialTip: true);
                }
            }

            if (天降雷罚.启动 && InDisMonsters.Count > 0)
            {
                天降雷罚.等待时间 += Time.deltaTime;
                if (天降雷罚.等待时间 > 天降雷罚.等待需要时间)
                {
                    天降雷罚.等待时间 = 0;

                    foreach (var monster in InDisMonsters)
                    {
                        CommonSkillUtility.CreateLighting(monster._AttachPoint.GetHits[0].GetOwner(), lightingType: CommonSkillUtility.LightingType.R_2, monster.transform.localPosition, GameDef.DamageSourceType.P_Throw_LightingBomb,
                            GameDef.ActionType.ThrowLightingBomb, 0);
                        monster.CurrentActionGroup.BeParryWeak(monster._AttachPoint.GetHits[0]);
                        monster.DecreaseHp(MonsterGetDamage);
                    }
                }
            }

            // 厉炎燃烧
            if (厉炎燃烧.启动 && InDisMonsters.Count > 0)
            {
                厉炎燃烧.等待时间 += Time.deltaTime;
                if (厉炎燃烧.等待时间 > 厉炎燃烧.等待需要时间)
                {
                    厉炎燃烧.等待时间 = 0;

                    foreach (var monster in InDisMonsters)
                    {
                        CommonSkillUtility.CreatCycleFireEffect(monster._AttachPoint.GetHits[0].GetOwner(), monster.transform.localPosition, 1f, 0.5f);
                        monster.CurrentActionGroup.BeParryWeak(monster._AttachPoint.GetHits[0]);
                        monster.DecreaseHp(MonsterGetDamage);
                    }
                }
            }

            if (黑洞.启动 && InDisMonsters.Count > 0)
            {
                黑洞.等待时间 += Time.deltaTime;
                if (黑洞.等待时间 > 黑洞.等待需要时间)
                {
                    黑洞.等待时间 = 0;
                    AttackInfo attackInfo = new AttackInfo();

                    foreach (var monster in InDisMonsters)
                    {
                        System.Func<Vector3> monsterPos = () => monster.transform.localPosition;
                        Il2CppSystem.Func<Vector3> posFunc = monsterPos;

                        CommonSkillUtility.CreatBlackHole(monster._AttachPoint.GetHits[0].GetOwner(), 0, posFunc, 10f, 10f, 10f, 10f, ActorDefine.AttackInfoType.MagicArm,
                            GameDef.DamageSourceType.P_Magic_BlackHole, GameDef.DamageSourceType.P_Magic_BlackHole, GameDef.ActionType.P_Magic_BlackHole, 2f, 2f, attackInfo,
                            2f, null, 2f);
                        monster.CurrentActionGroup.BeParryWeak(monster._AttachPoint.GetHits[0]);
                        monster.DecreaseHp(MonsterGetDamage);
                    }
                }
            }

            if (震地.启动 && InDisMonsters.Count > 0)
            {
                震地.等待时间 += Time.deltaTime;
                if (震地.等待时间 > 震地.等待需要时间)
                {
                    震地.等待时间 = 0;
                    foreach (var monster in InDisMonsters)
                    {
                        monster.CurrentActionGroup.DoFootEffect(monster.transform.localPosition, 3);
                        monster.CurrentActionGroup.BeParryWeak(monster._AttachPoint.GetHits[0]);
                        monster.DecreaseHp(MonsterGetDamage);
                    }
                }
            }
            #endregion

            if (触发了弹反)
            {
                使用了弹反的时间 += Time.deltaTime;

                float 剩余时间 = Mathf.Max(弹反判断时间 - 使用了弹反的时间, 0f);

                if (使用了弹反的时间 >= 弹反判断时间)
                {
                    触发了弹反 = false;
                    使用了弹反的时间 = 0f; 
                    //UnityEngine.Debug.Log("[弹反] 已结束");
                }
            }

        }
        public static void DrawFightWindowUI(GameObject tabGroup, UIBase Owner)
            {

            GameObject PlayerFight = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowPlayerFight", false, false, true, true, 5, default,
               new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            PlayerFight.CreateToggle(100, "无敌", (state) => { 无敌.启动 = state; }).gameObject.SetTooltip("神圣之人不可被攻击", tabGroup); ;
            PlayerFight.CreateToggle(100, "无限生命", (state) => { 无限生命 = state; }).gameObject.SetTooltip("什么叫不败，有本事再来！", tabGroup);
            PlayerFight.CreateToggle(100, "无限同步率", (state) => { 无限同步率 = state; }).gameObject.SetTooltip("妈妈再也不用担心我们的同步率不够用了，天天充满活力呢", tabGroup);
            PlayerFight.CreateToggle(100, "自动弹反", (state) => { Cheat.FightCheatHookManager.自动弹反 = state; }).gameObject.SetTooltip("站着让你打，你也打不到我，反死你" +
                "\n\n强制给怪物传招架状态，比如说远程和不可被反击的怪物也会传，但是对于这些而言实际上并没有效果，为了合理一点，我也会执行招架动作，但是实际没用0.0,", tabGroup);
            PlayerFight.CreateToggle(100, "延长弹反判断时间", (state) => { Cheat.FightCheatHookManager.延长弹反判断时间 = state; }).gameObject.SetTooltip("弹反判断时间维2秒", tabGroup);
            GameObject PlayerFightUse = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowPlayerFightUse", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);

            PlayerFightUse.CreateButton("战斗物品使用界面", () =>
            {
                if (OldPlayerFightUseWindow == null)
                {
                    OldPlayerFightUseWindow = new PlayerFightUseWindow(Owner);
                }
                OldPlayerFightUseWindow.SetActive(true); // 显示窗口
            }).SetLayoutElement(180, 40).gameObject.SetTooltip("进入战斗物品使用界面，不会消耗物品", tabGroup);


            GameObject MonsterFight = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowMonsterFight", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            MonsterFight.CreateButton("重置附近怪物", () =>
            {
                LevelLoadManager.Instance.CurLevelRoot.ResetMonster();
            }).SetLayoutElement(120, 40).gameObject.SetTooltip("不是我TM刚杀的怪，你又给我刷新了是吧", tabGroup);
            
            MonsterFight.CreateButton("重置附近NPC", () =>
            {
                LevelLoadManager.Instance.CurLevelRoot.ResetNPC(false);
                LevelLoadManager.Instance.CurLevelRoot.ResetNPC(true);
            }).SetLayoutElement(120, 40).gameObject.SetTooltip("顾名思义刷新NPC，有什么用？没什么用！", tabGroup);

            MonsterFight.CreateButton("重置当前场景", () =>
            {
                LevelLoadManager.Instance.CurLevelRoot.ResetScene(true);
                LevelLoadManager.Instance.CurLevelRoot.ResetScene(false);
            }).SetLayoutElement(80, 40).gameObject.SetTooltip("不要搞破坏了孩子们，咱有神力，还原场景，闹呢", tabGroup);

            MonsterFight.CreateInputEditToggle(100, "敌人受伤倍数设置", "2", (string text, bool state) =>
            {
                UnityCheatMainManager.Cheat.FightCheatHookManager.敌人受伤倍数.FloatSet = text.ConvertToFloatDef(2);
                UnityCheatMainManager.Cheat.FightCheatHookManager.敌人受伤倍数.启动 = state;
            }).gameObject.SetTooltip("作为一名机兵伤害高点很正常吧", tabGroup);

            GameObject LocalMonsteMonsterFightText= UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowMonsterFightText", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            LocalMonsteMonsterFightText.Createlabel("附近怪物设置").SetLayoutElement(120,40);
            GameObject LocalMonsteMonsterSplitPanel = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowLocalMonsteMonsterSplitPanel", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            LocalMonsteMonsterSplitPanel.CreateSplitPanel(Color.white);

            GameObject LocalMonsterFightDistance = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowLocalMonsterFightDistance", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            LocalMonsterFightDistance.CreateInputEditButton(160, "设置怪物判断距离", "10", "设置", (text) =>
            {
                float MonsterMaxDistance = text.ConvertToFloatDef(10);
                GetLocalMonsterDistance = MonsterMaxDistance;
            }).gameObject.SetTooltip("执行怪物操作的最远距离", tabGroup); ;

            LocalMonsterFightDistance.CreateInputEditButton(250, "特殊效果对怪物的伤害", "300", "设置", (text) =>
            {
                float Damage = text.ConvertToFloatDef(300);
                MonsterGetDamage = Damage;
            }).gameObject.SetTooltip("天降雷罚，厉炎燃烧，黑洞？震地的每次造成的伤害", tabGroup); ;

            GameObject LocalMonsterFight = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowLocalMonsterFight", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            LocalMonsterFight.CreateToggle(100, "杀戮光环", (state) => { 杀戮光环 = state; })
                .gameObject.SetTooltip("妈妈再也不用担心我打不过怪物了，孩子们刚来拥抱就没了", tabGroup);

            GameObject LocalMonsteSkillCreateText = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowLocalMonsteSkillCreate", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            LocalMonsteSkillCreateText.Createlabel("特殊效果").SetLayoutElement(80,40);
            GameObject LocalMonsteSkillSplitPane = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowLocalMonsteSkillSplitPane", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            LocalMonsteSkillSplitPane.CreateSplitPanel(Color.white);

            GameObject LocalMonsteSkillCreateLight = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowLocalMonsteSkillCreateLight", false, false, true, true, 5, default,
           new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            LocalMonsteSkillCreateLight.CreateInputEditButton(180, "天降雷罚每次需要几秒", "1", "设置", (text) =>
            {
                float LightTime = text.ConvertToFloatDef(300);
                天降雷罚.等待需要时间 = LightTime;
            }).gameObject.SetTooltip("顾名思义多少秒降下一次", tabGroup); ;
            LocalMonsteSkillCreateLight.CreateToggle(100, "天降雷罚", (state) => { 天降雷罚.启动 = state; }).gameObject.SetTooltip("你的行为激怒了神，你要受到对应的惩罚", tabGroup);

            GameObject LocalMonsteSkillCreateFire = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowLocalMonsteSkillCreateFire", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            LocalMonsteSkillCreateFire.CreateInputEditButton(180, "厉炎燃烧每次需要几秒", "1", "设置", (text) =>
            {
                float FireTime = text.ConvertToFloatDef(300);
                厉炎燃烧.等待需要时间 = FireTime;
            }).gameObject.SetTooltip("顾名思义多少秒燃烧一次", tabGroup); ;
            LocalMonsteSkillCreateFire.CreateToggle(100, "厉炎燃烧", (state) => { 厉炎燃烧.启动 = state; }).gameObject.SetTooltip("你的行为激怒了神，你要受到对应的炼狱惩罚", tabGroup);

            GameObject LocalMonsteSkillCreateBlackHold = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowLocalMonsteSkillCreateBlackHold", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            LocalMonsteSkillCreateBlackHold.CreateInputEditButton(180, "黑洞每次需要几秒", "10", "设置", (text) =>
            {
                float BlackHoldTime = text.ConvertToFloatDef(300);
                黑洞.等待需要时间 = BlackHoldTime;
            }).gameObject.SetTooltip("顾名思义多少秒产生一次，不要太快这玩意巨TM卡，时间最起码也要5秒以上", tabGroup); ;
            LocalMonsteSkillCreateBlackHold.CreateToggle(100, "黑洞", (state) => { 黑洞.启动 = state; }).gameObject.SetTooltip("你的行为激怒了神，你要受到对应的光明惩罚", tabGroup);

            GameObject LocalMonsteSkillCreateFoot = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowLocalMonsteSkillCreateFoot", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            LocalMonsteSkillCreateFoot.CreateInputEditButton(180, "震地每次需要几秒", "1", "设置", (text) =>
            {
                float LightTime = text.ConvertToFloatDef(300);
                震地.等待需要时间 = LightTime;
            }).gameObject.SetTooltip("顾名思义多少秒产生一次，不要太快这玩意巨TM卡，时间最起码也要5秒以上", tabGroup); ;
            LocalMonsteSkillCreateFoot.CreateToggle(100, "震地", (state) => { 震地.启动 = state; }).gameObject.SetTooltip("你的行为激怒了神，你要受到对应的震地惩罚", tabGroup);

        }
    }
}
