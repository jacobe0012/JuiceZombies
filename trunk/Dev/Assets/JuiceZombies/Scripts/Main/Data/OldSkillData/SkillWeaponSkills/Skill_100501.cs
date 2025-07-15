//using cfg.blobstruct;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Physics.Extensions;
//using Unity.Transforms;
//using UnityEngine;


//namespace Main
//{
//    /// <summary>
//    /// 链锤的技能逻辑
//    /// </summary>
//    public partial struct Skill_100501 : ISkillOld
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
//           // Debug.Log("OnCast");
//            ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;
//            //拿到当前技能效果id

//            for (int i = 0; i < skillArray.Length; ++i)
//            {
//                if (skillArray[i].id == id)
//                {
//                    currentSkillEffectID = skillArray[i].skillEffectArray[0];
//                    cooldown = skillArray[i].cd / 1000f;
//                }
//            }


//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 40000015)) return;
//            var buff = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 40000015);
//            var scale = buff[1].x;
//            var rollDuration = buff[0].x / 1000f;
//            var rollSpeed = buff[2].x;

//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 30000004)) return;
//            var buff3 = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 30000004);
//            var obstDamage = buff3[1].x;


//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 30000003)) return;
//            var buff2 = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 30000003);
//            var damageRate = buff2[1].x;
//            var damagePropRate = buff2[1].z;

//            // Debug.Log($"skillEffectID444444为{currentSkillEffectID}");
//            var skillPrefab = inData.prefabMapData.prefabHashMap["SkillChainHammer"];
//            var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);

//            if (currentSkillEffectID != 1005071)
//            {
//                refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//                {

//                    data = new SkillAttack_Hammer
//                    {
//                        id = currentSkillEffectID,
//                        caster = inData.entity,
//                        duration = rollDuration,
//                        skillDelay = 0,
//                        curStayAttackInterval = 0,
//                        hp = 999,
//                        scale = scale * inData.cdfeLocalTransform[skillPrefab].Scale,
//                        isBullet = true,
//                        stayAttack = false,
//                        stayAttackInterval = 0,
//                        tick = 0,
//                        loc = inData.cdfeLocalTransform[inData.entity],
//                        target = default,
//                        rollSpeed = rollSpeed,
//                    }.ToSkillAttack()
//                });

//                refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//                {
//                    buffID = 30000004,
//                    buffArgs = new float3x4 { c0 = new float3(obstDamage, 0, 0) }
//                });
//            }


//            //var dir = inData.cdfeChaStats[inData.entity].chaResource.direction;
//            //float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//            //    new Vector3(0, 1, 0));

//            //var qua = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));
//            //// //更新物体的位置
//            //var newLoc = new LocalTransform
//            //{
//            //    Position = inData.cdfeLocalTransform[inData.entity].Position +
//            //               dir * offset,
//            //    Scale = inData.cdfeLocalTransform[skillPrefab].Scale * radius,
//            //    Rotation = qua,
//            //};
//            //refData.ecb.SetComponent(inData.sortkey, skillEntity, newLoc);
//            //refData.ecb.AddComponent<QueryData>(inData.sortkey, skillEntity, new QueryData { CollectAllHits = true, Direction = dir, Distance = radius, ColliderDataInitialized = false, ColliderType = ColliderType.Sphere, InputColliderScale = 1, hitPos = new float3(StaticEnumDefine.PosMaxValue, StaticEnumDefine.PosMaxValue, 0) });


//        }

//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            if (currentSkillEffectID == 1005071)
//            {
//                if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 40000019)) return;
//                var buff = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 40000019);
//                var scale = buff[1].x;
//                //cooldown = buff[1].x;
//                var pushRate = buff[3].x;
//                var pushPropRate = buff[3].z;
//                var skillDuration = (int)(buff[0].x / 1000f*50);
//                if(tick%skillDuration == 0)
//                {
//                    var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//                    var ins = refData.ecb.Instantiate(inData.sortkey, prefab);


//                    var newpos = new LocalTransform
//                    {
//                        Position = inData.cdfeLocalTransform[inData.entity].Position,
//                        //TODO:大小*10
//                        Scale = scale,
//                        Rotation = inData.cdfeLocalTransform[prefab].Rotation,
//                    };

//                    refData.ecb.SetComponent(inData.sortkey, ins, newpos);
//                    refData.ecb.AddComponent(inData.sortkey, ins,
//                        new SkillAttackData
//                        {
//                            data = new SkillAttack_0
//                            {
//                                id = currentSkillEffectID,
//                                duration = 0.02f,
//                                tick = 0,
//                                caster = inData.entity,
//                                isBullet = false,
//                                hp = 0,
//                                stayAttack = false,
//                                stayAttackInterval = 0,
//                                curStayAttackInterval = 0,
//                                skillDelay = 0
//                            }.ToSkillAttack()
//                        });
//                    refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//                    {
//                        buffID = 40000019,
//                        buffArgs = new float3x4 { c0 = new float3(pushRate, 209000, pushPropRate) }
//                    });
//                }


//            }

//        }

//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }
//    }
//}

