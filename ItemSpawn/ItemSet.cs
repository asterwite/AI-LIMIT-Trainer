
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UnityCheatMainManager.UI;
using UnityCheatMainManager.UI.Models;
using UnityCheatMainManager.UI.Panels;
using static BackPackManager;
namespace UnityCheatMainManager.ItemSpawn
{
    public static class ItemSet
    {
        public const string DefaultSearchText = "请输入名称";
        public static Dictionary<int, List<ForItem>> BaseItems = new Dictionary<int, List<ForItem>>();
        //物品种类
        public static readonly List<string> ItemTypeDic = new List<string>()
        {
            "物品",
            "武器",
            "头饰",
            "饰品",
            "衣装",
            "术式",
            "刻印",
            "技能",
        };

        public static readonly List<string> CreatTypeDic = new List<string>()
        {

        };
        //获取所有物品
        public static int CreatType = 0;
        public static int Page = 0;
        public static int ItemType1Page = 0;
        public static int SpawnCount = 1;
        public static bool AddItemOnlyToMaxCount = false;
        public static bool AddItemToMaxCount = false;
        public static bool isDataFetched = false;
        public static List<int> HaveItemID = new List<int>();
        public static List<ArchiveDef.WeaponInfo> HaveWeaponID = new List<ArchiveDef.WeaponInfo>();
        public static List<ArchiveDef.AccessoriesInfo> HaveAccessoriesID = new List<ArchiveDef.AccessoriesInfo>();
        public static List<ArchiveDef.ArmorInfo> HaveArmorInfoID= new List<ArchiveDef.ArmorInfo>();
        public static List<ArchiveDef.HeadwearInfo> HaveHeadwearInfoID = new List<ArchiveDef.HeadwearInfo>();
        public static List<ArchiveDef.MagicInfo> HaveMagicInfoID = new List<ArchiveDef.MagicInfo>();
        public static List<ArchiveDef.SealInfo> HaveSealInfoID = new List<ArchiveDef.SealInfo>();
        public static List<ArchiveDef.SkillInfo> HaveSkillInfoID = new List<ArchiveDef.SkillInfo>();
        public static Dictionary<int,Sprite> ItemSprite = new Dictionary<int,Sprite>();
        public static Dictionary<int,Sprite> WeaponSprite = new Dictionary<int,Sprite>();
        public static Dictionary<int,Sprite> AccessoriesSprite = new Dictionary<int,Sprite>();
        public static Dictionary<int,Sprite> ArmorSprite = new Dictionary<int,Sprite>();
        public static Dictionary<int,Sprite> HeadwearSprite = new Dictionary<int,Sprite>();
        public static Dictionary<int,Sprite> MagicSprite = new Dictionary<int,Sprite>();
        public static Dictionary<int,Sprite> SealSprite = new Dictionary<int,Sprite>();
        public static Dictionary<int,Sprite> SkillSprite = new Dictionary<int,Sprite>();
        public static void GetBaseItemData()
        {
            if (isDataFetched)
                return;

            BaseItems.Clear();

            for (int i = 0; i < ItemTypeDic.Count; i++)
            {
                BaseItems.Add(i, new List<ForItem>());
            }

            isDataFetched = true;
            var LoaclizationMangerText = LocalizationManager.Instance;

            #region[物品]
            foreach (var HaveItem in BackPackManager.Instance.ItemDic)
            {
                HaveItemID.Add(HaveItem.Key);
            }
            foreach (var kvp in GlobalConfig.ConfigData._ItemByID)
            {
                BackPackManager.PickUpItemInfo ItemInfo = default(BackPackManager.PickUpItemInfo);
                ItemInfo.Id = kvp.Key;
                BackPackManager.ItemDistributionInfo itemDistributionInfo;
                var Item = BackPackManager.Instance.AddItem(ItemInfo, out itemDistributionInfo);
                if (Item == null)
                    continue;
                if (Item.ItemCount == 0 && !HaveItemID.Contains(Item.ItemID))
                    BackPackManager.Instance.ItemDic.Remove(kvp.Key);
               var ItemDesString =  LoaclizationMangerText.GetCurLocal(Item.Define.DescriptionID) + "\n" + LoaclizationMangerText.GetCurLocal(Item.Define.EffectDescriptionID) + "\n"
                    + LoaclizationMangerText.GetCurLocal(Item._Define.MemoryDescriptionID);
                ItemSprite[Item.DefineID] = Item.Icon;
                BaseItems[0].Add(new ForItem() { BackPackItemInfo = ItemInfo, ItemMaxCount = Item.Define.MaxCount ,ItemDes = ItemDesString, ItemID = Item.ItemID, ItemName = Item.Name, ItemIcon = Item.Icon });
            }
            #endregion

            #region[头饰]
            foreach (var HaveHeadwear in BackPackManager.Instance.HeadwearDic)
            {
                HaveHeadwearInfoID.Add(HaveHeadwear.Value);
            }
            foreach (var Headwearkvp in GlobalConfig.ConfigData._HeadwearByID)
            {
                var Headwear = BackPackManager.Instance.AddHeadwear(Headwearkvp.Key);
                if (Headwear == null)
                    continue;
                if (!HaveHeadwearInfoID.Contains(Headwear))
                    BackPackManager.Instance.HeadwearDic.Remove(Headwear.UniqueID);
                var ItemDesString = LoaclizationMangerText.GetCurLocal(Headwear.Define.DescriptionID) + "\n" + LoaclizationMangerText.GetCurLocal(Headwear.Define.MemoryDescriptionID);
                HeadwearSprite[Headwear.DefineId] = Headwear.Icon;
                BaseItems[2].Add(new ForItem() { ItemID = Headwear.DefineId,ItemDes = ItemDesString,ItemName = Headwear.Name, ItemIcon = Headwear.Icon });
            }
            #endregion

            #region[饰品]
            foreach (var HaveAccessories in BackPackManager.Instance.AccessoriesDic)
            {
                HaveAccessoriesID.Add(HaveAccessories.Value);
            }
            foreach (var Accessorieskvp in GlobalConfig.ConfigData._AccessoriesByID)
            {
                var Accessories = BackPackManager.Instance.AddAccessories(Accessorieskvp.Key);
                if (Accessories == null)
                    continue;
                if (!HaveAccessoriesID.Contains(Accessories) && BackPackManager.Instance.AccessoriesDic != null)
                    BackPackManager.Instance.AccessoriesDic.Remove(Accessories.UniqueID);
                var ItemDesString = LoaclizationMangerText.GetCurLocal(Accessories.Define.DescriptionID);
                AccessoriesSprite[Accessories.DefineID] = Accessories.Icon;
                BaseItems[3].Add(new ForItem() { ItemID = Accessories.DefineId,ItemDes = ItemDesString, ItemName = Accessories.Name, ItemIcon = Accessories.Icon });
            }
            #endregion

            #region[衣装]
            foreach (var HaveArmor in BackPackManager.Instance.ArmorDic)
            {
                HaveArmorInfoID.Add(HaveArmor.Value);
            }
            foreach (var Armorkvp in GlobalConfig.ConfigData._BodyByID)
            {
                var Armor = BackPackManager.Instance.AddArmor(Armorkvp.Key);
                if (Armor == null)
                    continue;
                if (!HaveArmorInfoID.Contains(Armor))
                    BackPackManager.Instance.ArmorDic.Remove(Armor.UniqueID);
                var ItemDesString = LoaclizationMangerText.GetCurLocal(Armor.Define.DescriptionID);
                BaseItems[4].Add(new ForItem() { ItemID = Armor.DefineId,ItemDes = ItemDesString, ItemName = Armor.Name, ItemIcon = Armor.Icon });
                ArmorSprite[Armor.DefineID] = Armor.Icon;
            }
            #endregion

            #region[术式]
            foreach (var HaveMagic in BackPackManager.Instance.MagicDic)
            {
                HaveMagicInfoID.Add(HaveMagic.Value);
            }
            foreach (var Magickvp in GlobalConfig.ConfigData._MagicByID)
            {
                var Magic = BackPackManager.Instance.AddMagic(Magickvp.Key);
                if (Magic == null)
                    continue;
                if (!HaveMagicInfoID.Contains(Magic))
                    BackPackManager.Instance.MagicDic.Remove(Magic.UniqueID);
                var ItemDesString = LoaclizationMangerText.GetCurLocal(Magic.Define.DescriptionID)+"\n" + LoaclizationMangerText.GetCurLocal(Magic.Define.MemoryDescriptionID);
                BaseItems[5].Add(new ForItem() { ItemID = Magic.DefineId, ItemDes = ItemDesString, ItemName = Magic.Name, ItemIcon = Magic.Icon });
                MagicSprite[Magic.DefineID] = Magic.Icon;
            }
            #endregion

            #region[刻印]
            foreach (var HaveSeal in BackPackManager.Instance.SealDic)
            {
                HaveSealInfoID.Add(HaveSeal.Value);
            }
            foreach (var Sealkvp in GlobalConfig.ConfigData._SealByID)
            {
                var Seal = BackPackManager.Instance.AddSeal(Sealkvp.Key);
                if (Seal == null)
                    continue;
                if (!HaveSealInfoID.Contains(Seal))
                    BackPackManager.Instance.SealDic.Remove(Seal.UniqueID);
                var ItemDesString = LoaclizationMangerText.GetCurLocal(Seal.Define.DescriptionID) + "\n" + LoaclizationMangerText.GetCurLocal(Seal.Define.LevelID);
                BaseItems[6].Add(new ForItem() { ItemID = Seal.DefineId, ItemDes = ItemDesString, ItemName = Seal.Define.Name, ItemIcon = Seal.Icon });
                SealSprite[Seal.DefineID] = Seal.Icon;
            }
            #endregion

            #region[技能]
            foreach (var HaveSkill in BackPackManager.Instance.SkillDic)
            {
                HaveSkillInfoID.Add(HaveSkill.Value);
            }
            foreach (var Skillkvp in GlobalConfig.ConfigData._SpecialSkillByID)
            {
                var Skill = BackPackManager.Instance.AddSkill(Skillkvp.Key);
                if (Skill == null)
                    continue;
                if (!HaveSkillInfoID.Contains(Skill))
                    BackPackManager.Instance.SkillDic.Remove(Skill.UniqueID);
                var ItemDesString = LoaclizationMangerText.GetCurLocal(Skill.Define.DescriptionID);
                BaseItems[7].Add(new ForItem() { ItemID = Skill.DefineId, ItemDes = ItemDesString, ItemName = Skill.Name, ItemIcon = Skill.Icon });
                SkillSprite[Skill.DefineID] = Skill.Icon;
            }
            #endregion

            #region[武器]
            foreach (var HaveWeapon in BackPackManager.Instance.WeaponDic)
            {
                HaveWeaponID.Add(HaveWeapon.Value);
            }
            Dictionary<string, int> WeaponLevelDict = new Dictionary<string, int>();

            foreach (var Weaponkvp in GlobalConfig.ConfigData._WeaponByID)
            {
                var Weapon = BackPackManager.Instance.AddWeaponWithSkill(Weaponkvp.Key);
                if (Weapon == null)
                    continue;
                if (!HaveWeaponID.Contains(Weapon))
                    BackPackManager.Instance.WeaponDic.Remove(Weapon.UniqueID);
                var ItemDesString = LoaclizationMangerText.GetCurLocal(Weapon.Define.DescriptionID)+"\n" + LoaclizationMangerText.GetCurLocal(Weapon.Define.LevelID) + "\n" + LoaclizationMangerText.GetCurLocal(Weapon.Define.MemoryDescriptionID);
                var LoadItemName = Weapon.Name + LoaclizationMangerText.GetCurLocal(Weapon.Define.LevelID);
                BaseItems[1].Add(new ForItem() { ItemID = Weapon.DefineId, ItemDes = ItemDesString, ItemName = LoadItemName, ItemIcon = Weapon.Icon });
                WeaponSprite[Weapon.DefineID] = Weapon.Icon;
            }

            #endregion

            #region[]
            #endregion


        }

