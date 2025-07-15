// //---------------------------------------------------------------------
// // JiYuStudio
// // Author: 如初
// // Time: 2023-07-17 12:15:10
// //---------------------------------------------------------------------
//
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
//
// namespace Main
// {
//     /// <summary>
//     /// 每移动x米生成诱饵
//     /// </summary>
//     public partial struct Buff_50000006_406013 : IBuffOld
//     {
//         ///<summary>0
//         ///buff的id
//         ///</summary>
//         public int id;
//
//         ///<summary>1
//         ///buff的优先级，优先级越低的buff越后面执行，这是一个非常重要的属性
//         ///比如经典的“吸收50点伤害”和“受到的伤害100%反弹给攻击者”应该反弹多少，取决于这两个buff的priority谁更高
//         ///</summary>
//         public int priority;
//
//         ///<summary>2
//         ///buff堆叠的规则中需要的层数，在这个游戏里只要id和caster相同的buffObj就可以堆叠
//         ///激战2里就不同，尽管图标显示堆叠，其实只是统计了有多少个相同id的buffObj作为层数显示了
//         ///</summary>
//         public int maxStack;
//
//         ///<summary>3
//         ///buff的tag
//         ///</summary>
//         public int tags;
//
//         ///<summary>4
//         ///buff的工作周期，单位：秒。
//         ///每多少秒执行工作一次，如果<=0则代表不会周期性工作，只要>0，则最小值为Time.FixedDeltaTime。
//         ///</summary>
//         public float tickTime;
//
//         ///<summary>5
//         ///buff已经存在了多少时间了，单位：帧
//         ///</summary>
//         public int timeElapsed;
//
//         ///<summary>6
//         ///buff执行了多少次onTick了，如果不会执行onTick，那将永远是0
//         ///</summary>
//         public int ticked;
//
//         ///<summary>7
//         ///剩余多久，单位：秒
//         ///</summary>
//         public float duration;
//
//         ///<summary>8
//         ///是否是一个永久的buff，永久的duration不会减少，但是timeElapsed还会增加
//         ///</summary>
//         public bool permanent;
//
//
//         ///<summary>9
//         ///buff的施法者是谁，当然可以是空的
//         ///</summary>
//         public Entity caster;
//
//         ///<summary>10
//         ///buff的携带者，实际上是作为参数传递给脚本用，具体是谁，可定是所在控件的this.gameObject了
//         ///</summary>
//         public Entity carrier;
//
//         ///<summary>11
//         ///合并时是否保持原有buff持续时间
//         ///</summary>
//         public bool canBeStacked;
//
//         ///<summary>12  
//         ///合并时是否保持原有buff持续时间
//         ///</summary>
//         public BuffStack buffStack;
//
//         ///<summary>13 
//         ///buff参数
//         ///</summary>
//         public BuffArgs buffArgs;
//
//         /// <summary>14
//         /// 当前移动总距离
//         /// </summary>
//         public float totalMoveDistance;
//
//         /// <summary>15
//         /// 每移动多少米调用一次
//         /// </summary>
//         public int xMetresPerInvoke;
//
//         /// <summary>16
//         /// 上一tick的位置
//         /// </summary>
//         public float3 lastPosition;
//
//
//         public void OnOccur(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//         {
//         }
//
//         public void OnRemoved(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//         {
//         }
//
//         public void OnTick(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//         {
//            
//         }
//
//         public void OnCast(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//         {
//         }
//
//         public void OnHit(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//         {
//         }
//
//         public void OnBeHurt(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//         {
//         }
//
//         public void OnKill(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//         {
//         }
//
//         public void OnBeforeBeKilled(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//         {
//         }
//
//         public void OnBeKilled(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//         {
//         }
//
//         public void OnPerUnitMove(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//         {
//
//
//             Debug.Log($"OnPerUnitMove:{totalMoveDistance}");
//             var prefab = inData.prefabMapData.prefabHashMap["skill_bait"];
//             var ins = refData.ecb.Instantiate(inData.sortkey, prefab);
//             var newpos = new LocalTransform
//             {
//                 Position = inData.cdfeLocalTransform[inData.player].Position,
//                 //TODO:大小*10
//                 Scale = 10 * inData.cdfeLocalTransform[prefab].Scale,
//                 Rotation = inData.cdfeLocalTransform[prefab].Rotation,
//             };
//
//             refData.ecb.SetComponent(inData.sortkey, ins, newpos);
//             refData.ecb.AddComponent(inData.sortkey,ins, new SpiritData { });
//             refData.ecb.AddComponent(inData.sortkey,ins, new ChaStats
//             {
//                 chaProperty = new ChaProperty
//                 {
//                     maxHp = 999,
//                     hpRatios = 0,
//                     hpAdd = 0,
//                     hpRecovery = 0,
//                     hpRecoveryRatios = 0,
//                     hpRecoveryAdd = 0,
//                     atk = 0,
//                     atkRatios = 0,
//                     atkAdd = 0,
//                     rebirthCount = 0,
//                     critical = 0,
//                     criticalDamageRatios = 0,
//                     damageRatios = 0,
//                     damageAdd = 0,
//                     reduceHurtRatios = 0,
//                     reduceHurtAdd = 0,
//                     maxMoveSpeed = 0,
//                     maxMoveSpeedRatios = 0,
//                     speedRecoveryTime = 0,
//                     mass = 0,
//                     massRatios = 0,
//                     pushForce = 0,
//                     pushForceRatios = 0,
//                     reduceHitBackRatios = 0,
//                     collDamagePlus = 0,
//                     continuousCollDamagePlus = 0,
//                     dodge = 0,
//                     shieldCount = 0,
//                     coolDown = 0,
//                     skillRefreshCount = 0,
//                     skillRangeRatios = 0,
//                 },
//                 enviromentProperty = default,
//                 chaResource = new ChaResource
//                 {
//                     curPushForce = 0,
//                     curMoveSpeed = 0,
//                     direction = default,
//                     continuousCollCount = 0,
//                     isInvincible = false,
//                     actionSpeed = 1,
//                     hp = 999
//                 },
//                 chaControlState = default
//             });
//             refData.ecb.AddBuffer<DamageInfo>(inData.sortkey,ins);
//
//             //refData.ecb.AddComponent(inData.sortkey, ins,
//             //    new SkillAttackData
//             //    {
//             //        data = new SkillAttack_406013()
//             //        {
//             //            id = 0,
//             //            duration = buffArgs.args0/1000f,
//             //            tick = 0,
//             //            caster = inData.player,
//             //            isBullet = false,
//             //            hp = 0,
//             //            stayAttack = false,
//             //            stayAttackInterval = 0,
//             //            curStayAttackInterval = 0,
//             //            skillDelay = 0,
//             //            target = default,
//             //            skillEntity = default
//             //        }.ToSkillAttack()
//             //    });
//             
//         }
//
//
//         public void OnLevelUp(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData)
//         {
//         }
//     }
// }

