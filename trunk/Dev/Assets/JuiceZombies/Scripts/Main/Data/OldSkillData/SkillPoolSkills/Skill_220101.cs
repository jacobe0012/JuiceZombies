// using cfg.blobstruct;
// //using NSprites;
// using System.Collections.Generic;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
//
//
// namespace Main
// {
//     /// <summary>
//     /// 光环的技能逻辑
//     /// </summary>
//     public partial struct Skill_220101 : ISkillOld
//     {
//         ///<summary>0
//         ///技能的id
//         ///</summary>
//         public int id;
//
//         ///<summary>1
//         ///技能等级
//         ///</summary>
//         public int level;
//
//         ///<summary>2
//         ///冷却时间，单位秒。尽管游戏设计里面是没有冷却时间的，但是我们依然需要这个数据
//         ///因为作为一个ARPG子分类，和ARPG游戏有一样的问题：一次按键（时间够久）会发生连续多次使用技能，所以得有一个GCD来避免问题
//         ///当然和wow的gcd不同，这个“GCD”就只会让当前使用的技能进入0.1秒的冷却
//         ///</summary>
//         public float cooldown;
//
//
//         ///<summary>3
//         ///持续时间，单位：秒
//         ///</summary>
//         public float duration;
//
//
//         ///<summary>4
//         ///倍速，1=100%，0.1=10%是最小值
//         ///</summary>
//         public float timeScale;
//
//         ///<summary>5
//         ///Timeline的焦点对象也就是创建timeline的负责人，比如技能产生的timeline，就是技能的施法者
//         ///</summary>
//         public Entity caster;
//
//         ///<summary>6
//         ///技能已经运行了多少帧了 无需赋值
//         ///</summary>
//         public int tick;
//
//         /// <summary>7
//         /// 技能当前冷却时间 无需赋值
//         /// </summary>
//         public float curCooldown;
//
//         ///<summary>8
//         ///剩余时间，单位：秒
//         ///</summary>
//         public float curDuration;
//
//         ///<summary>9
//         ///距离这个技能最近的目标
//         ///</summary>
//         public Entity target;
//
//         ///<summary>10
//         ///技能从创建到现在总的tick
//         ///</summary>
//         public int totalTick;
//
//         ///<summary>11
//         ///是否时一次性释放技能
//         ///</summary>
//         public bool isOneShotSkill;
//
//         ///<summary>12
//         ///是否是确定位置坐标
//         ///</summary>
//         public bool isUseCertainPos;
//
//         ///<summary>13
//         ///一次性释放技能的坐标
//         ///</summary>
//         public float3 pos;
//
//         /// <summary>
//         /// ...
//         /// </summary>
//         public int4x4 args;
//         public int scale;
//         public int skillEffectID;
//         public Entity skillEntity;
//         public int limitDis;
//
//         public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//             NativeList<bool> isActive = new NativeList<bool>(Allocator.Temp);
//             NativeList<int> buffRate = new NativeList<int>(Allocator.Temp);
//             ref var skillEffectsArray = ref inData.globalConfig.value.Value.configTbskill_effects.configTbskill_effects;
//             ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;
//
//             int skillEffectID = 0;
//
//
//             for (int i = 0; i < skillArray.Length; i++)
//             {
//                 if (skillArray[i].id == id)
//                 {
//                     cooldown = skillArray[i].cd / 1000f;
//                     //duration= skillArray[i].cd / 1000f;
//                     skillEffectID = skillArray[i].skillEffectArray[0];
//
//                     break;
//                 }
//             }
//
//             for (int i = 0; i < skillEffectsArray.Length; i++)
//             {
//                 if (skillEffectsArray[i].id == skillEffectID)
//
//                 {
//                     limitDis = skillEffectsArray[i].limit[0].y;
//                     break;
//                 }
//             }
//
//             GetLastParmeter(inData, ref skillEffectsArray, skillEffectID, buffRate);
//             if (id == 220101 || id == 220102 || id == 220103 || id == 220104 || id == 220105 || id == 220106)
//             {
//                 //buff1
//                 isActive.Add(true);
//
//                 GenerateLightRing(ref refData, inData);
//             }
//
//             //buff2
//             if (id == 220102 || id == 220103 || id == 220104 || id == 220105 || id == 220106)
//             {
//                 int num = refData.rand.rand.NextInt(1, 11);
//                 if (num <= 3)
//                 {
//                     isActive.Add(true);
//                 }
//                 else
//                     isActive.Add(false);
//             }
//
//             //buff3
//             if (id == 220103)
//             {
//                 int num = refData.rand.rand.NextInt(1, 11);
//                 if (num <= 1)
//                 {
//                     isActive.Add(true);
//                 }
//                 else
//                     isActive.Add(false);
//             }
//
//             //buff3
//             if (id == 220104 || id == 220105 || id == 220106)
//             {
//                 int num = refData.rand.rand.NextInt(1, 21);
//                 if (num <= 1)
//                 {
//                     isActive.Add(true);
//                 }
//                 else
//                     isActive.Add(false);
//             }
//
//             //buff4
//             if (id == 220105 || id == 220106)
//             {
//                 int num = refData.rand.rand.NextInt(1, 21);
//                 if (num <= 1)
//                 {
//                     isActive.Add(true);
//                 }
//                 else
//                     isActive.Add(false);
//             }
//
//             #region 添加buff
//
//             for (int i = 0; i < buffRate.Length; i++)
//             {
//                 //Debug.Log(buffRate[i]);
//             }
//
//             for (int i = 0; i < inData.allEntities.Length; i++)
//             {
//                 if (inData.allEntities[i] != null)
//                 {
//                     var dis = math.distance(inData.cdfeLocalTransform[inData.allEntities[i]].Position,
//                         inData.cdfeLocalTransform[inData.entity].Position);
//                     if (dis <= limitDis && inData.allEntities[i].CompareTo(inData.entity) != 0)
//                     {
//                         for (int j = 0; j < isActive.Length; j++)
//                         {
//                             if (isActive[j] && j == 0)
//                             {
//                                 refData.ecb.AppendToBuffer(inData.sortkey, inData.allEntities[i],
//                                     new Buff_20003001
//                                     {
//                                         id = 20003001,
//                                         priority = 0,
//                                         maxStack = 0,
//                                         tags = 0,
//                                         tickTime = 0,
//                                         timeElapsed = 0,
//                                         ticked = 0,
//                                         duration = cooldown / 1000f,
//                                         permanent = false,
//                                         caster = default,
//                                         buffArgs = new BuffArgs
//                                         {
//                                             args0 = buffRate[0]
//                                         },
//                                         carrier = inData.allEntities[i],
//                                         canBeStacked = false,
//                                         buffStack = default,
//                                     }.ToBuffOld());
//                             }
//
//                             if (isActive[j] && j == 1)
//                             {
//                                 if (id == 220106)
//                                 {
//                                     refData.ecb.AppendToBuffer(inData.sortkey, inData.allEntities[i],
//                                         new Buff_20003006
//                                         {
//                                             buffArgs = new BuffArgs { args0 = buffRate[1] },
//                                             carrier = inData.allEntities[i],
//                                         }.ToBuffOld());
//                                 }
//                                 else
//                                 {
//                                     refData.ecb.AppendToBuffer(inData.sortkey, inData.allEntities[i],
//                                         new Buff_20003001
//                                         {
//                                             buffArgs = new BuffArgs { args0 = buffRate[1] },
//                                             carrier = inData.allEntities[i],
//                                         }.ToBuffOld());
//                                 }
//                             }
//
//                             if (isActive[j] && j == 2)
//                             {
//                                 if (id == 220103)
//                                 {
//                                     refData.ecb.AppendToBuffer(inData.sortkey, inData.allEntities[i],
//                                         new Buff_20003001
//                                         {
//                                             buffArgs = new BuffArgs { args0 = buffRate[1] },
//                                             carrier = inData.allEntities[i],
//                                         }.ToBuffOld());
//                                 }
//
//                                 if (id == 220104)
//                                 {
//                                     // refData.ecb.AppendToBuffer(inData.sortkey, inData.allEntities[i],
//                                     //     new Buff_20003004
//                                     //     {
//                                     //         carrier = inData.allEntities[i],
//                                     //     }.ToBuffOld());
//                                 }
//
//                                 if (id == 220105 || id == 220106)
//                                 {
//                                     // refData.ecb.AppendToBuffer(inData.sortkey, inData.allEntities[i],
//                                     //     new Buff_20003005
//                                     //     {
//                                     //         carrier = inData.allEntities[i],
//                                     //     }.ToBuffOld());
//                                 }
//                             }
//
//                             if (isActive[j] && j == 3)
//                             {
//                                 // refData.ecb.AppendToBuffer(inData.sortkey, inData.allEntities[i],
//                                 //     new Buff_20003004
//                                 //     {
//                                 //         carrier = inData.allEntities[i],
//                                 //     }.ToBuffOld());
//                             }
//                         }
//                     }
//                 }
//             }
//
//             #endregion
//         }
//
//         private void GetLastParmeter(TimeLineData_ReadOnly inData,
//             ref BlobArray<ConfigTbskill_effect> skillEffectsArray, int currentSkillEffectID, NativeList<int> buffRate)
//         {
//             BlobArray<int3x4> buffDefault = new BlobArray<int3x4>();
//             ref BlobArray<int3x4> buffArray = ref buffDefault;
//
//
//             for (int i = 0; i < skillEffectsArray.Length; ++i)
//             {
//                 if (skillEffectsArray[i].id == currentSkillEffectID)
//                 {
//                     scale = skillEffectsArray[i].limit[0].y;
//                     //拿到buff组
//
//                     buffArray = ref skillEffectsArray[i].skillEffectBuffNew;
//                     if (buffArray.Length <= 0)
//                     {
//                         Debug.Log("没拿到数据");
//                         return;
//                     }
//
//
//                     //处理buff
//                     for (int j = 1; j <= 8; j++)
//
//                     {
//                         int currentBuffID = 0;
//                         int currentBuffCount = 0;
//                         int startIndex = 0;
//                         switch (j)
//                         {
//                             case 1:
//                                 currentBuffID = buffArray[0].c0.x;
//                                 currentBuffCount = buffArray[0].c0.y;
//                                 break;
//
//                             case 2:
//                                 currentBuffID = buffArray[0].c1.x;
//                                 currentBuffCount = buffArray[0].c1.y;
//                                 break;
//
//                             case 3:
//                                 currentBuffID = buffArray[0].c2.x;
//                                 currentBuffCount = buffArray[0].c2.y;
//                                 break;
//
//                             case 4:
//                                 currentBuffID = buffArray[0].c3.x;
//                                 currentBuffCount = buffArray[0].c3.y;
//                                 break;
//                             case 5:
//                                 currentBuffID = buffArray[1].c0.x;
//                                 currentBuffCount = buffArray[1].c0.y;
//                                 startIndex = 1;
//                                 break;
//
//                             case 6:
//                                 currentBuffID = buffArray[1].c1.x;
//                                 currentBuffCount = buffArray[1].c1.y;
//                                 startIndex = 1;
//                                 break;
//                             case 7:
//                                 currentBuffID = buffArray[1].c2.x;
//                                 currentBuffCount = buffArray[1].c2.y;
//                                 startIndex = 1;
//                                 break;
//                             case 8:
//                                 currentBuffID = buffArray[1].c3.x;
//                                 currentBuffCount = buffArray[1].c3.y;
//                                 startIndex = 1;
//                                 break;
//                         }
//
//                         HandleBuff(currentBuffID, currentBuffCount, ref buffArray, in inData, startIndex, j, buffRate);
//                     }
//
//                     break;
//                 }
//             }
//         }
//
//         private void HandleBuff(int currentBuffID, int buffCount, ref BlobArray<int3x4> buffArray,
//             in TimeLineData_ReadOnly inData, int startIndex, int buffIndex, NativeList<int> buffRate)
//         {
//             var buffPar = GetBuffParmeters(ref buffArray, buffIndex, startIndex, buffCount);
//             if (currentBuffID == 20003001 || currentBuffID == 20003004 || currentBuffID == 20003006)
//             {
//                 buffRate.Add(buffPar[1].x);
//             }
//         }
//
//         private NativeList<int3> GetBuffParmeters(ref BlobArray<int3x4> buffArray, int buffIndex, int startIndex,
//             int buffParameterCount)
//         {
//             NativeList<int3> temp = new NativeList<int3>(buffParameterCount, Allocator.Temp);
//             for (int j = 1; j <= buffParameterCount; ++j)
//             {
//                 int3 tempInt3 = new int3(0, 0, 0);
//
//                 switch (buffIndex)
//                 {
//                     case 1:
//                     case 5:
//                         int currentParameterRate = buffArray[2 * j + startIndex].c0.x;
//
//                         tempInt3.x = currentParameterRate;
//                         int currentParameterInfectID = buffArray[2 * j + startIndex].c0.y;
//                         if (currentParameterInfectID != 0)
//                         {
//                             tempInt3.y = currentParameterInfectID;
//                             int currentParameterInfectRate = buffArray[2 * j + startIndex].c0.z;
//                             tempInt3.z = currentParameterInfectRate;
//                         }
//                         else
//                         {
//                             tempInt3.y = 0;
//                             tempInt3.z = 0;
//                         }
//
//                         break;
//                     case 2:
//                     case 6:
//                         currentParameterRate = buffArray[2 * j + startIndex].c1.x;
//
//                         tempInt3.x = currentParameterRate;
//                         currentParameterInfectID = buffArray[2 * j + startIndex].c1.y;
//                         if (currentParameterInfectID != 0)
//                         {
//                             tempInt3.y = currentParameterInfectID;
//                             int currentParameterInfectRate = buffArray[2 * j + startIndex].c1.z;
//                             tempInt3.z = currentParameterInfectRate;
//                         }
//                         else
//                         {
//                             tempInt3.y = 0;
//                             tempInt3.z = 0;
//                         }
//
//                         break;
//
//                     case 3:
//                     case 7:
//                         currentParameterRate = buffArray[2 * j].c2.x;
//
//                         tempInt3.x = currentParameterRate;
//                         currentParameterInfectID = buffArray[2 * j].c2.y;
//                         if (currentParameterInfectID != 0)
//                         {
//                             tempInt3.y = currentParameterInfectID;
//                             int currentParameterInfectRate = buffArray[2 * j].c2.z;
//                             tempInt3.z = currentParameterInfectRate;
//                         }
//                         else
//                         {
//                             tempInt3.y = 0;
//                             tempInt3.z = 0;
//                         }
//
//                         break;
//                     case 4:
//                     case 8:
//                         currentParameterRate = buffArray[2 * j].c3.x;
//
//                         tempInt3.x = currentParameterRate;
//                         currentParameterInfectID = buffArray[2 * j].c3.y;
//                         if (currentParameterInfectID != 0)
//                         {
//                             tempInt3.y = currentParameterInfectID;
//                             int currentParameterInfectRate = buffArray[2 * j].c3.z;
//                             tempInt3.z = currentParameterInfectRate;
//                         }
//                         else
//                         {
//                             tempInt3.y = 0;
//                             tempInt3.z = 0;
//                         }
//
//                         break;
//                 }
//
//                 temp.Add(tempInt3);
//             }
//
//             return temp;
//         }
//
//         private void GenerateLightRing(ref TimeLineData_ReadWrite refData, TimeLineData_ReadOnly inData)
//         {
//             var skillLightRing = inData.prefabMapData.prefabHashMap["HaloPrefab"];
//             skillEntity = refData.ecb.Instantiate(inData.sortkey, skillLightRing);
//             refData.ecb.SetComponent<LocalTransform>(inData.sortkey, skillEntity, new LocalTransform
//             {
//                 Position = inData.cdfeLocalTransform[inData.entity].Position,
//                 Scale = scale * inData.cdfeLocalTransform[skillLightRing].Scale,
//                 Rotation = inData.cdfeLocalTransform[skillLightRing].Rotation
//             });
//             //TODO:技能实体
//             //refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//             //{
//             //    data = new SkillLifhtRingAttack
//             //    {
//             //        id = 2201011,
//             //        duration = cooldown,
//             //        tick = 0,
//             //        caster = inData.entity,
//             //        isBullet = false,
//             //        hp = 0,
//             //        stayAttack = false,
//             //        stayAttackInterval = 0,
//             //        curStayAttackInterval = 0,
//             //        entity = default,
//             //        radius = 0,
//             //        scale = 0
//             //    }.ToSkillAttack()
//             //});
//         }
//
//         public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//             //throw new NotImplementedException();
//         }
//
//         public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//         }
//
//         public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//         }
//
//         public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//         }
//     }
// }

