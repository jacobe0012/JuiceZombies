// using cfg.blobstruct;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
//
// namespace Main
// {
//     //旋转的飞刀
//     public partial struct SkillBulletAttack_210701 : ISkillAttack
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
//         /// 弹幕速度 8
//         /// </summary>
//         public float speed;
//
//         /// <summary>
//         /// 弹幕用 触发器id 9
//         /// </summary>
//         public int triggerID;
//
//         public float width;
//         public bool isS;
//
//         public float3 startPoint;
//         public float maxDuration;
//         public bool isLeft;
//
//
//         public int deSpeedRatios;
//
//         public int damageRatios;
//         // public int skilleffectid;
//         // private int radius;
//         // private int pushRate;
//         // private int attackRate;
//         // private int attackAddRate;
//         // private int damageValue;
//
//
//         public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//         }
//
//         /// <summary>
//         /// 每帧做位置变动
//         /// </summary>
//         /// <param name="refData"></param>
//         /// <param name="inData"></param>
//         /// <returns>变动后的LT</returns>
//         public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//             //var dir = new float3(0, 1, 0);
//             var actionspeed = inData.cdfeChaStats[caster].chaResource.actionSpeed < math.EPSILON
//                 ? 1
//                 : inData.cdfeChaStats[caster].chaResource.actionSpeed;
//             var playerTra = refData.cdfeLocalTransform[inData.player];
//             var entityTra = refData.cdfeLocalTransform[inData.entity];
//
//             if (maxDuration - duration > 1.5f && math.length(entityTra.Position - playerTra.Position) < 8)
//             {
//                 //refData.ecb.DestroyEntity(inData.sortkey, inData.entity);
//                 duration = maxDuration;
//
//                 startPoint = refData.cdfeLocalTransform[inData.entity].Position;
//                 float distance = 9999;
//
//                 var target = BuffHelper.NerestTarget(inData.allEntities, refData.cdfeLocalTransform,
//                     inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                     inData.cdfeObstacleTag,
//                     inData.entity, TargetAttackType.Enemy, ref distance);
//
//                 // float3 dir = new float3(0, 1, 0);
//                 isLeft = inData.cdfeChaStats[inData.player].chaResource.direction.x > 0 ? false : true;
//                 if (refData.storageInfoFromEntity.Exists(target))
//                 {
//                     isLeft = refData.cdfeLocalTransform[target].Position.x -
//                         refData.cdfeLocalTransform[inData.player].Position.x > 0
//                             ? false
//                             : true;
//                 }
//
//                 //inData.cdfeSkillAttackData.
//
//
//                 //refData.ecb.Instantiate(inData.sortkey, inData.entity);
//             }
//
//             //math.clamp(inData.cdfeChaStats[caster].chaResource.actionSpeed,1,)
//             //var dir = math.mul(refData.cdfeLocalTransform[inData.entity].Rotation, new float3(0, 1, 0));
//             var temp = refData.cdfeLocalTransform[inData.entity];
//             //temp.
//
//             const float offsetx = 40f;
//             const float offsety = 90f;
//             float offsetX = isLeft ? -offsetx : offsetx;
//             float offsetY = offsety;
//             float3 finalPoint = new float3(startPoint.x + offsetX, startPoint.y - offsetY * 3, 0);
//
//             float3 point1 = new float3(startPoint.x + offsetX / 2, startPoint.y + offsetY, 0);
//             float3 point2 = new float3(startPoint.x + offsetX, startPoint.y, 0);
//             temp.Position = PhysicsHelper.CubicBezier(
//                 (maxDuration - duration) / maxDuration,
//                 startPoint, point1, point2, finalPoint);
//             //temp.Position += dir * (speed / 100f) * inData.fDT * actionspeed;
//             //math.radians(20);
//             var qua = temp.RotateZ(0.16f);
//
//
//             return qua;
//         }
//
//         public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//             // throw new System.NotImplementedException();
//         }
//
//         public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//             ref var bulletConfig = ref inData.config.value.Value.configTbbullets.configTbbullets;
//             ref var skillConfig = ref inData.config.value.Value.configTbskills.configTbskills;
//             ref var skilleffectConfig = ref inData.config.value.Value.configTbskill_effects.configTbskill_effects;
//         }
//
//         public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//         }
//
//         public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//         {
//             startPoint = refData.cdfeLocalTransform[inData.entity].Position;
//             maxDuration = duration;
//         }
//     }
// }

