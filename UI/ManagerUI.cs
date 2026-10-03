using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniverseLib.UI;
using UnityCheatMainManager.UI.Models;
using UniverseLib;
using UnityCheatMainManager.UI.Panels;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ScrollView;
using System.Diagnostics;
using UnityCheatMainManager.Spawn;
using UnityCheatMainManager.ESP;
using UnityCheatMainManager.Sponsor;

namespace UnityCheatMainManager.UI
{
    public static class ManagerUI
    {
        public enum Panels
        {
            //主界面
            MainWindow,
            FightWindow,
            FightUseWindow,
            PortableShopWindow,
            //ESP
            ESPWindow,
            //各种生成
            SpawnWindow,
            ItemWindow,
            BuffWindow,
            MonsterWindow,
            //配置设置
            OptionWindow,
            //其他
            SponsorWindow,
            ZanZhuWindow,
            FreeCamWindow,
        }

        public static bool Initializing { get; internal set; } = true;
        internal static UIBase UiBase { get; private set; }

        public const string UiBaseGUID = "UnityCheatMainManager.UiBase";
        public static GameObject UIRoot => UiBase?.RootObject;
        public static GameObject UIToolTip { get; private set; }
        public static GameObject WorldToolTip { get; private set; }
        public static RectTransform UIRootRect { get; private set; }
        public static Canvas UICanvas { get; private set; }

        internal static readonly Dictionary<Panels, CheatMainPanel> UIPanels = new Dictionary<Panels, CheatMainPanel>();

        public static RectTransform NavBarRect;
        public static GameObject NavbarTabButtonHolder;
        public static GameObject TitleBar;
        public static GameObject DropdownRoot;
        public static GameObject DropdownContent;

        private static readonly Vector2 NAVBAR_DIMENSIONS = new Vector2(200f, 600f);
        private static readonly Vector2 TITLE_BAR_DIMENSIONS = new Vector2(1020f, 40f);

        private static ButtonRef closeBtn;
        private static ButtonRef UpdateBtn;
        private static Text Title;

        public static GameObject navbarPanelObject;
        public static ButtonRef MycloseBtn;

        public static bool ShowMenu
        {
            get => UiBase != null && UiBase.Enabled;
            set
            {
                if (UiBase == null || !UIRoot || UiBase.Enabled == value)
                    return;

                UniversalUI.SetUIActive(UiBaseGUID, value);
            }
        }

        internal static void InitUI()
        {
            UiBase = UniversalUI.RegisterUI(UiBaseGUID, null);

            UIRootRect = UIRoot.GetComponent<RectTransform>();
            UICanvas = UIRoot.GetComponent<Canvas>();
            UIToolTip = UIFactory.CreateUIObject("UIToolTip", UIRoot);

            WorldToolTip = UniversalUI.RegisterUI("UnityCheatMainManager.WorldToolTip", null).RootObject;
            UniversalUI.SetUIActive("UnityCheatMainManager.WorldToolTip", true);
            WorldToolTip.AddComponent<TooltipGUI>().Initialize("内置修改器", null, WorldToolTip);

            CreateTopNavBar();
            CreateDropdown();

            UIPanels.Add(Panels.MainWindow, new MainWindow(UiBase));
            UIPanels.Add(Panels.ESPWindow, new ESPWindow(UiBase));
            UIPanels.Add(Panels.SpawnWindow, new SpanwWinodw(UiBase));
            UIPanels.Add(Panels.OptionWindow, new OptionWindow(UiBase));
            UIPanels.Add(Panels.SponsorWindow, new SponsorWindow(UiBase)); 

            AddPanelButtonsToNavbar();

            foreach (Dropdown dropdown in UIRoot.GetComponentsInChildren<Dropdown>(true))
                dropdown.RefreshShownValue();

            Initializing = false;
            ShowMenu = false;
        }

        #region [导航栏功能]
        public static CheatMainPanel GetPanel(Panels panel) => UIPanels[panel];
        public static T GetPanel<T>(Panels panel) where T : CheatMainPanel => (T)UIPanels[panel];
        public static void TogglePanel(Panels panel)
        {
            CheatMainPanel uiPanel = GetPanel(panel);
            SetPanelActive(panel, !uiPanel.Enabled);
        }
        public static void SetPanelActive(Panels panelType, bool active)
        {
            var panel = GetPanel(panelType);
            panel.SetActive(active);
        }

