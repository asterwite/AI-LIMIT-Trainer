using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityCheatMainManager.MainUI.CreateUI.HungerUI.HungerUI;

namespace UnityCheatMainManager.MainUI.CreateUI.HungerUI
{
    internal class DrawHungerNeed
    {
        public enum HungerState
        {
            Normal,
            Consuming,
            Restoring
        }
        public static HungerState State = HungerState.Normal;
        private static float RestoreTimer = 0f;
        public static float RestoreRate = 0.01f;
        public static float RestoreDelay = 4f;
        private static bool HaveEat = false;
        private static float UseItemCanAddHungerCount = 10f;

        public static void DrawHungerBar(float CurrentHunger, float MaxHunger, float maxWidth, float height, Color HungerColor, Color backgroundColor, float screenBottomPadding = 80f)
        {
            // 计算饥饿百分比，确保在有效范围内
            float HungerPercentage = Mathf.Clamp01(CurrentHunger / MaxHunger);

            // 计算饥饿条在屏幕底部中间的位置
            float x = (Screen.width - maxWidth) / 2;
            float y = Screen.height - height - screenBottomPadding;
            Rect barRect = new Rect(x, y, maxWidth, height);

            // 保存当前GUI矩阵状态
            Matrix4x4 matrixBackup = GUI.matrix;

            // 计算当前饥饿条的宽度
            float currentWidth = maxWidth * HungerPercentage;
            Rect currentBarRect = new Rect(barRect.x, barRect.y, currentWidth, height);

            // 绘制背景（已消耗部分）
            GUI.color = backgroundColor;
            GUI.DrawTexture(barRect, Texture2D.whiteTexture);

            // 绘制当前饥饿（未消耗部分）
            GUI.color = HungerColor;
            GUI.DrawTexture(currentBarRect, Texture2D.whiteTexture);

            // 恢复GUI矩阵状态
            GUI.matrix = matrixBackup;
        }

        public static void UpdataHunger()
        {
            switch (State)
            {
                case HungerState.Consuming:
                    if (HaveEat)
                        State = HungerState.Restoring;
                    break;
                case HungerState.Restoring:
                    CurrentHunger += RestoreRate;
                    RestoreTimer += Time.deltaTime;
                    if (RestoreTimer > UseItemCanAddHungerCount)
                        State = HungerState.Consuming;
                    if (CurrentHunger >= MaxHunger)
                    {
                        CurrentHunger = MaxHunger;
                        State = HungerState.Normal;
                    }
                    break;
            }
        }
    }
}
