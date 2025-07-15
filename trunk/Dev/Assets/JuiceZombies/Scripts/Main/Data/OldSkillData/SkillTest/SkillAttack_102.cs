//using Cysharp.Threading.Tasks;
//using Unity.Entities;
//using Unity.Entities.UniversalDelegates;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace Main
//{
//    /// <summary>
//    /// 挥棒球棍
//    /// </summary>
//    public partial struct SkillAttack_102 : ISkillAttack
//    {
//        /// <summary>
//        /// 0
//        /// </summary>
//        public int id;

//        /// <summary>
//        /// 1 一次性释放的技能实体 值=0
//        /// </summary>
//        public float duration;

//        /// <summary>
//        /// 2
//        /// </summary>
//        public int tick;

//        /// <summary>
//        /// 3 技能实体释放者
//        /// </summary>
//        public Entity caster;

//        /// <summary>
//        /// 4 是否是弹幕
//        /// </summary>
//        public bool isBullet;

//        /// <summary>
//        /// 5 弹幕hp
//        /// </summary>
//        public int hp;

//        /// <summary>
//        /// 实体的击中目标 6
//        /// </summary>
//        public Entity target;

//        /// <summary>
//        /// 7
//        /// </summary>
//        public int4 args;

//        /// <summary>
//        /// 每帧做位置变动
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        /// <returns>变动后的LT</returns>
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            //var c = refData.cdfePhysicsCollider[inData.entity];

//            //BuffHelper.SetSkillAttackTarget(refData.ecb, inData.sortkey, inData.entity, inData.config, id, c);


//            return refData.cdfeLocalTransform[inData.entity];
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            //TODO:技能释放次数
//            for (int i = 0; i < refData.bfeSkill[caster].Length; i++)
//            {
//                var temp0 = refData.bfeSkill[caster];
//                var temp = temp0[i];

//                if (temp.Int32_0 == 1)
//                {
//                    Debug.Log($"OnStart");

//                    if (temp.int2x4_14.c0.x == id)
//                    {
//                        temp.int2x4_14.c0.y++;
//                    }
//                    else if (temp.int2x4_14.c1.x == id)
//                    {
//                        temp.int2x4_14.c1.y++;
//                    }
//                    else if (temp.int2x4_14.c2.x == id)
//                    {
//                        temp.int2x4_14.c2.y++;
//                    }
//                    else if (temp.int2x4_14.c3.x == id)
//                    {
//                        temp.int2x4_14.c3.y++;
//                    }
//                    else
//                    {
//                        if (temp.int2x4_14.c0.x == 0)
//                        {
//                            temp.int2x4_14.c0.x = id;
//                            temp.int2x4_14.c0.y = 1;
//                        }
//                        else if (temp.int2x4_14.c1.x == 0)
//                        {
//                            temp.int2x4_14.c1.x = id;
//                            temp.int2x4_14.c1.y = 1;
//                        }
//                        else if (temp.int2x4_14.c2.x == 0)
//                        {
//                            temp.int2x4_14.c2.x = id;
//                            temp.int2x4_14.c2.y = 1;
//                        }
//                        else if (temp.int2x4_14.c3.x == 0)
//                        {
//                            temp.int2x4_14.c3.x = id;
//                            temp.int2x4_14.c3.y = 1;
//                        }
//                    }

//                    temp0[i] = temp;
//                    break;
//                }
//            }
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            BlobArray<int> temp = new BlobArray<int>();
//            ref var elements = ref temp;
//            ref var triggers=ref temp;
//            ref var skillEffect = ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
//            for(int i = 0; i < skillEffect.Length; i++)
//            {
//                if (skillEffect[i].id == id)
//                {
//                    elements = ref skillEffect[i].elementList;
//                    triggers = ref skillEffect[i].elementTrigger;
//                    break;
//                }
//            }
//            for (int i = 0; i < elements.Length; i++)
//            {
//                //元素表！！！！
//                for(int j = 0; j < elements.Length; j++)
//                {

//                }
//            }

