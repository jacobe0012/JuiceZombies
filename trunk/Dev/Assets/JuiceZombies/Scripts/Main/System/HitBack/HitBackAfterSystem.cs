//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-09-05 12:00:25
//---------------------------------------------------------------------

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Stateful;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(EnemyHitBackPlayerSystem))]
    public partial struct HitBackAfterSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<HitBackData>();
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            var nonTriggerDynamicBodyQuery = SystemAPI.QueryBuilder()
                .WithAll<PhysicsVelocity, HitBackData>().WithNone<SkillAttackData>()
                .Build();

            new HitBackAfterSystemJob
            {
                ecb = ecb,
                NonTriggerDynamicBodyMask = nonTriggerDynamicBodyQuery.GetEntityQueryMask(),
                prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe),
                gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe),
                globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe),
                gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe),
                fDT = SystemAPI.Time.fixedDeltaTime,
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(),
                cdfeObstacleData = SystemAPI.GetComponentLookup<ObstacleTag>(true),
                cdfeMapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
                cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(true),
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(true),
                Masses = SystemAPI.GetComponentLookup<PhysicsMass>(),
                cdfePhysicsVelocity = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
                player = SystemAPI.GetSingletonEntity<PlayerData>(),
                wbe = wbe,
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup()
            }.ScheduleParallel();
        }

        [BurstCompile]
        partial struct HitBackAfterSystemJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            public EntityQueryMask NonTriggerDynamicBodyMask;
            [ReadOnly] public PrefabMapData prefabMapData;
            [ReadOnly] public GameSetUpData gameSetUpData;
            [ReadOnly] public GlobalConfigData globalConfigData;
            [ReadOnly] public GameOthersData gameOthersData;
            public float fDT;


            [ReadOnly] public ComponentLookup<ObstacleTag> cdfeObstacleData;
            [ReadOnly] public ComponentLookup<MapElementData> cdfeMapElementData;
            [ReadOnly] public ComponentLookup<EnemyData> cdfeEnemyData;
            [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
            [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsMass> Masses;
            [NativeDisableParallelForRestriction] public ComponentLookup<ChaStats> cdfeChaStats;
            [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsVelocity> cdfePhysicsVelocity;


            [ReadOnly] public Entity player;
            [ReadOnly] public Entity wbe;
            public EntityStorageInfoLookup storageInfoFromEntity;


            public void Execute([EntityIndexInQuery] int index, Entity e,
                ref DynamicBuffer<StatefulCollisionEvent> triggerEventBuffer, ref HitBackData hitBackData,
                in LocalTransform tran)
            {
                //碰撞条件都需满足

                ref var config = ref globalConfigData.value.Value.configTbbattle_constants.configTbbattle_constants;

                int battle_speed_border = 0;
                int battle_obstacle_speed_value = 0;
                int battle_obstacle_damage_value = 0;
                int battle_obstacle_damage_toplayer_value = 0;
                int battle_impact_damage_min = 0;

                int battle_monster_force_factor_value = 0;
                int battle_continuous_collision_max_num = 0;


                for (int i = 0; i < config.Length; i++)
                {
                    if (config[i].constantName == (FixedString128Bytes)"battle_speed_border")
                    {
                        battle_speed_border = config[i].constantValue;
                    }

                    if (config[i].constantName == (FixedString128Bytes)"battle_obstacle_speed_value")
                    {
                        battle_obstacle_speed_value = config[i].constantValue;
                    }

                    if (config[i].constantName == (FixedString128Bytes)"battle_obstacle_damage_value")
                    {
                        battle_obstacle_damage_value = config[i].constantValue;
                    }

                    if (config[i].constantName == (FixedString128Bytes)"battle_obstacle_damage_toplayer_value")
                    {
                        battle_obstacle_damage_toplayer_value = config[i].constantValue;
                    }

                    if (config[i].constantName == (FixedString128Bytes)"battle_impact_damage_min")
                    {
                        battle_impact_damage_min = config[i].constantValue;
                    }


                    if (config[i].constantName == (FixedString128Bytes)"battle_monster_force_factor_value")
                    {
                        battle_monster_force_factor_value = config[i].constantValue;
                    }

                    if (config[i].constantName == (FixedString128Bytes)"battle_continuous_collision_max_num")
                    {
                        battle_continuous_collision_max_num = config[i].constantValue;
                    }
                }

                //碰撞伤害触发条件
                if (hitBackData.hitTimes <= 0)
                {
                    ecb.RemoveComponent<HitBackData>(index, e);
                    return;
                }

                if (math.length(cdfePhysicsVelocity[e].Linear) < battle_speed_border / 1000f)
                {
                    return;
                }


                for (int i = 0; i < triggerEventBuffer.Length; i++)
                {
                    var triggerEvent = triggerEventBuffer[i];
                    var otherEntity = triggerEvent.GetOtherEntity(e);


                    // exclude static bodies, other triggers and in/exit events
                    if (triggerEvent.State != StatefulEventState.Enter ||
                        !NonTriggerDynamicBodyMask.MatchesIgnoreFilter(e))
                    {
                        continue;
                    }

                    if (!storageInfoFromEntity.Exists(e) || !storageInfoFromEntity.Exists(otherEntity))
                    {
                        continue;
                    }

                    var chaStats = cdfeChaStats[e];
                    chaStats.chaResource.continuousCollCount++;
                    cdfeChaStats[e] = chaStats;

                    // UnityHelper.CreateAudioClip(gameOthersData.allAudioClips["NormalCollideSound"],
                    //     tran.Position);
                    //UnityHelper.TryCreateAudioClip(gameOthersData, "NormalCollideSound");

                    // if (cdfeBoardTag.HasComponent(otherEntity))
                    // {
                    //     //玩家碰撞障碍物
                    //     if (cdfePlayerData.HasComponent(e))
                    //     {
                    //         hitBackData.hitTimes--;
                    //         //玩家受到伤害 = max(Math.Floor(玩家.速度/障碍物标准速度值),1)*障碍物对玩家标准伤害值*关卡难度值
                    //         //TODO:*关卡难度值
                    //         var damage = math.max(
                    //             math.floor(cdfeChaStats[e].chaResource.curMoveSpeed / battle_obstacle_speed_value),
                    //             1) * battle_obstacle_damage_toplayer_value;
                    //
                    //         ecb.AppendToBuffer(index, e, new DamageInfo
                    //         {
                    //             attacker = default,
                    //             defender = e,
                    //             tags = new DamageInfoTag
                    //             {
                    //                 directDamage = true,
                    //                 periodDamage = false,
                    //                 reflectDamage = false,
                    //                 copyDamage = false,
                    //                 directHeal = false,
                    //                 periodHeal = false
                    //             },
                    //             damage = new Damage
                    //             {
                    //                 normal = 0,
                    //                 bullet = 0,
                    //                 collide = (int)math.floor(damage)
                    //             },
                    //             criticalRate = 0,
                    //             criticalDamage = 0,
                    //             hitRate = 1,
                    //             degree = 0,
                    //
                    //             pos = 0,
                    //             bulletEntity = default,
                    //         });
                    //     }
                    //     //怪物A碰撞障碍物
                    //     else if (cdfeEnemyData.HasComponent(e))
                    //     {
                    //         //Debug.Log($"怪物A碰撞障碍物6666666666666666666666666666666");
                    //         var velocityEntity = cdfeChaStats[e].chaResource.curMoveSpeed;
                    //
                    //         //A.受到伤害  = max({玩家.攻击力 * [玩家.是否暴击(基础暴击率+暴击率加成) * 玩家.暴击伤害(基础暴击伤害+暴击伤害加成)] * [(1+玩家.增伤加成) * (1-A.减伤加成)] + [(玩家.增伤固定加成 - A.减伤固定加成)]},碰撞伤害下限) * A.速度 * 障碍物碰撞伤害
                    //         var damage = math.max(cdfeChaStats[player].chaProperty.atk *
                    //                               ((1 + cdfeChaStats[player].chaProperty
                    //                                    .damagePlus / 10000f) *
                    //                                (1 - cdfeChaStats[e].chaProperty
                    //                                    .reduceHurtRatios / 10000f) +
                    //                                (cdfeChaStats[player].chaProperty
                    //                                     .damageAdd -
                    //                                 cdfeChaStats[e].chaProperty.reduceHurt)),
                    //             battle_impact_damage_min) * velocityEntity;
                    //
                    //
                    //         ecb.AppendToBuffer(index, e, new DamageInfo
                    //         {
                    //             attacker = default,
                    //             defender = e,
                    //             tags = new DamageInfoTag
                    //             {
                    //                 directDamage = true,
                    //                 periodDamage = false,
                    //                 reflectDamage = false,
                    //                 copyDamage = false,
                    //                 directHeal = false,
                    //                 periodHeal = false
                    //             },
                    //             damage = new Damage
                    //             {
                    //                 normal = 0,
                    //                 bullet = 0,
                    //                 collide = (int)math.floor(damage)
                    //             },
                    //             criticalRate = cdfeChaStats[player]
                    //                                .chaProperty.critical /
                    //                            10000f,
                    //             criticalDamage = (battle_critical_base + cdfeChaStats[player]
                    //                                  .chaProperty.criticalDamageAdd) /
                    //                              10000f,
                    //             hitRate = 1,
                    //             degree = 0,
                    //
                    //             pos = 0,
                    //             bulletEntity = default,
                    //         });
                    //     }
                    // }
                    if (cdfeObstacleData.HasComponent(otherEntity))
                    {
                        ref var tbsceneModule =
                            ref globalConfigData.value.Value.configTbscene_modules.Get(
                                cdfeMapElementData[otherEntity].elementID);
                        ref var tblevel =
                            ref globalConfigData.value.Value.configTblevels.Get(gameOthersData.levelId);
                        //玩家碰撞障碍物
                        if (cdfePlayerData.HasComponent(e))
                        {
                            hitBackData.hitTimes--;
                            //玩家受到伤害 = max(Math.Floor(玩家.速度/障碍物标准速度值),1)*障碍物对玩家标准伤害值*关卡难度值

                            var damage = math.max(
                                             math.floor(cdfeChaStats[e].chaResource.curMoveSpeed /
                                                        battle_obstacle_speed_value),
                                             1) * battle_obstacle_damage_toplayer_value *
                                         (tbsceneModule.impactDamageRatio / 10000f) *
                                         (tblevel.atk / 10000f);

                            ecb.AppendToBuffer(index, wbe, new DamageInfo
                            {
                                attacker = default,
                                defender = e,
                                tags = new DamageInfoTag
                                {
                                    collideDamage = true
                                },
                                damage = new Damage
                                {
                                    normal = (int)math.floor(damage),
                                },
                                criticalRate = 0,
                                criticalDamage = 0,
                                hitRate = 1,
                                degree = 0,

                                pos = 0,
                                bulletEntity = default,
                            });

                            //障碍物受到伤害 = max(Math.Floor(玩家.速度/障碍物标准速度值),1)*障碍物标准伤害值*玩家的质量
                            if (cdfeChaStats.HasComponent(otherEntity))
                            {
                                // var damage0 = math.max(
                                //     math.floor(cdfeChaStats[e].chaResource.curMoveSpeed / battle_obstacle_speed_value),
                                //     1) * battle_obstacle_damage_value * cdfeChaStats[e].chaProperty.mass;
                                int damage0 = 1;
                                ecb.AppendToBuffer(index, wbe, new DamageInfo
                                {
                                    attacker = default,
                                    defender = otherEntity,
                                    tags = new DamageInfoTag
                                    {
                                        collideDamage = true
                                    },
                                    damage = new Damage
                                    {
                                        normal = (int)math.floor(damage0),
                                    },
                                    criticalRate = 0,
                                    criticalDamage = 0,
                                    hitRate = 1,
                                    degree = 0,
                                    pos = 0,

                                    bulletEntity = default,
                                });
                            }
                        }
                        //怪物A碰撞障碍物
                        else if (cdfeEnemyData.HasComponent(e))
                        {
                            var velocityEntity = cdfeChaStats[e].chaResource.curMoveSpeed;
                            Debug.Log($"怪物A碰撞障碍物 impact_speed_ratio{tbsceneModule.impactSpeedRatio}");
                            //A.受到伤害  = max({玩家.攻击力 * [玩家.是否暴击(基础暴击率+暴击率加成) * 玩家.暴击伤害(基础暴击伤害+暴击伤害加成)] * [(1+玩家.增伤加成) * (1-A.减伤加成)] + [(玩家.增伤固定加成 - A.减伤固定加成)]},碰撞伤害下限) * A.速度 * 障碍物碰撞伤害
                            var damage = math.max(cdfeChaStats[player].chaProperty.atk *
                                                  ((1 + cdfeChaStats[player].chaProperty
                                                       .damageRatios / 10000f) *
                                                   (1 - cdfeChaStats[e].chaProperty
                                                       .reduceHurtRatios / 10000f) *
                                                   (1 + cdfeChaStats[player].chaProperty.collideDamageRatios / 10000f +
                                                    cdfeChaStats[player].chaProperty.continuousCollideDamageRatios /
                                                    10000f * cdfeChaStats[e].chaResource.continuousCollCount) +
                                                   (cdfeChaStats[player].chaProperty.damageAdd -
                                                    cdfeChaStats[e].chaProperty.reduceHurtAdd)),
                                    battle_impact_damage_min) * velocityEntity *
                                (tbsceneModule.impactDamageRatio / 10000f) 
                                / 100000f; //新增需求修改

                            ecb.AppendToBuffer(index, wbe, new DamageInfo
                            {
                                attacker = player,
                                defender = e,
                                tags = new DamageInfoTag
                                {
                                    collideDamage = true
                                },
                                damage = new Damage
                                {
                                    normal = (int)math.floor(damage),
                                },
                                criticalRate = cdfeChaStats[player]
                                                   .chaProperty.critical /
                                               10000f,
                                criticalDamage = cdfeChaStats[player].chaProperty.criticalDamageRatios /
                                                 10000f,
                                hitRate = 1,
                                degree = 0,
                                pos = 0,
                                bulletEntity = default,
                            });


                            //障碍物受到伤害 = max(Math.Floor(A.速度/障碍物标准速度值),1)*障碍物标准伤害值*A的质量
                            if (cdfeChaStats.HasComponent(otherEntity))
                            {
                                // var damage0 = math.max(
                                //     math.floor(cdfeChaStats[e].chaResource.curMoveSpeed / battle_obstacle_speed_value),
                                //     1) * battle_obstacle_damage_value * cdfeChaStats[e].chaProperty.mass;
                                int damage0 = 1;
                                ecb.AppendToBuffer(index, wbe, new DamageInfo
                                {
                                    attacker = default,
                                    defender = otherEntity,
                                    tags = new DamageInfoTag
                                    {
                                        collideDamage = true
                                    },
                                    damage = new Damage
                                    {
                                        normal = (int)math.floor(damage0),
                                    },
                                    criticalRate = 0,
                                    criticalDamage = 0,
                                    hitRate = 1,
                                    degree = 0,
                                    pos = 0,
                                    bulletEntity = default,
                                });
                            }
                        }
                    }
                    else if (cdfeObstacleData.HasComponent(e))
                    {
                        ref var tbsceneModule =
                            ref globalConfigData.value.Value.configTbscene_modules.Get(
                                cdfeMapElementData[e].elementID);
                        ref var tblevel =
                            ref globalConfigData.value.Value.configTblevels.Get(gameOthersData.levelId);
                        //障碍物碰撞玩家
                        if (cdfePlayerData.HasComponent(otherEntity))
                        {
                            hitBackData.hitTimes--;
                            //玩家受到伤害 = max(Math.Floor(|玩家.速度+障碍物.速度*cosα|/障碍物标准速度值),1)*障碍物对玩家标准伤害值*障碍物的impact_damage_ratio字段*当前关卡atk字段

                            var otherPhysicsVelocity = cdfePhysicsVelocity[otherEntity].Linear;
                            var ePhysicsVelocity = cdfePhysicsVelocity[e].Linear;

                            var degree = math.degrees(MathHelper.Angle(otherPhysicsVelocity, ePhysicsVelocity));
                            degree = math.max(0, degree);
                            degree = degree > 90 ? 180 - degree : degree;
                            var eSpeedArgs = math.abs(math.length(otherPhysicsVelocity) +
                                                      math.length(ePhysicsVelocity) * math.cos(degree)) * 1000f;

                            var damage = math.max(
                                             math.floor(eSpeedArgs /
                                                        battle_obstacle_speed_value),
                                             1) * battle_obstacle_damage_toplayer_value *
                                         (tbsceneModule.impactDamageRatio / 10000f) *
                                         (tblevel.atk / 10000f);

                            ecb.AppendToBuffer(index, wbe, new DamageInfo
                            {
                                attacker = default,
                                defender = otherEntity,
                                tags = new DamageInfoTag
                                {
                                    collideDamage = true
                                },
                                damage = new Damage
                                {
                                    normal = (int)math.floor(damage),
                                },
                                criticalRate = 0,
                                criticalDamage = 0,
                                hitRate = 1,
                                degree = 0,

                                pos = 0,
                                bulletEntity = default,
                            });

                            //障碍物受到伤害 = max(Math.Floor(玩家.速度/障碍物标准速度值),1)*障碍物标准伤害值*玩家的质量
                            if (cdfeChaStats.HasComponent(e))
                            {
                                // var damage0 = math.max(
                                //     math.floor(cdfeChaStats[e].chaResource.curMoveSpeed / battle_obstacle_speed_value),
                                //     1) * battle_obstacle_damage_value * cdfeChaStats[e].chaProperty.mass;
                                int damage0 = 1;
                                ecb.AppendToBuffer(index, wbe, new DamageInfo
                                {
                                    attacker = default,
                                    defender = e,
                                    tags = new DamageInfoTag
                                    {
                                        collideDamage = true
                                    },
                                    damage = new Damage
                                    {
                                        normal = (int)math.floor(damage0),
                                    },
                                    criticalRate = 0,
                                    criticalDamage = 0,
                                    hitRate = 1,
                                    degree = 0,
                                    pos = 0,

                                    bulletEntity = default,
                                });
                            }
                        }
                        //障碍物碰撞怪物A
                        else if (cdfeEnemyData.HasComponent(otherEntity))
                        {
                            hitBackData.hitTimes--;

                            var otherPhysicsVelocity = cdfePhysicsVelocity[otherEntity].Linear;
                            var ePhysicsVelocity = cdfePhysicsVelocity[e].Linear;

                            var degree = math.degrees(MathHelper.Angle(otherPhysicsVelocity, ePhysicsVelocity));
                            degree = math.max(0, degree);
                            degree = degree > 90 ? 180 - degree : degree;
                            var eSpeedArgs = math.abs(math.length(otherPhysicsVelocity) +
                                                      math.length(ePhysicsVelocity) * math.cos(degree)) * 1000f;

                            var damage = math.max(
                                             math.floor(eSpeedArgs /
                                                        battle_obstacle_speed_value),
                                             1) * battle_obstacle_damage_toplayer_value *
                                         (tbsceneModule.impactDamageRatio / 10000f) *
                                         (tblevel.atk / 10000f);
                            Debug.Log($"障碍物碰撞怪物A dam:{damage}");
                            ecb.AppendToBuffer(index, wbe, new DamageInfo
                            {
                                attacker = default,
                                defender = otherEntity,
                                tags = new DamageInfoTag
                                {
                                    collideDamage = true
                                },
                                damage = new Damage
                                {
                                    normal = (int)math.floor(damage),
                                },
                                criticalRate = 0,
                                criticalDamage = 0,
                                hitRate = 1,
                                degree = 0,

                                pos = 0,
                                bulletEntity = default,
                            });

                            //障碍物受到伤害 = max(Math.Floor(玩家.速度/障碍物标准速度值),1)*障碍物标准伤害值*玩家的质量
                            if (cdfeChaStats.HasComponent(e))
                            {
                                // var damage0 = math.max(
                                //     math.floor(cdfeChaStats[e].chaResource.curMoveSpeed / battle_obstacle_speed_value),
                                //     1) * battle_obstacle_damage_value * cdfeChaStats[e].chaProperty.mass;
                                int damage0 = 1;
                                ecb.AppendToBuffer(index, wbe, new DamageInfo
                                {
                                    attacker = default,
                                    defender = e,
                                    tags = new DamageInfoTag
                                    {
                                        collideDamage = true
                                    },
                                    damage = new Damage
                                    {
                                        normal = (int)math.floor(damage0),
                                    },
                                    criticalRate = 0,
                                    criticalDamage = 0,
                                    hitRate = 1,
                                    degree = 0,
                                    pos = 0,

                                    bulletEntity = default,
                                });
                            }
                        }
                    }
                    else if (cdfeEnemyData.HasComponent(otherEntity))
                    {
                        UnityHelper.TryCreateAudioClip(globalConfigData, gameOthersData, 10002, out var _);
                        //怪物A碰撞怪物B

                        var otherPhysicsVelocity = cdfePhysicsVelocity[otherEntity].Linear;
                        var ePhysicsVelocity = cdfePhysicsVelocity[e].Linear;


                        var degree = math.degrees(MathHelper.Angle(otherPhysicsVelocity, ePhysicsVelocity));
                        degree = math.max(0, degree);
                        degree = degree > 90 ? 180 - degree : degree;
                        var eSpeedArgs = math.abs(math.length(ePhysicsVelocity) +
                                                  math.length(otherPhysicsVelocity) * math.cos(degree)) * 1000f;

                        if (cdfeEnemyData.HasComponent(e))
                        {
                            hitBackData.hitTimes--;

                            var damage = math.max(cdfeChaStats[player].chaProperty.atk *
                                                  ((1 + cdfeChaStats[player].chaProperty
                                                       .damageRatios / 10000f) *
                                                   (1 - cdfeChaStats[e].chaProperty
                                                       .reduceHurtRatios / 10000f) *
                                                   (1 + cdfeChaStats[player].chaProperty.collideDamageRatios / 10000f +
                                                    cdfeChaStats[player].chaProperty.continuousCollideDamageRatios /
                                                    10000f * cdfeChaStats[e].chaResource.continuousCollCount) +
                                                   (cdfeChaStats[player].chaProperty.damageAdd -
                                                    cdfeChaStats[e].chaProperty.reduceHurtAdd)),
                                             battle_impact_damage_min) * eSpeedArgs *
                                         (battle_monster_force_factor_value / 1000000f);


                            ecb.AppendToBuffer(index, wbe, new DamageInfo
                            {
                                attacker = player,
                                defender = e,
                                tags = new DamageInfoTag
                                {
                                    collideDamage = true
                                },
                                damage = new Damage
                                {
                                    normal = (int)math.floor(damage),
                                },
                                criticalRate = cdfeChaStats[player]
                                                   .chaProperty.critical /
                                               10000f,
                                criticalDamage = cdfeChaStats[player]
                                                     .chaProperty.criticalDamageRatios /
                                                 10000f,
                                hitRate = 1,
                                degree = 0,

                                pos = 0,
                                bulletEntity = default,
                                disableDamageNumber = false,
                            });

                            var otherSpeedArgs = math.abs(math.length(otherPhysicsVelocity) +
                                                          math.length(ePhysicsVelocity) * math.cos(degree)) * 1000f;
                            var damage0 = math.max(cdfeChaStats[player].chaProperty.atk *
                                                   ((1 + cdfeChaStats[player].chaProperty
                                                        .damageRatios / 10000f) *
                                                    (1 - cdfeChaStats[otherEntity].chaProperty
                                                        .reduceHurtRatios / 10000f) *
                                                    (1 + cdfeChaStats[player].chaProperty.collideDamageRatios / 10000f +
                                                     cdfeChaStats[player].chaProperty.continuousCollideDamageRatios /
                                                     10000f *
                                                     cdfeChaStats[otherEntity].chaResource.continuousCollCount) +
                                                    (cdfeChaStats[player].chaProperty.damageAdd -
                                                     cdfeChaStats[otherEntity].chaProperty.reduceHurtAdd)),
                                              battle_impact_damage_min) * otherSpeedArgs *
                                          (battle_monster_force_factor_value / 1000000f);
                            Debug.Log(
                                $"A速度{ePhysicsVelocity} B速度{otherPhysicsVelocity} degree{degree} damageA{damage} damageB{damage0}");

                            ecb.AppendToBuffer(index, wbe, new DamageInfo
                            {
                                attacker = player,
                                defender = otherEntity,
                                tags = new DamageInfoTag
                                {
                                    collideDamage = true
                                },
                                damage = new Damage
                                {
                                    normal = (int)math.floor(damage0),
                                },
                                criticalRate = cdfeChaStats[player]
                                                   .chaProperty.critical /
                                               10000f,
                                criticalDamage = +cdfeChaStats[player]
                                                     .chaProperty.criticalDamageRatios /
                                                 10000f,
                                hitRate = 1,
                                degree = 0,
                                pos = 0,
                                bulletEntity = default,
                            });
                            
                            // ecb.AddComponent(index, otherEntity, new HitBackData
                            // {
                            //     id = 0,
                            //     hitTimes = battle_continuous_collision_max_num,
                            //     attacker = player
                            // });
                            //Debug.LogError($"damage {damage}damage0 {damage0}");
                        }
                    }
                }
            }
        }
    }
}