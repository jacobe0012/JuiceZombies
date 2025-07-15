//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2025-04-17 11:25:25
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using XFramework;
using static XFramework.UIManager;

namespace HotFix_UI
{
    /// <summary>
    /// 切换游戏内外/登录等的方法
    /// </summary>
    public static class JiYuSceneHelper
    {
        public static void RestartApplication()
        {
#if UNITY_EDITOR
            Debug.LogWarning("Cannot restart application in Unity Editor. This method is for Android builds only.");
            return;

#elif UNITY_IOS
            //TODO:
#elif UNITY_ANDROID
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
                AndroidJavaObject intent =
                    pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);

                // 设置 Intent 标志，清除所有之前的 Activity，并创建一个新的任务栈
                intent.Call<AndroidJavaObject>("setFlags", 0x20000000); // Intent.FLAG_ACTIVITY_SINGLE_TOP
                // 或者使用更全面的标志来确保完全重置栈
                // intent.Call<AndroidJavaObject>("setFlags", 0x10000000 | 0x00008000); // Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK

                AndroidJavaClass pendingIntent = new AndroidJavaClass("android.app.PendingIntent");
                AndroidJavaObject contentIntent =
                    pendingIntent.CallStatic<AndroidJavaObject>("getActivity", currentActivity, 0, intent,
                        0x8000000); // PendingIntent.FLAG_UPDATE_CURRENT

                AndroidJavaObject alarmManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "alarm");
                AndroidJavaClass system = new AndroidJavaClass("java.lang.System");
                long currentTime = system.CallStatic<long>("currentTimeMillis");

                // 设置一个短暂的延迟，然后触发 PendingIntent
                alarmManager.Call("set", 1, currentTime + 100, contentIntent); // 100ms 延迟

                // 终止当前进程
                currentActivity.Call("finish"); // 结束当前的 Activity
                AndroidJavaClass process = new AndroidJavaClass("android.os.Process");
                int pid = process.CallStatic<int>("myPid");
                process.CallStatic("killProcess", pid); // 杀死当前进程
            }
#endif
        }


        #region RunTime

        public static void LoadRunTime()
        {
            Log.Debug($"进入RunTime场景");
            InitShader();
            ResourcesSingleton.Instance.FromRunTimeScene = true;
            InitRunTimeScene().Forget();
        }

        public static void InitShader()
        {
            var isSumulator = false;
#if !UNITY_EDITOR
            if (!isSumulator)
            {
                Shader.SetGlobalVector(Shader.PropertyToID(UnityHelper.sortingGlobalData),
                new Vector4(-UnityHelper.PerLayerOffset, -UnityHelper.PerSortingIndexOffset, default, default));
            }else{
                Shader.SetGlobalVector(Shader.PropertyToID(UnityHelper.sortingGlobalData),
                new Vector4(UnityHelper.PerLayerOffset, UnityHelper.PerSortingIndexOffset, default, default));
            }

#else

            Shader.SetGlobalVector(Shader.PropertyToID(UnityHelper.sortingGlobalData),
                new Vector4(UnityHelper.PerLayerOffset, UnityHelper.PerSortingIndexOffset, default, default));
#endif
        }

        private static async UniTaskVoid InitRunTimeScene()
        {
            //var global = Common.Instance.Get<Global>();
            // global.GameObjects.RollBackground?.SetViewActive(false);

            // 创建黑板实体
            CreateBlackBoardEntity();
            Debug.Log($"RunTimeScene");


            await InitInputPrefab();

            var hud = await UIHelper.CreateAsync(UIType.UIPanel_RunTimeHUD);

            hud.GetRectTransform().SetAnchoredPosition(Vector2.zero);

            //启用一次预制件映射系统
            var PrefabMapSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystemManaged<PrefabMapSystem>();
            PrefabMapSystem.Enabled = true;

            var initPlayerSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystemManaged<InitPlayerSystem>();
            var initSysGroup =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<InitializationSystemGroup>();
            initSysGroup.AddSystemToUpdateList(initPlayerSystem);
            initPlayerSystem.Enabled = true;
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

            InitMap();

            // AudioManager.Instance.
            //播放runtime BGM 
            //AudioManager.Instance.PlayBgmAudio("RunTimeBGM", false);

            AudioManager.Instance.ClearFModBgmAudio();
            //AudioManager.Instance.StopFModAudio(1101);
            AudioManager.Instance.PlayFModAudio(2103);
        }

        private static void InitMap()
        {
            // var initSysGroup =
            //     World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<InitializationSystemGroup>();


            //初始化地图
            // var initMapSystem =
            //     World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitMapSystem>();
            // ref var state = ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(initMapSystem);
            // state.Enabled = true;
            //SceneSystem


            //TODO:删掉
            // var SpawnEnemySystem =
            //     World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameEventHandleSystem>();
            // ref var state0 =
            //     ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(SpawnEnemySystem);
            // state0.Enabled = true;
            // var HitBackAfterSystem =
            //     World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<HitBackAfterSystem>();
            // ref var state1 =
            //     ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(HitBackAfterSystem);
            // state1.Enabled = false;

            //  var ConsumeDamageInfoSystem =
            //      World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConsumeDamageInfoSystem>();
            //  ref var state2 =
            //      ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(ConsumeDamageInfoSystem);
            // state2.Enabled = false;
            // initSysGroup.AddSystemToUpdateList(initMapSystem);
            // var state = World.DefaultGameObjectInjectionWorld.EntityManager.WorldUnmanaged
            //     .GetExistingSystemState<InitMapSystem>();
        }

        /// <summary>
        /// 输入及FPS显示
        /// </summary>
        private static async UniTask InitInputPrefab()
        {
            var task0 = await ResourcesManager.InstantiateAsync("Rewired Input Manager");

            //await task0;

            GameObject.DontDestroyOnLoad(task0);

            //Debug.Log($"[{GetType().FullName}] {task0.name}InitInputPrefab!");
        }


        /// <summary>
        /// 创建全局实体,给其添加各种单例组件
        /// </summary>
        /// <returns></returns>
        private static Entity CreateBlackBoardEntity()
        {
            var global = XFramework.Common.Instance.Get<Global>();
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var levelConfig = ConfigManager.Instance.Tables.Tblevel;
            var monsterTemplateConfig = ConfigManager.Instance.Tables.Tbmonster_template;
            int levelId = ResourcesSingleton.Instance.levelInfo.levelId;
            int sceneId = ConfigManager.Instance.Tables.Tblevel.Get(levelId).sceneId;
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
                mapData = new MapData { mapID = mapID, mapType = mapType, mapSize = mapSize },
                enableTest1 = false,
                enableTest2 = false,
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

            return e;
        }

        #endregion
    }
}