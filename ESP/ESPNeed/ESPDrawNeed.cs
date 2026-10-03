using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCheatMainManager.ESP.ESPNeed
{
    public class ESPDrawNeed 
    {
        public static GUIStyle StringStyle { get; set; } = GUI.skin.label;
        public static bool DrawString(Vector2 position, string label, Color color, int fontSize, bool Centered = false)
        {
            GUI.color = color;
            StringStyle.fontSize = fontSize;
            StringStyle.normal.textColor = color;
            StringStyle.alignment = TextAnchor.UpperLeft;
            var Guicontent = new GUIContent(label);
            if (Centered)
            {
                var vector = StringStyle.CalcSize(Guicontent);
                GUI.Label(new Rect(position - vector / 2f, vector), Guicontent, StringStyle);
            }
            else
            {
                // 不居中时，直接从指定点开始绘制
                GUI.Label(new Rect(position, StringStyle.CalcSize(Guicontent)), Guicontent, StringStyle);
            }

            return true;
        }
     public static void DrawLine(Vector2 start, Vector2 end, Color color, float width)
        {
            Matrix4x4 matrix = GUI.matrix;
            try
            {
                GUI.depth = 0;
                Vector2 direction = end - start;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float length = direction.magnitude;
                float halfWidth = width / 2f;      
                GUIUtility.RotateAroundPivot(angle, start);
                GUI.color = color;
                GUI.DrawTexture(
                    new Rect(start.x, start.y - halfWidth, length, width), 
                    Texture2D.whiteTexture, 
                    ScaleMode.StretchToFill
                );
            }
            finally
            {
                GUI.matrix = matrix;
            }
        }
        public static void DrawHealthBar(Vector2 screenHeadPos, Vector2 screenRightFootPos, Vector2 screenLeftFootPos, float currentHealth, float maxHealth, float width = 10f, Color? fullColor = null, Color? emptyColor = null)
        {
            Color fullHealthColor = fullColor ?? Color.green;
            Color emptyHealthColor = emptyColor ?? Color.red;

            float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);

            // 计算血条高度（屏幕坐标Y轴向下，所以取绝对值）
            float rawHeight = Mathf.Abs(screenRightFootPos.y - screenHeadPos.y);
            float height = Mathf.Max(rawHeight, 2f); // 确保高度不小于2个单位

            // 计算角色朝向（通过比较左右脚的X坐标）
            bool isFacingRight = screenRightFootPos.x > screenLeftFootPos.x;

            // 计算血条位置
            float centerX;
            if (isFacingRight)
            {
                // 角色面向右侧，血条在左脚的左侧（屏幕上的左侧）
                centerX = screenLeftFootPos.x - 20f - width / 2f;
            }
            else
            {
                // 角色面向左侧，血条在右脚的左侧（屏幕上的右侧）
                centerX = screenRightFootPos.x - 20f - width / 2f;
            }

            // 计算血条底部Y坐标，稍微向上偏移一点
            float bottomY = Mathf.Min(screenHeadPos.y, screenRightFootPos.y);

            // 计算血条左右边界
            float left = centerX - width / 2f;

            // 计算绿色和红色部分的高度
            float greenHeight = height * healthPercent;
            float redHeight = height - greenHeight;

            // 绘制红色背景部分
            Rect redRect = new Rect(left, bottomY, width, redHeight);
            GUI.color = emptyHealthColor;
            GUI.DrawTexture(redRect, Texture2D.whiteTexture);

            // 绘制绿色健康部分
            Rect greenRect = new Rect(left, bottomY + redHeight, width, greenHeight);
            GUI.color = fullHealthColor;
            GUI.DrawTexture(greenRect, Texture2D.whiteTexture);

            // 恢复默认颜色
            GUI.color = Color.white;
        }

        public static void Draw2DBox(Vector3 headPos, Vector3 leftFootPos, Vector3 rightFootPos, Color color)
        {
            // 1. 计算中心点（基于三个点的平均位置）
            Vector3 center = (headPos + leftFootPos + rightFootPos) / 3f;

            // 2. 找出所有点的最小和最大坐标
            float minX = Mathf.Min(headPos.x, leftFootPos.x, rightFootPos.x);
            float maxX = Mathf.Max(headPos.x, leftFootPos.x, rightFootPos.x);
            float minY = Mathf.Min(headPos.y, leftFootPos.y, rightFootPos.y);
            float maxY = Mathf.Max(headPos.y, leftFootPos.y, rightFootPos.y);

            // 3. 计算四个角点
            Vector2 topLeft = new Vector2(minX, maxY);
            Vector2 topRight = new Vector2(maxX, maxY);
            Vector2 bottomLeft = new Vector2(minX, minY);
            Vector2 bottomRight = new Vector2(maxX, minY);

            // 4. 绘制四条边
            DrawLine(topLeft, topRight, color, 2f);           // 上边
            DrawLine(topRight, bottomRight, color, 2f);       // 右边
            DrawLine(bottomRight, bottomLeft, color, 2f);     // 下边
            DrawLine(bottomLeft, topLeft, color, 2f);         // 左边
        }


        public static void DrawItemSprie(Sprite sprite, Vector2 screenPos)
        {
            // 基础空值检查
            if (sprite == null || sprite.texture == null)
            {
                Debug.LogError(sprite == null ? "Sprite为null" : $"Sprite {sprite.name} 的Texture为null");
                return;
            }

            Vector2 imageSize = new Vector2(40, 40);
            Color backgroundColor = new Color(1, 1, 1, 0.9f);

            // 计算背景和图片的矩形（以screenPos为中心）
            Rect backgroundRect = new Rect(
                screenPos.x ,
                screenPos.y ,
                imageSize.x + 2,
                imageSize.y + 2
            );
            Rect drawRect = new Rect(
                screenPos.x ,
                screenPos.y ,
                imageSize.x,
                imageSize.y
            );

            GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height)); 
            Color originalColor = GUI.color; 

            try
            {
                
                GUI.color = backgroundColor;
                 GUI.DrawTexture(backgroundRect, Texture2D.whiteTexture);
                GUI.color = Color.white;
                GUI.DrawTexture(drawRect, sprite.texture);
            }
            finally
            {
                // 2. 恢复全局GUI状态（无论是否报错，都确保恢复）
                GUI.color = originalColor;
                GUI.EndGroup(); // 结束当前绘制组，完全隔离状态
            }
        }
    }
}
