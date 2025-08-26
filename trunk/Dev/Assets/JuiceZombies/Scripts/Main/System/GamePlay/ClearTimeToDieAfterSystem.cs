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
    [UpdateAfter(typeof(EndFixedStepSimulationEntityCommandBufferSystem))]
    public partial struct ClearTimeToDieAfterSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //state.Enabled = false;
            state.RequireForUpdate<WorldBlackBoardTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new ClearTimeToDieAfter
            {
                bfeLinkedEntityGroup = SystemAPI.GetBufferLookup<LinkedEntityGroup>(),
                bfeChild = SystemAPI.GetBufferLookup<Child>(),

                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
            }.ScheduleParallel();
        }

        [BurstCompile]
        partial struct ClearTimeToDieAfter : IJobEntity
        {
            [NativeDisableParallelForRestriction] public BufferLookup<LinkedEntityGroup> bfeLinkedEntityGroup;
            [NativeDisableParallelForRestriction] public BufferLookup<Child> bfeChild;

            public EntityStorageInfoLookup storageInfoFromEntity;

            public void Execute([EntityIndexInChunk] int sortKey, Entity e, Parent parent, in SpecialEffectData _)
            {
                if (bfeLinkedEntityGroup.HasComponent(parent.Value))
                {
                    var linked = bfeLinkedEntityGroup[parent.Value];
                    UnityHelper.ClearLinkedEntityGroup(ref linked, ref storageInfoFromEntity);
                }

                if (bfeChild.HasComponent(parent.Value))
                {
                    var linked = bfeChild[parent.Value];
                    UnityHelper.ClearLinkedEntityGroup(ref linked, ref storageInfoFromEntity);
                }
            }
        }
    }
}