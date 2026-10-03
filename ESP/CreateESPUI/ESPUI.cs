
using LevelEdit;
using LevelEdit.EditorModeRuntime;
using SceneSettingDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCheatMainManager.ESP.ESPNeed;
using UnityCheatMainManager.UI;
using UnityEngine;
using UnityEngine.Playables;
using UniverseLib.UI;
using static ActorDefine;

namespace UnityCheatMainManager.ESP.CreateESPUI
{
    internal class ESPUI
    {
        public static bool 透视包含NPC = false;
        public static bool 骨骼绘制 = false;
        public static bool 名称显示 = false;
        public static bool ESP2D = false;
        public static bool 线条显示 = false;
        public static bool 血条显示 = false;
        public static bool 可互动内容 = false;
        public static bool 显示可互动物品图片 = false;
        public static float ESPMonsterDistance = 50f;
        public static float ESPNPCDistance = 50f;
        public static float ESPInteractiveItemDistance = 50f;

        private static List<NPCAction> _cachedNPCList = new List<NPCAction>();
        private static List<Monster> _cachedMonsterList = new List<Monster>();
        private static List<InteractiveObject> _cachedInteractiveList = new List<InteractiveObject>();
        private static List<LevelEdit.PaperTrigger> LocalCanInteractiveObjectList = new List<LevelEdit.PaperTrigger>();
        private static List<Tuple<Vector2, Vector2, bool>> _boneConnections = new List<Tuple<Vector2, Vector2, bool>>();
        private static float _lastListUpdateTime = 0;
        private const float LIST_UPDATE_INTERVAL = 0.1f; // 列表更新间隔（减少高频获取组件）

        public static void DrawESPCheat()
        {
            if (!名称显示 && !线条显示 && !血条显示 && !骨骼绘制 && !ESP2D && !可互动内容)
                return;

            if (LevelLoadManager.Instance == null || LevelLoadManager.Instance.CurLevelRoot == null)
                return;

            var cameraManager = ProcedureMonoSingleton<CameraManager>.Instance;
            if (cameraManager == null) return;

            var camera = cameraManager.GetRealCameraScript();
            if (camera == null) return;

            var playerTransform = cameraManager._Owner.transform;
            if (playerTransform == null) return;

            // 定期更新缓存列表
            UpdateCachedLists(camera);

            // 处理NPC绘制
            ProcessActors(_cachedNPCList, 透视包含NPC, camera, playerTransform, ESPNPCDistance, 骨骼绘制,名称显示, 线条显示, 血条显示, ESP2D,Color.green,Color.cyan,Color.white,
                (obj) => "NPC - " + obj.name, (obj) => 100f, (obj) => 100f  );

            // 处理怪物绘制
            ProcessActors( _cachedMonsterList, false, camera, playerTransform, ESPMonsterDistance,骨骼绘制, 名称显示,线条显示,血条显示, ESP2D,Color.red,Color.white, Color.yellow,
              (obj) => obj._Define?.Name ?? "未知怪物", (obj) => obj.GetHp(), (obj) => obj.GetHPMax());

            // 处理可互动内容绘制
            ProcessInteractiveItems( _cachedInteractiveList, camera, playerTransform, ESPInteractiveItemDistance, 可互动内容 );
            // 处理可阅读绘制
            ProcessPaperItems(LocalCanInteractiveObjectList, camera, playerTransform, ESPInteractiveItemDistance, 可互动内容);
        }

