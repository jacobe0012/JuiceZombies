//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-17 12:41:01
//---------------------------------------------------------------------

using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct BanBulletRepeatTriggerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<BanBulletTriggerBuffer>();
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new BanBulletRepeatTriggerSystemJob
            {
                fdT = SystemAPI.Time.fixedDeltaTime,
            }.ScheduleParallel();
        }


        [BurstCompile]
        partial struct BanBulletRepeatTriggerSystemJob : IJobEntity
        {
            [ReadOnly] public float fdT;

            public void Execute([EntityIndexInQuery] int sortKey, Entity entity,
                ref DynamicBuffer<BanBulletTriggerBuffer> buffers)
            {
                if (buffers.IsEmpty) return;

                for (var i = 0; i < buffers.Length; i++)
                {
                    var temp = buffers[i];
                    if (temp.duration < 0)
                    {
                        buffers.RemoveAt(i);
                        continue;
                    }

                    temp.duration -= fdT;
                    buffers[i] = temp;
                }
            }
        }
    }
}