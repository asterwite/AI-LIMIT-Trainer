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
using UnityEngine.Playables;

namespace UnityCheatMainManager.Spawn.CreateSpawnUI.SpawnMonster
{
    internal class MonsterWindow : CheatMainPanel
    {
        public static MonsterWindow Instance { get; private set; }
        public override ManagerUI.Panels PanelType => ManagerUI.Panels.MonsterWindow;
        public override string Name => "怪物生成";
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override Vector2 DefaultAnchorMin => new Vector2(0.35f, 0.175f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.8f, 0.925f);
        public override Vector2 DefaultPosition => new Vector2(0f, 0f);

        private readonly List<ForMonster> CheatMonsters = new List<ForMonster>();
        public int MonsterCount => CheatMonsters.Count;

        public override bool NavButtonWanted => false;

        //public ScrollPool<MonsterCell> ScrollPool { get; private set; }
        public bool Initialized { get; private set; } = false;

        public Dropdown MonsterTypeDropdown;

        public Dropdown CreatTypeDropdown;

        public static InputFieldRef SearchInput;

        public GameObject MonsterGrid;
        public UniverseLib.UI.Widgets.AutoSliderScrollbar autoSliderScrollbar;
        public List<GameObject> Monsters = new List<GameObject>();

        public GameObject MonsterGridScrollView;
        private bool CanAddListener = true;
        private int MonsterTypeDropdownIndex = 0;

        public override void OnFinishResize()
        {
            base.OnFinishResize();
        }
        public MonsterWindow(UIBase owner) : base(owner)
        {
            Instance = this;
            Initialize();
        }
        public void Initialize()
        {
            if (MonsterSet.BaseMonsters.Count != 0)
            {
                Initialized = true;
                CheatMonsters.Clear();
                CheatMonsters.AddRange(SearchInput.Text.FilterMonsterData(0));
                PrintMonsters();
            }
        }
        public void SetCell(MonsterCell cell, int index)
        {

            if (index < 0 || index >= CheatMonsters.Count)
            {
                cell.Disable();
                return;
            }

            ForMonster entry = CheatMonsters[index];
            cell.NameLabel.text = entry.GetMonsterName();
            cell.MonsterImage.sprite = entry.GetMonsterIcon();
            //cell.DescriptionLabel.text = entry.GetMonsterDescription();
            cell.SubmitButton.OnClick = new Action(() =>
            {
                entry.SpwanMonster();
            });
            cell.SubmitButton.OnClick = new Action(() =>
            {
                entry.DelMonster();
            });
        }
        public void OnCellBorrowed(MonsterCell cell)
        {

        }
        public void SearchMonster(string text)
        {
            CheatMonsters.Clear();
            CheatMonsters.AddRange(text.FilterMonsterData(MonsterTypeDropdown.value));
            PrintMonsters();
        }
        public void ChangeMonsterCount(string text)
        {
            if (int.TryParse(text, out int result))
            {
                MonsterSet.SpawnCount = result;
            }
        }

        public void ChangeMonsteSpanwDis(string text)
        {
            if (int.TryParse(text, out int result))
            {
                MonsterSet.SpawnDis = result;
            }
        }

        private void MonsterTypeDropdownValueChange(int value)
        {
            CheatMonsters.Clear();
            CheatMonsters.AddRange(SearchInput.Text.FilterMonsterData(value));
            MonsterSet.isDataFetched = false;
            PrintMonsters();
            MonsterSet.Page = value;
        }



        public void PrintMonsters()
        {
            foreach (var Monster in Monsters)
            {
                UnityEngine.Object.Destroy(Monster);
            }
            Monsters.Clear();

            foreach (var Monster in CheatMonsters)
            {
                var UIRoot = UIFactory.CreateUIObject("GridUIRoot", MonsterGrid, new Vector2(200, 60));
                var Rect = UIRoot.GetComponent<RectTransform>();

                // 添加按钮 
                var SubmitButton = UIFactory.CreateButton(UIRoot, "SubmitButton", "生成", new Color(0.15f, 0.19f, 0.15f));
                SubmitButton.OnClick = new Action(() =>
                {
                    Monster.SpwanMonster();
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
                    Monster.DelMonster();
                });
                RemoveButton.ButtonText.fontSize = CheatMain.FontSize.Value;
                float removeButtonWidth = 40f;
                RemoveButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth, 45);
                removeButtonWidth = RemoveButton.ButtonText.preferredWidth;
                RemoveButton.ButtonText.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth, 45);
                RemoveButton.Component.GetComponent<RectTransform>().sizeDelta = new Vector2(removeButtonWidth + 5, 50);

                float totalButtonWidth = addButtonWidth + removeButtonWidth + 10;