        private static void CreateTopNavBar()
        {
            GameObject navbarPanel = UIFactory.CreateUIObject("MainNavbar", UIRoot);
            UIFactory.SetLayoutGroup<VerticalLayoutGroup>(navbarPanel, false, true, true, true, 5, 5, 5, 5, 5);
            navbarPanel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
            NavBarRect = navbarPanel.GetComponent<RectTransform>();
            NavBarRect.pivot = new Vector2(0f, 1f);

            NavBarRect.anchorMin = new Vector2(0f, 1f);
            NavBarRect.anchorMax = new Vector2(0f, 1f);
            NavBarRect.anchoredPosition = new Vector2(0, 0);
            NavBarRect.sizeDelta = NAVBAR_DIMENSIONS;

            TitleBar = UIFactory.CreateUIObject("TitleBar", UIRoot);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(TitleBar, false, false, true, true, 5, 4, 4, 4, 4, TextAnchor.MiddleCenter);
            TitleBar.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f);

            RectTransform titleBarRect = TitleBar.GetComponent<RectTransform>();
            titleBarRect.pivot = new Vector2(0.5f, 1f);
            titleBarRect.anchorMin = new Vector2(0.5f, 1f);
            titleBarRect.anchorMax = new Vector2(0.5f, 1f);
            titleBarRect.anchoredPosition = new Vector2(0, 0);
            titleBarRect.sizeDelta = TITLE_BAR_DIMENSIONS;

            GameObject leftSpacer = UIFactory.CreateUIObject("LeftSpacer", TitleBar);
            UIFactory.SetLayoutElement(leftSpacer, flexibleWidth: 999);

            string titleTxt = $"{BepInExShow.PLUGIN_NAME} <i><color=grey>{BepInExShow.PLUGIN_VERSION}</color></i> {BepInExShow.For_Name}";
            Title = UIFactory.CreateLabel(TitleBar, "Title", titleTxt, TextAnchor.MiddleCenter, default, true, CheatMain.FontSize.Value);
            Title.SetLayoutElement(minWidth: 300, flexibleWidth: 0);

            GameObject rightSpacer = UIFactory.CreateUIObject("RightSpacer", TitleBar);
            UIFactory.SetLayoutElement(rightSpacer, flexibleWidth: 999);

            var JMPButton = UIFactory.CreateButton(TitleBar, "JMPButton", "跳转Mod发布页面");
            CheatMainUIManager.SetLayoutElement(JMPButton, minHeight: 25, minWidth: 60, flexibleWidth: 0);
            RuntimeHelper.SetColorBlock(JMPButton.Component, new Color(0.63f, 0.32f, 0.31f),
                new Color(0.81f, 0.25f, 0.2f), new Color(0.6f, 0.18f, 0.16f));
            JMPButton.OnClick += OnJMPButtonClicked;

            closeBtn = UIFactory.CreateButton(TitleBar, "CloseButton", CheatMain.ShowCounter.Value.ToString());
            CheatMainUIManager.SetLayoutElement(closeBtn, minHeight: 25, minWidth: 60, flexibleWidth: 0);
            RuntimeHelper.SetColorBlock(closeBtn.Component, new Color(0.63f, 0.32f, 0.31f),
                new Color(0.81f, 0.25f, 0.2f), new Color(0.6f, 0.18f, 0.16f));

            navbarPanelObject = navbarPanel;
            MycloseBtn = closeBtn;
            closeBtn.OnClick += OnCloseButtonClicked;

            NavbarTabButtonHolder = UIFactory.CreateUIObject("NavTabButtonHolder", navbarPanel);
            UIFactory.SetLayoutElement(NavbarTabButtonHolder, minHeight: 25, flexibleHeight: 999, flexibleWidth: 999);

            // 设置为垂直布局，按钮上下排列
            VerticalLayoutGroup layoutGroup = UIFactory.SetLayoutGroup<VerticalLayoutGroup>(
                NavbarTabButtonHolder,
                false, true, true, true,
                4, 2, 2, 2, 2
            );

            // 让按钮宽度填充可用空间，高度保持固定
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
        }

        private static void OnJMPButtonClicked()
        {
            Process.Start(new ProcessStartInfo("https://bbs.3dmgame.com/thread-6610072-1-1.html") { UseShellExecute = true });
        }
        private static void AddPanelButtonsToNavbar()
        {
            // 定义需要保留的按钮文本
            string[] keepButtons = { "主界面","ESP", "各种生成", "配置设置","其他" };

            // 清理重复按钮（只保留第一个匹配的按钮）
            ClearDuplicateButtons(keepButtons);

            // 确保需要的按钮存在
            EnsureRequiredButtons(keepButtons);

            // 统一按钮尺寸并填满宽度
            UniformButtonSizes();
        }

