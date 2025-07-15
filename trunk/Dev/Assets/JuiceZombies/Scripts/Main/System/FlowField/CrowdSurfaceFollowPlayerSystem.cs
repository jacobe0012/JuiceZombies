// //---------------------------------------------------------------------
// // JiYuStudio
// // Author: 格伦
// // Time: 2023-08-11 10:30:51
// //---------------------------------------------------------------------
//
// using NSprites;
// using ProjectDawn.Navigation;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Transforms;
// using UnityEngine;
//
// namespace Main
// {
//     /// <summary>
//     /// 控制2d渲染包动画切换的系统
//     /// </summary>
//     [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//     [UpdateAfter(typeof(StateMachineSystem))]
//     public partial struct CrowdSurfaceFollowPlayerSystem : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<WorldBlackBoardTag>();
//             state.RequireForUpdate<CrowdSurface>();
//             //state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<StateMachine, AnimatorAspect>().Build());
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             new CrowdSurfaceFollowPlayerJob
//             {
//                 eT = SystemAPI.Time.ElapsedTime,
//                 cdfeLocalToWorld = SystemAPI.GetComponentLookup<LocalToWorld>(true),
//                 player = SystemAPI.GetSingletonEntity<PlayerData>()
//             }.ScheduleParallel();
//         }
//
//         [BurstCompile]
//         private partial struct CrowdSurfaceFollowPlayerJob : IJobEntity
//         {
//             [ReadOnly] public double eT;
//             [ReadOnly] public ComponentLookup<LocalToWorld> cdfeLocalToWorld;
//             [ReadOnly] public Entity player;
//
//             private void Execute(ref LocalTransform tran, in CrowdSurface surface)
//             {
//                 tran.Position = cdfeLocalToWorld[player].Position;
//             }
//         }
//     }
// }

