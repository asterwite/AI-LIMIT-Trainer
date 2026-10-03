using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityCheatMainManager.UI;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using UnityCheatMainManager.UI.Panels;
using UniverseLib;
using static UnityEngine.UI.Dropdown;
using GameDef;
using Newtonsoft.Json;

namespace UnityCheatMainManager.MainUI.CreateUI.FightUI
{
    public static class QuickUseMagic
    {
        private static Dropdown MagicTypeDropdown;
        private static List<GameDef.MagicType> MagicTypeList = new List<GameDef.MagicType>();
        private static GameDef.MagicType UseMagic;
        private static Array UseMagicKeyCodes;
        public static Dropdown MagicTypeDown;
        public static BepInEx.Configuration.ConfigEntry<KeyCode> UseMagicShowKey { get; set; }
        [AttributeUsage(AttributeTargets.Field)]
        private class ChineseNameAttribute : Attribute
        {
            public string ChineseName { get; }
            public ChineseNameAttribute(string chineseName) => ChineseName = chineseName;
        }

        private enum CustomMagicType
        {
            [ChineseName("无")]
            None = 0,
            [ChineseName("防御格挡")]
            DefenseParry = 1001,
            [ChineseName("能量护盾")]
            EnergyShield = 1002,
            [ChineseName("闪电闪避")]
            LightingDodge = 1003,
            [ChineseName("死灵技能")]
            NecroSkill = 1004,
            [ChineseName("电磁轨道炮")]
            ElectroRailGun = 1,
            [ChineseName("部件改造")]
            PartReform = 2,
            [ChineseName("黑洞")]
            BlackHole = 3,
            [ChineseName("闪电长矛")]
            LightingSpear = 4,
            [ChineseName("闪电攻击")]
            LightingAttack = 5,
            [ChineseName("死灵火焰风暴")]
            NecroFireStorm = 6,
            [ChineseName("蓄力闪电锤")]
            AccumulatedLightingHammer = 7,
            [ChineseName("火焰横扫")]
            FlameSweep = 8,
            [ChineseName("尘埃之翼")]
            DustWings = 9,
            [ChineseName("雷电球云")]
            ThunderBallCloud = 10,
            [ChineseName("死灵能量球")]
            NecroEnergyBall = 11,
            [ChineseName("死灵激光")]
            NecroLaser = 12,
            [ChineseName("自我爆炸")]
            SelfExplosion = 13,
            [ChineseName("先锋誓言")]
            PioneerOath = 14,
            [ChineseName("火焰附魔")]
            FlameEnchantment = 15,
            [ChineseName("闪电附魔")]
            LightningEnchantment = 16,
            [ChineseName("王者领域")]
            KingDomain = 17,
            [ChineseName("导弹弹幕")]
            MissilesBarrage = 18,
            [ChineseName("超级电磁轨道炮")]
            SuperElectroRailGun = 19
        }

        private static string ToChinese(this CustomMagicType magicType)
        {
            MemberInfo[] memberInfo = typeof(CustomMagicType).GetMember(magicType.ToString());
            if (memberInfo.Length == 0) return magicType.ToString();

            ChineseNameAttribute attribute = memberInfo[0].GetCustomAttribute<ChineseNameAttribute>();
            return attribute?.ChineseName ?? magicType.ToString();
        }

        private static Dictionary<CustomMagicType, GameDef.MagicType> _magicTypeMap;
        static QuickUseMagic()
        {
            _magicTypeMap = new Dictionary<CustomMagicType, GameDef.MagicType>();
            foreach (CustomMagicType customType in Enum.GetValues(typeof(CustomMagicType)))
            {
                if (Enum.TryParse<GameDef.MagicType>(customType.ToString(), out var gameType))
                {
                    _magicTypeMap[customType] = gameType;
                }
                else
                {
                    CheatMain.WriteLog($"警告：自定义魔法类型 {customType} 未在 GameDef.MagicType 中找到匹配项");
                }
            }
        }

        private static void MagicTypeDropdownValueChange(int value)
        {
            if (value >= 0 && value < MagicTypeList.Count)
            {
                UseMagic = MagicTypeList[value];
                var customType = _magicTypeMap.FirstOrDefault(kv => kv.Value == UseMagic).Key;
                CheatMain.WriteLog($"选中的魔法类型: {UseMagic}（中文：{customType.ToChinese()}）");
            }
        }
        public static void InputUseQuickMagic()
        {
            if (UniverseLib.Input.InputManager.GetKeyDown(UseMagicShowKey.Value))
            {
                if (UseMagic != GameDef.MagicType.None)
                {
                    MagicSystem.RunMagicAction(UseMagic);
                    var customType = _magicTypeMap.FirstOrDefault(kv => kv.Value == UseMagic).Key;
                    CheatMain.WriteLog($"已触发魔法：{customType.ToChinese()}");
                }
                else
                {
                    CheatMain.WriteLog("未选择魔法类型，请先从下拉框中选择");
                }
            }
        }
        public static void DrawUseMagicUI(GameObject tabGroup)
        {
            GameObject PlayerFightMagicInput = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowPlayerFightMagicInput",
                false, false, true, true, 5, default, new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);

