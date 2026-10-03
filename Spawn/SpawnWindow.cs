using System;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using UnityCheatMainManager.UI.Models;
using HarmonyLib;
using System.Collections.Generic;
using UniverseLib.UI.Widgets.ScrollView;
using UnityCheatMainManager.MyManagerSet;
using UnityCheatMainManager.UI.Panels;
using UnityCheatMainManager.UI;
using UnityCheatMainManager.Spawn.CreateSpawnUI.Buff;
using UnityCheatMainManager.Spawn.CreateSpawnUI.SpawnMonster;
namespace UnityCheatMainManager.Spawn
{
    public class SpanwWinodw : CheatMainPanel
    {
        public static SpanwWinodw Instance { get; private set; }
        public override ManagerUI.Panels PanelType => ManagerUI.Panels.SpawnWindow;
        public override string Name => "各种生成";
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(0.35f, 0.175f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.8f, 0.925f);

        public GameObject NavbarHolder;
        public Dropdown MouseInspectDropdown;
        public GameObject ContentHolder;
        public RectTransform ContentRect;
        public static float CurrentPanelWidth => Instance.Rect.rect.width;
        public static float CurrentPanelHeight => Instance.Rect.rect.height;

        public SpanwWinodw(UIBase owner) : base(owner)
        {
            Instance = this;
        }
        public override void Update()
        {
            base.Update();
        }
        private static ItemWindow OldItemWindow;
        private static BuffWindow OldBuffWindow;
        private static MonsterWindow OldMonsterWindow;
        protected override void ConstructPanelContent()
        {
            ScrollPoolUI ScrollPoolUI = new ScrollPoolUI();
            var Object = ScrollPoolUI.DrawscrollPool(ContentRoot);

            GameObject tabGroup = UIFactory.CreateVerticalGroup(Object, "SpawnWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 0);

            GameObject SpawnItem = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowSpawnItem", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);

            SpawnItem.CreateButton("打开物品生成窗口", () =>
            {
                if (OldItemWindow == null)
                {
                    OldItemWindow = new ItemWindow(Owner);
                }
                OldItemWindow.SetActive(true); // 显示窗口
            }).SetLayoutElement(300, 40);

            GameObject SpawnBuff = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowSpawnBuff", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            SpawnBuff.CreateButton("打开Buff添加窗口", () =>
            {
                if (OldBuffWindow == null)
                {
                    OldBuffWindow = new BuffWindow(Owner);
                }
                OldBuffWindow.SetActive(true); // 显示窗口
            }).SetLayoutElement(300, 40);

            GameObject SpawnMonster = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowSpawnMonster", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            SpawnBuff.CreateButton("打开怪物生成窗口", () =>
            {
                if (OldMonsterWindow == null)
                {
                    OldMonsterWindow = new MonsterWindow(Owner);
                }
                OldMonsterWindow.SetActive(true); // 显示窗口
            }).SetLayoutElement(300, 40);

            // default active state: Active
            this.SetActive(true);
        }


    }
}
