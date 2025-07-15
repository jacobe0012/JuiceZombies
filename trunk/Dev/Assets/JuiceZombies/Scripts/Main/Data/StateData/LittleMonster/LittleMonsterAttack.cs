using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    public partial struct LittleMonsterAttack : IState
    {
        ///<summary>0
        ///动画ID
        ///</summary>
        public int stateId;

        ///<summary>1
        ///存在了多少时间了，单位：秒
        ///</summary>
        public float timeElapsed;

        ///<summary>2
        ///倍速，1=100%，0.1=10%是最小值
        ///</summary>
        public float timeScale;

        ///<summary>3
        ///剩余多少时间，单位：秒
        ///</summary>
        public float duration;

        ///<summary>4
        ///持续多少帧了，单位：帧
        ///</summary>
        public int tick;

        ///<summary>5
        ///是否是一次性播放动画
        ///</summary>
        public bool isOneShotAnim;


        public float3 attackDir;

        public int tbmonster_weaponsIndex;


        public void OnStateEnter(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            refData.stateMachine.curAnim = AnimationEnum.Idle;
            var entityGroupData = inData.cdfeEntityGroupData[inData.entity];
            var temp = refData.cdfeWeaponData[entityGroupData.weaponEntity];
            temp.curAttackTime = temp.attackTime;
            temp.curRepeatTimes = temp.repeatTimes;
            refData.cdfeWeaponData[entityGroupData.weaponEntity] = temp;

            ref var tbmonster_weapons = ref
                inData.configData.value.Value.configTbweapons.configTbweapons;
            tbmonster_weaponsIndex = 0;
            for (int i = 0; i < tbmonster_weapons.Length; i++)
            {
                if (tbmonster_weapons[i].id == refData.enemyData.weaponId)
                {
                    tbmonster_weaponsIndex = i;
                    break;
                }
            }

            ref var monster_weapon = ref tbmonster_weapons[tbmonster_weaponsIndex];
            duration = monster_weapon.displayTime / 1000f;
            duration /= timeScale;
            attackDir = new float3(0, 1, 0);
            if (refData.storageInfoFromEntity.Exists(refData.enemyData.target))
            {
                attackDir = math.normalizesafe(inData.cdfeLocalTransform[refData.enemyData.target].Position -
                                               inData.cdfeLocalTransform[inData.entity].Position);
            }
            // Debug.LogError($"enter attack");
            //refData.enemyData.
        }

        public void OnStateExit(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
        }

        public void OnStateUpdate(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData)
        {
            //const float attackDegrees = 80f;

            if (refData.chaStats.chaResource.hp <= 0)
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.LittleMonsterDie);
                return;
            }

            if (inData.cdfeHitBackData.HasComponent(inData.entity))
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.LittleMonsterGetHit);
                return;
            }

            if (refData.chaStats.IsStrongControl())
            {
                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.LittleMonsterBeControlled);
                return;
            }

            refData.agentBody.Stop();


            if (timeElapsed > duration)
            {
                // if (math.length(inData.Tran - inData.cdfeLocalToWorld[refData.enemyData.target].Position) <
                //     refData.enemyData.attackRadius)
                // {
                //     refData.stateMachine.transitionToStateIndex = (int) State.TypeId.GoblinAttack;
                //     return;
                // }

                refData.stateMachine.transitionToStateIndex =
                    BuffHelper.GetStateIndex(inData.states, State.TypeId.LittleMonsterMove);
                return;
            }

            //offset.curAttackTime
            ref var constConfig = ref inData.configData.value.Value.configTbconstants.configTbconstants;


            int battle_continuous_collision_max_num = 0;

            for (int j = 0; j < constConfig.Length; j++)
            {
                if (constConfig[j].constantName == (FixedString128Bytes)"battle_continuous_collision_max_num")
                {
                    battle_continuous_collision_max_num = constConfig[j].constantValue;
                    break;
                }
            }

            ref var tbmonster_weapons = ref
                inData.configData.value.Value.configTbweapons.configTbweapons;
            ref var monster_weapon = ref tbmonster_weapons[tbmonster_weaponsIndex];

            int degree = 0;
            //float radius = 0;
            int bulletId = 0;
            switch (monster_weapon.atkType)
            {
                case 1:
                    degree = monster_weapon.para1;
                    break;
                case 2:
                    degree = monster_weapon.para1;
                    break;
                case 3:
                    degree = monster_weapon.para1;
                    //radius = monster_weapon.para2 / 1000f;
                    break;
                case 4:
                    bulletId = monster_weapon.para1;
                    break;
                case 5:
                    bulletId = monster_weapon.para1;
                    break;
            }

            int attackTick = (int)((duration * (monster_weapon.para3 / 10000f)) / inData.fdT);
            //Debug.Log($"tick{tick}");
            if (tick == attackTick)
            {
                if (refData.enemyData.attackType == EnemyAttackType.NormalShortAttack)
                {
                    Debug.Log($"NormalShortAttack attackTick{attackTick} ");
                    // refData.ecb.AppendToBuffer(inData.sortkey, inData.entity, new SkillOld
                    // {
                    //     CurrentTypeId = (SkillOld.TypeId)100101,
                    //     Single_2 = 2,
                    //     Entity_5 = inData.entity,
                    //     Boolean_14 = true,
                    // });

                    var prefab = inData.prefabMapData.prefabHashMap["CommonATKPrefabGen2"];

                    var ins = refData.ecb.Instantiate(inData.sortkey, prefab);

                    //Debug.LogError($"sadfsadf13123");
                    /*var qua = MathHelper.LookRotation2D(attackDir);
                    var tran = new LocalTransform
                    {
                        Position = inData.cdfeLocalTransform[inData.entity].Position,
                        Scale = inData.cdfeLocalTransform[inData.entity].Scale + 16f,
                        Rotation = qua
                    };
                    refData.ecb.SetComponent(inData.sortkey, ins, tran);
                    refData.ecb.SetComponent(inData.sortkey, ins, new MonsterGeneralAttackData
                    {
                        Data = new float3(0.3f, inData.eT, degree)
                    });


                    // math.radians(dir, math.forward());
                    refData.ecb.AddComponent(inData.sortkey, ins, new TimeToDieData
                    {
                        duration = 1
                    });*/

                    //TODO:攻击判定帧和距离

                    if (!refData.storageInfoFromEntity.Exists(refData.enemyData.target))
                    {
                        return;
                    }

                    if (math.length(inData.cdfeLocalTransform[inData.entity].Position -
                                    inData.cdfeLocalTransform[refData.enemyData.target].Position) >
                        refData.enemyData.attackRadius)
                    {
                        return;
                    }

                    if (degree < 360)
                    {
                        float3 curDir = math.normalizesafe(
                            inData.cdfeLocalTransform[refData.enemyData.target].Position -
                            inData.cdfeLocalTransform[inData.entity].Position);


                        var angleToEnemy = MathHelper.VectorAngleUnsign(attackDir, curDir);


                        if (angleToEnemy >= degree / 2)
                        {
                            Debug.Log("敌人不在扇形范围内");
                            return;
                        }
                    }


                    var otherTotalProperty = inData.cdfeChaStats[refData.enemyData.target].chaProperty;
                    var enemyChaStats = inData.cdfeChaStats[inData.entity].chaProperty;


                    var damage = enemyChaStats.atk *
                                 ((1 + enemyChaStats.damageRatios / 10000f) *
                                  (1 - otherTotalProperty.reduceHurtRatios / 10000f) +
                                  (otherTotalProperty.damageAdd -
                                   otherTotalProperty.reduceHurtAdd));
                    var damPos = UnityHelper.GetDamageNumPos(inData.cdfeLocalTransform, inData.entity,
                        refData.enemyData.target);
                    //max({A.攻击力 * [A.是否暴击(基础暴击率+暴击率加成) * A.暴击伤害(基础暴击伤害倍率+暴击伤害加成)] * [(1+A.增伤加成) * (1-B.减伤加成)] + [(A.增伤固定加成 - B.减伤固定加成)]},1)
                    refData.ecb.AppendToBuffer(inData.sortkey, inData.wbe, new DamageInfo
                    {
                        attacker = inData.entity,
                        defender = refData.enemyData.target,
                        tags = new DamageInfoTag
                        {
                            directDamage = true,
                            directHeal = false,
                        },
                        damage = new Damage
                        {
                            normal = (int)math.floor(damage),
                        },
                        criticalRate = enemyChaStats.critical /
                                       10000f,
                        criticalDamage = enemyChaStats.criticalDamageRatios /
                                         10000f,
                        hitRate = 1,
                        degree = 0,

                        pos = damPos,
                        bulletEntity = default,
                    });

                    if (refData.enemyData.isHitBackAttack)
                    {
                        if (!inData.cdfeHitBackData.HasComponent(refData.enemyData.target))
                        {
                            refData.ecb.AddComponent(inData.sortkey, refData.enemyData.target, new HitBackData
                            {
                                id = 0,
                                hitTimes = battle_continuous_collision_max_num,
                                attacker = inData.entity,
                                isLittleEnemyAttack = true
                            });
                        }
                    }
                }
                else if (refData.enemyData.attackType == EnemyAttackType.NormalLongAttack)
                {
                    int index = 0;
                    ref var bulletConfig = ref inData.configData.value.Value.configTbbullets.configTbbullets;
                    for (int i = 0; i < bulletConfig.Length; i++)
                    {
                        if (bulletConfig[i].id == bulletId)
                        {
                            index = i;
                            break;
                        }
                    }

                    ref var bullet = ref bulletConfig[index];
                    float bulletDuration = (bullet.num / bullet.groupNum - 1) * bullet.interval / 1000f;
                    refData.ecb.AppendToBuffer(inData.sortkey, inData.entity, new BulletCastData
                    {
                        id = bulletId,
                        //TODO:限定一个效果最大值 大于的话看不到两技能的间隔
                        duration = bulletDuration > 0 ? bulletDuration : 0f,
                        caster = inData.entity,
                    });
                }
            }
        }
    }
}