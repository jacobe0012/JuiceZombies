//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-09-05 12:00:25
//---------------------------------------------------------------------


using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
    //[UpdateBefore(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
    public partial struct RandomSeedSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<GameRandomData>();
            //state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var singleton = SystemAPI
                .GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            //var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
            var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);
            new RandomSeedJob
            {
                //ecb = ecb,
                // bfeLinkedEntityGroup = SystemAPI.GetBufferLookup<LinkedEntityGroup>(),
                // bfeChild = SystemAPI.GetBufferLookup<Child>(),
                // cdfeParent = SystemAPI.GetComponentLookup<Parent>(true),
                // storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                gameTimeData = gameTimeData,
                //fdT = SystemAPI.Time.fixedDeltaTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        partial struct RandomSeedJob : IJobEntity
        {
            // [ReadOnly] public float fDT;
            // public EntityCommandBuffer.ParallelWriter ecb;
            // public const float dissolveTime = 2f;
            // public float fdT;
            // public EntityCommandBuffer.ParallelWriter ecb;
            // [NativeDisableParallelForRestriction] public BufferLookup<LinkedEntityGroup> bfeLinkedEntityGroup;
            // [NativeDisableParallelForRestriction] public BufferLookup<Child> bfeChild;
            // [ReadOnly] public ComponentLookup<Parent> cdfeParent;
            // public EntityStorageInfoLookup storageInfoFromEntity;
            [ReadOnly] public GameTimeData gameTimeData;

            public void Execute([EntityIndexInChunk] int sortKey, Entity e, ref GameRandomData gameRandomData)
            {
                gameRandomData.seed += (uint)gameTimeData.unScaledTime.tick;
            }
        }
    }
}