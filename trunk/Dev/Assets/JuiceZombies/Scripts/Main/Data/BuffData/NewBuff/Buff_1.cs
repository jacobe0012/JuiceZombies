//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-17 12:15:10
//---------------------------------------------------------------------

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    //输出元素 包含 伤害 推力 治疗
    //new 
    public partial struct Buff_1 : IBuff
    {
        ///<summary>0
        ///buff的id
        ///</summary>
        public int id;

        ///<summary>1
        ///buff添加时间点/刻 替换优先级
        ///</summary>
        public float startTime;

        ///<summary>2
        ///buff已经存在了多少时间了，单位：帧
        ///</summary>
        public int timeElapsed;

        ///<summary>3
        ///剩余多久，单位：秒
        ///</summary>
        public float duration;

        ///<summary>4
        ///是否是一个永久的buff，永久的duration不会减少，但是timeElapsed还会增加
        ///</summary>
        public bool permanent;

        ///<summary>5
        ///buff的施法者是谁，当然可以是空的
        ///</summary>
        public Entity caster;

        ///<summary>6
        ///buff的携带者，实际上是作为参数传递给脚本用，具体是谁，可定是所在控件的this.gameObject了
        ///</summary>
        public Entity carrier;

        /// <summary>7
        /// 替换类型 用于同类buff的替换 仅用于免疫和控制和属性变更 为属性变更时 x是类型 y是上限
        /// </summary>
        public int2 changeType;

        /// <summary>8
        /// 是否能够清除 用于不同类buff的替换 仅用于控制类
        /// new字段
        /// </summary>
        public int canClear;

        /// <summary>9
        /// 护盾层数 仅作用于免疫
        /// new
        /// </summary>
        public int immuneStack;

        /// <summary>10
        /// 从 effect传入的的方向
        /// new
        /// </summary>
        public float3 direction;

        /// <summary>11
        /// buff的状态类型 分无状态 正面状态和负面状态
        /// </summary>
        public int buffState;

        /// <summary>12
        /// float4-1:output 2:bonus_type 3:calc_type and pra
        /// </summary>
        public BuffArgsNew argsNew;

        ///<summary>13
        ///buff的优先级，优先级越低的buff越后面执行，这是一个非常重要的属性
        ///比如经典的“吸收50点伤害”和“受到的伤害100%反弹给攻击者”应该反弹多少，取决于这两个buff的priority谁更高
        ///</summary>
        public int priority;

        /// <summary>14
        /// 元素的概率 
        /// </summary>
        public int power;

        /// <summary>15
        /// 用来处理ontick的逻辑 =0时不执行ontick 
        /// </summary>
        public int tickTime;

        /// <summary>16
        /// 用来处理移动伤害的逻辑  第一个float3的x为每移动多少米 y为当前移动总距离 z为1时该移动伤害才生效！！！！
        /// 第二个记录上帧的位置 
        /// </summary>
        public float3x2 distancePar;

        /// <summary>17
        /// 是否是弹幕
        /// </summary>
        public bool isBullet;

        /// <summary>
        ///  18 buff2里上一份属性变更的值
        /// </summary>
        public int oldValue;

        /// <summary>
        /// 19 是否禁用特效
        /// </summary>
        public bool isDisableSpecial;

        /// <summary>
        /// 20 技能id
        /// </summary>
        public int skillId;

        //public FixedList64Bytes<int> skillList;

        public void OnOccur(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(inData.timeTick + caster.Index +
                                                                       caster.Version));
            var dropRate = math.clamp(power, 0, 10000);
            //Debug.LogError($"Buff_1 power:{dropRate}");
            var canTrigger = rand.NextInt(0, 10001) <= dropRate;
            if (!canTrigger)
                return;

            ref var skillConfig =
                ref inData.globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            int skillID = 0;
            for (int i = 0; i < skillConfig.Length; i++)
            {
                if (skillConfig[i].elementList.Length > 0)
                {
                    ref var elements = ref skillConfig[i].elementList;
                    for (int j = 0; j < elements.Length; j++)
                    {
                        if (elements[j] == id)
                        {
                            skillID = skillConfig[i].skillId;
                            break;
                        }
                    }
                }
            }

            //Debug.LogError($"Buff_1 OnOccur,{skillID}");
            var damageInfoTag = new DamageInfoTag
            {
                directDamage = false,
                seckillDamage = false,
                directHeal = false,
            };
            var damage = new Damage
            {
                normal = 0,
            };


            ref var constConfig =
                ref inData.globalConfigData.value.Value.configTbbattle_constants.configTbbattle_constants;

            int battle_force_factor = 0;
            int battle_continuous_collision_max_num = 0;

            for (int j = 0; j < constConfig.Length; j++)
            {
                if (constConfig[j].constantName == (FixedString128Bytes)"battle_force_factor")
                {
                    battle_force_factor = constConfig[j].constantValue;
                }

                if (constConfig[j].constantName == (FixedString128Bytes)"battle_continuous_collision_max_num")
                {
                    battle_continuous_collision_max_num = constConfig[j].constantValue;
                }
            }

            //TODO:常量 待定
            const float AdditionConst = 1;
            int output_type = argsNew.args1.x;
            int output_type_para1 = argsNew.args1.y;
            Debug.Log($"output_type_para1:{output_type_para1}");
            //output_type_para1 = 50000;
            int4 bonus_type = argsNew.args2;
            int4 calc_type = argsNew.args3;

            bool hasVelocity = refData.cdfePhysicsVolocity.HasComponent(carrier);
            bool carrierHasChaStats = refData.cdfeChaStats.HasComponent(carrier);
            bool casterHasChaStats = refData.cdfeChaStats.HasComponent(caster);

            if (!casterHasChaStats || !carrierHasChaStats)
            {
                return;
            }

            if (refData.cdfeChaStats[inData.entity].chaImmuneState.immuneDamage && output_type == 0)
                return;

            if (refData.cdfeChaStats[inData.entity].chaImmuneState.immunePush && output_type == 1)
                return;

            //Debug.LogError($"caster {caster.Index} carrier {carrier.Index}");
            //TODO:障碍物生成的弹幕伤害 需要读表固定值


            var casterChaStats = refData.cdfeChaStats[caster];
            var carrierChaStats = refData.cdfeChaStats[carrier];


            //TODO:
            //Debug.LogError($"cdfeMapElementDatacdfeMapElementDatacdfeMapElementData");


            int tempType = output_type * 10 + calc_type.x;
            //Debug.LogError($"output_type{output_type} calc_type{calc_type.x} tempType{tempType}");
            //Debug.LogError($"args1 {argsNew.args1} args2 {argsNew.args2} args3 {argsNew.args3}");
            float dirPushforce = 0;
            float atk = 0;

            //Debug.LogError($"output_type{output_type} calc_type{calc_type.x} tempType{tempType}");
            switch (tempType)
            {
                //伤害 攻击力万分比
                case 00:
                    damageInfoTag.directDamage = true;
                    atk = (casterChaStats.chaProperty.atk * (calc_type.y / 10000f));
                    break;
                //伤害 推力万分比
                case 01:
                    damageInfoTag.directDamage = true;
                    atk = (casterChaStats.chaProperty.pushForce * (calc_type.y / 10000f));
                    break;
                //伤害 生命值万分比
                case 02:
                    damageInfoTag.directDamage = true;
                    if (calc_type.z == 0)
                    {
                        calc_type.z = int.MaxValue;
                    }

                    atk = (carrierChaStats.chaProperty.maxHp * (calc_type.y / 10000f)) > calc_type.z
                        ? calc_type.z
                        : (carrierChaStats.chaProperty.maxHp * (calc_type.y / 10000f));
                    break;
                //伤害 固定值
                case 03:
                    damageInfoTag.directDamage = true;
                    atk = calc_type.y;
                    break;
                //推力 攻击力万分比
                case 10:

                    dirPushforce = (casterChaStats.chaProperty.atk * (calc_type.y / 10000f));
                    break;
                //推力 推力万分比
                case 11:
                    dirPushforce = (casterChaStats.chaProperty.pushForce * (calc_type.y / 10000f));
                    break;
                //推力 生命值万分比
                case 12:
                    dirPushforce = (carrierChaStats.chaProperty.maxHp * (calc_type.y / 10000f)) > calc_type.z
                        ? calc_type.z
                        : (carrierChaStats.chaProperty.maxHp * calc_type.y / 10000f);
                    break;
                //推力 固定值
                case 13:

                    dirPushforce = calc_type.y;
                    break;

                //治疗 攻击力万分比
                case 20:
                    damageInfoTag.directHeal = true;
                    atk = -(casterChaStats.chaProperty.atk * (calc_type.y / 10000f));
                    break;
                //治疗 推力万分比
                case 21:
                    damageInfoTag.directHeal = true;
                    atk = -(casterChaStats.chaProperty.pushForce * (calc_type.y / 10000f));
                    break;
                //治疗 生命值万分比
                case 22:
                    damageInfoTag.directHeal = true;
                    atk = -(carrierChaStats.chaProperty.maxHp * (calc_type.y / 10000f));
                    break;
                //治疗 固定值
                case 23:
                    damageInfoTag.directHeal = true;
                    atk = -calc_type.y;
                    break;
            }

            switch (bonus_type.x)
            {
                case 0:
                    break;
                case 1:
                    atk *= (1 + (casterChaStats.chaProperty.pushForceRatios / 10000f) *
                        AdditionConst * (bonus_type.y / 10000f));
                    dirPushforce *= (1 + casterChaStats.chaProperty.pushForceRatios *
                        AdditionConst * (bonus_type.y / 10000f));
                    break;
                case 2:
                    atk *= (1 + (casterChaStats.chaProperty.atkRatios / 10000f) *
                        AdditionConst * (bonus_type.y / 10000f));
                    dirPushforce *= (1 + (casterChaStats.chaProperty.atkRatios / 10000f) *
                        AdditionConst * (bonus_type.y / 10000f));
                    break;
                case 3:
                    // damage *= (1 + casterChaStats.chaProperty.skillRangeRatios *
                    //     AdditionConst * bonus_type.y / 10000f);
                    //
                    // dirPushforce *= (1 + casterChaStats.chaProperty.skillRangeRatios *
                    //     AdditionConst * bonus_type.y / 10000f);
                    break;
                case 4:
                    atk *= (1 + (casterChaStats.chaProperty.hpRatios / 10000f) *
                        AdditionConst * (bonus_type.y / 10000f));

                    dirPushforce *= (1 + (casterChaStats.chaProperty.hpRatios / 10000f) *
                        AdditionConst * (bonus_type.y / 10000f));
                    break;
                case 5:
                    atk *= (1 + (casterChaStats.chaProperty.maxMoveSpeedRatios / 10000f) *
                        AdditionConst * (bonus_type.y / 10000f));

                    dirPushforce *= (1 + (casterChaStats.chaProperty.maxMoveSpeedRatios / 10000f) *
                        AdditionConst * (bonus_type.y / 10000f));
                    break;
                case 6:
                    atk *= (1 + (casterChaStats.chaProperty.massRatios / 10000f) *
                        AdditionConst * bonus_type.y / 10000f);

                    dirPushforce *= (1 + (casterChaStats.chaProperty.massRatios / 10000f) *
                        AdditionConst * bonus_type.y / 10000f);
                    break;
                case 7:
                    var currentVeloctiy = math.length(refData.cdfePhysicsVolocity[carrier].Linear);
                    atk *= (1 + currentVeloctiy * (bonus_type.y / 10000f));
                    dirPushforce *= (1 + currentVeloctiy * (bonus_type.y / 10000f));
                    break;
            }


            dirPushforce *= (1 + AddtionOtherPar(inData));
            atk *= (1 + AddtionOtherPar(inData));

            //推力
            if (tempType >= 10 && tempType <= 13)
            {
                //Debug.LogError($" {tempType}");
                if (hasVelocity)
                {
                    //TODO:障碍物可被推
                    if (!inData.cdfeObstacleTag.HasComponent(carrier))
                    {
                        if (!inData.cdfeHitBackData.HasComponent(carrier) && !carrierChaStats.chaImmuneState.immunePush)
                        {
                            //var carrierChaStats = refData.cdfeChaStats[carrier];
                            var temp = refData.cdfePhysicsVolocity[carrier];
                            dirPushforce = (dirPushforce *
                                (1 - carrierChaStats.chaProperty.reduceHitBackRatios / 10000f) /
                                carrierChaStats.chaProperty.mass * ((battle_force_factor / 10000f) / 100f));
                            Debug.Log($"origin dirPushforce {dirPushforce}");
                            int battle_super_force = 0;
                            int battle_bounds_force = 0;
                            ref var constantConfig =
                                ref inData.globalConfigData.value.Value.configTbbattle_constants
                                    .configTbbattle_constants;
                            for (int i = 0; i < constantConfig.Length; i++)
                            {
                                if (constantConfig[i].constantName == (FixedString128Bytes)"battle_super_force")
                                {
                                    battle_super_force = constantConfig[i].constantValue;
                                }

                                if (constantConfig[i].constantName == (FixedString128Bytes)"battle_bounds_force")
                                {
                                    battle_bounds_force = constantConfig[i].constantValue;
                                }
                            }

                            if (refData.cdfePlayerData.HasComponent(caster))
                            {
                                var playerData = refData.cdfePlayerData[caster];
                                bool maxPushForce = rand.NextInt(0, 10001) <=
                                                    playerData.playerData.maxPushForceChance;

                                if (maxPushForce)
                                {
                                    dirPushforce *= (1 + battle_bounds_force / 10000f);

                                    //震屏
                                    var ins = refData.ecb.CreateEntity(inData.sortkey);
                                    refData.ecb.AddComponent(inData.sortkey, ins, new HybridEventData
                                    {
                                        type = 45,
                                        args = new float4(500),
                                    });
                                    refData.ecb.AddComponent(inData.sortkey, ins, new TimeToDieData
                                    {
                                        duration = 5f
                                    });
                                }
                                else
                                {
                                    bool superPushForce = rand.NextInt(0, 10001) <=
                                                          playerData.playerData.superPushForceChance;

                                    if (superPushForce)
                                    {
                                        dirPushforce *= (1 + battle_super_force / 10000f);

                                        //震屏
                                        var ins = refData.ecb.CreateEntity(inData.sortkey);
                                        refData.ecb.AddComponent(inData.sortkey, ins, new HybridEventData
                                        {
                                            type = 45,
                                            args = new float4(200),
                                        });
                                        refData.ecb.AddComponent(inData.sortkey, ins, new TimeToDieData
                                        {
                                            duration = 5f
                                        });
                                    }
                                }
                            }

                            //Debug.Log($"dirPushforce {dirPushforce}");
                            direction.z = 0;
                            direction = math.normalizesafe(direction);
                            var hitback = new HitBackData
                            {
                                id = id,
                                hitTimes = battle_continuous_collision_max_num,
                                attacker = caster,
                                dirPushforce = default
                            };
                            Debug.Log($"dirPushforce {dirPushforce}");
                            float3 addPushForce = math.clamp(dirPushforce, 0, math.abs(dirPushforce)) * direction;
                            if (refData.cdfePlayerData.HasComponent(carrier))
                            {
                                hitback.dirPushforce = addPushForce;
                            }
                            else
                            {
                                temp.Linear += addPushForce;
                            }

                            refData.cdfePhysicsVolocity[carrier] = temp;
                            refData.ecb.AddComponent(inData.sortkey, carrier, hitback);
                            refData.ecb.AddComponent<GetHitAnimData>(inData.sortkey, carrier);
                        }
                    }
                    else
                    {
                        if (output_type_para1 != 0)
                        {
                            dirPushforce *= output_type_para1 / 10000f;
                            if (!inData.cdfeHitBackData.HasComponent(carrier) &&
                                !carrierChaStats.chaImmuneState.immunePush)
                            {
                                //var carrierChaStats = refData.cdfeChaStats[carrier];
                                var temp = refData.cdfePhysicsVolocity[carrier];
                                dirPushforce = (dirPushforce *
                                    (1 - carrierChaStats.chaProperty.reduceHitBackRatios / 10000f) /
                                    carrierChaStats.chaProperty.mass * ((battle_force_factor / 10000f) / 100f));
                                Debug.Log($"origin dirPushforce {dirPushforce}");
                                int battle_super_force = 0;
                                int battle_bounds_force = 0;
                                ref var constantConfig =
                                    ref inData.globalConfigData.value.Value.configTbbattle_constants
                                        .configTbbattle_constants;
                                for (int i = 0; i < constantConfig.Length; i++)
                                {
                                    if (constantConfig[i].constantName == (FixedString128Bytes)"battle_super_force")
                                    {
                                        battle_super_force = constantConfig[i].constantValue;
                                    }

                                    if (constantConfig[i].constantName == (FixedString128Bytes)"battle_bounds_force")
                                    {
                                        battle_bounds_force = constantConfig[i].constantValue;
                                    }
                                }

                                if (refData.cdfePlayerData.HasComponent(caster))
                                {
                                    var playerData = refData.cdfePlayerData[caster];
                                    bool maxPushForce = rand.NextInt(0, 10001) <=
                                                        playerData.playerData.maxPushForceChance;

                                    if (maxPushForce)
                                    {
                                        dirPushforce *= (1 + battle_bounds_force / 10000f);
                                    }
                                    else
                                    {
                                        bool superPushForce = rand.NextInt(0, 10001) <=
                                                              playerData.playerData.superPushForceChance;

                                        if (superPushForce)
                                        {
                                            dirPushforce *= (1 + battle_super_force / 10000f);
                                        }
                                    }
                                }

                                //Debug.Log($"dirPushforce {dirPushforce}");
                                direction.z = 0;
                                direction = math.normalizesafe(direction);
                                //Debug.Log($"dirPushforce {dirPushforce}");
                                temp.Linear += math.clamp(dirPushforce, 0, math.abs(dirPushforce)) * direction;

                                refData.cdfePhysicsVolocity[carrier] = temp;
                                if (!refData.cdfePlayerData.HasComponent(carrier))
                                {
                                    var tempMass = refData.cdfePhysicsMass[carrier];
                                    tempMass.InverseMass = 1f / carrierChaStats.chaProperty.mass;
                                    refData.cdfePhysicsMass[carrier] = tempMass;
                                }


                                refData.ecb.AddComponent(inData.sortkey, carrier, new HitBackData
                                {
                                    id = id,
                                    hitTimes = battle_continuous_collision_max_num,
                                    attacker = caster
                                });
                                refData.ecb.AddComponent<GetHitAnimObstacleData>(inData.sortkey, carrier);
                            }
                        }
                    }
                }
            }
            else
            {
                //秒杀伤害
                if (calc_type.x == 2 && calc_type.y == 200000)
                {
                    damageInfoTag.SetAllTagsFalse();
                    damageInfoTag.seckillDamage = true;
                }
                else
                {
                    //障碍物伤害
                    if (inData.cdfeMapElementData.HasComponent(carrier))
                    {
                        //Debug.LogError($"cdfeMapElementDatacdfeMapElementDatacdfeMapElementData11");
                        // if (damageInfoTag.directDamage)
                        // {
                        //     damage.environment = 1;
                        // }
                        damageInfoTag.SetAllTagsFalse();
                        damageInfoTag.environmentDamage = true;
                        damage.normal = 1;
                    }
                    else
                    {
                        float reduceBulletRatios = isBullet ? carrierChaStats.chaProperty.reduceBulletRatios : 0;
                        var damageValue = atk *
                                          ((1 + casterChaStats.chaProperty
                                               .damageRatios / 10000f) *
                                           (1 - carrierChaStats.chaProperty
                                               .reduceHurtRatios / 10000f) * (1 - reduceBulletRatios / 10000f) +
                                           (casterChaStats.chaProperty.damageAdd -
                                            carrierChaStats.chaProperty.reduceHurtAdd));

                        damage.normal = (long)math.floor(damageValue);
                        if (refData.cdfePlayerData.HasComponent(caster) &&
                            skillID == refData.cdfePlayerData[caster].playerOtherData.weaponSkillId)
                        {
                            damageInfoTag.SetAllTagsFalse();
                            damageInfoTag.weaponDamage = true;
                            //damage.weaponDamage = (long)math.floor(damageValue);
                            damage.normal = (long)math.floor(damageValue);
                        }
                        else if (isBullet)
                        {
                            damageInfoTag.SetAllTagsFalse();
                            damageInfoTag.bulletDamage = true;
                            //damage.bullet = (long)math.floor(damageValue);
                            damage.normal = (long)math.floor(damageValue);
                        }
                    }
                }


                Debug.Log($"Buff_1 DamageInfo {atk}{damage.normal}");
                //TODO:暴击率


                var damPos = UnityHelper.GetDamageNumPos(inData.cdfeLocalTransform, caster, carrier);


                refData.ecb.AppendToBuffer(inData.sortkey, inData.wbe, new DamageInfo
                {
                    attacker = caster,
                    defender = carrier,
                    tags = damageInfoTag,
                    damage = damage,
                    criticalRate = casterChaStats
                                       .chaProperty.critical /
                                   10000f,
                    criticalDamage = casterChaStats
                                         .chaProperty.criticalDamageRatios /
                                     10000f,
                    hitRate = 1,
                    degree = 0,
                    pos = damPos,
                    bulletEntity = default
                });
            }
            //Debug.LogError($"buff1 tempType {tempType}");
            //Debug.LogError($"buff111");
            //if(output_type==0&&additonBuff==0) { }
            //else if(output_type==1 && additonBuff == 1) { }
        }

        private float AddtionOtherPar(BuffData_ReadOnly inData)
        {
            ref var skillElementsConfig =
                ref inData.globalConfigData.value.Value.configTbskill_effectElements.configTbskill_effectElements;
            int index = 0;
            for (int i = 0; i < skillElementsConfig.Length; i++)
            {
                if (skillElementsConfig[i].id == id)
                {
                    index = i;
                    break;
                }
            }

            ref var skillElement = ref skillElementsConfig[index];
            switch (skillElement.bonusOtherType)
            {
                case 1:
                    return math.distance(inData.cdfeLocalTransform[carrier].Position,
                               inData.cdfeLocalTransform[caster].Position) *
                           skillElement.bonusOtherTypePara[0] /
                           10000f;
            }

            return 0;
        }

        public void OnRemoved(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnUpdate(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnTick(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnHit(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeHurt(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnKill(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeforeBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnPerUnitMove(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }
    }
}