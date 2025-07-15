//using cfg.blobstruct;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;


//namespace Main
//{
//    /// <summary>
//    /// 呲水枪的技能逻辑
//    /// </summary>
//    public partial struct Skill_100201 : ISkillOld
//    {
//        ///<summary>0
//        ///技能的id
//        ///</summary>
//        public int id;

//        ///<summary>1
//        ///技能等级
//        ///</summary>
//        public int level;

//        ///<summary>2
//        ///冷却时间，单位秒。尽管游戏设计里面是没有冷却时间的，但是我们依然需要这个数据
//        ///因为作为一个ARPG子分类，和ARPG游戏有一样的问题：一次按键（时间够久）会发生连续多次使用技能，所以得有一个GCD来避免问题
//        ///当然和wow的gcd不同，这个“GCD”就只会让当前使用的技能进入0.1秒的冷却
//        ///</summary>
//        public float cooldown;


//        ///<summary>3
//        ///持续时间，单位：秒
//        ///</summary>
//        public float duration;


//        ///<summary>4
//        ///倍速，1=100%，0.1=10%是最小值
//        ///</summary>
//        public float timeScale;

//        ///<summary>5
//        ///Timeline的焦点对象也就是创建timeline的负责人，比如技能产生的timeline，就是技能的施法者
//        ///</summary>
//        public Entity caster;

//        ///<summary>6
//        ///技能已经运行了多少帧了 无需赋值
//        ///</summary>
//        public int tick;

//        /// <summary>7
//        /// 技能当前冷却时间 无需赋值
//        /// </summary>
//        public float curCooldown;

//        ///<summary>8
//        ///剩余时间，单位：秒
//        ///</summary>
//        public float curDuration;

//        ///<summary>9
//        ///距离这个技能最近的目标
//        ///</summary>
//        public Entity target;

//        /// <summary>
//        /// 是否是充能技能 10
//        /// </summary>


//        /// <summary>
//        /// 充能时间 毫秒 11 （充能次数充能也转换成时间进行取模）
//        /// </summary>


//        /// <summary>
//        /// 技能实体的执行时间 用于充能技能 12
//        /// </summary>


//        ///<summary>13
//        ///技能从创建到现在总的tick
//        ///</summary>
//        public int totalTick;

//        ///<summary>14
//        ///是否时一次性释放技能
//        ///</summary>
//        public bool isOneShotSkill;

//        ///<summary>15
//        ///是否是确定位置坐标
//        ///</summary>
//        public bool isUseCertainPos;

//        ///<summary>16
//        ///一次性释放技能的坐标
//        ///</summary>
//        public float3 pos;

//        public int currentSkillEffectID;
//        public int width;
//        public int height;
//        public float gap;
//        public float skillDuration;
//        public int skillHeight;
//        public float switchTime;
//        public int pushRate;
//        public int damageRate;
//        public int limitDistance;
//        public bool isLaunch;

//        public int bufferStack;
//        public float buffDuration;
//        public int debuffRate;

//        public float freezDuration;

//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            isLaunch = false;
//            ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;
//            //拿到当前技能效果id

//            for (int i = 0; i < skillArray.Length; ++i)
//            {
//                if (skillArray[i].id == id)
//                {
//                    currentSkillEffectID = skillArray[i].skillEffectArray[0];
//                    cooldown = skillArray[i].cd / 1000f;
//                }
//            }

//            ref var skillEffectsArray = ref inData.globalConfig.value.Value.configTbskill_effects.configTbskill_effects;


//            GetLastParmeter(inData, ref skillEffectsArray, currentSkillEffectID);
//            AddWeaponEquipSkill(inData, ref refData);
//        }

//        private void AddWeaponEquipSkill(TimeLineData_ReadOnly inData, ref TimeLineData_ReadWrite refData)
//        {
//            if (BuffHelper.JudgeEquipExist(inData.entity, inData.bfePlayerEquipSkillBuffer, 401023, inData.globalConfig,
//                    out int skillEffectID))
//            {
//                var buff = BuffHelper.UpdateParmeter(inData.globalConfig, skillEffectID, 10213000);
//                var additon = buff[1].x;
//                refData.ecb.AppendToBuffer(inData.sortkey, inData.entity, new Buff_10213000
//                {
//                    id = 10213000,
//                    priority = 0,
//                    maxStack = 0,
//                    tags = 0,
//                    tickTime = 0,
//                    timeElapsed = 0,
//                    ticked = 0,
//                    duration = 1,
//                    permanent = true,
//                    caster = inData.entity,
//                    carrier = inData.entity,
//                    canBeStacked = false,
//                    buffStack = default,
//                    buffArgs = new BuffArgs
//                    {
//                        args0 = additon,
//                        args1 = 0,
//                        args2 = 0,
//                        args3 = 0,
//                        args4 = 0
//                    }
//                }.ToBuffOld());
//            }
//        }

