using System.Threading;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    //旋涡和推开 用于boss犀牛
    public partial struct SkillAttack_888 : ISkillAttack
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



        /// <summary>
        /// 冲锋的速度
        /// </summary>
        private float velocity;

        /// <summary>
        /// 冲锋的方向
        /// </summary>
        public float3 crashDir;

        public float angle;

        private float2 velocityLimit;

        private float2 disLimit;

        private float3 oldSpeed;

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
            var acc = acceleration / 1000f;
            speed += acc * inData.fDT;

            return refData.cdfeLocalTransform[inData.entity];
        }

        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        
        }

        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            if (!refData.storageInfoFromEntity.Exists(caster)) return;
            isActive=false;

            //oldSpeed = refData.cdfePhysicsVelocity[carrier].Linear;

            //Debug.LogError($"SkillAttack888 {duration}");
            //Debug.LogError($"OnStartOnStartOnStartOnStartOnStartOnStartOnStartOnStartOnStartOnStartOnStart");
            //Debug.LogError($"dirdirdir:{dir},targetpos:{targetPos}");
            int skillEffectIndex = 0;
            ref var skillEffect = ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            ref var elementConfig =
                ref inData.config.value.Value.configTbskill_effectElements.configTbskill_effectElements;
            for (int i = 0; i < skillEffect.Length; i++)
            {
                if (skillEffect[i].id == id)
                {
                    skillEffectIndex = i;
                    break;
                }
            }

            ref var elementList = ref skillEffect[skillEffectIndex].elementList;

            int index = 0;
            for (int k = 0; k < elementList.Length; k++)
            {
                for (int m = 0; m < elementConfig.Length; m++)
                {
                    if (elementList[k] == elementConfig[m].id && elementConfig[m].elementType == 6)
                    {
                        index = m;
                        break;
                    }
                }
            }

            float3 crashPos = default;
            ref var forceMove = ref elementConfig[index];


            if (forceMove.pointType == 3)
            {
                angle = forceMove.pointTypePara[2];
                duration = forceMove.pointTypePara[1] / 1000f;
                velocity = forceMove.pointTypePara[0] / 1000f;
                // isOnStayTrigger = true;
            }
            else
            {
                angle = -1;
                duration = forceMove.pointTypePara[0] / 1000f;
                velocityLimit = new float2(forceMove.pointTypePara[2] / 1000f, forceMove.pointTypePara[1] / 1000f);
                disLimit = new float2(forceMove.pointTypePara[4] / 1000f, forceMove.pointTypePara[3] / 1000f);
            }


            //if (forceMove.passType == 1)
            //{
            //    Debug.LogError("碰撞盒改变");
            //    var physicCollider = refData.cdfePhysicsCollider[caster];
            //    unsafe
            //    {
            //        colliderWith = refData.cdfePhysicsCollider[caster].ColliderPtr->GetCollisionFilter().CollidesWith;
            //    }
            //     PhysicsHelper.CreateColliderWithCollidesWith(physicCollider, 0);
            //}




            int skillId = 0;
            for (int i = 0; i < skillEffect.Length; i++)
            {
                if (skillEffect[i].id == id)
                {
                    skillId = skillEffect[i].skillId;
                    break;
                }
            }

            var temp0 = refData.bfeSkill[caster];
            //在技能上记录skillEffect次数
            for (int i = 0; i < refData.bfeSkill[caster].Length; i++)
            {
                var temp = temp0[i];

                if (temp.Int32_0 == skillId)
                {
                    //Debug.Log($"OnStart");

                    if (temp.int2x4_13.c0.x == id)
                    {
                        temp.int2x4_13.c0.y++;
                    }
                    else if (temp.int2x4_13.c1.x == id)
                    {
                        temp.int2x4_13.c1.y++;
                    }
                    else if (temp.int2x4_13.c2.x == id)
                    {
                        temp.int2x4_13.c2.y++;
                    }
                    else if (temp.int2x4_13.c3.x == id)
                    {
                        temp.int2x4_13.c3.y++;
                    }
                    else
                    {
                        if (temp.int2x4_13.c0.x == 0)
                        {
                            temp.int2x4_13.c0.x = id;
                            temp.int2x4_13.c0.y = 1;
                        }
                        else if (temp.int2x4_13.c1.x == 0)
                        {
                            temp.int2x4_13.c1.x = id;
                            temp.int2x4_13.c1.y = 1;
                        }
                        else if (temp.int2x4_13.c2.x == 0)
                        {
                            temp.int2x4_13.c2.x = id;
                            temp.int2x4_13.c2.y = 1;
                        }
                        else if (temp.int2x4_13.c3.x == 0)
                        {
                            temp.int2x4_13.c3.x = id;
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
            if (!refData.cdfePhysicsVelocity.HasComponent(target)) return;

            if(isActive) return;
            oldSpeed = refData.cdfePhysicsVelocity[target].Linear;

            //refData.ecb.AppendToBuffer(inData.sortkey, target,
            //    new Buff_NotLetVIsZero { id = id, duration = duration, permanent = false, carrier = target }.ToBuff());


            float3 force = float3.zero;

            for (int i = 0; i < inData.otherEntities.Length; i++)
            {
                var entity = inData.otherEntities[i];
                if (!refData.cdfePhysicsVelocity.HasComponent(entity)) continue;
                if(inData.cdfeAreaTag.HasComponent(entity)|| inData.cdfeObstacleTag.HasComponent(entity)) continue;

                var physics = refData.cdfePhysicsVelocity[entity];


                //float3 force = float3.zero;
                if (angle != -1)
                {
                    force = math.normalizesafe(crashDir);
                    force = math.normalizesafe(new float3(MathHelper.RotateVector(force.xy, angle), 0));
                }
                else
                {
                    var dis = math.distance(refData.cdfeLocalTransform[inData.entity].Position,
                            refData.cdfeLocalTransform[entity].Position);
                    //var dis = math.clamp(
                    //    disLimit.y - math.distance(refData.cdfeLocalTransform[inData.entity].Position,
                    //        refData.cdfeLocalTransform[entity].Position), disLimit.x, disLimit.y);
                    ///朝释放者 用来吸target
                    velocity = math.min( (disLimit.y - dis)/(disLimit.y-disLimit.x),1)*(velocityLimit.y - velocityLimit.x) +velocityLimit.x;
                    Debug.LogError($"velocity:{velocity}");

                    force = math.normalizesafe(refData.cdfeLocalTransform[inData.entity].Position -
                                               refData.cdfeLocalTransform[entity].Position);
                }

                //var inversMass = 1f / (inData.cdfeChaStats[entity].chaProperty.mass * 1f);
                physics.Linear = force * velocity;
                //physics.Linear = force * 30;
             


                refData.cdfePhysicsVelocity[entity] = physics;
                Debug.LogError($"durationtime {duration}");

                var buffs = refData.bfeBuff[entity];
                bool isCanAdd=true;
                foreach(var buff in buffs)
                {
                    if (buff.CurrentTypeId == Buff.TypeId.Buff_NotLetVIsZero)
                    {
                        isCanAdd = false; break;
                    }
                }
                if (isCanAdd)
                {
                    Debug.LogError($"Buff_NotLetVIsZero");
                    refData.ecb.AppendToBuffer(inData.sortkey, entity,new Buff_NotLetVIsZero { id = id, duration = duration, permanent = false, carrier = entity }.ToBuff());
                }

            }
            isActive = true;

            ref var skillConfig = ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            int index = 0;
            for (int i = 0; i < skillConfig.Length; i++)
            {
                if (skillConfig[i].id == id)
                {
                    index = i;
                    break;
                }
            }

            ref var skilleffect = ref skillConfig[index];
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


            else if (skilleffect.rangeType == 2)
            {
                var lineDir = refData.cdfeLocalTransform[target].Position - refData.cdfeLocalTransform[caster].Position;
                var angle = MathHelper.SignedAngle(targetPos, lineDir);

                Debug.Log(
                    $"pos1:{refData.cdfeLocalTransform[target].Position},pos2:{refData.cdfeLocalTransform[caster].Position}");
                Debug.Log($"angle:{angle}");


                if (angle < (-skilleffect.rangeTypePara[0] / 2f) || angle > (skilleffect.rangeTypePara[0] / 2f))
                {
                    Debug.LogError("angele return");
                    return;
                }

                if (skilleffect.rangeTypePara.Length >= 3)
                {
                    var dis = math.distance(refData.cdfeLocalTransform[target].Position,
                        refData.cdfeLocalTransform[caster].Position);
                    float comparedis = skilleffect.rangeTypePara[2] / 1000f;
                    if (dis < comparedis)
                    {
                        Debug.LogError("distance return");
                        return;
                    }
                }
            }

            else if (skilleffect.rangeType == 3)
            {
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

            if (skilleffect.conditionType != 0)
            {
                //Debug.LogError("skilleffect.conditionType != 0");
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
                    float hp = inData.cdfeChaStats[target].chaResource.hp /
                        (inData.cdfeChaStats[target].chaProperty.maxHp * 1f) * 10000f;
                    float compareRations = skilleffect.conditionTypePara[1];
                    int compareType = skilleffect.conditionTypePara[0];
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


            if (!refData.storageInfoFromEntity.Exists(target))
            {
                Debug.LogError("storageInfoFromEntity");
                return;
            }


            //Debug.LogError($"NotMoveOnHit");
            BlobArray<int> temp = new BlobArray<int>();
            ref var elementList = ref temp;
            ref var elementTrigger = ref temp;
            ref var skilleffectconfig = ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            ref var elementsTable =
                ref inData.config.value.Value.configTbskill_effectElements.configTbskill_effectElements;


            elementList = ref skilleffect.elementList;
            elementTrigger = ref skilleffect.elementTrigger;

            ref var states = ref skilleffect.battleStatus;
            //skillEffect加状态
            BuffHelper.AddStates(ref refData, inData, ref states, target, caster, skillID);

            //技能添加的buff
            float3 dir = refData.cdfeLocalTransform[target].Position -
                         refData.cdfeLocalTransform[inData.entity].Position;
            if (refData.storageInfoFromEntity.Exists(caster))
            {
                dir = math.normalizesafe(refData.cdfeLocalTransform[target].Position -
                                         refData.cdfeLocalTransform[caster].Position);
            }

            AddElement(ref refData, inData, dir, ref elementList, skillID, isBullet);


            #region 炸弹

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
                    ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews[skillEffectIndex];
                var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(inData.timeTick + inData.entity.Index +
                                                                           inData.entity.Version));
                const int MaxPower = 10000;
                int dropRate = math.clamp(skilleffecttrigger.power, 0, MaxPower);
                bool canTrigger = rand.NextInt(0, MaxPower + 1) <= dropRate;
                if (!canTrigger)
                    return;
                ///效果id是目标的情况 不在系统处理 统一通过buff处理
                var delay = skilleffecttrigger.delayTypePara[0] / 1000f;
                // var targetEffect = skilleffecttrigger.calcTypePara[0];
                //var targetEffect = skillEffect[i].calcTypePara[0];
                refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff
                {
                    //TODO:炸弹走新技能
                    CurrentTypeId = (Buff.TypeId)elementTrigger[i],
                    Int32_0 = elementTrigger[i],
                    Single_3 = delay,
                    Boolean_4 = false,
                    Entity_5 = caster,
                    Entity_6 = target,
                });
            }

            #endregion
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
                }
            }
        }

        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }

        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }

        public void OnExit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }
    }
}