using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace UnityCheatMainManager.Sponsor
{
    internal class ImageOpen
    {
        public static void OpenWeiXingEmbeddedJpgInBrowser(string resourceName)
        {
            try
            {
                // 获取当前 DLL 的程序集
                Assembly assembly = Assembly.GetExecutingAssembly();

                // 读取嵌入资源流
                using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                    {
                        throw new ArgumentException($"未找到嵌入资源: {resourceName}");
                    }

                    // 创建临时文件
                    string tempFilePath = Path.Combine(
                        Path.GetTempPath(),
                        $"temp_{Guid.NewGuid():N}.jpg" // 生成唯一文件名
                    );

                    // 将资源流写入临时文件
                    using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        resourceStream.CopyTo(fileStream);
                    }

                    // 在默认浏览器中打开文件
                    Process.Start(new ProcessStartInfo(tempFilePath)
                    {
                        UseShellExecute = true, // 关键：使用系统外壳执行，自动关联默认程序
                        Verb = "open"           // 指定操作类型为"打开"
                    });

                    Console.WriteLine($"已打开微信赞助图片: {tempFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                throw; // 可选：将异常抛给调用者处理
            }
        }
        public static void OpenZhiFuBaoEmbeddedJpgInBrowser(string resourceName)
        {
            try
            {
                // 获取当前 DLL 的程序集
                Assembly assembly = Assembly.GetExecutingAssembly();

                // 读取嵌入资源流
                using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                    {
                        throw new ArgumentException($"未找到嵌入资源: {resourceName}");
                    }

                    // 创建临时文件
                    string tempFilePath = Path.Combine(
                        Path.GetTempPath(),
                        $"temp_{Guid.NewGuid():N}.jpg" // 生成唯一文件名
                    );

                    // 将资源流写入临时文件
                    using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        resourceStream.CopyTo(fileStream);
                    }

                    // 在默认浏览器中打开文件
                    Process.Start(new ProcessStartInfo(tempFilePath)
                    {
                        UseShellExecute = true, // 关键：使用系统外壳执行，自动关联默认程序
                        Verb = "open"           // 指定操作类型为"打开"
                    });

                    Console.WriteLine($"已打开支付宝赞助图片: {tempFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                throw; // 可选：将异常抛给调用者处理
            }
        }
        public static string[] GetAllResourceNames()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }
    }

    public static class ImageOpenInBrowser
    {
        /// <summary>
        /// 从当前 DLL 中提取嵌入资源并保存到临时文件
        /// </summary>
        /// <param name="resourceFullName">嵌入资源的完整名称（命名空间.文件夹.文件名）</param>
        /// <returns>临时文件路径，失败时返回 null</returns>
        public static string ExtractToTempFile(string resourceFullName)
        {
            try
            {
                // 获取当前 DLL 的程序集
                Assembly assembly = Assembly.GetExecutingAssembly();

                // 读取嵌入资源流
                using (Stream resourceStream = assembly.GetManifestResourceStream(resourceFullName))
                {
                    if (resourceStream == null)
                    {
                        Console.WriteLine($"错误：未找到资源 {resourceFullName}");
                        return null;
                    }

                    // 创建临时文件
                    string tempFilePath = Path.Combine(
                        Path.GetTempPath(),
                        $"temp_{Guid.NewGuid():N}{Path.GetExtension(resourceFullName)}"
                    );

                    // 将资源流写入临时文件
                    using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        resourceStream.CopyTo(fileStream);
                    }

                    return tempFilePath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"提取资源时出错：{ex.Message}");
                return null;
            }
        }
    }

    public static class BrowserOpener
    {
        public static void OpenWeiXingEmbeddedJpgInBrowser(string resourceFullName)
        {
            try
            {
                // 1. 提取嵌入资源到临时文件
                string tempFilePath = ExtractResourceToTempFile(resourceFullName);
                if (string.IsNullOrEmpty(tempFilePath))
                {
                    throw new Exception("无法提取嵌入资源");
                }

                // 2. 获取可用的浏览器路径
                string browserPath = GetInstalledBrowserPath();
                if (string.IsNullOrEmpty(browserPath))
                {
                    throw new Exception("未找到可用的浏览器");
                }

                // 3. 使用浏览器打开临时文件
                OpenFileWithBrowser(browserPath, tempFilePath);

                CheatMain.WriteLog($"已在浏览器中打开微信赞助图片: {tempFilePath}");
            }
            catch (Exception ex)
            {
                CheatMain.WriteLog($"错误: {ex.Message}");
                throw;
            }
        }

        public static void OpenZhiFuBaoEmbeddedJpgInBrowser(string resourceFullName)
        {
            try
            {
                // 1. 提取嵌入资源到临时文件
                string tempFilePath = ExtractResourceToTempFile(resourceFullName);
                if (string.IsNullOrEmpty(tempFilePath))
                {
                    throw new Exception("无法提取嵌入资源");
                }

                // 2. 获取可用的浏览器路径
                string browserPath = GetInstalledBrowserPath();
                if (string.IsNullOrEmpty(browserPath))
                {
                    throw new Exception("未找到可用的浏览器");
                }

                // 3. 使用浏览器打开临时文件
                OpenFileWithBrowser(browserPath, tempFilePath);

                CheatMain.WriteLog($"已在浏览器中打开支付宝赞助图片: {tempFilePath}");
            }
            catch (Exception ex)
            {
                CheatMain.WriteLog($"错误: {ex.Message}");
                throw;
            }
        }

        private static string ExtractResourceToTempFile(string resourceFullName)
        {
            // 获取当前DLL程序集
            Assembly assembly = Assembly.GetExecutingAssembly();

            // 读取嵌入资源流
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceFullName))
            {
                if (resourceStream == null)
                {
                    throw new ArgumentException($"未找到嵌入资源: {resourceFullName}");
                }

                // 创建临时文件
                string tempFilePath = Path.Combine(
                    Path.GetTempPath(),
                    $"img_{Guid.NewGuid():N}.jpg"
                );

                // 将资源写入临时文件
                using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    resourceStream.CopyTo(fileStream);
                }

                return tempFilePath;
            }
        }

        private static string GetInstalledBrowserPath()
        {
            // 按优先级检查常见浏览器
            string[] browserCandidates = new[]
            {
            // Chrome
            @"C:\Program Files\Google\Chrome\Application\chrome.exe",
            @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
            
            // Edge
            @"C:\Program Files\Microsoft\Edge\Application\msedge.exe",
            @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
            
            // Firefox
            @"C:\Program Files\Mozilla Firefox\firefox.exe",
            @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe"
        };

            foreach (string path in browserCandidates)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            // 尝试从PATH环境变量查找
            return FindBrowserInPath();
        }

        private static string FindBrowserInPath()
        {
            string[] browsers = { "chrome.exe", "msedge.exe", "firefox.exe" };
            string pathEnv = Environment.GetEnvironmentVariable("PATH") ?? "";

            foreach (string directory in pathEnv.Split(Path.PathSeparator))
            {
                foreach (string browser in browsers)
                {
                    string fullPath = Path.Combine(directory.Trim(), browser);
                    if (File.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }
            }

            return null;
        }

        private static void OpenFileWithBrowser(string browserPath, string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = browserPath,
                    Arguments = $"\"{filePath}\"", // 确保路径被引号包裹，处理空格
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"使用浏览器打开文件失败: {ex.Message}");
                throw;
            }
        }
    }
}