            PlayerFightMagicInput.Createlabel("快速使用魔法快捷键")
                .SetLayoutElement(180, 40);

            GameObject gameObject = UIFactory.CreateDropdown(PlayerFightMagicInput, "MainTypeDown",
                out MagicTypeDown, "快捷键", CheatMain.FontSize.Value, null, null);

            Il2CppSystem.Collections.Generic.List<OptionData> optionDataList = new Il2CppSystem.Collections.Generic.List<OptionData>();
            UseMagicKeyCodes = Enum.GetValues(typeof(UnityEngine.KeyCode));
            foreach (UnityEngine.KeyCode keyCode in UseMagicKeyCodes)
            {
                optionDataList.Add(new OptionData(keyCode.ToString()));
            }
            UIFactory.SetLayoutElement(gameObject, minHeight: 40, minWidth: 200);
            MagicTypeDown.captionText.fontSize = CheatMain.FontSize.Value;

            PlayerFightMagicInput.CreateButton("设置", () =>
            {
                MagicKeyCodeChange(MagicTypeDown.value);
                }).SetLayoutElement(80, 40);
            MagicTypeDown.AddOptions(optionDataList);
            MagicTypeDown.value = Array.IndexOf(UseMagicKeyCodes, UseMagicShowKey.Value);

           SetDropdownScrollSpeed(MagicTypeDown, 10f);

            GameObject PlayerFightMagic = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowPlayerFightMagic",
                false, false, true, true, 5, default, new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);

            var lable1 = UIFactory.CreateLabel(PlayerFightMagic, "Lable", "魔法种类", fontSize: CheatMain.FontSize.Value);
            PlayerFightMagic.SetLayoutElement(minHeight: 40, flexibleWidth: 0);

            GameObject MagicObject = UIFactory.CreateDropdown(PlayerFightMagic, "MagicTypeDropdown",
                out MagicTypeDropdown, "选择魔法类型", CheatMain.FontSize.Value, null, null);

            MagicTypeList.Clear();
            MagicTypeDropdown.options.Clear();
            foreach (CustomMagicType customType in Enum.GetValues(typeof(CustomMagicType)))
            {
                if (_magicTypeMap.TryGetValue(customType, out var gameType))
                {
                    MagicTypeDropdown.options.Add(new Dropdown.OptionData(customType.ToChinese()));
                    MagicTypeList.Add(gameType);
                }
            }
            UIFactory.SetLayoutElement(MagicObject, minHeight: 40, minWidth: 180);
            MagicTypeDropdown.captionText.fontSize = CheatMain.FontSize.Value;
            MagicTypeDropdown.onValueChanged.RemoveListener(MagicTypeDropdownValueChange);
            MagicTypeDropdown.onValueChanged.AddListener(MagicTypeDropdownValueChange);

            if (MagicTypeList.Contains(UseMagic))
            {
                MagicTypeDropdown.value = MagicTypeList.IndexOf(UseMagic);
            }
            else if (MagicTypeList.Count > 0)
            {
                MagicTypeDropdown.value = 0;
                UseMagic = MagicTypeList[0];
            }
        }

        private static void MagicKeyCodeChange(int value)
        {
            if (value >= 0 && value < UseMagicKeyCodes.Length)
            {
                UseMagicShowKey.Value = (UnityEngine.KeyCode)UseMagicKeyCodes.GetValue(value);
                CheatMain.WriteLog($"已设置魔法快捷键：{UseMagicShowKey}");
            }
        }

        public static void SetDropdownScrollSpeed(Dropdown dropdown, float scrollSpeed)
        {
            if (dropdown == null)
            {
                Debug.LogError("Dropdown is null.");
                return;
            }

            Transform dropdownTemplate = dropdown.template;
            if (dropdownTemplate == null)
            {
                Debug.LogError("Dropdown template is null.");
                return;
            }

            ScrollRect scrollRect = dropdownTemplate.GetComponent<ScrollRect>();
            if (scrollRect == null)
            {
                Debug.LogError("ScrollRect component not found on dropdown template.");
                return;
            }

            scrollRect.scrollSensitivity = scrollSpeed;
        }
    }
}