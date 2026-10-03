using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UniverseLib.UI;
using UnityCheatMainManager.UI;
using UnityEngine.UI;
using UnityCheatMainManager.ESP.CreateESPUI;

namespace UnityCheatMainManager.MainUI
{
    internal class BasicUI
    {
        public static bool ShowDev = false;
        struct BasicCheat
        {
            public bool 启动;
            public bool 一次;
            public BasicCheat(bool 启动, bool 一次)
            {
                this.启动 = 启动;
                this.一次 = 一次;
            }
        }

        private static float 移动速度倍数 = 1f;
        private static BasicCheat 移动速度倍数启动 = new BasicCheat(false, false);
        private static CreateUI.FightWindow OldFightWindow;
        private static CreatePortableShop.PortableShopWindow OldPortableShopWindow;
        public static void DrawBasicUICheat()
        {
            if (移动速度倍数启动.启动)
            {
                ProcedureMonoSingleton<CameraManager>.Instance._Owner._Define.Speed = 6f * 移动速度倍数;
                移动速度倍数启动.一次 = true;
            }
            if (!移动速度倍数启动.启动 && 移动速度倍数启动.一次)
            {
                ProcedureMonoSingleton<CameraManager>.Instance._Owner._Define.Speed = 6f;
                移动速度倍数启动.一次 = false;
            }

        }
        public static void DrawBasicUI(GameObject tabGroup, UIBase Owner)
        {
            GameObject DevShow = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowDevShow", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            DevShow.CreateToggle(500, "按F1显示游戏自带内置修改UI，再次按F1关闭或者鼠标右键退出一步", (state) => { ShowDev = state; });

            MainUI.CreateUI.StaminaUI.StaminaUI.DrawPlayerStaminaUI(tabGroup);

            GameObject Player = UIFactory.CreateHorizontalGroup(tabGroup, "MainWindowPlayer", false, false, true, true, 5, default,
              new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            Player.CreateInputEditToggle(150, "移动速度倍数", "2", (string text, bool state) =>
            {
                移动速度倍数 = text.ConvertToFloatDef(2);
                移动速度倍数启动.启动 = state;
            });
            tabGroup.CreateButton("打开战斗窗口", () =>
            {
                if (OldFightWindow == null)
                {
                    OldFightWindow = new CreateUI.FightWindow(Owner);
                }
                OldFightWindow.SetActive(true); 
            }).SetLayoutElement(300, 40);

            //tabGroup.CreateButton("打开随身商店窗口", () =>
            //{
            //    if (OldPortableShopWindow == null)
            //    {
            //        OldPortableShopWindow = new CreatePortableShop.PortableShopWindow(Owner);
            //    }
            //    OldPortableShopWindow.SetActive(true); 
            //}).SetLayoutElement(300, 40);
            
        }


    }
}