        // 定期更新缓存列表
        private static void UpdateCachedLists(Camera camera)
        {
            if (Time.time - _lastListUpdateTime < LIST_UPDATE_INTERVAL)
                return;

            // 更新NPC列表
            _cachedNPCList.Clear();
            var npcs = LevelLoadManager.Instance.CurLevelRoot.GetComponentsInChildren<NPCAction>(true);
            foreach (var npc in npcs)
            {
                if (npc != null && npc.isActiveAndEnabled)
                    _cachedNPCList.Add(npc);
            }

            // 更新怪物列表
            _cachedMonsterList.Clear();
            if (LevelLoadManager.Instance.CurLevelRoot.ActorList != null)
            {
                foreach (var actor in LevelLoadManager.Instance.CurLevelRoot.ActorList)
                {
                    if (actor is Monster monster && monster != null && monster.isActiveAndEnabled && monster.GetHp() > 0)
                    {
                        _cachedMonsterList.Add(monster);
                    }
                }
            }

            // 更新可互动内容列表

            LocalCanInteractiveObjectList.Clear();
            var LevelInteractiveObject = LevelLoadManager.Instance.CurLevelRoot.GetComponentsInChildren<LevelEdit.PaperTrigger>(true);
            foreach (var LoaclInteractivein in LevelInteractiveObject)
            {
                    var LoaclPaper = LoaclInteractivein.Cast<LevelEdit.PaperTrigger>();
                    LocalCanInteractiveObjectList.Add(LoaclPaper);
            }
            _cachedInteractiveList.Clear();
            if (InteractiveManager.Instance != null && InteractiveManager.Instance.InteractiveList != null)
            {
                foreach (var item in InteractiveManager.Instance.InteractiveList)
                {
                    if (item.Value is InteractiveObject interactiveObj && interactiveObj != null && interactiveObj.isActiveAndEnabled)
                    {
                        _cachedInteractiveList.Add(interactiveObj);
                    }
                }
            }

            _lastListUpdateTime = Time.time;
        }