        private static void ClearDuplicateButtons(string[] keepTexts)
        {
            Dictionary<string, int> buttonCounts = new Dictionary<string, int>();
            List<GameObject> buttonsToRemove = new List<GameObject>();

            // 初始化计数
            foreach (string text in keepTexts)
                buttonCounts[text] = 0;

            // 检查所有按钮
            foreach (Transform child in NavbarTabButtonHolder.transform)
            {
                Text btnText = child.GetComponentInChildren<Text>();
                if (btnText == null) continue;

                string text = btnText.text;
                if (buttonCounts.ContainsKey(text))
                {
                    if (buttonCounts[text] >= 1)
                    {
                        buttonsToRemove.Add(child.gameObject);
                    }
                    else
                    {
                        buttonCounts[text]++;
                    }
                }
                else
                {
                    buttonsToRemove.Add(child.gameObject);
                }
            }

            // 删除重复按钮
            buttonsToRemove.ForEach(GameObject.Destroy);
        }

        private static void EnsureRequiredButtons(string[] requiredTexts)
        {
            foreach (string text in requiredTexts)
            {
                bool exists = false;
                foreach (Transform child in NavbarTabButtonHolder.transform)
                {
                    Text btnText = child.GetComponentInChildren<Text>();
                    if (btnText != null && btnText.text == text)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    // 根据文本确定面板类型
                    Panels panelType = GetPanelTypeFromText(text);
                    CreatePanelButton(panelType, text);
                }
            }
        }

        private static Panels GetPanelTypeFromText(string text)
        {
            switch (text)
            {
                case "主界面": return Panels.MainWindow;
                case "ESP": return Panels.ESPWindow;
                case "各种生成": return Panels.SpawnWindow;
                case "配置设置": return Panels.OptionWindow;
                case "其他": return Panels.SponsorWindow;
                default: return Panels.MainWindow;
            }
        }

        private static void CreatePanelButton(Panels panelType, string buttonText)
        {
            ButtonRef panelButton = UIFactory.CreateButton(NavbarTabButtonHolder, $"PanelButton_{panelType}", buttonText);

            // 设置按钮布局参数，宽度填满，高度固定
            UIFactory.SetLayoutElement(
                panelButton.GameObject,
                minHeight: 30,
                minWidth: 0,    // 不设置最小宽度，让它们根据可用空间调整
                flexibleWidth: 999,  // 允许宽度扩展
                flexibleHeight: 0  // 高度固定
            );

            panelButton.OnClick += () =>
            {
                foreach (Panels p in UIPanels.Keys)
                {
                    if (p != panelType)
                        SetPanelActive(p, false);
                }
                TogglePanel(panelType);
            };

            panelButton.Component.colors = new ColorBlock
            {
                normalColor = new Color(0.2f, 0.2f, 0.2f),
                highlightedColor = new Color(0.3f, 0.3f, 0.3f),
                pressedColor = new Color(0.15f, 0.15f, 0.15f),
                disabledColor = new Color(0.1f, 0.1f, 0.1f),
                colorMultiplier = 1f,
                fadeDuration = 0.1f
            };
        }

        private static void UniformButtonSizes()
        {
            // 获取导航栏宽度
            float navBarWidth = NavBarRect.sizeDelta.x;

            // 获取布局组的左右边距
            VerticalLayoutGroup layoutGroup = NavbarTabButtonHolder.GetComponent<VerticalLayoutGroup>();
            float padding = layoutGroup.padding.left + layoutGroup.padding.right;

            // 应用导航栏宽度到所有按钮，减去边距
            foreach (Transform child in NavbarTabButtonHolder.transform)
            {
                Button button = child.GetComponent<Button>();
                if (button != null)
                {
                    LayoutElement layout = button.GetComponent<LayoutElement>();
                    if (layout != null)
                    {
                        // 设置为导航栏宽度减去边距
                        layout.minWidth = navBarWidth - padding;
                        layout.flexibleWidth = 0;  // 禁用弹性宽度，使用固定计算值
                    }
                }
            }
        }

        private static void OnCloseButtonClicked()
        {
            ShowMenu = false;
        }
        #endregion