                var NameLabel = UIFactory.CreateLabel(UIRoot, "NameLabel", Monster.GetMonsterName(), TextAnchor.MiddleLeft, fontSize: CheatMain.FontSize.Value);
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
                tool.Initialize(Monster.GetMonsterDescription(), MonsterGridScrollView);
                Monsters.Add(UIRoot);
            }
        }

        public override void Update()
        {
            base.Update();
            if (!CanAddListener)
            {
                if (MonsterTypeDropdownIndex != MonsterTypeDropdown.value)
                {
                    MonsterTypeDropdownIndex = MonsterTypeDropdown.value;
                    MonsterTypeDropdownValueChange(MonsterTypeDropdownIndex);
                }
            }
            if (!Initialized)
            {
                Initialize();
            }
            if (ProcedureMonoSingleton<CameraManager>.Instance && ProcedureMonoSingleton<CameraManager>.Instance._Owner)
            {
                MonsterSet.GetBaseMonsterData();
            }

        }
        protected override void ConstructPanelContent()
        {
            GameObject tabGroup = UIFactory.CreateVerticalGroup(ContentRoot, "MonsterWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 9999);

            tabGroup.Createlabel("需要进入存档怪物才会刷新").SetLayoutElement(250, 40);

            GameObject Row1 = UIFactory.CreateHorizontalGroup(tabGroup, "Row1", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row1, minHeight: 40, flexibleWidth: 9999);



            var lableSpawnType = UIFactory.CreateLabel(Row1, "LableSpawnType", "生成类型", fontSize: CheatMain.FontSize.Value);
            lableSpawnType.SetLayoutElement(minHeight: 40, flexibleWidth: 0);

            GameObject SpawnTypeDrop = UIFactory.CreateDropdown(Row1, "MonsterSpawnTypeDropdown", out CreatTypeDropdown, "选择怪物生成类型", CheatMain.FontSize.Value, null, null);
            for (int i = 0; i < MonsterSet.CreatTypeDic.Count; i++)
            {
                CreatTypeDropdown.options.Add(new Dropdown.OptionData(MonsterSet.CreatTypeDic[i]));
            }
            UIFactory.SetLayoutElement(SpawnTypeDrop, minHeight: 40, minWidth: 180);
            CreatTypeDropdown.captionText.fontSize = CheatMain.FontSize.Value;
            CreatTypeDropdown.onValueChanged.AddListener((UnityEngine.Events.UnityAction<int>)MonsterCreateTypeDropdownValueChange);

            Row1.CreateButton("删除全部已生成怪物", () =>
            {
                foreach (var LoaclMonster in MonsterSet.SpawnMonsterObject)
                {
                    UnityEngine.Object.Destroy(LoaclMonster.gameObject);

                }
            }).SetLayoutElement(250, 40).gameObject.SetTooltip("只能删除修改器生成的怪物，不能删除原本就有的怪物，或者控制台生成的怪物",tabGroup);

            GameObject Row2 = UIFactory.CreateHorizontalGroup(tabGroup, "Row2", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row2, minHeight: 40, flexibleWidth: 0);

            var lable2 = UIFactory.CreateLabel(Row2, "Lable", "搜索", fontSize: CheatMain.FontSize.Value);
            lable2.SetLayoutElement(minHeight: 40, flexibleWidth: 0);

            SearchInput = UIFactory.CreateInputField(Row2, "MonsterSearchInput", MonsterSet.DefaultSearchText);
            SearchInput.SetLayoutElement(minWidth: 180, minHeight: 40);
            SearchInput.OnValueChanged += new Action<string>(SearchMonster);

            GameObject Row3 = UIFactory.CreateHorizontalGroup(tabGroup, "Row3", false, false, true, true, 5, default,
                new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(Row3, minHeight: 40, flexibleWidth: 0);

            var lableSpanwCount = UIFactory.CreateLabel(Row3, "Lable", "生成数量", fontSize: CheatMain.FontSize.Value);
            lableSpanwCount.SetLayoutElement(minWidth: 40, minHeight: 40);

            var CountInput = UIFactory.CreateInputField(Row3, "MonsterSpawnCountInput", "1");
            CountInput.SetLayoutElement(minWidth: 180, minHeight: 40);
            CountInput.OnValueChanged += new Action<string>(ChangeMonsterCount);

            var lableSpanwDis = UIFactory.CreateLabel(Row3, "LableDis", "生成距离", fontSize: CheatMain.FontSize.Value);
            lableSpanwDis.SetLayoutElement(minWidth:40, minHeight: 40);

            var DisInput = UIFactory.CreateInputField(Row3, "MonsterSpawnDisInput", "5");
            DisInput.SetLayoutElement(minWidth: 180, minHeight: 40);
            DisInput.OnValueChanged += new Action<string>(ChangeMonsteSpanwDis);

            MonsterGridScrollView = tabGroup.CreateGridScrollView("MonsterGridScrollView", new Vector2(230, 60), new Vector2(10, 5), out MonsterGrid, out autoSliderScrollbar);
        }
        private void MonsterCreateTypeDropdownValueChange(int value)
        {
            MonsterSet.CreatType = value;
        }
    }
}
