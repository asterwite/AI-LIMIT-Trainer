using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UnityCheatMainManager.SaveAndLoad
{
    class SaveAndLoadNeed
    {
        private readonly byte[] Key = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes for AES-128
        private readonly byte[] IV = Encoding.UTF8.GetBytes("esacjk1254wopsxc");  // 16 bytes for AES
        private readonly string additionalText = "这么简单你都要破？";

        public bool SaveDictionaryToFile<T>(Dictionary<int, T> Dictionary, string SavePath, string SaveName)
        {
            string directory = Path.GetDirectoryName(SavePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            try
            {
                if (Dictionary.Count == 0)
                {
                    CheatMain.WriteLog(SaveName + " 字典为空，无法保存数据。");
                    return false;
                }

                string data = JsonConvert.SerializeObject(Dictionary);
                data += "\n" + additionalText; // 在数据后面添加额外文字
                byte[] encryptedData = EncryptStringToBytes_Aes(data, Key, IV);

                using (FileStream fs = new FileStream(SavePath, FileMode.Create))
                {
                    fs.Write(encryptedData, 0, encryptedData.Length);
                }
                CheatMain.WriteLog(SaveName + " 数据已保存到 BepInEx\\plugins\\TheSave文件夹内。");
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"保存数据时出错: {ex.Message}");
                return false;
            }
        }

        // 原有的字典读取方法
        public Dictionary<int, T> ReadDictionaryFromFile<T>(string SavePath)
        {
            try
            {
                if (!File.Exists(SavePath))
                {
                    CheatMain.WriteLog("文件不存在，无法读取数据。");
                    return null;
                }

                byte[] encryptedData = File.ReadAllBytes(SavePath);
                string decryptedData = DecryptStringFromBytes_Aes(encryptedData, Key, IV);

                // 分割数据，忽略额外文字
                string[] lines = decryptedData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 0)
                {
                    string jsonData = lines[0];
                    Dictionary<int, T> result = JsonConvert.DeserializeObject<Dictionary<int, T>>(jsonData);

                    //Console.WriteLine("AllFriend 数据已从文件中读取并解密。");
                    return result;
                }
                else
                {
                    CheatMain.WriteLog("未找到有效数据。");
                    return null;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"读取并解密数据时出错: {ex.Message}");
                return null;
            }
        }
        public bool SaveListToFile<T>(List<T> list, string savePath, string saveName)
        {
            string directory = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            try
            {
                if (list.Count == 0)
                {
                    CheatMain.WriteLog(saveName + " 列表为空，无法保存数据。");
                    return false;
                }

                string data = JsonConvert.SerializeObject(list);
                data += "\n" + additionalText; // 在数据后面添加额外文字
                byte[] encryptedData = EncryptStringToBytes_Aes(data, Key, IV);

                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    fs.Write(encryptedData, 0, encryptedData.Length);
                }
                CheatMain.WriteLog(saveName + " 列表数据已保存到 " + savePath);
                return true;
            }
            catch (Exception ex)
            {
                CheatMain.WriteLog($"保存列表数据时出错: {ex.Message}");
                return false;
            }
        }
        public List<T> ReadListFromFile<T>(string savePath)
        {
            try
            {
                if (!File.Exists(savePath))
                {
                    CheatMain.WriteLog("文件不存在，无法读取列表数据。");
                    return null;
                }

                byte[] encryptedData = File.ReadAllBytes(savePath);
                string decryptedData = DecryptStringFromBytes_Aes(encryptedData, Key, IV);

                // 分割数据，忽略额外文字
                string[] lines = decryptedData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 0)
                {
                    string jsonData = lines[0];
                    List<T> result = JsonConvert.DeserializeObject<List<T>>(jsonData);

                    CheatMain.WriteLog("列表数据已从文件中读取并解密。");
                    return result;
                }
                else
                {
                    CheatMain.WriteLog("未找到有效列表数据。");
                    return null;
                }
            }
            catch (Exception ex)
            {
                CheatMain.WriteLog($"读取并解密列表数据时出错: {ex.Message}");
                return null;
            }
        }
        public bool SaveClassToFile<T>(T classInstance, string savePath, string saveName) where T : class
        {
            string directory = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            try
            {
                if (classInstance == null)
                {
                    CheatMain.WriteLog(saveName + " 类实例为空，无法保存数据。");
                    return false;
                }

                string data = JsonConvert.SerializeObject(classInstance);
                data += "\n" + additionalText; // 在数据后面添加额外文字
                byte[] encryptedData = EncryptStringToBytes_Aes(data, Key, IV);

                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    fs.Write(encryptedData, 0, encryptedData.Length);
                }
                CheatMain.WriteLog(saveName + " 类数据已保存到 " + savePath);
                return true;
            }
            catch (Exception ex)
            {
                CheatMain.WriteLog($"保存类数据时出错: {ex.Message}");
                return false;
            }
        }
        public T ReadClassFromFile<T>(string savePath) where T : class
        {
            try
            {
                if (!File.Exists(savePath))
                {
                    CheatMain.WriteLog("文件不存在，无法读取类数据。");
                    return null;
                }

                byte[] encryptedData = File.ReadAllBytes(savePath);
                string decryptedData = DecryptStringFromBytes_Aes(encryptedData, Key, IV);

                // 分割数据，忽略额外文字
                string[] lines = decryptedData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 0)
                {
                    string jsonData = lines[0];
                    T result = JsonConvert.DeserializeObject<T>(jsonData);

                    CheatMain.WriteLog("类数据已从文件中读取并解密。");
                    return result;
                }
                else
                {
                    CheatMain.WriteLog("未找到有效类数据。");
                    return null;
                }
            }
            catch (Exception ex)
            {
                CheatMain.WriteLog($"读取并解密类数据时出错: {ex.Message}");
                return null;
            }
        }
    //加密方法
        private byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        // 解密方法
        private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}