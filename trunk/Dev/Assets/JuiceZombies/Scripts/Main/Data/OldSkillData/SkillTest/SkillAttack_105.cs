// using Cysharp.Threading.Tasks;
// using Unity.Entities;
// using Unity.Entities.UniversalDelegates;
// using Unity.Mathematics;
// using Unity.Physics;
// using Unity.Transforms;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace Main
// {
//     //没有位置大小方向变动的技能实体
//     public partial struct SkillAttack_105 : ISkillAttack
//     {
//         /// <summary>
//         /// 0
//         /// </summary>
//         public int id;
//
//         /// <summary>
//         /// 1 一次性释放的技能实体 值=0
//         /// </summary>
//         public float duration;
//
//         /// <summary>
//         /// 2
//         /// </summary>
//         public int tick;
//
//         /// <summary>
//         /// 3 技能实体释放者
//         /// </summary>
//         public Entity caster;
//
//         /// <summary>
//         /// 4 是否是弹幕
//         /// </summary>
//         public bool isBullet;
//
//         /// <summary>
//         /// 5 弹幕hp
//         /// </summary>
//         public int hp;
//
//         /// <summary>
//         /// 实体的击中目标 6
//         /// </summary>
//         public Entity target;
//
//         /// <summary>
//         /// 7
//         /// </summary>
//         public int4 args;
//
//         /// <summary>
//         /// 每帧做位置变动
//         /// </summary>
//         /// <param name="refData"></param>
//         /// <param name="inData"></param>
//         /// <returns>变动后的LT</returns>
//         public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//             var c = refData.cdfePhysicsCollider[inData.entity];
//
//             //BuffHelper.SetSkillAttackTarget(refData.ecb, inData.sortkey, inData.entity, inData.config, id, c);
//
//
//             return refData.cdfeLocalTransform[inData.entity];
//         }
//
//         public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//         }
//
//         public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//             //TODO:技能释放次数
//             for (int i = 0; i < refData.bfeSkill[caster].Length; i++)
//             {
//                 var temp0 = refData.bfeSkill[caster];
//                 var temp = temp0[i];
//                     //TODO:读表
//                 if (temp.Int32_0 == 1)
//                 {
//                     Debug.Log($"OnStart");
//
//                     if (temp.int2x4_14.c0.x == id)
//                     {
//                         temp.int2x4_14.c0.y++;
//                     }
//                     else if (temp.int2x4_14.c1.x == id)
//                     {
//                         temp.int2x4_14.c1.y++;
//                     }
//                     else if (temp.int2x4_14.c2.x == id)
//                     {
//                         temp.int2x4_14.c2.y++;
//                     }
//                     else if (temp.int2x4_14.c3.x == id)
//                     {
//                         temp.int2x4_14.c3.y++;
//                     }
//                     else
//                     {
//                         if (temp.int2x4_14.c0.x == 0)
//                         {
//                             temp.int2x4_14.c0.x = id;
//                             temp.int2x4_14.c0.y = 1;
//                         }
//                         else if (temp.int2x4_14.c1.x == 0)
//                         {
//                             temp.int2x4_14.c1.x = id;
//                             temp.int2x4_14.c1.y = 1;
//                         }
//                         else if (temp.int2x4_14.c2.x == 0)
//                         {
//                             temp.int2x4_14.c2.x = id;
//                             temp.int2x4_14.c2.y = 1;
//                         }
//                         else if (temp.int2x4_14.c3.x == 0)
//                         {
//                             temp.int2x4_14.c3.x = id;
//                             temp.int2x4_14.c3.y = 1;
//                         }
//                     }
//
//                     temp0[i] = temp;
//                     break;
//                 }
//             }
//         }
//
//         public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//             BlobArray<int> temp = new BlobArray<int>();
//             ref var elements = ref temp;
//             ref var triggers=ref temp;
//             ref var skillEffect = ref inData.config.value.Value.configTbskill_effectNews.configTbskill_effectNews;
//             ref var elementsTable=ref inData.config.value.Value.configTbskill_effectElements.configTbskill_effectElements;
//             for (int i = 0; i < skillEffect.Length; i++)
//             {
//                 if (skillEffect[i].id == id)
//                 {
//                     elements = ref skillEffect[i].elementList;
//                     triggers = ref skillEffect[i].elementTrigger;
//                     break;
//                 }
//             }
//             for (int i = 0; i < elements.Length; i++)
//             {
//                 //元素表！！！！
//                 for(int j = 0; j < elementsTable.Length; j++)
//                 {
//                     if (elementsTable[j].id == elements[i])
//                     {
//                         switch (elementsTable[j].elementType)
//                         {
//                             //输出元素
//                             case 1:
//                                 refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target, new Buff
//                                 {
//                                     CurrentTypeId = (Buff.TypeId)elementsTable[j].id,
//                                     Int32_0 = elementsTable[j].id,
//                                     Single_1 = 0,
//                                     Int32_2 = 0,
//                                     Single_3 = 0.02f,
//                                     Boolean_4 = false,
//                                     Entity_5 = caster,
//                                     Entity_6 = target,
//                                     Int32_7 = 0,
//                                     Int32_8 = 0,
//                                     Int32_9 = 0,
//                                     float3_10 = math.normalizesafe(refData.cdfeLocalTransform[target].Position-refData.cdfeLocalTransform[caster].Position) ,
//                                     Int32_11 = elementsTable[j].stateType,
//                                     BuffArgsNew_12 = new BuffArgsNew
//                                     {
//                                         args1 = new float4(elementsTable[j].outputType,0,0,0),
//                                         args2 = new float4(elementsTable[j].bonusType,0,0,0),
//                                         args3 = new float4(elementsTable[j].clearType,elementsTable[j].calcTypePara[0],0,0),
//                                         args4 = new float4(0,0,0,0)
//                                     }
//                                 });
//                                 break;
//                                 //更改属性
//                             case 2:
//                                 refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target, new Buff
//                                 {
//                                     CurrentTypeId = (Buff.TypeId)elementsTable[j].id,
//                                     Int32_0 = elementsTable[j].id,
//                                     Single_1 = 0,
//                                     Int32_2 = 0,
//                                     Single_3 = elementsTable[j].clearTypePara[0],
//                                     Boolean_4 = elementsTable[j].calcType==0?true:false,
//                                     Entity_5 = caster,
//                                     Entity_6 = target,
//                                     Int32_7 = 0,
//                                     Int32_8 = 0,
//                                     Int32_9 = 0,
//                                     float3_10 = math.normalizesafe(refData.cdfeLocalTransform[target].Position - refData.cdfeLocalTransform[caster].Position),
//                                     Int32_11 = elementsTable[j].stateType,
//                                     BuffArgsNew_12 = new BuffArgsNew
//                                     {
//                                         args1 = new float4(elementsTable[j].attrId, elementsTable[j].attrIdPara[0], 0, 0),
//                                         args2 = new float4(0, 0, 0, 0),
//                                         args3 = new float4(0, 0, 0, 0),
//                                         args4 = new float4(0, 0, 0, 0)
//                                     }
//                                 });
//                                 break;
//                             case 3:
//                                 break;
//                             case 4:
//                                 refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target, new Buff
//                                 {
//                                     CurrentTypeId = (Buff.TypeId)elementsTable[j].id,
//                                     Int32_0 = elementsTable[j].id,
//                                     Single_1 = 0,
//                                     Int32_2 = 0,
//                                     Single_3 = elementsTable[j].controlTypePara[0],
//                                     Boolean_4 = false,
//                                     Entity_5 = caster,
//                                     Entity_6 = target,
//                                     Int32_7 = elementsTable[j].changeType,
//                                     Int32_8 = elementsTable[j].clearType,
//                                     Int32_9 = 0,
//                                     float3_10 = math.normalizesafe(refData.cdfeLocalTransform[target].Position - refData.cdfeLocalTransform[caster].Position),
//                                     Int32_11 = elementsTable[j].stateType,
//                                     BuffArgsNew_12 = new BuffArgsNew
//                                     {
//                                         args1 = new int4(elementsTable[j].attrId, elementsTable[j].attrIdPara[0], 0, 0),
//                                         args2 = new int4(0, 0, 0, 0),
//                                         args3 = new int4(0, 0, 0, 0),
//                                         args4 = new int4(0, 0, 0, 0)
//                                     }
//                                 });
//                                 break;
//                             case 5:
//                                 //TODO:免疫 需要确认免疫能不能写到一起 写到一起需要判断字段的值
//                                 refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target, new Buff
//                                 {
//                                     CurrentTypeId = (Buff.TypeId)elementsTable[j].id,
//                                     Int32_0 = elementsTable[j].id,
//                                     Single_1 = 0,
//                                     Int32_2 = 0,
//                                     Single_3 = elementsTable[j].controlTypePara[0],
//                                     Boolean_4 = false,
//                                     Entity_5 = caster,
//                                     Entity_6 = target,
//                                     Int32_7 = elementsTable[j].changeType,
//                                     Int32_8 = elementsTable[j].clearType,
//                                     Int32_9 = 0,
//                                     float3_10 = math.normalizesafe(refData.cdfeLocalTransform[target].Position - refData.cdfeLocalTransform[caster].Position),
//                                     Int32_11 = elementsTable[j].stateType,
//                                     BuffArgsNew_12 = new BuffArgsNew
//                                     {
//                                         args1 = new float4(elementsTable[j].attrId, elementsTable[j].attrIdPara[0], 0, 0),
//                                         args2 = new float4(0, 0, 0, 0),
//                                         args3 = new float4(0, 0, 0, 0),
//                                         args4 = new float4(0, 0, 0, 0)
//                                     }
//                                 });
//                                 break;
//                             case 6:
//                                 refData.ecb.AppendToBuffer<Buff>(inData.sortkey, target, new Buff
//                                 {
//                                     CurrentTypeId = (Buff.TypeId)elementsTable[j].id,
//                                     Int32_0 = elementsTable[j].id,
//                                     Single_1 = 0,
//                                     Int32_2 = 0,
//                                     Single_3 = 0.02f,
//                                     Boolean_4 = false,
//                                     Entity_5 = caster,
//                                     Entity_6 = target,
//                                     Int32_7 = elementsTable[j].changeType,
//                                     Int32_8 = elementsTable[j].clearType,
//                                     Int32_9 = 0,
//                                     float3_10 = math.normalizesafe(refData.cdfeLocalTransform[target].Position - refData.cdfeLocalTransform[caster].Position),
//                                     Int32_11 = elementsTable[j].stateType,
//                                     BuffArgsNew_12 = new BuffArgsNew
//                                     {
//                                         args1 = new float4(elementsTable[j].displaceFrom, 0, 0, 0),
//                                         args2 = new float4(elementsTable[j].pointType, 0, 0, 0),
//                                         args3 = new float4(elementsTable[j].pointTypePara[0], elementsTable[j].pointTypePara[1], elementsTable[j].pointTypePara[2], 0),
//                                         args4 = new float4(0, 0, 0, 0)
//                                     }
//                                 });
//                                 break;
//                         }
//                     }
//                 }
//             }
//          
//             ///效果id是目标的情况 不在系统处理 统一通过buff处理
//             if (triggers.Length > 0)
//             {
//                 for (int i = 0; i < triggers.Length; i++)
//                 {
//                     for (int j = 0; j < skillEffect.Length; ++j)
//                     {
//                         if (triggers[i] == skillEffect[i].id)
//                         {
//                             var delay = skillEffect[i].delayTypePara[0] / 1000f;
//                             var targetEffect = skillEffect[i].calcTypePara[0];
//                             refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff
//                             {
//                                 CurrentTypeId = (Buff.TypeId)targetEffect,
//                                 Int32_0 = targetEffect,
//                                 Single_3=delay,
//                                 Boolean_4=false,
//                                 Entity_5=caster,
//                                 Entity_6=target,
//                             });
//                             //refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff_DelayBoom
//                             //{
//                             //    id = 0,
//                             //    priority = 0,
//                             //    maxStack = 0,
//                             //    tags = 0,
//                             //    tickTime = 0,
//                             //    timeElapsed = 0,
//                             //    ticked = 0,
//                             //    duration = delay,
//                             //    permanent = false,
//                             //    caster = default,
//                             //    carrier = target,
//                             //    canBeStacked = false,
//                             //    buffStack = default,
//                             //    buffArgs = default,
//                             //    totalMoveDistance = 0,
//                             //    xMetresPerInvoke = 0,
//                             //    lastPosition = default
//                             //}.ToBuffOld());
//                         }
//                     }
//                 }
//             }
//         }
//
//         public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//             if (id == 1003063)
//             {
//                 var buff1 = BuffHelper.UpdateParmeter(inData.config, id, 20003001);
//                 var rate = buff1[1].x;
//                 var duration = buff1[0].x / 1000f;
//                 var probability = buff1[2].x;
//                 for (int i = 0; i < inData.otherEntities.Length; i++)
//                 {
//                     var rand = inData.rand.NextInt(probability, 10000);
//                     if (rand <= probability)
//                     {
//                         refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
//                             new Buff_20003001
//                             {
//                                 id = 20003001,
//                                 priority = 0,
//                                 maxStack = 0,
//                                 tags = 0,
//                                 tickTime = 0,
//                                 timeElapsed = 0,
//                                 ticked = 0,
//                                 duration = duration,
//                                 permanent = false,
//                                 caster = default,
//                                 buffArgs = new BuffArgs
//                                 {
//                                     args0 = rate
//                                 },
//                                 carrier = inData.otherEntities[i],
//                                 canBeStacked = false,
//                                 buffStack = default,
//                             }.ToBuffOld());
//                     }
//
//                     var buff2 = BuffHelper.UpdateParmeterNew(inData.config, id, 20003009);
//                     rate = buff2[0].x;
//                     duration = buff2[1].x / 1000f;
//                     refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
//                         new Buff_20003009
//                         {
//                             id = 20003009,
//                             priority = 0,
//                             maxStack = 0,
//                             tags = 0,
//                             tickTime = 1f,
//                             timeElapsed = 0,
//                             ticked = 0,
//                             duration = duration,
//                             permanent = false,
//                             caster = default,
//                             buffArgs = new BuffArgs
//                             {
//                                 args0 = rate
//                             },
//                             carrier = inData.otherEntities[i],
//                             canBeStacked = false,
//                             buffStack = default,
//                         }.ToBuffOld());
//                 }
//             }
//             else if (id == 1003073)
//             {
//                 for (int i = 0; i < inData.otherEntities.Length; i++)
//                 {
//                     refData.ecb.AppendToBuffer(inData.sortkey, inData.otherEntities[i],
//                         new Buff_20003008
//                         {
//                             id = 20003008,
//                             priority = 0,
//                             maxStack = 0,
//                             tags = 0,
//                             tickTime = 1f,
//                             timeElapsed = 0,
//                             ticked = 0,
//                             duration = duration,
//                             permanent = false,
//                             caster = default,
//                             buffArgs = new BuffArgs
//                             {
//                                 args0 = id
//                             },
//                             carrier = inData.otherEntities[i],
//                             canBeStacked = false,
//                             buffStack = default,
//                         }.ToBuffOld());
//                 }
//             }
//         }
//
//         public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//         }
//     }
// }

