using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCheatMainManager.UI.Cells;
using UnityCheatMainManager.UI.Models;
using UnityCheatMainManager.UI.Panels;
using UnityCheatMainManager.UI;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI.Models;
using UniverseLib.UI;

namespace UnityCheatMainManager.Spawn.CreateSpawnUI.Buff
{
    internal class BuffWindow : CheatMainPanel
    {
        public static BuffWindow Instance { get; private set; }
        public override ManagerUI.Panels PanelType => ManagerUI.Panels.BuffWindow;
        public override string Name => "Buff添加";
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(0.35f, 0.175f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.8f, 0.925f);
        public override Vector2 DefaultPosition => new Vector2(0f, 0f);

        private readonly List<ForBuff> CheatBuffs = new List<ForBuff>();
        public int BuffCount => CheatBuffs.Count;

        public override bool NavButtonWanted => false;

        //public ScrollPool<BuffCell> ScrollPool { get; private set; }
        public bool Initialized { get; private set; } = false;

        public Dropdown BuffTypeDropdown;

        public Dropdown CreatTypeDropdown;

        public static InputFieldRef SearchInput;

        public GameObject BuffGrid;
        public UniverseLib.UI.Widgets.AutoSliderScrollbar autoSliderScrollbar;
        public List<GameObject> Buffs = new List<GameObject>();

        public GameObject BuffGridScrollView;
        private bool CanAddListener = true;
        private int BuffTypeDropdownIndex = 0;

        public override void OnFinishResize()
        {
            base.OnFinishResize();
        }
        public BuffWindow(UIBase owner) : base(owner)
        {
            Instance = this;
            Initialize();
        }
        public void Initialize()
        {
            if (BuffSet.BaseBuffs.Count != 0)
            {
                Initialized = true;
                CheatBuffs.Clear();
                CheatBuffs.AddRange(SearchInput.Text.FilterBuffData(0));
                PrintBuffs();
            }
        }
        public void SetCell(BuffCell cell, int index)
        {

            if (index < 0 || index >= CheatBuffs.Count)
            {
                cell.Disable();
                return;
            }

            ForBuff entry = CheatBuffs[index];
            cell.NameLabel.text = entry.GetBuffName();
            cell.BuffImage.sprite = entry.GetBuffIcon();
            //cell.DescriptionLabel.text = entry.GetBuffDescription();
            cell.SubmitButton.OnClick = new Action(() =>
            {
                entry.SpwanBuff();
            });
            cell.SubmitButton.OnClick = new Action(() =>
            {
                entry.DelBuff();
            });
        }
        public void OnCellBorrowed(BuffCell cell)
        {

        }
        public void SearchBuff(string text)
        {
            CheatBuffs.Clear();
            CheatBuffs.AddRange(text.FilterBuffData(BuffTypeDropdown.value));
            PrintBuffs();
        }
        public void ChangeBuffCount(string text)
        {
            if (int.TryParse(text, out int result))
            {
                BuffSet.SpawnCount = result;
            }
        }
        private void BuffTypeDropdownValueChange(int value)
        {
            CheatBuffs.Clear();
            CheatBuffs.AddRange(SearchInput.Text.FilterBuffData(value));
            BuffSet.isDataFetched = false;
            PrintBuffs();
            BuffSet.Page = value;
        }



        public void PrintBuffs()
        {
            foreach (var Buff in Buffs)
            {
                UnityEngine.Object.Destroy(Buff);
            }
            Buffs.Clear();

            foreach (var Buff in CheatBuffs)
            {
                var UIRoot = UIFactory.CreateUIObject("GridUIRoot", BuffGrid, new Vector2(200, 60));
                var Rect = UIRoot.GetComponent<RectTransform>();

                // 添加按钮 
                var SubmitButton = UIFactory.CreateButton(UIRoot, "SubmitButton", "添加", new Color(0.15f, 0.19f, 0.15f));
                SubmitButton.OnClick = new Action(() =>
                {
                    Buff.SpwanBuff();
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
                    Buff.DelBuff();
                });
                RemoveButton.ButtonText.fontSize = CheatMain.FontSize.Value;
                float removeButtonWidth = 40f;
                RemoveButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth, 45);
                removeButtonWidth = RemoveButton.ButtonText.preferredWidth;
                RemoveButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth, 45);
                RemoveButton.Component.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth + 5, 50);

