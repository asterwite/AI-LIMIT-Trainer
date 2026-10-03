using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCheatMainManager.MainUI.CreatePortableShop
{
    public static class PortableShopSet
    {
        public const string DefaultSearchText = "请输入名称";
        public static Dictionary<int, List<ForPortableShop>> BasePortableShops = new Dictionary<int, List<ForPortableShop>>();
        //PortableShop种类
        public static readonly List<string> PortableShopTypeDic = new List<string>()
        {
            "全部"
        };

        public static readonly List<string> CreatTypeDic = new List<string>()
        {
              "无行为",
              "巡逻",
              "攻击"
        };
        //获取所有PortableShop
        public static int Page = 0;
        public static int PortableShopType1Page = 0;
        public static int SpawnCount = 1;
        public static bool isDataFetched = false;
        public static int SpawnPortableShopType = 1;
        public static List<int> SpawnPortableShopIDList = new List<int>();
        public static bool IsPlayerSpawn = false;
        public static void GetBasePortableShopData()
        {
            if (isDataFetched)
                return;

            BasePortableShops.Clear();

            for (int i = 0; i < PortableShopTypeDic.Count; i++)
            {
                BasePortableShops.Add(i, new List<ForPortableShop>());
            }

            isDataFetched = true;

        }

        //获取PortableShop名称
        public static string GetPortableShopName(this ForPortableShop PortableShop)
        {
            var tmp = PortableShop.PortableShopName;
            return tmp != null ? PortableShop.PortableShopName : "";
        }
        //获取PortableShop图标
        public static Sprite GetPortableShopIcon(this ForPortableShop PortableShop)
        {
            var tmp = PortableShop.PortableShopIcon;
            return tmp != null ? tmp : null;
        }
        //获取PortableShopItem解释
        public static string GetPortableShopDescription(this ForPortableShop PortableShop)
        {
            return "注意如果生成位置不够，是不会生成怪物的。" +
                "\n如果是攻击方式的怪物，打死后一定要点击删除全部已生成怪物。" +
                "\n否则当你死亡后重新加载，生成的怪物也会被重新加载出来。" +
                "\n生成的如果是Boss不能直接删除，会导致Boss血条强行卡住，先击杀再删除。" +
                "\n生成数量大于1怪物是会重叠的哦。不采取随机坐标，因为容易导致怪物卡地";
        }
        //添加PortableShopItem
        public static void AddPortableShopItem(this ForPortableShop PortableShop)
        {
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
            {
            }
        }
        //移除PortableShopItem
        public static void DelPortableShopItem(this ForPortableShop PortableShop)
        {
          
        }
        public static List<ForPortableShop> GetTypePortableShops(int PortableShopType = -1)
        {
            List<ForPortableShop> result = new List<ForPortableShop>();
            if (PortableShopType < 0)
            {
                if (BasePortableShops.Count > 0)
                    result.AddRange(BasePortableShops[0]);
            }
            else
            {
                BasePortableShops.TryGetValue(PortableShopType, out result);
            }
            return result ?? new List<ForPortableShop>();
        }
        public static List<ForPortableShop> FilterPortableShopData(this string text, int type = -1)
        {
            if (!string.IsNullOrEmpty(text) && text != DefaultSearchText)
            {
                List<ForPortableShop> list = new List<ForPortableShop>();

                foreach (var PortableShop in GetTypePortableShops(type))
                {
                    string tmp1 = PortableShop.GetPortableShopName();
                    string tmp2 = PortableShop.GetPortableShopDescription();
                    if (tmp1 != null && tmp1.Contains(text.Replace(" ", "")))
                    {
                        list.Add(PortableShop);
                    }
                    else if (tmp2 != null && tmp2.Contains(text.Replace(" ", "")))
                    {
                        list.Add(PortableShop);
                    }
                }
                return list;
            }
            else { return GetTypePortableShops(type); }
        }
    }
}
