//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-11-03 11:25:25
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using cfg.blobstruct;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Google.Protobuf.Collections;
using Main;
using Newtonsoft.Json;
using NUnit.Framework;
using ProjectDawn.Navigation;
using Spine.Unity;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Profiling;
using UnityEngine.UI;
using XFramework;
using YooAsset;
using static XFramework.UIManager;
using Material = UnityEngine.Material;

namespace HotFix_UI
{
    /// <summary>
    /// 存放一些通用UI的函数
    /// </summary>
    public static class JiYuUIHelper
    {
        /// <summary>
        /// 打开一个commonresource前清除掉前者的
        /// </summary>
        public static void ClearCommonResource()
        {
            if (JiYuUIHelper.TryGetUI(UIType.UICommon_Resource, out UI ui))
            {
                var uis = ui as UICommon_Resource;
                var root = uis.GetFromReference(UICommon_Resource.KContainer).GetRectTransform();
                for (int i = 0; i < root.ChildCount; i++)
                {
                    var child = root.GetChild(i);
                    child.SetScale2(0f);
                }
            }
        }

        public static async UniTask<Texture2D> CaptureScreenAsync()
        {
            // 等待帧结束
            await UniTask.WaitForEndOfFrame();

            // 调试屏幕分辨率
            Debug.Log($"Screen resolution: {Screen.width}x{Screen.height}");

            // 创建固定分辨率的 RenderTexture

            var global = XFramework.Common.Instance.Get<Global>();

            int width = Screen.width; // 固定宽度
            int height = Screen.height; // 固定高度
            // RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            // renderTexture.Create();
            // Debug.Log(
            //     $"RenderTexture created: {renderTexture.IsCreated()}, Size: {renderTexture.width}x{renderTexture.height}");
            //
            // // 将所有相机渲染到 RenderTexture
            // Camera[] cameras = Camera.allCameras;
            // foreach (Camera cam in cameras)
            // {
            //     Debug.Log($"cam.name: {cam.name}");
            //     if (cam.isActiveAndEnabled)
            //     {
            //         cam.targetTexture = renderTexture;
            //         cam.Render();
            //         cam.targetTexture = null;
            //     }
            // }

            // 创建 Texture2D
            // Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            // //RenderTexture.active = global.MainCamera.targetTexture;
            // //global.MainCamera.targetTexture.
            // texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            // texture.Apply();
            // Debug.Log($"Texture readable: {texture.isReadable}");
            //
            // // 保存为 PNG 文件
            // string path = Path.Combine(Application.dataPath, "screen1.png");
            // byte[] bytes = texture.EncodeToPNG();
            // File.WriteAllBytes(path, bytes);
            // Debug.Log($"Screenshot saved to: {path}");

            // 清理
            //RenderTexture.active = null;
            //UnityEngine.Object.Destroy(renderTexture);
            RenderTexture originalRenderTexture = global.BlurCamera.targetTexture; // 示例，相机目标纹理

            // 创建一个新的 RenderTexture
            RenderTexture newRenderTexture = new RenderTexture(originalRenderTexture.width,
                originalRenderTexture.height, 0, originalRenderTexture.format);
            newRenderTexture.Create();

            // 将 originalRenderTexture 的内容读取到 Texture2D
            Texture2D tempTexture = new Texture2D(originalRenderTexture.width, originalRenderTexture.height,
                TextureFormat.ARGB32, false);
            RenderTexture.active = originalRenderTexture;
            tempTexture.ReadPixels(new Rect(0, 0, originalRenderTexture.width, originalRenderTexture.height), 0, 0);
            tempTexture.Apply();

            // 现在，将该 Texture2D 写入新创建的 RenderTexture
            //Graphics.Blit(tempTexture, newRenderTexture);

            //var newTex = global.MainCamera.targetTexture;
            return tempTexture;
        }

        public async static UniTask InitBlur(this UI PanelUI)
        {
            var KCommon_Blur = PanelUI.GetFromReference("Common_Blur");
            PanelUI.SetActive(false);
            //var KBlur = GetFromReference(UIPanel_SelectBoxNomal.KBlur);
            var image = KCommon_Blur.GetComponent<Image>();
            var shader = await ResourcesManager.Instance.Loader.LoadAssetAsync<Shader>("UIBlur");
            //Material material = new Material(Shader.Find("Custom/UIBlur"));

            Material material = new Material(shader);
            //var global = XFramework.Common.Instance.Get<Global>();
            var texture2D = await JiYuUIHelper.CaptureScreenshot();
            material.SetTexture("_MainTex", texture2D);
            image.material = material;
            //image.texture = texture2D;

            //await UniTask.DelayFrame(1);
            PanelUI.SetActive(true);
        }

        public async static UniTask<Texture2D> CaptureScreenshot()
        {
            await UniTask.WaitForEndOfFrame();
            Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
            // // 创建RenderTexture
            // RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            // renderTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            // renderTexture.wrapMode = TextureWrapMode.Clamp;
            // renderTexture.Create(); // 确保创建
            //
            // // 设置相机目标并渲染
            // cameraToCapture.targetTexture = renderTexture;
            // cameraToCapture.Render();
            //
            //
            // // 创建Texture2D
            // Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            //
            // // 读取像素
            // RenderTexture.active = renderTexture;
            // texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            // texture.Apply();
            //
            // // 保存以调试（可选）
            //byte[] bytes = texture.EncodeToPNG();
            //System.IO.File.WriteAllBytes("Assets/screen1.png", bytes);
            // Debug.Log("Screenshot saved to Assets/screen1.png");
            //
            // // 清理
            // cameraToCapture.targetTexture = null;
            // RenderTexture.active = null;
            // GameObject.Destroy(renderTexture);

            return texture;
        }


        #region 断线重连

        public static string GetPhoneTypeStr()
        {
            string str = "";
            string splitStr = "|";

#if !UNITY_EDITOR
    #if UNITY_IOS 
            str += $"IOS{splitStr}";
    #elif UNITY_ANDROID 
            str += $"Android{splitStr}";
    #else  
            str += $"HarmaonyOS{splitStr}";
    #endif
#else
            str += $"Windows{splitStr}";
#endif
            str += $"{SystemInfo.deviceModel}{splitStr}";

            str += $"Ram{SystemInfo.systemMemorySize.ToString()}";

            //Log.Debug($"str:{str}");
            return str;
        }

        public static void LoginRequest(int type, string privateKey, string userName = "")
        {
            var gameUser = new LogicRequestPb
            {
                Name = userName,
                UdId = SystemInfo.deviceUniqueIdentifier,
                PrivateKey = privateKey,
                //1:测试输入名字  2:快速
                Type = type,
//#if !UNITY_EDITOR
                PhoneType = JiYuUIHelper.GetPhoneTypeStr()
//#endif
            };

            NetWorkManager.Instance.SendMessage(CMD.LOGIN, gameUser);
        }

        public async static void ReConnect()
        {
            var sharedData = await JsonManager.Instance.LoadSharedData();
            var playerData = await JsonManager.Instance.LoadPlayerData(sharedData.lastLoginUserId);
            LoginRequest(2, playerData.privateKey, playerData.nickName);
        }

        #endregion

