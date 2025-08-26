//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-09-05 12:00:25
//---------------------------------------------------------------------


using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
    public partial struct TimeToDieSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<TimeToDieData>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var singleton = SystemAPI
                .GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();


            new TimeToDieJob
            {
                ecb = ecb,
                bfeLinkedEntityGroup = SystemAPI.GetBufferLookup<LinkedEntityGroup>(),
                bfeChild = SystemAPI.GetBufferLookup<Child>(),
                cdfeParent = SystemAPI.GetComponentLookup<Parent>(true),
                cdfeTransformHybridUpdateData = SystemAPI.GetComponentLookup<TransformHybridUpdateData>(true),
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                fdT = SystemAPI.Time.fixedDeltaTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        partial struct TimeToDieJob : IJobEntity
        {
            // [ReadOnly] public float fDT;
            // public EntityCommandBuffer.ParallelWriter ecb;
            public const float dissolveTime = 2f;
            public float fdT;
            public EntityCommandBuffer.ParallelWriter ecb;
            [NativeDisableParallelForRestriction] public BufferLookup<LinkedEntityGroup> bfeLinkedEntityGroup;
            [NativeDisableParallelForRestriction] public BufferLookup<Child> bfeChild;
            [ReadOnly] public ComponentLookup<Parent> cdfeParent;
            [ReadOnly] public ComponentLookup<TransformHybridUpdateData> cdfeTransformHybridUpdateData;
            public EntityStorageInfoLookup storageInfoFromEntity;

            public void Execute([EntityIndexInChunk] int sortKey, Entity e, ref TimeToDieData timeData)
            {
                timeData.duration -= fdT;

                if (timeData.duration < 0)
                {
                    ecb.DestroyEntity(sortKey, e);
                    if (cdfeTransformHybridUpdateData.HasComponent(e) && cdfeTransformHybridUpdateData[e].go.IsValid())
                    {
                        var newentity1 = ecb.CreateEntity(sortKey);
                        ecb.AddComponent(sortKey, newentity1, new HybridEventData
                        {
                            type = 13,
                            go = cdfeTransformHybridUpdateData[e].go
                        });
                    }
                }
            }
        }
    }
}