//        //应该加到玩家状态机里
//        private bool IsShortReduceChargingTime()
//        {
//            return false;
//        }

//        private bool IsLargeReduceChargingTime()
//        {
//            return false;
//        }


//        private void GetLastParmeter(TimeLineData_ReadOnly inData,
//            ref BlobArray<ConfigTbskill_effect> skillEffectsArray, int currentSkillEffectID)
//        {
//            BlobArray<int3x4> buffDefault = new BlobArray<int3x4>();
//            ref BlobArray<int3x4> buffArray = ref buffDefault;


//            for (int i = 0; i < skillEffectsArray.Length; ++i)
//            {
//                if (skillEffectsArray[i].id == currentSkillEffectID)
//                {
//                    limitDistance = skillEffectsArray[i].limit[0].y;
//                    //拿到buff组

//                    buffArray = ref skillEffectsArray[i].skillEffectBuffNew;
//                    if (buffArray.Length <= 0)
//                    {
//                        Debug.Log("没拿到数据");
//                        return;
//                    }


//                    //处理buff
//                    for (int j = 1; j <= 8; j++)

//                    {
//                        int currentBuffID = 0;
//                        int currentBuffCount = 0;
//                        int startIndex = 0;
//                        switch (j)
//                        {
//                            case 1:
//                                currentBuffID = buffArray[0].c0.x;
//                                currentBuffCount = buffArray[0].c0.y;
//                                break;

//                            case 2:
//                                currentBuffID = buffArray[0].c1.x;
//                                currentBuffCount = buffArray[0].c1.y;
//                                break;

//                            case 3:
//                                currentBuffID = buffArray[0].c2.x;
//                                currentBuffCount = buffArray[0].c2.y;
//                                break;

//                            case 4:
//                                currentBuffID = buffArray[0].c3.x;
//                                currentBuffCount = buffArray[0].c3.y;
//                                break;
//                            case 5:
//                                currentBuffID = buffArray[1].c0.x;
//                                currentBuffCount = buffArray[1].c0.y;
//                                startIndex = 1;
//                                break;

//                            case 6:
//                                currentBuffID = buffArray[1].c1.x;
//                                currentBuffCount = buffArray[1].c1.y;
//                                startIndex = 1;
//                                break;
//                            case 7:
//                                currentBuffID = buffArray[1].c2.x;
//                                currentBuffCount = buffArray[1].c2.y;
//                                startIndex = 1;
//                                break;
//                            case 8:
//                                currentBuffID = buffArray[1].c3.x;
//                                currentBuffCount = buffArray[1].c3.y;
//                                startIndex = 1;
//                                break;
//                        }

//                        HandleBuff(currentBuffID, currentBuffCount, ref buffArray, in inData, startIndex, j);
//                    }

//                    break;
//                }
//            }
//        }

//        private void HandleBuff(int currentBuffID, int buffCount, ref BlobArray<int3x4> buffArray,
//            in TimeLineData_ReadOnly inData, int startIndex, int buffIndex)
//        {
//            var buffPar = GetBuffParmeters(ref buffArray, buffIndex, startIndex, buffCount);
//            if (currentBuffID == 40000011)
//            {
//                width = buffPar[1].x;
//                height = buffPar[2].x *
//                         (1 + inData.cdfeChaStats[inData.entity].chaProperty.skillRangeRatios * buffPar[2].z / 10000);
//                gap = buffPar[3].x / 1000f;
//                skillDuration = buffPar[4].x / 1000f;
//                skillHeight = buffPar[5].x;
//                switchTime = buffPar[6].x / 1000f;
//                pushRate = buffPar[7].x;
//            }

//            if (currentBuffID == 40000012)
//            {
//                width = buffPar[1].x;
//                height = buffPar[2].x *
//                         (1 + inData.cdfeChaStats[inData.entity].chaProperty.skillRangeRatios * buffPar[2].z / 10000);
//                gap = buffPar[3].x / 1000f;
//                skillDuration = buffPar[4].x / 1000f;
//                switchTime = buffPar[5].x / 1000f;
//                pushRate = buffPar[6].x;
//            }