                float totalButtonWidth = addButtonWidth + removeButtonWidth + 10;

                var NameLabel = UIFactory.CreateLabel(UIRoot, "NameLabel", Buff.GetBuffName(), TextAnchor.MiddleLeft, fontSize: CheatMain.FontSize.Value);
                NameLabel.horizontalOverflow = HorizontalWrapMode.Wrap;

                float originalImageWidth = 50f;
                float originalImageX = -95f;
                float labelWidth = 200  - totalButtonWidth;

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
                tool.Initialize(Buff.GetBuffDescription(), BuffGridScrollView);
                Buffs.Add(UIRoot);
            }
        }

        public override void Update()
        {
            base.Update();
            if (!CanAddListener)
            {
                if (BuffTypeDropdownIndex != BuffTypeDropdown.value)
                {
                    BuffTypeDropdownIndex = BuffTypeDropdown.value;
                    BuffTypeDropdownValueChange(BuffTypeDropdownIndex);
                }
            }
            if (!Initialized)
            {
                Initialize();
            }
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
            {
                BuffSet.GetBaseBuffData();
            }

        }
        protected override void ConstructPanelContent()
        {
            GameObject tabGroup = UIFactory.CreateVerticalGroup(ContentRoot, "BuffWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 9999);

            tabGroup.Createlabel("需要进入存档Buff才会刷新").SetLayoutElement(250, 40);
            // Save button

            GameObject Row1 = UIFactory.CreateHorizontalGroup(tabGroup, "Row1", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row1, minHeight: 40, flexibleWidth: 9999);

            var lable1 = UIFactory.CreateLabel(Row1, "Lable", "种类", fontSize: CheatMain.FontSize.Value);
            Row1.SetLayoutElement(minHeight: 40, flexibleWidth: 0);

            GameObject gameObject2 = UIFactory.CreateDropdown(Row1, "BuffTypeDropdown", out BuffTypeDropdown, "选择物品类型", CheatMain.FontSize.Value, null, null);
            for (int i = 0; i < BuffSet.BuffTypeDic.Count; i++)
            {
                BuffTypeDropdown.options.Add(new Dropdown.OptionData(BuffSet.BuffTypeDic[i]));
            }
            BuffTypeDropdown.value = 0;
            UIFactory.SetLayoutElement(gameObject2, minHeight: 40, minWidth: 180);
            BuffTypeDropdown.captionText.fontSize = CheatMain.FontSize.Value;
            BuffTypeDropdown.onValueChanged.AddListener((UnityEngine.Events.UnityAction<int>)BuffTypeDropdownValueChange);

            Row1.CreateButton("刷新页面", () =>
            {
                BuffTypeDropdownValueChange(BuffSet.Page);
            }).SetLayoutElement(40, 40);
            Row1.CreateToggle(150, "添加的Buff无限时间", (state) => { BuffSet.AddBuffTimeIsinfinite = state; });

            GameObject Row2 = UIFactory.CreateHorizontalGroup(tabGroup, "Row2", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row2, minHeight: 40, flexibleWidth: 0);

            var lable2 = UIFactory.CreateLabel(Row2, "Lable", "搜索", fontSize: CheatMain.FontSize.Value);
            lable2.SetLayoutElement(minHeight: 40, flexibleWidth: 0);

            SearchInput = UIFactory.CreateInputField(Row2, "BuffSearchInput", BuffSet.DefaultSearchText);
            SearchInput.SetLayoutElement(minWidth: 180, minHeight: 40);
            SearchInput.OnValueChanged += new Action<string>(SearchBuff);

            GameObject Row3 = UIFactory.CreateHorizontalGroup(tabGroup, "Row3", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row3, minHeight: 40, flexibleWidth: 0);

            var lable3 = UIFactory.CreateLabel(Row3, "Lable", "时间倍数", fontSize: CheatMain.FontSize.Value);
            lable3.SetLayoutElement(minHeight: 40, flexibleWidth: 0);

            var CountInput = UIFactory.CreateInputField(Row3, "BuffCountInput", "1");
            CountInput.SetLayoutElement(minWidth: 180, minHeight: 40);
            CountInput.OnValueChanged += new Action<string>(ChangeBuffCount);

            BuffGridScrollView = tabGroup.CreateGridScrollView("BuffGridScrollView", new Vector2(230, 60), new Vector2(10, 5), out BuffGrid, out autoSliderScrollbar);
        }
    }
}
