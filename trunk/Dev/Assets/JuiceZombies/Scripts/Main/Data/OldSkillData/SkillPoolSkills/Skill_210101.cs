// using cfg.blobstruct;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Physics;
// using Unity.Transforms;
// using UnityEngine;
// using UnityEngine.UIElements;
//
//
// namespace Main
// {
//     /// <summary>
//     /// 摩托车冲撞的技能逻辑
//     /// </summary>
//     public partial struct Skill_210101 : ISkillOld
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
//
//         public int width;
//         public int height;
//         public int pushForce;
//
//         public float diffuseSpeed;
//         public int limitDistance;
//         public int xOffset;
//         public int yOffset;
//         public int propID;
//
//         public int attackRate;
//         public int currentSkillEffectID1;
//         public int currentSkillEffectID2;
//         public float gapTime; //
//         public bool isStart;
//
//         public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//             isStart = false;
//             ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;
//             //拿到当前技能效果id
//             for (int i = 0; i < skillArray.Length; ++i)
//             {
//                 if (skillArray[i].id == id)
//                 {
//                     currentSkillEffectID1 = skillArray[i].skillEffectArray[0];
//                     currentSkillEffectID2 = skillArray[i].skillEffectArray[1];
//                 }
//             }
//
//             ref var skillEffectsArray = ref inData.globalConfig.value.Value.configTbskill_effects.configTbskill_effects;
//             GetLastParmeter(inData, ref skillEffectsArray, currentSkillEffectID1);
//
//             //生成左右两个
//             GenerateMotorLAndR(ref refData, in inData, true);
//             GenerateMotorLAndR(ref refData, in inData, false);
//         }
//
//         private void GetLastParmeter(TimeLineData_ReadOnly inData,
//             ref BlobArray<ConfigTbskill_effect> skillEffectsArray, int currentSkillEffectID)
//         {
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
//                         // Debug.Log($"currentBuffID:{currentBuffID}");
//                         HandleBuff(currentBuffID, currentBuffCount, ref buffArray, in inData, startIndex, j);
//                     }
//
//                     break;
//                 }
//             }
//         }
//
//         private void HandleBuff(int currentBuffID, int buffCount, ref BlobArray<int3x4> buffArray,
//             in TimeLineData_ReadOnly inData, int startIndex, int buffIndex)
//         {
//             var buffPar = GetBuffParmeters(ref buffArray, buffIndex, startIndex, buffCount);
//             if (currentBuffID == 50000005)
//             {
//                 gapTime = buffPar[1].x / 1000f;
//             }
//
//             if (currentBuffID == 40000006)
//             {
//                 width = buffPar[1].x;
//                 height = buffPar[2].x;
//                 pushForce = buffPar[4].x;
//                 diffuseSpeed = buffPar[5].x / 100f;
//             }
//
//             if (currentBuffID == 50000001)
//             {
//                 xOffset = buffPar[1].x;
//                 yOffset = buffPar[2].x;
//             }
//
//             if (currentBuffID == 30000015)
//             {
//                 propID = buffPar[1].x;
//                 // var propID = 1001;
//                 //  Debug.Log($"propID:{propID}");
//             }
//
//             if (currentBuffID == 30000003)
//             {
//                 attackRate = buffPar[1].x;
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
//         private LocalTransform SetTrans(in TimeLineData_ReadOnly inData, Entity skillBaseBallBat, int limitDistance)
//         {
//             float distance = 9999;
//             target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//                 inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                 inData.cdfeObstacleTag,
//                 inData.entity, TargetAttackType.Enemy, ref distance);
//             if (distance > limitDistance)
//             {
//             }
//
//             //拿到对敌角度
//             if (IsAttackToEnemy(limitDistance, inData))
//             {
//                 //TODO:方向
//                 float3 dir = inData.cdfeLocalTransform[target].Position -
//                              inData.cdfeLocalTransform[inData.entity].Position;
//
//
//                 float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//                     new Vector3(0, 1, 0));
//
//                 var qua = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));
//
//                 var tran = new LocalTransform
//                 {
//                     Position = inData.cdfeLocalTransform[inData.entity].Position,
//                     Scale = inData.cdfeLocalTransform[inData.entity].Scale,
//                     Rotation = qua
//                 };
//
//
//                 return inData.cdfeLocalTransform[inData.entity].RotateZ(math.radians(needAngel));
//                 //return tran;
//
//                 //return inData.cdfeLocalTransform[inData.entity];
//             }
//
//             return inData.cdfeLocalTransform[inData.entity];
//         }
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
//             //throw new NotImplementedException();
//         }
//
//         public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//         {
//             if (math.abs(curDuration - gapTime) > 0.1f && !isStart)
//             {
//                 // Debug.Log("qqqqqqqqqqqqqqqqqqqqqqqqqqq");
//                 //实例化skill3 4
//                 GenerateMotorUpAndDown(ref refData, in inData, false);
//                 GenerateMotorUpAndDown(ref refData, in inData, true);
//                 isStart = true;
//             }
//         }
//
//         private readonly void GenerateMotorLAndR(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData,
//             bool isLeft)
//         {
//             int offset = xOffset;
//             //实例化skill1
//             var skillPrefab = inData.prefabMapData.prefabHashMap["SkillMotorCrash"];
//             var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//             // refData.ecb.SetComponent(inData.sortkey, skillEntity, SetTrans(in inData, skillPrefab, limitDistance));
//             if (isLeft)
//             {
//                 offset = -offset;
//             }
//
//             var dir = inData.cdfeChaStats[inData.entity].chaResource.direction;
//             float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//                 new Vector3(0, 1, 0));
//
//             var qua = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));
//             // 更新物体的位置
//             var newLoc = new LocalTransform
//             {
//                 Position = inData.cdfeLocalTransform[inData.entity].Position +
//                            inData.cdfeLocalTransform[inData.entity].Right() * offset,
//                 Scale = inData.cdfeLocalTransform[skillPrefab].Scale * width,
//                 Rotation = qua,
//             };
//             if (isUseCertainPos)
//             {
//                 newLoc = new LocalTransform
//                 {
//                     Position = pos,
//                     Scale = inData.cdfeLocalTransform[skillPrefab].Scale * width,
//                     Rotation = qua,
//                 };
//             }
//
//             // Debug.Log($"position{inData.cdfeLocalTransform[inData.entity].Position}");
//             refData.ecb.SetComponent(inData.sortkey, skillEntity, newLoc);
//
//             //refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//             //{
//             //    data = new SkillMotorAttack
//             //    {
//             //        currentLoc = newLoc,
//             //        id = currentSkillEffectID1,
//             //        duration = height / diffuseSpeed,
//             //        tick = 0,
//             //        caster = inData.entity,
//             //        diffuseSpeed = diffuseSpeed,
//             //        hp = 0,
//             //        isBullet = false
//             //    }.ToSkillAttack()
//             //});
//
//             refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, skillEntity,
//                 new SkillHitEffectBuffer
//                 {
//                     buffID = 40000008,
//                     buffArgs = new float3x4 { c0 = new float3(pushForce, 0, 0), c1 = dir, c2 = new float3(2, 0, 0) }
//                 });
//             refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, skillEntity,
//                 new SkillHitEffectBuffer
//                     { buffID = 30000003, buffArgs = new float3x4 { c0 = new float3(attackRate, 0, 0) } });
//             GenerateProp(ref refData, inData, newLoc);
//         }
//
//         private readonly void GenerateMotorUpAndDown(ref TimeLineData_ReadWrite refData,
//             in TimeLineData_ReadOnly inData, bool isUp)
//         {
//             int offset = xOffset;
//             //实例化skill1
//             var skillPrefab = inData.prefabMapData.prefabHashMap["SkillMotorCrash"];
//             var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//             refData.ecb.SetComponent(inData.sortkey, skillEntity, SetTrans(in inData, skillPrefab, limitDistance));
//             if (isUp)
//             {
//                 offset = -offset;
//             }
//
//             var dir = inData.cdfeChaStats[inData.entity].chaResource.direction;
//             float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//                 new Vector3(0, 1, 0));
//
//             var qua = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));
//
//             // 更新物体的位置
//             var newLoc = new LocalTransform
//             {
//                 Position = inData.cdfeLocalTransform[inData.entity].Position +
//                            inData.cdfeLocalTransform[inData.entity].Up() * offset,
//                 Scale = inData.cdfeLocalTransform[skillPrefab].Scale * width,
//                 Rotation = qua,
//             };
//             if (isUseCertainPos)
//             {
//                 newLoc = new LocalTransform
//                 {
//                     Position = pos,
//                     Scale = inData.cdfeLocalTransform[skillPrefab].Scale * width,
//                     Rotation = qua,
//                 };
//             }
//
//             refData.ecb.SetComponent(inData.sortkey, skillEntity, newLoc);
//
//             //refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//             ////{
//             ////    data = new SkillMotorAttack
//             ////    {
//             ////        currentLoc = newLoc,
//             ////        id = currentSkillEffectID2,
//             ////        duration = height / diffuseSpeed,
//             ////        tick = 0,
//             ////        caster = inData.entity,
//             ////        diffuseSpeed = diffuseSpeed,
//             ////        hp = 0,
//             ////        isBullet = false
//             ////    }.ToSkillAttack()
//             ////});
//             refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, skillEntity,
//                 new SkillHitEffectBuffer
//                 {
//                     buffID = 40000008,
//                     buffArgs = new float3x4 { c0 = new float3(pushForce, 0, 0), c1 = dir, c2 = new float3(2, 0, 0) }
//                 });
//             refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, skillEntity,
//                 new SkillHitEffectBuffer
//                     { buffID = 30000003, buffArgs = new float3x4 { c0 = new float3(attackRate, 0, 0) } });
//             GenerateProp(ref refData, inData, newLoc);
//         }
//
//         private readonly void GenerateProp(ref TimeLineData_ReadWrite refData, TimeLineData_ReadOnly inData,
//             LocalTransform loc)
//         {
//             FixedString128Bytes propName = "";
//             ref var propArray = ref inData.globalConfig.value.Value.configTbbattle_items.configTbbattle_items;
//             for (int k = 0; k < propArray.Length; k++)
//             {
//                 if (propArray[k].id == propID)
//                 {
//                     //    Debug.Log("121231212121");
//                     propName = propArray[k].model;
//                     break;
//                 }
//             }
//
//             //   Debug.Log($"propName{propName}");
//             var propPrefab = inData.prefabMapData.prefabHashMap[propName];
//             var propEntity = refData.ecb.Instantiate(inData.sortkey, propPrefab);
//             // 更新物体的位置
//             var newLoc = new LocalTransform
//             {
//                 Position = loc.Position,
//                 Scale = inData.cdfeLocalTransform[propPrefab].Scale,
//                 Rotation = loc.Rotation,
//             };
//             refData.ecb.SetComponent(inData.sortkey, propEntity, newLoc);
//             refData.ecb.SetComponent(inData.sortkey, propEntity, new DropsData
//             {
//                 point0 = newLoc.Position,
//                 point1 = default,
//                 point2 = new float3(newLoc.Position.x,
//                     newLoc.Position.y, 0),
//                 point3 = default,
//                 isLooting = false,
//                 duration = 1f,
//                 id = propID
//             });
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