//            if (currentBuffID == 30000003)
//            {
//                damageRate = buffPar[1].x;
//            }

//            if (currentBuffID == 30000011)
//            {
//                damageRate += buffPar[2].x;
//                pushRate += buffPar[3].x;
//            }

//            if (currentBuffID == 20003008)
//            {
//                debuffRate = buffPar[1].x;
//                buffDuration = buffPar[2].x / 1000f;
//            }

//            if (currentBuffID == 20003002)
//            {
//                freezDuration = buffPar[0].x / 1000f;
//            }
//        }

//        private NativeList<int3> GetBuffParmeters(ref BlobArray<int3x4> buffArray, int buffIndex, int startIndex,
//            int buffParameterCount)
//        {
//            NativeList<int3> temp = new NativeList<int3>(buffParameterCount, Allocator.Temp);
//            for (int j = 1; j <= buffParameterCount; ++j)
//            {
//                int3 tempInt3 = new int3(0, 0, 0);

//                switch (buffIndex)
//                {
//                    case 1:
//                    case 5:
//                        int currentParameterRate = buffArray[2 * j + startIndex].c0.x;

//                        tempInt3.x = currentParameterRate;
//                        int currentParameterInfectID = buffArray[2 * j + startIndex].c0.y;
//                        if (currentParameterInfectID != 0)
//                        {
//                            tempInt3.y = currentParameterInfectID;
//                            int currentParameterInfectRate = buffArray[2 * j + startIndex].c0.z;
//                            tempInt3.z = currentParameterInfectRate;
//                        }
//                        else
//                        {
//                            tempInt3.y = 0;
//                            tempInt3.z = 0;
//                        }

//                        break;
//                    case 2:
//                    case 6:
//                        currentParameterRate = buffArray[2 * j + startIndex].c1.x;

//                        tempInt3.x = currentParameterRate;
//                        currentParameterInfectID = buffArray[2 * j + startIndex].c1.y;
//                        if (currentParameterInfectID != 0)
//                        {
//                            tempInt3.y = currentParameterInfectID;
//                            int currentParameterInfectRate = buffArray[2 * j + startIndex].c1.z;
//                            tempInt3.z = currentParameterInfectRate;
//                        }
//                        else
//                        {
//                            tempInt3.y = 0;
//                            tempInt3.z = 0;
//                        }

//                        break;

//                    case 3:
//                    case 7:
//                        currentParameterRate = buffArray[2 * j].c2.x;

//                        tempInt3.x = currentParameterRate;
//                        currentParameterInfectID = buffArray[2 * j].c2.y;
//                        if (currentParameterInfectID != 0)
//                        {
//                            tempInt3.y = currentParameterInfectID;
//                            int currentParameterInfectRate = buffArray[2 * j].c2.z;
//                            tempInt3.z = currentParameterInfectRate;
//                        }
//                        else
//                        {
//                            tempInt3.y = 0;
//                            tempInt3.z = 0;
//                        }

//                        break;
//                    case 4:
//                    case 8:
//                        currentParameterRate = buffArray[2 * j].c3.x;

//                        tempInt3.x = currentParameterRate;
//                        currentParameterInfectID = buffArray[2 * j].c3.y;
//                        if (currentParameterInfectID != 0)
//                        {
//                            tempInt3.y = currentParameterInfectID;
//                            int currentParameterInfectRate = buffArray[2 * j].c3.z;
//                            tempInt3.z = currentParameterInfectRate;
//                        }
//                        else
//                        {
//                            tempInt3.y = 0;
//                            tempInt3.z = 0;
//                        }

//                        break;
//                }

//                temp.Add(tempInt3);
//            }

//            return temp;
//        }


//        private LocalTransform SetTrans(in TimeLineData_ReadOnly inData, Entity skillBaseBallBat, int limitDistance)
//        {
//            float distance = 9999;
//            target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//                inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                inData.cdfeObstacleTag,
//                inData.entity, TargetAttackType.Enemy, ref distance);
//            if (distance > limitDistance)
//            {
//            }

//            //拿到对敌角度
//            if (IsAttackToEnemy(limitDistance, inData))
//            {
//                //TODO:方向
//                float3 dir = inData.cdfeLocalTransform[target].Position -
//                             inData.cdfeLocalTransform[inData.entity].Position;


//                float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//                    new Vector3(0, 1, 0));

//                var qua = quaternion.AxisAngle(new float3(0, 0, -1), math.radians(needAngel));

