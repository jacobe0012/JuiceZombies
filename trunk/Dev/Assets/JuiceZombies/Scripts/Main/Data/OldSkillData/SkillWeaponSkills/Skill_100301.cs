//using cfg.blobstruct;
//using System;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;
//using UnityEngine.UIElements;


//namespace Main
//{
//    /// <summary>
//    /// 拳套的技能逻辑
//    /// </summary>
//    public partial struct Skill_100301 : ISkillOld
//    {
//        ///<summary>0
//        ///技能的id
//        ///</summary>
//        public int id;

//        ///<summary>1
//        ///技能等级
//        ///</summary>
//        public int level;

//        ///<summary>2
//        ///冷却时间，单位秒。尽管游戏设计里面是没有冷却时间的，但是我们依然需要这个数据
//        ///因为作为一个ARPG子分类，和ARPG游戏有一样的问题：一次按键（时间够久）会发生连续多次使用技能，所以得有一个GCD来避免问题
//        ///当然和wow的gcd不同，这个“GCD”就只会让当前使用的技能进入0.1秒的冷却
//        ///</summary>
//        public float cooldown;


//        ///<summary>3
//        ///持续时间，单位：秒
//        ///</summary>
//        public float duration;


//        ///<summary>4
//        ///倍速，1=100%，0.1=10%是最小值
//        ///</summary>
//        public float timeScale;

//        ///<summary>5
//        ///Timeline的焦点对象也就是创建timeline的负责人，比如技能产生的timeline，就是技能的施法者
//        ///</summary>
//        public Entity caster;

//        ///<summary>6
//        ///技能已经运行了多少帧了 无需赋值
//        ///</summary>
//        public int tick;

//        /// <summary>7
//        /// 技能当前冷却时间 无需赋值
//        /// </summary>
//        public float curCooldown;

//        ///<summary>8
//        ///剩余时间，单位：秒
//        ///</summary>
//        public float curDuration;

//        ///<summary>9
//        ///距离这个技能最近的目标
//        ///</summary>
//        public Entity target;

//        /// <summary>
//        /// 是否是充能技能 10
//        /// </summary>


//        /// <summary>
//        /// 充能时间 毫秒 11 （充能次数充能也转换成时间进行取模）
//        /// </summary>


//        /// <summary>
//        /// 技能实体的执行时间 用于充能技能 12
//        /// </summary>


// ///<summary>13
//        ///技能从创建到现在总的tick
//        ///</summary>
//        public int totalTick;
//                ///<summary>14
//        ///是否时一次性释放技能
//        ///</summary>
//        public bool isOneShotSkill;
//        ///<summary>15
//        ///是否是确定位置坐标
//        ///</summary>
//        public bool isUseCertainPos;

//        ///<summary>16
//        ///一次性释放技能的坐标
//        ///</summary>
//        public float3 pos;        public int currentSkillEffectID;


//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;
//            //拿到当前技能效果id
//            for (int i = 0; i < skillArray.Length; ++i)
//            {
//                if (skillArray[i].id == id)
//                {
//                    currentSkillEffectID = skillArray[i].skillEffectArray[0];

//                    //cooldown = skillArray[i].cd / 1000f;
//                    break;
//                }
//            }


//            //ref var skillEffectsArray = ref inData.globalConfig.value.Value.configTbskill_effects.configTbskill_effects;
//            GenerateGlove(ref refData, inData, currentSkillEffectID, 0);

//            //GetLastParmeter(inData, ref skillEffectsArray, currentSkillEffectID);
//        }

//        private void GenerateGlove(ref TimeLineData_ReadWrite refData, TimeLineData_ReadOnly inData, int updateID,
//            int timeGap)
//        {
//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, updateID, 40000016)) return;


//            var buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, updateID, 40000016);
//            var xMeter = buff1[1].x;
//            var pushRate = buff1[2].x;
//            var pushPropRate = buff1[3].z;
//            var gloveType = buff1[3].x;

//            var buff2 = BuffHelper.UpdateParmeter(inData.globalConfig, updateID, 30000003);
//            var damageRate = buff2[1].x;
//            var damagePropRate = buff2[1].z;

//            var buff3 = BuffHelper.UpdateParmeter(inData.globalConfig, updateID, 30000004);
//            var obstDamage = buff3[1].x;


//            Entity skillEntity = default;
//            Entity skillPrefab = default;
//            if (gloveType == 2)
//            {
//                skillPrefab = inData.prefabMapData.prefabHashMap["SkillGloveRight"];
//                skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//            }
//            else
//            {
//                skillPrefab = inData.prefabMapData.prefabHashMap["SkillGloveLeft"];
//                skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//            }