        /// <summary>
        /// 一键通关
        /// </summary>
        public async static void QuickSucceed()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag), typeof(GameTimeData),
                typeof(GameEnviromentData));
            var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];
            var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            if (wbeQuery.IsEmpty)
            {
                return;
            }

            var entity = wbeQuery.ToEntityArray(Allocator.Temp)[0];
            var timeData = entityManager.GetComponentData<GameTimeData>(entity);
            var gameOthersData = entityManager.GetComponentData<GameOthersData>(entity);
            timeData.unScaledTime.elapsedTime = 9999;
            timeData.logicTime.elapsedTime = 9999;
            entityManager.SetComponentData(entity, timeData);
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            //var tblevel =ConfigManager.Instance.Tables.Tblevel;
            //tblevel.Get(gameOthersData.levelId ).rewardDrop
            playerData.playerOtherData.killBoss = 3;
            playerData.playerOtherData.killBossIdList.Add(4502000);
            playerData.playerOtherData.killBossIdList.Add(2503000);
            playerData.playerOtherData.killBossIdList.Add(4501000);
            playerData.playerOtherData.killMonsterIdList.Add(4502000);
            playerData.playerOtherData.killMonsterIdList.Add(2503000);
            playerData.playerOtherData.killMonsterIdList.Add(4501000);
            entityManager.SetComponentData(player, playerData);
            var ui = await UIHelper.CreateAsync(UIType.UIPanel_Success);
        }

        /// <summary>
        /// 判断是否运行在模拟器上
        /// 通过cpu类型来判断，电脑cpu一般是intel和amd，都是x86架构
        /// </summary>
        /// <returns>是否运行在模拟器上</returns>
        public static bool IsSimulator()
        {
            if (SystemInfo.deviceModel.Contains("Emulator") || SystemInfo.deviceModel.Contains("Android SDK"))
                return true;
            if (SystemInfo.deviceName.Contains("Android SDK")) //设备名称的字符串检测
                return true;
            if (SystemInfo.deviceType == DeviceType.Unknown &&
                SystemInfo.operatingSystem.Contains("x86")) //当前设备的类型字符串检测
                return true;
            if (SystemInfo.processorType.Contains("Intel") || SystemInfo.processorType.Contains("AMD")) //当前设备的CPU类型检测
                return true;
            if (SystemInfo.graphicsDeviceName.Contains("Intel") ||
                SystemInfo.graphicsDeviceName.Contains("VBox")) //当前设备的显卡名称检测
                return true;
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass buildClass = new AndroidJavaClass("android.os.Build");
                string radioVersion = buildClass.CallStatic<string>("getRadioVersion");
                int.TryParse(radioVersion, out int result);
                if (result == 0)
                {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnableISystem<T>(bool enable) where T : ISystem
        {
            var systemHandle = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(T));
            ref var system =
                ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(systemHandle);
            system.Enabled = enable;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnableISystem<T1, T2, T3>(bool enable)
            where T1 : ISystem where T2 : ISystem where T3 : ISystem
        {
            var systemHandle1 = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(T1));
            var systemHandle2 = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(T2));
            var systemHandle3 = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(T3));
            ref var system1 =
                ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(systemHandle1);
            ref var system2 =
                ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(systemHandle2);
            ref var system3 =
                ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(systemHandle3);
            system1.Enabled = enable;
            system2.Enabled = enable;
            system3.Enabled = enable;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnableSystemBase<T>(bool enable) where T : SystemBase
        {
            var systemHandle = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged(typeof(T));
            systemHandle.Enabled = enable;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StartStopTime(bool isEnable)
        {
            UnityHelper.EnableTime(isEnable);

            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
            {
                var uiRuntime = ui as UIPanel_RunTimeHUD;
                uiRuntime.EnableInputBar(isEnable);
                uiRuntime.StartTimer(isEnable);
            }

            AudioManager.Instance.StopFModSFX(!isEnable);
            //return;
            // var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            // var wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag), typeof(GameTimeData));
            // if (wbeQuery.IsEmpty)
            // {
            //     return;
            // }
            //
            // var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];
            //
            // var gameTimeData = entityManager.GetComponentData<GameTimeData>(wbe);
            // gameTimeData.logicTime.gameTimeScale = enable ? gameTimeData.logicTime.defaultGameTimeScale : 0;
            // gameTimeData.refreshTime.gameTimeScale = enable ? gameTimeData.refreshTime.defaultGameTimeScale : 0;
            // entityManager.SetComponentData(wbe, gameTimeData);

            // var physicsSystemGroup =
            //     World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PhysicsSystemGroup>();
            // physicsSystemGroup.Enabled = enable;
        }

        #region IntroGuide

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async static UniTask<Entity> DoMonsterPos(int monsterId, float2 targetPos, float duration)
        {
            var EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = EntityManager.CreateEntityQuery(typeof(EnemyData), typeof(ChaStats));
            var enemies = playerQuery.ToEntityArray(Allocator.Temp);
            foreach (var enemy in enemies)
            {
                var enemyData = EntityManager.GetComponentData<EnemyData>(enemy);
                if (enemyData.enemyID == monsterId)
                {
                    var chaStats = EntityManager.GetComponentData<ChaStats>(enemy);
                    var agentBody = EntityManager.GetComponentData<AgentBody>(enemy);
                    var agentLocomotion = EntityManager.GetComponentData<AgentLocomotion>(enemy);
                    var tran = EntityManager.GetComponentData<LocalTransform>(enemy);
                    var stateMachine = EntityManager.GetComponentData<StateMachine>(enemy);
                    var targetPos0 = new float3(targetPos, 0);
                    agentBody.SetDestination(targetPos0);
                    agentLocomotion.Speed = math.length(targetPos0 - tran.Position) / duration;
                    stateMachine.curAnim = AnimationEnum.Run;
                    EntityManager.SetComponentData(enemy, agentBody);
                    EntityManager.SetComponentData(enemy, agentLocomotion);
                    EntityManager.SetComponentData(enemy, stateMachine);
                    await UniTask.Delay((int)(duration * 1000));
                    stateMachine.curAnim = AnimationEnum.Idle;
                    EntityManager.SetComponentData(enemy, stateMachine);
                    return enemy;
                    break;
                }
            }

            return Entity.Null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async static UniTaskVoid DestroyMonster(int monsterId)
        {
            var EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = EntityManager.CreateEntityQuery(typeof(EnemyData), typeof(ChaStats));
            var enemies = playerQuery.ToEntityArray(Allocator.Temp);
            foreach (var enemy in enemies)
            {
                var enemyData = EntityManager.GetComponentData<EnemyData>(enemy);
                if (enemyData.enemyID == monsterId)
                {
                    EntityManager.DestroyEntity(enemy);
                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async static UniTask DoPlayerPos(float2 targetPos, float duration)
        {
            var EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = EntityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var chaStats = EntityManager.GetComponentData<ChaStats>(player);
            var agentBody = EntityManager.GetComponentData<AgentBody>(player);
            var agentLocomotion = EntityManager.GetComponentData<AgentLocomotion>(player);
            var tran = EntityManager.GetComponentData<LocalTransform>(player);
            var stateMachine = EntityManager.GetComponentData<StateMachine>(player);
            var targetPos0 = new float3(targetPos, 0);
            agentBody.SetDestination(targetPos0);
            agentLocomotion.Speed = math.length(targetPos0 - tran.Position) / duration;
            stateMachine.curAnim = AnimationEnum.Run;
            EntityManager.SetComponentData(player, agentBody);
            EntityManager.SetComponentData(player, agentLocomotion);
            EntityManager.SetComponentData(player, stateMachine);
            await UniTask.Delay((int)(duration * 1000));
            stateMachine.curAnim = AnimationEnum.Idle;
            EntityManager.SetComponentData(player, stateMachine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async static UniTask EnableGuide(bool enable)
        {
            //JiYuUIHelper.EnableISystem<WeaponAnimSystem>(!enable);
            JiYuUIHelper.EnableISystem<TriggerSystem, SpawnEnemySystem, SkillCastSystem>(enable);
            // JiYuUIHelper.EnableISystem<SpawnEnemySystem>(!enable);
            // JiYuUIHelper.EnableISystem<SkillCastSystem>(!enable);
            // await JiYuTweenHelper.EnableLoading(true,
            //     enable ? UIManager.LoadingType.TranstionShattersEnter : UIManager.LoadingType.TranstionShattersExit);
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
            {
                var uiRuntime = ui as UIPanel_RunTimeHUD;
                uiRuntime.EnableInputBar(enable);
                uiRuntime.StartTimer(enable);
            }

            var EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = EntityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));
            while (playerQuery.IsEmpty)
            {
                await UniTask.Delay(200);
            }

            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var chaStats = EntityManager.GetComponentData<ChaStats>(player);
            chaStats.chaControlState.cantMove = !enable;
            chaStats.chaControlState.cantWeaponAttack = !enable;
            EntityManager.SetComponentData(player, chaStats);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async static UniTask SetPlayerMass(bool isNormalMass = true)
        {
            var EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = EntityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));
            while (playerQuery.IsEmpty)
            {
                await UniTask.Delay(200);
            }

            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var physicsMass = EntityManager.GetComponentData<PhysicsMass>(player);
            float mass = 0.0001f;
            if (isNormalMass)
            {
                var chaStats = EntityManager.GetComponentData<ChaStats>(player);
                mass = (float)chaStats.chaProperty.mass;
            }

            physicsMass.InverseMass = 1f / mass;

            EntityManager.SetComponentData(player, physicsMass);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapElementID">障碍物或地形id</param>
        /// <param name="pos">绝对坐标</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async static UniTask SpawnMapElement(int mapElementID, float3 pos)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag));
            while (wbeQuery.IsEmpty)
            {
                await UniTask.Delay(200);
            }

            var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];
            var prefabMapData = entityManager.GetComponentData<PrefabMapData>(wbe);

            var sceneModuleConfig = ConfigManager.Instance.Tables.Tbscene_module;
            var moduleName = sceneModuleConfig.Get(mapElementID).model;
            var size = sceneModuleConfig.Get(mapElementID).size;


            var entity = entityManager.Instantiate(prefabMapData.prefabHashMap[moduleName]);
            var loc = new LocalTransform { Position = pos, Rotation = quaternion.identity };
            var pushData = new PushColliderData
            {
                tick = 0,
                toBeSmall = false,
                initScale = 0,
                targetScale = size[0] / 1000f
            };
            entityManager.SetComponentData(entity, loc);
            entityManager.SetComponentData(entity, pushData);
            // entityManager.AddComponentData(entity, new PostTransformMatrix
            // {
            //     Value = float4x4.Scale(size[0] / 1000f, size[1] / 1000f, 1)
            // });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapElementID">障碍物或地形id</param>
        /// <param name="pos">绝对坐标</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async static UniTask SpawnMapElement(string prefab, float3 pos, float scale)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag));
            while (wbeQuery.IsEmpty)
            {
                await UniTask.Delay(200);
            }

            var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];

            var prefabMapData = entityManager.GetComponentData<PrefabMapData>(wbe);
            var entity = entityManager.Instantiate(prefabMapData.prefabHashMap[prefab]);
            var loc = new LocalTransform { Position = pos, Rotation = quaternion.identity };
            var pushData = new PushColliderData
            {
                tick = 0,
                toBeSmall = false,
                initScale = 0,
                targetScale = scale
            };
            entityManager.SetComponentData(entity, loc);
            entityManager.SetComponentData(entity, pushData);
            // entityManager.AddComponentData(entity, new PostTransformMatrix
            // {
            //     Value = float4x4.Scale(size[0] / 1000f, size[1] / 1000f, 1)
            // });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapElementID">障碍物或地形id</param>
        /// <param name="pos">绝对坐标</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SpawnDropItem(int itemId, float3 pos)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag));
            var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];
            var prefabMapData = entityManager.GetComponentData<PrefabMapData>(wbe);
            var gameOthersData = entityManager.GetComponentData<GameOthersData>(wbe);
            var battleItem = ConfigManager.Instance.Tables.Tbbattle_item.Get(itemId);


            var entity = entityManager.Instantiate(prefabMapData.prefabHashMap[battleItem.model]);
            var tran = entityManager.GetComponentData<LocalTransform>(prefabMapData.prefabHashMap[battleItem.model]);
            tran.Position = pos;

            entityManager.SetComponentData(entity, tran);
            entityManager.SetComponentData(entity, new DropsData
            {
                id = itemId,
                point0 = tran.Position,
                point1 = default,
                point2 = new float3(tran.Position.x,
                    tran.Position.y, 0),
                point3 = default,
                isDropAnimed = false,
                isLooting = false,
                lootingAniDuration = gameOthersData.pickupDuration / 1000f,
                dropPoint0 = tran.Position,
                dropPoint2 = tran.Position
            });
        }

        #endregion


        // public static void EnableTriggerSystem(bool enable)
        // {
        //     // var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //     // var wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag), typeof(GameTimeData));
        //     // if (wbeQuery.IsEmpty)
        //     // {
        //     //     return;
        //     // }
        //     //
        //     // var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];
        //
        //
        //     var initMapSystem =
        //         World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<TriggerSystem>();
        //     ref var state = ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(initMapSystem);
        //     state.Enabled = enable;
        // }
        public static BattleGain GetBattleGain(float liveTime, PlayerData playerData, ChaStats chaStats,
            bool isWin = true)
        {
            var tbmonster = ConfigManager.Instance.Tables.Tbmonster;
            var tbmonster_attr = ConfigManager.Instance.Tables.Tbmonster_attr;
            var tbmonster_model = ConfigManager.Instance.Tables.Tbmonster_model;
            var tbweapon = ConfigManager.Instance.Tables.Tbweapon;

            var battleGain = new BattleGain();
            //battleGain.Args.Add($"4;0;{playerData.playerData.exp}");
            //玩家局内获得的钞票
            battleGain.Args.Add($"3;0;{playerData.playerData.exp}");
            //可局外带走的局内掉落item
            foreach (var reward in playerData.playerOtherData.outerItemList)
            {
                battleGain.Args.Add(reward.Value);
            }

            battleGain.KillMobs = playerData.playerOtherData.killLittleMonster;
            battleGain.KillBoss = playerData.playerOtherData.killBoss;
            battleGain.KillElite = playerData.playerOtherData.killElite;

            var killBossIdList = playerData.playerOtherData.killBossIdList.ToArray();
            var killMonsterIdList = playerData.playerOtherData.killMonsterIdList.ToArray();
            battleGain.BossIdlist.AddRange(killBossIdList);
            //图鉴
            List<int> bookMonsterId = new List<int>();
            List<int> bookWeaponId = new List<int>();
            foreach (var monsterId in killMonsterIdList)
            {
                var monster = tbmonster.Get(monsterId);
                var monsterAttr = tbmonster_attr.Get(monster.monsterAttrId);
                var monsterModel = tbmonster_model.Get(monsterAttr.bookId);
                if (monsterModel.powerId > 0)
                {
                    bookMonsterId.TryAdd(monsterAttr.bookId);
                }

                if (monster.monsterWeaponId > 0)
                {
                    bookWeaponId.TryAdd(monster.monsterWeaponId);
                }
            }

            battleGain.BookIdlist.AddRange(bookMonsterId);
            battleGain.WeaponIdlist.AddRange(bookWeaponId);
            battleGain.LevelId = ResourcesSingleton.Instance.levelInfo.levelId;
            battleGain.PassStatus = isWin ? "true" : "false";
            battleGain.LiveTime = (long)liveTime;
            Log.Debug($"battleGain.LiveTime{battleGain.LiveTime}");
            battleGain.BattleId = ResourcesSingleton.Instance.battleData.battleId;
            return battleGain;
        }

        public static int FindFirstGreaterThan(List<long> list, long target)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] > target)
                {
                    return i;
                }
            }

            return -1;
        }

        public static void ShowPassLevelExp(int exp, UI slideUI, UI levelTextUI, UI passLevelTextUI)
        {
            var tbbattlepass_exp = ConfigManager.Instance.Tables.Tbbattlepass_exp;
            List<long> arr = new List<long>();
            foreach (var levellist in tbbattlepass_exp.DataList)
            {
                arr.Add(levellist.exp);
            }

            int result = FindFirstGreaterThan(arr, exp);
            Log.Debug($"result:{result}");
            arr.Clear();
            long curExp = 0;
            long levelUpNeedExp = 0;
            int level = 0;
            double expRatios;

            if (result != -1)
            {
                level = tbbattlepass_exp.DataList[result - 1].id;
            }
            else
            {
                level = tbbattlepass_exp.DataList[tbbattlepass_exp.DataList.Count - 1].id;
                expRatios = 1;
                levelTextUI.GetTextMeshPro().SetTMPText(level.ToString());
                slideUI.GetImage().DoFillAmount((float)expRatios, 0.3f);
                
                levelUpNeedExp = tbbattlepass_exp.DataList[tbbattlepass_exp.DataList.Count - 1].exp - tbbattlepass_exp.DataList[tbbattlepass_exp.DataList.Count - 2].exp;
                passLevelTextUI.GetTextMeshPro().SetTMPText($"{levelUpNeedExp}/{levelUpNeedExp}");
                return;
            }

            if (tbbattlepass_exp.DataList[result] != null)
            {
                levelUpNeedExp = tbbattlepass_exp.DataList[result].exp - tbbattlepass_exp.DataList[result-1].exp;
                curExp = exp - tbbattlepass_exp.DataList[result - 1].exp;
            }

            expRatios = curExp / (double)levelUpNeedExp;
            if (!double.IsNaN(expRatios))
            {
                slideUI.GetImage().DoFillAmount((float)expRatios, 0.3f);
            }

            passLevelTextUI.GetTextMeshPro().SetTMPText($"{curExp}/{levelUpNeedExp}");

            levelTextUI.GetTextMeshPro().SetTMPText(level.ToString());
        }

        /// <summary>
        /// 创建全局实体,给其添加各种单例组件
        /// </summary>
        /// <returns></returns>
        public static Entity CreateBlackBoardEntity(int sceneId)
        {
            var global = XFramework.Common.Instance.Get<Global>();
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var monsterTemplateConfig = ConfigManager.Instance.Tables.Tbmonster_template;
            //int levelId = ResourcesSingleton.Instance.levelInfo.levelId;
            //int sceneId = ConfigManager.Instance.Tables.Tblevel.Get(levelId).sceneId;
            int monsterRefreshId = ConfigManager.Instance.Tables.Tbscene.Get(sceneId)
                .monsterTemplateId;
            var scene = ConfigManager.Instance.Tables.Tbscene.Get(sceneId);
            var refreshTemplate = scene.monsterTemplatePara;
            // if (refreshTemplate.Count > monsterTemplateConfig.DataList.Count)
            // {
            //     Log.Error($"level表的monsterRefreshTemplate 字段长度超出monster_template表的长度！");
            // }
            var environmentList = scene.environmentId;
            var env = new GameEnviromentData
            {
                env = new Enviroment
                {
                    weather = 100,
                    time = 200
                }
            };

            foreach (var environment in environmentList)
            {
                if (environment / 100 == 1)
                {
                    env.env.weather = environment;
                }
                else if (environment / 100 == 2)
                {
                    env.env.time = environment;
                }
            }

            for (int i = 0; i < monsterTemplateConfig.DataList.Count; i++)
            {
                if (monsterTemplateConfig.DataList[i].id == monsterRefreshId)
                {
                    if (monsterTemplateConfig.DataList[i].paraPos == 0)
                    {
                        monsterTemplateConfig.DataList[i].monsterId = 0;
                        //Log.Error($"scene表的monster_template_para 字段长度超出monster_template表的长度！");
                        continue;
                    }

                    if (monsterTemplateConfig.DataList[i].paraPos <= -1)
                    {
                        monsterTemplateConfig.DataList[i].monsterId = -1;
                        //Log.Error($"scene表的monster_template_para 字段长度超出monster_template表的长度！");
                        continue;
                    }

                    if (monsterTemplateConfig.DataList[i].paraPos > refreshTemplate.Count)
                    {
                        Log.Error($"scene表的monster_template_para 字段不等于monster_template表的长度！无效");
                        continue;
                    }

                    monsterTemplateConfig.DataList[i].monsterId =
                        refreshTemplate[monsterTemplateConfig.DataList[i].paraPos - 1];
                }
            }

            // for (int i = 0; i < refreshTemplate.Count; i++)
            // {
            //     monsterTemplateConfig.DataList[i].monsterId = refreshTemplate[i];
            // }


            var blobAssetReference = GenGenBlobAssetReference.CreateBlob(ConfigManager.Instance.Tables);

            // Log.Debug(
            //     $"测试blob：{blobAssetReference.Value.configTbskill_effects.configTbskill_effects[0].skillEffectBuffNew[0]}");
            //
            // Log.Debug(
            //     $"测试blobc0：{blobAssetReference.Value.configTbskill_effects.configTbskill_effects[0].skillEffectBuffNew[0].c0}");

            //创建世界黑板entity 为避免多个原型 所有单例组件都应加到这一个entity上

            var e = entityManager.CreateSingleton(new GlobalConfigData
            {
                value = blobAssetReference
            }, "WorldBlackBoard");

            //
            entityManager.AddBuffer<DamageInfo>(e);
            entityManager.AddBuffer<GameEvent>(e);
            //游戏内添加设置组件
            entityManager.AddComponentData(e, new GameSetUpData
            {
                enableDamageNumber = true,
                enableSoundEffects = true
            });


            uint GenerateRandomUInt()
            {
                int lowerInt = UnityEngine.Random.Range(int.MinValue, int.MaxValue); // 生成一个int类型的随机值
                int upperInt = UnityEngine.Random.Range(int.MinValue, int.MaxValue); // 生成另一个int类型的随机值
                uint randomUInt = unchecked((uint)((uint)lowerInt << 16) | (uint)upperInt); // 结合两个int值生成一个uint值
                return randomUInt;
            }

            // 使用示例
            uint randomValue = GenerateRandomUInt();
            entityManager.AddComponentData(e, new GameRandomData
            {
                rand = Unity.Mathematics.Random.CreateFromIndex(randomValue),
                seed = randomValue
            });


            // var anims = new NativeHashMap<FixedString128Bytes, int>(50, Allocator.Persistent);
            // List<string> animsList = new List<string>();
            // animsList.Add("Geek_001_Walk_Left");
            // animsList.Add("Geek_001_Dying");
            // animsList.Add("Geek_001_Hurt_Force");
            // foreach (var VARIABLE in animsList)
            // {
            //     anims.TryAdd(VARIABLE, Animator.StringToHash(VARIABLE));
            // }

            var mapID = ConfigManager.Instance.Tables.Tbscene.Get(sceneId).mapId;

            var mapType = ConfigManager.Instance.Tables.Tbscene_module.Get(mapID).mapType;

            var mapSize = new float2(ConfigManager.Instance.Tables.Tbscene_module.Get(mapID).size[0] / 1000f,
                ConfigManager.Instance.Tables.Tbscene_module.Get(mapID).size[1] / 1000f);

            //Log.Error($"levelId{levelId} sceneId{sceneId} mapType{mapType} ");
            global.InitCameraBounds(default, mapType);
            global.DoCameraFOV(scene.camera);

            entityManager.AddComponentData(e, new GameOthersData
            {
                allAudioClips = AudioManager.Instance.InitRunTimeAudio(),
                //animations = anims,
                levelId = ResourcesSingleton.Instance.levelInfo.levelId,
                sceneId = sceneId,
                monsterRefreshId = monsterRefreshId,
                pickupDuration = 1000,
                dropPoint1Offset = 2000,
                //hitBackMaxCount = 2,
                mapData = new MapData
                {
                    mapID = mapID,
                    mapType = mapType,
                    mapSize = mapSize
                },
                enableTest1 = false,
                enableTest2 = false,
                BossEntity = default,
                battleShopStage = 0,
                gameOtherParas = new GameOthersData.GameOtherParas
                {
                    getHitDuration = 0.4f,
                    getHitOffset = 0.25f,
                    alphaSpeed = 0.25f,
                    dissolveSpeed = 1,
                    dropAnimedDuration = .35f,
                    dropAnimedHeight = 12
                },
            });
            entityManager.AddComponentData(e, new GameCameraSizeData
            {
                width = 110,
                height = 260
            });

            //var maprects = new NativeList<Rect>(9999, Allocator.Persistent);

            entityManager.AddComponentData(e, new MapRefreshData
            {
                isMapInit = false,
                maxPosUpleft = default,
                minPosBottomLeft = default,
                maxIndex = 0,
                minIndex = 0,
            });
            entityManager.AddComponentData(e, env);
            var gameEvents = entityManager.AddBuffer<GameEvent>(e);
            var anecdoteGroup = ConfigManager.Instance.Tables.Tbscene.Get(sceneId).anecdoteGroup;
            foreach (var ane in anecdoteGroup)
            {
                var eventGroup = ConfigManager.Instance.Tables.Tbanecdote.Get(ane).eventGroup;
                foreach (var event0 in eventGroup)
                {
                    var tbevent = ConfigManager.Instance.Tables.Tbevent_0.Get(event0);
                    gameEvents.Add(new GameEvent
                    {
                        CurrentTypeId = (GameEvent.TypeId)tbevent.type,
                        Int32_0 = event0
                    });
                }
            }

            entityManager.AddComponentData(e, new GameTimeData
            {
                refreshTime = new JiYuTime()
                {
                    elapsedTime = 0,
                    tick = 0,
                    gameTimeScale = 1,
                    defaultGameTimeScale = 1
                },
                logicTime = new JiYuTime()
                {
                    elapsedTime = 0,
                    tick = 0,
                    gameTimeScale = 1,
                    defaultGameTimeScale = 1
                },
                unScaledTime = new JiYuTime()
                {
                    elapsedTime = 0,
                    tick = 0,
                    gameTimeScale = 1,
                    defaultGameTimeScale = 1
                }
            });


            return e;
        }

        public static void InitSystem()
        {
            var initPlayerSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystemManaged<InitPlayerSystem>();
            var initSysGroup =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<InitializationSystemGroup>();
            initSysGroup.AddSystemToUpdateList(initPlayerSystem);
            initPlayerSystem.Enabled = true;
            //启用一次预制件映射系统
            var PrefabMapSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystemManaged<PrefabMapSystem>();
            PrefabMapSystem.Enabled = true;

            
            var fixedStepSimulationSystemGroup =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>();
            // var updatePlayerGOSystem = World.DefaultGameObjectInjectionWorld
            //     .GetOrCreateSystemManaged<UpdatePlayerGOSystem>();
            //
            // fixedStepSimulationSystemGroup.AddSystemToUpdateList(updatePlayerGOSystem);
            // updatePlayerGOSystem.Enabled = true;

            var hybridEventSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystemManaged<HybridEventSystem>();
            fixedStepSimulationSystemGroup.AddSystemToUpdateList(hybridEventSystem);
            hybridEventSystem.Enabled = true;

            var agentPathingSystemGroup =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<AgentPathingSystemGroup>();
            agentPathingSystemGroup.Enabled = false;
        }

        public static void EnterChapter(int id)
        {
            var tbchapter = ConfigManager.Instance.Tables.Tbchapter;
            var tblevel = ConfigManager.Instance.Tables.Tblevel;
            var levelId = tbchapter.Get(id).levelId;
            ResourcesSingleton.Instance.levelInfo.levelId = tbchapter.Get(id).levelId;
            int adNum = (int)tblevel.Get(levelId).reviveNum[0].x;
            int reviveNum = (int)tblevel.Get(levelId).reviveNum[0].y;
            ResourcesSingleton.Instance.levelInfo.rebirthNum = reviveNum;
            ResourcesSingleton.Instance.levelInfo.adRebirthNum = adNum;


            // InitShader();
            // CreateBlackBoardEntity();
            // InitSystem();
        }

        public static void DestoryWbe()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery =
                entityManager.CreateEntityQuery(new EntityQueryBuilder(Allocator.Temp).WithAll<WorldBlackBoardTag>());
            var playerQuery =
                entityManager.CreateEntityQuery(new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerData>());
            // var crowdQuery =
            //     entityManager.CreateEntityQuery(new EntityQueryBuilder(Allocator.Temp).WithAll<CrowdSurface>()
            //         .WithAll<LinkedEntityGroup>());

            if (entityQuery.IsEmpty) return;
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];

            var playerData = entityManager.GetComponentData<PlayerData>(player);

            var e = entityQuery.ToEntityArray(Allocator.Temp)[0];
            var gameOthersData = entityManager.GetComponentData<GameOthersData>(e);
            var configData = entityManager.GetComponentData<GlobalConfigData>(e);
            var prefabMapData = entityManager.GetComponentData<PrefabMapData>(e);
            prefabMapData.prefabHashMap.Dispose();
            configData.Dispose();
            gameOthersData.Dispose();
            // Debug.Log($"crowdQuery1 {crowdQuery.IsEmpty}");
            // if (!crowdQuery.IsEmpty)
            // {
            //     Debug.Log($"crowdQuery {crowdQuery.CalculateEntityCount()}");
            //     foreach (var VARIABLE in crowdQuery.ToEntityArray(Allocator.Temp))
            //     {
            //         var linkedEntityGroups = entityManager.GetBuffer<LinkedEntityGroup>(VARIABLE);
            //         var temp = linkedEntityGroups.ToNativeArray(Allocator.Temp);
            //         foreach (var entityGroup in temp)
            //         {
            //             entityManager.DestroyEntity(entityGroup.Value);
            //         }
            //
            //         entityManager.DestroyEntity(VARIABLE);
            //     }
            //
            //     //entityManager.DestroyEntity(crowdQuery);
            // }

            entityManager.DestroyEntity(entityQuery);
        }


        public static async UniTaskVoid ExitRunTimeScene()
        {
            JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionFXEnter);
            await UniTask.Delay(500, true);
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
            {
                ui.Dispose();
            }

            DestoryWbe();
            WebMessageHandler.Instance.Clear();

            var sceneController = XFramework.Common.Instance.Get<SceneController>();
            var sceneObj = sceneController.LoadSceneAsync<MenuScene>(SceneName.UIMenu);
            //SceneResManager.WaitForCompleted(sceneObj)();

            // var global = XFramework.Common.Instance.Get<Global>();
            // global.SetCameraPos(default);
        }

        #region GOTO 跳转

        static void ExtractUnlockOrGoToStr(string input, out int type, out List<int> paras)
        {
            type = ExtractTypeValue(input);
            paras = ExtractParaValues(input);
        }

        static int ExtractTypeValue(string input)
        {
            // 使用正则表达式提取 type 的值
            var match = Regex.Match(input, @"type=(\d+)");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }

            throw new FormatException("Type value not found.");
        }

        static List<int> ExtractParaValues(string input)
        {
            // 使用正则表达式提取 para 的值
            var match = Regex.Match(input, @"para=\[(.*?)\]");
            if (match.Success)
            {
                string paraContent = match.Groups[1].Value;
                // 将以逗号分隔的字符串拆分成数组，再转换成整型
                string[] values = paraContent.Split(',');
                List<int> paraValues = new List<int>();
                foreach (var value in values)
                {
                    if (int.TryParse(value.Trim(), out int paraValue)) // 将字符串转换为整型并添加到列表中
                    {
                        paraValues.Add(paraValue);
                    }
                }

                return paraValues;
            }

            throw new FormatException("Para values not found.");
        }


        private async static void GoToPanel(int type, int panelId)
        {
            if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                return;
            var uis = ui as UIPanel_JiyuGame;

            int tagId = 0;

            if (type == 2)
            {
                panelId = panelId / 100;
            }

            tagId = panelId / 100;
            var curstate = uis.GoToTagId(tagId);
            if (curstate == -1)
            {
                return;
            }
            else if (curstate == 1)
            {
                await UniTask.Delay(500);
            }

            int last2num = panelId % 100;
            if (tagId == 1)
            {
                if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Shop, out var ui0))
                    return;

                var ui0s = ui0 as UIPanel_Shop;
                Log.Debug($"last2num {last2num}");
                switch (last2num)
                {
                    case 11:
                        ui0s.SelectShopStateByTagPosType(1);
                        break;
                    case 21:
                        ui0s.SelectShopStateByTagPosType(2);
                        break;
                    case 31:
                        ui0s.SelectShopStateByTagPosType(3);
                        break;
                    case 41:
                        ui0s.SelectShopStateByTagPosType(4, 1);
                        break;
                    case 42:
                        ui0s.SelectShopStateByTagPosType(4, 2);
                        break;
                    case 43:
                        ui0s.SelectShopStateByTagPosType(4, 3);
                        break;
                }
            }
            else if (tagId == 2)
            {
                if (!JiYuUIHelper.TryGetUI(UIType.UISubPanel_Equipment, out var ui0))
                    return;
                var ui0s = ui0 as UISubPanel_Equipment;


                switch (last2num)
                {
                    case 11:
                        ui0s.OnBtnClickEvent(1);
                        await UniTask.Delay(200);
                        if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui1))
                            return;
                        var ui1s = ui1 as UIPanel_Equipment;
                        ui1s.GoToSubPanel(2101);
                        break;
                    case 12:
                        ui0s.OnBtnClickEvent(1);
                        await UniTask.Delay(200);
                        if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui2))
                            return;
                        var ui2s = ui2 as UIPanel_Equipment;
                        ui2s.GoToSubPanel(2102);
                        break;
                    case 13:
                        ui0s.OnBtnClickEvent(1);
                        await UniTask.Delay(200);
                        UIHelper.CreateAsync(UIType.UIPanel_Compound);
                        break;
                    case 21:

                        ui0s.OnBtnClickEvent(2);

                        break;
                    case 22:
                        ui0s.OnBtnClickEvent(2);
                        await UniTask.Delay(200);
                        if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Talent, out var ui3))
                            return;
                        var ui3s = ui3 as UIPanel_Talent;
                        ui3s.OpenPropPanel(1);

                        break;
                    case 23:

                        ui0s.OnBtnClickEvent(2);
                        await UniTask.Delay(200);
                        if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Talent, out var ui4))
                            return;
                        var ui4s = ui4 as UIPanel_Talent;
                        ui4s.OpenPropPanel(2);
                        break;
                }
            }
            else if (tagId == 3)
            {
                switch (last2num)
                {
                    case 91:
                        await UIHelper.CreateAsync(UIType.UIPanel_BuyEnergy);
                        break;
                }
            }
            else if (tagId == 4)
            {
                if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_Challege, out var ui0))
                    return;
                var ui0s = ui0 as UIPanel_Challege;
                switch (last2num)
                {
                    case 11:
                        ui0s.OnMainTreadClick();
                        break;
                    case 21:
                        ui0s.OnAreaTreadClick();
                        break;
                }
            }
            else if (tagId == 5)
            {
            }
        }


        public static void DestroyAllSubPanel()
        {
            var uim = XFramework.Common.Instance.Get<UIManager>();
            uim.DestroyAllSubPanel();
        }

        /// <summary>
        /// 跳转某个界面
        /// </summary>
        public static void GoToSomePanel(string gotoStr)
        {
            if (gotoStr.IsNullOrEmpty())
            {
                Log.Error($"gotoStr.IsNullOrEmpty()");
                return;
            }

            ExtractUnlockOrGoToStr(gotoStr, out int type, out List<int> paras);
            GoToPanel(type, paras[0]);

            if (type == 1)
            {
                var tagId = paras[0] / 100;
                //GoToPanel(type, paras[0]);
            }
            else if (type == 2)
            {
                var tagId = paras[0] / 10000;
                //GoToPanel(type, paras[0]);
            }
            else if (type == 3)
            {
                var tagId = paras[0] / 100;
                //GoToPanel(type, paras[0]);
            }
        }

        #endregion

        #region Guide 新手引导

        public static void EnableJiYuMask(bool enable)
        {
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var jiyuUI))
            {
                var KBg_JiYuMask = jiyuUI.GetFromReference(UIPanel_JiyuGame.KBg_JiYuMask);
                KBg_JiYuMask.SetActive(enable);
            }
        }


        /// <summary>
        /// 完成新手引导
        /// </summary>
        /// <param name="guideId"></param>
        public static bool TryFinishGuide(int guideId)
        {
            bool isFinished = false;
            if (guideId == 0)
            {
                return isFinished;
            }

            var tbguide = ConfigManager.Instance.Tables.Tbguide;
            var guide = tbguide.Get(guideId);
            var orderLastList = tbguide.DataList.Where(a => a.group == guide.group).OrderByDescending(s => s.order)
                .ToList();
            var lastGuide = orderLastList.OrderByDescending(s => s.id).ToList()[0];
            if (guide.id != lastGuide.id)
            {
                //Log.Debug($"guide.id:{guide.id} 不是组中最后一个 不能完成");
                return isFinished;
            }

            isFinished = true;
            var settingData = new SettingDate();
            settingData.GuideList.Add(guide.group);
            NetWorkManager.Instance.SendMessage(CMD.CHANGESETTINGS, settingData);
            Log.Debug($"完成引导组id:{guide.group} guide.id:{guide.id}是组中最后一个");
            if (ResourcesSingleton.Instance.settingData.GuideList.Contains(guide.group))
            {
                ResourcesSingleton.Instance.settingData.GuideList.Remove(guide.group);
            }

            return isFinished;
            // return;
            // var teamList = tbguide.DataList.Where(a => a.group == guide.group).ToList();
            //
            // int index = teamList.FindIndex(a => a.id == guide.id);
            // if (index != teamList.Count - 1)
            // {
            //     Log.Debug($"guide.id:{guide.id} 不是最后一个 不能完成");
            //     return;
            // }
            //
            // //tbguide.DataList[]
            // groupId = guide.group;
            //
            //
            // Log.Debug($"guide.group:{groupId} 完成");
            // var settingData = new SettingDate();
            // settingData.GuideList.Add(groupId);
            // NetWorkManager.Instance.SendMessage(CMD.CHANGESETTINGS, settingData);

            // if (ResourcesSingleton.Instance.settingData.GuideList.Contains(id))
            // {
            //     ResourcesSingleton.Instance.settingData.GuideList.Remove(id);
            //    
            // }
            //guide.templateId
        }

        /// <summary>
        /// 设置强引导类型 镂空UI
        /// </summary>
        /// <param name="itemUI">镂空的控件</param>
        /// <param name="hollowUI">覆盖在镂空控件上面的带Ximage组件的UI</param>
        public async static void SetForceGuideRectUI(UI itemUI, UI hollowUI)
        {
            var guidRect = hollowUI.GetXImage().Get().hollowArea;
            var parRect = hollowUI.GetRectTransform();
            await UniTask.Yield();
            //Log.Debug($"parRect {parRect.Width()} {parRect.Height()}");
            // 全屏矩形的定义
            var startRect = itemUI.GetRectTransform();

            guidRect = new Rect(JiYuUIHelper.GetUIPos(itemUI).x,
                JiYuUIHelper.GetUIPos(itemUI).y,
                startRect.Width(), startRect.Height());

            //guidRect = KBtn_Start.GetRectTransform().Get().rect;
            Log.Debug($"hollowUI {guidRect}");
            hollowUI.GetXImage().Get().hollowArea = guidRect;
            //Log.Debug($"hollowArea { KBg_TestGuid.GetXImage().Get().hollowArea }");
            Vector4 renderData = new Vector4(JiYuUIHelper.GetUIPos(itemUI).x,
                JiYuUIHelper.GetUIPos(itemUI).y,
                startRect.Width(), startRect.Height());
            hollowUI.GetXImage().Get().material.SetVector("_Rect", renderData);
            //hollowUI.SetActive(true);
        }

        #endregion

        public static bool TryGetActivityLink(int activityType, out int activityId, out int link)
        {
            var tbactivity = ConfigManager.Instance.Tables.Tbactivity;
            var tbmonopoly = ConfigManager.Instance.Tables.Tbmonopoly;
            link = 0;
            activityId = 0;
            var list = ResourcesSingleton.Instance.activity.activityMap.ActivityMap_.Where((a) =>
            {
                return tbactivity.Get(a.Key).type == activityType;
            }).ToList();

            if (list.Count > 0)
            {
                var activity = tbactivity.Get(list[0].Key);
                link = activity.link;
                activityId = activity.id;
                return true;
            }

            return false;
        }

        public static bool IsCompositeEquipReward(Vector3 reward)
        {
            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;

            if (rewardx == 11 && (rewardy / 100) % 100 == 0)
                return true;
            return false;
        }

        public static bool IsCompositeEquipReward(int equipId)
        {
            if ((equipId / 100) % 100 == 0)
                return true;
            return false;
        }

        public static bool IsCompositeEquipReward(EquipDto equipDto)
        {
            if (equipDto.EquipId % 100 == 0)
                return true;
            return false;
        }

        public static int GetBindingLevel(int exp)
        {
            var player_skill_binding_rank = ConfigManager.Instance.Tables.Tbskill_binding_rank;
            if (exp < player_skill_binding_rank.Get(1).exp)
            {
                return 0;
            }

            for (int i = 1; i < player_skill_binding_rank.DataList.Count - 1; i++)
            {
                var item = player_skill_binding_rank.DataList[i];
                var itemNext = player_skill_binding_rank.DataList[i + 1];
                if (exp < itemNext.exp)
                {
                    return item.id;
                }
            }

            return player_skill_binding_rank.DataList[player_skill_binding_rank.DataList.Count - 1].id;
        }

        public static bool TryGetCompositeEquipRewardCount(Vector3 reward)
        {
            //var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;

            // newRewardz = rewardz;
            // count = rewardz;
            if (IsCompositeEquipReward(reward))
            {
                // if (rewardz > equip_quality.DataList.Count)
                // {
                //     newRewardz = CmdHelper.GetCmd(rewardz);
                //     count = CmdHelper.GetSubCmd(rewardz);
                // }

                return true;
            }


            return false;
        }

        public static bool IsMailRedDot()
        {
            bool isRedDot = false;
            foreach (var mail in ResourcesSingleton.Instance.mails)
            {
                if (mail.Status != 1)
                {
                    isRedDot = true;
                    break;
                }
            }

            return isRedDot;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitAudioSettings()
        {
            for (int i = 0; i < 5; i++)
            {
                JiYuUIHelper.InitEnableSettings(i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InitEnableSettings(int index, bool toSetEnable = false)
        {
            bool enable = false;
            switch (index)
            {
                case 0:
                    if (toSetEnable)
                    {
                        ResourcesSingleton.Instance.settingData.EnableBgm =
                            !ResourcesSingleton.Instance.settingData.EnableBgm;
                    }

                    if (ResourcesSingleton.Instance.settingData.EnableBgm)
                    {
                        enable = true;
                        AudioManager.Instance.SetFModBgmMute(false);
                        AudioManager.Instance.SetFModSFXMute(false);
                    }
                    else
                    {
                        AudioManager.Instance.SetFModBgmMute(true);
                        AudioManager.Instance.SetFModSFXMute(true);
                    }


                    break;
                case 1:
                    if (toSetEnable)
                    {
                        ResourcesSingleton.Instance.settingData.EnableFx =
                            !ResourcesSingleton.Instance.settingData.EnableFx;
                    }

                    if (ResourcesSingleton.Instance.settingData.EnableFx)
                    {
                        enable = true;
                        AudioManager.Instance.SetFModBgmMute(false);
                    }
                    else
                    {
                        AudioManager.Instance.SetFModBgmMute(true);
                    }


                    break;
                case 2:
                    if (toSetEnable)
                    {
                        ResourcesSingleton.Instance.settingData.EnableShock =
                            !ResourcesSingleton.Instance.settingData.EnableShock;
                    }

                    if (ResourcesSingleton.Instance.settingData.EnableShock)
                    {
                        enable = true;
                    }


                    break;
                case 3:
                    if (toSetEnable)
                    {
                        ResourcesSingleton.Instance.settingData.EnableWeakEffect =
                            !ResourcesSingleton.Instance.settingData.EnableWeakEffect;
                    }

                    if (ResourcesSingleton.Instance.settingData.EnableWeakEffect)
                    {
                        enable = true;
                    }


                    break;
                case 4:
                    if (toSetEnable)
                    {
                        ResourcesSingleton.Instance.settingData.EnableShowStick =
                            !ResourcesSingleton.Instance.settingData.EnableShowStick;
                    }

                    if (ResourcesSingleton.Instance.settingData.EnableShowStick)
                    {
                        enable = true;
                    }


                    break;
            }

            return enable;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetFrameRate(bool isMax = false, int targetFrameRate = 60)
        {
            if (isMax)
            {
#if UNITY_ANDROID || UNITY_IOS
                int maxRefreshRate = Screen.currentResolution.refreshRate;
                Application.targetFrameRate = maxRefreshRate;
#else
                 Application.targetFrameRate = -1;
#endif

                return;
            }

            Application.targetFrameRate = targetFrameRate;
        }

        #region Property

        /// <summary>
        /// 后端转前端战斗内属性
        /// </summary>
        /// <param name="playerData"></param>
        /// <param name="chaStates"></param>
        /// <param name="mybattleProperty"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitPlayerProperty(ref PlayerData playerData, ref ChaStats chaStates,
            BattleProperty mybattleProperty)
        {
            //Log.Debug($"InitPlayerProperty", Color.red);
            //var chaStates = new ChaStats { };
            foreach (var property in mybattleProperty.Properties)
            {
                var strings = property.Split(";");
                var id = int.Parse(strings[0]);
                var value = int.Parse(strings[1]);
                id += 100000;
                //Log.Debug($"Properties:{property}", Color.green);
                UnityHelper.InitProperty(ref playerData, ref chaStates, id, value);
            }

            //playerData.chaProperty = chaStates.chaProperty;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WearOrUnWearEquipProperty(EquipDto gameEquip, bool isWearEquip = true)
        {
            int equipLevel = gameEquip.EquipLevel;


            var tbequip_data = ConfigManager.Instance.Tables.Tbequip_data;
            var tbequip_level = ConfigManager.Instance.Tables.Tbequip_level;

            var tbskill_effectNew = ConfigManager.Instance.Tables.Tbskill_effectNew;
            int equipId = gameEquip.EquipId * 100 + gameEquip.Quality;
            var equip_data = tbequip_data.Get(equipId);

            var defaultAttr = equip_data.mainEntryInit + (equipLevel - 1) * equip_data.mainEntryGrow;

            defaultAttr = isWearEquip ? defaultAttr : -defaultAttr;
            //Dictionary<int, int> attrIdValues = new Dictionary<int, int>();
            var attrIdValues = ResourcesSingleton.Instance.mainProperty;

            if (attrIdValues.ContainsKey(equip_data.mainEntryId))
            {
                attrIdValues[equip_data.mainEntryId] += defaultAttr;
            }
            else
            {
                attrIdValues.Add(equip_data.mainEntryId, defaultAttr);
            }


            foreach (var skillId in equip_data.unlockSkillGroup)
            {
                var skillEffectNew = tbskill_effectNew.DataList.Where(a =>
                        a.skillId == skillId &&
                        a.effectType == 3)
                    .ToList();

                foreach (var skillEffect in skillEffectNew)
                {
                    int attrId = skillEffect.attrId;
                    int value = skillEffect.attrIdPara[0];
                    value = isWearEquip ? value : -value;
                    if (attrIdValues.ContainsKey(attrId))
                    {
                        attrIdValues[attrId] += value;
                    }
                    else
                    {
                        attrIdValues.Add(attrId, value);
                    }
                }
            }

            RefreshPlayerPropertyEquipPanelUI();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChangeEquipProperty(EquipDto oldGameEquip, EquipDto newGameEquip)
        {
            WearOrUnWearEquipProperty(oldGameEquip, false);
            WearOrUnWearEquipProperty(newGameEquip, true);
            RefreshPlayerPropertyEquipPanelUI();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RefreshPlayerPropertyEquipPanelUI()
        {
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
            {
                var uiequip = ui as UIPanel_Equipment;
                uiequip.RefreshPlayerProperty();
                //Log.Error($"is");
            }
        }

        #endregion


        public enum DefaultRectType
        {
            /// <summary>
            /// 默认居中
            /// </summary>
            Normal,

            /// <summary>
            /// 延展
            /// </summary>
            Expand
        }

        /// <summary>
        /// 尝试拿到当前页面的UI脚本
        /// </summary>
        /// <param name="uiType">ui类型字符串</param>
        /// <param name="ui">ui脚本</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetUI(string uiType, out UI ui)
        {
            ui = null;
            var uiManager = XFramework.Common.Instance?.Get<UIManager>();
            if (uiManager.TryGet(uiType, out var ui0))
            {
                ui = ui0;
                return true;
            }

            //Log.Debug($"没有拿到UI");
            return false;
        }

        /// <summary>
        /// 弃用
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetMaterialDic()
        {
            // ResourcesSingleton.Instance.equipmentData.isMaterials.Clear();
            // var materialDic = new Dictionary<Vector3, int>();
            // foreach (var item in ResourcesSingleton.Instance.equipmentData.equipments)
            // {
            //     if (item.Value.equip.EquipId % 100 == 0)
            //     {
            //         //Log.Error($"{item.Value.equip}");
            //         var reward = new Vector3(11, item.Value.equip.EquipId, item.Value.equip.Quality);
            //         //如果有相同种类的通用合成材料,则数量加1
            //         if (materialDic.ContainsKey(reward))
            //             materialDic[reward]++;
            //         //如果没有,则视做新装备
            //         else
            //         {
            //             materialDic.Add(reward, 1);
            //         }
            //     }
            // }

            //ResourcesSingleton.Instance.equipmentData.isMaterials = materialDic;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortRewards(List<Vector3> rewards)
        {
            rewards.Sort(new RewardsComparer());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MergeRewardList(List<Vector3> rewards)
        {
            //Log.Error($"MergeRewardList start");
            Dictionary<Vector3, int> vector3CountDict = new Dictionary<Vector3, int>();

            foreach (Vector3 vector3 in rewards)
            {
                int rewardX = (int)vector3.x;
                int rewardY = (int)vector3.y;
                int rewardZ = (int)vector3.z;

                if (rewardX == 11 && !JiYuUIHelper.IsCompositeEquipReward(vector3))
                    continue;
                if (rewardX == 11)
                {
                    // 如果字典中已经包含这个Vector3元素，增加其数量
                    if (vector3CountDict.ContainsKey(vector3))
                    {
                        vector3CountDict[vector3]++;
                    }
                    else
                    {
                        // 如果字典中没有包含这个Vector3元素，将其添加到字典，并设置数量为1
                        vector3CountDict.Add(vector3, 1);
                    }
                }
                else
                {
                    if (vector3CountDict.ContainsKey(new Vector3(rewardX, rewardY, 0)))
                    {
                        vector3CountDict[new Vector3(rewardX, rewardY, 0)] += rewardZ;
                    }
                    else
                    {
                        // 如果字典中没有包含这个Vector3元素，将其添加到字典，并设置数量为1
                        vector3CountDict.Add(new Vector3(rewardX, rewardY, 0), rewardZ);
                    }
                }
            }

            rewards.RemoveAll(vector =>
            {
                int rewardX = (int)vector.x;
                int rewardY = (int)vector.y;
                int rewardZ = (int)vector.z;
                if (rewardX == 11)
                {
                    return vector3CountDict.ContainsKey(vector);
                }
                else
                {
                    return vector3CountDict.ContainsKey(new Vector3(rewardX, rewardY, 0));
                }
            });

            foreach (var keyValue in vector3CountDict)
            {
                if (JiYuUIHelper.IsCompositeEquipReward(keyValue.Key))
                {
                    var mergeZ = CmdHelper.GetMergeCmd((int)keyValue.Key.z, keyValue.Value);

                    rewards.Add(new Vector3((int)keyValue.Key.x, (int)keyValue.Key.y, mergeZ));
                }
                else
                {
                    rewards.Add(new Vector3((int)keyValue.Key.x, (int)keyValue.Key.y, keyValue.Value));
                }
            }

            vector3CountDict.Clear();
            vector3CountDict = null;
        }

        /// <summary>
        /// 设置基于Equip的common_EquipItem类预制件的点击弹板
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <param name="common_RewardItem">创建的common_RewardItem的UI</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetEquipOnClick(long uid, UI common_EquipItem)
        {
            //var btnui = common_EquipItem as UICommon_EquipItem;
            var btn = common_EquipItem.GetFromReference(UICommon_EquipItem.KBtn_Item);

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, async () =>
            {
                Log.Debug($"SetEquipOnClick uid {uid}", Color.green);
                // if (TryGetUI(UIType.UIPanel_Equipment, out var ui))
                // {
                //     var btnui = common_EquipItem as UICommon_EquipItem;
                //     var uiequip = ui as UIPanel_Equipment;
                //     uiequip.OnDestoryAllTips();
                //     // if (uiequip.lastClickTipItem != null)
                //     // {
                //     //     var lastselected =
                //     //         uiequip?.lastClickTipItem?.GetFromReference(UICommon_EquipItem.KImg_Selected);
                //     //     lastselected?.SetActive(false);
                //     // }
                //
                //     //uiequip.lastClickTipItem = btnui;
                // }

                DestoryAllTips();
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
                {
                    var uiequip = ui as UIPanel_Equipment;
                    uiequip.DestorySelected();
                    //uiequip.lastClickTipItem = btnui;
                }

                if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(uid,
                        out var equip))
                {
                    Log.Debug($"!ResourcesSingleton");
                    return;
                }

                var tipUI = await UIHelper.CreateAsync(UIType.UIPanel_EquipTips, equip) as
                    UICommon_EquipTips;
                var isSelected = common_EquipItem.GetFromReference(UICommon_EquipItem.KImg_Selected);
                isSelected.SetActive(true);
                //TODO:setPos
            });
            // btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
            // {
            //     DestoryAllTips();
            //     if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(uid,
            //             out var equip))
            //     {
            //         return;
            //     }
            //
            //     var tipUI = await UIHelper.CreateAsync(UIType.UICommon_EquipTips, equip) as
            //         UICommon_EquipTips;
            //
            //     //TODO:setPos
            // }));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetEquipOnClick(UI common_EquipItem)
        {
            var uis = common_EquipItem as UICommon_EquipItem;
            var btn = common_EquipItem.GetFromReference(UICommon_EquipItem.KBtn_Item);

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, async () =>
            {
                //Log.Debug($"SetEquipOnClick uid {uid}", Color.green);
                // if (TryGetUI(UIType.UIPanel_Equipment, out var ui))
                // {
                //     var btnui = common_EquipItem as UICommon_EquipItem;
                //     var uiequip = ui as UIPanel_Equipment;
                //     uiequip.OnDestoryAllTips();
                //     // if (uiequip.lastClickTipItem != null)
                //     // {
                //     //     var lastselected =
                //     //         uiequip?.lastClickTipItem?.GetFromReference(UICommon_EquipItem.KImg_Selected);
                //     //     lastselected?.SetActive(false);
                //     // }
                //
                //     //uiequip.lastClickTipItem = btnui;
                // }

                DestoryAllTips();
                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Equipment, out var ui))
                {
                    var uiequip = ui as UIPanel_Equipment;
                    uiequip.DestorySelected();
                    //uiequip.lastClickTipItem = btnui;
                }

                if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(uis.uid,
                        out var equip))
                {
                    Log.Debug($"!ResourcesSingleton");
                    return;
                }

                var tipUI = await UIHelper.CreateAsync(UIType.UIPanel_EquipTips, equip) as
                    UICommon_EquipTips;
                var isSelected = common_EquipItem.GetFromReference(UICommon_EquipItem.KImg_Selected);
                isSelected.SetActive(true);
                //TODO:setPos
            });
            // btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
            // {
            //     DestoryAllTips();
            //     if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(uid,
            //             out var equip))
            //     {
            //         return;
            //     }
            //
            //     var tipUI = await UIHelper.CreateAsync(UIType.UICommon_EquipTips, equip) as
            //         UICommon_EquipTips;
            //
            //     //TODO:setPos
            // }));
        }
        /// <summary>
        /// 设置基于Equip的common_EquipItem类预制件的点击弹板
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <param name="common_RewardItem">创建的common_RewardItem的UI</param>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static void SetWearedEquipOnClick(Dictionary<int, UICommon_EquipItem> wearEquipsDic, int posId, UI ui)
        // {
        //     var btn = ui.GetFromReference(UICommon_EquipItem.KBtn_Item);
        //
        //     JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
        //     {
        //         if (!wearEquipsDic.TryGetValue(posId, out var ui))
        //         {
        //             return;
        //         }
        //
        //         DestoryAllTips();
        //         if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(ui.uid,
        //                 out var equip))
        //         {
        //             return;
        //         }
        //
        //         var tipUI = await UIHelper.CreateAsync(UIType.UICommon_EquipTips, equip) as
        //             UICommon_EquipTips;
        //
        //         //TODO:setPos
        //     }));
        //     // btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
        //     // {
        //     //     DestoryAllTips();
        //     //     if (!ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(uid,
        //     //             out var equip))
        //     //     {
        //     //         return;
        //     //     }
        //     //
        //     //     var tipUI = await UIHelper.CreateAsync(UIType.UICommon_EquipTips, equip) as
        //     //         UICommon_EquipTips;
        //     //
        //     //     //TODO:setPos
        //     // }));
        // }

        //
        /// <summary>
        ///已弃用 销毁某个父节点下的所有子节点,一般用于公共UI的销毁
        /// </summary>
        /// <param name="parent">某个父级节点</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyAllChildren(UI parent)
        {
            // foreach (var item in UICommon_Labels)
            // {
            //     //UnityEngine.GameObject.DestroyImmediate(item.GameObject);
            // }
            var parentGo = parent.GameObject;
            int childCount = parentGo.transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject child = parentGo.transform.GetChild(i).gameObject;
                UnityEngine.GameObject.Destroy(child);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EquipUIComparer(UI obj11, UI obj21)
        {
            var obj111 = obj11 as UICommon_EquipItem;
            var obj211 = obj21 as UICommon_EquipItem;
            ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(obj111.uid, out var obj100);
            ResourcesSingleton.Instance.equipmentData.equipments.TryGetValue(obj211.uid, out var obj200);
            var obj1 = obj100.equip;
            var obj2 = obj200.equip;
            var equipData = ConfigManager.Instance.Tables.Tbequip_data;
            int equipId1 = obj1.EquipId * 100 + obj1.Quality;
            int equipId2 = obj2.EquipId * 100 + obj2.Quality;
            var equipobj1 = equipData.Get(equipId1);
            var equipobj2 = equipData.Get(equipId2);

            //ResourcesSingleton.Instance.equipmentData.isCompoundSort = isCompoundSort;
            if (ResourcesSingleton.Instance.equipmentData.isCompoundSort)
            {
                // 
                if (obj100.isWearing && !obj200.isWearing)
                    return -1;
                else if (!obj100.isWearing && obj200.isWearing)
                    return 1;
                // 
                if (obj100.canCompound && !obj200.canCompound)
                    return -1;
                else if (!obj100.canCompound && obj200.canCompound)
                    return 1;
            }

            // 品质由高到低
            if (obj1.Quality > obj2.Quality)
                return -1;
            else if (obj1.Quality < obj2.Quality)
                return 1;

            // S在前，普通在后
            if (equipobj1.sYn == 1 && equipobj2.sYn == 0)
                return -1;
            else if (equipobj1.sYn == 0 && equipobj2.sYn == 1)
                return 1;

            // 部位id由小到大
            if (equipobj1.posId < equipobj2.posId)
                return -1;
            else if (equipobj1.posId > equipobj2.posId)
                return 1;

            // 等级由高到低
            if (obj1.EquipLevel > obj2.EquipLevel)
                return -1;
            else if (obj1.EquipLevel < obj2.EquipLevel)
                return 1;

            // 装备id由大到小
            if (obj1.EquipId > obj2.EquipId)
                return -1;
            else if (obj1.EquipId < obj2.EquipId)
                return 1;

            // uid从小到大
            if (obj1.PartId < obj2.PartId)
                return -1;
            else if (obj1.PartId > obj2.PartId)
                return 1;

            return 0;
        }

        #region Comparer

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RewardUIComparer(UI obj11, UI obj21)
        {
            Tblanguage language = ConfigManager.Instance.Tables.Tblanguage;
            Tbuser_variable user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            Tbitem item = ConfigManager.Instance.Tables.Tbitem;
            Tbequip_data equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            Tbequip_quality equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            Tbquality quality = ConfigManager.Instance.Tables.Tbquality;

            var obj111 = obj11 as UICommon_RewardItem;
            var obj211 = obj21 as UICommon_RewardItem;
            var obj1 = obj111.trueReward;
            var obj2 = obj211.trueReward;
            var obj1rewardx = (int)obj1.x;
            var obj1rewardy = (int)obj1.y;
            var obj1rewardz = (int)obj1.z;
            var obj2rewardx = (int)obj2.x;
            var obj2rewardy = (int)obj2.y;
            var obj2rewardz = (int)obj2.z;

            if (obj1rewardx == 11 && obj2rewardx != 11)
                return -1;
            else if (obj1rewardx != 11 && obj2rewardx == 11)
                return 1;

            if (IsResourceReward(obj1) && !IsResourceReward(obj2))
                return -1;
            else if (!IsResourceReward(obj1) && IsResourceReward(obj2))
                return 1;

            // if (obj1rewardx != 5 && obj2rewardx == 5)
            //     return -1;
            // else if (obj1rewardx == 5 && obj2rewardx != 5)
            //     return 1;

            if (obj1rewardx == 11 && obj2rewardx == 11)
            {
                if (!IsCompositeEquipReward(obj1) &&
                    IsCompositeEquipReward(obj2))
                    return -1;
                else if (IsCompositeEquipReward(obj1) &&
                         !IsCompositeEquipReward(obj2))
                    return 1;

                if (equip_data.Get(obj1rewardy).quality >
                    equip_data.Get(obj2rewardy).quality)
                    return -1;
                else if (equip_data.Get(obj1rewardy).quality <
                         equip_data.Get(obj2rewardy).quality)
                    return 1;

                if (equip_data.Get(obj1rewardy).sYn == 1 &&
                    equip_data.Get(obj2rewardy).sYn != 1)
                    return -1;
                else if (equip_data.Get(obj1rewardy).sYn != 1 &&
                         equip_data.Get(obj2rewardy).sYn == 1)
                    return 1;


                if (equip_data.Get(obj1rewardy).posId <
                    equip_data.Get(obj2rewardy).posId)
                    return -1;
                else if (equip_data.Get(obj1rewardy).posId >
                         equip_data.Get(obj2rewardy).posId)
                    return 1;

                if (obj1rewardy > obj2rewardy)
                    return -1;
                else if (obj1rewardy < obj2rewardy)
                    return 1;
            }

            if (IsResourceReward(obj1) && IsResourceReward(obj2))
            {
                if (obj1rewardx < obj2rewardx)
                    return -1;
                else if (obj1rewardx > obj2rewardx)
                    return 1;
            }

            if (obj1rewardx == 5 && obj2rewardx == 5)
            {
                if (item.Get(obj1rewardy).sort < item.Get(obj2rewardy).sort)
                    return -1;
                else if (item.Get(obj1rewardy).sort > item.Get(obj2rewardy).sort)
                    return 1;
                if (obj1rewardy < obj2rewardy)
                    return -1;
                else if (obj1rewardy > obj2rewardy)
                    return 1;
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SelectedBoxItemUIComparer(UI obj11, UI obj21)
        {
            Tblanguage language = ConfigManager.Instance.Tables.Tblanguage;
            Tbuser_variable user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            Tbitem item = ConfigManager.Instance.Tables.Tbitem;
            Tbequip_data equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            Tbequip_quality equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            Tbquality quality = ConfigManager.Instance.Tables.Tbquality;

            var obj111 = obj11 as UISubPanel_SelectBoxItem;
            var obj211 = obj21 as UISubPanel_SelectBoxItem;
            var obj1 = obj111.reward;
            var obj2 = obj211.reward;
            var obj1rewardx = (int)obj1.x;
            var obj1rewardy = (int)obj1.y;
            var obj1rewardz = (int)obj1.z;
            var obj2rewardx = (int)obj2.x;
            var obj2rewardy = (int)obj2.y;
            var obj2rewardz = (int)obj2.z;

            if (obj1rewardx == 11 && obj2rewardx != 11)
                return -1;
            else if (obj1rewardx != 11 && obj2rewardx == 11)
                return 1;

            if (IsResourceReward(obj1) && !IsResourceReward(obj2))
                return -1;
            else if (!IsResourceReward(obj1) && IsResourceReward(obj2))
                return 1;

            // if (obj1rewardx != 5 && obj2rewardx == 5)
            //     return -1;
            // else if (obj1rewardx == 5 && obj2rewardx != 5)
            //     return 1;

            if (obj1rewardx == 11 && obj2rewardx == 11)
            {
                if (!IsCompositeEquipReward(obj1) &&
                    IsCompositeEquipReward(obj2))
                    return -1;
                else if (IsCompositeEquipReward(obj1) &&
                         !IsCompositeEquipReward(obj2))
                    return 1;

                if (equip_data.Get(obj1rewardy).quality >
                    equip_data.Get(obj2rewardy).quality)
                    return -1;
                else if (equip_data.Get(obj1rewardy).quality <
                         equip_data.Get(obj2rewardy).quality)
                    return 1;

                if (equip_data.Get(obj1rewardy).sYn == 1 &&
                    equip_data.Get(obj2rewardy).sYn != 1)
                    return -1;
                else if (equip_data.Get(obj1rewardy).sYn != 1 &&
                         equip_data.Get(obj2rewardy).sYn == 1)
                    return 1;


                if (equip_data.Get(obj1rewardy).posId <
                    equip_data.Get(obj2rewardy).posId)
                    return -1;
                else if (equip_data.Get(obj1rewardy).posId >
                         equip_data.Get(obj2rewardy).posId)
                    return 1;

                if (obj1rewardy > obj2rewardy)
                    return -1;
                else if (obj1rewardy < obj2rewardy)
                    return 1;
            }

            if (IsResourceReward(obj1) && IsResourceReward(obj2))
            {
                if (obj1rewardx < obj2rewardx)
                    return -1;
                else if (obj1rewardx > obj2rewardx)
                    return 1;
            }

            if (obj1rewardx == 5 && obj2rewardx == 5)
            {
                if (item.Get(obj1rewardy).sort < item.Get(obj2rewardy).sort)
                    return -1;
                else if (item.Get(obj1rewardy).sort > item.Get(obj2rewardy).sort)
                    return 1;
                if (obj1rewardy < obj2rewardy)
                    return -1;
                else if (obj1rewardy > obj2rewardy)
                    return 1;
            }

            return 0;
        }

        private class EquipComparer00 : IComparer<KeyValuePair<long, MyGameEquip>>
        {
            public int Compare(KeyValuePair<long, MyGameEquip> long1, KeyValuePair<long, MyGameEquip> long2)
            {
                var obj1 = long1.Value;
                var obj2 = long2.Value;
                var equipData = ConfigManager.Instance.Tables.Tbequip_data;
                int obj1EquipId = obj1.equip.EquipId * 100 + obj1.equip.Quality;
                int obj2EquipId = obj2.equip.EquipId * 100 + obj2.equip.Quality;

                var equipobj1 = equipData.Get(obj1EquipId);
                var equipobj2 = equipData.Get(obj2EquipId);

                if (ResourcesSingleton.Instance.equipmentData.isCompoundSort)
                {
                    // 
                    if (obj1.isWearing && !obj2.isWearing)
                        return -1;
                    else if (!obj1.isWearing && obj2.isWearing)
                        return 1;
                    // 
                    if (obj1.canCompound && !obj2.canCompound)
                        return -1;
                    else if (!obj1.canCompound && obj2.canCompound)
                        return 1;
                }

                // 品质由高到低
                if (obj1.equip.Quality > obj2.equip.Quality)
                    return -1;
                else if (obj1.equip.Quality < obj2.equip.Quality)
                    return 1;

                // S在前，普通在后
                if (equipobj1.sYn == 1 && equipobj2.sYn == 0)
                    return -1;
                else if (equipobj1.sYn == 0 && equipobj2.sYn == 1)
                    return 1;

                // 部位id由小到大
                if (equipobj1.posId < equipobj2.posId)
                    return -1;
                else if (equipobj1.posId > equipobj2.posId)
                    return 1;

                // 等级由高到低
                if (obj1.equip.EquipLevel > obj2.equip.EquipLevel)
                    return -1;
                else if (obj1.equip.EquipLevel < obj2.equip.EquipLevel)
                    return 1;

                // 装备id由大到小
                if (obj1.equip.EquipId > obj2.equip.EquipId)
                    return -1;
                else if (obj1.equip.EquipId < obj2.equip.EquipId)
                    return 1;

                // uid从小到大
                if (obj1.equip.PartId < obj2.equip.PartId)
                    return -1;
                else if (obj1.equip.PartId > obj2.equip.PartId)
                    return 1;

                return 0;
            }
        }

        #endregion

        /// <summary>
        /// 强制刷新UI布局
        /// </summary>
        /// <param name="parent">UI父节点</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForceRefreshLayout(UI parent)
        {
            if (parent == null)
            {
                Log.Error($"parent is null");
                return;
            }

            RectTransform rectTransform = parent.GetComponent<RectTransform>();

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortEquipments(bool isCompoundSort = false)
        {
            ResourcesSingleton.Instance.equipmentData.isCompoundSort = isCompoundSort;

            List<KeyValuePair<long, MyGameEquip>> equipList =
                ResourcesSingleton.Instance.equipmentData.equipments.ToList();

            ResourcesSingleton.Instance.equipmentData.equipments.Clear();
            equipList.Sort(new EquipComparer00());
            foreach (var VARIABLE in equipList)
            {
                if (!ResourcesSingleton.Instance.equipmentData.equipments.TryAdd(VARIABLE.Value.equip.PartId,
                        VARIABLE.Value))
                {
                    Log.Debug($"{VARIABLE.Value.equip.PartId}is not exist", Color.cyan);
                }
            }

            equipList.Clear();
        }

        /// <summary>
        /// 判断指定equip是否可以合成
        /// </summary>
        /// <param name="equip"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetCanCompound()
        {
            //var tempDic = new List<long>();
            foreach (var kv in ResourcesSingleton.Instance.equipmentData.equipments)
            {
                var myGameEquip = kv.Value;
                //var myGameEquip = ResourcesSingleton.Instance.equipmentData.equipments;
                var equip = myGameEquip.equip;

                long equipUid = equip.PartId;
                int equipId = equip.EquipId;
                int equipQuality = equip.Quality;
                int equipLevel = equip.EquipLevel;

                var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
                var user_variblesConfig = ConfigManager.Instance.Tables.Tbuser_variable;
                var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
                var equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;
                var equip_posConfig = ConfigManager.Instance.Tables.Tbequip_pos;
                var qualityConfig = ConfigManager.Instance.Tables.Tbquality;

                var newequipId = equipId * 100 + equipQuality;

                var equip_data = equip_dataConfig.Get(newequipId);
                var equip_quality = equip_qualityConfig.Get(equip_data.quality);
                var quality = qualityConfig.Get(equip_quality.type);
                var equip_pos = equip_posConfig.Get(equip_data.posId);


                bool canCompound = false;
                var needList = equip_quality.mergeRule;
                if (needList.Count <= 0)
                {
                    myGameEquip.canCompound = false;
                    continue;
                }

                var needQua = needList[0];
                var needCount = needList[1];

                if (equip_quality.mergeSelf == 1)
                {
                    var count =
                        ResourcesSingleton.Instance.equipmentData.equipments.Count(kv =>
                            kv.Value.equip.EquipId == equipId &&
                            kv.Value.equip.Quality == needQua &&
                            equip_dataConfig.Get(kv.Value.equip.EquipId * 100 + kv.Value.equip.Quality).posId ==
                            equip_data.posId && kv.Value.equip.PartId != equipUid &&
                            equip_dataConfig.Get(kv.Value.equip.EquipId * 100 + kv.Value.equip.Quality).sYn != 1);

                    if (count >= needCount)
                    {
                        myGameEquip.canCompound = true;
                    }
                    else
                    {
                        myGameEquip.canCompound = false;
                    }
                }
                else
                {
                    var count =
                        ResourcesSingleton.Instance.equipmentData.equipments.Count(kv =>
                            kv.Value.equip.Quality == needQua &&
                            equip_dataConfig.Get(kv.Value.equip.EquipId * 100 + kv.Value.equip.Quality).posId ==
                            equip_data.posId && kv.Value.equip.PartId != equipUid &&
                            equip_dataConfig.Get(kv.Value.equip.EquipId * 100 + kv.Value.equip.Quality).sYn != 1);


                    if (count >= needCount)
                    {
                        myGameEquip.canCompound = true;
                    }
                    else
                    {
                        myGameEquip.canCompound = false;
                    }
                }

                //ResourcesSingleton.Instance.equipmentData.equipments[equipUid] = myGameEquip;
            }
        }


        /// <summary>
        /// 设置对应的装备item
        /// </summary>
        /// <param name="equip">reward串</param>
        /// <param name="common_EquipItem">对应icon的ui</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetEquipIcon(MyGameEquip myGameEquip, UI common_EquipItem,
            UIPanel_Equipment.EquipPanelType panelType = UIPanel_Equipment.EquipPanelType.Main)
        {
            long equipUid = myGameEquip.equip.PartId;
            int equipId = myGameEquip.equip.EquipId;
            int equipQuality = myGameEquip.equip.Quality;
            int equipLevel = myGameEquip.equip.EquipLevel;
            equipId = myGameEquip.equip.EquipId * 100 + equipQuality;

            //Log.Debug($"EquipDto:{myGameEquip.equip}", Color.green);

            var languageConfig = ConfigManager.Instance.Tables.Tblanguage;
            var user_variblesConfig = ConfigManager.Instance.Tables.Tbuser_variable;
            var equip_dataConfig = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_qualityConfig = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_posConfig = ConfigManager.Instance.Tables.Tbequip_pos;
            var qualityConfig = ConfigManager.Instance.Tables.Tbquality;

            var KBtn_Item = common_EquipItem.GetFromReference(UICommon_EquipItem.KBtn_Item);
            var KBg_Item = common_EquipItem.GetFromReference(UICommon_EquipItem.KBg_Item);
            var KImg_ItemIcon = common_EquipItem.GetFromReference(UICommon_EquipItem.KImg_ItemIcon);
            var KTxt_ItemCount = common_EquipItem.GetFromReference(UICommon_EquipItem.KText_ItemCount);
            var KIsItem = common_EquipItem.GetFromReference(UICommon_EquipItem.KIsItem);
            var KIsEquip = common_EquipItem.GetFromReference(UICommon_EquipItem.KIsEquip);
            var KImg_Pos = common_EquipItem.GetFromReference(UICommon_EquipItem.KImg_Pos);
            var KImg_PosIcon = common_EquipItem.GetFromReference(UICommon_EquipItem.KImg_PosIcon);
            var KImg_IsS = common_EquipItem.GetFromReference(UICommon_EquipItem.KImg_IsS);
            var KImg_Quality = common_EquipItem.GetFromReference(UICommon_EquipItem.KImg_Quality);
            var KText_Quality = common_EquipItem.GetFromReference(UICommon_EquipItem.KText_Quality);
            var KImg_Mask = common_EquipItem.GetFromReference(UICommon_EquipItem.KImg_Mask);
            var KImg_MaskIcon = common_EquipItem.GetFromReference(UICommon_EquipItem.KImg_MaskIcon);
            var KIsWearing = common_EquipItem.GetFromReference(UICommon_EquipItem.KIsWearing);
            var KText_IsWearing = common_EquipItem.GetFromReference(UICommon_EquipItem.KText_IsWearing);
            var KImg_RedDot = common_EquipItem.GetFromReference(UICommon_EquipItem.KImg_RedDot);
            var KText_Grade = common_EquipItem.GetFromReference(UICommon_EquipItem.KText_Grade);
            var KImg_Selected = common_EquipItem.GetFromReference(UICommon_EquipItem.KImg_Selected);
            var KAphla = common_EquipItem.GetFromReference(UICommon_EquipItem.KAphla);

            var itemRect = KBtn_Item.GetRectTransform();
            itemRect.SetOffsetWithTop(0);
            itemRect.SetOffsetWithBottom(0);
            itemRect.SetOffsetWithRight(0);
            itemRect.SetOffsetWithLeft(0);
            KIsEquip.SetActive(true);
            KIsItem.SetActive(false);
            KImg_Selected.SetActive(false);
            KAphla.SetActive(false);
            var equip_data = equip_dataConfig.Get(equipId);
            var equip_quality = equip_qualityConfig.Get(equip_data.quality);
            var quality = qualityConfig.Get(equip_quality.type);
            var equip_pos = equip_posConfig.Get(equip_data.posId);


            KImg_ItemIcon.GetImage().SetSpriteAsync(equip_data.icon, false).Forget();

            KBg_Item.SetActive(true);
            if (myGameEquip.equip.EquipId % 100 == 0)
            {
                KBg_Item.GetImage().SetSpriteAsync(quality.frame, false).Forget();
                KImg_Pos.SetActive(false);
                KImg_IsS.SetActive(false);
                KImg_Quality.SetActive(false);
                KText_Grade.SetActive(false);
                KIsWearing.SetActive(false);
                KImg_RedDot.SetActive(false);
                return;
            }
            else
            {
                KImg_Pos.SetActive(true);
                KImg_IsS.SetActive(true);
                KImg_Quality.SetActive(true);
                KText_Grade.SetActive(true);
                KIsWearing.SetActive(true);
                KImg_RedDot.SetActive(true);
            }


            KText_Grade.GetTextMeshPro().SetTMPText($"lv.{equipLevel}");
            KImg_Pos.GetImage().SetSpriteAsync(quality.posFrame, false).Forget();
            KImg_PosIcon.GetImage().SetSpriteAsync(equip_pos.pic, false).Forget();
            KText_IsWearing.GetTextMeshPro()
                .SetTMPText($"{languageConfig.Get("common_state_equipping").current}");

            if (equip_quality.subtype == 1)
            {
                string frame = quality.frame;
                if (panelType == UIPanel_Equipment.EquipPanelType.MainWeapon)
                {
                    frame += "_weapon";
                }

                KImg_Quality.SetActive(false);
                KBg_Item.GetImage().SetSpriteAsync(frame, false).Forget();
            }
            else
            {
                string equipFrame = quality.frame;
                if (panelType == UIPanel_Equipment.EquipPanelType.MainWeapon)
                {
                    equipFrame += "_weapon";
                }

                KImg_Quality.SetActive(true);
                KImg_Quality.GetImage().SetSpriteAsync(quality.numFrame, false).Forget();
                KText_Quality.GetTextMeshPro().SetTMPText($"{equip_quality.subtype - 1}");

                KBg_Item.GetImage().SetSpriteAsync(equipFrame, false).Forget();
            }


            if (equip_data.sYn == 0)
            {
                KImg_IsS.SetActive(false);
            }
            else
            {
                KImg_IsS.SetActive(true);
            }


            KImg_Mask.SetActive(false);
            KIsWearing.SetActive(false);
            KImg_RedDot.SetActive(false);

            //common_state_equipping装备中

            bool curWearPos =
                ResourcesSingleton.Instance.equipmentData.isWearingEquipments.ContainsKey(equip_data.posId);


            switch (panelType)
            {
                case UIPanel_Equipment.EquipPanelType.Main:

                    KImg_RedDot.SetActive(!curWearPos);

                    break;
                case UIPanel_Equipment.EquipPanelType.Compose:

                    KImg_RedDot.SetActive(myGameEquip.canCompound);
                    KIsWearing.SetActive(myGameEquip.isWearing);

                    break;
                case UIPanel_Equipment.EquipPanelType.ComposeSelected:
                    break;
                case UIPanel_Equipment.EquipPanelType.MainWeapon:

                    KImg_RedDot.SetActive(!curWearPos);
                    break;
            }
        }

        /// <summary>
        /// 设置对应reward串的icon和count文本(可空)
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <param name="icon">对应icon的ui</param>
        /// <param name="text">对应text的ui</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetRewardIconAndCountText(Vector3 reward, UI common_RewardItem, float alpha = 1,
            bool enablePos = false)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;
            var equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_pos = ConfigManager.Instance.Tables.Tbequip_pos;
            var quality = ConfigManager.Instance.Tables.Tbquality;

            var KBtn_Item = common_RewardItem.GetFromReference(UICommon_RewardItem.KBtn_Item);
            var KBg_Item = common_RewardItem.GetFromReference(UICommon_RewardItem.KBg_Item);
            var KImg_ItemIcon = common_RewardItem.GetFromReference(UICommon_RewardItem.KImg_ItemIcon);
            var KTxt_ItemCount = common_RewardItem.GetFromReference(UICommon_RewardItem.KText_ItemCount);
            var KIsItem = common_RewardItem.GetFromReference(UICommon_RewardItem.KIsItem);
            var KIsEquip = common_RewardItem.GetFromReference(UICommon_RewardItem.KIsEquip);
            var KImg_Pos = common_RewardItem.GetFromReference(UICommon_RewardItem.KImg_Pos);
            var KImg_PosIcon = common_RewardItem.GetFromReference(UICommon_RewardItem.KImg_PosIcon);
            var KImg_IsS = common_RewardItem.GetFromReference(UICommon_RewardItem.KImg_IsS);
            var KImg_Quality = common_RewardItem.GetFromReference(UICommon_RewardItem.KImg_Quality);
            var KText_Quality = common_RewardItem.GetFromReference(UICommon_RewardItem.KText_Quality);
            var KImg_Mask = common_RewardItem.GetFromReference(UICommon_RewardItem.KImg_Mask);
            var KImg_MaskIcon = common_RewardItem.GetFromReference(UICommon_RewardItem.KImg_MaskIcon);
            var KIsWearing = common_RewardItem.GetFromReference(UICommon_RewardItem.KIsWearing);
            var KText_IsWearing = common_RewardItem.GetFromReference(UICommon_RewardItem.KText_IsWearing);
            var KImg_RedDot = common_RewardItem.GetFromReference(UICommon_RewardItem.KImg_RedDot);
            var KText_Grade = common_RewardItem.GetFromReference(UICommon_RewardItem.KText_Grade);
            var KImg_Selected = common_RewardItem.GetFromReference(UICommon_RewardItem.KImg_Selected);
            var KAphla = common_RewardItem.GetFromReference(UICommon_RewardItem.KAphla);
            var KImg_ItemMask = common_RewardItem.GetFromReference(UICommon_RewardItem.KImg_ItemMask);
            var KText_ItemMask = common_RewardItem.GetFromReference(UICommon_RewardItem.KText_ItemMask);
            var KImg_Outer = common_RewardItem.GetFromReference(UICommon_RewardItem.KImg_Outer);


            var itemRect = KBtn_Item.GetRectTransform();

            itemRect.SetOffsetWithRight(0);
            itemRect.SetOffsetWithLeft(0);
            itemRect.SetOffsetWithBottom(0);
            itemRect.SetOffsetWithTop(0);
            itemRect.SetAnchoredPosition(Vector2.zero);

            KBg_Item.GetXImage().SetAlpha(1);
            KImg_ItemIcon.GetXImage().SetAlpha(1);
            KTxt_ItemCount.GetTextMeshPro().SetAlpha(1);
            //KImg_ItemMask
            if (alpha < 1 && alpha > 0)
            {
                KBg_Item.GetXImage().SetAlpha(alpha);
                KImg_ItemIcon.GetXImage().SetAlpha(alpha);
                KTxt_ItemCount.GetTextMeshPro().SetAlpha(alpha);
            }

            KBg_Item.SetActive(true);
            KImg_ItemMask.SetActive(false);
            KAphla.SetActive(false);
            KImg_Selected.SetActive(false);
            KImg_Pos.SetActive(false);

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;
            bool isCompositeEquip = IsCompositeEquipReward(reward);


            if (rewardx == 11 && !isCompositeEquip)
            {
                KIsEquip.SetActive(true);
                KIsItem.SetActive(false);
                KImg_Outer.SetActive(false);
            }
            else
            {
                KIsEquip.SetActive(false);
                KIsItem.SetActive(true);
                KImg_Outer.SetActive(false);
            }

            int count = rewardz;
            switch (rewardx)
            {
                case 1:
                    KImg_ItemIcon.GetImage().SetSpriteAsync(user_varibles.Get(rewardx).icon, false).Forget();
                    SetCountText(KTxt_ItemCount, count);


                    //var quality5 = user_varibles.Get();

                    KBg_Item.GetImage().SetSpriteAsync(quality.Get(JiYuUIHelper.GetRewardQuality(reward)).frame, false)
                        .Forget();
                    break;
                case 2:
                    KImg_ItemIcon.GetImage().SetSpriteAsync(user_varibles.Get(rewardx).icon, false).Forget();
                    SetCountText(KTxt_ItemCount, count);
                    KBg_Item.GetImage().SetSpriteAsync(quality.Get(JiYuUIHelper.GetRewardQuality(reward)).frame, false)
                        .Forget();
                    break;
                case 3:
                    KImg_ItemIcon.GetImage().SetSpriteAsync(user_varibles.Get(rewardx).icon, false).Forget();
                    SetCountText(KTxt_ItemCount, count);
                    KBg_Item.GetImage().SetSpriteAsync(quality.Get(JiYuUIHelper.GetRewardQuality(reward)).frame, false)
                        .Forget();
                    break;
                case 4:
                    KImg_ItemIcon.GetImage().SetSpriteAsync(user_varibles.Get(rewardx).icon, false).Forget();
                    SetCountText(KTxt_ItemCount, count);
                    KBg_Item.GetImage().SetSpriteAsync(quality.Get(JiYuUIHelper.GetRewardQuality(reward)).frame, false)
                        .Forget();
                    break;
                case 5:

                    var quality5 = quality.Get(item.Get(rewardy).quality);

                    KBg_Item.GetImage().SetSpriteAsync(quality5.frame, false).Forget();

                    KImg_ItemIcon.GetImage().SetSpriteAsync(item.Get(rewardy).icon, false).Forget();
                    SetCountText(KTxt_ItemCount, count);

                    break;
                case 6:
                    KImg_ItemIcon.GetImage().SetSpriteAsync("icon_patrol_money", false).Forget();
                    SetCountText(KTxt_ItemCount, count);
                    KBg_Item.GetImage().SetSpriteAsync(quality.Get(JiYuUIHelper.GetRewardQuality(reward)).frame, false)
                        .Forget();
                    break;
                case 7:
                    KImg_ItemIcon.GetImage().SetSpriteAsync("icon_patrol_exp", false).Forget();
                    SetCountText(KTxt_ItemCount, count);
                    KBg_Item.GetImage().SetSpriteAsync(quality.Get(JiYuUIHelper.GetRewardQuality(reward)).frame, false)
                        .Forget();
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    // if (isCompositeEquip)
                    // {
                    //     KTxt_ItemCount.SetActive(true);
                    // }
                    // else
                    // {
                    //     count = 1;
                    // }
                    KTxt_ItemCount.SetActive(isCompositeEquip);
                    SetCountText(KTxt_ItemCount, count);

                    var equip_data11 = equip_data.Get(rewardy);
                    var equip_quality11 = equip_quality.Get(equip_data11.quality);
                    var quality11 = quality.Get(equip_quality11.type);
                    var equip_pos11 = equip_pos.Get(equip_data11.posId);

                    KImg_ItemIcon.GetImage().SetSpriteAsync(equip_data11.icon, false).Forget();


                    //TODO:reward串是否需要显示Pos
                    KImg_Pos.SetActive(enablePos);

                    KImg_Pos.GetImage().SetSpriteAsync(quality11.posFrame, false).Forget();
                    KImg_PosIcon.GetImage().SetSpriteAsync(equip_pos11.pic, false).Forget();

                    if (equip_quality11.subtype == 1)
                    {
                        KImg_Quality.SetActive(false);
                        KBg_Item.GetImage().SetSpriteAsync(quality11.frame, false).Forget();
                    }
                    else
                    {
                        KImg_Quality.SetActive(true);
                        KImg_Quality.GetImage().SetSpriteAsync(quality11.numFrame, false).Forget();
                        KText_Quality.GetTextMeshPro().SetTMPText($"{equip_quality11.subtype - 1}");
                        KBg_Item.GetImage().SetSpriteAsync(quality11.frame, false).Forget();
                    }


                    if (equip_data11.sYn == 0)
                    {
                        KImg_IsS.SetActive(false);
                    }
                    else
                    {
                        KImg_IsS.SetActive(true);
                    }

                    KImg_Mask.SetActive(false);
                    KIsWearing.SetActive(false);
                    KImg_RedDot.SetActive(false);
                    KText_Grade.SetActive(false);

                    break;
                case 12:
                    break;
                case 13:

                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }
        }

        /// <summary>
        /// 获取reward串的 iconText字符串
        /// </summary>
        /// <param name="reward">reward串</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetRewardTextIconName(Vector3 reward)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;
            var equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_pos = ConfigManager.Instance.Tables.Tbequip_pos;
            var quality = ConfigManager.Instance.Tables.Tbquality;

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;
            string name = default;

            switch (rewardx)
            {
                case 1:
                    name = user_varibles.Get(rewardx).icon;

                    break;
                case 2:
                    name = user_varibles.Get(rewardx).icon;
                    break;
                case 3:
                    name = user_varibles.Get(rewardx).icon;
                    break;
                case 4:
                    name = user_varibles.Get(rewardx).icon;
                    break;
                case 5:
                    var item5 = item.Get(rewardy);
                    name = item5.icon;

                    break;
                case 6:
                    //name = "patrol_money_name";
                    break;
                case 7:
                    //name = "patrol_exp_name";
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    var equip_data11 = equip_data.Get(rewardy);
                    name = equip_data11.icon;

                    break;
                case 12:
                    break;
                case 13:

                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }

            name = GetRewardTextIconName(name);

            return name;
        }

        /// <summary>
        /// 获取字符串的 iconText字符串
        /// </summary>
        /// <param name="iconName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetRewardTextIconName(string iconName)
        {
            var name = $"<sprite=\"item_atlas_tsa\" name=\"{iconName}\">";

            return name;
        }

        /// <summary>
        /// 转换成子弹样式文字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetBulletTypeStr(string input)
        {
            var closeStr =
                $"{JiYuUIHelper.GetRewardTextIconName("common_bullet_logo_left")}{input}{JiYuUIHelper.GetRewardTextIconName("common_bullet_logo_right")}";
            return input;
        }

        public enum Vector3Type
        {
            ENEERGY = 1,
            BITCOIN,
            DOLLARS,
            EXP,
            ITEM,
            PATROLMONEY,
            PATROLEXP,
            EQUIP = 11,
            SUMMON,
            PLAYER,
            ACCESSORIES,
            DRESS
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetVector3(Vector3Type type)
        {
            return new Vector3((int)type, 0, 1);
        }

        /// <summary>
        /// 获取reward串的名字(默认带品质)
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <param name="showQuality">默认显示品质</param>
        /// <returns>结果字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetRewardName(Vector3 reward, bool showQuality = true)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;
            var equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_pos = ConfigManager.Instance.Tables.Tbequip_pos;
            var quality = ConfigManager.Instance.Tables.Tbquality;

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;
            string name = default;
            switch (rewardx)
            {
                case 1:
                    name = user_varibles.Get(rewardx).name;

                    break;
                case 2:
                    name = user_varibles.Get(rewardx).name;
                    break;
                case 3:
                    name = user_varibles.Get(rewardx).name;
                    break;
                case 4:
                    name = user_varibles.Get(rewardx).name;
                    break;
                case 5:
                    var item5 = item.Get(rewardy);
                    var hex5 = quality.Get(item5.quality).fontColor;
                    var langkey5 = quality.Get(item5.quality).name;
                    name = $"{language.Get(item5.name).current}";

                    if (item5.qualityYn == 1 && showQuality)
                    {
                        name = string.Format(language.Get(item5.name).current,
                            UnityHelper.RichTextColor(language.Get(langkey5).current, hex5));
                    }

                    return name;

                    break;
                case 6:
                    name = "patrol_money_name";
                    break;
                case 7:
                    name = "patrol_exp_name";
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:

                    if (IsCompositeEquipReward(reward))
                    {
                        //rewardz = newRewardz;
                        var equip_data11 = equip_data.Get(rewardy);
                        var qualityid = equip_data11.quality;
                        var hex11 = quality.Get(equip_quality.Get(qualityid).type).fontColor;
                        var langkey11 = quality.Get(equip_quality.Get(qualityid).type).name;

                        if (showQuality)
                        {
                            name = string.Format(language.Get(equip_data11.name).current,
                                UnityHelper.RichTextColor(language.Get(langkey11).current, hex11));
                        }
                        else
                        {
                            name = language.Get(equip_data11.name).current;
                        }
                    }
                    else
                    {
                        var equip_data11 = equip_data.Get(rewardy);
                        var equip_quality11 = equip_quality.Get(equip_data11.quality);

                        var qualityid = equip_data11.quality;
                        var hex11 = quality.Get(equip_quality.Get(qualityid).type).fontColor;

                        var langkey11 = quality.Get(equip_quality.Get(qualityid).type).name;

                        string namepre = "";
                        if (equip_quality11.subtype - 1 != 0)
                        {
                            namepre = $"+{equip_quality11.subtype - 1}";
                            namepre = UnityHelper.RichTextColor(namepre, hex11);
                            name = $"{namepre}{language.Get(equip_data11.name).current}";
                        }
                        else
                        {
                            name = $"    {language.Get(equip_data11.name).current}";
                        }

                        if (showQuality)
                        {
                            name =
                                $"{UnityHelper.RichTextColor(language.Get(langkey11).current, hex11)}{name}";
                        }
                    }

                    return name;
                    break;
                case 12:
                    break;
                case 13:

                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }

            name = language.Get(name).current;
            return name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async UniTask TypeWriteEffect(UI textUI, string str,
            System.Threading.CancellationToken cancellationToken = default
            , float typingSpeed = 0.05f)
        {
            if (textUI == null || string.IsNullOrEmpty(str))
            {
                return;
            }

            // 清空文本
            var outPut = "";
            var tmp = textUI.GetTextMeshPro();
            tmp.SetTMPText(outPut);

            // 逐个字符显示
            foreach (char c in str)
            {
                outPut += c;
                tmp.SetTMPText(outPut);
                await UniTask.Delay(System.TimeSpan.FromSeconds(typingSpeed), cancellationToken: cancellationToken);
            }
        }


        /// <summary>
        /// 获取reward串的描述
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <returns>结果字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetRewardDes(Vector3 reward)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;
            var equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_pos = ConfigManager.Instance.Tables.Tbequip_pos;
            var quality = ConfigManager.Instance.Tables.Tbquality;

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;
            string name = default;
            switch (rewardx)
            {
                case 1:
                    name = user_varibles.Get(rewardx).desc;

                    break;
                case 2:
                    name = user_varibles.Get(rewardx).desc;
                    break;
                case 3:
                    name = user_varibles.Get(rewardx).desc;
                    break;
                case 4:
                    name = user_varibles.Get(rewardx).desc;
                    break;
                case 5:
                    name = item.Get(rewardy).desc;

                    break;
                case 6:
                    int tenMinMoney =
                        (int)(6 * rewardz * (1.0f + ResourcesSingleton.Instance.UserInfo.PatrolGainName / 10000.0f));

                    name = string.Format(language.Get($"patrol_money_desc").current, FormatNumber(tenMinMoney));

                    return name;


                    break;
                case 7:
                    int tenMinExp =
                        (int)(6 * rewardz * (1.0f + ResourcesSingleton.Instance.UserInfo.PatrolGainName / 10000.0f));
                    name = string.Format(language.Get($"patrol_exp_desc").current, FormatNumber(tenMinExp));

                    return name;
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    name = equip_data.Get(rewardy).desc;

                    break;
                case 12:
                    break;
                case 13:

                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }

            name = language.Get(name).current;
            return name;
        }


        /// <summary>
        /// 获取reward串的品质
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <returns>结果字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRewardQuality(Vector3 reward)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;
            var equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            var equip_pos = ConfigManager.Instance.Tables.Tbequip_pos;
            var quality = ConfigManager.Instance.Tables.Tbquality;

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;
            string name = default;
            switch (rewardx)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    return user_varibles.Get(rewardx).quality;

                case 5:
                    return item.Get(rewardy).quality;
                case 6:
                case 7:
                    return 3;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    return equip_data.Get(rewardy).quality;
                case 12:
                    break;
                case 13:

                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }

            return 1;
        }

        /// <summary>
        /// 只设置对应reward串的icon
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <param name="icon">对应icon的ui</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetIconOnly(Vector3 reward, UI icon)
        {
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;
            var equip_data = ConfigManager.Instance.Tables.Tbequip_data;

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;

            switch (rewardx)
            {
                case 1:

                    icon.GetImage().SetSpriteAsync(user_varibles.Get(rewardx).icon, false).Forget();

                    break;
                case 2:
                    icon.GetImage().SetSpriteAsync(user_varibles.Get(rewardx).icon, false).Forget();

                    break;
                case 3:
                    icon.GetImage().SetSpriteAsync(user_varibles.Get(rewardx).icon, false).Forget();

                    break;
                case 4:
                    icon.GetImage().SetSpriteAsync(user_varibles.Get(rewardx).icon, false).Forget();

                    break;
                case 5:
                    icon.GetImage().SetSpriteAsync(item.Get(rewardy).icon, false).Forget();

                    break;
                case 6:

                    break;
                case 7:

                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:

                    var equip_data11 = equip_data.Get(rewardy);
                    icon.GetImage().SetSpriteAsync(equip_data11.icon, false).Forget();

                    break;
                case 12:
                    break;
                case 13:

                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }
        }


        /// <summary>
        /// 销毁所有的tips弹窗
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestoryEquipTips()
        {
            Debug.Log("DestoryEquipTips");
            //UIHelper.Remove(UIType.UICommon_ItemTips);
            UIHelper.Remove(UIType.UICommon_EquipTips);
        }

        /// <summary>
        /// 销毁所有的tips弹窗
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestoryAllTips()
        {
            //bool isRewardTip = false;
            if (JiYuUIHelper.TryGetUI(UIType.UICommon_Reward_Tip, out var ui1))
            {
                //isRewardTip=true;
                UIHelper.Remove(UIType.UICommon_Reward_Tip);
            }
             if(JiYuUIHelper.TryGetUI(UIType.UICommon_Reward_TipWithTitle, out var ui2))
            {
                //isRewardTip = true;
                UIHelper.Remove(UIType.UICommon_Reward_TipWithTitle);
            }
            if (JiYuUIHelper.TryGetUI(UIType.UICommon_ItemTips, out var ui))
            {
                UIHelper.Remove(UIType.UICommon_ItemTips);
            }
          
            UIHelper.Remove(UIType.UICommon_EquipTips);
         

    

        }


        /// <summary>
        /// 销毁所有的tips弹窗
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestoryItemTips()
        {

            if (JiYuUIHelper.TryGetUI(UIType.UICommon_ItemTips, out var ui))
            {
                UIHelper.Remove(UIType.UICommon_ItemTips);
            }

            UIHelper.Remove(UIType.UICommon_EquipTips);
        }
        /// <summary>
        /// 设置基于reward串的common_RewardItem类预制件的点击弹板
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <param name="common_RewardItem">创建的common_RewardItem的UI</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async static UniTaskVoid SetRewardOnClickWithNoBtn(Vector3 reward, UI common_RewardItem)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;
            var equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            var quality = ConfigManager.Instance.Tables.Tbquality;

            //var btn = common_RewardItem.GetFromReference(UICommon_RewardItem.KBtn_Item);

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;

            UI tipUI = default;
            UI title = default;
            UI des = default;
            switch (rewardx)
            {
                case 1:

                    DestoryAllTips();
                    tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips) as
                        UICommon_ItemTips;
                    title = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title);
                    des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
                    SetRewardOnClickTipTitleDes(reward, title, des);
                    SetTipPosAndResize(common_RewardItem, tipUI);
                    //action?.Invoke();
                    // var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
                    //         new UICommon_ItemTips.ItemTipsData
                    //         {
                    //             itemUI = common_RewardItem,
                    //             KTxt_Title = "",
                    //             KTxt_Des = "",
                    //             ArrowType = UICommon_ItemTips.ArrowType.Default
                    //         }) as
                    //     UICommon_ItemTips;
                    // var title = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title);
                    // var des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
                    // SetRewardOnClickTipTitleDes(reward, title, des);
                    //
                    // // SetTipPos(common_RewardItem, tipUI, UICommon_ItemTips.KContent,
                    // //     UICommon_ItemTips.KImg_ArrowDown, UICommon_ItemTips.KImg_ArrowUp);
                    //
                    // action?.Invoke();

                    break;
                case 2:

                    DestoryAllTips();
                    tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips) as
                        UICommon_ItemTips;
                    title = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title);
                    des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
                    SetRewardOnClickTipTitleDes(reward, title, des);
                    SetTipPosAndResize(common_RewardItem, tipUI);
                    //action?.Invoke();

                    // var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
                    //         new UICommon_ItemTips.ItemTipsData
                    //         {
                    //             itemUI = common_RewardItem,
                    //             KTxt_Title = "",
                    //             KTxt_Des = "",
                    //             ArrowType = UICommon_ItemTips.ArrowType.Default
                    //         }) as
                    //     UICommon_ItemTips;
                    // var title = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title);
                    // var des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
                    // SetRewardOnClickTipTitleDes(reward, title, des);
                    // // SetTipPos(common_RewardItem, tipUI, UICommon_ItemTips.KContent,
                    // //     UICommon_ItemTips.KImg_ArrowDown, UICommon_ItemTips.KImg_ArrowUp);
                    // action?.Invoke();

                    break;
                case 3:

                    DestoryAllTips();
                    tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips) as
                        UICommon_ItemTips;
                    title = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title);
                    des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
                    SetRewardOnClickTipTitleDes(reward, title, des);
                    SetTipPosAndResize(common_RewardItem, tipUI);
                    //action?.Invoke();

                    break;
                case 4:

                    DestoryAllTips();
                    tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips) as
                        UICommon_ItemTips;

                    title = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title);
                    des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
                    SetRewardOnClickTipTitleDes(reward, title, des);
                    SetTipPosAndResize(common_RewardItem, tipUI);
                    //action?.Invoke();

                    break;
                case 5:
                    var item5 = item.Get(rewardy);
                    if (item5.tipsType == 0)
                    {
                        DestoryAllTips();
                        tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips) as
                            UICommon_ItemTips;

                        title = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title);
                        des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
                        SetRewardOnClickTipTitleDes(reward, title, des);
                        SetTipPosAndResize(common_RewardItem, tipUI);
                        //action?.Invoke();
                    }
                    else
                    {
                        //TODO:

                        DestoryAllTips();
                        Log.Debug($"{reward} 当前reward串弹板类型未生效", Color.green);
                        //action?.Invoke();
                    }

                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:


                    if (IsCompositeEquipReward(reward))
                    {
                        DestoryAllTips();
                        title = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title);
                        des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
                        SetRewardOnClickTipTitleDes(reward, title, des);

                        tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips, new UICommon_ItemTips.ItemTipsData
                            {
                                itemUI = common_RewardItem,
                                KTxt_Title = null,
                                KTxt_Des = null,
                                ArrowType = UICommon_ItemTips.ArrowType.Default
                            }) as
                            UICommon_ItemTips;
                        //SetTipPosAndResize(common_RewardItem, tipUI);
                        //action?.Invoke();
                    }
                    else
                    {
                        var gameEquip = new MyGameEquip
                        {
                            // equip = new EquipDto()
                            // {
                            //     EquipId = rewardy,
                            //     Quality = rewardz,
                            //     EquipLevel = 1
                            // }
                            reward = reward
                        };

                        DestoryAllTips();
                        tipUI = await UIHelper.CreateAsync<MyGameEquip>(UIType.UICommon_EquipTips, gameEquip) as
                            UICommon_EquipTips;

                        //TODO:设置位置
                        // var title = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title);
                        // var des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
                        // SetRewardOnClickTipTitleDes(reward, title, des);
                        // SetTipPos(common_RewardItem, tipUI, UICommon_ItemTips.KContent,
                        //     UICommon_ItemTips.KImg_ArrowDown, UICommon_ItemTips.KImg_ArrowUp);
                        tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle).SetActive(false);
                        tipUI.GetFromReference(UICommon_EquipTips.KBottom).SetActive(false);
                        tipUI.GetFromReference(UICommon_EquipTips.KBtn_Decrease).SetActive(false);

                        var itemPos = JiYuUIHelper.GetUIPos(common_RewardItem);

                        float tipMidH = tipUI.GetFromReference(UICommon_EquipTips.KMid).GetRectTransform().Height();
                        float tipTopH = tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform()
                            .Height();
                        float tipTopHelp = tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle)
                            .GetRectTransform().Height();
                        float tipBottomH = tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw)
                            .GetRectTransform().Height();
                        float tipBottomHelp = tipUI.GetFromReference(UICommon_EquipTips.KBottom).GetRectTransform()
                            .Height();
                        float screeenH = Screen.height;
                        float itemH = common_RewardItem.GetRectTransform().Height();
                        float TipMidPosY = 61.8f;

                        if (itemPos.y - tipTopH - tipMidH - itemH > -screeenH / 2)
                        {
                            //down
                            tipUI.GetRectTransform()
                                .SetAnchoredPositionY(itemPos.y - tipMidH / 2 - tipTopH - itemH / 2 - TipMidPosY);
                            tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(true);
                            tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).SetActive(false);
                            tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform()
                                .SetAnchoredPositionX(itemPos.x);
                        }
                        else
                        {
                            //up
                            tipUI.GetRectTransform()
                                .SetAnchoredPositionY(itemPos.y + tipMidH / 2 + tipBottomH + itemH / 2 -
                                                      TipMidPosY);
                            tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(false);
                            tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).SetActive(true);
                            tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).GetRectTransform()
                                .SetAnchoredPositionX(itemPos.x);
                        }


                        //action?.Invoke();
                    }

                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }
        }


     

        /// <summary>
        /// 设置基于reward串的common_RewardItem类预制件的点击弹板
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <param name="common_RewardItem">创建的common_RewardItem的UI</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetRewardOnClick(Vector3 reward, UI common_RewardItem,UI mask=default)
        {
            if (mask != null)
            {
                mask.SetActive(false);
                //SetCloseTip(mask);
            }

            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;
            var equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            var quality = ConfigManager.Instance.Tables.Tbquality;

            var btn = common_RewardItem.GetFromReference(UICommon_RewardItem.KBtn_Item);

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;
            switch (rewardx)
            {
                case 1:
                    btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
                    {
                        JiYuUIHelper.DestoryItemTips();
                        var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
                            new UICommon_ItemTips.TipsData()
                            {
                                itemUI = common_RewardItem,
                                reward = reward
                            }

                        );
                        if (mask != null)
                        {
                            mask.SetActive(true);
                            //SetCloseTip(mask);
                        }
                    }));
                    break;
                case 2:
                    btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
                    {
                        JiYuUIHelper.DestoryItemTips();
                        var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
                            new UICommon_ItemTips.TipsData()
                            {
                                itemUI = common_RewardItem,
                                reward = reward
                            });
                        if (mask != null)
                        {
                            mask.SetActive(true);
                            //SetCloseTip(mask);
                        }
                    }));
                    break;
                case 3:
                    btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
                    {
                        JiYuUIHelper.DestoryItemTips();
                        var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
                            new UICommon_ItemTips.TipsData()
                            {
                                itemUI = common_RewardItem,
                                reward = reward
                            });
                        if (mask != null)
                        {
                            mask.SetActive(true);
                            //SetCloseTip(mask);
                        }
                    }));
                    break;
                case 4:
                    btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
                    {
                        JiYuUIHelper.DestoryItemTips();
                        var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
                            new UICommon_ItemTips.TipsData()
                            {
                                itemUI = common_RewardItem,
                                reward = reward
                            });
                        if (mask != null)
                        {
                            mask.SetActive(true);
                            //SetCloseTip(mask);
                        }
                    }));
                    break;
                case 5:
                    var item5 = item.Get(rewardy);
                    if (item5.tipsType == 0)
                    {
                        btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
                        {
                            JiYuUIHelper.DestoryItemTips();
                            var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
                                new UICommon_ItemTips.TipsData()
                                {
                                    itemUI = common_RewardItem,
                                    reward = reward
                                });
                            if (mask != null)
                            {
                                mask.SetActive(true);
                                //SetCloseTip(mask);
                            }
                        }));
                    }
                    else if (item5.tipsType == 1)
                    {
                        //TODO:
                        btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
                        {
                            JiYuUIHelper.DestoryItemTips();
                            var tipUI = await UIHelper.CreateAsync(UIType.UIPanel_SelectBoxNomal, reward) as
                                UIPanel_SelectBoxNomal;
                            if (mask != null)
                            {
                                mask.SetActive(true);
                                //SetCloseTip(mask);
                            }
                        }));
                    }
                    else
                    {
                        //TODO:
                        btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
                        {
                            JiYuUIHelper.DestoryItemTips();
                            Log.Debug($"{reward} 当前reward串弹板类型未生效", Color.green);
                            //action?.Invoke();
                        }));
                    }

                    break;
                case 6:
                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
                    {
                        JiYuUIHelper.DestoryItemTips();
                        var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
                            new UICommon_ItemTips.TipsData()
                            {
                                itemUI = common_RewardItem,
                                reward = reward
                            });
                        if (mask != null)
                        {
                            mask.SetActive(true);
                            //SetCloseTip(mask);
                        }
                    }));
                    break;
                case 7:
                    JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
                    {
                        JiYuUIHelper.DestoryItemTips();
                        var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
                            new UICommon_ItemTips.TipsData()
                            {
                                itemUI = common_RewardItem,
                                reward = reward
                            });
                        if (mask != null)
                        {
                            mask.SetActive(true);
                            //SetCloseTip(mask);
                        }
                    }));
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:


                    if (IsCompositeEquipReward(reward))
                    {
                        btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
                        {
                            JiYuUIHelper.DestoryItemTips();
                            var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
                                new UICommon_ItemTips.TipsData()
                                {
                                    itemUI = common_RewardItem,
                                    reward = reward
                                });
                            if (mask != null)
                            {
                                mask.SetActive(true);
                                //SetCloseTip(mask);
                            }
                        }));
                    }
                    else
                    {
                        var gameEquip = new MyGameEquip
                        {
                            // equip = new EquipDto()
                            // {
                            //     EquipId = rewardy,
                            //     Quality = rewardz,
                            //     EquipLevel = 1,
                            // },
                            reward = reward
                        };
                        btn.GetXButton().OnClick.Add(UniTask.UnityAction(async () =>
                        {
                            JiYuUIHelper.DestoryItemTips();
                            var tipUI = await UIHelper.CreateAsync<MyGameEquip>(UIType.UICommon_EquipTips, gameEquip) as
                                UICommon_EquipTips;
                            tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle).SetActive(false);
                            tipUI.GetFromReference(UICommon_EquipTips.KBottom).SetActive(false);
                            tipUI.GetFromReference(UICommon_EquipTips.KBtn_Decrease).SetActive(false);

                            var itemPos = JiYuUIHelper.GetUIPos(common_RewardItem);

                            float tipMidH = tipUI.GetFromReference(UICommon_EquipTips.KMid).GetRectTransform().Height();
                            float tipTopH = tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform()
                                .Height();
                            float tipTopHelp = tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle)
                                .GetRectTransform().Height();
                            float tipBottomH = tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw)
                                .GetRectTransform().Height();
                            float tipBottomHelp = tipUI.GetFromReference(UICommon_EquipTips.KBottom).GetRectTransform()
                                .Height();
                            float screeenH = Screen.height;
                            float itemH = common_RewardItem.GetRectTransform().Height();
                            float TipMidPosY = 61.8f;

                            if (itemPos.y - tipTopH - tipMidH - itemH > -screeenH / 2)
                            {
                                //down
                                tipUI.GetRectTransform()
                                    .SetAnchoredPositionY(itemPos.y - tipMidH / 2 - tipTopH - itemH / 2 - TipMidPosY);
                                tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(true);
                                tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).SetActive(false);
                                tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform()
                                    .SetAnchoredPositionX(itemPos.x);
                            }
                            else
                            {
                                //up
                                tipUI.GetRectTransform()
                                    .SetAnchoredPositionY(itemPos.y + tipMidH / 2 + tipBottomH + itemH / 2 -
                                                          TipMidPosY);
                                tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(false);
                                tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).SetActive(true);
                                tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).GetRectTransform()
                                    .SetAnchoredPositionX(itemPos.x);
                            }
                            if (mask != null)
                            {
                                mask.SetActive(true);
                                //SetCloseTip(mask);
                            }

                            //action?.Invoke();
                        }));
                    }

                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }
            //Log.Debug("dfasdfdfd", Color.cyan);


        }

        ///// <summary>
        ///// 预览tips 设置基于reward串的common_RewardItem类预制件的纯Tips弹板
        ///// </summary>
        ///// <param name="reward">reward串</param>
        ///// <param name="common_RewardItem">创建的common_RewardItem的UI</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static void SetRewardOnClickTips(Vector3 reward, UI common_RewardItem)
        //{
        //    var language = ConfigManager.Instance.Tables.Tblanguage;
        //    var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
        //    var item = ConfigManager.Instance.Tables.Tbitem;
        //    var equip_data = ConfigManager.Instance.Tables.Tbequip_data;
        //    var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
        //    var quality = ConfigManager.Instance.Tables.Tbquality;

        //    var btn = common_RewardItem.GetFromReference(UICommon_RewardItem.KBtn_Item);

        //    var rewardx = (int)reward.x;
        //    var rewardy = (int)reward.y;
        //    var rewardz = (int)reward.z;
        //    switch (rewardx)
        //    {
        //        case 1:
        //            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
        //            {
        //                JiYuUIHelper.DestoryItemTips();
        //                var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
        //                    new UICommon_ItemTips.TipsData()
        //                    {
        //                        itemUI = common_RewardItem,
        //                        reward = reward
        //                    });
        //            }));

        //            break;
        //        case 2:
        //            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
        //            {
        //                JiYuUIHelper.DestoryItemTips();
        //                var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
        //                    new UICommon_ItemTips.TipsData()
        //                    {
        //                        itemUI = common_RewardItem,
        //                        reward = reward
        //                    });
        //            }));
        //            break;
        //        case 3:
        //            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
        //            {
        //                JiYuUIHelper.DestoryItemTips();
        //                var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
        //                    new UICommon_ItemTips.TipsData()
        //                    {
        //                        itemUI = common_RewardItem,
        //                        reward = reward
        //                    });
        //            }));
        //            break;
        //        case 4:
        //            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
        //            {
        //                DestoryItemTips();
        //                var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
        //                    new UICommon_ItemTips.TipsData()
        //                    {
        //                        itemUI = common_RewardItem,
        //                        reward = reward
        //                    });
        //            }));
        //            break;
        //        case 5:
        //            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
        //            {
        //                DestoryItemTips();
        //                var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
        //                    new UICommon_ItemTips.TipsData()
        //                    {
        //                        itemUI = common_RewardItem,
        //                        reward = reward
        //                    });
        //            }));

        //            break;
        //        case 6:
        //            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
        //            {
        //                DestoryItemTips();
        //                var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
        //                    new UICommon_ItemTips.TipsData()
        //                    {
        //                        itemUI = common_RewardItem,
        //                        reward = reward
        //                    });
        //            }));
        //            break;
        //        case 7:
        //            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
        //            {
        //                DestoryItemTips();
        //                var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
        //                    new UICommon_ItemTips.TipsData()
        //                    {
        //                        itemUI = common_RewardItem,
        //                        reward = reward
        //                    });
        //            }));
        //            break;
        //        case 8:
        //            break;
        //        case 9:
        //            break;
        //        case 10:
        //            break;
        //        case 11:
        //            if (IsCompositeEquipReward(reward))
        //            {
        //                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
        //                {
        //                    DestoryItemTips();
        //                    var tipUI = await UIHelper.CreateAsync(UIType.UICommon_ItemTips,
        //                        new UICommon_ItemTips.TipsData()
        //                        {
        //                            itemUI = common_RewardItem,
        //                            reward = reward
        //                        });
        //                }));
        //            }
        //            else
        //            {
        //                var gameEquip = new MyGameEquip
        //                {
        //                    reward = reward
        //                };

        //                JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn, UniTask.UnityAction(async () =>
        //                {
        //                    DestoryItemTips();
        //                    var tipUI = await UIHelper.CreateAsync(UIType.UICommon_EquipTips, gameEquip);
                            
        //                    //TODO:设置位置
        //                    // var title = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title);
        //                    // var des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);
        //                    // SetRewardOnClickTipTitleDes(reward, title, des);
        //                    // SetTipPos(common_RewardItem, tipUI, UICommon_ItemTips.KContent,
        //                    //     UICommon_ItemTips.KImg_ArrowDown, UICommon_ItemTips.KImg_ArrowUp);
        //                    tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle).SetActive(false);
        //                    tipUI.GetFromReference(UICommon_EquipTips.KBottom).SetActive(false);
        //                    tipUI.GetFromReference(UICommon_EquipTips.KBtn_Decrease).SetActive(false);

        //                    var itemPos = JiYuUIHelper.GetUIPos(common_RewardItem);

        //                    float tipMidH = tipUI.GetFromReference(UICommon_EquipTips.KMid).GetRectTransform().Height();
        //                    float tipTopH = tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform()
        //                        .Height();
        //                    float tipTopHelp = tipUI.GetFromReference(UICommon_EquipTips.KImg_TopTitle)
        //                        .GetRectTransform().Height();
        //                    float tipBottomH = tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw)
        //                        .GetRectTransform().Height();
        //                    float tipBottomHelp = tipUI.GetFromReference(UICommon_EquipTips.KBottom).GetRectTransform()
        //                        .Height();
        //                    float screeenH = Screen.height;
        //                    float itemH = common_RewardItem.GetRectTransform().Height();
        //                    float TipMidPosY = 61.8f;

        //                    if (itemPos.y - tipTopH - tipMidH - itemH > -screeenH / 2f)
        //                    {
        //                        //down
        //                        tipUI.GetRectTransform()
        //                            .SetAnchoredPositionY(itemPos.y - tipMidH / 2f - tipTopH - itemH / 2f - TipMidPosY);
        //                        tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(true);
        //                        tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).SetActive(false);
        //                        tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).GetRectTransform()
        //                            .SetAnchoredPositionX(itemPos.x);
        //                    }
        //                    else
        //                    {
        //                        //up
        //                        tipUI.GetRectTransform()
        //                            .SetAnchoredPositionY(itemPos.y + tipMidH / 2f + tipBottomH + itemH / 2f -
        //                                                  TipMidPosY);
        //                        tipUI.GetFromReference(UICommon_EquipTips.KImg_TopArraw).SetActive(false);
        //                        tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).SetActive(true);
        //                        tipUI.GetFromReference(UICommon_EquipTips.KImg_BottomArraw).GetRectTransform()
        //                            .SetAnchoredPositionX(itemPos.x);
        //                    }


        //                    //action?.Invoke();
        //                }));
        //            }


        //            break;
        //        case 12:
        //            break;
        //        case 13:
        //            break;
        //        case 14:
        //            break;
        //        case 15:
        //            break;
        //        default:
        //            Log.Debug($"{reward} 当前reward串没有定义");

        //            break;
        //    }
        //}

        /// <summary>
        /// 增加相应的reward串数据并播放动画(可选)
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <param name="playerAni">是否同时播放动画</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async static UniTaskVoid AddReward(Vector3 reward, bool playerAni = false)
        {
            const float delay = 0.9f;
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = Mathf.Abs((int)reward.z);
            switch (rewardx)
            {
                case 1:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy < 0)
                    {
                        Log.Debug($"energy:{ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy} 为负值,结果异常");
                    }

                    break;
                case 2:

                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin < 0)
                    {
                        Log.Debug($"gems:{ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin} 为负值,结果异常");
                    }

                    break;
                case 3:

                    ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill < 0)
                    {
                        Log.Debug($"gold:{ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill} 为负值,结果异常");
                    }

                    break;
                case 4:

                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp < 0)
                    {
                        Log.Debug(
                            $"playerTotalExp:{ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp} 为负值,结果异常");
                    }

                    break;
                case 5:
                    if (ResourcesSingleton.Instance.items.ContainsKey(rewardy))
                    {
                        ResourcesSingleton.Instance.items[rewardy] += rewardz;
                        if (ResourcesSingleton.Instance.items[rewardy] < 0)
                        {
                            Log.Debug($"{rewardy}item:{ResourcesSingleton.Instance.items[rewardy]} 为负值,结果异常");
                        }
                    }
                    else
                    {
                        ResourcesSingleton.Instance.items.Add(rewardy, rewardz);
                    }

                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    NetWorkManager.Instance.SendMessage(CMD.QUERYEQUIP);
                    if (!ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(2))
                    {
                        ResourcesSingleton.Instance.settingData.UnlockMap.Add(2, 1);
                        if (TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                        {
                            var uis = ui as UIPanel_JiyuGame;
                            await uis.UnLockTag(2);
                            uis.RefreshTags();
                            uis.SetToPageIndex(2);
                            uis.OnBtnClickEvent(2);
                        }
                    }

                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }

            if (playerAni)
            {
                UIHelper.CreateAsync(UIType.UIResource, reward);
                await UniTask.Delay((int)(delay * 1000));
            }

            ResourcesSingleton.Instance.UpdateResourceUI();
        }


        /// <summary>
        /// 增加相应的reward串数据并播放动画(可选)
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <param name="playerAni">是否同时播放动画</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async static UniTaskVoid AddRewardInternal(Vector3 reward, bool playerAni = false, bool hasEquip = true)
        {
            const float delay = 0.9f;
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;
            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = Mathf.Abs((int)reward.z);
            switch (rewardx)
            {
                case 1:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy < 0)
                    {
                        Log.Debug($"energy:{ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy} 为负值,结果异常");
                    }

                    break;
                case 2:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin < 0)
                    {
                        Log.Debug($"gems:{ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin} 为负值,结果异常");
                    }

                    break;
                case 3:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill < 0)
                    {
                        Log.Debug($"gold:{ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill} 为负值,结果异常");
                    }

                    break;
                case 4:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp < 0)
                    {
                        Log.Debug(
                            $"playerTotalExp:{ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp} 为负值,结果异常");
                    }

                    break;
                case 5:
                    if (ResourcesSingleton.Instance.items.ContainsKey(rewardy))
                    {
                        ResourcesSingleton.Instance.items[rewardy] += rewardz;
                        if (ResourcesSingleton.Instance.items[rewardy] < 0)
                        {
                            Log.Debug($"{rewardy}item:{ResourcesSingleton.Instance.items[rewardy]} 为负值,结果异常");
                        }
                    }
                    else
                    {
                        ResourcesSingleton.Instance.items.Add(rewardy, rewardz);
                    }

                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    //NetWorkManager.Instance.SendMessage(CMD.QUERYEQUIP);
                    if (!ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(2))
                    {
                        ResourcesSingleton.Instance.settingData.UnlockMap.Add(2, 1);
                        if (TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                        {
                            var uis = ui as UIPanel_JiyuGame;
                            await uis.UnLockTag(2);
                            uis.RefreshTags();
                            uis.SetToPageIndex(2);
                            uis.OnBtnClickEvent(2);
                        }
                    }

                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }

            if (playerAni)
            {
                UIHelper.CreateAsync(UIType.UIResource, reward);
                await UniTask.Delay((int)(delay * 1000));
            }

            ResourcesSingleton.Instance.UpdateResourceUI();
        }

        /// <summary>
        /// 增加相应的reward串数据并播放动画(可选)
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <param name="playerAni">是否同时播放动画</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async static UniTaskVoid AddRewardsInternal(Vector3 reward, bool playerAni = false,
            bool hasEquip = true)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;
            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = Mathf.Abs((int)reward.z);
            switch (rewardx)
            {
                case 1:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy < 0)
                    {
                        Log.Debug($"energy:{ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy} 为负值,结果异常");
                    }

                    break;
                case 2:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin < 0)
                    {
                        Log.Debug($"gems:{ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin} 为负值,结果异常");
                    }

                    break;
                case 3:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill < 0)
                    {
                        Log.Debug($"gold:{ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill} 为负值,结果异常");
                    }

                    break;
                case 4:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp += rewardz;
                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp < 0)
                    {
                        Log.Debug(
                            $"playerTotalExp:{ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp} 为负值,结果异常");
                    }

                    break;
                case 5:
                    if (ResourcesSingleton.Instance.items.ContainsKey(rewardy))
                    {
                        ResourcesSingleton.Instance.items[rewardy] += rewardz;
                        if (ResourcesSingleton.Instance.items[rewardy] < 0)
                        {
                            Log.Debug($"{rewardy}item:{ResourcesSingleton.Instance.items[rewardy]} 为负值,结果异常");
                        }
                    }
                    else
                    {
                        ResourcesSingleton.Instance.items.Add(rewardy, rewardz);
                    }

                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    //NetWorkManager.Instance.SendMessage(CMD.QUERYEQUIP);
                    if (!ResourcesSingleton.Instance.settingData.UnlockMap.ContainsKey(2))
                    {
                        ResourcesSingleton.Instance.settingData.UnlockMap.Add(2, 1);
                        if (TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
                        {
                            var uis = ui as UIPanel_JiyuGame;
                            await uis.UnLockTag(2);
                            uis.RefreshTags();
                            uis.SetToPageIndex(2);
                            uis.OnBtnClickEvent(2);
                        }
                    }

                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }

            ResourcesSingleton.Instance.UpdateResourceUI();
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // private async static UniTaskVoid PlayRewardsAnim(List<Vector3> rewards)
        // {
        //     
        // }

        /// <summary>
        /// 增加相应的reward数组并播放动画(可选)
        /// </summary>
        /// <param name="rewards">reward串</param>
        /// <param name="playerAni">是否同时播放动画</param>
        /// <param name="delay">每个动画之间的延迟</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async static void AddReward(List<Vector3> rewards, bool playerAni = false,
            bool updateEquip = true)
        {
            const float AnimDelay = 0.9f;
            if (rewards.Count <= 0)
            {
                Log.Error($"传入rewards串为空");
                return;
            }

            bool hasEquip = false;

            foreach (var reward in rewards)
            {
                var rewardx = (int)reward.x;
                var rewardy = (int)reward.y;
                var rewardz = Mathf.Abs((int)reward.z);
                if (rewardx == 11)
                {
                    hasEquip = true;
                    break;
                }
            }

            if (hasEquip && updateEquip)
            {
                //Log.Error($"hasEquip{hasEquip}");
                NetWorkManager.Instance.SendMessage(CMD.QUERYEQUIP);
            }

            if (playerAni)
            {
                UIHelper.CreateAsync(UIType.UIResource, rewards);
                await UniTask.Delay((int)(AnimDelay * 1000));
            }


            foreach (var reward in rewards)
            {
                AddRewardsInternal(reward, playerAni);
            }
        }

        /// <summary>
        /// 减少reward串
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <returns>减少成功返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReduceReward(Vector3 reward)
        {
            if (!IsRewardsEnough(reward))
            {
                Log.Debug($"该reward数量不足 当前:{reward.z}个");
                return false;
            }

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = -Mathf.Abs((int)reward.z);
            switch (rewardx)
            {
                case 1:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy += rewardz;


                    break;
                case 2:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin += rewardz;


                    break;
                case 3:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill += rewardz;


                    break;
                case 4:
                    ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp += rewardz;


                    break;
                case 5:
                    if (ResourcesSingleton.Instance.items.ContainsKey(rewardy))
                    {
                        ResourcesSingleton.Instance.items[rewardy] += rewardz;
                        if (ResourcesSingleton.Instance.items[rewardy] == 0)
                        {
                            ResourcesSingleton.Instance.items.Remove(rewardy);
                        }
                    }
                    else
                    {
                        Log.Debug($"{reward} 缓存的items没有这个reward");
                    }

                    break;
                case 6:

                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    //增加装备
                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }

            ResourcesSingleton.Instance.UpdateResourceUI();

            return true;
        }

        /// <summary>
        /// 减少reward串数组
        /// </summary>
        /// <param name="rewards">reward串数组</param>
        /// <returns>减少成功返回true</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReduceReward(List<Vector3> rewards)
        {
            if (!IsRewardsEnough(rewards))
            {
                return false;
            }

            foreach (var reward in rewards)
            {
                var rewardx = (int)reward.x;
                var rewardy = (int)reward.y;
                var rewardz = -Mathf.Abs((int)reward.z);
                switch (rewardx)
                {
                    case 1:
                        ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy += rewardz;


                        break;
                    case 2:
                        ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin += rewardz;


                        break;
                    case 3:
                        ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill += rewardz;


                        break;
                    case 4:
                        ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp += rewardz;


                        break;
                    case 5:
                        if (ResourcesSingleton.Instance.items.ContainsKey(rewardy))
                        {
                            ResourcesSingleton.Instance.items[rewardy] += rewardz;
                            if (ResourcesSingleton.Instance.items[rewardy] == 0)
                            {
                                ResourcesSingleton.Instance.items.Remove(rewardy);
                            }
                        }
                        else
                        {
                            Log.Debug($"{reward} 缓存的items没有这个reward");
                        }

                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        break;
                    case 10:
                        break;
                    case 11:
                        //增加装备
                        break;
                    case 12:
                        break;
                    case 13:
                        break;
                    case 14:
                        break;
                    case 15:
                        break;
                    default:
                        Log.Debug($"{reward} 当前reward串没有定义");

                        break;
                }
            }

            ResourcesSingleton.Instance.UpdateResourceUI();

            return true;
        }


        /// <summary>
        /// 判断reward是否足够
        /// </summary>
        /// <param name="rewards">reward串</param>
        /// <returns>bool</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRewardsEnough(Vector3 reward)
        {
            bool isEnough = true;

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;
            switch (rewardx)
            {
                case 1:

                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy < rewardz)
                    {
                        isEnough = false;
                    }

                    break;
                case 2:

                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin < rewardz)
                    {
                        isEnough = false;
                    }

                    break;
                case 3:

                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill < rewardz)
                    {
                        isEnough = false;
                    }

                    break;
                case 4:

                    if (ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp < rewardz)
                    {
                        isEnough = false;
                    }

                    break;
                case 5:
                    if (ResourcesSingleton.Instance.items.TryGetValue(rewardy, out var count))
                    {
                        if (count < rewardz)
                        {
                            isEnough = false;
                        }
                    }
                    else
                    {
                        isEnough = false;
                    }


                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    //增加装备
                    Log.Debug($"{reward} 装备数据参数不够，不能判断");
                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }

            return isEnough;
        }

        /// <summary>
        /// 判断reward是否足够
        /// </summary>
        /// <param name="rewards">reward串数组</param>
        /// <returns>bool</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRewardsEnough(List<Vector3> rewards)
        {
            bool isEnough = true;

            foreach (var reward in rewards)
            {
                if (!IsRewardsEnough(reward))
                {
                    isEnough = false;
                    break;
                }
            }

            return isEnough;
        }


        /// <summary>
        /// 获取reward串的缓存数量
        /// </summary>
        /// <param name="reward">reward串</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetRewardCount(Vector3 reward)
        {
            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            long count = 0;
            switch (rewardx)
            {
                case 1:
                    count = ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy;

                    break;
                case 2:
                    count = ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin;

                    break;
                case 3:

                    count = ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill;

                    break;
                case 4:
                    count = ResourcesSingleton.Instance.UserInfo.RoleAssets.Exp;

                    break;
                case 5:

                    if (ResourcesSingleton.Instance.items.ContainsKey(rewardy))
                    {
                        count = ResourcesSingleton.Instance.items[rewardy];
                    }
                    else
                    {
                        count = 0;
                    }

                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    //增加装备
                    Log.Debug($"装备不应使用reward串获取数量");

                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                default:
                    Log.Debug($"{reward} 当前reward串没有定义");

                    break;
            }

            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetRewardCountFormat(Vector3 reward)
        {
            return FormatNumber(GetRewardCount(reward));
        }


        public static void InitUIPosInfo()
        {
            var uiPosInfo = new UIPosInfo
            {
                KBtn_DiamondPos = default,
                KBtn_MoneyPos = default,
                KBagPos = default
            };
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui))
            {
                var uis = ui as UIPanel_JiyuGame;
                var KOptions = uis.GetFromReference(UIPanel_JiyuGame.KOptions);
                foreach (var child in KOptions.GetList().Children)
                {
                    var childs = child as UISubPanel_ToggleItem;
                    if (childs.sort == 2)
                    {
                        uiPosInfo.KBagPos = JiYuUIHelper.GetUIPos(childs);

                        break;
                    }
                }
            }

            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var uimain))
            {
                var uis = uimain as UIPanel_Main;
                var KBtn_Diamond = uis.GetFromReference(UIPanel_Main.KBtn_Diamond);
                var KBtn_Money = uis.GetFromReference(UIPanel_Main.KBtn_Money);
                uiPosInfo.KBtn_DiamondPos = JiYuUIHelper.GetUIPos(KBtn_Diamond);
                uiPosInfo.KBtn_MoneyPos = JiYuUIHelper.GetUIPos(KBtn_Money);

                uiPosInfo.KBtn_DiamondPos.x -= 100f;
                uiPosInfo.KBtn_MoneyPos.x -= 100f;
            }

            ResourcesSingleton.Instance.UIPosInfo = uiPosInfo;
        }

        /// <summary>
        /// 获取当前UI的基于Canvas的坐标
        /// 坐标原点为屏幕中心
        /// </summary>
        /// <param name="ui">当前UI</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetUIPos(UI ui)
        {
            Canvas canvas = ui.GameObject.transform.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Log.Error($"没找到父节点有Canvas组件");
                return default;
            }

            Vector3 uiPos =
                canvas.transform.InverseTransformPoint(ui.GetRectTransform().Position());

            return uiPos;
        }

        /// <summary>
        /// 获取GameObject基于Canvas的坐标
        /// </summary>
        /// <param name="go">当前GameObject</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetGOPos(GameObject go)
        {
            if (go == null)
            {
                Log.Error($"go 为空");
                return default;
            }

            Canvas canvas = go.transform.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Log.Error($"没找到父节点有Canvas组件");
                return default;
            }

            Vector3 uiPos =
                canvas.transform.InverseTransformPoint(go.transform.position);

            return uiPos;
        }


        /// <summary>
        /// 将字符串reward串列表转换为正常列表
        /// </summary>
        /// <param name="strList"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TurnList2StrReward(RepeatedField<string> oriList, List<Vector3> strList)
        {
            //var rewards = new RepeatedField<string>();
            foreach (var reward in strList)
            {
                oriList.Add($"{(int)reward.x};{(int)reward.y};{(int)reward.z}");
            }

            //return rewards;
        }

        /// <summary>
        /// 将字符串reward串列表转换为正常列表
        /// </summary>
        /// <param name="strList"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<Vector3> TurnStrReward2List(RepeatedField<string> strList)
        {
            var rewards = new List<Vector3>(strList.Count);
            foreach (var reward in strList)
            {
                Debug.Log($"reward{reward}");
                var rewardSplit = reward.Split(";");
                rewards.Add(new Vector3(int.Parse(rewardSplit[0]), int.Parse(rewardSplit[1]),
                    int.Parse(rewardSplit[2])));
            }

            return rewards;
        }

        /// <summary>
        /// 将字符串reward串列表转换为正常列表
        /// </summary>
        /// <param name="strList"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<Vector3> TurnStrReward2List(string strList)
        {
            var rewards = new List<Vector3>(1);
            var rewardSplit = strList.Split(";");
            rewards.Add(new Vector3(int.Parse(rewardSplit[0]), int.Parse(rewardSplit[1]),
                int.Parse(rewardSplit[2])));

            return rewards;
        }

        /// <summary>
        /// 将字符串reward串列表转换为正常列表
        /// </summary>
        /// <param name="strList"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<Vector3> TurnShopBoardCastStrReward2List(string shopBoardCastStr)
        {
            List<Vector3> result = new List<Vector3>();

            // 检查输入是否为空或无效
            if (string.IsNullOrEmpty(shopBoardCastStr))
            {
                Debug.LogError("输入字符串为空或无效");
                return result; // 返回空的列表
            }

            try
            {
                // 找到方括号内的部分
                int startIndex = shopBoardCastStr.IndexOf('[') + 1;
                int endIndex = shopBoardCastStr.IndexOf(']');
                if (startIndex == -1 || endIndex == -1)
                {
                    Debug.LogError("没有找到有效的方括号部分");
                    return result; // 返回空的列表
                }

                string vectorData = shopBoardCastStr.Substring(startIndex, endIndex - startIndex);

                // 按逗号分割不同的Vector3部分
                string[] vectorParts = vectorData.Split(',');

                // 遍历每部分数据，转化为Vector3并添加到列表
                foreach (var part in vectorParts)
                {
                    // 分割每个Vector3的x, y, z值
                    string[] values = part.Split(';');
                    if (values.Length == 3)
                    {
                        try
                        {
                            float x = float.Parse(values[0]);
                            float y = float.Parse(values[1]);
                            float z = float.Parse(values[2]);

                            // 创建一个新的Vector3并添加到列表
                            result.Add(new Vector3(x, y, z));
                        }
                        catch (System.FormatException e)
                        {
                            Debug.LogError("无法解析向量值：" + part + "，错误：" + e.Message);
                        }
                    }
                    else
                    {
                        Debug.LogError("向量格式不正确：" + part);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("解析时发生异常：" + ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 根据当前时间和结束时间拿到剩余时间的合理字符串
        /// </summary>
        /// <param name="nowTime">当前时间戳 /s</param>
        /// <param name="endTime">结束时间戳 /s</param>
        /// <param name="timeStr">时间字符串</param>
        /// <returns>true则没有超时</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetRemainingTime(long nowTime, long endTime, out string timeStr)
        {
            timeStr = "";
            if (endTime < nowTime)
            {
                timeStr = JiYuUIHelper.GeneralTimeFormat(new Unity.Mathematics.int4(2, 3, 2, 1), 0);
                return false;
            }
            else if (endTime - 24 * 3600 < nowTime)
            {
                timeStr = timeStr +
                          JiYuUIHelper.GeneralTimeFormat(new Unity.Mathematics.int4(2, 3, 2, 1), endTime - nowTime);
            }
            else
            {
                timeStr = timeStr +
                          JiYuUIHelper.GeneralTimeFormat(new Unity.Mathematics.int4(3, 4, 2, 1), endTime - nowTime);
            }

            return true;
        }

        /// <summary>
        /// 通用时间格式显示
        /// </summary>
        /// <param name="args">x:最小时间单位
        ///                    y:最大时间单位
        ///                    z:末位格式规则	
        ///                    w:多语言格式
        /// </param>
        /// <param name="timeStamp">时间戳 单位：秒</param>
        /// <returns>通用时间显示字符串</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GeneralTimeFormat(int4 args, long timeStamp)
        {
            if (timeStamp < 0 || args.x <= 0 || args.y <= 0)
            {
                Log.Debug($"时间参数错误:{args}timeStamp:{timeStamp}", Color.magenta);
                return null;
            }

            if (args.x > args.y || (args.x < 1 && args.x > 4) || (args.y < 1 && args.y > 4) ||
                (args.z < 1 && args.z > 2) || (args.w < 1 && args.w > 2))
            {
                Log.Debug($"传入时间参数错误:{args}", Color.magenta);
                return null;
            }

            var language = ConfigManager.Instance.Tables.Tblanguage;
            var secondLang = language.Get($"time_second_{args.w}").current;
            var minuteLang = language.Get($"time_minute_{args.w}").current;
            var hourLang = language.Get($"time_hour_{args.w}").current;
            var dayLang = language.Get($"time_day_{args.w}").current;


            double seconds = (double)timeStamp;
            int maxUnit = args.y;
            int minUnit = args.x;
            bool allShow = args.z == 1 ? true : false;

            string output = default;
            if (maxUnit == minUnit)
            {
                switch (maxUnit)
                {
                    case 1:
                        output = $"{seconds}{secondLang}";
                        break;
                    case 2:
                        var minutes = (long)(seconds / 60);
                        output = $"{minutes}{minuteLang}";
                        break;
                    case 3:
                        var hours = (long)(seconds / (60 * 60));
                        output = $"{hours}{hourLang}";
                        break;
                    case 4:
                        var days = (long)(seconds / (24 * 60 * 60));
                        output = $"{days}{dayLang}";
                        break;
                }
            }
            else
            {
                long days = default, minutes = default, hours = default;
                switch (maxUnit)
                {
                    case 2:
                        minutes = (long)(seconds / 60);
                        seconds %= 60;
                        seconds = (long)seconds;

                        switch (minUnit)
                        {
                            case 1:
                                if (!allShow && seconds == 0)
                                {
                                    output = $"{minutes}{minuteLang}";
                                }
                                else
                                {
                                    output = $"{minutes}{minuteLang}{seconds}{secondLang}";
                                }

                                break;
                            case 2:

                                break;
                            case 3:

                                break;
                        }

                        break;
                    case 3:
                        hours = (long)(seconds / (60 * 60));
                        seconds %= (60 * 60);
                        minutes = (long)(seconds / 60);
                        seconds %= 60;
                        seconds = (long)seconds;
                        switch (minUnit)
                        {
                            case 1:
                                if (!allShow && seconds == 0)
                                {
                                    output = $"{hours}{hourLang}{minutes}{minuteLang}";
                                }
                                else
                                {
                                    output = $"{hours}{hourLang}{minutes}{minuteLang}{seconds}{secondLang}";
                                }


                                break;
                            case 2:
                                if (!allShow && minutes == 0)
                                {
                                    output = $"{hours}{hourLang}";
                                }
                                else
                                {
                                    output = $"{hours}{hourLang}{minutes}{minuteLang}";
                                }

                                break;
                            case 3:

                                break;
                        }

                        break;
                    case 4:
                        days = (long)(seconds / (24 * 60 * 60));
                        seconds %= (24 * 60 * 60);
                        hours = (long)(seconds / (60 * 60));
                        seconds %= (60 * 60);
                        minutes = (long)(seconds / 60);
                        seconds %= 60;
                        seconds = (long)seconds;
                        switch (minUnit)
                        {
                            case 1:
                                if (!allShow && seconds == 0)
                                {
                                    output = $"{days}{dayLang}{hours}{hourLang}{minutes}{minuteLang}";
                                }
                                else
                                {
                                    output =
                                        $"{days}{dayLang}{hours}{hourLang}{minutes}{minuteLang}{seconds}{secondLang}";
                                }

                                break;
                            case 2:
                                if (!allShow && minutes == 0)
                                {
                                    output = $"{days}{dayLang}{hours}{hourLang}";
                                }
                                else
                                {
                                    output = $"{days}{dayLang}{hours}{hourLang}{minutes}{minuteLang}";
                                }

                                break;
                            case 3:
                                if (!allShow && hours == 0)
                                {
                                    output = $"{days}{dayLang}";
                                }
                                else
                                {
                                    output = $"{days}{dayLang}{hours}{hourLang}";
                                }

                                break;
                        }

                        break;
                }
                // long days = (long)(seconds / (24 * 60 * 60));
                // seconds %= (24 * 60 * 60);
                //
                // long hours = (long)(seconds / (60 * 60));
                // seconds %= (60 * 60);
                //
                // long minutes = (long)(seconds / 60);
                // seconds %= 60;
                // seconds = (long)seconds;

                //output = $"{days}{dayLang}{hours}{hourLang}{minutes}{minuteLang}{seconds}{secondLang}";
            }


            return output;
        }


        #region ServerTime

        /// <summary>
        /// 获取服务器时间戳
        /// </summary>
        /// <param name="inSeconds">单位为秒</param>
        /// <returns>服务器时间戳</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetServerTimeStamp(bool inSeconds = false)
        {
            var serverMil = TimeHelper.ClientNow() - ResourcesSingleton.Instance.serverDeltaTime;
            serverMil = inSeconds ? (serverMil / 1000) : serverMil;
            return serverMil;
        }

        #endregion

        /// <summary>
        /// 设置通用弹出TipUI的位置
        /// 预制件参考:Common_ItemTips
        /// </summary>
        /// <param name="itemUI">物品UI,通常是点击此物品才弹出TipUI</param>
        /// <param name="tipUI">弹出的TipUI</param>
        /// <param name="contentKey">正文气泡内容GO的key</param>
        /// <param name="arrowKey">Tip窗口正下方的箭头GO的key</param>
        /// <param name="contentGap">正文内容距离屏幕左右的间隔,单位:像素</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetTipPos(UI itemUI, UI tipUI, string contentKey, string arrowDownKey, string arrowUpKey,
            float contentGap = 30f)
        {
            var arrowDown = tipUI.GetFromReference(arrowDownKey);
            var arrowUp = tipUI.GetFromReference(arrowUpKey);
            var content = tipUI.GetFromReference(contentKey);

            var itemRect = itemUI.GetRectTransform();
            var tipRect = tipUI.GetRectTransform();
            bool isUp = false;

            isUp = GetUIPos(itemUI).y > Screen.height / 4f ? true : false;

            string contentImg = isUp ? "bg_tip_up" : "bg_tip_bottom";
            content.GetImage().SetSpriteAsync(contentImg, false).Forget();
            ScrollRect scrollRect = itemUI.GameObject.transform.GetComponentInParent<ScrollRect>();

            Canvas canvas = itemUI.GameObject.transform.GetComponentInParent<Canvas>();

            // var rewardPosX = itemRect.AnchoredPosition().x;
            // var parentPos = scrollRect.content.anchoredPosition;


            RectTransform canvasRect = canvas.transform.GetComponent<RectTransform>();
            Vector3 loadpos = canvas.transform.InverseTransformPoint(itemUI.GameObject.transform.position);


            if (scrollRect != null)
            {
                Vector3 scrollRectPos =
                    canvas.transform.InverseTransformPoint(scrollRect.transform.position);
                var scrollRectWidth = scrollRect.transform.GetComponent<RectTransform>().rect.width;
                var scrollRectLeftPos = scrollRectPos.x - scrollRectWidth / 2f;
                var scrollRectRightPos = scrollRectPos.x + scrollRectWidth / 2f;

                loadpos.x = math.clamp(loadpos.x, scrollRectLeftPos, scrollRectRightPos);
            }

            var itemUpOffset = itemRect.Width() / 2f * itemRect.Scale().x;
            var tipUpOffset = tipRect.Height() / 2f * tipRect.Scale().y;

            float offsetY = itemUpOffset + tipUpOffset;

            if (isUp)
            {
                offsetY -= itemRect.Width() * itemRect.Scale().x + tipRect.Height() * tipRect.Scale().y;
                arrowDown.SetActive(false);
                arrowUp.SetActive(true);
            }
            else
            {
                arrowDown.SetActive(true);
                arrowUp.SetActive(false);
            }


            var tipPos = new Vector3(loadpos.x, loadpos.y + offsetY);


            tipUI.GetRectTransform().SetAnchoredPosition(tipPos);
            var tipWidth = tipRect.Width() * tipRect.Scale().x;

            var arrow = tipUI.GetFromReference(arrowDownKey).GetRectTransform();
            var arrowWidth = arrow.Width() * arrow.Scale().x;


            var screenPosL = -(Screen.width / 2f);
            var screenPosR = Screen.width / 2f;
            var tipPosL = loadpos.x - tipWidth / 2;
            var tipPosR = loadpos.x + tipWidth / 2;
            var contentRect = tipUI.GetFromReference(contentKey).GetRectTransform();

            if (tipPosL < screenPosL + contentGap)
            {
                var contentPos = math.length(tipPosL) - math.length(screenPosL + contentGap);
                contentPos = math.min(contentPos, tipWidth / 2f - arrowWidth / 2f);

                contentRect.SetAnchoredPosition(new Vector2(contentPos, 0));
            }
            else if (tipPosR > screenPosR - contentGap)
            {
                var contentPos = math.length(tipPosR) - math.length(screenPosR - contentGap);
                contentPos = math.min(contentPos, tipWidth / 2f - arrowWidth / 2f);

                contentRect.SetAnchoredPosition(new Vector2(-contentPos, 0));
            }
            else
            {
                contentRect.SetAnchoredPosition(Vector2.zero);
            }
        }

        /// <summary>
        /// 设置通用弹出TipUI的位置
        /// 预制件参考:Common_ItemTips
        /// </summary>
        /// <param name="itemUI">物品UI,通常是点击此物品才弹出TipUI</param>
        /// <param name="tipUI">弹出的TipUI</param>
        /// <param name="contentKey">正文气泡内容GO的key</param>
        /// <param name="arrowKey">Tip窗口正下方的箭头GO的key</param>
        /// <param name="contentGap">正文内容距离屏幕左右的间隔,单位:像素</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetTipPosAndResize(UI itemUI, UI tipUI, string contentKey = UICommon_ItemTips.KContent,
            string arrowDownKey = UICommon_ItemTips.KImg_ArrowDown, string arrowUpKey = UICommon_ItemTips.KImg_ArrowUp,
            UICommon_ItemTips.ArrowType arrowType = UICommon_ItemTips.ArrowType.Default, float contentGap = 30f)
        {
            const float DefaultContentPosY = -52;
            var arrowDown = tipUI.GetFromReference(arrowDownKey);
            var arrowUp = tipUI.GetFromReference(arrowUpKey);
            var content = tipUI.GetFromReference(contentKey);

            //var KContent = tipUI.GetFromReference(UICommon_ItemTips.KContent);
            var KTxt_Title = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Title);
            var KTxt_Des = tipUI.GetFromReference(UICommon_ItemTips.KTxt_Des);

            var titleRect = KTxt_Title.GetRectTransform();
            var desRect = KTxt_Des.GetRectTransform();


            var itemRect = itemUI.GetRectTransform();
            var tipRect = tipUI.GetRectTransform();
            bool isUp = false;
            switch (arrowType)
            {
                case UICommon_ItemTips.ArrowType.Default:
                    isUp = GetUIPos(itemUI).y > Screen.height / 4f ? true : false;

                    break;
                case UICommon_ItemTips.ArrowType.Up:
                    isUp = true;

                    break;
                case UICommon_ItemTips.ArrowType.Down:
                    isUp = false;

                    break;
            }

            isUp = GetUIPos(itemUI).y > Screen.height / 4f ? true : false;

            string contentImg = isUp ? "bg_tip_up" : "bg_tip_bottom";
            content.GetImage().SetSpriteAsync(contentImg, false).Forget();

            ScrollRect scrollRect = itemUI.GameObject.transform.GetComponentInParent<ScrollRect>();

            Canvas canvas = itemUI.GameObject.transform.GetComponentInParent<Canvas>();

            // var rewardPosX = itemRect.AnchoredPosition().x;
            // var parentPos = scrollRect.content.anchoredPosition;


            RectTransform canvasRect = canvas.transform.GetComponent<RectTransform>();
            Vector3 loadpos = canvas.transform.InverseTransformPoint(itemUI.GameObject.transform.position);


            if (scrollRect != null)
            {
                Vector3 scrollRectPos =
                    canvas.transform.InverseTransformPoint(scrollRect.transform.position);
                var scrollRectWidth = scrollRect.transform.GetComponent<RectTransform>().rect.width;
                var scrollRectLeftPos = scrollRectPos.x - scrollRectWidth / 2f;
                var scrollRectRightPos = scrollRectPos.x + scrollRectWidth / 2f;

                loadpos.x = math.clamp(loadpos.x, scrollRectLeftPos, scrollRectRightPos);
            }

            var itemUpOffset = itemRect.Height() / 2f;
            var tipUpOffset = tipRect.Height() / 2f;

            float offsetY = itemUpOffset + tipUpOffset;
            var contentRect = content.GetRectTransform();
            if (isUp)
            {
                offsetY = -offsetY;
                arrowDown.SetActive(false);
                arrowUp.SetActive(true);
                //contentRect.SetAnchoredPositionY(DefaultContentPosY);
            }
            else
            {
                var offsetArrow = arrowDown.GetRectTransform().AnchoredPosition().y;

                arrowDown.GetRectTransform().SetAnchoredPositionY(0);

                //contentRect.SetAnchoredPositionY(-offsetArrow + contentRect.AnchoredPosition().y);
                //offsetY += itemRect.Height() + tipRect.Height();
                arrowDown.SetActive(true);
                arrowUp.SetActive(false);
            }


            var tipPos = new Vector3(loadpos.x, loadpos.y + offsetY);


            tipUI.GetRectTransform().SetAnchoredPosition(tipPos);
            var tipWidth = tipRect.Width();

            var arrow = tipUI.GetFromReference(arrowDownKey).GetRectTransform();
            var arrowWidth = arrow.Width();


            var screenPosL = -(Screen.width / 2f);
            var screenPosR = Screen.width / 2f;
            var tipPosL = loadpos.x - tipWidth / 2;
            var tipPosR = loadpos.x + tipWidth / 2;


            contentRect.SetAnchoredPositionX(0);
            //SetContentPosX
            if (tipPosL < screenPosL + contentGap)
            {
                var contentPos = math.length(tipPosL) - math.length(screenPosL + contentGap);
                contentPos = math.min(contentPos, tipWidth / 2f - arrowWidth / 2f);
                contentRect.SetAnchoredPositionX(contentPos);
                //contentRect.SetAnchoredPosition(new Vector2(contentPos, DefaultContentPosY));
            }
            else if (tipPosR > screenPosR - contentGap)
            {
                var contentPos = math.length(tipPosR) - math.length(screenPosR - contentGap);
                contentPos = math.min(contentPos, tipWidth / 2f - arrowWidth / 2f);

                contentRect.SetAnchoredPositionX(-contentPos);
            }

            //SetHeightAndContentPosY
            if (KTxt_Title.GameObject.activeSelf)
            {
                titleRect.SetHeight(KTxt_Title.GetTextMeshPro().Get().preferredHeight + 10f);
            }
            else
            {
                titleRect.SetHeight(0f);
                desRect.SetAnchoredPositionY(-10f);
            }


            desRect.SetHeight(KTxt_Des.GameObject.activeSelf
                ? KTxt_Des.GetTextMeshPro().Get().preferredHeight + 10f
                : 0f);

            titleRect.SetOffsetWithLeft(15f);
            titleRect.SetOffsetWithRight(-15f);
            desRect.SetOffsetWithLeft(15f);
            desRect.SetOffsetWithRight(-15f);

            var contentHeight = titleRect.Height() +
                                desRect.Height() + 22f;

            content.GetRectTransform().SetHeight(contentHeight);

            contentRect.SetAnchoredPositionY(-52f + (contentRect.Height() - 158f));
        }


        /// <summary>
        /// 设置通用弹出TipUI的位置
        /// 预制件参考:Common_ItemTips
        /// </summary>
        /// <param name="itemUI">物品UI,通常是点击此物品才弹出TipUI</param>
        /// <param name="tipUI">弹出的TipUI</param>
        /// <param name="contentKey">正文气泡内容GO的key</param>
        /// <param name="arrowKey">Tip窗口正下方的箭头GO的key</param>
        /// <param name="contentGap">正文内容距离屏幕左右的间隔,单位:像素</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetTipPosNew(UI itemUI, UI tipUI, string contentKey = UICommon_ItemTips.KContent,
            string arrowDownKey = UICommon_ItemTips.KImg_ArrowDown, string arrowUpKey = UICommon_ItemTips.KImg_ArrowUp,
            UICommon_ItemTips.ArrowType arrowType = UICommon_ItemTips.ArrowType.Default, float contentGap = 30f)
        {
            const float DefaultContentPosY = -52;
            var arrowDown = tipUI.GetFromReference(arrowDownKey);
            var arrowUp = tipUI.GetFromReference(arrowUpKey);
            var content = tipUI.GetFromReference(contentKey);

            var itemRect = itemUI.GetRectTransform();
            var tipRect = tipUI.GetRectTransform();
            bool isUp = false;
            switch (arrowType)
            {
                case UICommon_ItemTips.ArrowType.Default:
                    isUp = GetUIPos(itemUI).y > Screen.height / 4f ? true : false;

                    break;
                case UICommon_ItemTips.ArrowType.Up:
                    isUp = true;

                    break;
                case UICommon_ItemTips.ArrowType.Down:
                    isUp = false;

                    break;
            }

            isUp = GetUIPos(itemUI).y > Screen.height / 4f ? true : false;

            string contentImg = isUp ? "bg_tip_up" : "bg_tip_bottom";
            content.GetImage().SetSpriteAsync(contentImg, false).Forget();

            ScrollRect scrollRect = itemUI.GameObject.transform.GetComponentInParent<ScrollRect>();

            Canvas canvas = itemUI.GameObject.transform.GetComponentInParent<Canvas>();

            // var rewardPosX = itemRect.AnchoredPosition().x;
            // var parentPos = scrollRect.content.anchoredPosition;


            RectTransform canvasRect = canvas.transform.GetComponent<RectTransform>();
            Vector3 loadpos = canvas.transform.InverseTransformPoint(itemUI.GameObject.transform.position);


            if (scrollRect != null)
            {
                Vector3 scrollRectPos =
                    canvas.transform.InverseTransformPoint(scrollRect.transform.position);
                var scrollRectWidth = scrollRect.transform.GetComponent<RectTransform>().rect.width;
                var scrollRectLeftPos = scrollRectPos.x - scrollRectWidth / 2f;
                var scrollRectRightPos = scrollRectPos.x + scrollRectWidth / 2f;

                loadpos.x = math.clamp(loadpos.x, scrollRectLeftPos, scrollRectRightPos);
            }

            var itemUpOffset = itemRect.Height() / 2f;
            var tipUpOffset = tipRect.Height() / 2f;

            float offsetY = itemUpOffset + tipUpOffset;
            var contentRect = content.GetRectTransform();
            if (isUp)
            {
                offsetY = -offsetY;
                arrowDown.SetActive(false);
                arrowUp.SetActive(true);
                //contentRect.SetAnchoredPositionY(DefaultContentPosY);
            }
            else
            {
                var offsetArrow = arrowDown.GetRectTransform().AnchoredPosition().y;

                arrowDown.GetRectTransform().SetAnchoredPositionY(0);

                //contentRect.SetAnchoredPositionY(-offsetArrow + contentRect.AnchoredPosition().y);
                //offsetY += itemRect.Height() + tipRect.Height();
                arrowDown.SetActive(true);
                arrowUp.SetActive(false);
            }


            var tipPos = new Vector3(loadpos.x, loadpos.y + offsetY);


            tipUI.GetRectTransform().SetAnchoredPosition(tipPos);
            var tipWidth = tipRect.Width();

            var arrow = tipUI.GetFromReference(arrowDownKey).GetRectTransform();
            var arrowWidth = arrow.Width();


            var screenPosL = -(Screen.width / 2f);
            var screenPosR = Screen.width / 2f;
            var tipPosL = loadpos.x - tipWidth / 2;
            var tipPosR = loadpos.x + tipWidth / 2;


            if (tipPosL < screenPosL + contentGap)
            {
                var contentPos = math.length(tipPosL) - math.length(screenPosL + contentGap);
                contentPos = math.min(contentPos, tipWidth / 2f - arrowWidth / 2f);
                contentRect.SetAnchoredPositionX(contentPos);
                //contentRect.SetAnchoredPosition(new Vector2(contentPos, DefaultContentPosY));
            }
            else if (tipPosR > screenPosR - contentGap)
            {
                var contentPos = math.length(tipPosR) - math.length(screenPosR - contentGap);
                contentPos = math.min(contentPos, tipWidth / 2f - arrowWidth / 2f);

                contentRect.SetAnchoredPositionX(-contentPos);
            }


            contentRect.SetAnchoredPositionY(-52f + (contentRect.Height() - 158f));
        }

        /// <summary>
        /// KM换算
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatNumber(int value)
        {
            if (value >= 1000 && value < 1000000)
            {
                // 将数字除以1000，并保留一位小数
                double formattedValue = System.Math.Round((double)value / 1000, 1);
                return $"{formattedValue}K";
            }
            else if (value >= 1000000)
            {
                // 将数字除以1000000，并保留一位小数
                double formattedValue = System.Math.Round((double)value / 1000000, 1);
                return $"{formattedValue}M";
            }
            else
            {
                return $"{value}";
            }
        }

        /// <summary>
        /// KM换算
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatNumber(long value)
        {
            if (value >= 1000 && value < 1000000)
            {
                // 将数字除以1000，并保留一位小数
                double formattedValue = System.Math.Round((double)value / 1000, 1);
                return $"{formattedValue}K";
            }
            else if (value >= 1000000)
            {
                // 将数字除以1000000，并保留一位小数
                double formattedValue = System.Math.Round((double)value / 1000000, 1);
                return $"{formattedValue}M";
            }
            else
            {
                return $"{value}";
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetCountText(UI countUI, int count)
        {
            countUI.GetTextMeshPro().SetTMPText($"x {FormatNumber(count)}");
            countUI.SetActive(count > 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsResourceReward(Vector3 reward)
        {
            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;
            if (rewardx >= 1 && rewardx <= 4)
                return true;
            return false;
        }


        /// <summary>
        /// 设置对应reward串的提示语的标题和描述
        /// </summary>
        /// <param name="reward">reward串</param>
        /// <param name="title">对应title的ui</param>
        /// <param name="des">对应des的ui</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetRewardOnClickTipTitleDes(Vector3 reward, UI title, UI des, bool showQuality = true)
        {
            var language = ConfigManager.Instance.Tables.Tblanguage;
            var user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            var item = ConfigManager.Instance.Tables.Tbitem;
            var equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            var equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            var quality = ConfigManager.Instance.Tables.Tbquality;

            var rewardx = (int)reward.x;
            var rewardy = (int)reward.y;
            var rewardz = (int)reward.z;
            var titleStr = GetRewardName(reward, showQuality);
            var desStr = GetRewardDes(reward);
            title?.GetTextMeshPro().SetTMPText(titleStr);
            des?.GetTextMeshPro().SetTMPText(desStr);
        }

        /// <summary>
        /// 迅捷蟹
        /// 设置通用弹出TipsUI的位置,使用公用UI,且为相对位置
        /// </summary>
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public static void SetTipPos(UI tipUI, string contentKey, string arrowKey, float contentGap = 30f)
        // {
        //     UI ContentUI = tipUI.GetFromReference(contentKey);
        //     UI arrowUI = tipUI.GetFromReference(arrowKey);
        //
        //     Vector3 ContentRect = ContentUI.GetComponent<RectTransform>().anchoredPosition;
        //     Vector3 arrowRect = arrowUI.GetComponent<RectTransform>().anchoredPosition;
        //
        //     ContentRect = new Vector3(ContentRect.x - contentGap, ContentRect.y, ContentRect.z);
        //     arrowRect = new Vector3(ContentRect.x + contentGap, arrowRect.y, arrowRect.z);
        //
        //     tipUI.GetRectTransform(contentKey).SetAnchoredPosition3D(ContentRect);
        //     tipUI.GetRectTransform(arrowKey).SetAnchoredPosition3D(arrowRect);
        // }

        // 排序比较器
        public class RewardsComparer : IComparer<Vector3>
        {
            Tblanguage language = ConfigManager.Instance.Tables.Tblanguage;
            Tbuser_variable user_varibles = ConfigManager.Instance.Tables.Tbuser_variable;
            Tbitem item = ConfigManager.Instance.Tables.Tbitem;
            Tbequip_data equip_data = ConfigManager.Instance.Tables.Tbequip_data;
            Tbequip_quality equip_quality = ConfigManager.Instance.Tables.Tbequip_quality;
            Tbquality quality = ConfigManager.Instance.Tables.Tbquality;

            public int Compare(Vector3 obj1, Vector3 obj2)
            {
                var obj1rewardx = (int)obj1.x;
                var obj1rewardy = (int)obj1.y;
                var obj1rewardz = (int)obj1.z;
                var obj2rewardx = (int)obj2.x;
                var obj2rewardy = (int)obj2.y;
                var obj2rewardz = (int)obj2.z;

                if (obj1rewardx == 11 && obj2rewardx != 11)
                    return -1;
                else if (obj1rewardx != 11 && obj2rewardx == 11)
                    return 1;

                if (obj1rewardx != 5 && obj2rewardx == 5)
                    return -1;
                else if (obj1rewardx == 5 && obj2rewardx != 5)
                    return 1;

                if (obj1rewardx == 11 && obj2rewardx == 11)
                {
                    if (!IsCompositeEquipReward(obj1) &&
                        IsCompositeEquipReward(obj2))
                        return -1;
                    else if (IsCompositeEquipReward(obj1) &&
                             !IsCompositeEquipReward(obj2))
                        return 1;

                    if (equip_data.Get(obj1rewardy).quality >
                        equip_data.Get(obj2rewardy).quality)
                        return -1;
                    else if (equip_data.Get(obj1rewardy).quality <
                             equip_data.Get(obj2rewardy).quality)
                        return 1;

                    if (equip_data.Get(obj1rewardy).sYn == 1 &&
                        equip_data.Get(obj2rewardy).sYn != 1)
                        return -1;
                    else if (equip_data.Get(obj1rewardy).sYn != 1 &&
                             equip_data.Get(obj2rewardy).sYn == 1)
                        return 1;


                    if (equip_data.Get(obj1rewardy).posId <
                        equip_data.Get(obj2rewardy).posId)
                        return -1;
                    else if (equip_data.Get(obj1rewardy).posId >
                             equip_data.Get(obj2rewardy).posId)
                        return 1;

                    if (obj1rewardy > obj2rewardy)
                        return -1;
                    else if (obj1rewardy < obj2rewardy)
                        return 1;
                }

                if (IsResourceReward(obj1) && IsResourceReward(obj2))
                {
                    if (obj1rewardx < obj2rewardx)
                        return -1;
                    else if (obj1rewardx > obj2rewardx)
                        return 1;
                }

                if (obj1rewardx == 5 && obj2rewardx == 5)
                {
                    if (item.Get(obj1rewardy).sort < item.Get(obj2rewardy).sort)
                        return -1;
                    else if (item.Get(obj1rewardy).sort > item.Get(obj2rewardy).sort)
                        return 1;
                }

                return 0;
            }
        }

        /// <summary>
        /// 剔除rewardList中数量为0的reward串
        /// </summary>
        /// <param name="rewardList"></param>
        /// <returns></returns>
        public static List<Vector3> RewardRemoveEmptiness(List<Vector3> rewardList)
        {
            List<Vector3> vector3s = new List<Vector3>();
            for (int i = 0; i < rewardList.Count; i++)
            {
                if (rewardList[i].z == 0)
                {
                    vector3s.Add(rewardList[i]);
                }
            }

            for (int i = 0; i < vector3s.Count; i++)
            {
                rewardList.Remove(vector3s[i]);
            }

            return rewardList;
        }

        /// <summary>
        /// 通过链接下载文件到指定位置
        /// </summary>
        /// <param name="url">下载链接</param>
        /// <param name="localPath">存储地址</param>
        public static async UniTask DownloadNotice()
        {
            var url = JsonManager.Instance.noticeUrl + "/Notice.json";
            if (url == null)
            {
                Log.Debug("url is null");
                return;
            }

            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            //UnityWebRequest webRequest = new UnityWebRequest(url);
            //DownloadHandlerFile download = new DownloadHandlerFile(localPath);
            await webRequest.SendWebRequest().ToUniTask();
            // webRequest.downloadHandler = download;
            // await webRequest.SendWebRequest().ToUniTask();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                //webRequest.do
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"jsonResponse:{jsonResponse}");

                //string jsonData = await File.ReadAllTextAsync(jsonResponse).AsUniTask();
                var myNotice = JsonConvert.DeserializeObject<MyNoticeList>(jsonResponse);
                JsonManager.Instance.sharedData.noticesList = myNotice;
                JsonManager.Instance.SaveSharedData(JsonManager.Instance.sharedData);
                foreach (var noticeMulti in JsonManager.Instance.sharedData.noticesList.notices)
                {
                    var noticeSingle = noticeMulti.notices.Where(a => a.l10N == JsonManager.Instance.sharedData.l10N)
                        .FirstOrDefault();

                    string iconName = Path.GetFileName(noticeSingle.icon);
                    string iconPath = $"{JsonManager.Instance.sharedDataSavePath}/{iconName}";
                    //Log.Debug($"iconName{iconName} !File.Exists");

                    DownloadFileByUrl(noticeSingle.icon, iconPath);

                    string picName = Path.GetFileName(noticeSingle.pic);
                    string picPath = $"{JsonManager.Instance.sharedDataSavePath}/{picName}";

                    DownloadFileByUrl(noticeSingle.pic, picPath);
                }
            }
            else
            {
                //文件下载失败
                Debug.LogError("Request failed: " + webRequest.error);
            }
        }

        /// <summary>
        /// 通过链接下载文件到指定位置
        /// </summary>
        /// <param name="url">下载链接</param>
        /// <param name="localPath">存储地址</param>
        public static async UniTask DownloadShare()
        {
            var namepic = $"share_pic{ResourcesSingleton.Instance.gameShare.Id}.png";
            var nameicon = $"share_icon{ResourcesSingleton.Instance.gameShare.Id}.png";
            // var urlpic = $"{JsonManager.shareUrl}/share_icon{ResourcesSingleton.Instance.gameShare.Id}.png";
            // var urlicon = $"{JsonManager.shareUrl}/share_pic{ResourcesSingleton.Instance.gameShare.Id}.png";
            // var url = JsonManager.noticeUrl + "/Share.json";
            // if (url == null)
            // {
            //     Log.Debug("url is null");
            //     return;
            // }

            //UnityWebRequest webRequest = UnityWebRequest.Get(url);
            //UnityWebRequest webRequest = new UnityWebRequest(url);
            //DownloadHandlerFile download = new DownloadHandlerFile(localPath);
            //await webRequest.SendWebRequest().ToUniTask();
            // webRequest.downloadHandler = download;
            // await webRequest.SendWebRequest().ToUniTask();


            DownloadFileByUrl(JsonManager.Instance.shareUrl + "/" + namepic,
                JsonManager.Instance.sharedDataSavePath + "/" + namepic);


            DownloadFileByUrl(JsonManager.Instance.shareUrl + "/" + nameicon,
                JsonManager.Instance.sharedDataSavePath + "/" + nameicon);


            // if (webRequest.result == UnityWebRequest.Result.Success)
            // {
            //     //webRequest.do
            //     string jsonResponse = webRequest.downloadHandler.text;
            //     Debug.Log($"jsonResponse:{jsonResponse}");
            //
            //     //string jsonData = await File.ReadAllTextAsync(jsonResponse).AsUniTask();
            //     var myNotice = JsonConvert.DeserializeObject<MyNoticeList>(jsonResponse);
            //     JsonManager.Instance.sharedData.noticesList = myNotice;
            //     JsonManager.Instance.SaveSharedData(JsonManager.Instance.sharedData);
            //     foreach (var noticeMulti in JsonManager.Instance.sharedData.noticesList.notices)
            //     {
            //         var noticeSingle = noticeMulti.notices.Where(a => a.l10N == JsonManager.Instance.sharedData.l10N)
            //             .FirstOrDefault();
            //
            //         string iconName = Path.GetFileName(noticeSingle.icon);
            //         string iconPath = $"{JsonManager.Instance.sharedDataSavePath}/{iconName}";
            //         //Log.Debug($"iconName{iconName} !File.Exists");
            //
            //
            //         string picName = Path.GetFileName(noticeSingle.pic);
            //         string picPath = $"{JsonManager.Instance.sharedDataSavePath}/{picName}";
            //
            //         if (!File.Exists(picPath))
            //         {
            //             DownloadFileByUrl(noticeSingle.pic, picPath).Forget();
            //         }
            //     }
            // }
            // else
            // {
            //     //文件下载失败
            //     Debug.LogError("Request failed: " + webRequest.error);
            // }
        }

        /// <summary>
        /// 通过链接下载文件到指定位置
        /// </summary>
        /// <param name="url">下载链接</param>
        /// <param name="localPath">存储地址</param>
        public static async UniTask DownloadFileByUrl(string url, string localPath)
        {
            if (File.Exists(localPath))
            {
                Log.Debug($"localPath:{localPath} 已存在此文件");
                //return;
            }


            if (url == null || localPath == null)
            {
                Log.Debug("url or localPath is null");
                return;
            }

            bool isPic = false;
            if (url.Contains(".png") || url.Contains(".jpg"))
            {
                isPic = true;
            }

            //UnityWebRequest webRequest = UnityWebRequest.Get(url);
            UnityWebRequest webRequest = null;

            if (isPic)
            {
                webRequest = UnityWebRequestTexture.GetTexture(url);
            }
            else
            {
                webRequest = new UnityWebRequest(url);
            }

            DownloadHandlerFile download = new DownloadHandlerFile(localPath);
            webRequest.downloadHandler = download;
            await webRequest.SendWebRequest().ToUniTask();

            //Log.Debug($"responseCode {webRequest.responseCode}");
            if (webRequest.result == UnityWebRequest.Result.Success && webRequest.responseCode == 200)
            {
                //文件下载成功
                Log.Debug("下载成功");
            }
            else
            {
                download.Dispose();
                //文件下载失败
                Log.Debug("下载失败");
            }
        }

        /// <summary>
        /// 给一个有Image组件的ui加载本地图片
        /// </summary>
        /// <param name="name">图片名包含后缀</param>
        /// <param name="ui">ui</param>
        public static async UniTask LoadImage(string name, UI ui)
        {
            string path = $"{JsonManager.Instance.sharedDataSavePath}/{name}";
            // 检查文件是否存在
            if (System.IO.File.Exists(path))
            {
                // 读取文件字节
                byte[] fileData = await System.IO.File.ReadAllBytesAsync(path).AsUniTask();
                //var texture = await Resources.LoadAsync<Texture2D>(path) as Texture2D;

                // 创建 Texture2D 实例并加载图片
                Texture2D texture = new Texture2D(2, 2); // 创建一个临时的 2x2 大小的纹理
                if (texture.LoadImage(fileData)) // 将字节数组加载为图片
                    //if (texture) // 将字节数组加载为图片
                {
                    // 将加载的纹理转换为 Sprite
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));

                    // 将 Sprite 设置给 Image 组件
                    ui.GetImage().SetOverrideSprite(sprite, true);
                }
                else
                {
                    Debug.LogError("无法加载图片！");
                }
            }
            else
            {
                Debug.LogError("图片文件不存在！");
            }
        }


        public static void SetGrayOrNot(UI btn, string gray, string normal, bool isGray = false)
        {
            if (isGray)
            {
                btn.GetImage().SetSpriteAsync(gray, false);
            }
            else
            {
                btn.GetImage().SetSpriteAsync(normal, false);
            }
        }

        /// <summary>
        /// 将输入的string裁剪为最后一个“/”后的内容
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        // public static string GetNameStringFormUrl(string input)
        // {
        //     if (input == null)
        //     {
        //         Log.Error("the input url is null");
        //         return null;
        //     }
        //     else
        //     {
        //         int lastSlashIndex = input.LastIndexOf("/");
        //         if (lastSlashIndex != -1)
        //         {
        //             return input.Substring(lastSlashIndex + 1);
        //         }
        //         else
        //         {
        //             Log.Debug("input do not have \"/\"");
        //             return input;
        //         }
        //     }
        // }

        /// <summary>
        /// 把List<notice>转变为Dictionary<id, Notice>方便查找
        /// </summary>
        /// <param name="input">输入的List<notice></param>
        /// <returns>Dictionary<id, Notice></returns>
        // public static Dictionary<int, Notice> NoticeListToDic(List<Notice> input)
        // {
        //     if (input == null)
        //     {
        //         return null;
        //     }
        //
        //     Dictionary<int, Notice> resoult = new Dictionary<int, Notice>();
        //     for (int i = 0; i < input.Count; i++)
        //     {
        //         resoult.Add(input[i].id, input[i]);
        //     }
        //
        //     return resoult;
        // }

        /// <summary>
        /// 根据id查找notice并且修改其阅读状态为已读(readStatus = 1)
        /// </summary>
        /// <param name="id"></param>
        // public static void SetNoticeReadStatusByID(int id)
        // {
        //     if (JsonManager.Instance.sharedData.noticesList == null)
        //     {
        //         return;
        //     }
        //     else
        //     {
        //         if (JsonManager.Instance.sharedData.noticesList.notices == null)
        //         {
        //             return;
        //         }
        //         else
        //         {
        //             if (JsonManager.Instance.sharedData.noticesList.notices.Count == 0)
        //             {
        //                 return;
        //             }
        //             else
        //             {
        //                 bool idExists = false;
        //                 for (int i = 0; i < JsonManager.Instance.sharedData.noticesList.notices.Count; i++)
        //                 {
        //                     if (JsonManager.Instance.sharedData.noticesList.notices[i].id == id)
        //                     {
        //                         JsonManager.Instance.sharedData.noticesList.notices[i].readStatus = 1;
        //                         idExists = true;
        //                         break;
        //                     }
        //                 }
        //
        //                 if (idExists)
        //                 {
        //                     JsonManager.Instance.SavePlayerData(JsonManager.Instance.userData);
        //                 }
        //                 else
        //                 {
        //                     Log.Debug("input id does not exist");
        //                 }
        //             }
        //         }
        //     }
        // }

        /// <summary>
        /// 处理字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HandleStr(string str)
        {
            const string regex = @"\\(u|x)[[a-z\d]{1,4}";

            str = Regex.Replace(str, regex, string.Empty);
            str = Regex.Replace(str, @"\p{C}+", string.Empty);

            return str;
        }


        public static void SetResourceNotEnoughTip(UI notEnoughTipUI, UI btnUI, float contentGap = 30f)
        {
            var itemPos = JiYuUIHelper.GetUIPos(btnUI);
            float btnH = btnUI.GetRectTransform().Height();
            float tipMidH = notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_Back).GetRectTransform()
                .Height();
            float tipDownH = notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_TipDown).GetRectTransform()
                .Height();
            float tipUpH = notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_TipUp).GetRectTransform()
                .Height();
            float tipW = notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_Back).GetRectTransform()
                .Width();
            float screeenH = Screen.height;

            if (itemPos.y + tipMidH + tipDownH + btnH / 2 < screeenH / 2)
            {
                //up
                notEnoughTipUI.GetRectTransform().SetAnchoredPositionY(itemPos.y + tipDownH + tipMidH / 2 + btnH / 2);
                notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.Kbarbottom).SetActive(true);
                notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.Kbarup).SetActive(false);
            }
            else
            {
                //down
                notEnoughTipUI.GetRectTransform().SetAnchoredPositionY(itemPos.y - tipUpH - tipMidH / 2 - btnH / 2);
                notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.Kbarbottom).SetActive(false);
                notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.Kbarup).SetActive(true);
            }

            float screenW = Screen.width;

            if (itemPos.x - tipW / 2 - contentGap < -screenW / 2)
            {
                notEnoughTipUI.GetRectTransform().SetAnchoredPositionX(-screenW / 2 + contentGap + tipW / 2);
                notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_TipDown).GetRectTransform()
                    .SetAnchoredPositionX(itemPos.x - (-screenW / 2 + contentGap + tipW / 2));
                notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_TipUp).GetRectTransform()
                    .SetAnchoredPositionX(itemPos.x - (-screenW / 2 + contentGap + tipW / 2));
            }
            else if (itemPos.x + tipW / 2 + contentGap > screenW / 2)
            {
                var tipPos = screenW / 2 - contentGap - tipW / 2;
                notEnoughTipUI.GetRectTransform().SetAnchoredPositionX(tipPos);
                //notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_TipDown).GetRectTransform()
                //   .SetAnchoredPositionX(itemPos.x);
                //notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_TipUp).GetRectTransform()
                //    .SetAnchoredPositionX(itemPos.x);


                notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_TipDown).GetRectTransform()
                    .SetAnchoredPositionX(itemPos.x - (screenW / 2 - contentGap - tipW / 2));
                notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_TipUp).GetRectTransform()
                    .SetAnchoredPositionX(itemPos.x - (screenW / 2 - contentGap - tipW / 2));
            }
            else
            {
                notEnoughTipUI.GetRectTransform().SetAnchoredPositionX(itemPos.x);
                notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_TipDown).GetRectTransform()
                    .SetAnchoredPositionX(0);
                notEnoughTipUI.GetFromReference(UICommon_ResourceNotEnough.KImg_TipUp).GetRectTransform()
                    .SetAnchoredPositionX(0);
            }
        }

        #region Time

        public static long GetTimeStampToNext6Clock(DateTime dateTime)
        {
            return 0;
        }

        #endregion

        #region Animation

        /// <summary>
        /// 得到动画时长 毫秒
        /// </summary>
        /// <param name="skeletonGraphic"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetAnimaionDuration(SkeletonGraphic skeletonGraphic, string name)
        {
            //skeletonGraphic.AnimationState.SetAnimation(0, "die", false);
            skeletonGraphic.skeletonDataAsset.GetSkeletonData(true);
            var animation = skeletonGraphic.SkeletonData.FindAnimation(name);
            if (animation != null)
            {
                return (int)(animation.Duration * 1000f);
            }

            Log.Error($"没找到动画");
            return 0;
        }


        public static void SetSpine(SkeletonGraphic skeletonGraphic, string spineFileName, string skinName = "default")
        {
            skeletonGraphic.skeletonDataAsset =
                ResourcesManager.LoadAsset<SkeletonDataAsset>(spineFileName);
            if (skeletonGraphic.skeletonDataAsset == null)
            {
                skeletonGraphic.skeletonDataAsset =
                    ResourcesManager.LoadAsset<SkeletonDataAsset>("spine_boss_Belt_1501_Json_SkeletonData");
                Log.Error($"spineFileName {spineFileName} {skinName} 不存在此文件");
            }

            if (skeletonGraphic.skeletonDataAsset != null)
            {
                var skeletonData = skeletonGraphic.skeletonDataAsset.GetSkeletonData(false);
                var skin = skeletonData.FindSkin(skinName);
                if (skin != null)
                {
                    skeletonGraphic.initialSkinName = skinName;
                }
                else
                {
                    var skin0 = skeletonData.Skins.Items[0];
                    skeletonGraphic.initialSkinName = skin0.Name;
                }

                // skeletonGraphic.Skeleton.SetSlotsToSetupPose(); // 重置插槽姿势
                // skeletonGraphic.MatchRectTransformWithBounds();

                // skeletonGraphic.startingLoop = true;
                // skeletonGraphic.startingAnimation = "run";
                skeletonGraphic.Initialize(true);
            }
        }

        #endregion

        #region 技能选择函数待做