//                var tran = new LocalTransform
//                {
//                    Position = inData.cdfeLocalTransform[inData.entity].Position,
//                    Scale = inData.cdfeLocalTransform[inData.entity].Scale,
//                    Rotation = qua
//                };


//                return tran.RotateZ(math.radians(needAngel));
//                //return tran;

//                //return inData.cdfeLocalTransform[inData.entity];
//            }

//            return inData.cdfeLocalTransform[inData.entity];
//        }

//        public bool IsAttackToEnemy(int limitDistance, TimeLineData_ReadOnly inData)
//        {
//            //判断是否对怪
//            float distance = 999f;
//            target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//                inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                inData.cdfeObstacleTag,
//                inData.entity, TargetAttackType.Enemy, ref distance);
//            if (distance > limitDistance)
//            {
//                target = default;
//            }

//            return target != default ? true : false;
//        }

//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //throw new NotImplementedException();
//        }

//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            Entity skillEntity = default;
//            var temp = switchTime / inData.fdT;
//            if (tick % temp == 0 && IsAttackToEnemy(limitDistance, inData))
//            {
//                //实例化skill
//                //实现s1冻结效果
//                if (!refData.storageInfoFromEntity.Exists(target))
//                    return;
//                int buffStack = 0;
//                if (id == 100206)
//                {
//                    var buffs = inData.bfeBuff[target];
//                    for (int i = 0; i < buffs.Length; i++)
//                    {
//                        if (buffStack >= 10)
//                        {
//                            // refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff_20003002
//                            // {
//                            //     id = 20003002,
//                            //     duration = freezDuration,
//                            //     carrier = target,
//                            // }.ToBuffOld());

//                            buffStack = 0;
//                        }


//                        // if (buffs[i].Int32_0 == 20003007)
//                        // {
//                        //     buffStack++;
//                        // }
//                    }
//                }

//                if (id == 100201 || id == 100202 || id == 100203 || id == 100204 || id == 100205 || id == 100206)
//                {
//                    var skillPrefab = inData.prefabMapData.prefabHashMap["SkillWaterGun"];
//                    skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//                    var newPos = SetTrans(in inData, skillPrefab, limitDistance);
//                    refData.ecb.SetComponent(inData.sortkey, skillEntity, newPos);


//                    var data = new SkillWaterGunAttack
//                    {
//                        id = currentSkillEffectID,
//                        caster = inData.entity,
//                        currentEnemy = target,
//                        duration = switchTime,
//                        width = width,
//                        height = height,
//                        scale = skillHeight,
//                        tick = 0,
//                        skillEntity = skillEntity,
//                        damageRate = damageRate,
//                        pushRate = pushRate,
//                        buffDuration = buffDuration,
//                        debuffRate = debuffRate,
//                        buffStack = buffStack,
//                        stayAttack = true,
//                        stayAttackInterval = skillDuration,
//                        skillDelay = 0,
//                        isBullet = false,
//                        hp = 0
//                    }.ToSkillAttack();

//                    refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData { data = data, });
//                }

//                if (id == 100207)
//                {
//                    var skillPrefab = inData.prefabMapData.prefabHashMap["SkillWaterGunS2"];
//                    skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//                    var newPos = SetTrans(in inData, skillPrefab, limitDistance);
//                    refData.ecb.SetComponent(inData.sortkey, skillEntity, newPos);
//                    var data = new SkillWaterGun_S2_Attack
//                    {
//                        id = currentSkillEffectID,
//                        caster = inData.entity,
//                        currentEnemy = target,
//                        duration = switchTime,
//                        width = width,
//                        height = height,
//                        tick = 0,
//                        skillEntity = skillEntity,
//                        damageRate = damageRate,
//                        pushRate = pushRate,
//                    }.ToSkillAttack();

//                    refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData { data = data, });
//                }
//            }

//            if (IsAttackToEnemy(limitDistance, inData) && id == 100207 && skillEntity != default &&
//                skillEntity != Entity.Null)
//            {
//                float3 dir = inData.cdfeLocalTransform[target].Position -
//                             inData.cdfeLocalTransform[inData.entity].Position;
//                //添加推力和伤害
//                refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//                {
//                    buffID = 40000011,
//                    buffArgs = new float3x4 { c0 = new float3(pushRate, 602, 10000), c1 = dir }
//                });
//                refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//                {
//                    buffID = 30000003,
//                    buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, 10000), c1 = dir }
//                });
//            }


//            // Debug.Log($"skillDuration{skillDuration}");
//        }

//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }
//    }
//}

