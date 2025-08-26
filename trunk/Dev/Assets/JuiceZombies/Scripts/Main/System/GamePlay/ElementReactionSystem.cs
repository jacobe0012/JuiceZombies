//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-09-13 12:00:25
//---------------------------------------------------------------------

using Main;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(SkillAttackSystem))]
public partial struct ElementReactionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.Enabled = false;
        state.RequireForUpdate<WorldBlackBoardTag>();
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<ElementData>();
        //state.RequireForUpdate(SystemAPI.QueryBuilder().WithNone<StaySkillAttackTag>().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

        var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
        var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
        var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
        var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);

        var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var nonTriggerDynamicBodyQuery =
            SystemAPI.QueryBuilder().WithAll<LocalTransform>().WithAll<ElementData>().Build();
        var stateQuery = SystemAPI.QueryBuilder().WithAll<State, StateMachine>().WithNone<Prefab>().Build();
        // var skillAttackQuery = SystemAPI.QueryBuilder().WithAll<SkillAttackData>().Build();

        var player = SystemAPI.GetSingletonEntity<PlayerData>();

        new ElementReactionJob
        {
            ecb = ecb,
            NonTriggerDynamicBodyMask = nonTriggerDynamicBodyQuery.GetEntityQueryMask(),
            fDT = SystemAPI.Time.fixedDeltaTime,
            cdfeLocalTransforms = SystemAPI.GetComponentLookup<LocalTransform>(),
            cdfeMasses = SystemAPI.GetComponentLookup<PhysicsMass>(),
            cdfeVelocities = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
            cdfeAgentBodys = SystemAPI.GetComponentLookup<AgentBody>(),
            globalConfigData = globalConfigData,
            prefabMapData = prefabMapData,
            cdfeIgnoreHitBackData = SystemAPI.GetComponentLookup<IgnoreHitBackData>(true),
            cdfeHitBackData = SystemAPI.GetComponentLookup<HitBackData>(true),
            cdfeElementData = SystemAPI.GetComponentLookup<ElementData>(true),
            cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(true),
            cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
            cdfeSpiritData = SystemAPI.GetComponentLookup<SpiritData>(true),
            cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(true),
            cdfeSkillAttackData = SystemAPI.GetComponentLookup<SkillAttackData>(true),
            cdfeAreaTag = SystemAPI.GetComponentLookup<AreaTag>(true),
            bfeDamageInfo = SystemAPI.GetBufferLookup<DamageInfo>(true),
            cdfePhysicsCollider = SystemAPI.GetComponentLookup<PhysicsCollider>(),
            player = player,
            storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
            allEntities = stateQuery.ToEntityArray(Allocator.TempJob),
            cdfeTrapTag = SystemAPI.GetComponentLookup<TrapTag>(true),
            cdfeObstacleTag = SystemAPI.GetComponentLookup<ObstacleTag>(true),
            cdfeBlackHoleSuction = SystemAPI.GetComponentLookup<BlackHoleSuction>(true),
            randomData = gameRandomData,
            cdfeBuff = SystemAPI.GetBufferLookup<BuffOld>(true),
            bfeBossTag = SystemAPI.GetComponentLookup<BossTag>(true),
            // cdfeQueryData=SystemAPI.GetComponentLookup<QueryData>(true),
            // cdfeRayCastDect=SystemAPI.GetComponentLookup<RayCastDect>(true),
        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct ElementReactionJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public EntityQueryMask NonTriggerDynamicBodyMask;
        public float fDT;

        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsMass> cdfeMasses;
        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsVelocity> cdfeVelocities;
        [NativeDisableParallelForRestriction] public ComponentLookup<AgentBody> cdfeAgentBodys;
        [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransforms;
        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
        [ReadOnly] public ComponentLookup<HitBackData> cdfeHitBackData;
        [ReadOnly] public ComponentLookup<ElementData> cdfeElementData;
        [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
        [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
        [ReadOnly] public ComponentLookup<SpiritData> cdfeSpiritData;
        [ReadOnly] public BufferLookup<DamageInfo> bfeDamageInfo;
        [ReadOnly] public GlobalConfigData globalConfigData;
        [ReadOnly] public PrefabMapData prefabMapData;
        [ReadOnly] public ComponentLookup<IgnoreHitBackData> cdfeIgnoreHitBackData;
        [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;
        [ReadOnly] public ComponentLookup<SkillAttackData> cdfeSkillAttackData;

        [ReadOnly] public ComponentLookup<AreaTag> cdfeAreaTag;
        [ReadOnly] public Entity player;

        [ReadOnly] public ComponentLookup<TrapTag> cdfeTrapTag;
        [ReadOnly] public ComponentLookup<ObstacleTag> cdfeObstacleTag;
        [ReadOnly] public ComponentLookup<BlackHoleSuction> cdfeBlackHoleSuction;

        [ReadOnly] public GameRandomData randomData;
        public EntityStorageInfoLookup storageInfoFromEntity;

        [ReadOnly] public BufferLookup<BuffOld> cdfeBuff;
        [ReadOnly] public ComponentLookup<BossTag> bfeBossTag;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> allEntities;

        // [ReadOnly] public ComponentLookup<QueryData> cdfeQueryData;
        // [ReadOnly] public ComponentLookup<RayCastDect> cdfeRayCastDect;
        public void Execute([EntityIndexInQuery] int index, Entity e,
            ref DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer, in ElementData elementData)
        {
            //技能 地形外return
            if (!cdfeSkillAttackData.HasComponent(e) && !cdfeAreaTag.HasComponent(e))
                return;


            ref var constConfig = ref globalConfigData.value.Value.configTbconstants.configTbconstants;
            // ref var skillEffectConfig =
            //     ref globalConfigData.value.Value.configTbskill_effects.configTbskill_effects;
            // ref var buffEffectsConfig =
            //     ref globalConfigData.value.Value.configTbbuff_effects.configTbbuff_effects;
            ref var elementEffectConfig =
                ref globalConfigData.value.Value.configTbelement_effects.configTbelement_effects;


            //int elementEffectIndex = 0;


            //处理技能实体碰撞和伤害
            for (int i = 0; i < triggerEventBuffer.Length; i++)
            {
                var triggerEvent = triggerEventBuffer[i];
                var otherEntity = triggerEvent.GetOtherEntity(e);
                if (triggerEvent.State != StatefulEventState.Enter)
                {
                    continue;
                }

                // exclude static bodies, other triggers and enter/exit events
                if (!NonTriggerDynamicBodyMask.MatchesIgnoreFilter(otherEntity))
                {
                    continue;
                }

                if (!storageInfoFromEntity.Exists(e) || !storageInfoFromEntity.Exists(otherEntity))
                {
                    continue;
                }

                bool isExist = false;
                Entity caster = default;
                var pos = cdfeLocalTransforms[otherEntity].Position;

                int otherEntityElementId = cdfeElementData[otherEntity].id;
                if (cdfeSkillAttackData.HasComponent(e))
                {
                    if (storageInfoFromEntity.Exists(cdfeSkillAttackData[e].data.Entity_3))
                    {
                        if (cdfeSkillAttackData[e].data.Entity_3 == otherEntity)
                        {
                            continue;
                        }

                        caster = cdfeSkillAttackData[e].data.Entity_3;
                        isExist = true;
                    }
                    else
                    {
                        continue;
                    }
                }

                for (int j = 0; j < elementEffectConfig.Length; j++)
                {
                    if (elementEffectConfig[j].from == elementData.id)
                    {
                        //elementEffectConfig[j].para1
                        ref var elementEffect = ref elementEffectConfig[j].para;
                        switch (elementData.id)
                        {
                            case 103:
                                if (storageInfoFromEntity.Exists(caster))
                                {
                                    switch (otherEntityElementId)
                                    {
                                        case 201:

                                            ecb.AppendToBuffer(index, caster, new Skill_9999103
                                            {
                                                id = 9999103,
                                                level = 0,
                                                cooldown = 0,
                                                duration = 0,
                                                timeScale = 0,
                                                caster = caster,
                                                tick = 0,
                                                curCooldown = 0,
                                                curDuration = 0,
                                                target = default,
                                                totalTick = 0,
                                                isOneShotSkill = true,
                                                isUseCertainPos = true,
                                                pos = pos,
                                                args1 = new int4(elementEffect[0], elementEffect[1], 0, 0)
                                            }.ToSkillOld());
                                            break;
                                        case 301:
                                            ecb.AppendToBuffer(index, caster, new Skill_9999103
                                            {
                                                id = 9999103,
                                                level = 0,
                                                cooldown = 0,
                                                duration = 0,
                                                timeScale = 0,
                                                caster = caster,
                                                tick = 0,
                                                curCooldown = 0,
                                                curDuration = 0,
                                                target = default,
                                                totalTick = 0,
                                                isOneShotSkill = true,
                                                isUseCertainPos = true,
                                                pos = pos,
                                                args1 = new int4(elementEffect[0], elementEffect[1], 0, 0)
                                            }.ToSkillOld());
                                            break;
                                    }
                                }

                                break;
                            case 101:

                                break;
                            case 102:

                                break;
                        }

                        //elementEffectIndex = i;
                        break;
                    }
                }


                //ref var elementEffect = ref elementEffectConfig[elementEffectIndex];
            }
        }
    }
}