        #region [下拉栏功能]
        private static void CreateDropdown()
        {
            DropdownRoot = UIFactory.CreateUIObject("DropdownRoot", UIRoot);
            UIFactory.SetLayoutElement(DropdownRoot, minWidth: 300, minHeight: 200, flexibleWidth: 0, flexibleHeight: 0);

            RectTransform dropdownRect = DropdownRoot.GetComponent<RectTransform>();
            dropdownRect.anchorMin = new Vector2(0.5f, 0.5f);
            dropdownRect.anchorMax = new Vector2(0.5f, 0.5f);
            dropdownRect.pivot = new Vector2(0.5f, 0.5f);
            dropdownRect.anchoredPosition = Vector2.zero;

            Image dropdownBg = DropdownRoot.AddComponent<Image>();
            dropdownBg.color = new Color(0.15f, 0.15f, 0.15f);

            var titleObj = UIFactory.CreateLabel(DropdownRoot, "DropdownTitle", "选择选项", TextAnchor.MiddleLeft, default, true, 14);
            UIFactory.SetLayoutElement(titleObj.gameObject, minHeight: 30, flexibleHeight: 0);

            DropdownContent = DrawscrollPool(DropdownRoot);
            DropdownRoot.SetActive(false);
        }

        public static void ToggleDropdown(bool show)
        {
            if (DropdownRoot != null)
                DropdownRoot.SetActive(show);
        }

        public static GameObject DrawscrollPool(GameObject ContentRoot)
        {
            var scrollPool = UIFactory.CreateScrollPool<ICell>(
                parent: ContentRoot,
                name: "MainScrollPool",
                uiRoot: out GameObject scrollRoot,
                content: out GameObject scrollContent,
                bgColor: new Color(0.12f, 0.12f, 0.12f)
            );

            UIFactory.SetLayoutElement(scrollRoot, flexibleWidth: 9999, flexibleHeight: 1000);

            var scrollRect = scrollRoot.GetComponent<ScrollRect>();
            if (scrollRect == null)
            {
                CheatMain.WriteLog("ScrollRect component not found on scrollRoot.");
                return null;
            }

            scrollRect.vertical = true;
            scrollRect.horizontal = false;

            var scrollbarObj = UIFactory.CreateScrollbar(scrollRoot, "Scrollbar", out Scrollbar scrollbar);
            if (scrollbar == null)
            {
                CheatMain.WriteLog("Scrollbar component not found on scrollbarObj.");
                return null;
            }

            scrollbar.direction = Scrollbar.Direction.TopToBottom;

            RectTransform scrollbarRect = scrollbarObj.GetComponent<RectTransform>();
            if (scrollbarRect == null)
            {
                CheatMain.WriteLog("RectTransform component not found on scrollbarObj.");
                return null;
            }

            scrollbarRect.anchorMin = new Vector2(1, 1);
            scrollbarRect.anchorMax = new Vector2(1, 1);
            scrollbarRect.pivot = new Vector2(1, 0.5f);
            scrollbarRect.sizeDelta = new Vector2(20, 0);
            scrollbarRect.anchoredPosition = Vector2.zero;

            scrollRect.verticalScrollbar = scrollbar;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

            Image scrollbarBackground = scrollbarObj.GetComponentInChildren<Image>();
            if (scrollbarBackground != null)
            {
                scrollbarBackground.color = new Color(0.2f, 0.2f, 0.2f);
            }

            if (scrollbar.handleRect != null)
            {
                Image scrollbarHandle = scrollbar.handleRect.GetComponent<Image>();
                if (scrollbarHandle != null)
                {
                    scrollbarHandle.color = new Color(0.5f, 0.5f, 0.5f);
                }
            }

            UIFactory.SetLayoutElement(scrollbar.gameObject, minWidth: 20, flexibleWidth: 0);

            scrollRect.inertia = true;
            scrollRect.decelerationRate = 0.135f;

            scrollRect.content = scrollContent.GetComponent<RectTransform>();

            return scrollContent;
        }
        #endregion

        private static void CreateCloseButton(GameObject navbarPanel)
        {
            var closeBtn = UIFactory.CreateButton(navbarPanel, "CloseButton", CheatMain.ShowCounter.Value.ToString());
            CheatMainUIManager.SetLayoutElement(closeBtn, minHeight: 25, minWidth: 60, flexibleWidth: 0);
            RuntimeHelper.SetColorBlock(closeBtn.Component, new Color(0.63f, 0.32f, 0.31f),
                new Color(0.81f, 0.25f, 0.2f), new Color(0.6f, 0.18f, 0.16f));
            navbarPanelObject = navbarPanel;
            MycloseBtn = closeBtn;
        }

        public static void OnShowCounterValueChanged(ButtonRef closeBtn, GameObject navbarPanel)
        {
            UnityEngine.Object.Destroy(closeBtn.GameObject);
            CreateCloseButton(navbarPanel);
        }
    }
}