using cfg.blobstruct;
using cfg.config;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    //非触发类型的触发器通用  跟随caster移动的attack 存在duration
    public partial struct SkillAttack_999 : ISkillAttack
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

        private LocalTransform oldLoc;

        private bool isCrashSucess;

        private uint colliderWith;
        private int passType;
        private int passTypeDuration;
        private int displaceFrom;

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
            if (!refData.storageInfoFromEntity.Exists(caster))
            {
                return default;
            }

            if (inData.cdfeObstacleTag.HasComponent(target) && inData.cdfeBoardData.HasComponent(target))
            {
                Debug.LogError($"11111loc.position:{refData.cdfeLocalTransform[caster].Position}");
                velocity = 0;
                var physicsVelocity = refData.cdfePhysicsVelocity[caster];
                physicsVelocity.Linear = 0;
                physicsVelocity.Angular = 0;
                refData.cdfePhysicsVelocity[caster] = physicsVelocity;
            }

            if (inData.cdfeObstacleTag.HasComponent(target) &&
                refData.cdfePhysicsCollider[inData.entity].Value.Value.GetCollisionResponse() !=
                Unity.Physics.CollisionResponsePolicy.RaiseTriggerEvents)
            {
                Debug.LogError($"2222 loc.position:{refData.cdfeLocalTransform[caster].Position}");
                velocity = 0;
                var physicsVelocity = refData.cdfePhysicsVelocity[caster];
                physicsVelocity.Linear = 0;
                physicsVelocity.Angular = 0;
                refData.cdfePhysicsVelocity[caster] = physicsVelocity;
            }

            var casterPos = refData.cdfeLocalTransform[caster];
            casterPos.Position += velocity * crashDir;
            refData.cdfeLocalTransform[caster] = casterPos;
            var loc = refData.cdfeLocalTransform[inData.entity];
            loc.Position = casterPos.Position;
            //Debug.LogError($"速度:{velocity},方向:{crashDir}");
            return loc;
        }

        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            //冲锋完毕后 加一个临时的boss可移动 来改变boss的朝向
            refData.ecb.AppendToBuffer(inData.sortkey, caster,
                new Buff_LetBossFlip() { carrier = caster, duration = 0.04f, permanent = false }
                    .ToBuff());
            //冲锋未命中的话 添加未命中trigger
            if (!isCrashSucess && args.y != -1)
            {
                AddTrigger(inData, ref refData, caster, (int)args.y);
                Debug.Log($"未命中添加trigger，id:{(int)args.y}");
            }

            if (passType == 1&&displaceFrom!=2)
            {
                Debug.LogError($"碰撞盒还原时间：{passTypeDuration/1000f}s");
                refData.ecb.AppendToBuffer(inData.sortkey, caster, new Buff_ColliderWithNo { duration = passTypeDuration/1000f, caster = caster, carrier = caster, argsNew = new BuffArgsNew { args1 = new int4((int)colliderWith,0,0,0) } }.ToBuff());
             
                //var physicCollider = refData.cdfePhysicsCollider[caster];
                //var newCollider = PhysicsHelper.CreateColliderWithCollidesWith(physicCollider, colliderWith);
                //refData.cdfePhysicsCollider[caster] = newCollider;
            }
        }

        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            if (!refData.storageInfoFromEntity.Exists(caster)) return;

            oldLoc = refData.cdfeLocalTransform[inData.entity];
            //Debug.LogError($"SkillAttack999 {duration}");
            //Debug.LogError($"OnStartOnStartOnStartOnStartOnStartOnStartOnStartOnStartOnStartOnStartOnStart");
            //Debug.LogError($"dirdirdir:{crashDir},targetpos:{targetPos}");
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

            //ref var deviatPar= ref skillEffect[skillEffectIndex].d;



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

          
            ref var forceMove = ref elementConfig[index];
            passType = forceMove.passType;
            duration = forceMove.pointTypePara[2] / 1000f;
            var dirType = forceMove.directionType;
            if (dirType == 1)
            {
                crashDir = -crashDir;
            }
            if (passType == 1)
            {
                passTypeDuration = forceMove.passTypePara[0];
            }

            if (passType == 1)
            {

                var physicCollider = refData.cdfePhysicsCollider[caster];
                unsafe
                {
                    colliderWith = refData.cdfePhysicsCollider[caster].ColliderPtr->GetCollisionFilter().CollidesWith;
                }

                var newCollider = PhysicsHelper.CreateColliderWithCollidesWith(physicCollider, 0);
                refData.cdfePhysicsCollider[caster] = newCollider;
                //Debug.LogError($"碰撞盒改变:{newCollider.Value.Value.GetCollisionFilter().CollidesWith}");
            }
            if (crashPos.x == 0 && crashPos.y == 0 && crashPos.z == 0)
            {
                var pointType = forceMove.pointType;
                ref var pointTypePar = ref forceMove.pointTypePara;
                displaceFrom = forceMove.displaceFrom;
                if (pointType == 0)
                {
                    float distance = pointTypePar[0] / 1000f;

                    crashPos = GetRandomPointOutsideLine(inData, ref refData, displaceFrom,
                        distance - pointTypePar[1] / 1000f,
                        distance + pointTypePar[1] / 1000f, crashDir);
                    //Debug.LogError($"crashPos0:{crashPos}");
                    crashDir = math.normalize(crashPos - refData.cdfeLocalTransform[caster].Position);
                }
                else if (pointType == 1)
                {
                    float radius = pointTypePar[0] / 1000f;
                    crashPos = GetRandomPointOutsideCircle(ref refData, inData, displaceFrom, pointTypePar[1] / 1000f,
                        radius, caster);

                    //Debug.LogError($"crashPos1:{crashPos}");
                }
                else if (pointType == 4)
                {
                    float crashX = 0f;
                    if (refData.cdfeLocalTransform[caster].Position.x - targetPos.x > 0)
                    {
                        crashX = targetPos.x + pointTypePar[0] / 1000f;
                    }
                    else
                    {
                        crashX = targetPos.x - pointTypePar[0] / 1000f;
                    }
                    var crashY = targetPos.y + pointTypePar[1] / 1000f;
                    crashPos = new float3(crashX, crashY, 0);
                }
                else if (pointType == 5)
                {

                    var playerDir = inData.cdfeChaStats[inData.player].chaResource.direction;
                    if (playerDir.x > 0)
                    {
                        crashPos.x = targetPos.x - pointTypePar[0] / 1000f;
                    }
                    else
                    {
                        crashPos.x = targetPos.x + pointTypePar[0] / 1000f;
                    }
                    crashPos.y = targetPos.y + pointTypePar[1] / 1000f;

                    if (!BuffHelper.IsInBossMap(crashPos, inData.cdfePlayerData[inData.player].playerOtherData))
                    {
                        var dir = crashPos - refData.cdfeLocalTransform[inData.player].Position;
                        crashPos = refData.cdfeLocalTransform[inData.player].Position - dir;
                    }


                }
            }
          

        
         
            var trueDis = math.distance(refData.cdfeLocalTransform[caster].Position, crashPos);

            if (float.IsNaN(trueDis))
            {
                trueDis = 0;
            }


            if (duration != 0)
            {
                velocity = (trueDis / duration) * inData.fDT;
            }
            else
            {
                velocity = 0;
                var loc = refData.cdfeLocalTransform[caster];
               
                loc.Position = crashPos;
                refData.cdfeLocalTransform[caster] = loc;
            }

            var speedRations = inData.cdfeChaStats[caster].chaProperty.maxMoveSpeedRatios > 0
                ? inData.cdfeChaStats[caster].chaProperty.maxMoveSpeedRatios
                : 0;
            velocity = velocity * (1 + speedRations / 10000f);


            Debug.LogError($"velocity:{velocity},truDis:{trueDis}");
            //velocity = 2 * inData.fDT;
            //Debug.LogError($"velocity:{velocity},duration:{duration},caster{caster.Index},player{inData.player.Index},trueDis:{trueDis}");

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

            if (targetPos.x == 0 && targetPos.y == 0 && targetPos.z == 0)
            {
                return;
            }
        }


        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }


        public void OnExit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }


        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            if (inData.cdfePlayerData.HasComponent(target))
            {
                Debug.LogError(
                    $"SkillAttack_99999 OnHitOnHit SkillAttack_99999 OnHitOnHitSkillAttack_99999 OnHitOnHit ");
                isCrashSucess = true;
                if (args.x != -1)
                {
                    AddTrigger(inData, ref refData, caster, (int)args.x);
                    Debug.Log($"命中添加trigger，id:{(int)args.x}");
                }
            }

            if (isCrashSucess)
            {
                if (args.w == 1)
                {
                    velocity = 0;
                    Debug.LogError("args.w == 1");
                }
            }


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


            float3 dir = refData.cdfeLocalTransform[target].Position -
                         refData.cdfeLocalTransform[inData.entity].Position;
            if (refData.storageInfoFromEntity.Exists(caster))
            {
                dir = math.normalizesafe(refData.cdfeLocalTransform[target].Position -
                                         refData.cdfeLocalTransform[caster].Position);
            }

            ref var elemts = ref skilleffect.elementList;
            ref var states = ref skilleffect.battleStatus;
            ref var elementTrigger = ref skilleffect.elementTrigger;
            AddElement(ref refData, inData, dir, ref elemts, skillID, false);
            BuffHelper.AddStates(ref refData, inData, ref states, target, caster, skillID);

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
        }

        private void AddElement(ref SkillAttackData_ReadWrite refData, SkillAttackData_ReadOnly inData,
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
                                args1 = new int4(element.outputType,element.outputTypePara[0], 0, 0),
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

        private float3 GetRandomPointOutsideCircle(ref SkillAttackData_ReadWrite refData,
            SkillAttackData_ReadOnly inData, int displaceFrom,
            float offset, float radius, Entity caster)
        {
            var tempPos = displaceFrom == 1 ? refData.cdfeLocalTransform[caster].Position : targetPos;
            var initPos = BuffHelper.GetRandomPointInCircle(tempPos, radius + offset, radius - offset, inData.seed);
            //圆 地图外
            if (!BuffHelper.IsInBossMap(initPos,inData.cdfePlayerData[inData.player].playerOtherData))
            {
                var dir = initPos - refData.cdfeLocalTransform[inData.player].Position;
                initPos = refData.cdfeLocalTransform[inData.player].Position - dir;
                return initPos;
            }
            else
            {
                if (BuffHelper.IsRectCanUse(initPos, caster, inData.config, inData.targetEntities,
                   inData.cdfemapElementData, refData.cdfeLocalTransform, refData.cdfePhysicsCollider, inData.cdfePlayerData[inData.player].playerOtherData, inData.player))
                {
                    return initPos;
                }
                else
                {
                    //根据规则扩大选点
                    int initStep = 0;
                    float3 pos = initPos;
                    do
                    {
                        initStep += 2;
                        int times = 5;

                        do
                        {
                            times--;
                            pos = BuffHelper.GetRandomPointInCircle(initPos, initStep, initStep - 2,
                                (uint)(inData.seed + times + initStep));
                        } while (!BuffHelper.IsRectCanUse(pos, caster, inData.config, inData.targetEntities,
                                     inData.cdfemapElementData, refData.cdfeLocalTransform, refData.cdfePhysicsCollider, inData.cdfePlayerData[inData.player].playerOtherData, inData.player) &&
                                 times > 0);
                    } while (!BuffHelper.IsRectCanUse(pos, caster, inData.config, inData.targetEntities,
                                 inData.cdfemapElementData, refData.cdfeLocalTransform, refData.cdfePhysicsCollider, inData.cdfePlayerData[inData.player].playerOtherData, inData.player) &&
                             initStep < 100);

                    if (math.length(pos - initPos) < math.EPSILON)
                    {
                        return refData.cdfeLocalTransform[caster].Position;
                    }

                    return pos;
                }
            }
           
        }

        private float3 GetRandomPointOutsideLine(SkillAttackData_ReadOnly inData, ref SkillAttackData_ReadWrite refData,
            int displaceFrom, float x, float y, float3 dir)
        {
            dir = displaceFrom == 1 ? inData.cdfeChaStats[caster].chaResource.direction : dir;

            var initPos = BuffHelper.GetRandomPointOutsideLine(caster, refData.cdfeLocalTransform, inData.cdfeChaStats,
                x, y, dir, inData.seed);
            if (!BuffHelper.IsInBossMap(initPos, inData.cdfePlayerData[inData.player].playerOtherData))
            {
                float3 pos=initPos;
                if (BuffHelper.TryGetIntersectionWithVertical(new float2(initPos.xy), new float2(crashDir.xy), new float2(0, 1), out float2 intersection))
                {
                    pos.x = intersection.x;
                    pos.y = intersection.y;
                    return pos;
                }
                return initPos;
            }
            else
            {
                if (BuffHelper.IsRectCanUse(initPos, caster, inData.config, inData.targetEntities,
                  inData.cdfemapElementData, refData.cdfeLocalTransform, refData.cdfePhysicsCollider, inData.cdfePlayerData[inData.player].playerOtherData, inData.player))
                {
                    return initPos;
                }
                else
                {
                    //根据规则扩大选点
                    int initStep = 0;
                    float3 pos = initPos;
                    do
                    {
                        initStep += 2;
                        int times = 5;

                        do
                        {
                            times--;
                            pos = BuffHelper.GetRandomPointInCircle(initPos, initStep, initStep - 2,
                                (uint)(inData.seed + times + initStep));
                        } while (!BuffHelper.IsRectCanUse(pos, caster, inData.config, inData.targetEntities,
                                     inData.cdfemapElementData, refData.cdfeLocalTransform, refData.cdfePhysicsCollider, inData.cdfePlayerData[inData.player].playerOtherData, inData.player) &&
                                 times > 0);
                    } while (!BuffHelper.IsRectCanUse(pos, caster, inData.config, inData.targetEntities,
                                 inData.cdfemapElementData, refData.cdfeLocalTransform, refData.cdfePhysicsCollider, inData.cdfePlayerData[inData.player].playerOtherData, inData.player) &&
                             initStep < 100);

                    if (math.length(initPos - pos) < math.EPSILON)
                    {
                        return refData.cdfeLocalTransform[caster].Position;
                    }

                    return pos;
                }
            }
          
        }


        /// <summary>
        /// 首次添加一个trigger 额外添加 
        /// </summary>
        /// <param name="skill">添加该trigger的技能信息</param>
        /// <param name="sortKey"></param>
        /// <param name="caster">技能携带者 trigger的持有者</param>
        /// <param name="triggers"></param>
        /// <param name="skillEffect"></param>
        /// <param name="filterTriggersIndex">需要添加的trigger</param>
        private void AddTrigger(SkillAttackData_ReadOnly inData, ref SkillAttackData_ReadWrite refData, Entity caster,
            int triggerID)
        {
            ref var skillEffect =
                ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
            int index = 0;
            for (int i = 0; i < skillEffect.Length; i++)
            {
                if (skillEffect[i].id == triggerID)
                {
                    index = i;
                    break;
                }
            }

            ref var skilleffect = ref skillEffect[index];

            ///先判断是否能添加trigger
            int power = skilleffect.power;
            var rand = Unity.Mathematics.Random.CreateFromIndex((uint)(inData.timeTick + caster.Index +
                                                                       caster.Version));
            var dropRate = math.clamp(power, 0, 10000);

            var canTrigger = rand.NextInt(0, 10001) <= dropRate;
            if (!canTrigger)
                return;


            //Debug.Log($"!!!!!!!!!!!!!!!!!!!!!!!!isContainisContainisContain:{skilleffect.id}");
            float delay = 0;
            TriggerType type = (TriggerType)skilleffect.triggerType;
            int delayType = skilleffect.delayType;
            float4 triggerTypeArgs = default;
            int4 triggerConditionTypeArgs = default;
            TriggerConditionType condition = default;
            condition = (TriggerConditionType)(skilleffect.conditionType == 4
                ? skilleffect.conditionType * 10 + skilleffect.conditionTypePara[0]
                : skilleffect.conditionType);
            for (int j = 0; j < skilleffect.triggerTypePara.Length; j++)
            {
                switch (j)
                {
                    case 0:
                        triggerTypeArgs.x = skilleffect.triggerTypePara[j];
                        if (type == TriggerType.PerTime)
                        {
                            triggerTypeArgs.x = skilleffect.triggerTypePara[j] / 1000f;
                        }

                        if (type == TriggerType.PerNum)
                        {
                            if (skilleffect.triggerTypePara.Length == 1)
                            {
                                triggerTypeArgs.y = MathHelper.MaxNum;
                            }
                        }

                        break;
                    case 1:
                        triggerTypeArgs.y = skilleffect.triggerTypePara[j];
                        if (type == TriggerType.PerNum)
                        {
                            if (skilleffect.triggerTypePara[j] == 0)
                            {
                                triggerTypeArgs.y = MathHelper.MaxNum;
                            }
                        }

                        break;
                    case 2:
                        triggerTypeArgs.z = skilleffect.triggerTypePara[j];
                        break;
                    case 3:
                        triggerTypeArgs.w = skilleffect.triggerTypePara[j];
                        break;
                }
            }

            for (int j = 0; j < skilleffect.conditionTypePara.Length; j++)
            {
                switch (j)
                {
                    case 0:
                        triggerConditionTypeArgs.x = skilleffect.conditionTypePara[j];
                        break;
                    case 1:
                        triggerConditionTypeArgs.y = skilleffect.conditionTypePara[j];
                        break;
                    case 2:
                        triggerConditionTypeArgs.z = skilleffect.conditionTypePara[j];
                        break;
                    case 3:
                        triggerConditionTypeArgs.w = skilleffect.conditionTypePara[j];
                        break;
                }
            }

            for (int j = 0; j < skilleffect.delayTypePara.Length; j++)
            {
                switch (j)
                {
                    case 0:
                        delay = skilleffect.delayTypePara[j] / 1000f;
                        break;
                }
            }

            float haloGap = 0.5f;
            if (skilleffect.triggerType == (int)TriggerType.Halo)
            {
                if (skilleffect.triggerTypePara[0] != 0)
                {
                    haloGap = skilleffect.triggerTypePara[0] / 1000f;
                }
            }


            //后延不算在延迟里面
            if (delayType == 4)
            {
                delay = 0f;
            }

            triggerTypeArgs.z = triggerTypeArgs.y;

            refData.ecb.AppendToBuffer(inData.sortkey, caster, new Trigger
            {
                CurrentTypeId = (Trigger.TypeId)101,
                Int32_0 = skilleffect.id,
                Single_1 = 0,
                Int32_2 = 0,
                float3_3 = new float3(delayType,
                    delay,
                    delay),
                Boolean_4 = false,
                TriggerType_5 = type,
                float4_6 = triggerTypeArgs,
                TriggerConditionType_7 = condition,
                int4_8 = triggerConditionTypeArgs,
                float4_9 = default,
                Int32_10 = skillID,
                Boolean_11 = false,
                Entity_12 = caster,
                Boolean_13 = false,
                Int32_14 = power,
                Single_15 = haloGap,
                Int32_16 = 0,
                Int32_17 = skilleffect.compareType,
                Entity_20 = caster,
                Boolean_22 = type != TriggerType.Halo ? true : false,
                Boolean_26 = true
            });
        }
    }
}