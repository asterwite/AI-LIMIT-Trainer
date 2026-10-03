using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCheatMainManager.UI;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using UnityCheatMainManager.UI.Panels;

namespace UnityCheatMainManager.MainUI.CreateUI.FightUI
{
    public class PlayerFightUseWindow : CheatMainPanel
    {
        public static PlayerFightUseWindow Instance { get; private set; }
        public override ManagerUI.Panels PanelType => ManagerUI.Panels.FightUseWindow;
        public override string Name => "战斗物品使用";
        public override int MinWidth => 350;
        public override int MinHeight => 200;
        public override bool ShowByDefault => false;
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

        public PlayerFightUseWindow(UIBase owner) : base(owner)
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

            GameObject PlayerFightEat = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowPlayerFightEat", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);

            PlayerFightEat.CreateButton("喝圣子之血结晶", () =>
            {
                DrinkEXMedicine eat1 = new DrinkEXMedicine();
                eat1._Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                eat1.DoAction(null, 0);
            }).SetLayoutElement(180, 40).gameObject.SetTooltip("真神药，一颗提神醒脑，两颗永不疲劳，这真的是喝吗？你确定不是捏吗", tabGroup);

            PlayerFightEat.CreateButton("喝药", () =>
            {
                DrinkMedicine eat1 = new DrinkMedicine();
                eat1._Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                eat1.DoAction(null, 0);
            }).SetLayoutElement(120, 40).gameObject.SetTooltip("恢复生命，这真的是喝吗？你确定不是捏吗", tabGroup);

            PlayerFightEat.CreateButton("吃药一级", () =>
            {
                Eat1 eat = new Eat1();
                eat._Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                eat.DoAction(null, 0);
            }).SetLayoutElement(120, 40).gameObject.SetTooltip("一级恢复，饿了怎么办，吃就好了", tabGroup);

            PlayerFightEat.CreateButton("吃药二级", () =>
            {
                Eat2 eat = new Eat2();
                eat._Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                eat.DoAction(null, 0);
            }).SetLayoutElement(120, 40).gameObject.SetTooltip("二级恢复，饿了怎么办，吃就好了", tabGroup);

            PlayerFightEat.CreateButton("吃药三级", () =>
            {
                Eat3 eat = new Eat3();
                eat._Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                eat.DoAction(null, 0);
            }).SetLayoutElement(120, 40).gameObject.SetTooltip("三级恢复，饿了怎么办，吃就好了", tabGroup);


            GameObject PlayerFightEat1 = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowPlayerFightEat1", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);

            PlayerFightEat1.CreateButton("吃混乱胎种", () =>
            {
                Eat3 eat = new Eat3();
                eat._Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                eat.DoAction(null, 0);
            }).SetLayoutElement(120, 40).gameObject.SetTooltip("吃吃吃就知道吃，什么都吃是吧，真不挑啊你", tabGroup);


            PlayerFightEat1.CreateButton("吃失活胎种", () =>
            {
                Eat3 eat = new Eat3();
                eat._Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                eat.DoAction(null, 0);
            }).SetLayoutElement(120, 40).gameObject.SetTooltip("吃吃吃就知道吃，什么都吃是吧，真不挑啊你", tabGroup);

            GameObject PlayerFightUse = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowPlayerFightUse", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            PlayerFightUse.CreateButton("武器附火", () =>
            {
                EnchantmentFire WeaponFire = new EnchantmentFire();
                WeaponFire._Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                WeaponFire.DoAction(null, 0);
            }).SetLayoutElement(120, 40).gameObject.SetTooltip("武器附火吗？有点意思", tabGroup);

            PlayerFightUse.CreateButton("武器附电", () =>
            {
                EnchantmentLighting WeapontLighting = new EnchantmentLighting();
                WeapontLighting._Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                WeapontLighting.DoAction(null, 0);
            }).SetLayoutElement(120, 40).gameObject.SetTooltip("武器附电吗？有点意思", tabGroup);

            PlayerFightUse.CreateButton("返回上个传送点", () =>
            {
                GoBackHome GoHome = new GoBackHome();
                GoHome._Player = ProcedureMonoSingleton<CameraManager>.Instance._Owner;
                GoHome.DoAction(null, 0);
            }).SetLayoutElement(180, 40).gameObject.SetTooltip("不玩了，我回家了", tabGroup);


            QuickUseMagic.DrawUseMagicUI(tabGroup);



            SetActive(true);
        }


    }
}
