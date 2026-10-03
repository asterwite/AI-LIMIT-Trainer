using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityCheatMainManager.MainUI.CreateUI.StaminaUI.DrawStaminaNeed;
using static UnityCheatMainManager.MainUI.CreateUI.StaminaUI.StaminaUI;

namespace UnityCheatMainManager.MainUI.CreateUI.StaminaUI
{
    public class DrawStaminaNeed
    {
        public enum StaminaState
        {
            Normal,
            Consuming,
            Restoring
        }

        public static void DrawStaminaBar(float currentStamina, float maxStamina, float maxWidth, float height, Color staminaColor, Color backgroundColor, float screenBottomPadding = 20f)
        {
            // 计算体力百分比，确保在有效范围内
            float healthPercentage = Mathf.Clamp01(currentStamina / maxStamina);

            // 计算体力条在屏幕底部中间的位置
            float x = (Screen.width - maxWidth) / 2;
            float y = Screen.height - height - screenBottomPadding;
            Rect barRect = new Rect(x, y, maxWidth, height);

            // 保存当前GUI矩阵状态
            Matrix4x4 matrixBackup = GUI.matrix;

            // 计算当前体力条的宽度
            float currentWidth = maxWidth * healthPercentage;
            Rect currentBarRect = new Rect(barRect.x, barRect.y, currentWidth, height);

            // 绘制背景（已消耗部分）
            GUI.color = backgroundColor;
            GUI.DrawTexture(barRect, Texture2D.whiteTexture);

            // 绘制当前体力（未消耗部分）
            GUI.color = staminaColor;
            GUI.DrawTexture(currentBarRect, Texture2D.whiteTexture);

            // 恢复GUI矩阵状态
            GUI.matrix = matrixBackup;
        }
        public static void ConsumeStamina(float amount)
        {
            if (模拟体力条)
            {
                CurrentStamina -= amount;
                if (CurrentStamina < 0)
                    CurrentStamina = 0;
                State = StaminaState.Consuming;
                RestoreTimer = 0f;
            }
        }
        public static void UpdataStamina()
        {
            switch (State)
            {
                case StaminaState.Consuming:
                   RestoreTimer += Time.deltaTime;
                    if (RestoreTimer >= RestoreDelay)
                    {
                        State = StaminaState.Restoring;
                    }
                    break;
                case StaminaState.Restoring:
                    CurrentStamina += RestoreRate;
                    if (CurrentStamina >= MaxStamina)
                    {
                        CurrentStamina = MaxStamina;
                        State = StaminaState.Normal;
                    }
                    break;
            }
           CheatMain.CurrentStamina.Value = CurrentStamina;
           CheatMain.MaxStamina.Value = MaxStamina;
           CheatMain.RestoreDelay.Value = RestoreDelay;
           CheatMain.RestoreRate.Value = RestoreRate;

        }

        public static Stamina GetNowStamina()
        {
            return new Stamina() { Current = CurrentStamina, Max = MaxStamina, RestoreRate = RestoreRate, RestoreDelay = RestoreDelay };
        }
    }
}
