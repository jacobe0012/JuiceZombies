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
    public partial struct ManageBuffNewSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<Buff>();
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //var buffQuery = SystemAPI.QueryBuilder().WithAll<Buff>().Build();
            //var configData = SystemAPI.GetSingleton<GlobalConfigData>();

            //Debug.Log($"{configData.value.Value.configTbareas.configTbareas[0].areaEvents[0]}");
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            var gameTimdData = SystemAPI.GetComponent<GameTimeData>(wbe);
            var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);

            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var dropsQuery = SystemAPI.QueryBuilder().WithAll<DropsData>().Build();
            var stateQuery = SystemAPI.QueryBuilder().WithAll<State, StateMachine>().WithNone<Prefab>().Build();
            new BuffNewJob
            {
                cdfeDropsData = SystemAPI.GetComponentLookup<DropsData>(),
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(),
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(),
                cdfeAgentBody = SystemAPI.GetComponentLookup<AgentBody>(),
                cdfeAgentLocomotion = SystemAPI.GetComponentLookup<AgentLocomotion>(),
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(),
                cdfeMapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
                gameRandomData = gameRandomData,
                prefabMapData = prefabMapData,
                gameSetUpData = gameSetUpData,
                globalConfigData = globalConfigData,
                fdT = SystemAPI.Time.fixedDeltaTime,
                eT = (float)SystemAPI.Time.ElapsedTime,
                timeTick = (uint)(SystemAPI.Time.ElapsedTime * 1000f),
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                dropsEntities = dropsQuery.ToEntityArray(Allocator.TempJob),
                player = SystemAPI.GetSingletonEntity<PlayerData>(),
                wbe = wbe,
                ecb = ecb,
                cdfePhysicsCollider = SystemAPI.GetComponentLookup<PhysicsCollider>(),
                cdfePhysicsVolocity = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
                cdfeSpecialEffectData = SystemAPI.GetComponentLookup<SpecialEffectData>(true),
                cdfeHitBackData = SystemAPI.GetComponentLookup<HitBackData>(true),
                cdfeObstacleTag = SystemAPI.GetComponentLookup<ObstacleTag>(true),
                bfeLinkedEntityGroup = SystemAPI.GetBufferLookup<LinkedEntityGroup>(true),
                bfeChild = SystemAPI.GetBufferLookup<Child>(true),
                bfeBackTrackTimeBuffer = SystemAPI.GetBufferLookup<BackTrackTimeBuffer>(),
                allEntities = stateQuery.ToEntityArray(Allocator.TempJob),
                cbfSkill = SystemAPI.GetBufferLookup<Skill>(true),
                cdfePhysicsMass = SystemAPI.GetComponentLookup<PhysicsMass>(),
                cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(),
                cdfeWeaponData = SystemAPI.GetComponentLookup<WeaponData>(true),
                gameTimeData = gameTimdData,
                gameOthersData = gameOthersData,
                enviromentData = SystemAPI.GetComponent<GameEnviromentData>(wbe),
                cdfeStateMachine = SystemAPI.GetComponentLookup<StateMachine>(),
                cdfeEntityGroupData = SystemAPI.GetComponentLookup<EntityGroupData>(true),
                cdfeTargetData = SystemAPI.GetComponentLookup<TargetData>(true),
                cdfeJiYuSort = SystemAPI.GetComponentLookup<JiYuSort>(),
                cdfePostTransformMatrix = SystemAPI.GetComponentLookup<PostTransformMatrix>(true),
            }.ScheduleParallel();
        }


        [BurstCompile]
        partial struct BuffNewJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;

            //public BufferTypeHandle<Buff> bufferTypeHandle;
            [NativeDisableParallelForRestriction] public ComponentLookup<DropsData> cdfeDropsData;
            [NativeDisableParallelForRestriction] public ComponentLookup<ChaStats> cdfeChaStats;
            [NativeDisableParallelForRestriction] public ComponentLookup<PlayerData> cdfePlayerData;
            [NativeDisableParallelForRestriction] public ComponentLookup<AgentBody> cdfeAgentBody;
            [NativeDisableParallelForRestriction] public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;
            [NativeDisableParallelForRestriction] public ComponentLookup<EnemyData> cdfeEnemyData;
            [NativeDisableParallelForRestriction] public ComponentLookup<StateMachine> cdfeStateMachine;
            [ReadOnly] public ComponentLookup<EntityGroupData> cdfeEntityGroupData;

            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransform;

            [ReadOnly] public ComponentLookup<MapElementData> cdfeMapElementData;

            [ReadOnly] public BufferLookup<Skill> cbfSkill;

            //[ReadOnly] public EntityTypeHandle cthEntity;
            [ReadOnly] public PrefabMapData prefabMapData;
            [ReadOnly] public GameSetUpData gameSetUpData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            public GameRandomData gameRandomData;

            [ReadOnly] public float fdT;
            [ReadOnly] public float eT;
            [ReadOnly] public uint timeTick;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> dropsEntities;
            [ReadOnly] public Entity player;
            [ReadOnly] public Entity wbe;
            [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;

            [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsVelocity> cdfePhysicsVolocity;

            //[NativeDisableParallelForRestriction] public ComponentLookup<JiYuSort> cdfeJiYuSort;
            [ReadOnly] public ComponentLookup<SpecialEffectData> cdfeSpecialEffectData;
            [ReadOnly] public ComponentLookup<HitBackData> cdfeHitBackData;
            [ReadOnly] public ComponentLookup<ObstacleTag> cdfeObstacleTag;
            [ReadOnly] public BufferLookup<LinkedEntityGroup> bfeLinkedEntityGroup;
            [ReadOnly] public BufferLookup<Child> bfeChild;
            [NativeDisableParallelForRestriction] public BufferLookup<BackTrackTimeBuffer> bfeBackTrackTimeBuffer;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> allEntities;

            //[NativeDisableParallelForRestriction] public BufferLookup<SkillOld> cdfeSkill;
            [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsMass> cdfePhysicsMass;
            [ReadOnly] public ComponentLookup<WeaponData> cdfeWeaponData;
            [ReadOnly] public GameTimeData gameTimeData;
            [ReadOnly] public GameOthersData gameOthersData;
            [ReadOnly] public GameEnviromentData enviromentData;
            [ReadOnly] public ComponentLookup<TargetData> cdfeTargetData;
            [NativeDisableParallelForRestriction] public ComponentLookup<JiYuSort> cdfeJiYuSort;
            [ReadOnly] public ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix;
            public EntityStorageInfoLookup storageInfoFromEntity;

            public void Execute([EntityIndexInQuery] int index, Entity entity, ref DynamicBuffer<Buff> buffs)
            {
                if (buffs.IsEmpty) return;
                BuffHelper.MergeBuffsNew(ref buffs);
                BuffData_ReadWrite refData = new BuffData_ReadWrite
                {
                    buff = buffs,
                    ecb = ecb,
                    damageInfo = default,
                    cdfeChaStats = cdfeChaStats,
                    cdfePlayerData = cdfePlayerData,
                    skill = default,
                    storageInfoFromEntity = storageInfoFromEntity,
                    cdfeDropsData = cdfeDropsData,
                    defenderChaStats = default,
                    cdfeAgentBody = cdfeAgentBody,
                    cdfeAgentLocomotion = cdfeAgentLocomotion,
                    cdfeSkill = default,
                    //cdfeSkill = cbfSkill,
                    //sumDamage = 0,
                    cdfePhysicsCollider = cdfePhysicsCollider,
                    cdfePhysicsVolocity = cdfePhysicsVolocity,
                    cdfeEnemyData = cdfeEnemyData,
                    cdfeLocalTransform = cdfeLocalTransform,
                    cdfeStateMachine = cdfeStateMachine,
                    cdfePhysicsMass = cdfePhysicsMass,
                    bfeBackTrackTimeBuffer = bfeBackTrackTimeBuffer,
                    cdfeJiYuSort = cdfeJiYuSort
                };
                BuffData_ReadOnly inData = new BuffData_ReadOnly
                {
                    sortkey = 0,
                    entity = entity,
                    player = player,
                    wbe = wbe,
                    prefabMapData = prefabMapData,
                    globalConfigData = globalConfigData,
                    cdfeLocalTransform = cdfeLocalTransform,
                    dropsEntities = dropsEntities,
                    mapModules = default,
                    cdfeMapElementData = cdfeMapElementData,
                    cdfeSpiritData = default,
                    entities = allEntities,
                    cdfeHitBackData = cdfeHitBackData,
                    cdfeObstacleTag = cdfeObstacleTag,
                    fdT = fdT,
                    eT = eT,
                    timeTick = timeTick,
                    gameRandomData = gameRandomData,
                    cdfeSpecialEffectData = cdfeSpecialEffectData,
                    bfeSkill = cbfSkill,
                    bfeChild = bfeChild.HasComponent(entity)
                        ? bfeChild[entity]
                        : default,
                    cdfeWeaponData = cdfeWeaponData,
                    linkedEntityGroup = bfeLinkedEntityGroup.HasComponent(entity)
                        ? bfeLinkedEntityGroup[entity]
                        : default,
                    gameTimeData = gameTimeData,
                    gameOthersData = gameOthersData,
                    enviromentData = enviromentData,
                    cdfeTargetData = cdfeTargetData,
                    cdfeJiYuSort = cdfeJiYuSort,
                    cdfePostTransformMatrix = cdfePostTransformMatrix,
                    cdfeEntityGroupData = cdfeEntityGroupData
                };

                bool isNew = false;
                for (var i = 0; i < buffs.Length; i++)
                {
                    var temp = buffs[i];
                    //temp.CurrentTypeId

                    if (temp.Int32_0 >= 101 && temp.Int32_0 <= 129)
                    {
                        for (int j = i + 1; j < buffs.Length; j++)
                        {
                            if (buffs[j].Int32_0 == temp.Int32_0)
                            {
                                var buff = buffs[j];
                                buff.Single_3 = 0;
                                buffs[j] = buff;
                            }
                        }
                    }

                    if (temp.Single_3 < 0 && !temp.Boolean_4)
                    {
                        temp.OnRemoved(ref refData, in inData);
                        buffs[i] = temp;
                        buffs.RemoveAt(i);
                        continue;
                    }
                    else if (temp.CurrentTypeId == Buff.TypeId.Buff_BeforeBeKilled)
                    {
                        for (int j = i + 1; j < buffs.Length; j++)
                        {
                            if (temp.BuffArgsNew_12.args1.x == buffs[j].BuffArgsNew_12.args1.x &&
                                buffs[j].CurrentTypeId == Buff.TypeId.Buff_BeforeBeKilled)
                            {
                                buffs.RemoveAt(j);
                                continue;
                            }
                        }
                    }
                    else if (temp.CurrentTypeId == Buff.TypeId.Buff_5 && temp.Boolean_4 && temp.Int32_9 <= 0)
                    {
                        temp.OnRemoved(ref refData, in inData);
                        buffs[i] = temp;
                        buffs.RemoveAt(i);
                        continue;
                    }

                    // else
                    // {
                    //     temp.Single_3 -= fdT;
                    // }
                    temp.Single_3 -= fdT;
                    if (temp.Int32_2 == 0)
                    {
                        temp.Single_1 = timeTick;
                        if (temp.CurrentTypeId == Buff.TypeId.Buff_4 ||
                            (temp.CurrentTypeId == Buff.TypeId.Buff_5 && temp.BuffArgsNew_12.args1.x == 4))
                        {
                            isNew = true;
                            temp.Int32_13 += (int)timeTick;
                        }

                        temp.OnOccur(ref refData, in inData);

                        if (temp.Int32_9 != -1 && temp.CurrentTypeId == Buff.TypeId.Buff_5)
                        {
                            temp.Boolean_4 = true;
                        }

                        if (storageInfoFromEntity.Exists(temp.Entity_6) &&
                            cdfeLocalTransform.HasComponent(temp.Entity_6))
                        {
                            temp.float3x2_16.c1 = cdfeLocalTransform[temp.Entity_6].Position;
                        }
                    }

                    temp.OnUpdate(ref refData, in inData);
                    if (temp.Int32_15 > 0)
                    {
                        //TODO:
                        int tick = math.max(1, (int)(temp.Int32_15 / gameTimeData.logicTime.gameTimeScale));
                        if (temp.Int32_2 > 0 && temp.Int32_2 % tick == 0)
                        {
                            temp.OnTick(ref refData, inData);
                        }
                    }

                    if (storageInfoFromEntity.Exists(temp.Entity_6) && cdfeLocalTransform.HasComponent(temp.Entity_6) &&
                        !cdfeObstacleTag.HasComponent(temp.Entity_6))
                    {
                        if (temp.float3x2_16.c0.x > 0 && temp.float3x2_16.c0.z > 0)
                        {
                            temp.float3x2_16.c0.y = math.distance(cdfeLocalTransform[temp.Entity_6].Position,
                                temp.float3x2_16.c1);
                            if (temp.float3x2_16.c0.y > temp.float3x2_16.c0.x)
                            {
                                temp.OnPerUnitMove(ref refData, inData);
                                temp.float3x2_16.c0.y = 0;
                            }

                            temp.float3x2_16.c1 = cdfeLocalTransform[temp.Entity_6].Position;
                        }
                    }

                    temp.Int32_2++;

                    buffs[i] = temp;
                }

                if (isNew)
                {
                    BuffHelper.QuickSortNew(buffs, 0, buffs.Length - 1);
                    for (var i = 0; i < buffs.Length; i++)
                    {
                        var temp = buffs[i];
                        if (temp.CurrentTypeId == Buff.TypeId.Buff_4 ||
                            (temp.CurrentTypeId == Buff.TypeId.Buff_5 && temp.BuffArgsNew_12.args1.x == 4))
                        {
                            temp.OnOccur(ref refData, in inData);
                            buffs[i] = temp;
                            break;
                        }
                    }
                }
            }
        }
    }
}