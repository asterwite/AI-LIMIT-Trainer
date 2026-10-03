using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UnityCheatMainManager.Config
{
    // 配置项基础类
    public class ConfigEntry<T>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public T DefaultValue { get; set; }
        public T Value { get; set; }

        public ConfigEntry(string name, string description, T defaultValue)
        {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
            Value = defaultValue;
        }
    }

    // 配置管理器
    public static class ConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(
            Application.dataPath,
            "UnityCheatMainManager",
            "UnityCheatMainManagerConig.json"
        );

        // 配置项集合
        private static readonly Dictionary<string, object> ConfigEntries = new();

        // 公开的配置项
        public static ConfigEntry<KeyCode> ShowCounter { get; private set; }
        public static ConfigEntry<int> FontSize { get; private set; }
        public static ConfigEntry<float> StartupDelay { get; private set; }

        static ConfigManager()
        {
            // 初始化配置项
            InitializeEntries();

            // 加载配置
            Load();
        }

        private static void InitializeEntries()
        {
            ShowCounter = new ConfigEntry<KeyCode>(
                "ShowCounter",
                "呼出快捷键",
                KeyCode.Insert
            );

            FontSize = new ConfigEntry<int>(
                "FontSize",
                "菜单字体大小",
                17
            );

            StartupDelay = new ConfigEntry<float>(
                "StartupDelay",
                "启动延迟时间",
                1.0f
            );

            // 添加到配置集合
            ConfigEntries.Add(ShowCounter.Name, ShowCounter);
            ConfigEntries.Add(FontSize.Name, FontSize);
            ConfigEntries.Add(StartupDelay.Name, StartupDelay);
        }

        // 保存配置到文件（Newtonsoft.Json实现）
        public static void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Newtonsoft 序列化设置（支持枚举字符串化、缩进格式）
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented, // 格式化输出（美观）
                    Converters = { new StringEnumConverter() }, // 枚举以字符串形式保存
                    TypeNameHandling = TypeNameHandling.Auto // 自动处理类型信息（复杂类型需要）
                };

                // 序列化配置项字典
                string json = JsonConvert.SerializeObject(ConfigEntries, settings);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"保存配置失败: {ex.Message}");
            }
        }

        // 从文件加载配置（Newtonsoft.Json实现）
        public static void Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                    return;

                string json = File.ReadAllText(ConfigPath);

                // Newtonsoft 反序列化设置（与序列化对应）
                var settings = new JsonSerializerSettings
                {
                    Converters = { new StringEnumConverter() },
                    TypeNameHandling = TypeNameHandling.Auto
                };

                // 反序列化为字典
                var loadedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, settings);

                foreach (var entry in loadedData)
                {
                    if (ConfigEntries.TryGetValue(entry.Key, out var configEntry))
                    {
                        // 使用Newtonsoft的Convert方法转换类型
                        var targetType = configEntry.GetType().GetProperty("Value").PropertyType;
                        var value = JsonConvert.DeserializeObject(
                            JsonConvert.SerializeObject(entry.Value),
                            targetType
                        );
                        configEntry.GetType().GetProperty("Value").SetValue(configEntry, value);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"加载配置失败: {ex.Message}");
            }
        }

        // 还原默认配置
        public static void RestoreDefaults()
        {
            ShowCounter.Value = ShowCounter.DefaultValue;
            FontSize.Value = FontSize.DefaultValue;
            StartupDelay.Value = StartupDelay.DefaultValue;
            Save(); // 立即保存默认值
        }
    }
}