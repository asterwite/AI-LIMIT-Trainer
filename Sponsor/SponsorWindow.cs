using System.IO;
using System.Reflection;
using UnityCheatMainManager.Spawn;
using UnityCheatMainManager.UI.Panels;
using UnityCheatMainManager.UI;
using UnityEngine.UI;
using UnityEngine;
using UniverseLib.UI;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UnityCheatMainManager.Sponsor
{
    public class UrlOpener
    {
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int ShellExecute(
            IntPtr hwnd,
            string lpOperation,
            string lpFile,
            string lpParameters,
            string lpDirectory,
            int nShowCmd
        );

        public static void OpenUrl(string url)
        {
            // 调用系统外壳打开 URL，1 表示正常显示窗口
            int result = ShellExecute(IntPtr.Zero, "open", url, null, null, 1);
            if (result <= 32)
            {
                throw new Exception($"打开 URL 失败，错误码：{result}");
            }
        }
    }
    internal class SponsorWindow : CheatMainPanel
    {
        public static SponsorWindow Instance { get; private set; }
        public override ManagerUI.Panels PanelType => ManagerUI.Panels.SponsorWindow;
        public override string Name => "其他";
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

        public SponsorWindow(UIBase owner) : base(owner)
        {
            Instance = this;
        }
        public override void Update()
        {
            base.Update();
        }
        protected override void ConstructPanelContent()
        {

            GameObject tabGroup = UIFactory.CreateVerticalGroup(ContentRoot, "SponsorWindowVerticalGroup", true, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
            UIFactory.SetLayoutElement(tabGroup, minHeight: 40, flexibleHeight: 0);

            GameObject SponsorWeiXingUI = UIFactory.CreateHorizontalGroup(tabGroup, "SponsorWindowWeiXingUI", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);

            SponsorWeiXingUI.CreateButton("赞赏微信（看情况，当然有最好，如果可以，每个人五毛或者几块就行了，炎炎夏日必须来根小布丁）", () =>
            {
                BrowserOpener.OpenWeiXingEmbeddedJpgInBrowser("UnityCheatMainManager.Resources.WeiXing.jpg");
            }).SetLayoutElement(800, 80);

            GameObject SponsorZhiFuBaoUI = UIFactory.CreateHorizontalGroup(tabGroup, "SponsorWindowZhiFuBaoUI", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);

            SponsorZhiFuBaoUI.CreateButton("赞赏支付宝（看情况，当然有最好，如果可以，每个人五毛或者几块就行了，炎炎夏日必须来根小布丁）", () =>
            {
                BrowserOpener.OpenZhiFuBaoEmbeddedJpgInBrowser("UnityCheatMainManager.Resources.ZhiFuBao.jpg");
            }).SetLayoutElement(520,80);

            GameObject QQUI = UIFactory.CreateHorizontalGroup(tabGroup, "QQUI", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            QQUI.CreateButton("加入Q群（欢迎各位，反馈问题或者聊天，当然还是聊天好 ε=(´ο｀*)))", () =>
            {
                UrlOpener.OpenUrl("http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=9hHQ9Fb_anWacjEiXjRNhONKiiNKwqy0&authKey=RTtEop2qO7p5t96AlO6n1%2BLOKWRYWd47%2BUTXBPHlF1NA%2B56fIwVKU3k7x6kpfevU&noverify=0&group_code=963707322");
            }).SetLayoutElement(500, 80);

            // default active state: Active
            this.SetActive(true);
        }

    }
}
