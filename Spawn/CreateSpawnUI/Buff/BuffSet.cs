using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCheatMainManager.Spawn.CreateSpawnUI.Buff
{
    public static class BuffSet
    {
        public const string DefaultSearchText = "请输入名称";
        public static Dictionary<int, List<ForBuff>> BaseBuffs = new Dictionary<int, List<ForBuff>>();
        //Buff种类
        public static readonly List<string> BuffTypeDic = new List<string>()
        {
            "全部"
        };

        public static readonly List<string> CreatTypeDic = new List<string>()
        {

        };
        //获取所有Buff
        public static int CreatType = 0;
        public static int Page = 0;
        public static int BuffType1Page = 0;
        public static int SpawnCount = 1;
        public static bool isDataFetched = false;
        public static bool AddBuffTimeIsinfinite = false;
        public static List<int> SpawnBuffIDList = new List<int>();
        public static void GetBaseBuffData()
        {
            if (isDataFetched)
                return;

            BaseBuffs.Clear();

            for (int i = 0; i < BuffTypeDic.Count; i++)
            {
                BaseBuffs.Add(i, new List<ForBuff>());
            }

            isDataFetched = true;
            var Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;

            #region[Buff]
            foreach (var Buffkvp in GlobalConfig.ConfigData._BuffByBuffID)
            {
                    BaseBuffs[0].Add(new ForBuff() { BuffID = Buffkvp.Value.ID, BuffName = Buffkvp.Value.Name});
            }
            #endregion

        }

        //获取Buff名称
        public static string GetBuffName(this ForBuff Buff)
        {
            var tmp = Buff.BuffName;
            return tmp != null ? Buff.BuffName : "";
        }
        //获取Buff图标
        public static Sprite GetBuffIcon(this ForBuff Buff)
        {
            var tmp = Buff.BuffIcon;
            return tmp != null ? tmp : null;
        }
        //获取Buff解释
        public static string GetBuffDescription(this ForBuff Buff)
        {
            return Buff.BuffDes;
        }
        //添加Buff
        public static void SpwanBuff(this ForBuff Buff)
        {
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
            {
                var Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                var SpawnBuff = Player._MyBuffController.AddBuff(Buff.BuffID);
                SpawnBuff.MaxTime = SpawnBuff.MaxTime * (float)SpawnCount;
                SpawnBuff.LastTime = SpawnBuff.MaxTime;
                SpawnBuffIDList.Add(Buff.BuffID);
            }
        }
        //移除Buff
        public static void DelBuff(this ForBuff Buff)
        {
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
            {
                var Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                Player._MyBuffController.RemoveBuff(Buff.BuffID);
                SpawnBuffIDList.Remove(Buff.BuffID);
            }
        }
        public static List<ForBuff> GetTypeBuffs(int BuffType = -1)
        {
            List<ForBuff> result = new List<ForBuff>();
            if (BuffType < 0)
            {
                if (BaseBuffs.Count > 0)
                    result.AddRange(BaseBuffs[0]);
            }
            else
            {
                BaseBuffs.TryGetValue(BuffType, out result);
            }
            return result ?? new List<ForBuff>();
        }
        public static List<ForBuff> FilterBuffData(this string text, int type = -1)
        {
            if (!string.IsNullOrEmpty(text) && text != DefaultSearchText)
            {
                List<ForBuff> list = new List<ForBuff>();

                foreach (var Buff in GetTypeBuffs(type))
                {
                    string tmp1 = GetBuffName(Buff);
                    string tmp2 = GetBuffDescription(Buff);
                    if (tmp1 != null && tmp1.Contains(text.Replace(" ", "")))
                    {
                        list.Add(Buff);
                    }
                    else if (tmp2 != null && tmp2.Contains(text.Replace(" ", "")))
                    {
                        list.Add(Buff);
                    }
                }
                return list;
            }
            else { return GetTypeBuffs(type); }
        }
        public static void BuffCheat(Player Player)
        {
            if (AddBuffTimeIsinfinite)
            {
                foreach (var kvp in Player._MyBuffController.BuffDic)
                {
                    if (BuffSet.SpawnBuffIDList.Contains(kvp.Key))
                        kvp.Value.LastTime = kvp.Value.MaxTime;
                }
            }
          
        }
    }
}
