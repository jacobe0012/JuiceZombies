// using cfg.blobstruct;
// using Flow;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
//
//
// namespace Main
// {
//
//     /// <summary>
//     /// 弹射陷阱的技能逻辑
//     /// </summary>
//     public partial struct Skill_220301 : ISkillOld
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
//         public int radius;
//         public float gap;
//
//         public int limitDistance;
//         public int trapEffectID;
//
//         public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//
//             ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;
//             //拿到当前技能效果id
//             int currentSkillEffectID = 0;
//             for (int i = 0; i < skillArray.Length; ++i)
//             {
//                 if (skillArray[i].id == id)
//                     currentSkillEffectID = skillArray[i].skillEffectArray[0];
//             }
//             ref var skillEffectsArray = ref inData.globalConfig.value.Value.configTbskill_effects.configTbskill_effects;
//
//             GetLastParmeter(inData, ref skillEffectsArray, currentSkillEffectID);
//             //实例化skill
//             var skillPrefab = inData.prefabMapData.prefabHashMap["SkillGenerateTrap"];
//             var generateTrap = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//             var newLoc = new LocalTransform
//             {
//                 Position = inData.cdfeLocalTransform[inData.entity].Position,
//                 Scale = inData.cdfeLocalTransform[skillPrefab].Scale * radius,
//                 Rotation = inData.cdfeLocalTransform[skillPrefab].Rotation
//             };
//             refData.ecb.SetComponent(inData.sortkey, generateTrap, newLoc);
//
//             var playerPos = inData.cdfeLocalTransform[inData.entity].Position;
//             //refData.ecb.AddComponent(inData.sortkey, generateTrap, new SkillAttackData
//             //{
//             //    data = new SkillPopTrapAttack
//             //    {
//             //        duration = cooldown,
//             //        tick = 0,
//             //        caster = inData.entity,
//             //        localTrans = newLoc,
//             //        trapID = trapEffectID,
//             //        judgeDis = radius,
//             //        isActive = false,
//             //        skillDelay=0,
//             //        playerPos=playerPos
//             //    }.ToSkillAttack()
//             //});
//
//
//         }
//
//         private void GetLastParmeter(TimeLineData_ReadOnly inData,
//    ref BlobArray<ConfigTbskill_effect> skillEffectsArray, int currentSkillEffectID)
//         {
//
//             BlobArray<int3x4> buffDefault = new BlobArray<int3x4>();
//             ref BlobArray<int3x4> buffArray = ref buffDefault;
//
//
//             for (int i = 0; i < skillEffectsArray.Length; ++i)
//             {
//                 if (skillEffectsArray[i].id == currentSkillEffectID)
//                 {
//                     limitDistance = skillEffectsArray[i].limit[0].y;
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
//
//                     //处理buff
//                     for (int j = 1; j <= 8; j++)
//
//                     {
//
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
//                         HandleBuff(currentBuffID, currentBuffCount, ref buffArray, in inData, startIndex, j);
//
//                     }
//                     break;
//                 }
//             }
//         }
//
//         private NativeList<int3> GetBuffParmeters(ref BlobArray<int3x4> buffArray, int buffIndex, int startIndex, int buffParameterCount)
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
//                             tempInt3.y = 0; tempInt3.z = 0;
//                         }
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
//                             tempInt3.y = 0; tempInt3.z = 0;
//                         }
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
//                             tempInt3.y = 0; tempInt3.z = 0;
//                         }
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
//                             tempInt3.y = 0; tempInt3.z = 0;
//                         }
//                         break;
//                 }
//
//                 temp.Add(tempInt3);
//             }
//
//             return temp;
//         }
//
//         private void HandleBuff(int currentBuffID, int buffCount, ref BlobArray<int3x4> buffArray, in TimeLineData_ReadOnly inData, int startIndex, int buffIndex)
//         {
//             var buffPar = GetBuffParmeters(ref buffArray, buffIndex, startIndex, buffCount);
//             if (currentBuffID == 30000022)
//             {
//                 radius = buffPar[1].x;
//                 //cooldown = buffPar[0].x / 1000f;
//                 gap = buffPar[2].x / 1000f;
//                 trapEffectID = buffPar[3].x;
//
//             }
//
//
//         }
//
//
//         public bool IsAttackToEnemy(int limitDistance, TimeLineData_ReadOnly inData)
//         {
//             //判断是否对怪
//             float distance = 999f;
//             target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//                 inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                 inData.cdfeObstacleTag,
//                 inData.entity, TargetAttackType.Enemy, ref distance);
//             if (distance > limitDistance)
//             {
//                 target = default;
//             }
//
//             return target != default ? true : false;
//         }
//
//         public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//
//             //throw new NotImplementedException();
//         }
//
//         public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//
//
//
//
//         }
//
//         public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//         }
//
//         public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//
//         }
//     }
// }
//
//
//
//

