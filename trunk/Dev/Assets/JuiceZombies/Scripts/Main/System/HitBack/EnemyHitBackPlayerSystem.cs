//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-09-05 12:00:25
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
public partial struct EnemyHitBackPlayerSystem : ISystem
{
    public const int hitBackMaxCount = 2;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WorldBlackBoardTag>();
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PlayerData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

        var globalConfigData = SystemAPI.GetSingleton<GlobalConfigData>();

        var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        var nonTriggerDynamicBodyQuery = SystemAPI.QueryBuilder()
            .WithAllRW<PhysicsVelocity>()
            .WithAll<LocalTransform, PhysicsMass, EnemyData, ChaStats>()
            .WithNone<HitBackData>().WithNone<TimeToDieData>().Build();


        new EnemyHitBackSystemJob
        {
            ecb = ecb,
            NonTriggerDynamicBodyMask = nonTriggerDynamicBodyQuery.GetEntityQueryMask(),
            fDT = SystemAPI.Time.fixedDeltaTime,
            cdfeLocalTransforms = SystemAPI.GetComponentLookup<LocalTransform>(true),
            cdfeMasses = SystemAPI.GetComponentLookup<PhysicsMass>(),
            cdfeVelocities = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
            cdfeagentBodys = SystemAPI.GetComponentLookup<AgentBody>(),
            globalConfigData = globalConfigData,
            cdfeHitBackData = SystemAPI.GetComponentLookup<HitBackData>(),
            cdfeWeaponData = SystemAPI.GetComponentLookup<WeaponData>(),
            cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(true),
            cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
            cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(true),
            player = SystemAPI.GetSingletonEntity<PlayerData>(),
            wbe = wbe,
            cdfeIgnoreHitBackData = SystemAPI.GetComponentLookup<IgnoreHitBackData>(true),
            storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
            cdfeEntityGroupData = SystemAPI.GetComponentLookup<EntityGroupData>(true),
        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct EnemyHitBackSystemJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public EntityQueryMask NonTriggerDynamicBodyMask;
        public float fDT;

        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsMass> cdfeMasses;
        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsVelocity> cdfeVelocities;
        [NativeDisableParallelForRestriction] public ComponentLookup<AgentBody> cdfeagentBodys;
        [NativeDisableParallelForRestriction] public ComponentLookup<HitBackData> cdfeHitBackData;
        [NativeDisableParallelForRestriction] public ComponentLookup<WeaponData> cdfeWeaponData;
        [ReadOnly] public ComponentLookup<IgnoreHitBackData> cdfeIgnoreHitBackData;
        [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
        [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
        [ReadOnly] public ComponentLookup<LocalTransform> cdfeLocalTransforms;
        [ReadOnly] public GlobalConfigData globalConfigData;

        [ReadOnly] public ComponentLookup<ChaStats> cdfeChaStats;
        [ReadOnly] public ComponentLookup<EntityGroupData> cdfeEntityGroupData;
        [ReadOnly] public Entity player;
        [ReadOnly] public Entity wbe;
        public EntityStorageInfoLookup storageInfoFromEntity;

        public void Execute([EntityIndexInQuery] int index, Entity e,
            ref DynamicBuffer<StatefulCollisionEvent> triggerEventBuffer, in PlayerData attackData)
        {
            for (int i = 0; i < triggerEventBuffer.Length; i++)
            {
                var triggerEvent = triggerEventBuffer[i];
                var otherEntity = triggerEvent.GetOtherEntity(e);

                //TODO:怪物攻击间隔
                // exclude static bodies, other triggers and enter/exit events
                if (triggerEvent.State != StatefulEventState.Enter ||
                    !NonTriggerDynamicBodyMask.MatchesIgnoreFilter(otherEntity))
                {
                    continue;
                }

                if (!storageInfoFromEntity.Exists(e) || !storageInfoFromEntity.Exists(otherEntity))
                {
                    continue;
                }

                var enemyChaStats = cdfeChaStats[otherEntity];
                var playerChaStats = cdfeChaStats[e];
                bool canAttack = true;

                if (!enemyChaStats.chaImmuneState.immuneControl)
                {
                    if (enemyChaStats.chaControlState.cantWeaponAttack)
                    {
                        canAttack = false;
                    }
                }

                if (cdfeEnemyData[otherEntity].attackType != EnemyAttackType.CollideAttack || !canAttack)
                {
                    continue;
                }


                if (!cdfeIgnoreHitBackData.HasComponent(e))
                {
                    if (cdfeEnemyData[otherEntity].isHitBackAttack)
                    {
                        //施加推力
                        if (!cdfeHitBackData.HasComponent(e) && !playerChaStats.chaImmuneState.immunePush)
                        {
                            ecb.AddComponent(index, e, new HitBackData
                            {
                                id = 0,
                                hitTimes = hitBackMaxCount,
                                attacker = otherEntity
                            });
                        }
                        else continue;

                        // ecb.AddComponent(inData.sortkey, refData.enemyData.target, new HitBackData
                        // {
                        //     id = 0,
                        //     hitTimes = inData.gameOthersData.hitBackMaxCount,
                        //     attacker = inData.entity
                        // });


                        // var dir = math.normalizesafe(
                        //     cdfeLocalTransforms[e].Position - cdfeLocalTransforms[otherEntity].Position);
                        //
                        // //v=(推力*（1-A的击退抗性万分比）-A的击退抗性固定值）/A的质量*碰撞速度修正系数(constants表中常量值)
                        //
                        // var otherTotalProperty = playerChaStats.chaProperty;
                        // var force = (enemyChaStats.chaProperty.pushForce *
                        //              (1 + enemyChaStats.chaProperty.pushForceRatios / 10000f) *
                        //              (1 - otherTotalProperty.reduceHitBackRatios / 10000f) -
                        //              otherTotalProperty.reduceHitBackRatios) /
                        //     otherTotalProperty.mass *
                        //     (battle_force_factor / 10000f) / 100f;
                        // Debug.Log($"isHitBackAttack :{math.length(force)}");
                        //
                        // physicsVelocity.Linear = math.clamp(force, 0, math.abs(force)) * dir;
                    }
                }

                var physicsVelocity = cdfeVelocities.HasComponent(e) ? cdfeVelocities[e] : default;

                var mass = cdfeMasses.HasComponent(e) ? cdfeMasses[e] : default;

                // ref var constConfig = ref globalConfigData.value.Value.configTbconstants.configTbconstants;
                //
                // int battle_force_factor = 0;
                //
                // for (int j = 0; j < constConfig.Length; j++)
                // {
                //     if (constConfig[j].constantName == (FixedString128Bytes)"battle_force_factor")
                //     {
                //         battle_force_factor = constConfig[j].constantValue;
                //     }
                // }
                var otherEntityGroupData = cdfeEntityGroupData[otherEntity];
                if (storageInfoFromEntity.Exists(otherEntityGroupData.weaponEntity))
                {
                    var weaponData = cdfeWeaponData[otherEntityGroupData.weaponEntity];
                    weaponData.curAttackTime = weaponData.attackTime;
                    weaponData.curRepeatTimes = weaponData.repeatTimes;
                    cdfeWeaponData[otherEntityGroupData.weaponEntity] = weaponData;
                }


                // if (!cdfeHitBackData.HasComponent(e))
                // {
                //     ecb.AddComponent(index, e, new HitBackData
                //     {
                //         id = 0,
                //         hitTimes = hitBackMaxCount
                //     });
                // }
                // else
                // {
                //     continue;
                // }

                // else
                // {
                //     // var hitBackData = cdfeHitBackData[otherEntity];
                //     // hitBackData.hitTimes = hitBackMaxCount;
                //     // cdfeHitBackData[otherEntity] = hitBackData;
                // }
                //Debug.Log($"enemyhitbackplayer");
                if (!playerChaStats.chaImmuneState.immuneDamage)
                {
                    var damage = enemyChaStats.chaProperty.atk *
                                 ((1 + enemyChaStats.chaProperty
                                      .damageRatios / 10000f) *
                                  (1 - playerChaStats.chaProperty
                                      .reduceHurtRatios / 10000f) +
                                  (enemyChaStats.chaProperty
                                       .damageAdd -
                                   playerChaStats.chaProperty.reduceHurtAdd));
                    //Debug.LogError($"damage{damage}");
                    //max({A.攻击力 * [A.是否暴击(基础暴击率+暴击率加成) * A.暴击伤害(基础暴击伤害倍率+暴击伤害加成)] * [(1+A.增伤加成) * (1-B.减伤加成)] + [(A.增伤固定加成 - B.减伤固定加成)]},1)
                    ecb.AppendToBuffer(index, wbe, new DamageInfo
                    {
                        attacker = otherEntity,
                        defender = e,
                        tags = new DamageInfoTag
                        {
                            directDamage = true,
                            directHeal = false,
                        },
                        damage = new Damage
                        {
                            normal = (int)math.floor(damage),
                        },
                        criticalRate = enemyChaStats
                                           .chaProperty.critical /
                                       10000f,
                        criticalDamage = enemyChaStats.chaProperty.criticalDamageRatios /
                                         10000f,
                        hitRate = 1,
                        degree = 0,

                        pos = 0,
                        bulletEntity = default,
                    });
                }


                //agentBody.Stop();


                // write back

                // write back
                if (cdfeVelocities.HasComponent(e))
                {
                    cdfeVelocities[e] = physicsVelocity;
                }

                // if (cdfeMasses.HasComponent(otherEntity))
                // {
                //     cdfeMasses[otherEntity] = mass;
                // }
            }
        }
    }
}