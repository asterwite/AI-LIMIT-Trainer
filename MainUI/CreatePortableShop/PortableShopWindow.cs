using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCheatMainManager.UI.Models;
using UnityCheatMainManager.UI;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI.Models;
using UniverseLib.UI;
using UnityCheatMainManager.UI.Panels;
using static UnityEngine.UI.Dropdown;

namespace UnityCheatMainManager.MainUI.CreatePortableShop
{
    public class PortableShopWindow : CheatMainPanel
    {
        public static PortableShopWindow Instance { get; private set; }
        public override ManagerUI.Panels PanelType => ManagerUI.Panels.PortableShopWindow;
        public override string Name => "随身商店";
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(0.35f, 0.175f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.8f, 0.925f);
        public override Vector2 DefaultPosition => new Vector2(0f, 0f);

        private readonly List<ForPortableShop> CheatPortableShops = new List<ForPortableShop>();
        public int PortableShopCount => CheatPortableShops.Count;

        public override bool NavButtonWanted => false;

        //public ScrollPool<PortableShopCell> ScrollPool { get; private set; }
        public bool Initialized { get; private set; } = false;

        public Dropdown PortableShopTypeDropdown;
        private static Dropdown PortableShopUseKeyDropdown;
        private static Array UsePortableShopKeyCodes;
        public static BepInEx.Configuration.ConfigEntry<KeyCode> UsePortableShopShowKey { get; set; }

        public static InputFieldRef SearchInput;

        public GameObject PortableShopGrid;
        public UniverseLib.UI.Widgets.AutoSliderScrollbar autoSliderScrollbar;
        public List<GameObject> PortableShops = new List<GameObject>();

        public GameObject PortableShopGridScrollView;
        private bool CanAddListener = true;
        private int PortableShopTypeDropdownIndex = 0;

        public override void OnFinishResize()
        {
            base.OnFinishResize();
        }
        public PortableShopWindow(UIBase owner) : base(owner)
        {
            Instance = this;
            Initialize();
        }
        public void Initialize()
        {
            if (PortableShopSet.BasePortableShops.Count != 0)
            {
                Initialized = true;
                CheatPortableShops.Clear();
                CheatPortableShops.AddRange(SearchInput.Text.FilterPortableShopData(0));
                PrintPortableShops();
            }
        }
        public void SetCell(PortableShopCell cell, int index)
        {

            if (index < 0 || index >= CheatPortableShops.Count)
            {
                cell.Disable();
                return;
            }

            ForPortableShop entry = CheatPortableShops[index];
            cell.NameLabel.text = entry.GetPortableShopName();
            cell.PortableShopImage.sprite = entry.GetPortableShopIcon();
            //cell.DescriptionLabel.text = entry.GetPortableShopDescription();
            cell.SubmitButton.OnClick = new Action(() =>
            {
                entry.AddPortableShopItem();
            });
            cell.SubmitButton.OnClick = new Action(() =>
            {
                entry.DelPortableShopItem();
            });
        }
        public void OnCellBorrowed(PortableShopCell cell)
        {

        }
        public void SearchPortableShop(string text)
        {
            CheatPortableShops.Clear();
            CheatPortableShops.AddRange(text.FilterPortableShopData(PortableShopTypeDropdown.value));
            PrintPortableShops();
        }
        public void ChangePortableShopCount(string text)
        {
            if (int.TryParse(text, out int result))
            {
                PortableShopSet.SpawnCount = result;
            }
        }

        private void PortableShopTypeDropdownValueChange(int value)
        {
            CheatPortableShops.Clear();
            CheatPortableShops.AddRange(SearchInput.Text.FilterPortableShopData(value));
            PortableShopSet.isDataFetched = false;
            PrintPortableShops();
            PortableShopSet.Page = value;
        }



