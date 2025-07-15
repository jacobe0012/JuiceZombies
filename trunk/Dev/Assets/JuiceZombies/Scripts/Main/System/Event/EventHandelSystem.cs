using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(StateMachineSystem))]
    public partial struct EventHandelSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameEvent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
            var nonTriggerDynamicBodyQuery = SystemAPI.QueryBuilder().WithAll<ChaStats>()
                .WithNone<PlayerData, EnemyData>().Build();
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);
            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);
            var gameCameraSizeData = SystemAPI.GetComponent<GameCameraSizeData>(wbe);
            var player = SystemAPI.GetSingletonEntity<PlayerData>();
            //var indexMap = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>().Length - 1;
            // var tiggerEvent = SystemAPI.QueryBuilder().WithAny<StatefulTriggerEvent>().Build();
            // var colliderEvent = SystemAPI.QueryBuilder().WithAny<StatefulTriggerEvent>().Build();
            //var eventQuery = SystemAPI.QueryBuilder().WithAll<GameEvent>().Build();
            new EventHandelSystemJob
            {
                ecb = ecb,
                fdT = SystemAPI.Time.fixedDeltaTime,
                timeTick = (int)(SystemAPI.Time.ElapsedTime * 1000),
                gameRandomData = gameRandomData,
                allLocDatas = SystemAPI.GetComponentLookup<LocalTransform>(true),
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(),
                allTriggers = SystemAPI.GetBufferLookup<StatefulTriggerEvent>(true),
                prefabMapData = prefabMapData,
                globalConfigData = globalConfigData,
                gameOthersData = gameOthersData,
                NonTriggerDynamicBodyMask = nonTriggerDynamicBodyQuery.GetEntityQueryMask(),
                targets = SystemAPI.QueryBuilder()
                    .WithAny<PlayerData, EnemyData, MapElementData>()
                    .Build()
                    .ToEntityArray(Allocator.TempJob),
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                cdfeBuffs = SystemAPI.GetBufferLookup<Buff>(),
                cdfeSkills = SystemAPI.GetBufferLookup<Skill>(),
                cdfeEnviromentData = SystemAPI.GetComponentLookup<GameEnviromentData>(),
                cdfeGameTimeData = SystemAPI.GetComponentLookup<GameTimeData>(),
                cdfeWeaponData = SystemAPI.GetComponentLookup<WeaponData>(true),
                cdfeAgentLocomotion = SystemAPI.GetComponentLookup<AgentLocomotion>(true),
                cdfePhysicsMass = SystemAPI.GetComponentLookup<PhysicsMass>(),
                player = SystemAPI.GetSingletonEntity<PlayerData>(),
                wbe = wbe,
                allEnemies = SystemAPI.GetComponentLookup<EnemyData>(true),
                allPlayers = SystemAPI.GetComponentLookup<PlayerData>(),
                mapModules =
                    SystemAPI.QueryBuilder()
                        .WithAny<ObstacleTag, AreaTag>()
                        .Build()
                        .ToEntityArray(Allocator.TempJob),
                cdfeMapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
                cdfeTargetData = SystemAPI.GetComponentLookup<TargetData>(true),
                gameTimeData = gameTimeData,
                gameCameraSizeData = gameCameraSizeData,
                cdfeTimeToDieData = SystemAPI.GetComponentLookup<TimeToDieData>(true),
                cdfeTriggers = SystemAPI.GetBufferLookup<Trigger>(),
                cdfePostTransformMatrix = SystemAPI.GetComponentLookup<PostTransformMatrix>(true),
                playerData=SystemAPI.GetComponent<PlayerData>(player),
                cdfePhysicsVelocity=SystemAPI.GetComponentLookup<PhysicsVelocity>()
            }.ScheduleParallel();
        }


        [BurstCompile]
        public partial struct EventHandelSystemJob : IJobEntity
        {
            
            public EntityQueryMask NonTriggerDynamicBodyMask;
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] public float fdT;
            [ReadOnly] public int timeTick;
            public GameRandomData gameRandomData;
            public GameTimeData gameTimeData;
            [ReadOnly] public ComponentLookup<LocalTransform> allLocDatas;
            [NativeDisableParallelForRestriction] public ComponentLookup<ChaStats> cdfeChaStats;
            [ReadOnly] public ComponentLookup<EnemyData> allEnemies;
            [NativeDisableParallelForRestriction] public ComponentLookup<PlayerData> allPlayers;
            [ReadOnly] public PrefabMapData prefabMapData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public GameOthersData gameOthersData;
            [ReadOnly] public GameCameraSizeData gameCameraSizeData;
            [ReadOnly] public BufferLookup<StatefulTriggerEvent> allTriggers;
            [ReadOnly] public Entity player;
            [ReadOnly] public Entity wbe;
            [ReadOnly] public ComponentLookup<TimeToDieData> cdfeTimeToDieData;
            [ReadOnly] public PlayerData playerData;
            [ReadOnly] public ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> targets;

            [ReadOnly] public ComponentLookup<TargetData> cdfeTargetData;
            [ReadOnly] public ComponentLookup<WeaponData> cdfeWeaponData;
            [ReadOnly] public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;
            [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsMass> cdfePhysicsMass;

            //[ReadOnly] public float3 startPos;
            //[ReadOnly] public int currentMapIndex;
            [ReadOnly] public ComponentLookup<MapElementData> cdfeMapElementData;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> mapModules;

            public EntityStorageInfoLookup storageInfoFromEntity;
            [NativeDisableParallelForRestriction] public BufferLookup<Buff> cdfeBuffs;

            [NativeDisableParallelForRestriction] public BufferLookup<Skill> cdfeSkills;
            [NativeDisableParallelForRestriction] public BufferLookup<Trigger> cdfeTriggers;
            [NativeDisableParallelForRestriction] public ComponentLookup<GameEnviromentData> cdfeEnviromentData;
            [NativeDisableParallelForRestriction] public ComponentLookup<GameTimeData> cdfeGameTimeData;

            [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsVelocity> cdfePhysicsVelocity;
            //[ReadOnly] public ComponentLookup<PhysicsMass> allPhysicsMass;
            //[NativeDisableParallelForRestriction] public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;
            //[NativeDisableParallelForRestriction] public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
            //[ReadOnly] public ComponentLookup<ObstacleTag> obstacles;
            public void Execute([EntityIndexInQuery] int index, Entity elementEntity,
                ref DynamicBuffer<GameEvent> gameEvents)
            {
                //var rand = gameRandomData.rand;


                GameEventData_ReadWrite refData = new GameEventData_ReadWrite
                {
                    //cdfeBuffs = default,
                    ecb = ecb,
                    cdfeEnviromentData = cdfeEnviromentData,
                    cdfeGameTimeData = cdfeGameTimeData,
                    cdfeBuffs = cdfeBuffs,
                    cdfeSkills = cdfeSkills,
                    cdfePlayerData = allPlayers,
                    cdfeChaStats = cdfeChaStats,
                    randomData = gameRandomData,
                    cdfePhysicsMass = cdfePhysicsMass,
                    cdfeTriggers = cdfeTriggers,
                    cdfeGameEvent = gameEvents,
                    StorageInfoLookup = storageInfoFromEntity,
                    cdfePhysicsVelocity= cdfePhysicsVelocity
                };
                GameEventData_ReadOnly inData = new GameEventData_ReadOnly
                {
                    wbe = wbe,
                    storageInfoFromEntity = storageInfoFromEntity,
                    cdfeLocalTransform = allLocDatas,
                    cdfeWeaponData = cdfeWeaponData,
                    config = globalConfigData,
                    sortKey = index,
                    selefEntity = elementEntity,
                    prefabMapData = prefabMapData,
                    gameOthersData = gameOthersData,
                    gameTimeData = gameTimeData,
                    entities = targets,
                    player = player,
                    fDT = fdT,
                    cdfeEnemyData = allEnemies,
                    currentMapStartPos = default,
                    cdfeMapElementData = cdfeMapElementData,
                    mapModels = mapModules,
                    cdfeTargetData = cdfeTargetData,
                    cdfeAgentLocomotion = cdfeAgentLocomotion,
                    timeTick = timeTick,
                    gameCameraSizeData = gameCameraSizeData,
                    cdfeTimeToDieData = cdfeTimeToDieData,
                    cdfePostTransformMatrix = cdfePostTransformMatrix,
                    gameRandomData = gameRandomData,
                    playerData= playerData
                };

                for (int i = 0; i < gameEvents.Length; i++)
                {
                    var gameEvent = gameEvents[i];
                    var currentTick = gameEvent.Int32_5;
                    //第一帧执行
                    if (currentTick == 0 && !gameEvent.Boolean_12)
                    {
                        ref var tbevent = ref globalConfigData.value.Value.configTbevent_0s.configTbevent_0s;
                        int eventIndex = -1;
                        for (int j = 0; j < tbevent.Length; j++)
                        {
                            if (tbevent[j].id == gameEvent.Int32_0)
                            {
                                eventIndex = j;
                                break;
                            }
                        }

                        if (eventIndex == -1)
                        {
                            Debug.Log($"事件id:{gameEvent.Int32_0} event表里不存在");
                            //continue;
                        }
                        else
                        {
                            ref var thisEvent = ref tbevent[eventIndex];
                            gameEvent.CurrentTypeId = (GameEvent.TypeId)thisEvent.type;
                        }
                    }


                    if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_11 ||
                        gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_12)
                    {
                        if (currentTick == 0)
                        {
                            OnTick0(ref gameEvent);
                        }

                        gameEvent.Int32_5++;
                        //gameEvent.Single_4 -= fdT;
                        gameEvents[i] = gameEvent;
                        continue;
                    }


                    //剩余执行时间<=0
                    if (gameEvent.Single_4 <= 0 && !gameEvent.Boolean_6 && currentTick > 0)
                    {
                        gameEvent.OnEventRemove(ref refData, in inData);

                        gameEvent.Int32_5++;
                        gameEvents[i] = gameEvent;
                        gameEvents.RemoveAt(i);

                        continue;
                    }


                    //第一帧执行
                    if (currentTick == 0)
                    {
                        if (!gameEvent.Boolean_12)
                        {
                            OnTick0(ref gameEvent);
                        }

                        //gameEvent.OnTick0(ref refData, in inData);
                        if (gameEvent.GameEventTriggerType_1 == GameEventTriggerType.ImmediateEffect)
                        {
                            gameEvent.OnOccur(ref refData, in inData);
                            if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_12 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_13 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_23 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_21 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_31 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_32 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_33 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_34 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_35 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_36 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_37 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_38 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_51 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_52 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_54 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_56)
                            {
                                gameEvent.Boolean_6 = false;
                            }
                        }
                    }

                    if (currentTick > 0)
                    {
                        //UnityEngine.Debug.LogError($"currentTick:{currentTick}");
                        gameEvent.OnUpdate(ref refData, in inData);
                        //处理随机时间的逻辑 bool 13为true int3-7的x为间隔 y为便宜
                        if (gameEvent.Boolean_13)
                        {
                            if (gameEvent.int3_7.x > 0 && gameEvent.int3_7.z == 0)
                            {
                                UnityEngine.Debug.Log(
                                    $"OnRandom gameEvent.int3_7.x > 0 && gameEvent.int3_7.z == 0");
                                var rand = Random.CreateFromIndex((uint)timeTick);
                                var randTime = rand.NextInt(gameEvent.int3_7.x - gameEvent.int3_7.y,
                                    gameEvent.int3_7.x + gameEvent.int3_7.y + 1);
                                gameEvent.Single_2 = randTime;
                                gameEvent.int3_7.z = 1;
                            }

                            if (gameEvent.Single_2 > 0)
                            {
                                //UnityEngine.Debug.Log($"gameEvent.Single_2 > 0 :{gameEvent.Single_2}");
                                if ((currentTick % (int)(gameEvent.Single_2 / fdT)) == 0)
                                {
                                    UnityEngine.Debug.Log(
                                        $"currentTick :{currentTick},gap:{gameEvent.Single_2 / fdT}");
                                    gameEvent.int3_7.z = 0;
                                    gameEvent.OnRandom(ref refData, inData);
                                }
                            }
                        }
                        var tickGap = (int)(gameEvent.Single_2 / fdT);
                        if (tickGap > 0)
                        {
                            //UnityEngine.Debug.LogError($"onPerGap,1111111111111:{currentTick}");
                            if (currentTick % tickGap == 0 && gameEvent.Int32_10 > 0)
                            {
                                UnityEngine.Debug.Log($"onPerGap,currentTick{currentTick},tickgap:{tickGap}");
                                gameEvent.OnPerGap(ref refData, in inData);
                                gameEvent.Int32_10--;
                            }
                        }
                    }
                    ////存在延迟时
                    //var needDelay = (int)(gameEvent.Single_10 / fdT);
                    //if (needDelay != 0 && currentTick <= needDelay)
                    //{
                    //    gameEvent.Single_4 -= fdT;
                    //    gameEvent.Single_5 += fdT;
                    //    gameEvents[i] = gameEvent;
                    //    return;
                    //}

                    if (gameEvent.GameEventTriggerType_1 == GameEventTriggerType.IntervalTime ||
                        (gameEvent.Boolean_12 && gameEvent.Single_2 > 0))
                    {
                        if (gameEvent.Int32_10 <= 0)
                        {
                            if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_12 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_13 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_21 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_25 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_31 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_32 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_33 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_34 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_35 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_36 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_37 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_38 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_51 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_52 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_54 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_56)
                            {
                                gameEvent.Boolean_6 = false;
                            }
                        }
                        //if ((currentTick - needDelay) % tickGap == 0)
                        //{
                        //    gameEvent.OnPerGap(ref refData, in inData);
                        //}
                    }

                    if (gameEvent.GameEventTriggerType_1 == GameEventTriggerType.AppointedTime)
                    {
                        if ((int)(gameEvent.Single_3 / fdT) == gameTimeData.logicTime.elapsedTime)
                        {
                            gameEvent.OnOnceAct(ref refData, in inData);
                            if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_12 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_13 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_21 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_25 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_31 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_32 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_33 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_34 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_35 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_36 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_37 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_38 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_51 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_52 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_54 ||
                                gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_56)
                            {
                                gameEvent.Boolean_6 = false;
                            }
                        }
                    }


                    ////之后处理碰撞
                    //if (gameEvent.Int32_1 == 4 || gameEvent.Int32_1 == 8)
                    //{
                    //    if (!allTriggers.HasBuffer(elementEntity))
                    //    {
                    //        //障碍物而不是地形
                    //        for (int j = 0; j < targets.Length; j++)
                    //        {
                    //            if (math.distance(allLocDatas[targets[j]].Position,
                    //                    allLocDatas[elementEntity].Position) <= gameEvent.Int32_9)
                    //            {
                    //                //TODO:
                    //                gameEvent.Entity_7 = targets[j];
                    //                gameEvent.OnCollider(ref refData, in inData);
                    //            }
                    //        }

                    //        return;
                    //    }
                    //    else
                    //    {
                    //        var trigger = allTriggers[elementEntity];
                    //        for (int k = 0; k < trigger.Length; k++)
                    //        {
                    //            var otherEntity = trigger[k].GetOtherEntity(elementEntity);
                    //            //TODO:此处排除了所有除了玩家和怪之外的entity，后续可能需要有障碍物的trigger需求
                    //            if (NonTriggerDynamicBodyMask.MatchesIgnoreFilter(otherEntity))
                    //            {
                    //                continue;
                    //            }

                    //            if (!storageInfoFromEntity.Exists(elementEntity) ||
                    //                !storageInfoFromEntity.Exists(otherEntity))
                    //            {
                    //                continue;
                    //            }

                    //            //不处理停留
                    //            if (trigger[k].State == StatefulEventState.Stay) continue;
                    //            gameEvent.Entity_7 = otherEntity;
                    //            if (trigger[k].State == StatefulEventState.Enter)
                    //            {
                    //                gameEvent.OnCharacterEnter(ref refData, in inData);
                    //            }
                    //            else if (trigger[k].State == StatefulEventState.Exit)
                    //            {
                    //                gameEvent.OnCharacterExit(ref refData, in inData);
                    //            }
                    //        }
                    //    }
                    //}


                    gameEvent.Single_4 -= fdT;
                    gameEvent.Int32_5++;
                    gameEvents[i] = gameEvent;
                }

                cdfeEnviromentData[wbe] = refData.cdfeEnviromentData[wbe];
                cdfeGameTimeData[wbe] = refData.cdfeGameTimeData[wbe];
            }

            private void OnTick0(ref GameEvent gameEvent)
            {
                gameEvent.Boolean_11 = true;
                gameEvent.Single_4 = -1;
                gameEvent.Boolean_6 = true;
                int index = 0;
                ref var current = ref globalConfigData.value.Value.configTbevent_0s.Get(gameEvent.Int32_0);

                // for (int i = 0; i < eventsConfig.Length; i++)
                // {
                //     if (eventsConfig[i].id == gameEvent.Int32_0)
                //     {
                //         index = i;
                //         break;
                //     }
                // }
                //
                // ref var current = ref eventsConfig[index];

                gameEvent.CurrentTypeId = (GameEvent.TypeId)current.type;

                if (gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_22 ||
                    gameEvent.CurrentTypeId == GameEvent.TypeId.GameEvent_40)
                {
                    gameEvent.Boolean_11 = false;
                }

                if (current.triggerType == 2)
                {
                    gameEvent.GameEventTriggerType_1 = GameEventTriggerType.IntervalTime;

                    gameEvent.Single_2 = current.triggerPara[0] / 1000f;
                }
                else if (current.triggerType == 0)
                {
                    gameEvent.GameEventTriggerType_1 = GameEventTriggerType.ImmediateEffect;
                }
                else if (current.triggerType == 1)
                {
                    gameEvent.GameEventTriggerType_1 = GameEventTriggerType.AppointedTime;
                    gameEvent.Single_3 = current.triggerPara[0] / 1000f;
                }
                
                gameEvent.int3_7 = new int3(current.para1, current.para2, current.para3);
                gameEvent.int3_8 = new int3(current.para4, current.para5, current.para6);
                gameEvent.int3_14 = new int3(current.para7, 0, 0);
                //gameEvent.int3_14 = new int3(current.para7, 0,0);
                gameEvent.Int32_10 = current.triggerNum;

                //Debug.LogError($"id{gameEvent.Int32_0} type {gameEvent.CurrentTypeId}");
            }
        }
    }
}