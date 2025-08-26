//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-25 11:00:25
//---------------------------------------------------------------------

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SpawnEnemySystem))]
    public partial struct TargetDataSystem : ISystem
    {
        private readonly static bool isDebug = false;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<TargetData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);
            var stateQuery = SystemAPI.QueryBuilder().WithAll<State, StateMachine>().WithNone<Prefab>().Build();
            var skillAtackQuery = SystemAPI.QueryBuilder().WithAll<SkillAttackData>().WithNone<Prefab>().Build();
            var mapMoudleQuery = SystemAPI.QueryBuilder().WithAll<MapElementData>().WithNone<BoardData>().Build();
            var player = SystemAPI.GetSingletonEntity<PlayerData>();
            new TargetDataSystemJob
            {
                timeTick = (uint)(SystemAPI.Time.ElapsedTime * 1000f),
                fdT = SystemAPI.Time.fixedDeltaTime,
                eT = (float)SystemAPI.Time.ElapsedTime,
                ecb = ecb,
                player = player,
                wbe = wbe,
                gameRandomData = gameRandomData,
                prefabMapData = prefabMapData,
                gameTimeData = gameTimeData,
                gameSetUpData = gameSetUpData,
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                globalConfigData = globalConfigData,
                cdfeEntityGroupData = SystemAPI.GetComponentLookup<EntityGroupData>(),
                bfeGameEvent = SystemAPI.GetBufferLookup<GameEvent>(),
                bfeLinkedEntityGroup = SystemAPI.GetBufferLookup<LinkedEntityGroup>(true),
                cdfeWeaponData = SystemAPI.GetComponentLookup<WeaponData>(true),
                cdfeRenderingEntityTag = SystemAPI.GetComponentLookup<RenderingEntityTag>(true),
            }.ScheduleParallel();
        }


        [BurstCompile]
        public partial struct TargetDataSystemJob : IJobEntity
        {
            [ReadOnly] public uint timeTick;
            [ReadOnly] public float fdT;
            [ReadOnly] public float eT;

            [NativeDisableParallelForRestriction] public BufferLookup<GameEvent> bfeGameEvent;
            [ReadOnly] public BufferLookup<LinkedEntityGroup> bfeLinkedEntityGroup;
            public EntityCommandBuffer.ParallelWriter ecb;

            [ReadOnly] public Entity player;
            [ReadOnly] public Entity wbe;
            public GameRandomData gameRandomData;
            [ReadOnly] public PrefabMapData prefabMapData;

            [ReadOnly] public GameTimeData gameTimeData;

            [ReadOnly] public GameSetUpData gameSetUpData;

            [ReadOnly] public GlobalConfigData globalConfigData;
            [NativeDisableParallelForRestriction] public ComponentLookup<EntityGroupData> cdfeEntityGroupData;
            [ReadOnly] public ComponentLookup<WeaponData> cdfeWeaponData;
            [ReadOnly] public ComponentLookup<RenderingEntityTag> cdfeRenderingEntityTag;
            public EntityStorageInfoLookup storageInfoFromEntity;

            public void Execute([EntityIndexInQuery] int index, Entity entity, ref TargetData targetData)
            {
                if (targetData.tick == 0)
                {
                    if (cdfeEntityGroupData.HasComponent(entity))
                    {
                        var entityGroup = cdfeEntityGroupData[entity];
                        if (bfeLinkedEntityGroup.HasComponent(entity))
                        {
                            var linked = bfeLinkedEntityGroup[entity];
                            for (int i = 0; i < linked.Length; i++)
                            {
                                if (cdfeWeaponData.HasComponent(linked[i].Value))
                                {
                                    entityGroup.weaponEntity = linked[i].Value;
                                }

                                if (cdfeRenderingEntityTag.HasComponent(linked[i].Value))
                                {
                                    entityGroup.renderingEntity = linked[i].Value;
                                }
                            }


                            cdfeEntityGroupData[entity] = entityGroup;
                        }
                    }
                }

                var buffer = bfeGameEvent[wbe];
                for (int j = 0; j < buffer.Length; j++)
                {
                    var temp = buffer[j];

                    if (temp.CurrentTypeId == GameEvent.TypeId.GameEvent_11 ||
                        temp.CurrentTypeId == GameEvent.TypeId.GameEvent_12)
                    {
                        int target = temp.int3_7.x;
                        int skillId = temp.int3_7.y;


                        if (!PhysicsHelper.IsTargetEnabled((uint)target, targetData.BelongsTo))
                        {
                            continue;
                        }
                        // if ((int)targetData.BelongsTo != target
                        //     
                        //     )
                        // {
                        //     continue;
                        // }


                        switch (temp.CurrentTypeId)
                        {
                            case GameEvent.TypeId.GameEvent_11:
                                if (temp.Int32_5 == 1)
                                {
                                    switch (temp.GameEventTriggerType_1)
                                    {
                                        case GameEventTriggerType.ImmediateEffect:
                                            //TODO:
                                            Debug.Log($"GameEvent_11 skillId11:{skillId} entity{entity.Index}");
                                            ecb.AppendToBuffer(index, entity, new Skill
                                            {
                                                CurrentTypeId = (Skill.TypeId)1,
                                                Int32_0 = skillId,
                                                Entity_5 = entity,
                                                Int32_10 = 1
                                            });
                                            //Debug.Log($"GameEvent_11 skillId:{skillId}");
                                            break;
                                        case GameEventTriggerType.AppointedTime:
                                            // if ((int)(temp.Single_3 / fdT) == gameTimeData.tick)
                                            // {
                                            // }

                                            break;
                                        case GameEventTriggerType.IntervalTime:
                                            if (temp.Int32_5 % (int)(temp.Single_2 / fdT) == 0)
                                            {
                                            }

                                            break;
                                    }
                                }
                                else if (targetData.tick == 0)
                                {
                                    Debug.Log($"GameEvent_11 skillId11:{skillId} entity{entity.Index}");
                                    //TODO:
                                    ecb.AppendToBuffer(index, entity, new Skill
                                    {
                                        CurrentTypeId = (Skill.TypeId)1,
                                        Int32_0 = skillId,
                                        Entity_5 = entity,
                                        Int32_10 = 1
                                    });
                                    //Debug.Log($"GameEvent_11 skillId:{skillId}");

                                    // switch (temp.GameEventTriggerType_1)
                                    // {
                                    //     case GameEventTriggerType.ImmediateEffect:
                                    //
                                    //
                                    //        
                                    //         //}
                                    //
                                    //         break;
                                    //     case GameEventTriggerType.AppointedTime:
                                    //
                                    //         break;
                                    //     case GameEventTriggerType.IntervalTime:
                                    //
                                    //         break;
                                    // }
                                }


                                break;
                            case GameEvent.TypeId.GameEvent_12:
                                if (temp.Int32_5 == 1)
                                {
                                    //temp.Int32_5++;
                                    switch (temp.GameEventTriggerType_1)
                                    {
                                        case GameEventTriggerType.ImmediateEffect:
                                            //TODO:
                                            // for (int j = 0; j < temp.Int32_9; j++)
                                            // {
                                            ecb.AppendToBuffer(index, entity, new Skill
                                            {
                                                CurrentTypeId = (Skill.TypeId)1,
                                                Int32_0 = skillId,
                                                Entity_5 = entity,
                                                Int32_10 = 1
                                            });
                                            //Debug.Log($"GameEvent_12 skillId:{skillId}");
                                            break;
                                        case GameEventTriggerType.AppointedTime:
                                            // if ((int)(temp.Single_3 / fdT) == gameTimeData.tick)
                                            // {
                                            // }

                                            break;
                                        case GameEventTriggerType.IntervalTime:
                                            if (temp.Int32_5 % (int)(temp.Single_2 / fdT) == 0)
                                            {
                                            }

                                            break;
                                    }

                                    buffer.RemoveAt(j);
                                    continue;
                                }

                                break;
                        }
                    }

                    //buffer[j] = temp;
                    //bfeGameEvent[wbe][i] = temp;
                }

                targetData.tick++;
            }
        }
    }
}