//  private void UpdatePlayerSkill(int id)
// {
//     var userSkillTable = ConfigManager.Instance.Tables.Tbplayer_skill.DataMap;
//     var skillTable = ConfigManager.Instance.Tables.Tbskill.DataMap;
//     var skillEffectTable = ConfigManager.Instance.Tables.Tbskill_effect.DataMap;
//     int skillEffectID = skillTable[id].skillEffectArray[0];
//
//
//     //Log.Debug(buff1Args.ToString(), Color.green);
//     float coldDown = skillTable[id].cd / 1000f;
//
//
//     if (manager.HasBuffer<Skill>(player))
//     {
//         skills = manager.GetBuffer<Skill>(player);
//     }
//
//     for (int i = 0; i < skills.Length; i++)
//     {
//         if (userSkillTable[skills[i].Int32_0].group == userSkillTable[id].group)
//         {
//             var temp = skills[i];
//             temp.Int32_0 = id;
//             skills[i] = temp;
//             return;
//             //替换
//         }
//     }
//
//     if (id == 240201)
//     {
//         skills.Add(new Skill_240201
//         {
//             caster = default,
//             duration = coldDown,
//             cooldown = coldDown,
//             target = default,
//             tick = 0,
//             timeScale = 1,
//             id = id,
//             level = 1
//         }.ToSkillOld());
//     }
//
//     //添加一个技能或者buff
//     else if (id == 220101)
//     {
//         skills.Add(new SkillLightRing
//         {
//             caster = default,
//             duration = coldDown,
//             cooldown = coldDown,
//             target = default,
//             tick = 0,
//             timeScale = 1,
//             id = id,
//             level = 1
//         }.ToSkillOld());
//     }
//     else if (id == 220201)
//     {
//         skills.Add(new SkillGenerateTrap
//         {
//             caster = default,
//             duration = coldDown,
//             cooldown = coldDown,
//             target = default,
//             tick = 0,
//             timeScale = 1,
//             id = id,
//             level = 1
//         }.ToSkillOld());
//     }
//     else if (id == 210101)
//     {
//         skills.Add(new SkillMotoCrash
//         {
//             caster = default,
//             duration = coldDown,
//             cooldown = coldDown,
//             target = default,
//             tick = 0,
//             timeScale = 1,
//             id = id,
//             level = 1
//         }.ToSkillOld());
//     }
//     else if (id == 210201)
//     {
//         skills.Add(new Skill_210201
//         {
//             caster = default,
//             duration = coldDown,
//             cooldown = coldDown,
//             target = default,
//             tick = 0,
//             timeScale = 1,
//             id = id,
//             level = 1
//         }.ToSkillOld());
//     }
//
//
//     if (manager.HasBuffer<Buff>(player))
//     {
//         buffs = manager.GetBuffer<Buff>(player);
//     }
//
//     for (int i = 0; i < buffs.Length; i++)
//     {
//         if (buffs[i].Int32_0 < 1)
//         {
//             continue;
//         }
//
//         if (userSkillTable[buffs[i].Int32_0].group == userSkillTable[id].group)
//         {
//             buffs.RemoveAt(i);
//             buffs = manager.GetBuffer<Buff>(player);
//         }
//     }
//
//     var group = userSkillTable[id].group;
//     if (group == 2001)
//     {
//         int buff1Args = skillEffectTable[skillEffectID].buff1Para[0];
//         int buff2Args = skillEffectTable[skillEffectID].buff1Para[1];
//         buffs.Add(new Buff_30000020
//         {
//             id = id,
//             carrier = player,
//             duration = -1,
//             permanent = true,
//             buffArgs = new BuffArgs
//             {
//                 args0 = buff1Args,
//                 args1 = buff2Args,
//                 args2 = 0,
//                 args3 = 0,
//                 args4 = 0
//             }
//         }.ToBuffOld());
//     }
//
//
//     if (group == 2301)
//     {
//         int buff1Args = skillEffectTable[skillEffectID].buff1Para[0];
//         buffs.Add(new Buff_202
//         {
//             id = id,
//             carrier = player,
//             duration = -1,
//             permanent = true,
//             buffArgs = new BuffArgs { args0 = buff1Args }
//         }.ToBuffOld());
//     }
//
//     if (group == 2302)
//     {
//         int buff1Args = skillEffectTable[skillEffectID].buff1Para[0];
//         buffs.Add(new Buff_902
//         {
//             id = id,
//             carrier = player,
//             duration = -1,
//             permanent = true,
//             buffArgs = new BuffArgs { args0 = buff1Args }
//         }.ToBuffOld());
//     }
//
//     if (group == 2303)
//     {
//         int buff1Args = skillEffectTable[skillEffectID].buff1Para[0];
//         buffs.Add(new Buff_302
//         {
//             id = id,
//             carrier = player,
//             duration = -1,
//             permanent = true,
//             buffArgs = new BuffArgs { args0 = buff1Args }
//         }.ToBuffOld());
//     }
//
//     if (group == 2401)
//     {
//         int buff1Args = skillEffectTable[skillEffectID].buff1Para[0];
//         buffs.Add(new Buff_1204
//         {
//             id = id,
//             carrier = player,
//             duration = -1,
//             permanent = true,
//             buffArgs = new BuffArgs { args0 = buff1Args / 10000 }
//         }.ToBuffOld());
//     }
//     //throw new NotImplementedException();
// }

        #endregion

        #region Shop

        /// <summary>
        /// 发送购买信息
        /// </summary>
        /// <param name="tagFuncId"></param>
        /// <param name="id"></param>
        public static void SendBuyMessage(int tagFuncId, int id)
        {
            var shopStr = JiYuUIHelper.GetShopStr(tagFuncId, id);

            NetWorkManager.Instance.SendMessage(CMD.PREPAY, new StringValue
            {
                Value = shopStr
            });
            Log.Debug($"SendBuyMessage:{shopStr}", Color.green);
        }

        /// <summary>
        /// 发送购买信息
        /// </summary>
        /// <param name="shopNum">商品编号</param>
        /// <param name="id"></param>
        public static void SendBuyMessage(string shopNum, int id)
        {
            var shopStr = JiYuUIHelper.GetShopStr(shopNum, id);

            NetWorkManager.Instance.SendMessage(CMD.PREPAY, new StringValue
            {
                Value = shopStr
            });
            Log.Debug($"SendBuyMessage:{shopStr}", Color.green);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetShopNum(string shopStr)
        {
            var shopNum = shopStr.Split("|")[1];
            shopNum = shopNum.Substring(0, 3);
            return shopNum;
        }

        private static string GetShopStr(int tagFuncId, int id)
        {
            int resultLength = 9;
            string prefix = default;
            int number = id; // 例如数字部分是 1
            string formattedNumber = default;

            int middle0 = 0;


            switch (tagFuncId)
            {
                case 1401:
                    prefix = "A01";
                    middle0 = resultLength - (prefix.Length + id.ToString().Length);
                    formattedNumber = number.ToString().PadLeft(middle0, '0'); // 填充到6位
                    break;
                case 1202:
                    prefix = "B02";
                    middle0 = resultLength - (prefix.Length + id.ToString().Length);
                    formattedNumber = number.ToString().PadLeft(middle0, '0'); // 填充到6位
                    break;
                case 1301:
                    prefix = "B01";
                    middle0 = resultLength - (prefix.Length + id.ToString().Length);
                    formattedNumber = number.ToString().PadLeft(middle0, '0'); // 填充到6位
                    break;
                case 1302:
                    prefix = "B01";
                    middle0 = resultLength - (prefix.Length + id.ToString().Length);
                    formattedNumber = number.ToString().PadLeft(middle0, '0'); // 填充到6位
                    break;
                case 1404:
                    prefix = "C02";
                    middle0 = resultLength - (prefix.Length + id.ToString().Length);
                    formattedNumber = number.ToString().PadLeft(middle0, '0'); // 填充到6位
                    break;
                case 3405:
                    prefix = "B03";
                    middle0 = resultLength - (prefix.Length + id.ToString().Length);
                    formattedNumber = number.ToString().PadLeft(middle0, '0'); // 填充到6位
                    break;

                case 3201:
                    prefix = "C01";
                    middle0 = resultLength - (prefix.Length + id.ToString().Length);
                    formattedNumber = number.ToString().PadLeft(middle0, '0'); // 填充到6位
                    break;
                case 1403:
                    Log.Error($"基金两档 使用其他方式获取str");
                    break;
            }

            return prefix + formattedNumber;
        }

        private static string GetShopStr(string shopPrefix, int id)
        {
            int resultLength = 9;
            var middle0 = resultLength - (shopPrefix.Length + id.ToString().Length);
            var formattedNumber = id.ToString().PadLeft(middle0, '0'); // 填充到6位
            return shopPrefix + formattedNumber;
        }

        #endregion

        /// <summary>
        /// 返回格式化后的资源值
        /// </summary>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public static string ReturnFormatResourceNum(int resourceType)
        {
            long num = 0;
            //比特币
            if (resourceType == 1)
            {
                num = ResourcesSingleton.Instance.UserInfo.RoleAssets.Bitcoin;
            }
            //金币
            else if (resourceType == 2)
            {
                num = ResourcesSingleton.Instance.UserInfo.RoleAssets.UsBill;
            }
            else if (resourceType == 3)
            {
                num = ResourcesSingleton.Instance.UserInfo.RoleAssets.Energy;
            }

            if (num / 1000 == 0)
            {
                return num.ToString();
            }
            else if (num / 1000 > 0 && num / 1000000 == 0)
            {
                var high = num / 1000;
                var low = num / 100 % 10;
                if (low == 0)
                {
                    return high.ToString() + "K";
                }

                return high.ToString() + "." + low.ToString() + "K";
            }
            else if (num / 1000000 > 0 && num / 1000000000 == 0)
            {
                var high = num / 1000000;
                var low = num / 100000 % 10;
                if (low == 0)
                {
                    return high.ToString() + "M";
                }

                return high.ToString() + "." + low.ToString() + "M";
            }
            else
            {
                var high = num / 1000000000;
                var low = num / 100000000 % 10;
                if (low == 0)
                {
                    return high.ToString() + "M";
                }

                return high.ToString() + "." + low.ToString() + "M";
            }
        }

        /// <summary>
        /// 返回格式化后的资源值
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ReturnFormatResourceNum(long num)
        {
            if (num / 1000 == 0)
            {
                return num.ToString();
            }
            else if (num / 1000 > 0 && num / 1000000 == 0)
            {
                var high = num / 1000;
                var low = num / 100 % 10;
                if (low == 0)
                {
                    return high.ToString() + "K";
                }

                return high.ToString() + "." + low.ToString() + "K";
            }
            else if (num / 1000000 > 0 && num / 1000000000 == 0)
            {
                var high = num / 1000000;
                var low = num / 100000 % 10;
                if (low == 0)
                {
                    return high.ToString() + "M";
                }

                return high.ToString() + "." + low.ToString() + "M";
            }
            else
            {
                var high = num / 1000000000;
                var low = num / 100000000 % 10;
                if (low == 0)
                {
                    return high.ToString() + "M";
                }

                return high.ToString() + "." + low.ToString() + "M";
            }
        }


        #region L10N

        public async static UniTask RefreshAllPanelL10N(int L10N)
        {
            if (L10N == ResourcesSingleton.Instance.settingData.CurrentL10N)
            {
                return;
            }

            JiYuTweenHelper.EnableLoading(true);

            ResourcesSingleton.Instance.settingData.CurrentL10N = L10N;
            ConfigManager.Instance.SwitchLanguages(ResourcesSingleton.Instance.settingData.CurrentL10N);


            JiYuEventManager.Instance.TriggerEvent("OnSwitchL10NResponse", L10N.ToString());
            //UIHelper.Remove(UIType.UIPanel_JiyuGame);
            //var ui = await UIHelper.CreateAsync(UIType.UIPanel_JiyuGame, ResourcesSingleton.Instance.UserInfo);

            // if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui1))
            // {
            //     
            // }


            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var ui0))
            {
                var uis = ui0 as UIPanel_JiyuGame;
                uis.RefreshToggleLanguage();
                await uis.ReCreateAllTagPanel();
            }


            //
            // if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Main, out var ui3))
            // {
            //     var uis = ui3 as UIPanel_Main;
            //     uis.RefreshText();
            // }
            //
            //
            // if (JiYuUIHelper.TryGetUI(UIType.UIPanel_Person, out var ui5))
            // {
            //     var uis = ui5 as UIPanel_Person;
            //     uis.Initialize();
            // }

            JiYuTweenHelper.EnableLoading(false);
        }

        /// <summary>
        /// 获得多语言图片的名字
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetL10NPicName(string key)
        {
            var tbart = ConfigManager.Instance.Tables.Tbart;
            string str = default;
            switch (JsonManager.Instance.sharedData.l10N)
            {
                case (int)Tblanguage.L10N.en:
                    str = tbart.Get(key).en;
                    break;
                case (int)Tblanguage.L10N.zh_cn:
                    str = tbart.Get(key).zhCn;
                    break;
            }

            return str;
        }

        #endregion

        #region Rebirth

        public static void RebirthPlayer()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));
            var player = query.ToEntityArray(Allocator.Temp)[0];
            var chaStats = entityManager.GetComponentData<ChaStats>(player);
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            chaStats.chaResource.hp = Mathf.Max(1, (int)(chaStats.chaProperty.maxHp * 0.5f));
            chaStats.chaResource.isDead = false;
            playerData.playerOtherData.rebirthCount++;
            entityManager.SetComponentData(player, chaStats);
            entityManager.SetComponentData(player, playerData);
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
            {
                var uiRuntime = ui as UIPanel_RunTimeHUD;
                uiRuntime.livefailTime = 0;
                uiRuntime.reBirthCount++;
            }

            JiYuUIHelper.StartStopTime(true);
        }

        #endregion

        public static void PrintMemery()
        {
            var bytes1 = Profiler.GetTotalAllocatedMemoryLong();
            var bytes2 = Profiler.GetTotalReservedMemoryLong();

            // 将字节转换为 GB
            double gigabytes1 = bytes1 / (1024.0 * 1024.0 * 1024.0);
            double gigabytes2 = bytes2 / (1024.0 * 1024.0 * 1024.0);
            //await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionShattersEnter);
            Log.Debug($"GetTotalAllocatedMemoryLong {gigabytes1} GetTotalReservedMemoryLong{gigabytes2}");
        }


        #region Pool

        private static void ClearTagPools(int sort)
        {
            switch (sort)
            {
                case 1:
                    //GameObjectPool.Instance.RemovePool(UIPathSet.UIPanel_MonsterCollection);

                    break;
                case 2:
                    //GameObjectPool.Instance.RemovePool(UIPathSet.UIPanel_MonsterCollection);

                    break;
                case 3:

                    //GameObjectPool.Instance.RemovePool(UIPathSet.UIPanel_MonsterCollection);
                    break;
                case 4:

                    //GameObjectPool.Instance.RemovePool(UIPathSet.UIPanel_MonsterCollection);
                    break;
                case 5:
                    GameObjectPool.Instance.RemovePool(UIPathSet.UISubPanel_Collection_Item);

                    break;
            }
        }

        /// <summary>
        /// 对象池优化对应Tag下所有需要创建对象池优化的面板
        /// </summary>
        /// <param name="sort"></param>
        public async static void CreateTagPools(int sort)
        {
            for (int i = 1; i < 6; i++)
            {
                int index = i;
                if (sort != index)
                {
                    ClearTagPools(index);
                }
            }

            if (!JiYuUIHelper.TryGetUI(UIType.UIPanel_JiyuGame, out var jiyu))
            {
                return;
            }

            var KPoolContent = jiyu.GetFromReference(UIPanel_JiyuGame.KPoolContent);
            var list = KPoolContent.GetList();
            switch (sort)
            {
                case 1:
                    //GameObjectPool.Instance.RemovePool(UIPathSet.UIPanel_MonsterCollection);

                    break;
                case 2:
                    //GameObjectPool.Instance.RemovePool(UIPathSet.UIPanel_MonsterCollection);

                    break;
                case 3:

                    //GameObjectPool.Instance.RemovePool(UIPathSet.UIPanel_MonsterCollection);
                    break;
                case 4:

                    //GameObjectPool.Instance.RemovePool(UIPathSet.UIPanel_MonsterCollection);
                    break;
                case 5:


                    for (int i = 0; i < 20; i++)
                    {
                        var ui = await list.CreateWithUITypeAsync(UIType.UISubPanel_Collection_Item, false);
                    }


                    break;
            }

            list.Clear();
        }

        #endregion
    }
}