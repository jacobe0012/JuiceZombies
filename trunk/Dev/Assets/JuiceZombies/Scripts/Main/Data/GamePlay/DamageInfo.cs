using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Main
{
    //[InternalBufferCapacity(0)]
    public struct DamageInfo : IBufferElementData
    {
        ///<summary>
        ///造成伤害的攻击者，nullable
        ///</summary>
        public Entity attacker;

        ///<summary>
        ///造成攻击伤害的受击者，这个必须有
        ///</summary>
        public Entity defender;

        ///<summary>
        ///这次伤害的类型Tag，这个会被用于buff相关的逻辑，是一个极其重要的信息
        ///这里是策划根据游戏设计来定义的，比如游戏中可能存在"frozen" "fire"之类的伤害类型，还会存在"directDamage" "period" "reflect"之类的类型伤害
        ///根据这些伤害类型，逻辑处理可能会有所不同，典型的比如"reflect"，来自反伤的，那本身一个buff的作用就是受到伤害的时候反弹伤害，如果双方都有这个buff
        ///并且这个buff没有判断damageInfo.tags里面有reflect，则可能造成“短路”，最终有一下有一方就秒了。
        ///</summary>
        public DamageInfoTag tags;

        ///<summary>
        ///伤害值，其实伤害值是多元的，通常游戏都会有多个属性伤害，所以会用一个struct，否则就会是一个int
        ///尽管起名叫Damage，但实际上治疗也是这个，只是负数叫做治疗量，这个起名看似不严谨，对于游戏（这个特殊的业务）而言却又是严谨的
        ///</summary>
        public Damage damage;

        ///<summary>
        ///是否暴击，这是游戏设计了有暴击的可能性存在。
        ///这里记录一个总暴击率，随着buff的不断改写，最后这个暴击率会得到一个0-1的值，代表0%-100%。
        ///最终处理的时候，会根据这个值来进行抉择，可以理解为，当这个值超过1的时候，buff就可以认为这次攻击暴击了。
        ///</summary>
        public float criticalRate;

        ///<summary>
        ///暴击伤害
        ///</summary>
        public float criticalDamage;

        ///<summary>
        ///是否命中，是否命中与是否暴击并不直接相关，都是单独的算法
        ///要不要这个属性还是取决于游戏设计，比如当前游戏，本不该有这个属性。
        ///</summary>
        public float hitRate;

        ///<summary>
        ///伤害的角度，作为伤害打向角色的入射角度，比如子弹，就是它当前的飞行角度
        ///</summary>
        public float degree;

        ///<summary>
        ///造成伤害的pos
        ///</summary>
        public float3 pos;

        ///<summary>
        ///造成伤害的弹幕
        ///</summary>
        public Entity bulletEntity;


        ///<summary>
        ///关闭伤害数字显示
        ///</summary>
        public bool disableDamageNumber;

        public void SumDamage(ref DamageData_ReadWrite refData, in DamageData_ReadOnly inData, out bool isCritical)
        {
            bool isPlayerDefender = refData.storageInfoFromEntity.Exists(defender) &&
                                    refData.cdfePlayerData.HasComponent(defender);
            bool isPlayerAttacker = refData.storageInfoFromEntity.Exists(attacker) &&
                                    refData.cdfePlayerData.HasComponent(attacker);
            bool isObstDefender = refData.storageInfoFromEntity.Exists(defender) &&
                                  inData.cdfeObstacleTag.HasComponent(defender);

            disableDamageNumber = (!isPlayerDefender && !isPlayerAttacker) || isObstDefender;

            var defenderChaStats = refData.cdfeChaStats[defender];

            bool hasAttacker = refData.storageInfoFromEntity.Exists(attacker);
            if (hasAttacker && refData.cdfeChaStats.HasComponent(attacker))
            {
                //Debug.LogError($"临时暴击率{refData.cdfeChaStats[attacker].chaProperty.tmpCritical / 10000f}");
                criticalRate += (refData.cdfeChaStats[attacker].chaProperty.tmpCritical / 10000f);
            }

            if (hasAttacker && refData.cdfePlayerData.HasComponent(attacker) &&
                inData.cdfeTargetData.HasComponent(defender))
            {
                var playerData = refData.cdfePlayerData[attacker];

                switch (inData.cdfeTargetData[defender].BelongsTo)
                {
                    case (uint)BuffHelper.TargetEnum.LittleMonster:
                        damage.RatiosDamage(playerData.playerData.normalMonsterDamageRatios / 10000f);
                        break;
                    case (uint)BuffHelper.TargetEnum.ElliteMonster:
                        damage.RatiosDamage(playerData.playerData.specialMonsterDamageRatios / 10000f);
                        break;
                    case (uint)BuffHelper.TargetEnum.BossMonster:
                        damage.RatiosDamage(playerData.playerData.bossMonsterDamageRatios / 10000f);
                        break;
                }
            }


            isCritical = false;
            long finalDamage = 0;
            //tags.seckillDamage = true;
            if (tags.seckillDamage)
            {
                Debug.Log("秒杀");
                damage.normal = MathHelper.MaxNum;
                return;
            }

            if ((tags.directDamage || tags.weaponDamage) && !tags.ratiosDamage)
            {
                isCritical = true;
                criticalRate *= 100;
                if (criticalRate < 100)
                {
                    isCritical = refData.rand.NextInt(0, 101) < criticalRate;
                    if (isCritical)
                    {
                        Debug.Log($"暴击");
                        if (hasAttacker && refData.cdfeChaStats.HasComponent(attacker))
                        {
                            var temp = refData.cdfeChaStats[attacker];
                            temp.chaProperty.tmpCritical = 0;
                            refData.cdfeChaStats[attacker] = temp;
                        }
                    }
                }
            }

            // if (isClean)
            // {
            //     return (int)((damage.normal + damage.bullet +
            //                   damage.collide) * criticalDamage);
            // }

            // if (damage.collide > 0)
            // {
            //     ref var constantsConfig = ref inData.globalConfigData.value.Value.configTbconstants.configTbconstants;
            //     ref var map_elementsConfig =
            //         ref inData.globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
            //
            //
            //     int battle_obstacle_speed_value = 0;
            //     int battle_obstacle_damage_value = 0;
            //     int battle_impact_damage_factor = 0;
            //
            //     int battle_impact_damage_min = 0;
            //     int battle_monster_force_factor_value = 0;
            //
            //     for (int j = 0; j < constantsConfig.Length; j++)
            //     {
            //         if (constantsConfig[j].constantName == (FixedString128Bytes)"battle_obstacle_speed_value")
            //         {
            //             battle_obstacle_speed_value = constantsConfig[j].constantValue;
            //         }
            //
            //         if (constantsConfig[j].constantName == (FixedString128Bytes)"battle_obstacle_damage_value")
            //         {
            //             battle_obstacle_damage_value = constantsConfig[j].constantValue;
            //         }
            //
            //         if (constantsConfig[j].constantName == (FixedString128Bytes)"battle_impact_damage_factor")
            //         {
            //             battle_impact_damage_factor = constantsConfig[j].constantValue;
            //         }
            //
            //         if (constantsConfig[j].constantName == (FixedString128Bytes)"battle_impact_damage_min")
            //         {
            //             battle_impact_damage_min = constantsConfig[j].constantValue;
            //         }
            //
            //         if (constantsConfig[j].constantName == (FixedString128Bytes)"battle_monster_force_factor_value")
            //         {
            //             battle_monster_force_factor_value = constantsConfig[j].constantValue;
            //         }
            //     }
            //
            //     //怪物A碰撞障碍物    A.受到伤害 //TODO:
            //     if (inData.cdfeEnemyData.HasComponent(defender) && inData.cdfeObstacleTag.HasComponent(realEntity))
            //     {
            //         for (int i = 0; i < map_elementsConfig.Length; i++)
            //         {
            //             if (inData.cdfeElementData[realEntity].elementID == map_elementsConfig[i].id)
            //             {
            //                 // math.max(damage.collide * (isCritical ? criticalDamage : 1), battle_impact_damage_min) *
            //                 //     collSpeed * (battle_monster_force_factor_value / 10000f);
            //                 finalDamage = (int)(math.max(damage.collide * (isCritical ? criticalDamage : 1),
            //                                         battle_impact_damage_min) *
            //                                     collSpeed * (map_elementsConfig[i].hitDamageRatio / 10000f));
            //
            //
            //                 //finalDamage = math.min(dam, obstaclesConfig[i].obstacleHurtMax);
            //                 return (int)math.floor(finalDamage);
            //             }
            //         }
            //     }
            //
            //
            //     finalDamage = (int)(math.max(damage.collide * (isCritical ? criticalDamage : 1),
            //                             battle_impact_damage_min) *
            //                         collSpeed * (battle_monster_force_factor_value / 10000f));
            //     //Debug.Log($"{finalDamage}");
            //     return (int)math.floor(finalDamage);
            // }
            //Debug.Log($"bullet{damage.bullet}normal{damage.normal}collide{damage.collide}");


            if (tags.environmentDamage)
            {
                finalDamage = (long)math.floor(
                    damage.normal * (1 - defenderChaStats.chaProperty.reduceHurtRatios / 10000f) -
                    defenderChaStats.chaProperty.reduceHurtAdd);
            }
            else
            {
                finalDamage = isCritical
                    ? (long)math.floor(damage.SumDamage() * criticalDamage)
                    : (long)math.floor(damage.SumDamage());
            }

            if (!tags.directHeal)
            {
                finalDamage = math.max(finalDamage, 1);
            }
            else if (tags.propsHeal)
            {
                if (refData.cdfePlayerData.HasComponent(defender))
                {
                    var defenderPlayerData = refData.cdfePlayerData[defender];
                    finalDamage = (long)math.floor(
                        finalDamage * (1 + defenderChaStats.chaProperty.hpRecoveryRatios / 10000f +
                                       defenderPlayerData.playerData.propsRecoveryRatios / 10000f) +
                        defenderChaStats.chaProperty.hpRecoveryAdd + defenderPlayerData.playerData.propsRecoveryAdd);
                }
                else
                {
                    finalDamage = (long)math.floor(
                        finalDamage * (1 + defenderChaStats.chaProperty.hpRecoveryRatios / 10000f) -
                        defenderChaStats.chaProperty.hpRecoveryAdd);
                }

                finalDamage = -math.abs(finalDamage);
            }
            else
            {
                finalDamage = (long)math.floor(
                    finalDamage * (1 + defenderChaStats.chaProperty.hpRecoveryRatios / 10000f) -
                    defenderChaStats.chaProperty.hpRecoveryAdd);
                finalDamage = -math.abs(finalDamage);
            }

            if (inData.cdfeEnemyData.HasComponent(defender) && refData.cdfePlayerData.HasComponent(attacker))
            {
                if (!tags.directHeal)
                {
                    var playerData = refData.cdfePlayerData[inData.player];
                    //新手引导 首次触发伤害
                    int type = 323;
                    if (playerData.playerOtherData.guideList.Contains(type))
                    {
                        var e = refData.ecb.CreateEntity(inData.sortkey);

                        refData.ecb.AddComponent(inData.sortkey, e, new HybridEventData
                        {
                            type = 999000 + type
                        });
                        refData.ecb.AddComponent(inData.sortkey, e, new TimeToDieData
                        {
                            duration = 5
                        });
                        playerData.playerOtherData.guideList.Remove(type);
                        refData.cdfePlayerData[inData.player] = playerData;
                    }
                }
            }

            // if (refData.cdfePlayerData.HasComponent(defender))
            // {
            //     if (!tags.directHeal)
            //     {
            //         if (tags.collideDamage)
            //         {
            //             var playerData = refData.cdfePlayerData[inData.player];
            //             //新手引导 3  首次被怪物碰撞
            //             int id = 10003;
            //             if (playerData.playerOtherData.guideList.Contains(id))
            //             {
            //                 var e = refData.ecb.CreateEntity(inData.sortkey);
            //
            //                 refData.ecb.AddComponent(inData.sortkey, e, new HybridEventData
            //                 {
            //                     type = 99903
            //                 });
            //                 refData.ecb.AddComponent(inData.sortkey, e, new TimeToDieData
            //                 {
            //                     duration = 5
            //                 });
            //                 playerData.playerOtherData.guideList.Remove(id);
            //                 refData.cdfePlayerData[inData.player] = playerData;
            //             }
            //         }
            //     }
            // }

            //伤害统计

            if (!refData.cdfePlayerData.HasComponent(defender))
            {
                if (refData.storageInfoFromEntity.Exists(attacker))
                {
                    if (refData.cdfePlayerData.HasComponent(attacker))
                    {
                        if (!tags.directHeal)
                        {
                            var playerData = refData.cdfePlayerData[inData.player];
                            if (tags.weaponDamage)
                            {
                                playerData.playerOtherData.playerDamageSumInfo.weaponDamage += finalDamage;
                            }
                            else if (tags.collideDamage)
                            {
                                playerData.playerOtherData.playerDamageSumInfo.collideDamage += finalDamage;
                            }
                            else if (tags.environmentDamage)
                            {
                                playerData.playerOtherData.playerDamageSumInfo.areaDamage += finalDamage;
                            }


                            refData.cdfePlayerData[inData.player] = playerData;
                        }
                    }
                }
                else
                {
                    if (!tags.directHeal)
                    {
                        var playerData = refData.cdfePlayerData[inData.player];
                        if (tags.weaponDamage)
                        {
                            playerData.playerOtherData.playerDamageSumInfo.weaponDamage += finalDamage;
                        }
                        else if (tags.collideDamage)
                        {
                            playerData.playerOtherData.playerDamageSumInfo.collideDamage += finalDamage;
                        }
                        else if (tags.environmentDamage)
                        {
                            playerData.playerOtherData.playerDamageSumInfo.areaDamage += finalDamage;
                        }

                        refData.cdfePlayerData[inData.player] = playerData;
                    }
                }
            }

            if (inData.cdfeEnemyData.HasComponent(defender))
            {
                if (!tags.directHeal)
                {
                    if (refData.storageInfoFromEntity.Exists(attacker))
                    {
                        var playerData = refData.cdfePlayerData[inData.player];
                        if (tags.collideDamage)
                        {
                            playerData.playerOtherData.playerDamageSumInfo.enemy2obstan += finalDamage;
                        }

                        // Debug.Log(
                        //     $"collider->enemy2obstan:{playerData.playerOtherData.playerDamageSumInfo.enemy2obstan}");
                        refData.cdfePlayerData[inData.player] = playerData;
                    }
                    else
                    {
                        var playerData = refData.cdfePlayerData[inData.player];
                        if (tags.collideDamage)
                        {
                            playerData.playerOtherData.playerDamageSumInfo.enemy2enemy += finalDamage;
                        }

                        // Debug.Log(
                        //     $"collider->enemy2enemy:{playerData.playerOtherData.playerDamageSumInfo.enemy2enemy}");
                        refData.cdfePlayerData[inData.player] = playerData;
                    }
                }
            }

            damage.normal = finalDamage;
            //return finalDamage;
        }
    }

    public struct DamageData_ReadWrite
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public EntityStorageInfoLookup storageInfoFromEntity;
        public Random rand;
        public ComponentLookup<PlayerData> cdfePlayerData;
        public ComponentLookup<ChaStats> cdfeChaStats;
    }

    public struct DamageData_ReadOnly
    {
        public int sortkey;
        public uint seed;
        public Entity player;
        public Damage beforeDamage;
        public BufferLookup<LinkedEntityGroup> bfeLinkedEntityGroup;
        public PrefabMapData prefabMapData;
        public GlobalConfigData globalConfigData;
        public ComponentLookup<MapElementData> cdfeElementData;

        public ComponentLookup<EnemyData> cdfeEnemyData;
        public ComponentLookup<TargetData> cdfeTargetData;
        public ComponentLookup<ObstacleTag> cdfeObstacleTag;
        public ComponentLookup<LocalTransform> cdfeLocalTransform;
    }

    /// <summary>
    /// 伤害类型
    /// </summary>
    public struct Damage
    {
        //普通伤害
        public long normal;

        // //弹幕伤害
        // public long bullet;
        //
        // //碰撞伤害
        // public long collide;
        //
        // //环境伤害
        // public long environment;
        //
        // //武器伤害
        // public long weaponDamage;

        //public int sumDamage;

        public long SumDamage()
        {
            //return normal + bullet + collide + environment + weaponDamage;
            return normal;
        }

        /// <summary>
        /// 增加百分比伤害 乘算
        /// </summary>
        /// <param name="ratios"></param>
        /// <returns></returns>
        public void RatiosDamage(float ratios)
        {
            normal = (long)(normal * (1 + ratios));
            // bullet = (long)(bullet * (1 + ratios));
            // collide = (long)(collide * (1 + ratios));
            // environment = (long)(environment * (1 + ratios));
            // weaponDamage = (long)(weaponDamage * (1 + ratios));
        }

        /// <summary>
        /// 增加固定伤害  加算
        /// </summary>
        /// <param name="ratios"></param>
        /// <returns></returns>
        public void AddDamage(float add)
        {
            normal = (long)(normal + add);
            // bullet = (long)(bullet + add);
            // collide = (long)(collide + add);
            // environment = (long)(environment + add);
            // weaponDamage = (long)(weaponDamage + add);
        }
        // public int explosion;
        // public int fire;
        // public int freeze;
        // public int thunder;
        // public int water;
        // public int poison;
        // public int enemy;
    }

    public struct DamageInfoTag
    {
        /// <summary>
        /// 直接伤害 
        /// </summary>
        public bool directDamage; //直接伤害 

        /// <summary>
        /// 秒杀伤害 直接秒杀
        /// </summary>
        public bool seckillDamage; //

        /// <summary>
        /// 直接治疗 
        /// </summary>
        public bool directHeal; //

        /// <summary>
        /// 实际的恢复值=道具恢复值*（1+道具恢复加成+生命恢复加成）+道具恢复固定加成+生命恢复固定加成
        /// </summary>
        public bool propsHeal; //道具治疗 

        /// <summary>
        /// 直接治疗 
        /// </summary>
        public bool bulletDamage; //直接治疗 

        /// <summary>
        /// 碰撞治疗 
        /// </summary>
        public bool collideDamage; //直接治疗 

        /// <summary>
        /// 环境治疗
        /// </summary>
        public bool environmentDamage; //直接治疗

        /// <summary>
        /// 武器伤害
        /// </summary>
        public bool weaponDamage;

        /// <summary>
        /// 万分比伤害
        /// </summary>
        public bool ratiosDamage; //

        //public bool periodHeal; //间歇性治疗
        //public bool periodDamage; //间歇性伤害 
        // public bool reflectDamage; //反弹伤害 反弹收到的伤害
        // public bool copyDamage; //复制伤害 一帧造成两次伤害
        public void SetAllTagsFalse()
        {
            directDamage = false;
            seckillDamage = false;
            directHeal = false;
            propsHeal = false;
            bulletDamage = false;
            collideDamage = false;
            environmentDamage = false;
            weaponDamage = false;
        }

        public bool IsHeal()
        {
            return directHeal || propsHeal;
        }
    }


    public struct DamageUnManagedData : IComponentData
    {
        public bool isDestory;

        /// <summary>
        /// x:isHit  y:isCritcal  z:number
        /// </summary>
        public float2x4 damage;

        public Color32 color;
        // damage = new float2x4
        // {
        //     c0 = new int2(),x:类型id  y:伤害值
        //     c1 = new int2(),x:isK    y:isM
        //     c2 = new int2(),x:0       y:eT
        //     c3 = new int2(), x:0      y:0
        // }
        // public Damage damage;
        // public DamageInfoTag damageTag;
    }
}