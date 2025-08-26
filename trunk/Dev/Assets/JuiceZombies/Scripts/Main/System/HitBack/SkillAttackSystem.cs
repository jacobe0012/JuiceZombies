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
using UnityEngine;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(StateMachineSystem))]
public partial struct SkillAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WorldBlackBoardTag>();
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<SkillAttackData>();
        //state.RequireForUpdate(SystemAPI.QueryBuilder().WithNone<StaySkillAttackTag>().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

        var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
        var gameOtherData = SystemAPI.GetComponent<GameOthersData>(wbe);
        var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
        var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
        var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);
        var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        // var nonTriggerDynamicBodyQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform>()
        //     .WithNone<StatefulTriggerEvent>().WithNone<SkillAttackData>().WithNone<BulletTempTag>().Build();
        var nonTriggerDynamicBodyQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform>()
            .WithNone<SkillTag>().WithNone<BulletTempTag>().Build();

        var stateQuery = SystemAPI.QueryBuilder().WithAll<State, StateMachine>().WithNone<Prefab>().Build();
        var targetQuery = SystemAPI.QueryBuilder().WithAll<TargetData>().WithNone<Prefab>().Build();

        var skillAttackQuery = SystemAPI.QueryBuilder().WithAll<SkillAttackData>().Build();

        var player = SystemAPI.GetSingletonEntity<PlayerData>();

        new SkillHitBackSystemJob
        {
            timeTick = (uint)(SystemAPI.Time.ElapsedTime * 1000f),
            ecb = ecb,
            NonTriggerDynamicBodyMask = nonTriggerDynamicBodyQuery.GetEntityQueryMask(),
            fDT = SystemAPI.Time.fixedDeltaTime,
            eT = (float)SystemAPI.Time.ElapsedTime,
            cdfeLocalTransforms = SystemAPI.GetComponentLookup<LocalTransform>(),
            cdfePhysicsMass = SystemAPI.GetComponentLookup<PhysicsMass>(),
            cdfeVelocities = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
            cdfeAgentBodys = SystemAPI.GetComponentLookup<AgentBody>(),
            globalConfigData = globalConfigData,
            prefabMapData = prefabMapData,
            gameTimeData = gameTimeData,
            cdfeIgnoreHitBackData = SystemAPI.GetComponentLookup<IgnoreHitBackData>(true),
            cdfeHitBackData = SystemAPI.GetComponentLookup<HitBackData>(true),
            cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(true),
            cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
            cdfeSpiritData = SystemAPI.GetComponentLookup<SpiritData>(true),
            cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(true),
            bfeDamageInfo = SystemAPI.GetBufferLookup<DamageInfo>(true),
            cdfePhysicsCollider = SystemAPI.GetComponentLookup<PhysicsCollider>(),
            player = player,
            wbe = wbe,
            storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
            bfeBanBulletTriggerBuffer = SystemAPI.GetBufferLookup<BanBulletTriggerBuffer>(true),
            allEntities = stateQuery.ToEntityArray(Allocator.TempJob),
            targetEntities = targetQuery.ToEntityArray(Allocator.TempJob),
            cdfeBulletTempTag = SystemAPI.GetComponentLookup<BulletTempTag>(true),
            cdfeBulletSonData = SystemAPI.GetComponentLookup<BulletSonData>(),
            cdfeTrapTag = SystemAPI.GetComponentLookup<TrapTag>(true),
            cdfeObstacleTag = SystemAPI.GetComponentLookup<ObstacleTag>(true),
            cdfeAreaTag = SystemAPI.GetComponentLookup<AreaTag>(true),
            cdfeBulletTag = SystemAPI.GetComponentLookup<BulletData>(true),
            skillAttackDatas = skillAttackQuery.ToComponentDataArray<SkillAttackData>(Allocator.TempJob),
            seed = gameRandomData.seed,
            bfeSkill = SystemAPI.GetBufferLookup<Skill>(),
            bfeBuff = SystemAPI.GetBufferLookup<Buff>(),
            cdfeBossTag = SystemAPI.GetComponentLookup<BossTag>(true),
            cdfeElementData = SystemAPI.GetComponentLookup<ElementData>(),
            cdfemapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
            cdfeThroneTag = SystemAPI.GetComponentLookup<ThronTag>(true),
            cdfeBoardData = SystemAPI.GetComponentLookup<BoardData>(true),
            cdfeTargetData = SystemAPI.GetComponentLookup<TargetData>(),
            cdfeAreaTempHp = SystemAPI.GetComponentLookup<AreaTempHp>(),
            cdfeWeaponData = SystemAPI.GetComponentLookup<WeaponData>(true),
            cdfeTriggerBufferLookup = SystemAPI.GetBufferLookup<Trigger>(true),
            gameOtherData = gameOtherData,
            cdfeAgentLocomotion = SystemAPI.GetComponentLookup<AgentLocomotion>(),
            cdfePostTransformMatrix = SystemAPI.GetComponentLookup<PostTransformMatrix>(true),
            cdfeJiYuColor = SystemAPI.GetComponentLookup<JiYuColor>(),
            // cdfeQueryData=SystemAPI.GetComponentLookup<QueryData>(true),
            // cdfeRayCastDect=SystemAPI.GetComponentLookup<RayCastDect>(true),
        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct SkillHitBackSystemJob : IJobEntity
    {
        private const float OnEnterCoolDown = 0.5f;
        [ReadOnly] public GameTimeData gameTimeData;
        [ReadOnly] public GameOthersData gameOtherData;
        [ReadOnly] public uint timeTick;
        [ReadOnly] public uint seed;
        public EntityCommandBuffer.ParallelWriter ecb;
        public EntityQueryMask NonTriggerDynamicBodyMask;
        public float fDT;
        public float eT;
        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsMass> cdfePhysicsMass;
        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsVelocity> cdfeVelocities;
        [NativeDisableParallelForRestriction] public ComponentLookup<AgentBody> cdfeAgentBodys;
        [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransforms;
        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
        [NativeDisableParallelForRestriction] public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;
        [NativeDisableParallelForRestriction] public ComponentLookup<ElementData> cdfeElementData;
        [NativeDisableParallelForRestriction] public ComponentLookup<TargetData> cdfeTargetData;
        [NativeDisableParallelForRestriction] public ComponentLookup<BulletSonData> cdfeBulletSonData;

        [ReadOnly] public ComponentLookup<HitBackData> cdfeHitBackData;
        [ReadOnly] public BufferLookup<Trigger> cdfeTriggerBufferLookup;
        [ReadOnly] public ComponentLookup<WeaponData> cdfeWeaponData;
        [ReadOnly] public ComponentLookup<MapElementData> cdfemapElementData;
        [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
        [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
        [ReadOnly] public ComponentLookup<SpiritData> cdfeSpiritData;
        [ReadOnly] public BufferLookup<DamageInfo> bfeDamageInfo;
        [ReadOnly] public GlobalConfigData globalConfigData;
        [ReadOnly] public PrefabMapData prefabMapData;
        [ReadOnly] public ComponentLookup<IgnoreHitBackData> cdfeIgnoreHitBackData;
        [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;
        [ReadOnly] public Entity player;
        [ReadOnly] public Entity wbe;
        [ReadOnly] public ComponentLookup<TrapTag> cdfeTrapTag;
        [ReadOnly] public ComponentLookup<ObstacleTag> cdfeObstacleTag;
        [ReadOnly] public ComponentLookup<AreaTag> cdfeAreaTag;
        [ReadOnly] public ComponentLookup<BoardData> cdfeBoardData;
        [ReadOnly] public ComponentLookup<BulletData> cdfeBulletTag;
        [NativeDisableParallelForRestriction] public ComponentLookup<AreaTempHp> cdfeAreaTempHp;
        [NativeDisableParallelForRestriction] public BufferLookup<Skill> bfeSkill;
        [NativeDisableParallelForRestriction] public BufferLookup<Buff> bfeBuff;
        [NativeDisableParallelForRestriction] public ComponentLookup<JiYuColor> cdfeJiYuColor;
        public EntityStorageInfoLookup storageInfoFromEntity;

        [ReadOnly] public BufferLookup<BanBulletTriggerBuffer> bfeBanBulletTriggerBuffer;

        [ReadOnly] public ComponentLookup<BossTag> cdfeBossTag;
        [ReadOnly] public ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> allEntities;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> targetEntities;
        [ReadOnly] public ComponentLookup<BulletTempTag> cdfeBulletTempTag;
        [ReadOnly] public ComponentLookup<ThronTag> cdfeThroneTag;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<SkillAttackData> skillAttackDatas;


        // [ReadOnly] public ComponentLookup<QueryData> cdfeQueryData;
        // [ReadOnly] public ComponentLookup<RayCastDect> cdfeRayCastDect;
        public void Execute([EntityIndexInQuery] int index, Entity entity,
            ref DynamicBuffer<StatefulTriggerEvent> triggerEventBuffer, ref SkillAttackData attackData)
        {
            SkillAttackData_ReadWrite refData = new SkillAttackData_ReadWrite
            {
                cdfePhysicsCollider = cdfePhysicsCollider,
                cdfeLocalTransform = cdfeLocalTransforms,
                ecb = ecb,
                storageInfoFromEntity = storageInfoFromEntity,
                physicsCollider = cdfePhysicsCollider.HasComponent(entity)
                    ? cdfePhysicsCollider[entity]
                    : default,
                cdfeAgentBody = cdfeAgentBodys,
                bfeSkill = bfeSkill,
                bfeBuff = bfeBuff,
                cdfePhysicsVelocity = cdfeVelocities,
                cdfeTargetData = cdfeTargetData,
                cdfeEnemyData = cdfeEnemyData,
                cdfeBulletSonData = cdfeBulletSonData,
                cdfeAreaTempHp = cdfeAreaTempHp,
                cdfeAgentLocomotion = cdfeAgentLocomotion,
                cdfePhysicsMass = cdfePhysicsMass,
            };
            SkillAttackData_ReadOnly inData = new SkillAttackData_ReadOnly
            {
                timeTick = timeTick,
                sortkey = index,
                fDT = fDT,
                eT = eT,
                config = globalConfigData,
                prefabMapData = prefabMapData,
                player = player,
                entity = entity,
                cdfeChaStats = cdfeChaStats,
                allEntities = allEntities,
                targetEntities = targetEntities,
                cdfeTrapTag = cdfeTrapTag,
                cdfeObstacleTag = cdfeObstacleTag,
                cdfeAreaTag = cdfeAreaTag,
                cdfeBulletTag = cdfeBulletTag,
                otherEntities = default,
                cdfeSpiritData = cdfeSpiritData,
                cdfePlayerData = cdfePlayerData,
                bfeBossTag = cdfeBossTag,
                cdfeElementData = cdfeElementData,
                skillAttackDatas = skillAttackDatas,
                cdfeThronTag = cdfeThroneTag,
                cdfeBoardData = cdfeBoardData,
                cdfemapElementData = cdfemapElementData,
                cdfeWeaponData = cdfeWeaponData,
                cdfeBulletTempTag = cdfeBulletTempTag,
                gameOthersData = gameOtherData,
                wbe = wbe,
                gameTimeData = gameTimeData,
                cdfeTrigger = cdfeTriggerBufferLookup,
                //cdfeQueryData = cdfeQueryData,
                // cdfeRayCastDect = cdfeRayCastDect,
            };

            if (attackData.data.Single_1 < 0f)
            {
                attackData.data.OnDestroy(ref refData, in inData);
                //inData.chargeStruct = new float2(attackData.data.Single_12, attackData.data.Int32_13);
                ecb.DestroyEntity(index, entity);
                return;
            }

            if (attackData.data.Boolean_4 && attackData.data.Int32_5 <= 0)
            {
                attackData.data.OnDestroy(ref refData, in inData);
                ecb.DestroyEntity(index, entity);
                return;
            }

            if (attackData.data.Int32_2 == 0)
            {
                //Debug.Log(
                //    $"attackData.data.Int32_0 attackData.data{attackData.data.Int32_0} {attackData.data.CurrentTypeId}");
                //attackData.data.Single_18 = attackData.data.Single_1;
                attackData.data.Single_31 = attackData.data.Single_8;
                attackData.data.OnStart(ref refData, in inData);
                //attackData.data.OnUpdate(ref refData, in inData);
                //Debug.
            }
            else
            {
                attackData.data.OnUpdate(ref refData, in inData);
            }

            if (attackData.data.Int32_2 == 1 && attackData.data.Boolean_4 && cdfeJiYuColor.HasComponent(entity))
            {
                var jiyuColor = cdfeJiYuColor[entity];
                jiyuColor.value.w = 1;
                cdfeJiYuColor[entity] = jiyuColor;
            }

            //处理技能实体碰撞和伤害
            FixedList512Bytes<Entity> otherEntities = new FixedList512Bytes<Entity>();
            //NativeList<Entity> otherEntities = new NativeList<Entity>(triggerEventBuffer.Length, Allocator.Temp);
            for (int i = 0; i < triggerEventBuffer.Length; i++)
            {
                //Debug.LogError($"11111111111111111111");
                var triggerEvent = triggerEventBuffer[i];
                var otherEntity = triggerEvent.GetOtherEntity(entity);
                //otherEntities[i] = otherEntity;
                // exclude static bodies, other triggers and enter/exit events
                //Debug.LogError($"dsfsafsf000");
                if (!NonTriggerDynamicBodyMask.MatchesIgnoreFilter(otherEntity))
                {
                    //Debug.LogError($"entity:{otherEntity.Index}");
                    continue;
                }


                if (!storageInfoFromEntity.Exists(entity) || !storageInfoFromEntity.Exists(otherEntity))
                {
                    //Debug.LogError($"xxxxxxxxxxxxxxxxxxxx");
                    continue;
                }

                if (attackData.data.Entity_3 == otherEntity)
                {
                    continue;
                }


                if (attackData.data.Boolean_4)
                {
                    if (triggerEvent.State == StatefulEventState.Exit && !attackData.data.Boolean_20)
                    {
                        continue;
                    }

                    if (triggerEvent.State == StatefulEventState.Stay)
                    {
                        //onstay冷却
                        if (attackData.data.Single_23 > 0)
                        {
                            if (attackData.data.Single_17 <= attackData.data.Single_23)
                            {
                                continue;
                            }
                            else
                            {
                                attackData.data.Single_17 = 0;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (triggerEvent.State == StatefulEventState.Enter && attackData.data.Single_23 <= 0)
                    {
                        if (bfeBanBulletTriggerBuffer.HasBuffer(otherEntity))
                        {
                            var buffers = bfeBanBulletTriggerBuffer[otherEntity];
                            bool hasThisBullet = false;
                            foreach (var buffer in buffers)
                            {
                                if (buffer.id == attackData.data.Int32_0 &&
                                    buffer.curTick == attackData.data.Int32_25)
                                {
                                    hasThisBullet = true;
                                    break;
                                }
                            }

                            if (hasThisBullet)
                            {
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (triggerEvent.State == StatefulEventState.Exit && !attackData.data.Boolean_20)
                    {
                        continue;
                    }

                    // if (attackData.data.Int32_21 == 0 && triggerEvent.State == StatefulEventState.Stay)
                    // {
                    //     Debug.LogError($"triggerEvent.State == StatefulEventState.Stay");
                    //     continue;
                    // }
                }


                if (!(cdfeTargetData.HasComponent(otherEntity) && cdfeTargetData.HasComponent(entity)))
                {
                    //Debug.LogError($"22222222222222222222222222");
                    continue;
                }


                var denfenderTarget = cdfeTargetData[otherEntity];
                var attackerTarget = cdfeTargetData[entity];
                //Debug.Log($"dsfsafsf111111 attackerTarget  denfenderTarget");
                if (!PhysicsHelper.IsTargetEnabled(attackerTarget, denfenderTarget))
                {
                    //Debug.Log($"333333333333333333333333");
                    continue;
                }


                //Debug.Log($"4444444444444444444");
                bool isExist = false;
                if (storageInfoFromEntity.Exists(attackData.data.Entity_3))
                {
                    if (attackData.data.Entity_3 == otherEntity)
                    {
                        continue;
                    }

                    isExist = true;
                }

                //Debug.LogError($"555555555555");
                //一次生命周期只对同一只怪造成一次击退
                if (!cdfeObstacleTag.HasComponent(otherEntity))
                {
                    if (cdfeHitBackData.HasComponent(otherEntity) && cdfeHitBackData[otherEntity].id ==
                        (attackData.data.Int32_0 + entity.Index + entity.Version))
                    {
                        //Debug.Log($"{attackData.data.Int32_0}");
                        continue;
                    }
                }

                bool hasIgnoreHitBack = false;
                if (cdfeIgnoreHitBackData.HasComponent(otherEntity))
                {
                    hasIgnoreHitBack = true;
                }

                if (hasIgnoreHitBack && cdfeIgnoreHitBackData[otherEntity].IgnoreType == 0)
                {
                    continue;
                }

                otherEntities.Add(otherEntity);
                //Debug.LogError($"66666666666666666");
            }

            inData.otherEntities = otherEntities;
            for (int i = 0; i < triggerEventBuffer.Length; i++)
            {
                //Debug.LogError($"11111111111111111111");
                var triggerEvent = triggerEventBuffer[i];
                var otherEntity = triggerEvent.GetOtherEntity(entity);
                //otherEntities[i] = otherEntity;
                // exclude static bodies, other triggers and enter/exit events
                //Debug.LogError($"dsfsafsf000");
                if (!NonTriggerDynamicBodyMask.MatchesIgnoreFilter(otherEntity))
                {
                    //Debug.LogError($"entity:{otherEntity.Index}");
                    continue;
                }


                if (!storageInfoFromEntity.Exists(entity) || !storageInfoFromEntity.Exists(otherEntity))
                {
                    //Debug.LogError($"xxxxxxxxxxxxxxxxxxxx");
                    continue;
                }

                if (attackData.data.Entity_3 == otherEntity)
                {
                    continue;
                }


                if (attackData.data.Boolean_4)
                {
                    if (triggerEvent.State == StatefulEventState.Exit && !attackData.data.Boolean_20)
                    {
                        continue;
                    }

                    if (triggerEvent.State == StatefulEventState.Stay)
                    {
                        if (attackData.data.Single_23 > 0)
                        {
                            attackData.data.Single_17 += fDT;
                            if (attackData.data.Single_17 <= attackData.data.Single_23)
                            {
                                continue;
                            }
                            else
                            {
                                attackData.data.Single_17 = 0;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (triggerEvent.State == StatefulEventState.Enter && attackData.data.Single_23 <= 0)
                    {
                        if (bfeBanBulletTriggerBuffer.HasBuffer(otherEntity))
                        {
                            var buffers = bfeBanBulletTriggerBuffer[otherEntity];
                            bool hasThisBullet = false;
                            foreach (var buffer in buffers)
                            {
                                if (buffer.id == attackData.data.Int32_0 &&
                                    buffer.curTick == attackData.data.Int32_25)
                                {
                                    hasThisBullet = true;
                                    break;
                                }
                            }

                            if (hasThisBullet)
                            {
                                continue;
                            }
                            else
                            {
                                ecb.AppendToBuffer(index, otherEntity, new BanBulletTriggerBuffer
                                {
                                    id = attackData.data.Int32_0,
                                    curTick = attackData.data.Int32_25,
                                    duration = OnEnterCoolDown
                                });
                            }
                        }
                    }
                }
                else
                {
                    if (triggerEvent.State == StatefulEventState.Exit && !attackData.data.Boolean_20)
                    {
                        continue;
                    }

                    // PhysicsWorld physicsWorld;
                    // physicsWorld.CastRay(new RaycastInput
                    // {
                    //     Filter = default,
                    //     Start = default,
                    //     End = default
                    // },ref );

                    // if (attackData.data.Int32_21 == 0 && triggerEvent.State == StatefulEventState.Stay)
                    //     continue;

                    // else
                    // {
                    // }
                }


                if (!(cdfeTargetData.HasComponent(otherEntity) && cdfeTargetData.HasComponent(entity)))
                {
                    //Debug.LogError($"22222222222222222222222222");
                    continue;
                }

                var denfenderTarget = cdfeTargetData[otherEntity];
                var attackerTarget = cdfeTargetData[entity];
                //Debug.LogError($"dsfsafsf111111 attackerTarget entity{entity} otherEntity{otherEntity}");
                if (!PhysicsHelper.IsTargetEnabled(attackerTarget, denfenderTarget))
                {
                    //Debug.LogError($"333333333333333333333333");
                    continue;
                }


                //Debug.LogError($"4444444444444444444");
                bool isExist = false;
                if (storageInfoFromEntity.Exists(attackData.data.Entity_3))
                {
                    if (attackData.data.Entity_3 == otherEntity)
                    {
                        continue;
                    }

                    isExist = true;
                }


                //一次生命周期只对同一只怪造成一次伤害/击退
                if (!cdfeObstacleTag.HasComponent(otherEntity))
                {
                    if (cdfeHitBackData.HasComponent(otherEntity) &&
                        cdfeHitBackData[otherEntity].id == (attackData.data.Int32_0 + entity.Index + entity.Version))
                    {
                        //Debug.Log($"{attackData.data.Int32_0}");
                        continue;
                    }
                }


                bool hasIgnoreHitBack = false;
                if (cdfeIgnoreHitBackData.HasComponent(otherEntity))
                {
                    hasIgnoreHitBack = true;
                }

                if (hasIgnoreHitBack && cdfeIgnoreHitBackData[otherEntity].IgnoreType == 0)
                {
                    continue;
                }


                if (cdfeChaStats.HasComponent(otherEntity) && cdfeChaStats[otherEntity].chaResource.hp <= 0)
                {
                    if (attackData.data.CurrentTypeId != SkillAttack.TypeId.SkillAttack_3)
                    {
                        continue;
                    }
                }

                // if (attackData.data.Int32_19 == 105100 &&
                //     attackData.data.CurrentTypeId == SkillAttack.TypeId.SkillAttack_0)
                // {
                //     if (refData.cdfeBulletSonData.HasComponent(otherEntity))
                //     {
                //         var bulletSonData = refData.cdfeBulletSonData[otherEntity];
                //
                //         if (!bulletSonData.Equals(attackData.data.Entity_3, attackData.data.Int32_19,
                //                 attackData.data.Int32_20))
                //         {
                //             bulletSonData.addTime = attackData.data.Int32_20;
                //             bulletSonData.skillId = attackData.data.Int32_19;
                //             bulletSonData.caster = attackData.data.Entity_3;
                //             refData.cdfeBulletSonData[otherEntity] = bulletSonData;
                //         }
                //         else
                //         {
                //             continue;
                //         }
                //     }
                //     else
                //     {
                //         refData.ecb.AddComponent(inData.sortkey, attackData.data.Entity_3, new BulletSonData
                //         {
                //             caster = attackData.data.Entity_3,
                //             skillId = attackData.data.Int32_19,
                //             addTime = attackData.data.Int32_20
                //         });
                //     }
                // }


                // if (hasIgnoreHitBack && cdfeIgnoreHitBackData[otherEntity].id == 0)
                // {
                //     continue;
                // }
                //Debug.LogError($"5555555555555555555");
                if (triggerEvent.State == StatefulEventState.Exit)
                {
                    attackData.data.Entity_6 = otherEntity;
                    attackData.data.OnExit(ref refData, in inData);
                    continue;
                }

                //Debug.Log($"OnHit {attackData.data.Int32_0}");
                //Debug.LogError($"66666666666666666");
                attackData.data.Entity_6 = otherEntity;
                attackData.data.OnHit(ref refData, in inData);

                if (attackData.data.CurrentTypeId != SkillAttack.TypeId.SkillAttack_3 &&
                    attackData.data.CurrentTypeId != SkillAttack.TypeId.SkillAttack_6 &&
                    attackData.data.Int32_5 != MathHelper.MaxNum)
                {
                    attackData.data.Int32_5--;
                }

                #region MyRegion

                //var collerotherEntity = cdfePhysicsCollider[otherEntity].Value.Value.GetCollisionFilter();

                //Debug.Log($"HitTime");


                // int battle_force_factor = 0;
                //
                // for (int j = 0; j < constConfig.Length; j++)
                // {
                //     if (constConfig[j].constantName == (FixedString128Bytes)"battle_force_factor")
                //     {
                //         battle_force_factor = constConfig[j].constantValue;
                //     }
                // }

                //var physicsVelocity = cdfeVelocities.HasComponent(otherEntity) ? cdfeVelocities[otherEntity] : default;


                //var agentBody = cdfeAgentBodys.HasComponent(otherEntity) ? cdfeAgentBodys[otherEntity] : default;
                // int otherEntityElementId = 0;
                //
                // if (cdfeElementData.HasComponent(otherEntity))
                // {
                //     otherEntityElementId = cdfeElementData[otherEntity].id;
                // }
                //bool is
                // for (int j = 0; j < skillHitEffect.Length; j++)
                // {
                //     if (skillHitEffect[j].buffID == 30000004 || skillHitEffect[j].buffID == 30000003 ||
                //         skillHitEffect[j].buffID == 30000009)
                //     {
                //         if (!cdfeChaStats.HasComponent(otherEntity))
                //         {
                //             continue;
                //         }
                //     }
                //     else if (skillHitEffect[j].buffID >= 40000003 && skillHitEffect[j].buffID <= 40000010)
                //     {
                //         if (cdfeObstacleTag.HasComponent(otherEntity))
                //         {
                //             continue;
                //         }
                //     }
                //
                //     //Debug.LogError($"{attackData.data.Int32_0}");
                //     //推力效果
                //     if (skillHitEffect[j].buffID >= 40000003 && skillHitEffect[j].buffID <= 40000037)
                //     {
                //         //Debug.LogError($"force {attackData.data.Int32_0}");
                //         //施加推力
                //         //推力方向
                //         var varcollfliter = CollisionFilter.Default;
                //         varcollfliter.CollidesWith = (uint)skillHitEffect[j].buffArgs.c3.x;
                //         if ((uint)skillHitEffect[j].buffArgs.c3.x == 0)
                //         {
                //             varcollfliter.CollidesWith = (uint)60;
                //         }
                //
                //
                //         if (CollisionFilter.IsCollisionEnabled(varcollfliter, collerotherEntity))
                //         {

                //
                //             float3 dir = default;
                //             Entity dirEntity = isExist ? attackData.data.Entity_3 : e;
                //             if (skillHitEffect[j].buffArgs.c2.x == 0)
                //             {
                //                 if (skillHitEffect[j].buffArgs.c1.x == 0 &&
                //                     skillHitEffect[j].buffArgs.c1.y == 0 && skillHitEffect[j].buffArgs.c1.z == 0)
                //                 {
                //                     dir = math.normalizesafe(
                //                         cdfeLocalTransforms[otherEntity].Position -
                //                         cdfeLocalTransforms[dirEntity].Position);
                //                 }
                //                 else
                //                 {
                //                     dir = math.normalizesafe(skillHitEffect[j].buffArgs.c1);
                //                 }
                //             }
                //             else if (skillHitEffect[j].buffArgs.c2.x == 1)
                //             {
                //                 dir = math.normalizesafe(
                //                     cdfeLocalTransforms[otherEntity].Position -
                //                     cdfeLocalTransforms[dirEntity].Position);
                //             }
                //             else if (skillHitEffect[j].buffArgs.c2.x == 2)
                //             {
                //                 dir = math.normalizesafe(
                //                     cdfeLocalTransforms[otherEntity].Position -
                //                     cdfeLocalTransforms[e].Position);
                //             }
                //
                //
                //             dir = math.normalizesafe(
                //                 cdfeLocalTransforms[otherEntity].Position -
                //                 cdfeLocalTransforms[e].Position);
                //
                //             //battle_force_factor
                //
                //             //var dir = new float3(0, 1, 0);
                //             //v=(推力*（1-A的击退抗性万分比）-A的击退抗性固定值）/A的质量*碰撞速度修正系数(constants表中常量值)
                //
                //
                //             if (!cdfeChaStats.HasComponent(otherEntity))
                //             {
                //                 continue;
                //             }
                //
                //             var otherTotalProperty = cdfeChaStats[otherEntity].chaProperty;
                //
                //             float ratios = 0;
                //             if (skillHitEffect[j].buffArgs.c0.y == 0)
                //             {
                //                 //Debug.LogError($"c0.y == 0");
                //                 ratios = ((int)(skillHitEffect[j].buffArgs.c0.x) *
                //                           (1 - otherTotalProperty.reduceHitBackRatios / 10000f) -
                //                           otherTotalProperty.reduceHitBackRatios) /
                //                     otherTotalProperty.mass *
                //                     (battle_force_factor / 10000f) / 100f;
                //             }
                //             else if (isExist)
                //             {
                //                 var playerData = new PlayerData();
                //                 var temp = cdfeChaStats[attackData.data.Entity_3];
                //
                //                 UnityHelper.HandleBuffArgs(skillHitEffect[j].buffArgs,
                //                     ref temp, ref playerData);
                //
                //                 ratios =
                //                     (temp.chaProperty.pushForce * (1 + temp.chaProperty.pushForceRatios / 10000f) *
                //                      (1 - otherTotalProperty.reduceHitBackRatios / 10000f) -
                //                      otherTotalProperty.reduceHitBackRatios) / otherTotalProperty.mass *
                //                     (battle_force_factor / 10000f) / 100f;
                //                 //Debug.LogError($"isExist {ratios}");
                //             }
                //
                //
                //             if (!hasIgnoreHitBack)
                //             {
                //                 physicsVelocity.Linear += math.clamp(ratios, 0, math.abs(ratios)) * dir;
                //             }
                //             else
                //             {
                //                 if (cdfeIgnoreHitBackData[otherEntity].IgnoreType != 2)
                //                 {
                //                     physicsVelocity.Linear += math.clamp(ratios, 0, math.abs(ratios)) * dir;
                //                 }
                //             }
                //
                //             if (ratios > 2000)
                //             {
                //                 Debug.LogError($"ratios {ratios} {skillHitEffect[j].buffID}");
                //             }
                //             // if (!cdfeIgnoreHitBackData.HasComponent(otherEntity))
                //             // {
                //             // }
                //
                //             //Debug.Log($"dir{dir}  Linear{physicsVelocity.Linear} ratios{ratios}"); 
                //         }
                //     }
                //
                //     //障碍物伤害buff 障碍物受到伤害 = 技能中附加的固定伤害*（1+玩家.移动速度转化为推力加成）
                //     else if (skillHitEffect[j].buffID == 30000004)
                //     {
                //         //Debug.Log($"障碍物伤害buff== 30000004");
                //         //TODO:后续改
                //         var varcollfliter = CollisionFilter.Default;
                //         varcollfliter.CollidesWith = (uint)skillHitEffect[j].buffArgs.c3.x;
                //         if ((uint)skillHitEffect[j].buffArgs.c3.x == 0)
                //         {
                //             varcollfliter.CollidesWith = (uint)64;
                //         }
                //
                //         if (CollisionFilter.IsCollisionEnabled(varcollfliter, collerotherEntity))
                //         {
                //             //Debug.Log($"障碍物伤害0");
                //             //var damage = skillHitEffect[j].buffArgs.c0.x;
                //             if (!isExist)
                //             {
                //                 Debug.LogError($"{attackData.data.Entity_3}is Empty or Invaid");
                //                 continue;
                //             }
                //
                //
                //             var damageBefore = skillHitEffect[j].buffArgs.c0.x *
                //                                (1 + cdfeChaStats[attackData.data.Entity_3].chaProperty.pushForceRatios /
                //                                    10000f);
                //             float damageRatios = 0;
                //             if (cdfeElementData.HasComponent(e))
                //             {
                //                 int casterId = cdfeElementData[e].id;
                //                 switch (casterId)
                //                 {
                //                     case 101:
                //                         switch (otherEntityElementId)
                //                         {
                //                             case 306:
                //                                 if (BuffHelper.TryGetElementArgs(globalConfigData, casterId,
                //                                         otherEntityElementId, out var list))
                //                                 {
                //                                     damageRatios = list[0] / 10000f;
                //                                 }
                //
                //                                 break;
                //                         }
                //
                //                         break;
                //
                //                     case 102:
                //                         switch (otherEntityElementId)
                //                         {
                //                             case 306:
                //
                //                                 ecb.AppendToBuffer(index, wbe, new DamageInfo
                //                                 {
                //                                     attacker = default,
                //                                     defender = attackData.data.Entity_3,
                //                                     tags = new DamageInfoTag
                //                                     {
                //                                         directDamage = false,
                //                                         periodDamage = false,
                //                                         reflectDamage = false,
                //                                         copyDamage = false,
                //                                         directHeal = true,
                //                                         periodHeal = false
                //                                     },
                //                                     damage = new Damage
                //                                     {
                //                                         normal = (int)math.floor(damageBefore),
                //                                         bullet = 0,
                //                                         collide = 0
                //                                     },
                //                                     criticalRate = 0,
                //                                     criticalDamage = 0,
                //                                     hitRate = 1,
                //                                     degree = 0,
                //                                     pos = 0,
                //                                     bulletEntity = default,
                //                                 });
                //                                 break;
                //                         }
                //
                //                         break;
                //                 }
                //             }
                //
                //             var damage = (1 + damageRatios) * damageBefore;
                //
                //
                //             ecb.AppendToBuffer(index, wbe, new DamageInfo
                //             {
                //                 attacker = attackData.data.Entity_3,
                //                 defender = otherEntity,
                //                 tags = new DamageInfoTag
                //                 {
                //                     directDamage = true,
                //                     periodDamage = false,
                //                     reflectDamage = false,
                //                     copyDamage = false,
                //                     directHeal = false,
                //                     periodHeal = false
                //                 },
                //                 damage = new Damage
                //                 {
                //                     normal = (int)math.floor(damage),
                //                     bullet = 0,
                //                     collide = 0
                //                 },
                //                 criticalRate = 0,
                //                 criticalDamage = 0,
                //                 hitRate = 1,
                //                 degree = 0,
                //
                //                 pos = 0,
                //                 bulletEntity = default,
                //             });
                //             //Debug.Log($"障碍物伤害1");
                //         }
                //     }
                //     //伤害buff30000003
                //     else if (skillHitEffect[j].buffID == 30000003)
                //     {
                //         //Debug.LogError($"damage{attackData.data.Int32_0}");
                //         //TODO:后续改
                //         var varcollfliter = CollisionFilter.Default;
                //         varcollfliter.CollidesWith = (uint)skillHitEffect[j].buffArgs.c3.x;
                //         if ((uint)skillHitEffect[j].buffArgs.c3.x == 0)
                //         {
                //             varcollfliter.CollidesWith = (uint)60;
                //         }
                //
                //         if (CollisionFilter.IsCollisionEnabled(varcollfliter, collerotherEntity))
                //         {
                //             if (!isExist)
                //             {
                //                 Debug.LogError($"{attackData.data.Entity_3}is Empty or Invaid");
                //                 continue;
                //             }
                //
                //             var playerData = new PlayerData();
                //             var temp = cdfeChaStats[attackData.data.Entity_3];
                //
                //             UnityHelper.HandleBuffArgs(skillHitEffect[j].buffArgs,
                //                 ref temp, ref playerData);
                //
                //             var atk = (temp.chaProperty.atk + temp.chaProperty.atkAdd) *
                //                       (1 + temp.chaProperty.atkRatios / 10000f);
                //             var damage = atk *
                //                          ((1 + cdfeChaStats[attackData.data.Entity_3].chaProperty
                //                               .damageRatios / 10000f) *
                //                           (1 - cdfeChaStats[otherEntity].chaProperty
                //                               .reduceHurtRatios / 10000f) +
                //                           (cdfeChaStats[attackData.data.Entity_3].chaProperty
                //                                .damageAdd -
                //                            cdfeChaStats[otherEntity].chaProperty.reduceHurtRatios));
                //
                //             //（玩家攻击力默认值(301)+攻击力固定加成(309)）*（1+攻击力加成(302)					
                //             //max({A.攻击力 * [A.是否暴击(基础暴击率+暴击率加成) * A.暴击伤害(基础暴击伤害倍率+暴击伤害加成)] * [(1+A.增伤加成) * (1-B.减伤加成)] + [(A.增伤固定加成 - B.减伤固定加成)]},1)
                //             ecb.AppendToBuffer(index, wbe, new DamageInfo
                //             {
                //                 attacker = attackData.data.Entity_3,
                //                 defender = otherEntity,
                //                 tags = new DamageInfoTag
                //                 {
                //                     directDamage = true,
                //                     periodDamage = false,
                //                     reflectDamage = false,
                //                     copyDamage = false,
                //                     directHeal = false,
                //                     periodHeal = false
                //                 },
                //                 damage = new Damage
                //                 {
                //                     normal = (int)math.floor(damage),
                //                     bullet = 0,
                //                     collide = 0
                //                 },
                //                 criticalRate = cdfeChaStats[attackData.data.Entity_3]
                //                                    .chaProperty.critical /
                //                                10000f,
                //                 criticalDamage = cdfeChaStats[attackData.data.Entity_3]
                //                                      .chaProperty.criticalDamageRatios /
                //                                  10000f,
                //                 hitRate = 1,
                //                 degree = 0,
                //
                //                 pos = 0,
                //                 bulletEntity = default,
                //             });
                //         }
                //     }
                //     //伤害buff
                //     else if (skillHitEffect[j].buffID == 30000012)
                //     {
                //         //TODO:后续改
                //         var varcollfliter = CollisionFilter.Default;
                //         varcollfliter.CollidesWith = (uint)skillHitEffect[j].buffArgs.c3.x;
                //         if ((uint)skillHitEffect[j].buffArgs.c3.x == 0)
                //         {
                //             varcollfliter.CollidesWith = (uint)60;
                //         }
                //
                //         if (CollisionFilter.IsCollisionEnabled(varcollfliter, collerotherEntity))
                //         {
                //             if (!isExist)
                //             {
                //                 Debug.LogError($"{attackData.data.Entity_3}is Empty or Invaid");
                //                 continue;
                //             }
                //
                //
                //             var playerData = new PlayerData();
                //             var temp = cdfeChaStats[attackData.data.Entity_3];
                //
                //             UnityHelper.HandleBuffArgs(skillHitEffect[j].buffArgs,
                //                 ref temp, ref playerData);
                //
                //             var atk = (temp.chaProperty.atk + temp.chaProperty.atkAdd) *
                //                       (1 + temp.chaProperty.atkRatios / 10000f);
                //             var damage = atk *
                //                          ((1 + cdfeChaStats[attackData.data.Entity_3].chaProperty
                //                               .damageRatios / 10000f) *
                //                           (1 - cdfeChaStats[otherEntity].chaProperty
                //                               .reduceHurtRatios / 10000f) +
                //                           (cdfeChaStats[attackData.data.Entity_3].chaProperty
                //                                .damageAdd -
                //                            cdfeChaStats[otherEntity].chaProperty.reduceHurtRatios));
                //
                //             //（玩家攻击力默认值(301)+攻击力固定加成(309)）*（1+攻击力加成(302)					
                //             //max({A.攻击力 * [A.是否暴击(基础暴击率+暴击率加成) * A.暴击伤害(基础暴击伤害倍率+暴击伤害加成)] * [(1+A.增伤加成) * (1-B.减伤加成)] + [(A.增伤固定加成 - B.减伤固定加成)]},1)
                //             ecb.AppendToBuffer(index, wbe, new DamageInfo
                //             {
                //                 attacker = attackData.data.Entity_3,
                //                 defender = otherEntity,
                //                 tags = new DamageInfoTag
                //                 {
                //                     directDamage = true,
                //                     periodDamage = false,
                //                     reflectDamage = false,
                //                     copyDamage = false,
                //                     directHeal = false,
                //                     periodHeal = false
                //                 },
                //                 damage = new Damage
                //                 {
                //                     normal = (int)math.floor(damage),
                //                     bullet = 0,
                //                     collide = 0
                //                 },
                //                 criticalRate = cdfeChaStats[attackData.data.Entity_3]
                //                                    .chaProperty.critical /
                //                                10000f,
                //                 criticalDamage = cdfeChaStats[attackData.data.Entity_3]
                //                                      .chaProperty.criticalDamageRatios /
                //                                  10000f,
                //                 hitRate = 1,
                //                 degree = 0,
                //
                //                 pos = 0,
                //                 bulletEntity = default,
                //             });
                //         }
                //     }
                //     //TODO：炸弹二阶伤害可以附加buff    
                //     else if (skillHitEffect[j].buffID == 30000009)
                //     {
                //     }
                //
                //     //skillHitEffect.RemoveAt(j);
                // }


                // write back
                // if (cdfeVelocities.HasComponent(otherEntity))
                // {
                //     cdfeVelocities[otherEntity] = physicsVelocity;
                // }
                //
                // if (cdfeAgentBodys.HasComponent(otherEntity))
                // {
                //     agentBody.Stop();
                //     cdfeAgentBodys[otherEntity] = agentBody;
                // }

                #endregion
            }

            
            attackData.data.Single_1 -= fDT * gameTimeData.logicTime.gameTimeScale;
            attackData.data.Int32_2++;

            attackData.data.Single_8 = attackData.data.Single_31
                                       * gameTimeData.logicTime.gameTimeScale;

            var tran = refData.cdfeLocalTransform[entity];

            tran = attackData.data.DoSkillMove(ref refData, in inData);


            refData.cdfeLocalTransform[entity] = tran;
            
            //otherEntities.Dispose();
        }
        
    }
}