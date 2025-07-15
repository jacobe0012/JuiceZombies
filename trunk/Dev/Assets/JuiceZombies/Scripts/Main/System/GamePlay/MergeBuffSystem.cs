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
//     /// <summary>
//     ///buff之间因替换类型和清楚类型导致的替换
//     ///新增
//     /// </summary>
//     public struct MergeBuffData : IComponentData
//     {
//         public Entity entity;
//     }
//
//     [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//     //[UpdateAfter(typeof(ManageBuffSystem))]
//     public partial struct MergeBuffSystem : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             state.Enabled = false;
//             state.RequireForUpdate<WorldBlackBoardTag>();
//             state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
//             state.RequireForUpdate<MergeBuffData>();
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             var mergebuffQuery = SystemAPI.QueryBuilder().WithAll<MergeBuffData>().Build();
//
//
//             var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
//             var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
//
//             state.Dependency = new MergeBuffJob
//             {
//                 ecb = ecb,
//                 mergeBuffDataTH = SystemAPI.GetComponentTypeHandle<MergeBuffData>(true),
//                 entityTypeHandle = SystemAPI.GetEntityTypeHandle(),
//                 bufferLookup = SystemAPI.GetBufferLookup<BuffOld>()
//             }.ScheduleParallel(mergebuffQuery, state.Dependency);
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
//         public struct MergeBuffJob : IJobChunk
//         {
//             public EntityCommandBuffer.ParallelWriter ecb;
//
//             [ReadOnly] public ComponentTypeHandle<MergeBuffData> mergeBuffDataTH;
//             public EntityTypeHandle entityTypeHandle;
//
//             [NativeDisableParallelForRestriction] public BufferLookup<BuffOld> bufferLookup;
//
//             public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
//                 in v128 chunkEnabledMask)
//             {
//                 var mergeBuffs = chunk.GetNativeArray(mergeBuffDataTH);
//                 var entities = chunk.GetNativeArray(entityTypeHandle);
//                 for (var i = 0; i < chunk.Count; i++)
//                 {
//                     var mergeBuff = mergeBuffs[i];
//                     var e = entities[i];
//                     BuffHelper.MergeBuffs(bufferLookup[mergeBuff.entity]);
//                     BuffHelper.QuickSort(bufferLookup[mergeBuff.entity], 0, bufferLookup[mergeBuff.entity].Length - 1);
//                     ecb.DestroyEntity(unfilteredChunkIndex, e);
//                 }
//             }
//         }
//     }
// }