//            var newpos = new LocalTransform
//            {
//                Position = inData.cdfeLocalTransform[inData.entity].Position,
//                //TODO:大小*10
//                Scale = xMeter * inData.cdfeLocalTransform[skillPrefab].Scale,
//                Rotation = inData.cdfeLocalTransform[skillPrefab].Rotation,
//            };

//            refData.ecb.SetComponent(inData.sortkey, skillEntity, newpos);
//            refData.ecb.AddComponent(inData.sortkey, skillEntity,
//                new SkillAttackData
//                {
//                    data = new SkillAttack_FollowPlayer
//                    {
//                        id = currentSkillEffectID,
//                        duration = 1f,
//                        tick = 0,
//                        caster = inData.entity,
//                        isBullet = false,
//                        hp = 0,
//                        stayAttack = false,
//                        stayAttackInterval = 0,
//                        curStayAttackInterval = 0,
//                        skillDelay = timeGap,
//                        scale = xMeter,
//                        skillPrefab = skillPrefab,
//                    }.ToSkillAttack()
//                });
//            refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//            {
//                buffID = 40000016,
//                buffArgs = new float3x4 { c0 = new float3(pushRate, 209000, pushPropRate) }
//            });
//            refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//            {
//                buffID = 30000003,
//                buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) }
//            });
//            refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//            {
//                buffID = 30000004,
//                buffArgs = new float3x4 { c0 = new float3(obstDamage, 0, 0) }
//            });

//            var buff4 = BuffHelper.UpdateParmeter(inData.globalConfig, updateID, 50000005);
//            // if (buff4.Length > 0)
//            // {
//            //     timeGap = buff4[1].x;
//            //     updateID = buff4[1].y;
//            //     if ((updateID != 1003063 || updateID != 1003073))
//            //     {
//            //         GenerateGlove(ref refData, inData, updateID, timeGap);
//            //     }
//            //     else
//            //     {
//            //         buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, updateID, 40000017);
//            //         var scale = buff1[1].x;
//            //         pushRate = buff1[2].x;
//            //         pushPropRate = buff1[2].z;
//            //
//            //
//            //         buff2 = BuffHelper.UpdateParmeter(inData.globalConfig, updateID, 30000003);
//            //         damageRate = buff2[1].x;
//            //         damagePropRate = buff2[1].z;
//            //
//            //         buff3 = BuffHelper.UpdateParmeter(inData.globalConfig, updateID, 30000004);
//            //         obstDamage = buff3[1].x;
//            //
//            //         var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//            //         var ins = refData.ecb.Instantiate(inData.sortkey, prefab);
//            //
//            //
//            //         newpos = new LocalTransform
//            //         {
//            //             Position = inData.cdfeLocalTransform[inData.entity].Position,
//            //             //TODO:大小*10
//            //             Scale = scale * inData.cdfeLocalTransform[skillPrefab].Scale,
//            //             Rotation = inData.cdfeLocalTransform[prefab].Rotation,
//            //         };
//            //
//            //         refData.ecb.SetComponent(inData.sortkey, ins, newpos);
//            //         refData.ecb.AddComponent(inData.sortkey, ins,
//            //             new SkillAttackData
//            //             {
//            //                 data = new SkillAttack_0
//            //                 {
//            //                     id = updateID,
//            //                     duration = 0.02f,
//            //                     tick = 0,
//            //                     caster = inData.entity,
//            //                     isBullet = false,
//            //                     hp = 0,
//            //                     stayAttack = false,
//            //                     stayAttackInterval = 0,
//            //                     curStayAttackInterval = 0,
//            //                     skillDelay = timeGap
//            //                 }.ToSkillAttack()
//            //             });
//            //
//            //         refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            //         {
//            //             buffID = 40000017,
//            //             buffArgs = new float3x4 { c0 = new float3(pushRate, 209000, pushPropRate) }
//            //         });
//            //         refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            //         {
//            //             buffID = 30000003,
//            //             buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) }
//            //         });
//            //         refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            //         {
//            //             buffID = 30000004,
//            //             buffArgs = new float3x4 { c0 = new float3(obstDamage, 0, 0) }
//            //         });
//            //     }
//            // }
//        }


//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //throw new NotImplementedException();
//        }

//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            // Debug.Log($"skillDuration{skillDuration}");
//        }

//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }
//    }
//}

