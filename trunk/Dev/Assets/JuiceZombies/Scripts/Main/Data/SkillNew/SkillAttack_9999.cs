using System;
using System.Globalization;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Main
{
    //非触发类型的触发器通用  没有位置大小方向变动的技能实体 只存在一帧
    public partial struct SkillAttack_9999 : ISkillAttack
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
            var c = refData.cdfePhysicsCollider[inData.entity];

            //BuffHelper.SetSkillAttackTarget(refData.ecb, inData.sortkey, inData.entity, inData.config, id, c);

            return refData.cdfeLocalTransform[inData.entity];
        }

        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
        }

        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            //duration = 0.1f;
            if (!refData.storageInfoFromEntity.Exists(caster)) return;
            enableExit = true;
        }

        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
        {
            //地形和障碍物的元素反应
            NativeList<int> activeList = new NativeList<int>(10, Allocator.Temp);
            var entities = inData.otherEntities;

            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                if (refData.storageInfoFromEntity.Exists(entity))
                {
                    if (inData.cdfeObstacleTag.HasComponent(entity))
                    {
                        if (inData.cdfeElementData.HasComponent(entity) &&
                            inData.cdfeElementData.HasComponent(inData.entity))
                        {
                            DoElementAction(inData.cdfeElementData[entity], inData.cdfeElementData[inData.entity],
                                entity, inData.entity, ref refData, inData, ref activeList);
                        }
                    }
                }
            }


            //地形的效果作用
            if (inData.cdfeChaStats.HasComponent(target) && !inData.cdfemapElementData.HasComponent(target))
            {
                var buffs = refData.bfeBuff[inData.entity];
                ref var elementConfig = ref inData.config.value.Value.configTbelements.configTbelements;
                ref var elementEffectConfig =
                    ref inData.config.value.Value.configTbelement_effects.configTbelement_effects;
                int index = -1;
                //Debug.LogError($"Onhit 元素触发 地形上有{buffs.Length}个buff");
                if (inData.cdfeElementData.HasComponent(inData.entity))
                {
                    var elementID = inData.cdfeElementData[inData.entity].id;
                    for (int i = 0; i < elementConfig.Length; i++)
                    {
                        if (elementConfig[i].id == elementID)
                        {
                            index = i;
                            break;
                        }
                    }

                    var elementDuration = elementConfig[index].time / 1000f;
                    //火
                    if (elementID == 201)
                    {
                        //var elementTarget = inData.cdfeElementData[target];
                        //var elementCaster = inData.cdfeElementData[inData.entity];
                        bool isActive = true;
                        for (int i = 0; i < buffs.Length; i++)
                        {
                            if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_201301 ||
                                buffs[i].CurrentTypeId == Buff.TypeId.Buff_201302)
                            {
                                isActive = false;
                                break;
                            }
                        }
                        bool isContains=false;
                        var buffsTarget = refData.bfeBuff[target];
                        foreach (var buff in buffsTarget)
                        {
                            if (buff.Int32_0 == 102)
                            {
                                Debug.Log("states isContains");
                                isContains = true;
                                break;
                            }
                        }
                        if (isActive&&!isContains)
                        {
                            refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target,
                                new Buff_102
                                {
                                    id = 102, permanent = false, carrier = target, caster = inData.entity,
                                    duration = elementDuration
                                }.ToBuff());
                        }

                        //if (activeList.Contains(201301))
                        //{

                        //}
                        //else if (activeList.Contains(201302))
                        //{


                        //}
                        //else
                        //{
                        //    //Debug.LogError("Onhit 元素触发  201");

                        //}
                    }
                    //水
                    else if (elementID == 202)
                    {
                        if (activeList.Contains(202301))
                        {
                            Debug.LogError("Onhit 元素触发 202301");
                            //for (int i = 0; i < elementEffectConfig.Length; i++)
                            //{
                            //    if (elementEffectConfig[i].from == elementID && elementEffectConfig[i].target == 301)
                            //    {
                            //        index = i; break;
                            //    }
                            //}

                            //refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target, new Buff_125 { id = 125, permanent = false, carrier = target, caster = inData.entity }.ToBuff());
                        }
                        else if (activeList.Contains(202303))
                        {
                        }
                        else
                        {
                            bool isContains = false;
                            var buffsTarget = refData.bfeBuff[target];
                            foreach (var buff in buffsTarget)
                            {
                                if (buff.Int32_0 == 124)
                                {
                                    Debug.Log("states isContains");
                                    isContains = true;
                                    break;
                                }
                            }
                            //Debug.LogError("Onhit 元素触发  202");
                            if (!isContains)
                            {
                                refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target,
    new Buff_124
    {
        id = 124,
        permanent = false,
        carrier = target,
        caster = inData.entity,
        duration = elementDuration
    }.ToBuff());
                            }

                        }
                    }
                    //电
                    else if (elementID == 203)
                    {

                        bool isContains = false;
                        var buffsTarget = refData.bfeBuff[target];
                        foreach (var buff in buffsTarget)
                        {
                            if (buff.Int32_0 == 110)
                            {
                                Debug.Log("states isContains");
                                isContains = true;
                                break;
                            }
                        }
                        //Debug.LogError("Onhit 元素触发  202");
                        if (!isContains)
                        {
                            //Debug.LogError("Onhit 元素触发  203");
                            refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target,
                            new Buff_110
                            {
                                id = 110,
                                permanent = false,
                                carrier = target,
                                caster = inData.entity,
                                duration = elementDuration
                            }.ToBuff());
                        }

                    }
                    //油
                    else
                    {
                        bool isContains = false;
                        var buffsTarget = refData.bfeBuff[target];
                        foreach (var buff in buffsTarget)
                        {
                            if (buff.Int32_0 == 125)
                            {
                                Debug.Log("states isContains");
                                isContains = true;
                                break;
                            }
                        }
                        //Debug.LogError("Onhit 元素触发  202");
                        if (!isContains)
                        {
                            //Debug.LogError("Onhit 元素触发  204");
                            refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target,
                            new Buff_125
                            {
                                id = 125,
                                permanent = false,
                                carrier = target,
                                caster = inData.entity,
                                duration = elementDuration
                            }.ToBuff());
                        }

                    }

                    for (int i = 0; i < buffs.Length; i++)
                    {
                        //if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_103202)
                        //{
                        //    for (int j = 0; j < elementConfig.Length; j++)
                        //    {
                        //        if (elementConfig[j].id == 203)
                        //        {
                        //            index = j; break;
                        //        }
                        //    }
                        //    var duration = elementConfig[index].time / 1000f;
                        //    Debug.LogError("Onhit 元素触发 103202");
                        //    refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target, new Buff_110 { id = 110, permanent = false, carrier = target, caster = inData.entity, duration = duration }.ToBuff());
                        //}
                    }
                }
            }
        }

        private void DoElementAction(ElementData obstacle, ElementData area, Entity target, Entity caster,
            ref SkillAttackData_ReadWrite refData, SkillAttackData_ReadOnly inData, ref NativeList<int> activeList)
        {
            if (area.id == 201)
            {
                if (obstacle.id == 301)
                {
                    int index = 0;
                    ref var elementConfig =
                        ref inData.config.value.Value.configTbelement_effects.configTbelement_effects;

                    for (int i = 0; i < elementConfig.Length; i++)
                    {
                        if (elementConfig[i].from == 201 && elementConfig[i].target == 301)
                        {
                            index = i;
                            break;
                        }
                    }

                    ref var element = ref elementConfig[index];
                    int dur = element.para[1], coodDown = element.para[2];

                    //给地形上了一个灼烧别人的能力 caster为地形
                    activeList.Add(201301);
                    refData.ecb.AppendToBuffer<Buff>(inData.sortkey, caster,
                        new Buff_201301
                        {
                            carrier = caster, permanent = true,
                            argsNew = new BuffArgsNew { args1 = new int4(dur, coodDown, 0, 0) }
                        }.ToBuff());
                }
                else if (obstacle.id == 302)
                {
                    activeList.Add(201302);
                    int index = 0;
                    ref var elementConfig =
                        ref inData.config.value.Value.configTbelement_effects.configTbelement_effects;

                    for (int i = 0; i < elementConfig.Length; i++)
                    {
                        if (elementConfig[i].from == 201 && elementConfig[i].target == 302)
                        {
                            index = i;
                            break;
                        }
                    }

                    ref var element = ref elementConfig[index];
                    int dur = element.para[1], coodDown = element.para[2];

                    //给地形上了一个灼烧别人的能力 caster为地形
                    activeList.Add(201302);
                    refData.ecb.AppendToBuffer<Buff>(inData.sortkey, caster,
                        new Buff_201302
                        {
                            carrier = caster, permanent = true,
                            argsNew = new BuffArgsNew { args1 = new int4(dur, coodDown, 0, 0) }
                        }.ToBuff());
                }
            }
            else if (area.id == 202)
            {
                //变成有油的水潭
                if (obstacle.id == 301)
                {
                    int index = 0;
                    ref var elementConfig =
                        ref inData.config.value.Value.configTbelement_effects.configTbelement_effects;

                    for (int i = 0; i < elementConfig.Length; i++)
                    {
                        if (elementConfig[i].from == 202 && elementConfig[i].target == 301)
                        {
                            index = i;
                            break;
                        }
                    }

                    ref var element = ref elementConfig[index];
                    var dur = element.para[0];
                    activeList.Add(202301);
                    //生成油滴
                    var ins = inData.prefabMapData.prefabHashMap["you_test"];
                    refData.ecb.Instantiate(inData.sortkey, ins);
                    refData.ecb.SetComponent(inData.sortkey, ins,
                        new LocalTransform
                        {
                            Position = refData.cdfeLocalTransform[caster].Position,
                            Scale = refData.cdfeLocalTransform[caster].Scale * 4
                        });
                    refData.ecb.AddComponent(inData.sortkey, ins, new TimeToDieData { duration = dur / 1000f });
                    refData.ecb.AppendToBuffer<Buff>(inData.sortkey, inData.entity,
                        new Buff_202301 { carrier = inData.entity, duration = dur / 1000f }.ToBuff());
                }
                //通电效果
                else if (obstacle.id == 303)
                {
                    //生成电特效
                    activeList.Add(202303);
                    //refData.ecb.Instantiate(inData.sortkey,)
                    refData.ecb.AppendToBuffer<Buff>(inData.sortkey, inData.entity,
                        new Buff_202303 { carrier = inData.entity, id = 202303, permanent = true }.ToBuff());
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
            if (enableExit)
            {
                if (inData.cdfeObstacleTag.HasComponent(target))
                {
                    if (inData.cdfeElementData.HasComponent(target) &&
                        inData.cdfeElementData.HasComponent(inData.entity) && refData.bfeBuff.HasBuffer(inData.entity))
                    {
                        var elementTarget = inData.cdfeElementData[target];
                        var elementCaster = inData.cdfeElementData[inData.entity];
                        var buffs = refData.bfeBuff[inData.entity];
                        if (elementCaster.id == 203)
                        {
                            if (elementTarget.id == 303)
                            {
                                for (int i = 0; i < buffs.Length; i++)
                                {
                                    if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_202303)
                                    {
                                        var buff = buffs[i];
                                        buff.Boolean_4 = false;
                                        buff.Single_3 = 0;
                                        buffs[i] = buff;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (elementCaster.id == 201)
                        {
                            if (elementTarget.id == 301)
                            {
                                for (int i = 0; i < buffs.Length; i++)
                                {
                                    if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_201301)
                                    {
                                        var buff = buffs[i];
                                        buff.Boolean_4 = false;
                                        buff.Single_3 = 0;
                                        buffs[i] = buff;
                                        break;
                                    }
                                }
                            }
                            else if (elementTarget.id == 302)
                            {
                                for (int i = 0; i < buffs.Length; i++)
                                {
                                    if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_201302)
                                    {
                                        var buff = buffs[i];
                                        buff.Boolean_4 = false;
                                        buff.Single_3 = 0;
                                        buffs[i] = buff;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}