//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-11 10:30:51
//---------------------------------------------------------------------

using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Main
{

    //[WorldSystemFilter(WorldSystemFilterFlags.Editor, WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    //[UpdateAfter(typeof(StateMachineSystem))]
    public partial struct JiYuStartTimeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
#if !UNITY_EDITOR
            state.RequireForUpdate<WorldBlackBoardTag>();
#endif
            state.RequireForUpdate<JiYuStartTime>();
            //state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<StateMachine, AnimatorAspect>().Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new SpecialEffectTimeJob
            {
                //gameTimeData = gameTimeData,
                dT = SystemAPI.Time.DeltaTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct SpecialEffectTimeJob : IJobEntity
        {
            [ReadOnly] public float dT;
            //[ReadOnly] public GameTimeData gameTimeData;

            private void Execute(ref JiYuStartTime time)
            {
                time.value += dT;
            }
        }
    }
}