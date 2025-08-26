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
    /// <summary>
    /// 控制动画切换的系统
    /// </summary>
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(StateMachineSystem))]
    public partial struct AnimationControlSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            //state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<StateMachine, AnimatorAspect>().Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);


            new ChangeAnimationJob
            {
                gameTimeData = gameTimeData,
                eT = SystemAPI.Time.ElapsedTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct ChangeAnimationJob : IJobEntity
        {
            [ReadOnly] public double eT;
            [ReadOnly] public GameTimeData gameTimeData;

            private void Execute(ref GpuEcsAnimatorControlComponent gpuEcsAnimatorControlComponent,
                ref StateMachine stateMachine
                , ref GpuEcsAnimatorStateComponent gpuEcsAnimatorStateComponent)
            {
                // var curInt = (int)stateMachine.curAnim;
                // var lastInt = (int)stateMachine.lastAnim;
                // var highInt = (int)AnimationEnum.Die;
                //
                // if (curInt == lastInt)
                // {
                //     if (curInt >= highInt)
                //     {
                //         // if (gpuEcsAnimatorStateComponent.currentNormalizedTime > 0.98f)
                //         // {
                //         //     gpuEcsAnimatorStateComponent.currentNormalizedTime = 0;
                //         // }
                //     }
                // }
                // else
                // {
                //     if (curInt >= highInt)
                //     {
                //         //立即切
                //     }
                //     else
                //     {
                //         if (lastInt >= highInt)
                //         {
                //             if (gpuEcsAnimatorStateComponent.currentNormalizedTime <= 0.98f)
                //             {
                //                 return;
                //             }
                //             else
                //             {
                //                 stateMachine.lastAnim = stateMachine.curAnim;
                //                 stateMachine.curAnim = 0;
                //                 gpuEcsAnimatorControlComponent.animatorInfo.speedFactor *=
                //                     stateMachine.animSpeedScale;
                //                 gpuEcsAnimatorControlComponent.animatorInfo.animationID = (int)stateMachine.curAnim;
                //                 return;
                //             }
                //         }
                //         else
                //         {
                //             //立即切
                //         }
                //     }
                // }

                if (stateMachine.curAnim == AnimationEnum.Die || stateMachine.curAnim == AnimationEnum.Die_lm)
                {
                    gpuEcsAnimatorStateComponent.currentNormalizedTime =
                        math.min(gpuEcsAnimatorStateComponent.currentNormalizedTime, 0.92f);
                }

                stateMachine.lastAnim = stateMachine.curAnim;
                gpuEcsAnimatorControlComponent.animatorInfo.speedFactor = stateMachine.animSpeedScale *
                                                                          gameTimeData.logicTime.gameTimeScale;

                gpuEcsAnimatorControlComponent.animatorInfo.animationID = (int)stateMachine.curAnim;
            }
        }
    }
}