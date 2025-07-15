using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    //非触发类型的触发器通用  没有位置大小方向变动的技能实体 只存在一帧
    public partial struct SkillAttack_0 : ISkillAttack
    {
        /// <summary>
        /// 0
        /// </summary>
        public int id;

        /// <summary>
        /// 1 一次性释放的技能实体 值=0
        /// </summary>
        public float duration;

        /// <summary>
        /// 2
        /// </summary>
        public int tick;

        /// <summary>
        /// 3 技能实体释放者
        /// </summary>
        public Entity caster;

        /// <summary>
        /// 4 是否是弹幕
        /// </summary>
        public bool isBullet;

        /// <summary>
        /// 5 弹幕hp 初始hp 当前hp
        /// </summary>
        public int hp;

        /// <summary>
        /// 实体的击中目标 6
        /// </summary>
        public Entity target;

        /// <summary>
        /// 7
        /// </summary>
        public int4 args;

        /// <summary>
        /// 弹幕速度 8
        /// </summary>
        public float speed;

        /// <summary>
        /// 弹幕用 触发器id 9
        /// </summary>
        public int triggerID;

        /// <summary>
        /// 是否被障碍物吸收 10
        /// </summary>
        public bool isAbsorb;

        /// <summary>
        /// 目标int值 11
        /// </summary>
        public int targetInt;

        /// <summary>
        /// 是否可触发OnStay回调 12
        /// </summary>
        public bool isOnStayTrigger;


        /// <summary>
        /// 弹幕旋转速度 13  度/s 
        /// </summary>
        public float rotateSpeed;

        /// <summary>
        /// 弹幕类型参数 14
        /// </summary>
        public float3 trackPar;

        /// <summary>
        /// 终点位置 15
        /// </summary>
        public float3 targetPos;

        /// <summary>
        /// 跟随的目标 16
        /// </summary>
        public Entity followedEntity;

        /// <summary>
        /// 当前可触发OnStay回调 冷却 17
        /// </summary>
        public float curOnStayTriggerCd;

        /// <summary>
        /// 总持续时间 18
        /// </summary>
        public float totalDuration;

        /// <summary>
        /// skill的id 19
        /// </summary>
        public int skillID;

        /// <summary>
        /// 是否走exit回调 20
        /// </summary>
        public bool enableExit;

        /// <summary>
        /// 回调类型 21  0:只走onEnter 1:走onEnter或者onStay
        /// </summary>
        public int funcType;

        /// <summary>
        /// 死亡触发器id 22 
        /// </summary>
        public int deadEffectID;

        /// <summary>
        /// OnStay回调cd 23
        /// </summary>
        public float onStayTriggerCd;

        /// <summary>
        /// 每时间触发的当前次数  24 用来处理多段矩形
        /// </summary>
        public int tirggerCount;

        /// <summary>
        /// 弹幕生成的tick时间 25
        /// </summary>
        public int curTick;

        /// <summary>
        /// 脏东西的entity 26
        /// </summary>
        public Entity holder;

        /// <summary>
        /// 是否是弹幕伤害 27
        /// </summary>
        public bool isBulletDamage;

        /// <summary>
        /// 是否是武器攻击 28
        /// </summary>
        public bool isWeaponAttack;

        /// <summary>
        /// 加速度 mm/s^2  29
        /// </summary>
        public int acceleration;

        /// <summary>
        /// 默认飞行方向 目标朝向向量 30 
        /// </summary>
        public float3 defaultDir;

        /// <summary>
        /// 弹幕最大速度 31
        /// </summary>
        public float defaultSpeed;

        /// <summary>
        /// triggerId 32
        /// </summary>
        public int triggerId;

        /// <summary>
        /// 用于冲锋的选点 33
        /// </summary>
        public float3 crashPos;
        
        private bool isActive;

        /// <summary>
        /// 每帧做位置变动
        /// Tween参数:
        /// t:当前时间
        /// b:初始位置
        /// c:变化量
        /// d:持续时间
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        /// <returns>变动后的LT</returns>
        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            //Debug.Log("DoSkillMove");
            var c = refData.cdfePhysicsCollider[inData.entity];
            //闪电链特效特殊处理
            //Debug.Log($"DoSkillMove {id}");

            //BuffHelper.SetSkillAttackTarget(refData.ecb, inData.sortkey, inData.entity, inData.config, id, c);
            if (id == 13041404 && !isActive)
            {
                Entity center = target;
                Debug.Log($"==========闪电链 13041404  target {center}  {holder}");
                if (inData.cdfeBulletTempTag.HasComponent(holder))
                {
                    center = inData.cdfeBulletTempTag[holder].onHitTarget;
                    Debug.Log($"==========闪电链 13041404  center {center}");
                }

                isActive = true;
                //Debug.Log($"==========闪电链 13041404  center {center}");
                var order = refData.ecb.CreateEntity(inData.sortkey);
                var data = new SpecialEffectData
                {
                    type = 2,
                    id = 13041404,
                    stateId = 0,
                    groupId = 0,
                    sort = 0,
                    caster = default,
                    tick = 0,
                    skillId = skillID,
                    startPos = default,
                    targetPos = default,
                    duration = 10,
                    chainCenterEntity = center,
                    chainLastEntity = default,
                    chainNextEntity = default
                    //chainRadius = casterTran.Scale
                };
                refData.ecb.AddComponent(inData.sortkey, order, new LocalTransform());
                refData.ecb.AddComponent(inData.sortkey, order, data);
                refData.ecb.AddBuffer<SpecialEffectChainBuffer>(inData.sortkey, order);
                foreach (var e in inData.otherEntities)
                {
                    if (refData.cdfeEnemyData.HasComponent(e))
                    {
                        refData.ecb.AppendToBuffer(inData.sortkey, order, new SpecialEffectChainBuffer
                        {
                            Entity = e
                        });
                    }
                }
            }

            return refData.cdfeLocalTransform[inData.entity];
        }

        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }

        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            Debug.Log($"attack0OnStart id:{id}");
            //isActive = false;
            duration = 0f;
            if (id == 13041404)
            {
                duration = 0.02f;
            }

            if (!refData.storageInfoFromEntity.Exists(caster)) return;

            // int targetInt = 0;
            // for (int i = 0; i < skillEffect.Length; i++)
            // {
            //     if (skillEffect[i].id == id)
            //     {
            //         targetInt = skillEffect[i].targetPara.Length > 0 ? skillEffect[i].targetPara[0] : 0;
            //     }
            // }
            //
            // var tempTargetData = refData.cdfeTargetData[inData.entity];
            // tempTargetData.targetValue = (uint)(targetInt + 128);
            // refData.cdfeTargetData[inData.entity] = tempTargetData;
            // var tempphysicsCollider = refData.cdfePhysicsCollider[inData.entity];
            // tempphysicsCollider =
            //     PhysicsHelper.CreateColliderWithCollidesWith(refData.physicsCollider, (uint)targetInt | 128);
            // refData.cdfePhysicsCollider[inData.entity] = tempphysicsCollider;
            ref var skillEffect = ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            AddEffectTime(ref refData, inData, skillID, id);
        }

        private readonly void AddEffectTime(ref SkillAttackData_ReadWrite refData, SkillAttackData_ReadOnly inData,
            int skillID, int skilleffectid)
        {
            //在技能上记录skillEffect次数
            if (!refData.bfeSkill.HasBuffer(caster))
            {
                return;
            }

            for (int i = 0; i < refData.bfeSkill[caster].Length; i++)
            {
                var temp0 = refData.bfeSkill[caster];
                var temp = temp0[i];

                if (temp.Int32_0 == skillID)
                {
                    //Debug.Log($"OnStart");

                    if (temp.int2x4_13.c0.x == skilleffectid)
                    {
                        temp.int2x4_13.c0.y++;
                    }
                    else if (temp.int2x4_13.c1.x == skilleffectid)
                    {
                        temp.int2x4_13.c1.y++;
                    }
                    else if (temp.int2x4_13.c2.x == skilleffectid)
                    {
                        temp.int2x4_13.c2.y++;
                    }
                    else if (temp.int2x4_13.c3.x == skilleffectid)
                    {
                        temp.int2x4_13.c3.y++;
                    }
                    else
                    {
                        if (temp.int2x4_13.c0.x == 0)
                        {
                            temp.int2x4_13.c0.x = skilleffectid;
                            temp.int2x4_13.c0.y = 1;
                        }
                        else if (temp.int2x4_13.c1.x == 0)
                        {
                            temp.int2x4_13.c1.x = skilleffectid;
                            temp.int2x4_13.c1.y = 1;
                        }
                        else if (temp.int2x4_13.c2.x == 0)
                        {
                            temp.int2x4_13.c2.x = skilleffectid;
                            temp.int2x4_13.c2.y = 1;
                        }
                        else if (temp.int2x4_13.c3.x == 0)
                        {
                            temp.int2x4_13.c3.x = skilleffectid;
                            temp.int2x4_13.c3.y = 1;
                        }
                    }

                    temp0[i] = temp;
                    break;
                }
            }
        }

        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            if (id == 51102502 || id == 51102602 || id == 51102702)
            {
                caster = inData.player;
            }

            if (!refData.storageInfoFromEntity.Exists(target))
            {
                Debug.Log("storageInfoFromEntity");
                return;
            }

            //能打弹幕的技能将会吸收销毁弹幕 除了boss的太极反弹技能
            if (inData.cdfeBulletTag.HasComponent(target) && caster != inData.cdfeBulletTag[target].caster)
            {
                refData.ecb.DestroyEntity(inData.sortkey, target);
                Debug.LogError("cdfeBulletTagcdfeBulletTagcdfeBulletTagcdfeBulletTagcdfeBulletTag");

                //生成新的特效
                if (skillID == 1111)
                {
                    var dirReturn = math.normalize(refData.cdfeLocalTransform[target].Position -
                                                   refData.cdfeLocalTransform[inData.entity].Position);
                    //UnityHelper.SpawnSpecialEffect(ref refData.ecb, inData.sortkey,inData.eT, refData.cdfeLocalTransform, inData.config,
                    //    ref skillEffect.specialEffects,inData.prefabMapData, holder);
                }
            }

            //Debug.Log($"闪电链 13041404  ");


            Debug.Log($"SkillAttack_0 OnHitOnHit effectid:{id},skillid:{skillID}");
            ref var skillConfig = ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            int skilleffectIndex = -1;
            for (int i = 0; i < skillConfig.Length; i++)
            {
                if (skillConfig[i].id == id)
                {
                    skilleffectIndex = i;
                    break;
                }
            }

            if (skilleffectIndex < 0) return;

            ref var skilleffect = ref skillConfig[skilleffectIndex];


            //环状矩形
            if (skilleffect.rangeType == 1)
            {
                var dis = math.distance(refData.cdfeLocalTransform[target].Position,
                    refData.cdfeLocalTransform[caster].Position);
                if (skilleffect.rangeTypePara.Length >= 3)
                {
                    float comparedis = skilleffect.rangeTypePara[2] / 1000f;
                    if (dis < comparedis)
                    {
                        return;
                    }
                }
            }

            //判断扇形
            else if (skilleffect.rangeType == 2)
            {
                var lineDir = refData.cdfeLocalTransform[target].Position - refData.cdfeLocalTransform[caster].Position;
                var angle = MathHelper.SignedAngle(targetPos, lineDir);

                Debug.Log(
                    $"pos1:{refData.cdfeLocalTransform[target].Position},pos2:{refData.cdfeLocalTransform[caster].Position},targetPos:{targetPos}");
                Debug.Log($"angle:{angle}");


                if (angle < (-skilleffect.rangeTypePara[0] / 2f) || angle > (skilleffect.rangeTypePara[0] / 2f))
                {
                    Debug.Log("angele return");
                    return;
                }

                if (skilleffect.rangeTypePara.Length >= 3)
                {
                    var dis = math.distance(refData.cdfeLocalTransform[target].Position,
                        refData.cdfeLocalTransform[caster].Position);
                    float comparedis = skilleffect.rangeTypePara[2] / 1000f;
                    if (dis < comparedis)
                    {
                        Debug.Log("distance return");
                        return;
                    }
                }
            }

            else if (skilleffect.rangeType == 3)
            {
                //环状圆形
                if (skilleffect.rangeTypePara.Length >= 2)
                {
                    var dis = math.distance(refData.cdfeLocalTransform[target].Position,
                        refData.cdfeLocalTransform[caster].Position);
                    float comparedis = skilleffect.rangeTypePara[1] / 1000f;
                    if (dis < comparedis)
                    {
                        return;
                    }
                }
            }

            //多段环状矩形
            else if (skilleffect.rangeType == 4)
            {
                var dis = math.distance(refData.cdfeLocalTransform[target].Position,
                    refData.cdfeLocalTransform[caster].Position);
                if (skilleffect.rangeTypePara.Length >= 5)
                {
                    float comparedis =
                        (skilleffect.rangeTypePara[2] + skilleffect.rangeTypePara[5] * (tirggerCount - 1)) / 1000f;
                    if (dis < comparedis)
                    {
                        Debug.Log($"dis:{dis},compareDis:{comparedis}");
                        return;
                    }
                }
            }


            if (skilleffect.conditionType != 0)
            {
                //Debug.Log("skilleffect.conditionType != 0");
                if (skilleffect.conditionType == 1)
                {
                    var dis = math.distance(refData.cdfeLocalTransform[target].Position,
                        refData.cdfeLocalTransform[caster].Position);
                    float comparedis = skilleffect.conditionTypePara[0] / 1000f;
                    int compareType = skilleffect.conditionTypePara[1];
                    if (dis >= comparedis && compareType != 2)
                    {
                        return;
                    }
                    else if (dis <= comparedis && compareType != 1)
                    {
                        return;
                    }
                    else if (dis == comparedis && compareType != 0)
                    {
                        return;
                    }
                }
                else if (skilleffect.conditionType == 2)
                {
                    if (!inData.cdfeChaStats.HasComponent(target)) return;
                    float hp = inData.cdfeChaStats[target].chaResource.hp /
                        (inData.cdfeChaStats[target].chaProperty.maxHp * 1f) * 10000f;
                    float compareRations = skilleffect.conditionTypePara[0];
                    int compareType = skilleffect.conditionTypePara[1];
                    if (hp >= compareRations && compareType != 2)
                    {
                        return;
                    }
                    else if (hp <= compareRations && compareType != 1)
                    {
                        return;
                    }
                    else if (hp == compareRations && compareType != 0)
                    {
                        return;
                    }
                }
                else if (skilleffect.conditionType == 3)
                {
                    bool isActive = false;
                    if (refData.bfeBuff.HasBuffer(target))
                    {
                        var buffs = refData.bfeBuff[target];
                        foreach (var buff in buffs)
                        {
                            if (buff.CurrentTypeId == (Buff.TypeId)skilleffect.conditionTypePara[0])
                            {
                                isActive = true;
                            }
                        }
                    }

                    if (!isActive) return;
                }
                else
                {
                    bool isHave = false;
                    if (refData.bfeBuff.HasBuffer(target))
                    {
                        var buffs = refData.bfeBuff[target];
                        foreach (var buff in buffs)
                        {
                            if (buff.CurrentTypeId == (Buff.TypeId)skilleffect.conditionTypePara[0])
                            {
                                isHave = true;
                            }
                        }
                    }

                    if (isHave) return;
                }
            }

            float3 dir = math.normalizesafe(refData.cdfeLocalTransform[target].Position -
                                            refData.cdfeLocalTransform[inData.entity].Position);
            if (refData.storageInfoFromEntity.Exists(caster))
            {
                dir = math.normalizesafe(refData.cdfeLocalTransform[target].Position -
                                         refData.cdfeLocalTransform[caster].Position);
            }

            if (isWeaponAttack)
            {
                int skillid = 0;
                NativeList<int> calcEffect = new NativeList<int>(10, Allocator.Temp);
                //var targetInt = skilleffect.target;
                if (inData.cdfeTrigger.HasBuffer(caster))
                {
                    var triggers = inData.cdfeTrigger[caster];
                    for (int i = 0; i < triggers.Length; i++)
                    {
                        var trigger = triggers[i];
                        if (trigger.TriggerType_5 == TriggerType.WeaponAttack && trigger.float4_6.x == 0 &&
                            trigger.float4_6.y == 0)
                        {
                            int triggerId = trigger.Int32_0;
                            for (int j = 0; j < skillConfig.Length; j++)
                            {
                                //Debug.Log($"id:{skillConfig[j].id}");
                                if (skillConfig[j].id == triggerId)
                                {
                                    Debug.Log($"skillConfig[j].id == triggerId");
                                    int effectId = skillConfig[j].calcTypePara[0];
                                    skillid = skillConfig[j].skillId;
                                    calcEffect.Add(effectId);
                                    break;
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < calcEffect.Length; i++)
                {
                    for (int j = 0; j < skillConfig.Length; j++)
                    {
                        if (calcEffect[i] == skillConfig[j].id)
                        {
                            Debug.Log($"calcEffect[i] == skillConfig[j].id:{calcEffect[i]}");
                            var rand = Unity.Mathematics.Random.CreateFromIndex(
                                (uint)(inData.timeTick + inData.entity.Index + i));
                            const int MaxPower = 10000;
                            int dropRate = math.clamp(skillConfig[j].power, 0, MaxPower);
                            bool canTrigger = rand.NextInt(0, MaxPower + 1) <= dropRate;
                            if (!canTrigger)
                            {
                                break;
                            }
                            else
                            {
                                //加次数
                                AddEffectTime(ref refData, inData, skillid, calcEffect[i]);
                                ref var elemts = ref skillConfig[j].elementList;
                                ref var states = ref skillConfig[j].battleStatus;
                                AddElement(ref refData, inData, dir, ref elemts, skillid, false);
                                BuffHelper.AddStates(ref refData, inData, ref states, target, caster, skillid);
                                break;
                            }
                        }
                    }
                }
            }

            if (triggerId == 1)
            {
                int extraIndex = -1;
                for (int j = 0; j < skillConfig.Length; j++)
                {
                    if (skillConfig[j].id == triggerID)
                    {
                        if (skillConfig[j].extraType == 2)
                        {
                            Debug.Log($"需要命中后触发trigger:{skillConfig[j].extraTypePara[0]}");
                            extraIndex = j;
                            break;
                        }
                    }
                }

                if (extraIndex > 0)
                {
                    ref var triggerList = ref skillConfig[extraIndex].extraTypePara;
                    for (int j = 0; j < triggerList.Length; j++)
                    {
                        Debug.Log($"添加trigger:{triggerList[j]}");
                        BuffHelper.AddTriggerForExtra(ref refData, in inData, inData.sortkey, triggerList[j], caster,
                            target, float3.zero);
                    }
                }
            }


            Debug.Log($"SkillAttack_0 1111111111111");
            if (inData.cdfeChaStats.HasComponent(target) &&
                refData.bfeBuff.HasBuffer(target))
            {
                Debug.Log($"SkillAttack_0 22222222");
                ref var states = ref skilleffect.battleStatus;
                //skillEffect加状态
                BuffHelper.AddStates(ref refData, inData, ref states, target, caster, skillID);

                //技能添加的buff
                ref var elementList = ref skilleffect.elementList;
                ref var elementTrigger = ref skilleffect.elementTrigger;
                Debug.Log($"SkillAttack_0 333333333");
                AddElement(ref refData, inData, dir, ref elementList, skillID, isBullet);

                for (int i = 0; i < elementTrigger.Length; i++)
                {
                    int skillEffectIndex = 0;

                    for (int j = 0; j < skillConfig.Length; ++j)
                    {
                        if (elementTrigger[i] == skillConfig[j].id)
                        {
                            skillEffectIndex = j;
                            break;
                        }
                    }

                    ref var skilleffecttrigger =
                        ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews[
                            skillEffectIndex];
                    var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(inData.seed + inData.entity.Index +
                                                                               inData.entity.Version));
                    const int MaxPower = 10000;
                    int dropRate = math.clamp(skilleffecttrigger.power, 0, MaxPower);
                    bool canTrigger = rand.NextInt(0, MaxPower + 1) <= dropRate;
                    if (!canTrigger)
                        return;


                    var buffs = refData.bfeBuff[target];
                    bool isAdd = true;
                    foreach (var buff in buffs)
                    {
                        if (buff.Int32_0 == elementTrigger[i])
                        {
                            isAdd = false;
                            break;
                        }
                    }

                    if (isAdd)
                    {
                        ///效果id是目标的情况 特殊处理
                        var delay = skilleffecttrigger.delayTypePara[0] / 1000f;
                        Debug.Log($"挂炸弹{elementTrigger[i]}");

                        refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff
                        {
                            //TODO:炸弹走新技能
                            CurrentTypeId = (Buff.TypeId)elementTrigger[i],
                            Int32_0 = elementTrigger[i],
                            Single_3 = delay,
                            Boolean_4 = false,
                            Entity_5 = caster,
                            Entity_6 = target,
                            Int32_20 = skillID
                        });
                    }
                }
            }

            //火水木油等元素
            if (inData.cdfeElementData.HasComponent(inData.entity))
            {
                Debug.Log("Onhit element has");
                var elementID = inData.cdfeElementData[inData.entity].id;
                int index = -1;
                ref var elementConfig = ref inData.config.value.Value.configTbelements.configTbelements;
                ref var elementEffectConfig =
                    ref inData.config.value.Value.configTbelement_effects.configTbelement_effects;

                for (int i = 0; i < elementConfig.Length; i++)
                {
                    if (elementConfig[i].id == elementID)
                    {
                        index = i;
                        break;
                    }
                }

                if (index < 0) return;

                if (inData.otherEntities.Length > 0)
                {
                    NativeList<int> activeList = new NativeList<int>(10, Allocator.Temp);
                    var entities = inData.otherEntities;

                    for (int i = 0; i < entities.Length; i++)
                    {
                        var entity = entities[i];
                        if (refData.storageInfoFromEntity.Exists(entity))
                        {
                            if (inData.cdfeElementData.HasComponent(entity) && inData.cdfeAreaTag.HasComponent(entity))
                            {
                                DoElementActionToArea(inData.cdfeElementData[entity],
                                    inData.cdfeElementData[inData.entity], entity, inData.entity, ref refData, inData,
                                    ref activeList);

                                Debug.Log("Onhit 地形触发");
                            }
                            else if (inData.cdfeObstacleTag.HasComponent(entity) &&
                                     inData.cdfeElementData.HasComponent(entity))
                            {
                                DoElementActionToObstacle(inData.cdfeElementData[entity],
                                    inData.cdfeElementData[inData.entity], entity, inData.entity, ref refData, inData,
                                    ref activeList);
                                Debug.Log("Onhit 障碍物触发");
                            }
                        }
                    }

                    if (inData.cdfeChaStats.HasComponent(target) && !inData.cdfeObstacleTag.HasComponent(target))
                    {
                        var buffs = refData.bfeBuff[target];
                        //火
                        if (elementID == 101)
                        {
                            if (!activeList.Contains(101202))
                            {
                                refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target,
                                    new Buff_102
                                    {
                                        id = 102, permanent = false, carrier = target, caster = inData.entity,
                                        duration = elementConfig[index].time / 1000f
                                    }.ToBuff());
                                Debug.Log("Onhit 元素触发  火");
                            }
                        }
                        else if (elementID == 102)
                        {
                            Debug.Log("Onhit 元素触发  水");
                            refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target,
                                new Buff_124
                                {
                                    id = 124, permanent = false, carrier = target, caster = inData.entity,
                                    duration = elementConfig[index].time / 1000f
                                }.ToBuff());
                            if (activeList.Contains(102201))
                            {
                                //Debug.LogError("Onhit 元素触发  102201");
                                //var tempHp = refData.cdfeAreaTempHp[target];
                                //var hp = tempHp.maxHp--;
                                //refData.cdfeAreaTempHp[target] = tempHp;
                                //if (hp <= 0)
                                //{
                                //    refData.ecb.AddComponent(inData.sortkey, target,
                                //        new TimeToDieData { duration = 0.02f });
                                //}
                            }
                            else if (activeList.Contains(102203))
                            {
                                for (int i = 0; i < elementEffectConfig.Length; i++)
                                {
                                    if (elementEffectConfig[i].from == 203 && elementEffectConfig[i].target == 102)
                                    {
                                        index = i;
                                        break;
                                    }
                                }

                                Debug.Log("Onhit 元素触发  102203");
                                ref var element = ref elementEffectConfig[index];
                                refData.ecb.AppendToBuffer(inData.sortkey, target,
                                    new Buff_110 { id = 110, carrier = target, duration = element.para[0] / 1000f }
                                        .ToBuff());
                            }
                        }
                        else if (elementID == 103)
                        {
                            Debug.Log("Onhit 元素触发  电");
                            refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target,
                                new Buff_110
                                {
                                    id = 110, permanent = false, carrier = target, caster = inData.entity,
                                    duration = elementConfig[index].time / 1000f
                                }.ToBuff());
                        }
                    }
                }


                //for (int i = 0; i < buffs.Length; i++)
                //{
                //    if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_201301)
                //    {
                //        refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target, new Buff_127 { id = 127, permanent = false, carrier = target, caster = inData.entity, duration = elementConfig[index].time / 1000f }.ToBuff());
                //    }
                //    else if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_201302)
                //    {
                //        refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target, new Buff_126 { id = 126, permanent = false, carrier = target, caster = inData.entity, duration = elementConfig[index].time / 1000f }.ToBuff());
                //    }
                //    else if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_202301)
                //    {
                //        refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target, new Buff_125 { id = 125, permanent = false, carrier = target, caster = inData.entity, duration = elementConfig[index].time / 1000f }.ToBuff());
                //    }
                //    else if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_202303)
                //    {
                //        refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target, new Buff_110 { id = 110, permanent = false, carrier = target, caster = inData.entity, duration = elementConfig[index].time / 1000f }.ToBuff());
                //    }
                //}
            }
        }

        private readonly void AddElement(ref SkillAttackData_ReadWrite refData, SkillAttackData_ReadOnly inData,
            float3 dir, ref BlobArray<int> elementList, int skillid, bool isBullet)
        {
            ref var elementsTable =
                ref inData.config.value.Value.configTbskill_effectElements.configTbskill_effectElements;
            for (int i = 0; i < elementList.Length; i++)
            {
                Debug.Log($"AddElement:{elementList[i]}");
                //Debug.LogError($"{elementList[i]}");
                //元素表！！！！
                int elementIndex = 0;
                for (int j = 0; j < elementsTable.Length; j++)
                {
                    if (elementsTable[j].id == elementList[i])
                    {
                        elementIndex = j;
                        break;
                    }
                }

                ref var element =
                    ref inData.config.value.Value.configTbskill_effectElements.configTbskill_effectElements[
                        elementIndex];

                switch (element.elementType)
                {
                    //输出元素
                    case 1:
                        // Debug.LogError(
                        //     $"target:{target.Index},elements{element.id}:{element.outputType},{element.bonusType},{element.calcType}");
                        refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff
                        {
                            CurrentTypeId = (Buff.TypeId)element.elementType,
                            Int32_0 = element.id,
                            Single_1 = 0,
                            Int32_2 = 0,
                            Single_3 = 0,
                            Boolean_4 = false,
                            Entity_5 = caster,
                            Entity_6 = target,
                            int2_7 = 0,
                            Int32_8 = 0,
                            Int32_9 = 0,
                            float3_10 = dir,
                            Int32_11 = element.stateType,
                            BuffArgsNew_12 = new BuffArgsNew
                            {
                                args1 = new int4(element.outputType, element.outputTypePara[0], 0, 0),
                                args2 = new int4(element.bonusType, 0, 0, 0),
                                args3 = new int4(element.calcType, element.calcTypePara[0],
                                    element.calcTypePara.Length > 1 ? element.calcTypePara[1] : 0, 0),
                                args4 = new int4(0, 0, 0, 0)
                            },
                            Int32_14 = element.power,
                            Boolean_17 = isBullet,
                            Int32_20 = skillid
                        });
                        break;
                    //更改属性
                    case 2:
                        //Debug.LogError($"222222");
                        refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff
                        {
                            CurrentTypeId = (Buff.TypeId)element
                                .elementType,
                            Int32_0 = element
                                .id,
                            Single_1 = 0,
                            Int32_2 = 0,
                            Single_3 = element.calcTypePara.Length > 0
                                ? element.calcTypePara[0] / 1000f
                                : 0,
                            Boolean_4 = element
                                .calcType == 0
                                ? true
                                : false,
                            Entity_5 = caster,
                            Entity_6 = target,
                            int2_7 = new int2(element.changeType,
                                element.changeTypePara.Length > 0 ? element.changeTypePara[0] : 0),
                            Int32_8 = 0,
                            Int32_9 = 0,
                            float3_10 = dir,
                            Int32_11 = element
                                .stateType,
                            BuffArgsNew_12 = new BuffArgsNew
                            {
                                args1 = new int4(element
                                        .attrId,
                                    element
                                        .attrIdPara[0],
                                    0,
                                    0),
                                args2 = new int4(0,
                                    0,
                                    0,
                                    0),
                                args3 = new int4(0,
                                    0,
                                    0,
                                    0),
                                args4 = new int4(0,
                                    0,
                                    0,
                                    0)
                            },
                            Int32_14 = element.power,
                            Int32_20 = skillid
                        });
                        break;
                    case 3:
                        break;
                    case 4:
                        //控制类
                        var buff4 = new Buff
                        {
                            CurrentTypeId = (Buff.TypeId)element.elementType,
                            Int32_0 = element.id,
                            Single_1 = 0,
                            Int32_2 = 0,
                            Single_3 = element.controlTypePara.Length > 0
                                ? element.controlTypePara[0] / 1000f
                                : 0,
                            Boolean_4 = false,
                            Entity_5 = caster,
                            Entity_6 = target,
                            int2_7 = new int2(element.changeType, 0),
                            Int32_8 = element.clearType,
                            Int32_9 = 0,
                            float3_10 = dir,
                            Int32_11 = element.stateType,
                            BuffArgsNew_12 = new BuffArgsNew
                            {
                                args1 = new int4(element.controlType, 0, 0,
                                    0),
                                args2 = new int4(element.controlTypePara[0],
                                    element.controlTypePara.Length > 1
                                        ? element.controlTypePara[1]
                                        : 0, 0, 0),
                                args3 = new int4(0, 0, 0, 0),
                                args4 = new int4(0, 0, 0, 0)
                            },
                            Int32_14 = element.power,
                            Int32_20 = skillID
                        };
                        //refData.ecb.AppendToBuffer(inData.sortkey, target, buff4);

                        var buffs4 = refData.bfeBuff[target];
                        if (BuffHelper.TryAddBuffNew(ref buffs4, buff4))
                        {
                            refData.ecb.AppendToBuffer(inData.sortkey, target, buff4);
                        }

                        break;
                    case 5:
                        //TODO:免疫 
                        var buff5 = new Buff
                        {
                            CurrentTypeId = (Buff.TypeId)element.elementType,
                            Int32_0 = element.id,
                            Single_1 = 0,
                            Int32_2 = 0,
                            Single_3 = element.calcType == 1
                                ? element.calcTypePara[0] / 1000f
                                : 0,
                            Boolean_4 = element.calcType == 0 ? true : false,
                            Entity_5 = caster,
                            Entity_6 = target,
                            int2_7 = new int2(element.changeType, 0),
                            Int32_8 = element.clearType,
                            Int32_9 = element.calcType == 2 ? element.calcTypePara[0] : -1,
                            float3_10 = dir,
                            Int32_11 = element.stateType,
                            BuffArgsNew_12 = new BuffArgsNew
                            {
                                args1 = new int4(element.immuneType, 0, 0,
                                    0),
                                args2 = new int4(0, 0, 0, 0),
                                args3 = new int4(0, 0, 0, 0),
                                args4 = new int4(0, 0, 0, 0)
                            },
                            Int32_13 = element.immuneType == 4
                                ? element.clearType * 2000
                                : 0,
                            Int32_14 = element.power,
                            Int32_20 = skillID
                        };

                        var buffs5 = refData.bfeBuff[target];
                        if (BuffHelper.TryAddBuffNew(ref buffs5, buff5))
                        {
                            refData.ecb.AppendToBuffer(inData.sortkey, target, buff5);
                        }

                        break;
                    case 6:
                        refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff
                        {
                            CurrentTypeId = (Buff.TypeId)element.elementType,
                            Int32_0 = element.id,
                            Single_1 = 0,
                            Int32_2 = 0,
                            Single_3 = element.pointTypePara[2] / 1000f,
                            Boolean_4 = false,
                            Entity_5 = caster,
                            Entity_6 = target,
                            int2_7 = new int2(element.changeType, 0),
                            Int32_8 = element.clearType,
                            Int32_9 = 0,
                            float3_10 = dir,
                            Int32_11 = element.stateType,
                            BuffArgsNew_12 = new BuffArgsNew
                            {
                                args1 = new int4(element.displaceFrom, 0, 0, 0),
                                args2 = new int4(element.pointType, 0, 0, 0),
                                args3 = new int4(element.pointTypePara[0],
                                    element.pointTypePara[1], element.pointTypePara[2], 0),
                                args4 = new int4(element.passType, 0, 0, 0)
                            },
                            Int32_14 = element.power,
                            Int32_20 = skillID
                        });
                        break;
                }
            }
        }

        private void DoElementActionToObstacle(ElementData obstacle, ElementData skill, Entity target, Entity caster,
            ref SkillAttackData_ReadWrite refData, SkillAttackData_ReadOnly inData, ref NativeList<int> activeList)
        {
            if (skill.id == 101)
            {
                if (obstacle.id == 301)
                {
                    int index = 0;
                    ref var elementConfig =
                        ref inData.config.value.Value.configTbelement_effects.configTbelement_effects;

                    for (int i = 0; i < elementConfig.Length; i++)
                    {
                        if (elementConfig[i].from == 301 && elementConfig[i].target == 101)
                        {
                            index = i;
                            break;
                        }
                    }

                    ref var element = ref elementConfig[index];
                    Entity fireArea = default;
                    bool isinBoss = inData.cdfePlayerData[inData.player].playerOtherData.isBossFight;
                    float bossScenSize = inData.cdfePlayerData[inData.player].playerOtherData.bossScenePos.y;
                    BuffHelper.GenerateMapModule(inData.cdfePostTransformMatrix,
                        refData.cdfeLocalTransform[inData.player].Position, inData.gameOthersData, ref refData.ecb,
                        inData.config, inData.prefabMapData, inData.sortkey,
                        1001, refData.cdfeLocalTransform[target].Position, ref fireArea, isinBoss, bossScenSize);
                    Debug.Log("Onhit 障碍物触发");
                    refData.ecb.AddComponent(inData.sortkey, fireArea,
                        new TimeToDieData { duration = element.para[0] / 1000f });
                }
            }
            else if (skill.id == 102)
            {
                if (obstacle.id == 302)
                {
                    int index = 0;
                    ref var elementConfig =
                        ref inData.config.value.Value.configTbelement_effects.configTbelement_effects;

                    for (int i = 0; i < elementConfig.Length; i++)
                    {
                        if (elementConfig[i].from == 302 && elementConfig[i].target == 102)
                        {
                            index = i;
                            break;
                        }
                    }

                    ref var element = ref elementConfig[index];
                    var activeID = 102302;
                    activeList.Add(activeID);

                    refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target,
                        new Buff_102302
                        {
                            carrier = target, permanent = false, duration = element.para[2] / 1000f * 3,
                            argsNew = new BuffArgsNew { args1 = new int4(element.para[0], element.para[1], 0, 0) }
                        }.ToBuff());
                }
            }
        }

        private void DoElementActionToArea(ElementData area, ElementData skill, Entity target, Entity caster,
            ref SkillAttackData_ReadWrite refData, SkillAttackData_ReadOnly inData, ref NativeList<int> activeList)
        {
            Debug.Log("Onhit 元素触发  DoElementActionToArea");
            if (skill.id == 101)
            {
                if (area.id == 202)
                {
                    int index = 0;
                    ref var elementConfig =
                        ref inData.config.value.Value.configTbelement_effects.configTbelement_effects;

                    for (int i = 0; i < elementConfig.Length; i++)
                    {
                        if (elementConfig[i].from == 202 && elementConfig[i].target == 101)
                        {
                            index = i;
                            break;
                        }
                    }

                    ref var element = ref elementConfig[index];
                    var activeID = 101202;
                    activeList.Add(activeID);
                    Debug.Log("Onhit 加buff 101202");
                }
            }
            else if (skill.id == 102)
            {
                if (area.id == 201)
                {
                    Debug.Log("Onhit 元素触发 area.id == 201");
                    refData.ecb.AppendToBuffer(inData.sortkey, inData.wbe, new DamageInfo
                    {
                        attacker = default,
                        defender = target,
                        tags = new DamageInfoTag
                        {
                            directDamage = true
                        },
                        damage = new Damage
                        {
                            normal = (int)math.floor(1),
                        },
                        criticalRate = 0,
                        criticalDamage = 0,
                        hitRate = 1,
                        degree = 0,
                        pos = 0,
                        bulletEntity = default,
                    });
                }
                else if (area.id == 203)
                {
                    int index = 0;
                    ref var elementConfig =
                        ref inData.config.value.Value.configTbelement_effects.configTbelement_effects;

                    for (int i = 0; i < elementConfig.Length; i++)
                    {
                        if (elementConfig[i].from == 203 && elementConfig[i].target == 102)
                        {
                            index = i;
                            break;
                        }
                    }

                    ref var element = ref elementConfig[index];
                    var activeID = 101203;
                    activeList.Add(activeID);
                    //给技能上了一个通电的能力
                    Debug.Log("Onhit 加buff 102203");
                    //refData.ecb.AppendToBuffer<Buff>(inData.sortkey, caster, new Buff_102203 { permanent = true }.ToBuff());
                }
            }
            else if (skill.id == 103)
            {
                if (area.id == 202)
                {
                    int index = 0;
                    ref var elementConfig =
                        ref inData.config.value.Value.configTbelement_effects.configTbelement_effects;

                    for (int i = 0; i < elementConfig.Length; i++)
                    {
                        if (elementConfig[i].from == 202 && elementConfig[i].target == 103)
                        {
                            index = i;
                            break;
                        }
                    }

                    ref var element = ref elementConfig[index];
                    //生成闪电效果
                    Debug.Log("Onhit 加buff 103202");
                    var activeID = 103202;
                    activeList.Add(activeID);
                    refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target,
                        new Buff_103202
                        {
                            permanent = false, duration = element.para[0] / 1000f,
                            argsNew = new BuffArgsNew { args1 = new int4(element.para[0]) }, carrier = target
                        }.ToBuff());
                }
            }
        }

        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            // if (id == 1003063)
            // {
            //     var buff1 = BuffHelper.UpdateParmeter(inData.config, id, 20003001);
            //     var rate = buff1[1].x;
            //     var duration = buff1[0].x / 1000f;
            //     var probability = buff1[2].x;
            //     for (int i = 0; i < inData.otherEntities.Length; i++)
            //     {
            //         var rand = inData.rand.NextInt(probability, 10000);
            //         if (rand <= probability)
            //         {
            //             refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
            //                 new Buff_20003001
            //                 {
            //                     id = 20003001,
            //                     priority = 0,
            //                     maxStack = 0,
            //                     tags = 0,
            //                     tickTime = 0,
            //                     timeElapsed = 0,
            //                     ticked = 0,
            //                     duration = duration,
            //                     permanent = false,
            //                     caster = default,
            //                     buffArgs = new BuffArgs
            //                     {
            //                         args0 = rate
            //                     },
            //                     carrier = inData.otherEntities[i],
            //                     canBeStacked = false,
            //                     buffStack = default,
            //                 }.ToBuffOld());
            //         }
            //
            //         var buff2 = BuffHelper.UpdateParmeterNew(inData.config, id, 20003009);
            //         rate = buff2[0].x;
            //         duration = buff2[1].x / 1000f;
            //         refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
            //             new Buff_20003009
            //             {
            //                 id = 20003009,
            //                 priority = 0,
            //                 maxStack = 0,
            //                 tags = 0,
            //                 tickTime = 1f,
            //                 timeElapsed = 0,
            //                 ticked = 0,
            //                 duration = duration,
            //                 permanent = false,
            //                 caster = default,
            //                 buffArgs = new BuffArgs
            //                 {
            //                     args0 = rate
            //                 },
            //                 carrier = inData.otherEntities[i],
            //                 canBeStacked = false,
            //                 buffStack = default,
            //             }.ToBuffOld());
            //     }
            // }
            // else if (id == 1003073)
            // {
            //     for (int i = 0; i < inData.otherEntities.Length; i++)
            //     {
            //         refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
            //             new Buff_20003008
            //             {
            //                 id = 20003008,
            //                 priority = 0,
            //                 maxStack = 0,
            //                 tags = 0,
            //                 tickTime = 1f,
            //                 timeElapsed = 0,
            //                 ticked = 0,
            //                 duration = duration,
            //                 permanent = false,
            //                 caster = default,
            //                 buffArgs = new BuffArgs
            //                 {
            //                     args0 = id
            //                 },
            //                 carrier = inData.otherEntities[i],
            //                 canBeStacked = false,
            //                 buffStack = default,
            //             }.ToBuffOld());
            //     }
            // }
        }

        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }

        public void OnExit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            if (refData.bfeBuff.HasBuffer(inData.entity) && inData.cdfeElementData.HasComponent(inData.entity) &&
                inData.cdfeElementData.HasComponent(target))
            {
                var elementID = inData.cdfeElementData[inData.entity].id;
                var elementTargetId = inData.cdfeElementData[target].id;
                var buffs = refData.bfeBuff[inData.entity];
                for (int i = 0; i < buffs.Length; i++)
                {
                    if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_101202 && elementID == 101 && elementTargetId == 202)
                    {
                        buffs.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}