        //获取物品名称
        public static string GetItemName(this ForItem item)
        {
            var tmp = item.ItemName;
            return tmp != null ? item.ItemName : "";
        }
        //获取物品图标
        public static Sprite GetItemIcon(this ForItem item)
        {
            var tmp = item.ItemIcon;
            return tmp != null ? tmp : null;
        }
        //获取物品解释
        public static string GetItemDescription(this ForItem item)
        {
            return item.ItemDes;
        }
        //添加物品
        public static void SpwanItem(this ForItem Item)
        {
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
            {
                if (Page == 0)
                {
                    var BackPackSpawnItem = Item.BackPackItemInfo;
                    bool IsHaveItem = false;
                    if (AddItemToMaxCount)
                    {
                        foreach (var kvp in BackPackManager.Instance.ItemDic)
                        {
                            if (kvp.Value.ItemID == Item.ItemID)
                            {
                                BackPackSpawnItem.Count = Item.ItemMaxCount - kvp.Value.ItemCount;
                                IsHaveItem = true;
                            }
                        }
                        if (BackPackSpawnItem.Count == 0 && !IsHaveItem)
                            BackPackSpawnItem.Count = Item.ItemMaxCount;
                    }
                    else
                        BackPackSpawnItem.Count = SpawnCount;
                    BackPackManager.Instance.AddItem(BackPackSpawnItem);
                    CheatMain.WriteLog($"添加物品ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}  添加数量：{BackPackSpawnItem.Count.ToString()}");
                }

                if (Page == 1)
                {
                    BackPackManager.Instance.AddWeaponWithSkill(Item.ItemID);
                    CheatMain.WriteLog($"添加武器ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
                }
                if (Page == 2)
                {
                    BackPackManager.Instance.AddHeadwear(Item.ItemID);
                    CheatMain.WriteLog($"添加头饰ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
                }
                if (Page == 3)
                {
                    BackPackManager.Instance.AddAccessories(Item.ItemID);
                    CheatMain.WriteLog($"添加饰品ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
                }
                if (Page == 4)
                {
                    BackPackManager.Instance.AddArmor(Item.ItemID);
                    CheatMain.WriteLog($"添加衣装ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
                }
                if (Page == 5)
                {
                    BackPackManager.Instance.AddMagic(Item.ItemID);
                    CheatMain.WriteLog($"添加术式ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
                }
                if (Page == 6)
                {
                    BackPackManager.Instance.AddSeal(Item.ItemID);
                    CheatMain.WriteLog($"添加刻印ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
                }
                if (Page == 7)
                {
                    BackPackManager.Instance.AddSkill(Item.ItemID);
                    CheatMain.WriteLog($"添加技能ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
                }
            }
        }
        //移除物品
        public static void DelItem(this ForItem Item)
        {
            if (Page == 0)
            {
                bool CanDel = false;
                int DelCount = 0;
                foreach (var kvp in BackPackManager.Instance.ItemDic)
                {
                    if (kvp.Value.ItemID == Item.ItemID)
                    {
                        CanDel = true;
                        if (kvp.Value.ItemCount > SpawnCount - 1)
                            DelCount = SpawnCount;
                        else if (kvp.Value.ItemCount < SpawnCount + 1)
                            DelCount = kvp.Value.ItemCount;
                    }
                    if (CanDel)
                    {
                        CanDel = false;
                        BackPackManager.Instance.DeductItem(Item.ItemID, DelCount);
                        CheatMain.WriteLog($"移除物品ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}  移除数量：{DelCount.ToString()}");
                    }

                }
            }
            if (Page == 1)
            {
                BackPackManager.Instance.DeductWeaponByDefineId(Item.ItemID);
                CheatMain.WriteLog($"移除武器ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
            }
            if (Page == 2)
            {
                BackPackManager.Instance.DeductHeadwearByDefineId(Item.ItemID);
                CheatMain.WriteLog($"移除头饰ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
            }
            if (Page == 3)
            {
                BackPackManager.Instance.DeductAccessoriesByDefineId(Item.ItemID);
                CheatMain.WriteLog($"移除饰品ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
            }
            if (Page == 4)
            {
                BackPackManager.Instance.DeductArmorByDefineId(Item.ItemID);
                CheatMain.WriteLog($"移除衣装ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
            }
            if (Page == 5)
            {
                BackPackManager.Instance.DeductMagicByDefineId(Item.ItemID);
                CheatMain.WriteLog($"移除术式ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
            }
            if (Page == 6)
            {
                BackPackManager.Instance.DeductSealByDefineId(Item.ItemID);
                CheatMain.WriteLog($"移除刻印ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
            }
            if (Page == 7)
            {
                BackPackManager.Instance.DeductSkillByDefineId(Item.ItemID);
                CheatMain.WriteLog($"移除技能ID：{Item.ItemID.ToString()} 名称：{Item.ItemName}");
            }
        }
        public static List<ForItem> GetTypeItems(int ItemType = -1)
        {
            List<ForItem> result = new List<ForItem>();
            if (ItemType < 0)
            {
                if (BaseItems.Count > 0)
                result.AddRange(BaseItems[0]);
            }
            else
            {
                BaseItems.TryGetValue(ItemType, out result);
            }
            return result ?? new List<ForItem>();
        }
        public static List<ForItem> FilterItemData(this string text, int type = -1)
        {
            if (!string.IsNullOrEmpty(text) && text != DefaultSearchText)
            {
                List<ForItem> list = new List<ForItem>();

                foreach (var item in GetTypeItems(type))
                {
                    string tmp1 = GetItemName(item);
                    string tmp2 = GetItemDescription(item);
                    if (tmp1 != null && tmp1.Contains(text.Replace(" ", "")))
                    {
                        list.Add(item);
                    }
                    else if (tmp2 != null && tmp2.Contains(text.Replace(" ", "")))
                    {
                        list.Add(item);
                    }
                }
                return list;
            }
            else { return GetTypeItems(type); }
        }
    }
}