        public void PrintPortableShops()
        {
            foreach (var PortableShop in PortableShops)
            {
                UnityEngine.Object.Destroy(PortableShop);
            }
            PortableShops.Clear();

            foreach (var PortableShop in CheatPortableShops)
            {
                var UIRoot = UIFactory.CreateUIObject("GridUIRoot", PortableShopGrid, new Vector2(200, 60));
                var Rect = UIRoot.GetComponent<RectTransform>();

                // 添加按钮 
                var SubmitButton = UIFactory.CreateButton(UIRoot, "SubmitButton", "添加", new Color(0.15f, 0.19f, 0.15f));
                SubmitButton.OnClick = new Action(() =>
                {
                    PortableShop.AddPortableShopItem();
                });
                SubmitButton.ButtonText.fontSize = CheatMain.FontSize.Value;
                float addButtonWidth = 40f;
                SubmitButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(addButtonWidth, 45);
                addButtonWidth = SubmitButton.ButtonText.preferredWidth;
                SubmitButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(addButtonWidth, 45);
                SubmitButton.Component.GetComponent<RectTransform>().sizeDelta = new Vector2(addButtonWidth + 5, 50);

                // 移除按钮 
                var RemoveButton = UIFactory.CreateButton(UIRoot, "RemoveButton", "移除", new Color(0.15f, 0.19f, 0.15f));
                RemoveButton.OnClick = new Action(() =>
                {
                    PortableShop.DelPortableShopItem();
                });
                RemoveButton.ButtonText.fontSize = CheatMain.FontSize.Value;
                float removeButtonWidth = 40f;
                RemoveButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth, 45);
                removeButtonWidth = RemoveButton.ButtonText.preferredWidth;
                RemoveButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth, 45);
                RemoveButton.Component.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth + 5, 50);

                float totalButtonWidth = addButtonWidth + removeButtonWidth + 10;

                var NameLabel = UIFactory.CreateLabel(UIRoot, "NameLabel", PortableShop.GetPortableShopName(), TextAnchor.MiddleLeft, fontSize: CheatMain.FontSize.Value);
                NameLabel.horizontalOverflow = HorizontalWrapMode.Wrap;

                float originalImageWidth = 50f;
                float originalImageX = -95f;
                float labelWidth = 200 - totalButtonWidth;

                NameLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(labelWidth, 50);
                // 设置为原图片的位置
                NameLabel.GetComponent<RectTransform>().localPosition = new Vector2(originalImageX + labelWidth / 2, 0);

                NameLabel.resizeTextForBestFit = true;
                NameLabel.resizeTextMinSize = 10;
                NameLabel.resizeTextMaxSize = CheatMain.FontSize.Value;

                // 设置添加按钮位置 
                SubmitButton.Component.GetComponent<RectTransform>().localPosition = new Vector2(95 - totalButtonWidth / 2, 0);
                // 设置移除按钮位置 
                RemoveButton.Component.GetComponent<RectTransform>().localPosition = new Vector2(95 - totalButtonWidth / 2 + addButtonWidth + 20, 0);

