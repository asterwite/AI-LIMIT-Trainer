using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityCheatMainManager
{
    public class WindowsMessageBoxPlugin
    {
        public const string PluginGuid = "com.example.windowsmessagebox";
        public const string PluginName = "Windows MessageBox";
        public const string PluginVersion = "1.0.0";

        // Windows MessageBox API 声明
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        // MessageBox 按钮选项
        [Flags]
        // MessageBox 按钮和图标选项
        public enum MessageBoxOptions : uint
        {
            // 按钮配置
            OK = 0x00000000,
            OKCancel = 0x00000001,
            AbortRetryIgnore = 0x00000002,
            YesNoCancel = 0x00000003,
            YesNo = 0x00000004,
            RetryCancel = 0x00000005,
            CancelTryContinue = 0x00000006,

            // 图标选项 (添加中文注释说明)
            /// <summary>
            /// 显示错误图标 (红色X)
            /// </summary>
            Error = 0x00000010,

            /// <summary>
            /// 显示问号图标 (已过时，不推荐使用)
            /// </summary>
            [Obsolete("Question图标已过时，建议使用Error、Warning或Information")]
            Question = 0x00000020,

            /// <summary>
            /// 显示警告图标 (黄色!)
            /// </summary>
            Warning = 0x00000030,

            /// <summary>
            /// 显示信息图标 (蓝色i)
            /// </summary>
            Information = 0x00000040,

            // 默认按钮选项
            DefaultButton1 = 0x00000000,
            DefaultButton2 = 0x00000100,
            DefaultButton3 = 0x00000200,
            DefaultButton4 = 0x00000300,

            // 窗口样式选项
            TopMost = 0x00040000
        }

        // MessageBox 返回值
        public enum MessageBoxResult
        {
            OK = 1,
            Cancel = 2,
            Abort = 3,
            Retry = 4,
            Ignore = 5,
            Yes = 6,
            No = 7,
            TryAgain = 10,
            Continue = 11
        }

        // 同步显示MessageBox（阻塞当前线程）
        public static MessageBoxResult Show(string text, string caption = "Message",
            MessageBoxOptions options = MessageBoxOptions.OK)
        {
            try
            {
                // 获取当前Unity窗口句柄
                IntPtr hWnd = GetUnityWindowHandle();

                // 调用Windows MessageBox，添加TopMost标志
                int result = MessageBox(hWnd, text, caption, (uint)options);

                return (MessageBoxResult)result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to show MessageBox: {ex}");

                // 失败时返回默认值
                return MessageBoxResult.Cancel;
            }
        }

        // 异步显示MessageBox（不阻塞主线程）
        public static void ShowAsync(string text, string caption = "Message",
            MessageBoxOptions options = MessageBoxOptions.OK, Action<MessageBoxResult> callback = null)
        {
            // 在单独线程中调用MessageBox，避免阻塞Unity主线程
            System.Threading.ThreadPool.QueueUserWorkItem(_ => {
                MessageBoxResult result = Show(text, caption, options);

                // 将回调发送回Unity主线程执行
                if (callback != null)
                {
                    UnityMainThreadDispatcher.Enqueue(() => callback(result));
                }
            });
        }

        // 获取Unity窗口句柄的方法
        private static IntPtr GetUnityWindowHandle()
        {
            // 在Windows平台上，可以通过UnityPlayer.dll获取窗口句柄
            // 注意：这种方法可能在不同Unity版本中有所不同

#if UNITY_STANDALONE_WIN
            try
            {
                // 使用反射获取Unity窗口句柄
                Type type = Type.GetType("UnityEngine.WindowsStandaloneWindow, UnityEngine");
                if (type != null)
                {
                    MethodInfo method = type.GetMethod("GetNativeWindowHandle", 
                        BindingFlags.Static | BindingFlags.Public);
                    if (method != null)
                    {
                        return (IntPtr)method.Invoke(null, null);
                    }
                }
                
                // 备选方法：使用User32.dll查找窗口
                IntPtr hwnd = FindWindow(null, Application.productName);
                if (hwnd != IntPtr.Zero)
                    return hwnd;
                
                // 如果找不到，使用NULL作为父窗口
                return IntPtr.Zero;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get window handle: {ex}");
                return IntPtr.Zero;
            }
#else
            // 非Windows平台返回IntPtr.Zero
            return IntPtr.Zero;
#endif
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }

    // 用于在Unity主线程执行代码的辅助类
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static UnityMainThreadDispatcher instance;
        private System.Collections.Generic.Queue<System.Action> executionQueue = new System.Collections.Generic.Queue<System.Action>();

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static void Enqueue(System.Action action)
        {
            if (instance == null)
            {
                GameObject dispatcherObject = new GameObject("UnityMainThreadDispatcher");
                instance = dispatcherObject.AddComponent<UnityMainThreadDispatcher>();
            }

            lock (instance.executionQueue)
            {
                instance.executionQueue.Enqueue(action);
            }
        }

        private void Update()
        {
            lock (executionQueue)
            {
                while (executionQueue.Count > 0)
                {
                    executionQueue.Dequeue().Invoke();
                }
            }
        }
    }
}