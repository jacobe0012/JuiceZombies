// //---------------------------------------------------------------------
// // JiYuStudio
// // Author: 格伦
// // Time: 2023-08-09 11:00:25
// //---------------------------------------------------------------------
//
//
// using Unity.Burst;
// using Unity.Burst.Intrinsics;
// using Unity.Collections;
// using Unity.Entities;
//
// namespace Main
// {
//     [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//     [UpdateBefore(typeof(ManageBuffNewSystem))]
//     public partial struct MergeBuffNewSystem : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<WorldBlackBoardTag>();
//             state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
//             state.RequireForUpdate<Buff>();
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
//             var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
//             var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
//             var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
//             var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
//             var gameTimdData = SystemAPI.GetComponent<GameTimeData>(wbe);
//             var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);
//
//             var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
//             var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
//
//             new MergeBuffJob
//             {
//             }.ScheduleParallel();
//
//             // Entities.WithNativeDisableContainerSafetyRestriction(bufferFromEntity).ForEach(
//             //     (Entity e, int entityInQueryIndex, in MergeBuffData data) =>
//             //     {
//             //         BuffHelper.MergeBuffs(bufferFromEntity[data.entity]);
//             //         BuffHelper.QuickSort(bufferFromEntity[data.entity], 0, bufferFromEntity[data.entity].Length - 1);
//             //
//             //         ecb.DestroyEntity(entityInQueryIndex, e);
//             //     }).ScheduleParallel();
//         }
//
//         [BurstCompile]
//         public partial struct MergeBuffJob : IJobEntity
//         {
//             public void Execute([EntityIndexInQuery] int sortKey, Entity entity, ref DynamicBuffer<Buff> buffs)
//             {
//                 BuffHelper.MergeBuffsNew(ref buffs);
//             }
//         }
//     }
// }