using System;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using UnityCheatMainManager.UI.Models;
using HarmonyLib;
using System.Collections.Generic;
using UniverseLib.UI.Widgets.ScrollView;
using UnityCheatMainManager.MyManagerSet;
namespace UnityCheatMainManager.UI.Panels
{
    public class MainWindow : CheatMainPanel
    {
        public static MainWindow Instance { get; private set; }
        public override ManagerUI.Panels PanelType => ManagerUI.Panels.MainWindow;
        public override string Name => "主界面";
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

        public MainWindow(UIBase owner) : base(owner)
        {
            Instance = this;
        }
        public override void Update()
        {
            base.Update();
        }

        public static Dropdown SpawnTypeDown;

        protected override void ConstructPanelContent()
        {
            ScrollPoolUI ScrollPoolUI = new ScrollPoolUI();
            var Object = ScrollPoolUI.DrawscrollPool(ContentRoot);

            // Tab bar
            GameObject tabGroup = UIFactory.CreateVerticalGroup(Object, "MainWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 0);

            MainUI.BasicUI.DrawBasicUI(tabGroup,Owner);

            // default active state: Active
            this.SetActive(true);
        }
      
      
    }
}
