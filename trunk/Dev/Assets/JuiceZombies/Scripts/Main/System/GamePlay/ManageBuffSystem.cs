// //---------------------------------------------------------------------
// // JiYuStudio
// // Author: 格伦
// // Time: 2023-07-17 12:41:01
// //---------------------------------------------------------------------
//
// using ProjectDawn.Navigation;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Physics;
// using Unity.Transforms;
// using UnityEngine;
//
// namespace Main
// {
//     //管理Buff生命周期的系统
//     [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//     public partial struct ManageBuffSystem : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//             state.Enabled = false;
//             state.RequireForUpdate<WorldBlackBoardTag>();
//             state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
//             state.RequireForUpdate<BuffOld>();
//         }
//
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             //var buffQuery = SystemAPI.QueryBuilder().WithAll<Buff>().Build();
//             //var configData = SystemAPI.GetSingleton<GlobalConfigData>();
//
//             //Debug.Log($"{configData.value.Value.configTbareas.configTbareas[0].areaEvents[0]}");
//             var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
//
//             var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
//             var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
//             var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
//             var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
//
//             var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
//
//             var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
//             var dropsQuery = SystemAPI.QueryBuilder().WithAll<DropsData>().Build();
//             var stateQuery = SystemAPI.QueryBuilder().WithAll<State, StateMachine>().WithNone<Prefab>().Build();
//             new BuffHandleJob
//             {
//                 cdfeDropsData = SystemAPI.GetComponentLookup<DropsData>(),
//                 cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(),
//                 cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(),
//                 cdfeAgentBody = SystemAPI.GetComponentLookup<AgentBody>(),
//                 cdfeAgentLocomotion = SystemAPI.GetComponentLookup<AgentLocomotion>(),
//                 cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(true),
//                 cdfeMapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
//                 gameRandomData = gameRandomData,
//                 prefabMapData = prefabMapData,
//                 gameSetUpData = gameSetUpData,
//                 globalConfigData = globalConfigData,
//                 fdT = SystemAPI.Time.fixedDeltaTime,
//                 storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
//                 dropsEntities = dropsQuery.ToEntityArray(Allocator.TempJob),
//                 player = SystemAPI.GetSingletonEntity<PlayerData>(),
//                 wbe = wbe,
//                 ecb = ecb,
//                 cdfePhysicsCollider = SystemAPI.GetComponentLookup<PhysicsCollider>(),
//                 cdfePhysicsVolocity = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
//                 allEntities = stateQuery.ToEntityArray(Allocator.TempJob),
//                 cdfeSkill = SystemAPI.GetBufferLookup<SkillOld>(),
//                 cdfePhysicsMass = SystemAPI.GetComponentLookup<PhysicsMass>(true)
//             }.ScheduleParallel();
//         }
//
//
//         [BurstCompile]
//         partial struct BuffHandleJob : IJobEntity
//         {
//             public EntityCommandBuffer.ParallelWriter ecb;
//
//             //public BufferTypeHandle<Buff> bufferTypeHandle;
//             [NativeDisableParallelForRestriction] public ComponentLookup<DropsData> cdfeDropsData;
//             [NativeDisableParallelForRestriction] public ComponentLookup<ChaStats> cdfeChaStats;
//             [NativeDisableParallelForRestriction] public ComponentLookup<PlayerData> cdfePlayerData;
//             [NativeDisableParallelForRestriction] public ComponentLookup<AgentBody> cdfeAgentBody;
//             [NativeDisableParallelForRestriction] public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;
//
//             [ReadOnly] public ComponentLookup<LocalTransform> cdfeLocalTransform;
//
//             [ReadOnly] public ComponentLookup<MapElementData> cdfeMapElementData;
//
//             //[ReadOnly] public EntityTypeHandle cthEntity;
//             [ReadOnly] public PrefabMapData prefabMapData;
//             [ReadOnly] public GameSetUpData gameSetUpData;
//             [ReadOnly] public GlobalConfigData globalConfigData;
//             public GameRandomData gameRandomData;
//             [ReadOnly] public float fdT;
//
//             [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> dropsEntities;
//             [ReadOnly] public Entity player;
//             [ReadOnly] public Entity wbe;
//             [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
//             [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsVelocity> cdfePhysicsVolocity;
//             [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> allEntities;
//             [NativeDisableParallelForRestriction] public BufferLookup<SkillOld> cdfeSkill;
//             [ReadOnly] public ComponentLookup<PhysicsMass> cdfePhysicsMass;
//             public EntityStorageInfoLookup storageInfoFromEntity;
//
//             public void Execute([EntityIndexInQuery] int index, Entity entity, ref DynamicBuffer<BuffOld> buffs)
//             {
//                 if (buffs.IsEmpty) return;
//                 BuffOldData_ReadWrite refData = new BuffOldData_ReadWrite
//                 {
//                     buff = buffs,
//                     ecb = ecb,
//                     damageInfo = default,
//                     cdfeChaStats = cdfeChaStats,
//                     cdfePlayerData = default,
//                     skill = default,
//                     storageInfoFromEntity = storageInfoFromEntity,
//                     cdfeDropsData = cdfeDropsData,
//                     defenderChaStats = default,
//                     cdfeAgentBody = cdfeAgentBody,
//                     cdfeAgentLocomotion = cdfeAgentLocomotion,
//                     cdfeSkill = cdfeSkill,
//                     subDamage = 0,
//                     cdfePhysicsCollider = cdfePhysicsCollider,
//                     cdfePhysicsVolocity = cdfePhysicsVolocity
//                 };
//                 BuffOldData_ReadOnly inData = new BuffOldData_ReadOnly
//                 {
//                     sortkey = 0,
//                     entity = entity,
//                     player = player,
//                     wbe = wbe,
//                     prefabMapData = prefabMapData,
//                     globalConfigData = globalConfigData,
//                     cdfeLocalTransform = cdfeLocalTransform,
//                     dropsEntities = dropsEntities,
//                     cdfeMapElementData = cdfeMapElementData,
//                     cdfeEnemyData = default,
//                     cdfeSpiritData = default,
//                     entities = allEntities,
//                     cdfePhysicsMass = cdfePhysicsMass,
//                     fdT = fdT
//                 };
//                 for (var i = 0; i < buffs.Length; i++)
//                 {
//                     var temp = buffs[i];
//
//
//                     if (!temp.Boolean_8)
//                     {
//                         if (temp.Single_7 < 0)
//                         {
//                             temp.OnRemoved(ref refData, in inData);
//                             buffs[i] = temp;
//                             buffs.RemoveAt(i);
//                             continue;
//                         }
//                         else
//                         {
//                             temp.Single_7 -= fdT;
//                         }
//                     }
//
//                     if (temp.Int32_5 == 0)
//                     {
//                         temp.OnOccur(ref refData, in inData);
//
//                         if (storageInfoFromEntity.Exists(temp.Entity_10) &&
//                             cdfeLocalTransform.HasComponent(temp.Entity_10))
//                         {
//                             if (temp.Int32_15 != 0)
//                             {
//                                 temp.float3_16 = cdfeLocalTransform[temp.Entity_10].Position;
//                             }
//                         }
//                     }
//
//                     if (temp.Boolean_11 && temp.BuffStack_12.stack != temp.BuffStack_12.laststack)
//                     {
//                         temp.OnOccur(ref refData, in inData);
//                         temp.BuffStack_12.laststack = temp.BuffStack_12.stack;
//                     }
//
//                     if (temp.Single_4 > 0 && temp.Int32_5 > 0 &&
//                         temp.Int32_5 % (uint)(temp.Single_4 / fdT) == 0)
//                     {
//                         temp.Int32_6++;
//                         temp.OnTick(ref refData, in inData);
//                     }
//
//                     if (storageInfoFromEntity.Exists(temp.Entity_10) && cdfeLocalTransform.HasComponent(temp.Entity_10))
//                     {
//                         if (temp.Int32_15 != 0)
//                         {
//                             temp.Single_14 +=
//                                 math.distance(cdfeLocalTransform[temp.Entity_10].Position, temp.float3_16);
//                             if (temp.Single_14 >= temp.Int32_15)
//                             {
//                                 temp.OnPerUnitMove(ref refData, in inData);
//                                 temp.Single_14 = 0;
//                             }
//                             // if ((int)temp.Single_14 > 0 && (int)temp.Single_14 % temp.Int32_15 == 0)
//                             // {
//                             //     temp.OnPerUnitMove(ref refData, in inData);
//                             // }
//
//                             temp.float3_16 = cdfeLocalTransform[temp.Entity_10].Position;
//                         }
//                     }
//                     else
//                     {
//                         Debug.LogError($"{temp.Entity_10} no exists or has no Tran Component");
//                     }
//
//                     temp.Int32_5++;
//
//                     buffs[i] = temp;
//                 }
//             }
//         }
//     }
// }