        // 通用Actor处理方法（NPC和怪物）
        private static void ProcessActors<T>(IEnumerable<T> actors,bool isNPC, Camera camera,Transform playerTransform,float espDistance, bool drawSkeleton, bool showName,  bool showLines,
            bool showHealth,  bool showESP2D, Color skeletonColor,Color LineColor, Color ESP2DColor,Func<T, string> nameGetter, Func<T, float> hpGetter, Func<T, float> maxHpGetter) where T : Component
        {
            foreach (var actor in actors)
            {
                if (actor == null)
                    continue;

                // 距离过滤
                var actorPos = actor.transform.position;
                var distance = Vector3.Distance(playerTransform.position, actorPos);
                if (distance >= espDistance)
                    continue;

                // 视野过滤（只处理相机前方的物体）
                var viewportPos = camera.WorldToViewportPoint(actorPos);
                if (viewportPos.z <= 0 || viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
                    continue;

                // 处理骨骼数据
                var skeleton = new SkeletonData();
                ProcessSkeletonData(actor, camera, ref skeleton);

                // 绘制骨骼
                if (drawSkeleton)
                {
                    DrawSkeletons(skeleton, skeletonColor, 1.5f);
                }

                // 转换屏幕坐标（基于骨骼关键点）
                var headScreenPos = skeleton.FoundHead ? ConvertToScreen(skeleton.Head) : ConvertToScreen(camera.WorldToScreenPoint(actorPos));
                var spine1ScreenPos = skeleton.FoundSpine1 ? ConvertToScreen(skeleton.Spine1) : headScreenPos + new Vector2(0, 30);
                var lFootScreenPos = skeleton.FoundLFoot ? ConvertToScreen(skeleton.LFoot) : ConvertToScreen(camera.WorldToScreenPoint(actorPos));
                var rFootScreenPos = skeleton.FoundRFoot ? ConvertToScreen(skeleton.RFoot) : lFootScreenPos;

                // 绘制名称
                if (showName)
                {
                    ESPNeed.ESPDrawNeed.DrawString(spine1ScreenPos, nameGetter(actor), Color.white, 17);
                    ESPNeed.ESPDrawNeed.DrawString(new Vector2(spine1ScreenPos.x, spine1ScreenPos.y + 17),
                        $"距离：{Mathf.Round(distance)}m", Color.white, CheatMain.FontSize.Value);
                }

                // 绘制线条（从玩家屏幕中心到目标）
                if (showLines && skeleton.FoundHead)
                {
                    ESPNeed.ESPDrawNeed.DrawLine(headScreenPos, new Vector2(Screen.width / 2f, 0), skeletonColor, 2f);
                }

                // 绘制血条
                if (showHealth && (skeleton.FoundHead || skeleton.FoundLFoot))
                {
                    float currentHp = hpGetter(actor);
                    float maxHp = maxHpGetter(actor);
                    ESPNeed.ESPDrawNeed.DrawHealthBar(headScreenPos, lFootScreenPos, rFootScreenPos, currentHp, maxHp);
                }

                // 绘制2D方框
                if (showESP2D && skeleton.FoundHead && (skeleton.FoundLFoot || skeleton.FoundRFoot))
                {
                    ESPNeed.ESPDrawNeed.Draw2DBox(headScreenPos, lFootScreenPos, rFootScreenPos, ESP2DColor);
                }
            }
        }

        // 处理可互动内容
        private static void ProcessInteractiveItems(List<InteractiveObject> items,Camera camera,Transform playerTransform, float espDistance,bool showInteractive )
        {
            if (!showInteractive || items == null || items.Count == 0)
                return;

            foreach (var item in items)
            {
                if (item == null || !item.isActiveAndEnabled)
                    continue;

                var itemPos = item.transform.position;
                var distance = Vector3.Distance(playerTransform.position, itemPos);
                if (distance >= espDistance)
                    continue;

                // 视野过滤
                var viewportPos = camera.WorldToViewportPoint(itemPos);
                if (viewportPos.z <= 0 || viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
                    continue;

                // 屏幕坐标转换
                var screenPos = ConvertToScreen(camera.WorldToScreenPoint(itemPos));
                if (item.GetScriptClassName() == "Transfer")
                {
                    ESPNeed.ESPDrawNeed.DrawString(screenPos, "可互动：晶枝", Color.cyan, 17);
                    ESPNeed.ESPDrawNeed.DrawString(new Vector2(screenPos.x, screenPos.y + 17),
                        $"距离：{Mathf.Round(distance)}m", Color.cyan, CheatMain.FontSize.Value);
                    ESPNeed.ESPDrawNeed.DrawLine(screenPos, new Vector2(Screen.width / 2f, 0), Color.cyan, 2f);
                }
                else if (item.GetScriptClassName() == "Elevator")
                {
                    ESPNeed.ESPDrawNeed.DrawString(screenPos, "可互动：电梯", Color.cyan, 17);
                    ESPNeed.ESPDrawNeed.DrawString(new Vector2(screenPos.x, screenPos.y + 17),
                        $"距离：{Mathf.Round(distance)}m", Color.cyan, CheatMain.FontSize.Value);
                    ESPNeed.ESPDrawNeed.DrawLine(screenPos, new Vector2(Screen.width / 2f, 0), Color.cyan, 2f);
                }
                
                else if (item.GetScriptClassName() =="Door")
                {
                    var Door = item.Cast<LevelEdit.Door>(); 
                    if (Door._IsOpen)
                        continue;
                    ESPNeed.ESPDrawNeed.DrawString(screenPos, "可互动：门" , Color.cyan, 17);
                    ESPNeed.ESPDrawNeed.DrawString(new Vector2(screenPos.x, screenPos.y + 17),
                        $"距离：{Mathf.Round(distance)}m", Color.cyan, CheatMain.FontSize.Value);
                    ESPNeed.ESPDrawNeed.DrawLine(screenPos, new Vector2(Screen.width / 2f, 0), Color.cyan, 2f);
                }
               else if (item.GetScriptClassName() == "Chest")
                {
                    var Chest = item.Cast<LevelEdit.Chest>();
                    if (Chest._IsOpen)
                        continue;
                    ESPNeed.ESPDrawNeed.DrawString(screenPos, "可互动：箱子", Color.cyan, 17);
                    ESPNeed.ESPDrawNeed.DrawString(new Vector2(screenPos.x, screenPos.y + 17),
                        $"距离：{Mathf.Round(distance)}m", Color.cyan, CheatMain.FontSize.Value);
                    ESPNeed.ESPDrawNeed.DrawLine(screenPos, new Vector2(Screen.width / 2f, 0), Color.cyan, 2f);
                }
                else if(item.GetScriptClassName() == "ItemGetTrigger")
                {
                    if (显示可互动物品图片)
                    {
                        var ItemTrigger = item.Cast<ItemGetTrigger>();
                        var LocalItemSprite = ESPItemCMP.GetItemSprite(ItemTrigger);
                        ESPDrawNeed.DrawItemSprie(LocalItemSprite, new Vector2(screenPos.x, screenPos.y + 45));
                    }
                    ESPNeed.ESPDrawNeed.DrawString(screenPos, "可互动：" + item.name, Color.cyan, 17);
                    ESPNeed.ESPDrawNeed.DrawString(new Vector2(screenPos.x, screenPos.y + 17),
                        $"距离：{Mathf.Round(distance)}m", Color.cyan, CheatMain.FontSize.Value);
                    ESPNeed.ESPDrawNeed.DrawLine(screenPos, new Vector2(Screen.width / 2f, 0), Color.cyan, 2f);
                }
                else            
                {
                    // 绘制可互动信息
                    ESPNeed.ESPDrawNeed.DrawString(screenPos, "可互动：" + item.name, Color.cyan, 17);
                    ESPNeed.ESPDrawNeed.DrawString(new Vector2(screenPos.x, screenPos.y + 17),
                        $"距离：{Mathf.Round(distance)}m", Color.cyan, CheatMain.FontSize.Value);
                    ESPNeed.ESPDrawNeed.DrawLine(screenPos, new Vector2(Screen.width / 2f, 0), Color.cyan, 2f);
                }
            
            }
        }

        private static void ProcessPaperItems(List<LevelEdit.PaperTrigger> Paper, Camera camera, Transform playerTransform, float espDistance, bool showInteractive)
        {
            if (!showInteractive)
                return;
            foreach (var LocalPaper in Paper)
            {
                var LocalPaperPos = LocalPaper.transform.position;
                var distance = Vector3.Distance(playerTransform.position, LocalPaperPos);
                if (distance >= espDistance)
                    continue;

                // 视野过滤
                var viewportPos = camera.WorldToViewportPoint(LocalPaperPos);
                if (viewportPos.z <= 0 || viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
                    continue;

                // 屏幕坐标转换
                var screenPos = ConvertToScreen(camera.WorldToScreenPoint(LocalPaperPos));
                if (显示可互动物品图片)
                    ESPDrawNeed.DrawItemSprie(LocalPaper.Icon, new Vector2(screenPos.x, screenPos.y + 45));
                ESPNeed.ESPDrawNeed.DrawString(screenPos, "可互动：调查物品" , Color.cyan, 17);
                ESPNeed.ESPDrawNeed.DrawString(new Vector2(screenPos.x, screenPos.y + 17),
                    $"距离：{Mathf.Round(distance)}m", Color.cyan, CheatMain.FontSize.Value);
                ESPNeed.ESPDrawNeed.DrawLine(screenPos, new Vector2(Screen.width / 2f, 0), Color.cyan, 2f);
            }
        }


        // 骨骼数据结构
        private struct SkeletonData
        {
            // 骨骼坐标（屏幕坐标）
            public Vector3 Head, Neck, Spine2, Spine1, Spine, Pelvis;
            public Vector3 LHand, LForearm, LUpperArm, RHand, RForearm, RUpperArm;
            public Vector3 LFoot, LCalf, LThigh, RFoot, RCalf, RThigh;
            public Vector3 RClavicle, LClavicle;

            // 骨骼是否找到
            public bool FoundHead, FoundNeck, FoundSpine2, FoundSpine1, FoundSpine, FoundPelvis;
            public bool FoundLHand, FoundLForearm, FoundLUpperArm;
            public bool FoundRHand, FoundRForearm, FoundRUpperArm;
            public bool FoundLFoot, FoundLCalf, FoundLThigh;
            public bool FoundRFoot, FoundRCalf, FoundRThigh;
            public bool FoundRClavicle, FoundLClavicle;
        }

        // 处理骨骼数据
        private static void ProcessSkeletonData<T>(T actor, Camera camera, ref SkeletonData data) where T : Component
        {
            // 重置骨骼数据
            data = new SkeletonData();

            // 处理所有骨骼类型
            ProcessSkeleton(actor, camera, SkeletonType.Head, ref data.Head, ref data.FoundHead);
            ProcessSkeleton(actor, camera, SkeletonType.Neck, ref data.Neck, ref data.FoundNeck);
            ProcessSkeleton(actor, camera, SkeletonType.Spine2, ref data.Spine2, ref data.FoundSpine2);
            ProcessSkeleton(actor, camera, SkeletonType.Spine1, ref data.Spine1, ref data.FoundSpine1);
            ProcessSkeleton(actor, camera, SkeletonType.Spine, ref data.Spine, ref data.FoundSpine);
            ProcessSkeleton(actor, camera, SkeletonType.Pelvis, ref data.Pelvis, ref data.FoundPelvis);

            ProcessSkeleton(actor, camera, SkeletonType.LHand, ref data.LHand, ref data.FoundLHand);
            ProcessSkeleton(actor, camera, SkeletonType.LForearm, ref data.LForearm, ref data.FoundLForearm);
            ProcessSkeleton(actor, camera, SkeletonType.LUpperArm, ref data.LUpperArm, ref data.FoundLUpperArm);
            ProcessSkeleton(actor, camera, SkeletonType.RHand, ref data.RHand, ref data.FoundRHand);
            ProcessSkeleton(actor, camera, SkeletonType.RForearm, ref data.RForearm, ref data.FoundRForearm);
            ProcessSkeleton(actor, camera, SkeletonType.RUpperArm, ref data.RUpperArm, ref data.FoundRUpperArm);

            ProcessSkeleton(actor, camera, SkeletonType.LFoot, ref data.LFoot, ref data.FoundLFoot);
            ProcessSkeleton(actor, camera, SkeletonType.LCalf, ref data.LCalf, ref data.FoundLCalf);
            ProcessSkeleton(actor, camera, SkeletonType.LThigh, ref data.LThigh, ref data.FoundLThigh);
            ProcessSkeleton(actor, camera, SkeletonType.RFoot, ref data.RFoot, ref data.FoundRFoot);
            ProcessSkeleton(actor, camera, SkeletonType.RCalf, ref data.RCalf, ref data.FoundRCalf);
            ProcessSkeleton(actor, camera, SkeletonType.RThigh, ref data.RThigh, ref data.FoundRThigh);

            ProcessSkeleton(actor, camera, SkeletonType.RClavicle, ref data.RClavicle, ref data.FoundRClavicle);
            ProcessSkeleton(actor, camera, SkeletonType.LClavicle, ref data.LClavicle, ref data.FoundLClavicle);
        }

        // 绘制骨骼连接
        private static void DrawSkeletons(SkeletonData data, Color color, float lineWidth)
        {
            _boneConnections.Clear(); // 复用列表，减少GC

            // 添加骨骼连接（只添加找到的骨骼）
            if (data.FoundHead && data.FoundNeck)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.Head), ConvertToScreen(data.Neck), true));
            if (data.FoundNeck && data.FoundSpine2)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.Neck), ConvertToScreen(data.Spine2), true));
            if (data.FoundSpine2 && data.FoundSpine1)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.Spine2), ConvertToScreen(data.Spine1), true));
            if (data.FoundSpine1 && data.FoundSpine)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.Spine1), ConvertToScreen(data.Spine), true));
            if (data.FoundSpine && data.FoundPelvis)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.Spine), ConvertToScreen(data.Pelvis), true));

            if (data.FoundLClavicle && data.FoundLUpperArm)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.LClavicle), ConvertToScreen(data.LUpperArm), true));
            if (data.FoundLUpperArm && data.FoundLForearm)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.LUpperArm), ConvertToScreen(data.LForearm), true));
            if (data.FoundLForearm && data.FoundLHand)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.LForearm), ConvertToScreen(data.LHand), true));

            if (data.FoundRClavicle && data.FoundRUpperArm)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.RClavicle), ConvertToScreen(data.RUpperArm), true));
            if (data.FoundRUpperArm && data.FoundRForearm)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.RUpperArm), ConvertToScreen(data.RForearm), true));
            if (data.FoundRForearm && data.FoundRHand)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.RForearm), ConvertToScreen(data.RHand), true));

            if (data.FoundPelvis && data.FoundLThigh)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.Pelvis), ConvertToScreen(data.LThigh), true));
            if (data.FoundLThigh && data.FoundLCalf)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.LThigh), ConvertToScreen(data.LCalf), true));
            if (data.FoundLCalf && data.FoundLFoot)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.LCalf), ConvertToScreen(data.LFoot), true));

            if (data.FoundPelvis && data.FoundRThigh)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.Pelvis), ConvertToScreen(data.RThigh), true));
            if (data.FoundRThigh && data.FoundRCalf)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.RThigh), ConvertToScreen(data.RCalf), true));
            if (data.FoundRCalf && data.FoundRFoot)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.RCalf), ConvertToScreen(data.RFoot), true));

            if (data.FoundNeck && data.FoundLClavicle)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.Neck), ConvertToScreen(data.LClavicle), true));
            if (data.FoundNeck && data.FoundRClavicle)
                _boneConnections.Add(Tuple.Create(ConvertToScreen(data.Neck), ConvertToScreen(data.RClavicle), true));

            // 绘制所有有效连接
            foreach (var conn in _boneConnections)
            {
                if (conn.Item3)
                {
                    ESPNeed.ESPDrawNeed.DrawLine(conn.Item1, conn.Item2, color, lineWidth);
                }
            }
        }

        // 屏幕坐标转换（Unity的Y轴与屏幕Y轴相反）
        private static Vector2 ConvertToScreen(Vector3 worldScreen)
        {
            return new Vector2(worldScreen.x, Screen.height - worldScreen.y);
        }

        // 骨骼处理核心方法（获取骨骼位置并转换为屏幕坐标）
        private static void ProcessSkeleton<T>(T actor, Camera camera, SkeletonType type, ref Vector3 screenPos, ref bool foundFlag) where T : Component
        {
            Transform boneTransform = null;

            // 根据演员类型获取骨骼（需要NPCAction和Monster实现GetSkeleton方法）
            if (actor is NPCAction npc)
            {
                boneTransform = npc.GetSkeleton(type);
            }
            else if (actor is Monster monster)
            {
                boneTransform = monster.GetSkeleton(type);
            }

            // 骨骼不存在或无效
            if (boneTransform == null)
            {
                foundFlag = false;
                return;
            }

            // 转换世界坐标到屏幕坐标（只保留相机前方的骨骼）
            Vector3 worldToScreen = camera.WorldToScreenPoint(boneTransform.position);
            if (worldToScreen.z < 0f) // 骨骼在相机后方
            {
                foundFlag = false;
                return;
            }

            // 更新屏幕坐标和找到状态
            screenPos = worldToScreen;
            foundFlag = true;
        }
        public static void DrawESPUI(GameObject tabGroup, UIBase Owner)
        {


            GameObject ESPUISet = UIFactory.CreateHorizontalGroup(tabGroup, "ESPWindowDrawESPUISet", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            ESPUISet.CreateInputEditButton(150, "设置怪物ESP距离", "50", "设置", (text) =>
            {
                float ESPMaxDistance = text.ConvertToFloatDef(50);
                ESPMonsterDistance = ESPMaxDistance;
            });
            ESPUISet.CreateInputEditButton(150, "设置NPCESP距离", "50", "设置", (text) =>
            {
                float ESPMaxDistance = text.ConvertToFloatDef(50);
                ESPNPCDistance = ESPMaxDistance;
            });
            ESPUISet.CreateInputEditButton(150, "设置可互动物品ESP距离", "50", "设置", (text) =>
            {
                float ESPMaxDistance = text.ConvertToFloatDef(50);
                ESPInteractiveItemDistance = ESPMaxDistance;
            });
            GameObject ESPItem = UIFactory.CreateHorizontalGroup(tabGroup, "ESPWindowESPItem", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            ESPItem.CreateToggle(100, "可互动内容", (state) => { 可互动内容 = state; }).SetTooltip("作为一名机兵，知道什么能开什么不能开很正常吧，不包含调查物品", tabGroup);
            ESPItem.CreateToggle(180, "显示可互动物品图片", (state) => { 显示可互动物品图片 = state; }).SetTooltip("作为一名机兵，可以看到互动物品的图片很合理吧", tabGroup);

            GameObject ESPCheckActor = UIFactory.CreateHorizontalGroup(tabGroup, "ESPWindowESPCheckActor", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            ESPCheckActor.CreateToggle(150, "透视包含NPC", (state) => { 透视包含NPC = state; }).SetTooltip("作为一名机兵，扫描NPC很合理吧", tabGroup); ;

            GameObject ESPUI = UIFactory.CreateHorizontalGroup(tabGroup, "ESPWindowDrawESP", false, false, true, true, 5, default,
            new Color(0.065f, 0.065f, 0.065f), TextAnchor.MiddleLeft);
            ESPUI.CreateToggle(100, "骨骼绘制", (state) => { 骨骼绘制 = state; }).SetTooltip("作为一名机兵，可以看到骨骼很合理吧", tabGroup);
            ESPUI.CreateToggle(100, "2D方框", (state) => { ESP2D = state; }).SetTooltip("作为一名机兵，可以看到方框很合理吧", tabGroup);
            ESPUI.CreateToggle(100, "名称显示", (state) => { 名称显示 = state; }).SetTooltip("作为一名机兵，可以看到名称很合理吧", tabGroup);
            ESPUI.CreateToggle(100, "线条显示", (state) => { 线条显示 = state; }).SetTooltip("作为一名机兵，可以看到线条很合理吧", tabGroup);
            ESPUI.CreateToggle(100, "血条显示", (state) => { 血条显示 = state; }).SetTooltip("作为一名机兵，可以看到血条很合理吧", tabGroup);
        }
    }
}
