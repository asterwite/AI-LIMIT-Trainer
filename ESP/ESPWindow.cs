using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCheatMainManager.UI.Panels;
using UnityCheatMainManager.UI;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using UnityCheatMainManager.MyManagerSet;
using UnityCheatMainManager.ESP.CreateESPUI;

namespace UnityCheatMainManager.ESP
{
    internal class ESPWindow : CheatMainPanel
    {
        public static ESPWindow Instance { get; private set; }
        public override ManagerUI.Panels PanelType => ManagerUI.Panels.ESPWindow;
        public override string Name => "ESP";
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

        public ESPWindow(UIBase owner) : base(owner)
        {
            Instance = this;
        }
        public override void Update()
        {
            base.Update();
        }

        protected override void ConstructPanelContent()
        {
            ScrollPoolUI ScrollPoolUI = new ScrollPoolUI();
            var Object = ScrollPoolUI.DrawscrollPool(ContentRoot);

            GameObject tabGroup = UIFactory.CreateVerticalGroup(Object, "ESPWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 0);

            ESPUI.DrawESPUI(tabGroup, Owner);
            // default active state: Active
            this.SetActive(true);
        }
    }
}
