using System.Collections.Generic;
using System.Linq;
using cfg.blobstruct;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using HotFix_UI;
using Main;
using ProjectDawn.Navigation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace XFramework
{
    public class IntroGuide : Scene
    {
        protected override void OnCompleted()
        {
            Log.Debug($"进入RunTime场景");
            UnicornSceneHelper.InitShader();
            InitRunTimeScene().Forget();
            ResourcesSingletonOld.Instance.FromRunTimeScene = true;
        }


        async UniTaskVoid InitRunTimeScene()
        {
            Debug.Log($"InitRunTimeScene");
            await InitInputPrefab();
            // 创建黑板实体
            int sceneId = int.Parse(ConfigManager.Instance.Tables.Tbguide.DataList.Where(a => a.guideType == 900)
                .FirstOrDefault().guidePara[0]);
            UnicornUIHelper.CreateBlackBoardEntity(sceneId);
            UnicornUIHelper.InitSystem();

            AudioManager.Instance.ClearFModBgmAudio();
            AudioManager.Instance.PlayFModAudio(2103);
            int group = ResourcesSingletonOld.Instance.settingData.GuideList.OrderBy(a => a).ToList()[0];
            Debug.Log($"IntroGuideScene{group}");
            var hud = await UIHelper.CreateAsync(UIType.UIPanel_RunTimeHUD, group);
            hud.GetRectTransform().SetAnchoredPosition(Vector2.zero);
        }

        // async UniTaskVoid InitRunTimeScene()
        // {
        //     //var global = Common.Instance.Get<Global>();
        //     // global.GameObjects.RollBackground?.SetViewActive(false);
        //
        //     await InitInputPrefab();
        //     //创建黑板实体
        //     CreateBlackBoardEntity();
        //     UnicornUIHelper.InitSystem();
        //     //InitMap();
        //     // AudioManager.Instance.
        //     //播放runtime BGM 
        //     //AudioManager.Instance.PlayBgmAudio("RunTimeBGM", false);
        //     AudioManager.Instance.ClearFModBgmAudio();
        //     //AudioManager.Instance.StopFModAudio(1101);
        //     AudioManager.Instance.PlayFModAudio(2103);
        //
        //     int group = ResourcesSingletonOld.Instance.settingData.GuideList.OrderBy(a => a).ToList()[0];
        //     Debug.Log($"IntroGuideScene{group}");
        //     var hud = await UIHelper.CreateAsync(UIType.UIPanel_RunTimeHUD, group);
        //     hud.GetRectTransform().SetAnchoredPosition(Vector2.zero);
        // }


        private void InitMap()
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
            //state0.Enabled = true;
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
        private async UniTask InitInputPrefab()
        {
            var task0 = await ResourcesManager.InstantiateAsync("Rewired Input Manager");

            //await task0;

            GameObject.DontDestroyOnLoad(task0);

            Debug.Log($"[{GetType().FullName}] {task0.name}InitInputPrefab!");
        }


        /// <summary>
        /// 创建全局实体,给其添加各种单例组件
        /// </summary>
        /// <returns></returns>
        private Entity CreateBlackBoardEntity()
        {
            //int levelId = 10000;
            var global = XFramework.Common.Instance.Get<Global>();
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var levelConfig = ConfigManager.Instance.Tables.Tblevel;
            var monsterTemplateConfig = ConfigManager.Instance.Tables.Tbmonster_template;
            //int levelId = ResourcesSingletonOld.Instance.levelInfo.levelId;
            int sceneId = int.Parse(ConfigManager.Instance.Tables.Tbguide.DataList.Where(a => a.guideType == 900)
                .FirstOrDefault().guidePara[0]);

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
                levelId = ResourcesSingletonOld.Instance.levelInfo.levelId,
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

            return e;
        }


        protected override void OnDestroy()
        {
            Log.Debug("离开RunTimeScene场景");
            //DestoryWorldBlackBoardEntity();


            base.OnDestroy();
        }

        // public void DestoryWorldBlackBoardEntity()
        // {
        //     var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //     var entityQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag));
        //     var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
        //     var crowdQuery = entityManager.CreateEntityQuery(typeof(CrowdSurface));
        //
        //
        //     if (entityQuery.IsEmpty) return;
        //     var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
        //     var playerData = entityManager.GetComponentData<PlayerData>(player);
        //     playerData.playerOtherData.propsHashMap.Dispose();
        //
        //     var e = entityQuery.ToEntityArray(Allocator.Temp)[0];
        //     var gameOthersData = entityManager.GetComponentData<GameOthersData>(e);
        //     //gameOthersData.animations.Dispose();
        //     gameOthersData.allAudioClips.Dispose();
        //
        //     Debug.LogError($"crowdQuery1 {crowdQuery.IsEmpty}");
        //     if (!crowdQuery.IsEmpty)
        //     {
        //         Debug.LogError($"crowdQuery {crowdQuery.CalculateEntityCount()}");
        //         entityManager.DestroyEntity(crowdQuery);
        //     }
        //
        //     entityManager.DestroyEntity(entityQuery);
        // }
    }
}