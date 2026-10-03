using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI.Models;
using UniverseLib.UI;
using UnityCheatMainManager.UI.Models;
using UnityCheatMainManager.Hook;
using UnityCheatMainManager.ItemSpawn;
using UnityCheatMainManager.UI.Cells;
using UniverseLib;
using LevelEdit;

namespace UnityCheatMainManager.UI.Panels
{
    public class ItemWindow : CheatMainPanel
    {
        public static ItemWindow Instance { get; private set; }
        public override ManagerUI.Panels PanelType => ManagerUI.Panels.ItemWindow;
        public override string Name => "物品生成";
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(0.35f, 0.175f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.8f, 0.925f);
        public override Vector2 DefaultPosition => new Vector2(0f, 0f);

        private readonly List<ForItem> CheatItems = new List<ForItem>();
        public int ItemCount => CheatItems.Count;

        public override bool NavButtonWanted => false;

        //public ScrollPool<ItemCell> ScrollPool { get; private set; }
        public bool Initialized { get; private set; } = false;

        public Dropdown ItemTypeDropdown;

        public Dropdown CreatTypeDropdown;

        public static InputFieldRef SearchInput;

        public GameObject ItemGrid;
        public UniverseLib.UI.Widgets.AutoSliderScrollbar autoSliderScrollbar;
        public List<GameObject> Items = new List<GameObject>();

        public GameObject ItemGridScrollView;
        private bool CanAddListener = true;
        private int ItemTypeDropdownIndex = 0;

        public override void OnFinishResize()
        {
            base.OnFinishResize();
        }
        public ItemWindow(UIBase owner) : base(owner)
        {
            Instance = this;
            Initialize();
        }
        public void Initialize()
        {
            if (ItemSet.BaseItems.Count != 0)
            {
                Initialized = true;
                CheatItems.Clear();
                CheatItems.AddRange(SearchInput.Text.FilterItemData(ItemSet.Page));
                PrintItems();
            }
        }
        public void SetCell(ItemCell cell, int index)
        {

            if (index < 0 || index >= CheatItems.Count)
            {
                cell.Disable();
                return;
            }

            ForItem entry = CheatItems[index];
            cell.NameLabel.text = entry.GetItemName();
            cell.ItemImage.sprite = entry.GetItemIcon();
            //cell.DescriptionLabel.text = entry.GetItemDescription();
            cell.SubmitButton.OnClick = new Action(() =>
            {
                entry.SpwanItem();
            });
            cell.SubmitButton.OnClick = new Action(() =>
            {
                entry.DelItem();
            });
        }
        public void OnCellBorrowed(ItemCell cell)
        {

        }
        public void SearchItem(string text)
        {
            CheatItems.Clear();
            CheatItems.AddRange(text.FilterItemData(ItemTypeDropdown.value));
            PrintItems();
        }
        public void ChangeItemCount(string text)
        {
            if (int.TryParse(text, out int result))
            {
                ItemSet.SpawnCount = result;
            }
        }
        private void ItemTypeDropdownValueChange(int value)
        {
            CheatItems.Clear();
            CheatItems.AddRange(SearchInput.Text.FilterItemData(value));
            ItemSet.isDataFetched = false;
            PrintItems();
            ItemSet.Page = value;
        }



        public void PrintItems()
        {
            foreach (var Item in Items)
            {
                UnityEngine.Object.Destroy(Item);
            }
            Items.Clear();
            foreach (var Item in CheatItems)
            {
                var UIRoot = UIFactory.CreateUIObject("GridUIRoot", ItemGrid, new Vector2(200, 60));
                var Rect = UIRoot.GetComponent<RectTransform>();

                // 物品图片 
                var ItemUI = UIFactory.CreateUIObject("ItemUI", UIRoot, new Vector2(50, 50));
                var ItemImage = ItemUI.AddComponent<Image>();
                ItemImage.preserveAspect = true;
                ItemImage.sprite = Item.GetItemIcon();
                UIFactory.SetLayoutElement(ItemUI, minHeight: 50, minWidth: 50, flexibleWidth: 0, flexibleHeight: 0);
                ItemUI.GetComponent<RectTransform>().localPosition = new Vector2(-70, 0);

                // 添加按钮 
                var SubmitButton = UIFactory.CreateButton(UIRoot, "SubmitButton", "添加", new Color(0.15f, 0.19f, 0.15f));
                SubmitButton.OnClick = new Action(() =>
                {
                    Item.SpwanItem();
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
                    Item.DelItem();
                });
                RemoveButton.ButtonText.fontSize = CheatMain.FontSize.Value;
                float removeButtonWidth = 40f;
                RemoveButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth, 45);
                removeButtonWidth = RemoveButton.ButtonText.preferredWidth;
                RemoveButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth, 45);
                RemoveButton.Component.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth + 5, 50);

                // 计算按钮总宽度 
                float totalButtonWidth = addButtonWidth + removeButtonWidth + 10;

