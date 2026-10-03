using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCheatMainManager.Spawn.CreateSpawnUI.SpawnMonster
{
    public static class MonsterSet
    {
        public const string DefaultSearchText = "请输入名称";
        public static Dictionary<int, List<ForMonster>> BaseMonsters = new Dictionary<int, List<ForMonster>>();
        //Monster种类
        public static readonly List<string> MonsterTypeDic = new List<string>()
        {
            "全部"
        };

        public static readonly List<string> CreatTypeDic = new List<string>()
        {
              "无行为",
              "巡逻",
              "攻击"
        };
        //获取所有Monster
        public static int CreatType = 0;
        public static int Page = 0;
        public static int MonsterType1Page = 0;
        public static int SpawnCount = 1;
        public static bool isDataFetched = false;
        public static int SpawnMonsterType = 1;   
        public static List<int> SpawnMonsterIDList = new List<int>();
        public static List<Monster> SpawnMonsterObject = new List<Monster>();
        public static bool IsPlayerSpawn = false;
        public static float SpawnDis = 5f;
        public static void GetBaseMonsterData()
        {
            if (isDataFetched)
                return;

            BaseMonsters.Clear();

            for (int i = 0; i < MonsterTypeDic.Count; i++)
            {
                BaseMonsters.Add(i, new List<ForMonster>());
            }

            isDataFetched = true;
            var Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;

            #region[怪物]
            foreach (var Monsterkvp in GlobalConfig.ConfigData._MonsterByID)
            {
                    BaseMonsters[0].Add(new ForMonster() { MonsterRes = Monsterkvp.Value.Resource,  MonsterID = Monsterkvp.Value.ID, MonsterName = Monsterkvp.Value.Name});
            }
            #endregion

        }

        //获取Monster名称
        public static string GetMonsterName(this ForMonster Monster)
        {
            var tmp = Monster.MonsterName;
            return tmp != null ? Monster.MonsterName : "";
        }
        //获取Monster图标
        public static Sprite GetMonsterIcon(this ForMonster Monster)
        {
            var tmp = Monster.MonsterIcon;
            return tmp != null ? tmp : null;
        }
        //获取Monster解释
        public static string GetMonsterDescription(this ForMonster Monster)
        {
            return "注意如果生成位置不够，是不会生成怪物的。" +
                "\n如果是攻击方式的怪物，打死后一定要点击删除全部已生成怪物。" +
                "\n否则当你死亡后重新加载，生成的怪物也会被重新加载出来。" +
                "\n生成的如果是Boss不能直接删除，会导致Boss血条强行卡住，先击杀再删除。" +
                "\n生成数量大于1怪物是会重叠的哦。不采取随机坐标，因为容易导致怪物卡地";
        }
        //添加Monster
        public static void SpwanMonster(this ForMonster Monster)
        {
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
            {
                for (int i = 0; i < SpawnCount; i++)
                {
                    IsPlayerSpawn = true;
                    if (CreatType < 2)
                    {
                        SceneSettingDef.ActorSetting MonsterActorSetting = null;
                        var MonsterObject = AddressResourcesManager.GetGameObject(Monster.MonsterRes);
                        var MonsterDefine = GlobalConfig.ConfigData.GetMonsterByID(Monster.MonsterID);
                        if (CreatType == 0)
                            MonsterActorSetting = LevelLoadManager.Instance.CurLevelRoot._MySceneSetting.ActorList[0];
                        if (CreatType == 1)
                            MonsterActorSetting = LevelLoadManager.Instance.CurLevelRoot.ActorList[0]._ActorSetting;
                        if (MonsterActorSetting != null)
                        {
                            var LoaclMonster = ActorManager.Instance.SpawnMonster(MonsterObject.Result, MonsterDefine, MonsterActorSetting, null);
                        }
                    }
                    if (CreatType == 2)
                    {
                        CommandTerminal.Terminal.Shell.RunCommand("spawnactor" + " " + Monster.MonsterID.ToString());
                    }
                }
            }
        }
        //移除Monster
        public static void DelMonster(this ForMonster Monster)
        {
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
            {
                for (int i = 0; i < SpawnCount; i++)
                {
                    foreach (var PlayerSpawnMonster in SpawnMonsterObject)
                    {
                        if (PlayerSpawnMonster.GetDefineId() == Monster.MonsterID)
                            UnityEngine.Object.Destroy(PlayerSpawnMonster.gameObject);
                    }
                }
            }
        }
        public static List<ForMonster> GetTypeMonsters(int MonsterType = -1)
        {
            List<ForMonster> result = new List<ForMonster>();
            if (MonsterType < 0)
            {
                if (BaseMonsters.Count > 0)
                    result.AddRange(BaseMonsters[0]);
            }
            else
            {
                BaseMonsters.TryGetValue(MonsterType, out result);
            }
            return result ?? new List<ForMonster>();
        }
        public static List<ForMonster> FilterMonsterData(this string text, int type = -1)
        {
            if (!string.IsNullOrEmpty(text) && text != DefaultSearchText)
            {
                List<ForMonster> list = new List<ForMonster>();

                foreach (var Monster in GetTypeMonsters(type))
                {
                    string tmp1 = GetMonsterName(Monster);
                    string tmp2 = GetMonsterDescription(Monster);
                    if (tmp1 != null && tmp1.Contains(text.Replace(" ", "")))
                    {
                        list.Add(Monster);
                    }
                    else if (tmp2 != null && tmp2.Contains(text.Replace(" ", "")))
                    {
                        list.Add(Monster);
                    }
                }
                return list;
            }
            else { return GetTypeMonsters(type); }
        }
    }
}