                var tool = UIRoot.AddComponent<TooltipGUI>();
                tool.Initialize(PortableShop.GetPortableShopDescription(), PortableShopGridScrollView);
                PortableShops.Add(UIRoot);
            }
        }

        public override void Update()
        {
            base.Update();
            if (!CanAddListener)
            {
                if (PortableShopTypeDropdownIndex != PortableShopTypeDropdown.value)
                {
                    PortableShopTypeDropdownIndex = PortableShopTypeDropdown.value;
                    PortableShopTypeDropdownValueChange(PortableShopTypeDropdownIndex);
                }
            }
            if (!Initialized)
            {
                Initialize();
            }
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
            {
                PortableShopSet.GetBasePortableShopData();
            }

        }
        protected override void ConstructPanelContent()
        {
            GameObject tabGroup = UIFactory.CreateVerticalGroup(ContentRoot, "PortableShopWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 9999);

            tabGroup.Createlabel("需要进入存档随身商店才会刷新").SetLayoutElement(250, 40);

            GameObject PortableShop = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowPlayerFightMagicInput",
               false, false, true, true, 5, default, new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);

            PortableShop.Createlabel("快速使用魔法快捷键").SetLayoutElement(180, 40);

            GameObject gameObject = UIFactory.CreateDropdown(PortableShop, "PortableShopKeyTypeDown",
                out PortableShopUseKeyDropdown, "快捷键", CheatMain.FontSize.Value, null, null);

            Il2CppSystem.Collections.Generic.List<OptionData> optionDataList = new Il2CppSystem.Collections.Generic.List<OptionData>();
            UsePortableShopKeyCodes = Enum.GetValues(typeof(UnityEngine.KeyCode)); ;
            foreach (UnityEngine.KeyCode keyCode in UsePortableShopKeyCodes)
            {
                optionDataList.Add(new OptionData(keyCode.ToString()));
            }
            UIFactory.SetLayoutElement(gameObject, minHeight: 40, minWidth: 200);
            PortableShopUseKeyDropdown.captionText.fontSize = CheatMain.FontSize.Value;

            //PortableShop.CreateButton("设置", () =>
            //{
            //    MagicKeyCodeChange(PortableShopUseKeyDropdown.value);
            //}).SetLayoutElement(80, 40);
            //PortableShopUseKeyDropdown.AddOptions(optionDataList);
            //PortableShopUseKeyDropdown.value = Array.IndexOf(UsePortableShopKeyCodes, UsePortableShopShowKey.Value);

            CreateUI.FightUI.QuickUseMagic.SetDropdownScrollSpeed(PortableShopUseKeyDropdown, 10f);

            GameObject Row1 = UIFactory.CreateHorizontalGroup(tabGroup, "Row1", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row1, minHeight: 40, flexibleWidth: 9999);

            var lableSpawnType = UIFactory.CreateLabel(Row1, "LableSpawnType", "随身商店物品类型", fontSize: CheatMain.FontSize.Value);
            lableSpawnType.SetLayoutElement(minHeight: 40, flexibleWidth: 0);

            GameObject SpawnTypeDrop = UIFactory.CreateDropdown(Row1, "PortableShopSpawnTypeDropdown", out PortableShopTypeDropdown, "选择随身商店物品生成类型", CheatMain.FontSize.Value, null, null);
            for (int i = 0; i < PortableShopSet.CreatTypeDic.Count; i++)
            {
                PortableShopTypeDropdown.options.Add(new Dropdown.OptionData(PortableShopSet.CreatTypeDic[i]));
            }
            UIFactory.SetLayoutElement(SpawnTypeDrop, minHeight: 40, minWidth: 180);
            PortableShopTypeDropdown.captionText.fontSize = CheatMain.FontSize.Value;
            PortableShopTypeDropdown.onValueChanged.AddListener((UnityEngine.Events.UnityAction<int>)PortableShopTypeDropdownValueChange);

            GameObject Row2 = UIFactory.CreateHorizontalGroup(tabGroup, "Row2", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row2, minHeight: 40, flexibleWidth: 0);

            var lable2 = UIFactory.CreateLabel(Row2, "Lable", "搜索", fontSize: CheatMain.FontSize.Value);
            lable2.SetLayoutElement(minHeight: 40, flexibleWidth: 0);

            SearchInput = UIFactory.CreateInputField(Row2, "PortableShopSearchInput", PortableShopSet.DefaultSearchText);
            SearchInput.SetLayoutElement(minWidth: 180, minHeight: 40);
            SearchInput.OnValueChanged += new Action<string>(SearchPortableShop);

            GameObject Row3 = UIFactory.CreateHorizontalGroup(tabGroup, "Row3", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row3, minHeight: 40, flexibleWidth: 0);

            var lableSpanwCount = UIFactory.CreateLabel(Row3, "Lable", "添加数量", fontSize: CheatMain.FontSize.Value);
            lableSpanwCount.SetLayoutElement(minWidth: 40, minHeight: 40);

            var CountInput = UIFactory.CreateInputField(Row3, "PortableShopSpawnCountInput", "1");
            CountInput.SetLayoutElement(minWidth: 180, minHeight: 40);
            CountInput.OnValueChanged += new Action<string>(ChangePortableShopCount);

            PortableShopGridScrollView = tabGroup.CreateGridScrollView("PortableShopGridScrollView", new Vector2(230, 60), new Vector2(10, 5), out PortableShopGrid, out autoSliderScrollbar);
        }
        private static void MagicKeyCodeChange(int value)
        {
            if (value >= 0 && value < UsePortableShopKeyCodes.Length)
            {
                UsePortableShopShowKey.Value = (UnityEngine.KeyCode)UsePortableShopKeyCodes.GetValue(value);
                CheatMain.WriteLog($"已设置快捷呼出随身商店快捷键：{UsePortableShopShowKey}");
            }
        }
    }
}