                // 物品名称标签 
                var NameLabel = UIFactory.CreateLabel(UIRoot, "NameLabel", Item.GetItemName(), TextAnchor.MiddleLeft, fontSize: CheatMain.FontSize.Value);
                NameLabel.horizontalOverflow = HorizontalWrapMode.Wrap;

                float labelWidth = 210 - 60 - totalButtonWidth;
                NameLabel.GetComponent<RectTransform>().sizeDelta = new Vector2(labelWidth, 50);
                NameLabel.GetComponent<RectTransform>().localPosition = new Vector2(-40 + labelWidth / 2, 0);

                NameLabel.resizeTextForBestFit = true;
                NameLabel.resizeTextMinSize = 10;
                NameLabel.resizeTextMaxSize = CheatMain.FontSize.Value;

                // 设置添加按钮位置 
                SubmitButton.Component.GetComponent<RectTransform>().localPosition = new Vector2(95 - totalButtonWidth / 2, 0);
                // 设置移除按钮位置 
                RemoveButton.Component.GetComponent<RectTransform>().localPosition = new Vector2(95 - totalButtonWidth / 2 + addButtonWidth + 20, 0);

                var tool = UIRoot.AddComponent<TooltipGUI>();
                tool.Initialize(Item.GetItemDescription(), ItemGridScrollView);
                Items.Add(UIRoot);
            }
        }

        public override void Update()
        {
            base.Update();
            if (!CanAddListener)
            {
                if (ItemTypeDropdownIndex != ItemTypeDropdown.value)
                {
                    ItemTypeDropdownIndex = ItemTypeDropdown.value;
                    ItemTypeDropdownValueChange(ItemTypeDropdownIndex);
                }
            }
            if (!Initialized)
            {
                Initialize();
            }
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
                ItemSet.GetBaseItemData();
        }
        protected override void ConstructPanelContent()
        {
            GameObject tabGroup = UIFactory.CreateVerticalGroup(ContentRoot, "ItemWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 9999);

            tabGroup.Createlabel("需要进入存档物品才会加载").SetLayoutElement(250, 40);
            // Save button

            GameObject Row0 = UIFactory.CreateHorizontalGroup(tabGroup, "Row0", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row0, minHeight: 40, flexibleWidth: 9999);
            Row0.CreateToggle(250, "添加到当前物品的最大堆叠值", (bool state) => { ItemSet.AddItemToMaxCount = state; });

            GameObject Row1 = UIFactory.CreateHorizontalGroup(tabGroup, "Row1", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row1, minHeight: 40, flexibleWidth: 9999);

            var lable1= UIFactory.CreateLabel(Row1, "Lable", "种类", fontSize: CheatMain.FontSize.Value);
            Row1.SetLayoutElement(minHeight: 40, flexibleWidth: 0);

            GameObject gameObject2 = UIFactory.CreateDropdown(Row1, "ItemTypeDropdown", out ItemTypeDropdown, "选择物品类型", CheatMain.FontSize.Value, null, null);
            for (int i = 0; i < ItemSet.ItemTypeDic.Count; i++)
            {
                ItemTypeDropdown.options.Add(new Dropdown.OptionData(ItemSet.ItemTypeDic[i]));
            }
            ItemTypeDropdown.value = 0;
            UIFactory.SetLayoutElement(gameObject2, minHeight: 40, minWidth: 180);
            ItemTypeDropdown.captionText.fontSize = CheatMain.FontSize.Value;
            ItemTypeDropdown.onValueChanged.AddListener(ItemTypeDropdownValueChange);

            Row1.CreateButton("刷新页面", () =>
            {
                ItemTypeDropdownValueChange(ItemSet.Page);
            }).SetLayoutElement(40, 20);


            GameObject Row2 = UIFactory.CreateHorizontalGroup(tabGroup, "Row2", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row2, minHeight: 40, flexibleWidth: 0);

            var lable2 = UIFactory.CreateLabel(Row2, "Lable", "搜索", fontSize: CheatMain.FontSize.Value);
            lable2.SetLayoutElement(minHeight: 40, flexibleWidth: 0);

            SearchInput = UIFactory.CreateInputField(Row2, "ItemSearchInput", ItemSet.DefaultSearchText);
            SearchInput.SetLayoutElement(minWidth: 180, minHeight: 40);
            SearchInput.OnValueChanged += new Action<string>(SearchItem);

            ////添加数量
            GameObject Row3 = UIFactory.CreateHorizontalGroup(tabGroup, "Row3", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row3, minHeight: 40, flexibleWidth: 0);

            var lable3 = UIFactory.CreateLabel(Row3, "Lable", "数量", fontSize: CheatMain.FontSize.Value);
            lable3.SetLayoutElement(minHeight: 40, flexibleWidth: 0);

            var CountInput = UIFactory.CreateInputField(Row3, "ItemCountInput", "1");
            CountInput.SetLayoutElement(minWidth: 180, minHeight: 40);
            CountInput.OnValueChanged += new Action<string>(ChangeItemCount);

            ItemGridScrollView = tabGroup.CreateGridScrollView("ItemGridScrollView", new Vector2(230, 60), new Vector2(10, 5), out ItemGrid, out autoSliderScrollbar);
        }
    }
}
