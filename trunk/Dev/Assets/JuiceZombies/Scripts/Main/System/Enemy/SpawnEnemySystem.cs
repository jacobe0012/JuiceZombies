//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-24 12:25:25
//---------------------------------------------------------------------

using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Main
{
    //刷新怪物系统
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SpawnMapAndAreaSystem))]
    public partial struct SpawnEnemySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //state.Enabled = false;
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PlayerData>();
            //state.RequireForUpdate<ObstacleTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var damageQuery = SystemAPI.QueryBuilder().WithAll<DamageInfo>().Build();
            var enemyQuery = SystemAPI.QueryBuilder().WithAll<EnemyData>().Build();
            //var mapDataQuery = SystemAPI.QueryBuilder().WithAll<MapBaseData>().Build();

            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
            //var surfaceEntity = SystemAPI.GetSingletonEntity<CrowdSurface>();

            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            var gameCameraSizeData = SystemAPI.GetComponent<GameCameraSizeData>(wbe);
            var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);
            var enviromentData = SystemAPI.GetComponent<GameEnviromentData>(wbe);

            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var obstacleQuery = SystemAPI.QueryBuilder().WithAll<ObstacleTag, MapElementData>().Build();
            //var crowdSurfaceQuery = SystemAPI.QueryBuilder().WithAll<CrowdSurface>().Build();

            new SpawnEnemyJob
            {
                timeTick = (uint)(SystemAPI.Time.ElapsedTime * 1000f),
                fdT = SystemAPI.Time.fixedDeltaTime,
                prefabMapData = prefabMapData,
                gameSetUpData = gameSetUpData,
                globalConfigData = globalConfigData,
                gameCameraSizeData = gameCameraSizeData,
                gameOthersData = gameOthersData,
                gameRandomData = gameRandomData,
                enviromentData = enviromentData,
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(true),
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(true),
                cdfeAgentLocomotion = SystemAPI.GetComponentLookup<AgentLocomotion>(true),
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(true),
                cdfePhysicsMass = SystemAPI.GetComponentLookup<PhysicsMass>(true),
                cdfePhysicsCollider = SystemAPI.GetComponentLookup<PhysicsCollider>(true),
                cdfeMapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
                cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
                cdfeWeaponData = SystemAPI.GetComponentLookup<WeaponData>(true),
                cdfeTargetData = SystemAPI.GetComponentLookup<TargetData>(true),
                bfeGameEvent = SystemAPI.GetBufferLookup<GameEvent>(),
                player = SystemAPI.GetSingletonEntity<PlayerData>(),
                cdfePostTransformMatrix = SystemAPI.GetComponentLookup<PostTransformMatrix>(true),
                wbe = wbe,
                //surface = SystemAPI.GetSingletonEntity<PlayerData>(),
                ecb = ecb,
                enemyDatas = enemyQuery.ToComponentDataArray<EnemyData>(Allocator.TempJob),
                enemies = enemyQuery.ToEntityArray(Allocator.TempJob),
                obstacles = obstacleQuery.ToEntityArray(Allocator.TempJob),
                //crowdSurfaces = crowdSurfaceQuery.ToEntityArray(Allocator.TempJob),
            }.ScheduleParallel();
        }

        [BurstCompile]
        public partial struct SpawnEnemyJob : IJobEntity
        {
            [ReadOnly] public uint timeTick;
            [ReadOnly] public float fdT;
            [ReadOnly] public PrefabMapData prefabMapData;
            [ReadOnly] public GameSetUpData gameSetUpData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public GameCameraSizeData gameCameraSizeData;
            [ReadOnly] public GameOthersData gameOthersData;
            public GameEnviromentData enviromentData;

            public GameRandomData gameRandomData;
            [ReadOnly] public ComponentLookup<LocalTransform> cdfeLocalTransform;
            [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;
            [ReadOnly] public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;
            [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
            [ReadOnly] public ComponentLookup<PhysicsMass> cdfePhysicsMass;
            [ReadOnly] public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
            [ReadOnly] public ComponentLookup<MapElementData> cdfeMapElementData;
            [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
            [ReadOnly] public ComponentLookup<WeaponData> cdfeWeaponData;
            [ReadOnly] public ComponentLookup<TargetData> cdfeTargetData;
            [NativeDisableParallelForRestriction] public BufferLookup<GameEvent> bfeGameEvent;

            [ReadOnly] public Entity player;
            [ReadOnly] public Entity wbe;

            //public Entity surface;
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<EnemyData> enemyDatas;

            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> obstacles;
            [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> enemies;
            //[ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> crowdSurfaces;
            [ReadOnly] public ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix;

            private void Execute([EntityIndexInQuery] int entityIndexInQuery, Entity e, ref GameTimeData jiyuTimeData)
            {
                var timeData = jiyuTimeData.refreshTime;
                var logicTimeData = jiyuTimeData.logicTime;
                var unScaledTimeData = jiyuTimeData.unScaledTime;

                unScaledTimeData.elapsedTime += fdT * unScaledTimeData.gameTimeScale;
                unScaledTimeData.tick++;
                jiyuTimeData.unScaledTime = unScaledTimeData;

                logicTimeData.elapsedTime += fdT * logicTimeData.gameTimeScale;
                logicTimeData.tick++;
                jiyuTimeData.logicTime = logicTimeData;
                /*
                for (int i = 0; i < enemies.Length; i++)
                {
                    var entity = enemies[i];
                    if (cdfeTargetData[entity].BelongsTo == (uint)BuffHelper.TargetEnum.BossMonster)
                    {
                        var hp = cdfeChaStats[entity].chaResource.hp;
                        if (hp <= 0)
                        {
                            var tran = cdfeLocalTransform[player];
                            var playerData = cdfePlayerData[player];
                            playerData.playerOtherData.isBossFight = false;
                            tran.Position = playerData.playerOtherData.bossBeforePos;
                            ecb.SetComponent(entityIndexInQuery, player, tran);
                            ecb.SetComponent(entityIndexInQuery, player, playerData);
                            //ecb.DestroyEntity(entityIndexInQuery, crowdSurfaces);
                            ecb.AddComponent(entityIndexInQuery, crowdSurfaces, new TimeToDieData
                            {
                                duration = 5
                            });
                            var newentity = ecb.CreateEntity(entityIndexInQuery);
                            ecb.AddComponent(entityIndexInQuery, newentity, new HybridEventData
                            {
                                type = 6,
                                args = new float4(playerData.playerOtherData.bossBeforePos,
                                    gameOthersData.mapData.mapType)
                            });
                            timeData.defaultGameTimeScale = 1;
                            timeData.gameTimeScale = timeData.defaultGameTimeScale;

                            jiyuTimeData.refreshTime = timeData;
                        }

                        break;
                    }
                }
                */

                if (timeData.gameTimeScale < math.EPSILON)
                    return;
                if (cdfeChaStats[player].chaResource.hp <= 0)
                    return;
                //添加环境事件
                if (enviromentData.env.weather != enviromentData.env.lastWeather)
                {
                    ref var tbenvironment = ref globalConfigData.value.Value.configTbenvironments.configTbenvironments;
                    enviromentData.env.lastWeather = enviromentData.env.weather;

                    for (int j = 0; j < bfeGameEvent[wbe].Length; j++)
                    {
                        var temp = bfeGameEvent[wbe][j];
                        bool isEnv = false;
                        for (int i = 0; i < tbenvironment.Length; i++)
                        {
                            if (tbenvironment[i].id == temp.Int32_0 &&
                                tbenvironment[i].type == 1)
                            {
                                isEnv = true;
                                bfeGameEvent[wbe].RemoveAt(j);
                                break;
                            }
                        }

                        if (isEnv)
                        {
                            break;
                        }
                    }

                    ecb.AppendToBuffer(entityIndexInQuery, wbe, new GameEvent
                    {
                        CurrentTypeId = (GameEvent.TypeId)enviromentData.env.weather,
                        Int32_0 = enviromentData.env.weather,
                        Boolean_12 = true,
                        Boolean_6 = true,
                    });
                    ecb.SetComponent(entityIndexInQuery, wbe, enviromentData);
                }

                if (enviromentData.env.time != enviromentData.env.lastTime)
                {
                    ref var tbenvironment = ref globalConfigData.value.Value.configTbenvironments.configTbenvironments;
                    enviromentData.env.lastTime = enviromentData.env.time;

                    for (int j = 0; j < bfeGameEvent[wbe].Length; j++)
                    {
                        var temp = bfeGameEvent[wbe][j];
                        bool isEnv = false;
                        for (int i = 0; i < tbenvironment.Length; i++)
                        {
                            if (tbenvironment[i].id == temp.Int32_0 &&
                                tbenvironment[i].type == 2)
                            {
                                isEnv = true;
                                bfeGameEvent[wbe].RemoveAt(j);
                                break;
                            }
                        }

                        if (isEnv)
                        {
                            break;
                        }
                    }

                    ecb.AppendToBuffer(entityIndexInQuery, wbe, new GameEvent
                    {
                        //CurrentTypeId = (GameEvent.TypeId)enviromentData.env.time,
                        Int32_0 = enviromentData.env.time,
                        Boolean_12 = true
                    });
                    ecb.SetComponent(entityIndexInQuery, wbe, enviromentData);
                }


                //进入攻击范围

                ref var monster_refreshs =
                    ref globalConfigData.value.Value.configTbmonster_templates.configTbmonster_templates;
                ref var monstersConfig = ref globalConfigData.value.Value.configTbmonsters.configTbmonsters;
                ref var levelConfig = ref globalConfigData.value.Value.configTblevels.configTblevels;
                ref var monster_attrs = ref globalConfigData.value.Value.configTbmonster_attrs.configTbmonster_attrs;
                ref var monster_weapons =
                    ref globalConfigData.value.Value.configTbweapons.configTbweapons;
                ref var monster_formations =
                    ref globalConfigData.value.Value.configTbmonster_forations.configTbmonster_forations;
                ref var scene_bosss =
                    ref globalConfigData.value.Value.configTbscene_bosss.configTbscene_bosss;

                var mapWidthHeight =
                    GetMapWidthHeight(globalConfigData, gameOthersData, out var mapId, out var mapType);
                mapWidthHeight -= 10f;
                for (int i = 0; i < monster_refreshs.Length; i++)
                {
                    if (gameOthersData.monsterRefreshId != monster_refreshs[i].id) continue;

                    if (monster_refreshs[i].monsterId < 0) continue;
                    int monsterIndex = 0;
                    for (int l = 0; l < monstersConfig.Length; l++)
                    {
                        if (monstersConfig[l].id == monster_refreshs[i].monsterId)
                        {
                            monsterIndex = l;
                            break;
                        }
                    }

                    ref var monster = ref monstersConfig[monsterIndex];

                    int monsterAttrIndex = 0;
                    for (int l = 0; l < monster_attrs.Length; l++)
                    {
                        if (monster_attrs[l].id == monster.monsterAttrId)
                        {
                            monsterAttrIndex = l;
                            break;
                        }
                    }

                    ref var monsterAttr = ref monster_attrs[monsterAttrIndex];
                    var type = UnityHelper.GetTargetId(monsterAttr.type);
                    float addRatios = 0;
                    for (int j = 0; j < bfeGameEvent[wbe].Length; j++)
                    {
                        var temp = bfeGameEvent[wbe][j];
                        if (temp.CurrentTypeId == GameEvent.TypeId.GameEvent_22)
                        {
                            if (temp.int3_7.x == type)
                            {
                                addRatios += temp.int3_7.y / 10000f;
                            }
                        }
                    }

                    switch (monster_refreshs[i].type)
                    {
                        case 1:
                            var startT = monster_refreshs[i].time[0] / timeData.gameTimeScale;
                            var endT = monster_refreshs[i].time[1] / timeData.gameTimeScale;

                            if (timeData.tick == (int)(startT / fdT))
                            {
                                ref var event0 = ref monster_refreshs[i].event0;
                                for (int j = 0; j < event0.Length; j++)
                                {
                                    HandleEvent(event0[j], entityIndexInQuery);
                                }
                            }

                            break;
                        case 2:
                            for (int j = 0; j < monster_refreshs[i].time.Length; j++)
                            {
                                if (timeData.tick ==
                                    (int)((monster_refreshs[i].time[j] / timeData.gameTimeScale) / fdT))
                                {
                                    ref var event1 = ref monster_refreshs[i].event0;
                                    for (int k = 0; k < event1.Length; k++)
                                    {
                                        HandleEvent(event1[k], entityIndexInQuery);
                                    }
                                }
                            }


                            break;
                    }

                    if (monster_refreshs[i].monsterId == 0) continue;
                    if (enemies.Length > UnityHelper.MaxEnemyCount &&
                        type != (int)BuffHelper.TargetEnum.BossMonster)
                        continue;

                    int curNum = 0;
                    foreach (var enemy in enemyDatas)
                    {
                        if (enemy.enemyRefreshGroupId == monster_refreshs[i].ruleId)
                        {
                            curNum++;
                        }
                    }

                    int formationId = monster_refreshs[i].monsterFormationId;
                    int count = GetSpawnFormationCount(formationId);

                    int maxNum = math.max(1, (int)(count * (1 + addRatios) * monster_refreshs[i].limitMax));

                    maxNum = monster_refreshs[i].limitMax == 0
                        ? MathHelper.MaxNum
                        : maxNum;

                    switch (monster_refreshs[i].type)
                    {
                        case 1:
                            var startT = monster_refreshs[i].time[0] / timeData.gameTimeScale;
                            var endT = monster_refreshs[i].time[1] / timeData.gameTimeScale;

                            // if (timeData.tick == (int)(startT / fdT))
                            // {
                            //     ref var event0 = ref monster_refreshs[i].event0;
                            //     for (int j = 0; j < event0.Length; j++)
                            //     {
                            //         HandleEvent(event0[j], entityIndexInQuery);
                            //     }
                            // }

                            if (timeData.tick >= (int)(startT / fdT) && timeData.tick <= (int)(endT / fdT) &&
                                (timeData.tick - (int)(startT / fdT)) %
                                (int)(((monster_refreshs[i].interval / 1000f) / timeData.gameTimeScale) / fdT) == 0
                                && curNum < maxNum
                               )
                            {
                                var curTimes = (timeData.tick - (int)(startT / fdT)) /
                                               (int)(((monster_refreshs[i].interval / 1000f) / timeData.gameTimeScale) /
                                                     fdT);
                                int times = (int)math.ceil((monster_refreshs[i].time[1] - monster_refreshs[i].time[0]) /
                                                           (monster_refreshs[i].interval / 1000f));
                                //Debug.LogError($"curTimes{curTimes}  {times}");
                                //____________________________________________
                                SpawnEnemy(ref timeData, i, entityIndexInQuery, mapId, mapWidthHeight, formationId,
                                    addRatios, times, curTimes);
                            }
                            else
                            {
                                //Debug.Log($"curNum{curNum} maxNum{maxNum}");
                                continue;
                            }


                            break;
                        case 2:
                            for (int j = 0; j < monster_refreshs[i].time.Length; j++)
                            {
                                if (timeData.tick ==
                                    (int)((monster_refreshs[i].time[j] / timeData.gameTimeScale) / fdT))
                                {
                                    //Debug.Log(
                                    //    $"tick{monster_refreshs[i].time[j]} : {monster_refreshs[i].monsterId}");
                                    //____________________________________________
                                    //
                                    //int totalNum=  GetSpawnFormationCount(monster_refreshs[i].monsterFormationId)*
                                    SpawnEnemy(ref timeData, i, entityIndexInQuery, mapId, mapWidthHeight, formationId,
                                        addRatios);
                                    // ref var event1 = ref monster_refreshs[i].event0;
                                    // for (int k = 0; k < event1.Length; k++)
                                    // {
                                    //     HandleEvent(event1[k], entityIndexInQuery);
                                    // }
                                }
                            }


                            break;
                    }
                }


                // gameRandomData.rand = rand;
                //Debug.Log($"{gameRandomData.rand.state}");

                ecb.SetComponent(entityIndexInQuery, wbe, gameRandomData);
                timeData.elapsedTime += fdT * timeData.gameTimeScale;
                timeData.tick++;

                jiyuTimeData.refreshTime = timeData;
            }

            void HandleEvent(int eventId, int entityIndexInQuery)
            {
                Debug.Log($"HandleEvent  eventId{eventId}");
                ecb.AppendToBuffer(entityIndexInQuery, wbe, new GameEvent
                {
                    Int32_0 = eventId,
                    Boolean_6 = true
                });
            }

            void SpawnEnemy(ref JiYuTime timeData, int i, int sortKey, int mapId, float2 mapWidthHeight,
                int formationId, float addRatios, int spawnTimes = 1, int curTimes = 0)
            {
                ref var monster_refreshs =
                    ref globalConfigData.value.Value.configTbmonster_templates.configTbmonster_templates;
                ref var monstersConfig = ref globalConfigData.value.Value.configTbmonsters.configTbmonsters;
                ref var levelConfig = ref globalConfigData.value.Value.configTblevels.configTblevels;
                ref var monster_attrs = ref globalConfigData.value.Value.configTbmonster_attrs.configTbmonster_attrs;
                ref var monster_weapons =
                    ref globalConfigData.value.Value.configTbweapons.configTbweapons;
                ref var monster_formations =
                    ref globalConfigData.value.Value.configTbmonster_forations.configTbmonster_forations;
                ref var scene_bosss =
                    ref globalConfigData.value.Value.configTbscene_bosss.configTbscene_bosss;
                ref var monster_models =
                    ref globalConfigData.value.Value.configTbmonster_models.configTbmonster_models;


                int monsterIndex = 0;
                for (int l = 0; l < monstersConfig.Length; l++)
                {
                    if (monstersConfig[l].id == monster_refreshs[i].monsterId)
                    {
                        monsterIndex = l;
                        break;
                    }
                }

                ref var monster = ref monstersConfig[monsterIndex];

                int groupNum = math.max(1, (int)(1 + addRatios) * monster_refreshs[i].num);
                int totalSpawnNum = (int)(groupNum * GetSpawnFormationCount(formationId) * spawnTimes);

                int insIndex = (int)groupNum * curTimes;
                //Debug.LogError($"insIndexB{insIndex}");
                //groupNum * curTimes
                for (int k = 1; k < groupNum + 1; k++)
                {
                    int levelIndex = 0;
                    for (int l = 0; l < levelConfig.Length; l++)
                    {
                        if (levelConfig[l].id == gameOthersData.levelId)
                        {
                            levelIndex = l;
                            break;
                        }
                    }

                    ref var level = ref levelConfig[levelIndex];

                    int monsterAttrIndex = 0;
                    for (int m = 0; m < monster_attrs.Length; m++)
                    {
                        if (monster_attrs[m].id == monster.monsterAttrId)
                        {
                            monsterAttrIndex = m;
                            break;
                        }
                    }

                    ref var monsterAttr = ref monster_attrs[monsterAttrIndex];

                    int scene_bosssIndex = 0;
                    for (int m = 0; m < scene_bosss.Length; m++)
                    {
                        if (monster.sceneBossId == scene_bosss[m].id)
                        {
                            scene_bosssIndex = m;
                            break;
                        }
                    }

                    ref var scene_boss = ref scene_bosss[scene_bosssIndex];

                    int monster_modelsIndex = 0;
                    for (int m = 0; m < monster_models.Length; m++)
                    {
                        if (monsterAttr.bookId == monster_models[m].id)
                        {
                            monster_modelsIndex = m;
                            break;
                        }
                    }

                    ref var monsterModel = ref monster_models[monster_modelsIndex];

                    if (!prefabMapData.prefabHashMap.TryGetValue(monsterModel.model, out var prefab))
                    {
                        Debug.Log($"没有{monsterModel.model}预制件");
                        continue;
                    }

                    //Debug.Log($"找到{monsterAttr.model}预制件");
                    bool isBossScene = monster.sceneBossId > 0;
                    float3 playerPos = cdfeLocalTransform[player].Position;
                    Entity groupEntity = Entity.Null;
                    Entity surfaceEntity = Entity.Null;

                    if (isBossScene)
                    {
                        Debug.Log($"monster.sceneBossId {monster.sceneBossId}");
                        // float3 pos = default;
                        // switch (mapType)
                        // {
                        //     case 1:
                        //         pos = new float3(1999, 0, 0);
                        //         break;
                        //     case 2:
                        //         pos = new float3(0, 1999, 0);
                        //         break;
                        //     case 3:
                        //         pos = new float3(1999, 0, 0);
                        //
                        //         break;
                        //     case 4:
                        //         //TODO:全开放的boss场景选点
                        //         pos = new float3(1999, 1999, 0);
                        //         break;
                        // }

                        var playerTran = cdfeLocalTransform[player];
                        var playerData = cdfePlayerData[player];
                        playerData.playerOtherData.bossBeforePos = playerTran.Position;
                        playerData.playerOtherData.isBossFight = true;

                        playerPos = BuffHelper.GenerateBossMap(obstacles, cdfeMapElementData, cdfeLocalTransform,
                            gameRandomData.seed, cdfePostTransformMatrix, ref ecb,
                            sortKey,
                            globalConfigData,
                            prefabMapData, monster.id, mapId, out groupEntity, out surfaceEntity,
                            out float3 bossScenePos);
                        playerData.playerOtherData.bossScenePos = bossScenePos;
                        // playerTran.Position = playerPos;
                        // ecb.SetComponent(sortKey, player, playerTran);
                        ecb.SetComponent(sortKey, player, playerData);
                        timeData.defaultGameTimeScale = 0;
                        timeData.gameTimeScale = timeData.defaultGameTimeScale;
                        ecb.DestroyEntity(sortKey, enemies);
                    }

                    var newScale = (monsterAttr.modelSize / 10000f) * cdfeLocalTransform[prefab].Scale;
                    var insList = SpawnFormationEnemy(ecb, prefab, formationId, sortKey,
                        cdfeLocalTransform, mapWidthHeight, (uint)k, isBossScene, playerPos,
                        groupEntity, surfaceEntity, monster_refreshs[i].distance, newScale, monster.id);


                    foreach (var ins in insList)
                    {
                        //var prefab = prefabMapData.prefabHashMap[monsterAttr.model];
                        //var ins = ecb.Instantiate(entityIndexInQuery, prefab);

                        //Debug.LogError($"insIndexB{insIndex} curTimes{curTimes}");


                        //Debug.LogError($"insIndex{insIndex}  totalSpawnNum{totalSpawnNum} curTimes{curTimes}");
                        for (int l = 0; l < monster_refreshs[i].reward.Length; l++)
                        {
                            //monster_refreshs[i].reward[l].y
                            var dropnum = monster_refreshs[i].reward[l].y;
                            var noDropNum = totalSpawnNum - dropnum;
                            double interval = (double)(totalSpawnNum - 1) / (dropnum - 1);

                            var dropList = new NativeList<int>(Allocator.Temp);

                            for (int m = 0; m < dropnum; m++)
                            {
                                // 使用 Floor 调整以更接近 [0, 3, 9]
                                int index = math.max(0, (int)math.floor(m * interval));
                                //if (m == 1 && index == 4) index = 3; // 手动调整中间索引

                                if (!dropList.Contains(index))
                                {
                                    dropList.Add(index);
                                }
                            }

                            // Debug.Log(
                            //     $"totalSpawnNum{totalSpawnNum}dropnum{dropnum}noDropNum {noDropNum} ");
                            // foreach (var inta in dropList)
                            // {
                            //     Debug.Log(
                            //         $" drop{inta} ");
                            // }

                            if (dropList.Length < dropnum)
                            {
                                int index = 0;
                                while (dropList.Length == dropnum)
                                {
                                    if (!dropList.Contains(index))
                                    {
                                        dropList.Add(index);
                                    }

                                    index++;
                                }
                            }

                            int singleNum = monster_refreshs[i].reward[l].y / totalSpawnNum;


                            int otherNum = singleNum > 0 ? monster_refreshs[i].reward[l].y % totalSpawnNum : 0;

                            singleNum = math.max(1, singleNum);


                            if (dropList.Contains(insIndex))
                            {
                                //Debug.Log($"刷的index {insIndex}");
                                for (int j = 0; j < singleNum; j++)
                                {
                                    ecb.AppendToBuffer(sortKey, ins, new DropsBuffer
                                    {
                                        id = monster_refreshs[i].reward[l].x
                                    });
                                }

                                if (otherNum > 0 && insIndex < otherNum)
                                {
                                    //Debug.Log($"刷的附加index {insIndex}");
                                    ecb.AppendToBuffer(sortKey, ins, new DropsBuffer
                                    {
                                        id = monster_refreshs[i].reward[l].x
                                    });
                                }
                            }


                            dropList.Dispose();
                            // if (startNum <= insIndex)
                            // {
                            //    
                            // }
                        }

                        insIndex++;
                        //weapon
                        ref var monsterWeapon = ref monster_weapons[0];
                        Entity weaponIns = Entity.Null;

                        #region Weapon

                        if (monsterWeapon.type == 1)
                        {
                            int monsterWeaponIndex = 0;
                            for (int l = 0; l < monster_weapons.Length; l++)
                            {
                                if (monster.monsterWeaponId == monster_weapons[l].id)
                                {
                                    monsterWeaponIndex = l;
                                    break;
                                }
                            }

                            monsterWeapon = ref monster_weapons[monsterWeaponIndex];

                            //FixedString128Bytes weaponName = $"weapon_{monster.monsterWeaponId}";

                            if (prefabMapData.prefabHashMap.TryGetValue(monsterWeapon.model,
                                    out var weaponPrefab))
                            {
                                var weaponData = cdfeWeaponData[weaponPrefab];

                                weaponIns = ecb.Instantiate(sortKey, weaponPrefab);

                                ecb.AppendToBuffer(sortKey, ins, new LinkedEntityGroup()
                                {
                                    Value = weaponIns
                                });
                                ecb.AddComponent(sortKey, weaponIns, new Parent()
                                {
                                    Value = ins
                                });
                                weaponData.enableEnemyWeapon =
                                    monster.monsterWeaponRunDisplay == 1 ? true : false;
                                weaponData.weaponId = monster.monsterWeaponId;
                                ref var animPara = ref monsterWeapon.displayPara1;
                                if (monsterAttr.type == 5)
                                {
                                    ref var animPara2 = ref monsterWeapon.displayPara2;
                                    BuffHelper.SetWeaponData(ref weaponData, ref animPara2,
                                        monsterWeapon.displayType,
                                        true);
                                }
                                else
                                {
                                    BuffHelper.SetWeaponData(ref weaponData, ref animPara,
                                        monsterWeapon.displayType);
                                }


                                weaponData.offset = new float3(monster.monsterWeaponIndex[0] / 1000f,
                                    monster.monsterWeaponIndex[1] / 1000f, 0);
                                weaponData.scale = monster.monsterWeaponIndex[2] / 10000f;
                                weaponData.rotation = quaternion.RotateZ(math.radians(monster.monsterWeaponIndex[3]));


                                ecb.SetComponent(sortKey, weaponIns, weaponData);
                            }
                        }

                        #endregion


                        // var newpos = new LocalTransform
                        // {
                        //     Position = new float3(pos, 0),
                        //     Scale = cdfeLocalTransform[prefab].Scale,
                        //     Rotation = cdfeLocalTransform[prefab].Rotation
                        // };
                        //
                        //
                        // ecb.SetComponent(entityIndexInQuery, ins, newpos);

                        var chaStat = cdfeChaStats[prefab];

                        //var physicsCollider = cdfePhysicsCollider[prefab];
                        var AgentLocomotion = cdfeAgentLocomotion[prefab];

                        var enemyData = cdfeEnemyData[prefab];
                        // PhysicsCollider newCollider =
                        //     PhysicsHelper.CreateColliderWithBelongsTo(physicsCollider,
                        //         (uint)monster.type);


                        chaStat.chaProperty.maxHp =
                            (int)(monster_refreshs[i].hp * (level.hp / 10000f) *
                                  (monsterAttr.hp / 10000f));
                        // chaStat.chaResource.hp =
                        //     (int)(monster_refreshs[i].hp * (level.hp / 10000f) *
                        //           (monsterAttr.hp / 10000f));


                        chaStat.chaProperty.atk =
                            (int)(monster_refreshs[i].atk * (level.atk / 10000f) *
                                  (monsterAttr.atk / 10000f) *
                                  (monsterWeapon.paraAtk / 10000f));


                        if (monster.monsterWeaponId > 0 && monsterAttr.type != 5)
                        {
                            chaStat.chaProperty.pushForce =
                                (int)((monsterAttr.force) *
                                      (monsterWeapon.paraForce / 10000f));
                        }
                        else
                        {
                            chaStat.chaProperty.pushForce =
                                (int)((monsterAttr.force));
                        }


                        chaStat.chaProperty.maxMoveSpeed = (int)(monsterAttr.speed * monsterWeapon.paraSpeed / 10000f);

                        chaStat.chaProperty.mass = monsterAttr.mass;
                        if (cdfePhysicsMass.HasComponent(prefab))
                        {
                            var physicsMass = cdfePhysicsMass[prefab];
                            physicsMass.InverseMass = 1f / (float)monsterAttr.mass;
                            ecb.SetComponent(sortKey, ins, physicsMass);
                        }


                        //TODO:三个字段
                        //baseRestoreTime    repelResistPlus  repelResistRestore
                        //chaStat.chaProperty.basicRecoveryTime = monsters[index].baseRestoreTime;
                        //chaStat.chaProperty.speedRecoveryTime = monsterAttr.speedRefresh;
                        chaStat.chaProperty.reduceHitBackRatios = monsterAttr.repelResistAddition;
                        chaStat.chaProperty.scaleRatios = 10000;


                        // chaStat.chaProperty.maxReduceHitBack =
                        //     monsters[index].repelResistPlus;
                        // chaStat.chaProperty.speedRecoveryTime =
                        //     monsters[index].repelResistRestore;
                        // var rand = Unity.Mathematics.Random.CreateFromIndex(
                        //     (uint)math.abs(ins.Index + ins.Version + k));
                        // var nextFloat = rand.NextFloat(0.4f, 0.6f);
                        // chaStat.chaResource.actionSpeed = nextFloat;
                        //TODO:
                        chaStat.chaResource.actionSpeed = 1;

                        AgentLocomotion.Speed = chaStat.chaProperty.maxMoveSpeed / 1000f;
                        enemyData.enemyID = monster_refreshs[i].monsterId;
                        enemyData.type = UnityHelper.GetTargetId(monsterAttr.type);
                        enemyData.bossSceneId = monster.sceneBossId;

                        enemyData.commonSkillCd =
                            monster.commonCd / 1000f +
                            math.max(monsterWeapon.paraInterval / 1000f - monsterWeapon.displayTime / 1000f, 0);
                        enemyData.curCommonSkillCd = enemyData.commonSkillCd;

                        enemyData.changeDirYn = monsterAttr.changeDirYn;
                        enemyData.attackType = EnemyAttackType.None;
                        enemyData.isHitBackAttack = false;
                        if (monsterWeapon.type == 1)
                        {
                            //enemyData.entityGroup.weaponEntity = weaponIns;
                            enemyData.weaponId = monsterWeapon.id;
                            switch (monsterWeapon.atkType)
                            {
                                case 1:
                                    enemyData.attackType = EnemyAttackType.NormalShortAttack;
                                    enemyData.isHitBackAttack = false;

                                    //enemyData.attackRadius = UnityHelper.ShortAttackRange;
                                    break;
                                case 2:
                                    enemyData.attackType = EnemyAttackType.NormalShortAttack;
                                    enemyData.isHitBackAttack = true;
                                    //enemyData.attackRadius = UnityHelper.ShortAttackRange;
                                    break;
                                case 3:
                                    enemyData.attackType = EnemyAttackType.NormalShortAttack;
                                    enemyData.isHitBackAttack = true;
                                    //enemyData.attackRadius = UnityHelper.ShortAttackRange;
                                    break;
                                case 4:
                                    enemyData.attackType = EnemyAttackType.NormalLongAttack;
                                    enemyData.isHitBackAttack = false;
                                    //enemyData.attackRadius = UnityHelper.LongAttackRange;
                                    break;
                                case 5:
                                    enemyData.attackType = EnemyAttackType.NormalLongAttack;
                                    enemyData.isHitBackAttack = true;
                                    //enemyData.attackRadius = UnityHelper.LongAttackRange;
                                    break;
                            }
                        }

                        enemyData.enemyRefreshGroupId = monster_refreshs[i].ruleId;


                        enemyData.moveType = (EnemyMoveType)1;
                        enemyData.moveTypePara = new int4(1, 0, 0, 0);
                        if (monster.monsterWeaponId > 0)
                        {
                            enemyData.moveType = (EnemyMoveType)monsterWeapon.moveType[0];
                            enemyData.moveTypePara = new int4(monsterWeapon.moveType[0], monsterWeapon.moveType[1],
                                monsterWeapon.moveType[2], monsterWeapon.moveType[3]);
                        }

                        enemyData.targetGroup = 1;
                        for (int l = 0; l < monsterModel.featureId.Length; l++)
                        {
                            var id = monsterModel.featureId[l];
                            enemyData.enemyFeature.Add(id);

                            ecb.AppendToBuffer(sortKey, ins, new Buff()
                            {
                                CurrentTypeId = (Buff.TypeId)id + 999000,
                                Int32_0 = id,
                                Boolean_4 = true,
                                Entity_6 = ins
                            });
                        }

                        //混合spine流程
                        if (enemyData.isHybridSpine)
                        {
                            ecb.AddComponent(sortKey, ins, new SpineHybridData()
                            {
                            });
                            ecb.AddComponent(sortKey, ins, new JiYuFlip()
                            {
                            });
                        }

                        if (!isBossScene && enemyData.isHybridSpine)
                        {
                            var newentity = ecb.CreateEntity(sortKey);
                            ecb.AddComponent(sortKey, newentity, new HybridEventData
                            {
                                type = 12,
                                args = new float4(monster.id, float3.zero),
                                bossEntity = ins
                            });
                        }

                        var stateType = monster.skillGroup.Length > 0 || monster.passiveSkill.Length > 0 ? 3 : 2;
                        enemyData.canCastSkill = stateType == 3;
                        if (enemyData.type == (int)BuffHelper.TargetEnum.BossMonster)
                        {
                            ecb.AddComponent<BossTag>(sortKey, ins);
                        }

                        //ecb.SetComponent(entityIndexInQuery, ins, newCollider);
                        UnityHelper.AddStateToStatesBuffer(ref ecb, sortKey,
                            stateType, ins);
                        chaStat.chaProperty.atk = math.max(1, chaStat.chaProperty.atk);
                        chaStat.chaProperty.maxHp = math.max(1, chaStat.chaProperty.maxHp);

                        chaStat.chaResource.hp = chaStat.chaProperty.maxHp;
                        chaStat.chaProperty.defaultMass = chaStat.chaProperty.mass;
                        chaStat.chaProperty.defaultAtk = chaStat.chaProperty.atk;
                        chaStat.chaProperty.defaultMaxHp = chaStat.chaProperty.maxHp;
                        chaStat.chaProperty.defaultHpRecovery = chaStat.chaProperty.hpRecovery;
                        chaStat.chaProperty.defaultMaxMoveSpeed = chaStat.chaProperty.maxMoveSpeed;
                        chaStat.chaProperty.defaultPushForce = chaStat.chaProperty.pushForce;
                        //chaStat.chaProperty.defaultBulletRangeRatios = chaStat.chaProperty.b;
                        if (monster.timeToDie > 0)
                        {
                            ecb.AddComponent(sortKey, ins, new TimeToDieData
                            {
                                duration = monster.timeToDie / 1000f
                            });
                        }

                        ecb.SetComponent(sortKey, ins, chaStat);
                        ecb.SetComponent(sortKey, ins, AgentLocomotion);
                        ecb.SetComponent(sortKey, ins, enemyData);
                        ecb.SetComponent(sortKey, ins, new TargetData
                        {
                            tick = 0,
                            BelongsTo = (uint)enemyData.type,
                            AttackWith = (uint)BuffHelper.TargetGroupEnum.PlayerGroup
                        });
                        for (int l = 0; l < monster.skillGroup.Length; l++)
                        {
                            var skill = new Skill()
                            {
                                CurrentTypeId = (Skill.TypeId)1,
                                Int32_0 = monster.skillGroup[l],
                                Entity_5 = ins,
                                Int32_10 = 2,
                                //Boolean_11 = true,
                                Int32_6 = 2
                            };
                            ecb.AppendToBuffer(sortKey, ins, skill);
                        }

                        for (int l = 0; l < monster.passiveSkill.Length; l++)
                        {
                            var skill = new Skill()
                            {
                                CurrentTypeId = (Skill.TypeId)1,
                                Int32_0 = monster.passiveSkill[l],
                                Entity_5 = ins,
                                Int32_10 = 2,
                                Boolean_17 = true
                            };
                            ecb.AppendToBuffer(sortKey, ins, skill);
                        }
                    }
                }
            }

            private int GetSpawnFormationCount(int formationId)
            {
                int hundreds = formationId / 100; // 获取百位数字
                int tens = (formationId % 100) / 10; // 获取十位数字
                int ones = formationId % 10; // 获取个位数字
                int count = default;
                if (tens == 0)
                {
                    count = ones;
                }
                else
                {
                    count = tens * 10 + ones;
                }

                return count;
            }

            private NativeList<Entity> SpawnFormationEnemy(EntityCommandBuffer.ParallelWriter ecb, Entity prefab,
                int formationId, int sortkey, ComponentLookup<LocalTransform> cdfeLocalTransform, float2 mapWidthHeight,
                uint index, bool isBossScene, float3 playerPos, Entity groupEntity, Entity surfaceEntity,
                float bigCircle, float scale, int monsterId)
            {
                const float Internal = 12f;

                int count = GetSpawnFormationCount(formationId);
                //Debug.LogError($"{count}");
                NativeList<Entity> enemies = new NativeList<Entity>(count, Allocator.Temp);
                var mapRect = new MathHelper.Rectangle
                {
                    center = default,
                    width = mapWidthHeight.x,
                    height = mapWidthHeight.y
                };
                //bool isInit = false;
                float2 originalPos = default;
                for (int i = 0; i < count; i++)
                {
                    var ins = ecb.Instantiate(sortkey, prefab);

                    if (i == 0 && !isBossScene)
                    {
                        var rand = Unity.Mathematics.Random.CreateFromIndex(
                            (uint)math.abs(gameRandomData.seed + index + timeTick));

                        originalPos = GenerateRandomPoint(bigCircle, bigCircle + 80f, playerPos.xy
                            , ref rand, mapWidthHeight, globalConfigData, obstacles,
                            cdfeLocalTransform,
                            cdfeMapElementData);

                        originalPos = GenerateRandomPoint(new MathHelper.Rectangle
                            {
                                center = playerPos.xy,
                                width = gameCameraSizeData.width,
                                height = gameCameraSizeData.height
                            }, new MathHelper.Rectangle
                            {
                                center = playerPos.xy,
                                width = gameCameraSizeData.width + UnityHelper.MaxRefreshEnemyInterval,
                                height = gameCameraSizeData.height + UnityHelper.MaxRefreshEnemyInterval
                            }, ref rand, mapWidthHeight, globalConfigData, obstacles,
                            cdfeLocalTransform,
                            cdfeMapElementData);
                    }

                    float3 pos = default;
                    int row = default; // 每行3个敌人
                    int col = default; // 每列3个敌人
                    switch (formationId)
                    {
                        case 102:

                            pos.x = i == 0 ? -Internal : Internal;

                            break;
                        case 103:
                            if (i == 1)
                            {
                                pos.x = -Internal;
                            }
                            else if (i == 2)
                            {
                                pos.x = Internal;
                            }

                            break;
                        case 105:
                            if (i == 1)
                            {
                                pos.x = -Internal;
                            }
                            else if (i == 2)
                            {
                                pos.x = Internal;
                            }
                            else if (i == 3)
                            {
                                pos.y = -Internal;
                            }
                            else if (i == 4)
                            {
                                pos.y = Internal;
                            }

                            break;
                        case 109:
                            row = i / 3; // 每行3个敌人
                            col = i % 3; // 每列3个敌人

                            pos.x = col * Internal;
                            pos.y = -row * Internal;

                            break;
                        case 122:
                            row = i / 4; // 每行4个敌人
                            col = i % 4; // 每列4个敌人

                            pos.x = col * Internal;

                            if (row % 2 == 1) // 奇数行向右偏移一定距离
                            {
                                pos.x += Internal / 2;
                            }

                            pos.y = -row * Internal;
                            break;
                    }

                    var newpos = new LocalTransform
                    {
                        Position = pos + new float3(originalPos.xy, 0),
                        Scale = 0.001f,
                        Rotation = cdfeLocalTransform[prefab].Rotation
                    };
                    if (!mapRect.Contains(newpos.Position.xy) && !isBossScene)
                    {
                        newpos.Position = pos;
                    }

                    if (isBossScene)
                    {
                        var bosspos = playerPos;
                        bosspos.y += 50f;
                        newpos.Position = bosspos;
                        //TODO:
                        // ecb.AddSharedComponent(sortkey, ins, new AgentCrowdPath
                        // {
                        //     Group = groupEntity
                        // });
                        ecb.AppendToBuffer(sortkey, surfaceEntity, new LinkedEntityGroup
                        {
                            Value = ins
                        });

                        //var newentity = ecb.CreateEntity(sortKey);
                        // ecb.AddComponent(sortKey, newentity, new HybridEventData
                        // {
                        //     type = 2,
                        //     args = new float4(pos, bossMapID)
                        // });

                        //boss转换场景事件
                        Debug.LogError($"生成boss {monsterId}");
                        var newentity = ecb.CreateEntity(sortkey);
                        ecb.AddComponent(sortkey, newentity, new HybridEventData
                        {
                            type = 2,
                            args = new float4(monsterId, playerPos),
                            bossEntity = ins
                        });
                        
                        var newentity1 = ecb.CreateEntity(sortkey);
                        ecb.AddComponent(sortkey, newentity1, new HybridEventData
                        {
                            type = 12,
                            args = new float4(monsterId),
                            bossEntity = ins
                        });
                    }

                    //TODO：目标大小

                    //var scale = UnityHelper.LittleMonsterScale;

                    // if (isBossScene)
                    // {
                    //     scale = UnityHelper.BossScale;
                    // }

                    ecb.SetComponent(sortkey, ins, new PushColliderData
                    {
                        targetScale = scale
                    });
                    ecb.SetComponent(sortkey, ins, newpos);
                    ecb.SetComponent(sortkey, ins, new AgentShape
                    {
                        Radius = scale / 2f * 1.5f,
                        Height = 0,
                        Type = ShapeType.Circle
                    });
                    enemies.Add(ins);
                }

                return enemies;
            }

            private float2 GetMapWidthHeight(GlobalConfigData globalConfigData, GameOthersData gameOthersData,
                out int mapId, out int mapType)
            {
                mapType = gameOthersData.mapData.mapType;
                mapId = gameOthersData.mapData.mapID;

                float2 mapWithHeight = gameOthersData.mapData.mapSize;
                if (mapType == 1)
                {
                    mapWithHeight = new float2(mapWithHeight.x, MathHelper.MaxNum);
                }
                else if (mapType == 2)
                {
                    mapWithHeight = new float2(MathHelper.MaxNum, mapWithHeight.y);
                }
                else if (mapType == 3)
                {
                }
                else if (mapType == 4)
                {
                    mapWithHeight = new float2(MathHelper.MaxNum, MathHelper.MaxNum);
                }

                //Debug.LogError($"mapId{mapId} mapWithHeight {mapWithHeight} mapType{mapType} ");
                return mapWithHeight;
            }

            private float2 GenerateRandomPoint(MathHelper.Rectangle smallRect, MathHelper.Rectangle bigRect,
                ref Random rand,
                float2 mapWithHeight, GlobalConfigData globalConfigData, NativeArray<Entity> obstacles,
                ComponentLookup<LocalTransform> cdfeLocalTransform, ComponentLookup<MapElementData> cdfeMapElementData)
            {
                ref var scene_module = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;

                NativeList<MathHelper.Rectangle> obstacleList =
                    new NativeList<MathHelper.Rectangle>(obstacles.Length, Allocator.Temp);
                for (int i = 0; i < obstacles.Length; i++)
                {
                    float width = 0;
                    float height = 0;
                    bool hasSize = true;
                    for (int j = 0; j < scene_module.Length; j++)
                    {
                        if (cdfeMapElementData[obstacles[i]].elementID == scene_module[j].id)
                        {
                            if (scene_module[j].size.Length <= 0)
                            {
                                hasSize = false;
                                break;
                            }

                            width = scene_module[j].size[0] / 1000f;
                            height = scene_module[j].size[1] / 1000f;
                            break;
                        }
                    }

                    if (!hasSize)
                    {
                        continue;
                    }

                    obstacleList.Add(new MathHelper.Rectangle
                    {
                        center = cdfeLocalTransform[obstacles[i]].Position.xy,
                        width = width,
                        height = height
                    });
                }

                var mapRectangle = new MathHelper.Rectangle
                {
                    center = float2.zero,
                    width = math.abs(mapWithHeight.x),
                    height = math.abs(mapWithHeight.y)
                };

                float2 point = smallRect.center;
                for (int i = 0; i < MathHelper.MaxNum; i++)
                {
                    point.x = rand.NextFloat((bigRect.center.x - bigRect.width / 2f),
                        (bigRect.center.x + bigRect.width / 2f));
                    point.y = rand.NextFloat((bigRect.center.y - bigRect.height / 2f),
                        (bigRect.center.y + bigRect.height / 2f));

                    //TODO:在地图内部 不可在地图外部生成
                    //Debug.Log($"point {point}");
                    if (!smallRect.Contains(point) && mapRectangle.Contains(point))
                    {
                        bool isAllOut = true;
                        foreach (var rec in obstacleList)
                        {
                            if (rec.Contains(point))
                            {
                                isAllOut = false;
                                break;
                            }
                        }

                        if (isAllOut)
                        {
                            break;
                        }
                    }
                }


                return point;
            }

            private float2 GenerateRandomPoint(float smallCircle, float bigCircle, float2 playerPos,
                ref Random rand,
                float2 mapWithHeight, GlobalConfigData globalConfigData, NativeArray<Entity> obstacles,
                ComponentLookup<LocalTransform> cdfeLocalTransform, ComponentLookup<MapElementData> cdfeMapElementData)
            {
                ref var scene_module = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;

                NativeList<MathHelper.Rectangle> obstacleList =
                    new NativeList<MathHelper.Rectangle>(obstacles.Length, Allocator.Temp);
                for (int i = 0; i < obstacles.Length; i++)
                {
                    float width = 0;
                    float height = 0;
                    bool hasSize = true;
                    for (int j = 0; j < scene_module.Length; j++)
                    {
                        if (cdfeMapElementData[obstacles[i]].elementID == scene_module[j].id)
                        {
                            if (scene_module[j].size.Length <= 0)
                            {
                                hasSize = false;
                                break;
                            }

                            width = scene_module[j].size[0] / 1000f;
                            height = scene_module[j].size[1] / 1000f;
                            break;
                        }
                    }

                    if (!hasSize)
                    {
                        continue;
                    }

                    obstacleList.Add(new MathHelper.Rectangle
                    {
                        center = cdfeLocalTransform[obstacles[i]].Position.xy,
                        width = width,
                        height = height
                    });
                }

                var mapRectangle = new MathHelper.Rectangle
                {
                    center = float2.zero,
                    width = math.abs(mapWithHeight.x),
                    height = math.abs(mapWithHeight.y)
                };

                float2 point = playerPos;
                const int MaxTimes = 999;
                for (int i = 0; i < MaxTimes; i++)
                {
                    if (i == MaxTimes - 1)
                    {
                        Debug.LogError($"没选到点");
                    }

                    // 随机选择一个角度
                    float angle = (float)(rand.NextFloat() * 2 * math.PI); // 0到2π的随机数

                    // 随机选择一个半径，范围在小圆到大圆之间
                    float radius = (float)(rand.NextFloat() * (bigCircle - smallCircle) + smallCircle);

                    // 使用极坐标转化为笛卡尔坐标
                    point.x = playerPos.x + radius * math.cos(angle);
                    point.y = playerPos.y + radius * math.sin(angle);

                    //TODO:在地图内部 不可在地图外部生成

                    if (math.length(point - playerPos) > smallCircle && mapRectangle.Contains(point))
                    {
                        bool isAllOut = true;
                        foreach (var rec in obstacleList)
                        {
                            if (rec.Contains(point))
                            {
                                isAllOut = false;
                                break;
                            }
                        }

                        if (isAllOut)
                        {
                            break;
                        }
                    }
                }


                return point;
            }
        }
    }
}