//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2024-05-15 10:30:25
//---------------------------------------------------------------------

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;

namespace Main
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SpawnEnemySystem))]
    public partial struct PushColliderSystem : ISystem
    {
        //private const float normalRestitution = 0.3f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //state.Enabled = false;
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PushColliderData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);

            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var nonTriggerDynamicBodyQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform>()
                .WithAll<PhysicsCollider>()
                .WithNone<StatefulTriggerEvent>().WithNone<SkillAttackData>().Build();
            new PushColliderJob
            {
                ecb = ecb,
                fDT = SystemAPI.Time.fixedDeltaTime,
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                NonTriggerDynamicBodyMask = nonTriggerDynamicBodyQuery.GetEntityQueryMask(),
                cdfeHitBackData = SystemAPI.GetComponentLookup<HitBackData>(true),
                globalConfigData = globalConfigData,
                prefabMapData = prefabMapData
            }.ScheduleParallel();
        }

        [BurstCompile]
        public partial struct PushColliderJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] public float fDT;
            [ReadOnly] public ComponentLookup<HitBackData> cdfeHitBackData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public PrefabMapData prefabMapData;
            public EntityQueryMask NonTriggerDynamicBodyMask;
            public EntityStorageInfoLookup storageInfoFromEntity;

            public void Execute([EntityIndexInQuery] int index, Entity e,
                ref LocalTransform localTransform, ref PushColliderData pushColliderData)
            {
                ref var constant = ref globalConfigData.value.Value.configTbbattle_constants.configTbbattle_constants;

                float battle_module_refresh_time = default;
                float battle_module_force_refresh_time = default;

                for (int i = 0; i < constant.Length; i++)
                {
                    if (constant[i].constantName == (FixedString128Bytes)"battle_module_refresh_time")
                    {
                        battle_module_refresh_time = constant[i].constantValue;
                    }

                    if (constant[i].constantName == (FixedString128Bytes)"battle_module_force_refresh_time")
                    {
                        battle_module_force_refresh_time = constant[i].constantValue;
                    }
                }

                // if (pushColliderData.tick == 0)
                // {
                //     pushColliderData.tick++;
                //     localTransform.Scale = 0.001f;
                //     return;
                // }

                float times = (battle_module_refresh_time / 1000f) / fDT;

                float inter = (pushColliderData.targetScale - pushColliderData.initScale) / times;
                if (!pushColliderData.toBeSmall)
                {
                    if (localTransform.Scale >= pushColliderData.targetScale)
                    {
                        localTransform.Scale = pushColliderData.targetScale;
                        ecb.RemoveComponent<PushColliderData>(index, e);
                        ecb.RemoveComponent<StatefulTriggerEvent>(index, e);
                        return;
                    }
                }
                else
                {
                    if (localTransform.Scale <= pushColliderData.targetScale)
                    {
                        localTransform.Scale = pushColliderData.targetScale;
                        ecb.RemoveComponent<PushColliderData>(index, e);
                        ecb.RemoveComponent<StatefulTriggerEvent>(index, e);
                        return;
                    }
                }


                pushColliderData.tick++;
                localTransform.Scale += inter;
            }
        }
    }
}