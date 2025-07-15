//using Unity.Collections;
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
//    /// 每10s扔出一颗炸弹
//    /// </summary>
//    public partial struct Skill_404036 : ISkillOld
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


//        ///<summary>13
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
//        public float3 pos;
//        public LocalTransform curLoc;

//        private int scale;
//        private int pushRate;
//        private int speed;
//        private int offset;
//        private float distance;
//        private int currentEffectID;

//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //if (!BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, id, inData.globalConfig, out int needEffectID))
//            //    return;

//            //currentEffectID = needEffectID;

//            //if (!BuffHelper.TryGetParmeter(inData.globalConfig, needEffectID, 40000003)) return;
//            //ref var skillEffectsArray = ref inData.globalConfig.value.Value.configTbskill_effects.configTbskill_effects;
//            //for (int i = 0; i < skillEffectsArray.Length; i++)
//            //{
//            //    if (skillEffectsArray[i].id == needEffectID)
//            //    {
//            //        distance = skillEffectsArray[i].limit[0].y;
//            //        break;
//            //    }
//            //}

//            //var buff = BuffHelper.UpdateParmeter(inData.globalConfig, needEffectID, 40000003);
//            //scale = buff[2].x;
//            //pushRate = buff[3].x;
//        }


//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //throw new NotImplementedException();
//        }


//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            ////    Debug.Log($"continuousCollCount{inData.cdfeChaStats[inData.entity].chaResource.continuousCollCount}");
//            //if (tick % 10 * 50 == 0)
//            //{
//            //    var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//            //    var ins = refData.ecb.Instantiate(inData.sortkey, prefab);


//            //    target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//            //        inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//            //        inData.entity, TargetAttackType.Enemy, ref distance);

//            //    float3 dir = new float3(0, 1, 0);
//            //    if (refData.storageInfoLookup.Exists(target))
//            //    {
//            //        var randPos = refData.rand.rand.NextFloat3(-1, 1);
//            //        dir = math.mul(inData.cdfeLocalTransform[target].Rotation, randPos);
//            //    }

//            //    var tran = new LocalTransform
//            //    {
//            //        Position = inData.cdfeLocalTransform[target].Position + dir * offset,
//            //        Scale = scale * inData.cdfeLocalTransform[target].Scale,
//            //        Rotation = inData.cdfeLocalTransform[target].Rotation,
//            //    };

//            //    var dis = math.distance(inData.cdfeLocalTransform[inData.entity].Position, dir);
//            //    var skillAttackDuration = dis / speed / 100f;

//            //    refData.ecb.SetComponent(inData.sortkey, ins, tran);
//            //    refData.ecb.AddComponent(inData.sortkey, ins,
//            //        new SkillAttackData
//            //        {
//            //            data = new SkillThrowBoomAttack
//            //            {
//            //                id = currentEffectID,
//            //                duration = skillAttackDuration,
//            //                tick = 0,
//            //                caster = inData.entity,
//            //                isBullet = false,
//            //                hp = 0,
//            //                stayAttack = false,
//            //                stayAttackInterval = 0,
//            //                curStayAttackInterval = 0,
//            //                speed = speed,
//            //            }.ToSkillAttack()
//            //        });

//            //    refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            //    {
//            //        buffID = 40000003,
//            //        buffArgs = new float3x4 { c0 = new float3(1, 0, 0) }
//            //    });
//            //    refData.ecb.AppendToBuffer(inData.sortkey, ins, new SkillHitEffectBuffer
//            //    {
//            //        buffID = 30000003,
//            //        buffArgs = new float3x4 { c0 = new float3(1, 0, 0) }
//            //    });
//            //}
//        }

//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }
//    }
//}

