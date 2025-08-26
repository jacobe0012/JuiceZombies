// //---------------------------------------------------------------------
// // UnicornStudio
// // Author: jaco0012
// // Time: 2025-04-18 10:50:51
// //---------------------------------------------------------------------
//
// //using NSprites;
//
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
//
// namespace Main
// {
//     [UpdateInGroup(typeof(PresentationSystemGroup))]
//     public partial struct DropItemsAnimSystem : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<WorldBlackBoardTag>();
//             state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
//             state.RequireForUpdate<DropsData>();
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             var singleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
//
//             var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
//             var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
//
//             var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
//             var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
//             var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
//             var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
//             var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);
//             var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);
//             new GetHitAnimObstacleSystemJob
//             {
//                 ecb = ecb,
//                 dT = SystemAPI.Time.DeltaTime,
//                 eT = (float)SystemAPI.Time.ElapsedTime,
//                 player = SystemAPI.GetSingletonEntity<PlayerData>(),
//                 cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(true),
//                 entityStorageInfoLookup = SystemAPI.GetEntityStorageInfoLookup(),
//                 cdfeStateMachine = SystemAPI.GetComponentLookup<StateMachine>(true),
//                 cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
//                 cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(true),
//                 cdfeBossTag = SystemAPI.GetComponentLookup<BossTag>(true),
//                 bfeBuff = SystemAPI.GetBufferLookup<Buff>(true),
//                 gameTimeData = gameTimeData,
//                 cdfeJiYuScaleXY = SystemAPI.GetComponentLookup<JiYuScaleXY>(),
//                 storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
//                 cdfeJiYuDissolveThreshold = SystemAPI.GetComponentLookup<JiYuDissolveThreshold>(true),
//                 cdfeJiYuOverlayColor = SystemAPI.GetComponentLookup<JiYuOverlayColor>(),
//                 gameOthersData = gameOthersData,
//                 globalConfigData = globalConfigData,
//             }.ScheduleParallel();
//         }
//
//         [BurstCompile]
//         private partial struct GetHitAnimObstacleSystemJob : IJobEntity
//         {
//             public EntityCommandBuffer.ParallelWriter ecb;
//             [ReadOnly] public float dT;
//             [ReadOnly] public float eT;
//             [ReadOnly] public Entity player;
//             [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;
//             public EntityStorageInfoLookup entityStorageInfoLookup;
//             [ReadOnly] public GameOthersData gameOthersData;
//             [ReadOnly] public GlobalConfigData globalConfigData;
//             [ReadOnly] public ComponentLookup<StateMachine> cdfeStateMachine;
//
//             [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
//             [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
//             [ReadOnly] public ComponentLookup<BossTag> cdfeBossTag;
//             [ReadOnly] public BufferLookup<Buff> bfeBuff;
//             [ReadOnly] public GameTimeData gameTimeData;
//             [NativeDisableParallelForRestriction] public ComponentLookup<JiYuScaleXY> cdfeJiYuScaleXY;
//             [NativeDisableParallelForRestriction] public ComponentLookup<JiYuOverlayColor> cdfeJiYuOverlayColor;
//             public EntityStorageInfoLookup storageInfoFromEntity;
//
//             [ReadOnly] public ComponentLookup<JiYuDissolveThreshold> cdfeJiYuDissolveThreshold;
//             // public const float AnimDuration = 0.4f;
//             // public const float Offset = 0.25f;
//
//             public void Execute([EntityIndexInQuery] int sortKey, Entity e, ref LocalTransform localTransform,
//                 ref DropsData dropsData)
//             {
//                 var AnimDuration = gameOthersData.gameOtherParas.getHitDuration;
//                 var Offset = gameOthersData.gameOtherParas.getHitOffset;
//                 //dropsData.
//
//             }
//         }
//     }
// }