using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityCheatMainManager.ItemSpawn
{
    public class ForItem
    {
        public BackPackManager.PickUpItemInfo BackPackItemInfo { get; set; }
        public  Sprite ItemIcon { get; set; }
        public int ItemID { get; set; }

        public int ItemMaxCount { get; set; }
        public string ItemName { get; set; }
        public string ItemDes { get; set; }
    }
}


