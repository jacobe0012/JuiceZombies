using Main;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;
using UnityEngine;

namespace HotFix_UI
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct FadeObstacleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ObstacleFadeData>();
            state.RequireForUpdate<PlayerData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
            // var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            // var gameOtherData = SystemAPI.GetComponent<GameOthersData>(wbe);
            // var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            // var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            // var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);
            // var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            // var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            // // var nonTriggerDynamicBodyQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform>()
            // //     .WithNone<StatefulTriggerEvent>().WithNone<SkillAttackData>().WithNone<BulletTempTag>().Build();
            // var stateQuery = SystemAPI.QueryBuilder().WithAll<State, StateMachine>().WithNone<Prefab>().Build();
            // var targetQuery = SystemAPI.QueryBuilder().WithAll<TargetData>().WithNone<Prefab>().Build();
            // var skillAttackQuery = SystemAPI.QueryBuilder().WithAll<SkillAttackData>().Build();
            // var player = SystemAPI.GetSingletonEntity<PlayerData>();
            var singleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var nonTriggerDynamicBodyQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform>()
                .WithAll<PlayerData>().Build();
            new FadeObstacelSystemJob
            {
                NonTriggerDynamicBodyMask = nonTriggerDynamicBodyQuery.GetEntityQueryMask(),
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(),
                cdfeJiYuColor = SystemAPI.GetComponentLookup<JiYuColor>(true),
                cdfeEntityGroupData = SystemAPI.GetComponentLookup<EntityGroupData>(true),
                ecb = ecb,
            }.ScheduleParallel();
        }

        [BurstCompile]
        partial struct FadeObstacelSystemJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;

            //20621
            public EntityQueryMask NonTriggerDynamicBodyMask;
            public EntityStorageInfoLookup storageInfoFromEntity;
            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransform;
            [ReadOnly] public ComponentLookup<JiYuColor> cdfeJiYuColor;
            [ReadOnly] public ComponentLookup<EntityGroupData> cdfeEntityGroupData;

            public void Execute([EntityIndexInQuery] int sortKey, Entity entity,
                ref DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer,
                in ObstacleFadeData fadeData)
            {
                if (storageInfoFromEntity.Exists(fadeData.Parent))
                {
                    var parentTran = cdfeLocalTransform[fadeData.Parent];
                    var tran = cdfeLocalTransform[entity];

                    tran = parentTran;
                    cdfeLocalTransform[entity] = tran;
                }


                for (int i = 0; i < triggerEventBuffer.Length; i++)
                {
                    var triggerEvent = triggerEventBuffer[i];
                    var otherEntity = triggerEvent.GetOtherEntity(entity);

                    if (!NonTriggerDynamicBodyMask.MatchesIgnoreFilter(otherEntity))
                    {
                        continue;
                    }


                    if (!storageInfoFromEntity.Exists(entity) || !storageInfoFromEntity.Exists(otherEntity))
                    {
                        continue;
                    }

                    if (triggerEvent.State == StatefulEventState.Enter)
                    {
                        Debug.Log($"FadeEnter");
                        if (cdfeEntityGroupData.HasComponent(fadeData.Parent))
                        {
                            var rendering = cdfeEntityGroupData[fadeData.Parent].renderingEntity;
                            if (cdfeJiYuColor.HasComponent(rendering))
                            {
                                //var color = cdfeJiYuColor[rendering];
                                ecb.AddComponent(sortKey, rendering, new JiYuColorCommand
                                {
                                    type = 0,
                                    curDuration = 1
                                });
                            }
                        }
                    }
                    else if (triggerEvent.State == StatefulEventState.Exit)
                    {
                        Debug.Log($"FadeExit");
                        if (cdfeEntityGroupData.HasComponent(fadeData.Parent))
                        {
                            var rendering = cdfeEntityGroupData[fadeData.Parent].renderingEntity;
                            if (cdfeJiYuColor.HasComponent(rendering))
                            {
                                //var color = cdfeJiYuColor[rendering];
                                ecb.AddComponent(sortKey, rendering, new JiYuColorCommand
                                {
                                    type = 1,
                                    curDuration = 1
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}