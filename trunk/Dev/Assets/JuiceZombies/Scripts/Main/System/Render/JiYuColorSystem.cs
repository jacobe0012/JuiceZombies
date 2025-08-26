//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-11 10:30:51
//---------------------------------------------------------------------

using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Main
{

    //[WorldSystemFilter(WorldSystemFilterFlags.Editor, WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    //[UpdateAfter(typeof(StateMachineSystem))]
    public partial struct JiYuColorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
#if !UNITY_EDITOR
            state.RequireForUpdate<WorldBlackBoardTag>();
#endif
            state.RequireForUpdate<JiYuColor>();
            //state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<StateMachine, AnimatorAspect>().Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            new JiYuColorSystemJob
            {
                //gameTimeData = gameTimeData,
                dT = SystemAPI.Time.DeltaTime,
                ecb = ecb,
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct JiYuColorSystemJob : IJobEntity
        {
            const float JiYuColorMaxDuration = 1f;
            const float MinAlpha = 0.5f;
            [ReadOnly] public float dT;

            //[ReadOnly] public GameTimeData gameTimeData;
            public EntityCommandBuffer.ParallelWriter ecb;

            private void Execute([EntityIndexInQuery] int sortKey, Entity e, ref JiYuColor color,
                ref JiYuColorCommand colorCommand)
            {
                
                switch (colorCommand.type)
                {
                    case 0:
                        if (color.value.w <= 0.5f)
                        {
                            return;
                        }

                        break;
                    case 1:
                        if (color.value.w >= 1f)
                        {
                            return;
                        }

                        break;
                }


                var offset = 1 - MinAlpha;
                var speed = offset / dT;
                speed /= 3000f;
                float alpha = 0;
                switch (colorCommand.type)
                {
                    case 0:

                        color.value.w -= speed;
                        alpha = math.remap(0, JiYuColorMaxDuration, MinAlpha, 1, colorCommand.curDuration);

                        break;
                    case 1:
                        color.value.w += speed;
                        alpha = math.remap(0, JiYuColorMaxDuration, MinAlpha, 1,
                            JiYuColorMaxDuration - colorCommand.curDuration);

                        break;
                }

                color.value.w = math.clamp(color.value.w, MinAlpha, 1);
                //color.value.w = alpha;
                colorCommand.curDuration -= dT;
            }
        }
    }
}