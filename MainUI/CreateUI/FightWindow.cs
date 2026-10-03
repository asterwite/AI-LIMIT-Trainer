using System;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using HarmonyLib;
using System.Collections.Generic;
using UniverseLib.UI.Widgets.ScrollView;
using UnityCheatMainManager.UI;
using UnityCheatMainManager.UI.Panels;
using GameDef;
using UniverseLib.UI.Widgets;
namespace UnityCheatMainManager.MainUI.CreateUI
{
    public class FightWindow : CheatMainPanel
    {
        public static FightWindow Instance { get; private set; }
        public override ManagerUI.Panels PanelType => ManagerUI.Panels.FightWindow;
        public override string Name => "战斗设置";
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(0.35f, 0.175f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.8f, 0.925f);
        public override bool NavButtonWanted => false;

        public GameObject CreatePlayerRoleGridSet;
        public GameObject NavbarHolder;
        public Dropdown MouseInspectDropdown;
        public GameObject ContentHolder;
        public RectTransform ContentRect;
        public static float CurrentPanelWidth => Instance.Rect.rect.width;
        public static float CurrentPanelHeight => Instance.Rect.rect.height;

        public FightWindow(UIBase owner) : base(owner)
        {
            Instance = this;
        }
        public override void Update()
        {
            base.Update();
        }
        protected override void ConstructPanelContent()
        {
            // Tab bar
            GameObject tabGroup = UIFactory.CreateVerticalGroup(ContentRoot, "PlayerWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 0);
            FightUI.FightWindowUI.DrawFightWindowUI(tabGroup,Owner);
           
            SetActive(true);
        }


    }
}
