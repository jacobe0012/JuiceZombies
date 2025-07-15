// //---------------------------------------------------------------------
// // JiYuStudio
// // Author: 迅捷蟹
// // Time: 2023-08-30 14:45:10
// //---------------------------------------------------------------------
//
// using Main;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using UnityEngine;
//
// /// <summary>
// /// 这个系统只执行一次,而且可以把其中的部分内容转到Baker里实现
// /// </summary>
// namespace Flow
// {
//     [UpdateBefore(typeof(InitializeFlowFieldGridSystem))]
//     public partial struct EntitySpawner : ISystem, ISystemStartStop
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             //在挂载怪物信息才更新系统   
//             state.RequireForUpdate<EnemyData>();
//
//             //state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             //按下按钮更新速度
//             if (Input.GetKeyDown(KeyCode.Space))
//             {
//                 state.Enabled = true;
//             }
//
//             //var singleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
//
//             //var _commandBuffer = singleton.CreateCommandBuffer(state.EntityManager.WorldUnmanaged);
//             var _commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
//             EntityCommandBuffer.ParallelWriter _ecbParallel = _commandBuffer.AsParallelWriter();
//             var entitySpawnerJob = new EntitySpawnerJob
//             {
//                 ecbParallel = _ecbParallel,
//             };
//
//             //ecb回放
//             state.Dependency = entitySpawnerJob.ScheduleParallel(state.Dependency);
//             state.Dependency.Complete();
//             _commandBuffer.Playback(state.EntityManager);
//             _commandBuffer.Dispose();
//
//
//             state.Enabled = false;
//         }
//
//         [BurstCompile]
//         public void OnStartRunning(ref SystemState state)
//         {
//         }
//
//         [BurstCompile]
//         public void OnStopRunning(ref SystemState state)
//         {
//         }
//     }
//
//     [BurstCompile]
//     public partial struct EntitySpawnerJob : IJobEntity
//     {
//         public EntityCommandBuffer.ParallelWriter ecbParallel;
//
//         public void Execute([EntityIndexInQuery] int entityIndex, Entity entity, in EnemyData enemyData,
//             in ChaStats enemyChaStatsData)
//         {
//             EntityMovementData newEntityMovementData = new EntityMovementData
//             {
//                 //这里给赋值怪物的速度
//                 moveSpeed = enemyChaStatsData.chaResource.curMoveSpeed + enemyData.enemyID - enemyData.enemyID + 3,
//                 destinationReached = false,
//                 destinationMoveSpeed = 0,
//             };
//             //移除上一次的怪物寻路速度组件
//             ecbParallel.RemoveComponent<EntityMovementData>(entityIndex, entity);
//
//             //添加当前的怪物寻路速度组件
//             ecbParallel.AddComponent(entityIndex, entity, newEntityMovementData);
//         }
//     }
// }