//            ///效果id是目标的情况 不在系统处理 统一通过buff处理
//            if (triggers.Length > 0)
//            {
//                for (int i = 0; i < triggers.Length; i++)
//                {
//                    for (int j = 0; j < skillEffect.Length; ++j)
//                    {
//                        if (triggers[i] == skillEffect[i].id)
//                        {
//                            var delay = skillEffect[i].delayTypePara[0] / 1000f;
//                            var targetEffect = skillEffect[i].calcTypePara[0];
//                            refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff
//                            {
//                                CurrentTypeId = (Buff.TypeId)targetEffect,
//                                Int32_0 = targetEffect,
//                                Single_3=delay,
//                                Boolean_4=false,
//                                Entity_5=caster,
//                                Entity_6=target,
//                            });
//                            //refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff_DelayBoom
//                            //{
//                            //    id = 0,
//                            //    priority = 0,
//                            //    maxStack = 0,
//                            //    tags = 0,
//                            //    tickTime = 0,
//                            //    timeElapsed = 0,
//                            //    ticked = 0,
//                            //    duration = delay,
//                            //    permanent = false,
//                            //    caster = default,
//                            //    carrier = target,
//                            //    canBeStacked = false,
//                            //    buffStack = default,
//                            //    buffArgs = default,
//                            //    totalMoveDistance = 0,
//                            //    xMetresPerInvoke = 0,
//                            //    lastPosition = default
//                            //}.ToBuffOld());
//                        }
//                    }
//                }
//            }
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (id == 1003063)
//            {
//                var buff1 = BuffHelper.UpdateParmeter(inData.config, id, 20003001);
//                var rate = buff1[1].x;
//                var duration = buff1[0].x / 1000f;
//                var probability = buff1[2].x;
//                for (int i = 0; i < inData.otherEntities.Length; i++)
//                {
//                    var rand = inData.rand.NextInt(probability, 10000);
//                    if (rand <= probability)
//                    {
//                        refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
//                            new Buff_20003001
//                            {
//                                id = 20003001,
//                                priority = 0,
//                                maxStack = 0,
//                                tags = 0,
//                                tickTime = 0,
//                                timeElapsed = 0,
//                                ticked = 0,
//                                duration = duration,
//                                permanent = false,
//                                caster = default,
//                                buffArgs = new BuffArgs
//                                {
//                                    args0 = rate
//                                },
//                                carrier = inData.otherEntities[i],
//                                canBeStacked = false,
//                                buffStack = default,
//                            }.ToBuffOld());
//                    }

//                    var buff2 = BuffHelper.UpdateParmeterNew(inData.config, id, 20003009);
//                    rate = buff2[0].x;
//                    duration = buff2[1].x / 1000f;
//                    refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
//                        new Buff_20003009
//                        {
//                            id = 20003009,
//                            priority = 0,
//                            maxStack = 0,
//                            tags = 0,
//                            tickTime = 1f,
//                            timeElapsed = 0,
//                            ticked = 0,
//                            duration = duration,
//                            permanent = false,
//                            caster = default,
//                            buffArgs = new BuffArgs
//                            {
//                                args0 = rate
//                            },
//                            carrier = inData.otherEntities[i],
//                            canBeStacked = false,
//                            buffStack = default,
//                        }.ToBuffOld());
//                }
//            }
//            else if (id == 1003073)
//            {
//                for (int i = 0; i < inData.otherEntities.Length; i++)
//                {
//                    refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
//                        new Buff_20003008
//                        {
//                            id = 20003008,
//                            priority = 0,
//                            maxStack = 0,
//                            tags = 0,
//                            tickTime = 1f,
//                            timeElapsed = 0,
//                            ticked = 0,
//                            duration = duration,
//                            permanent = false,
//                            caster = default,
//                            buffArgs = new BuffArgs
//                            {
//                                args0 = id
//                            },
//                            carrier = inData.otherEntities[i],
//                            canBeStacked = false,
//                            buffStack = default,
//                        }.ToBuffOld());
//                }
//            }
//        }

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }
//    }
//}

