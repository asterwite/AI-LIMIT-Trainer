using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCheatMainManager.ESP.CreateESPUI
{
    internal class ESPItemCMP
    {
        public static Sprite GetItemSprite(ItemGetTrigger ItemTrigger)
        {
            if (ItemTrigger.MyPickUpInfo.AccessoriesInfoList.Count > 0)
                return ItemSpawn.ItemSet.AccessoriesSprite[ItemTrigger.MyPickUpInfo.AccessoriesInfoList[0]];

           else if (ItemTrigger.MyPickUpInfo.ArmorInfoList.Count > 0)
                return ItemSpawn.ItemSet.ArmorSprite[ItemTrigger.MyPickUpInfo.ArmorInfoList[0]];

            else if (ItemTrigger.MyPickUpInfo.HeadwearInfoList.Count > 0)
                return ItemSpawn.ItemSet.HeadwearSprite[ItemTrigger.MyPickUpInfo.HeadwearInfoList[0]];

            else if(ItemTrigger.MyPickUpInfo.ItemInfoList.Count > 0)
                return ItemSpawn.ItemSet.ItemSprite[ItemTrigger.MyPickUpInfo.ItemInfoList[0].Id];

            else  if (ItemTrigger.MyPickUpInfo.MagicInfoList.Count > 0)
                return ItemSpawn.ItemSet.MagicSprite[ItemTrigger.MyPickUpInfo.MagicInfoList[0]];

           else if (ItemTrigger.MyPickUpInfo.SealInfoList.Count > 0)
                return ItemSpawn.ItemSet.SealSprite[ItemTrigger.MyPickUpInfo.SealInfoList[0]];

           else if (ItemTrigger.MyPickUpInfo.WeaponInfoList.Count > 0)
                return ItemSpawn.ItemSet.WeaponSprite[ItemTrigger.MyPickUpInfo.WeaponInfoList[0].WeaponId];

            else if (ItemTrigger.MyPickUpInfo.SkillInfoList.Count > 0)
                return ItemSpawn.ItemSet.SkillSprite[ItemTrigger.MyPickUpInfo.SkillInfoList[0]];

            return null;
        }
    }
}
