//using cfg.blobstruct;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;


//namespace Main
//{
//    /// <summary>
//    /// 手斧的技能逻辑
//    /// </summary>
//    public partial struct Skill_100601 : ISkillOld
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

//        ///<summary>14
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

//        /// <summary>
//        /// 充能次数 
//        /// </summary>
//        public int chargeTimes;

//        /// <summary>
//        /// 对敌距离
//        /// </summary>
//        public int limitDistance;


//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;
//            //Debug.Log($"currentSkillID:{id}");
//            //拿到当前技能效果id
//            int currentSkillEffectID = 0;
//            for (int i = 0; i < skillArray.Length; ++i)
//            {
//                //Debug.Log($"currentSkillID000000:{skillArray[i].id}");
//                if (skillArray[i].id == id)
//                {
//                    currentSkillEffectID = skillArray[i].skillEffectArray[0];
//                    //Debug.Log($"currentSkillEffectID:{currentSkillEffectID}");
//                    cooldown = skillArray[i].cd / 1000f;
//                    break;
//                }
//            }

//            ref var skillEffectsArray = ref inData.globalConfig.value.Value.configTbskill_effects.configTbskill_effects;
//            for (int i = 0; i < skillEffectsArray.Length; i++)
//            {
//                if (skillEffectsArray[i].id == currentSkillEffectID)
//                {
//                    limitDistance = skillEffectsArray[i].limit[0].y;
//                    break;
//                }
//            }

//            //Debug.Log($"currentSkillEffectID:{currentSkillEffectID}");
//            GenerateSkillEntity(currentSkillEffectID, inData, ref refData);
//        }

//        private LocalTransform SetTrans(TimeLineData_ReadOnly inData, int limitDistance)
//        {
//            //拿到对敌角度
//            if (IsAttackToEnemy(limitDistance, inData))
//            {
//                //TODO:方向
//                float3 dir = inData.cdfeLocalTransform[target].Position -
//                             inData.cdfeLocalTransform[inData.entity].Position;


//                float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//                    new Vector3(0, 1, 0));

//                return inData.cdfeLocalTransform[inData.entity].RotateZ(math.radians(needAngel));
//            }

//            return inData.cdfeLocalTransform[inData.entity];
//        }


//        public void GenerateSkillEntity(int currentEffectID, TimeLineData_ReadOnly inData,
//            ref TimeLineData_ReadWrite refData)
//        {
//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentEffectID, 40000005)) return;
//            var buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, currentEffectID, 40000005);
//            float angle = buff1[1].x;
//            var scale = buff1[2].x + buff1[2].x *
//                (int)(inData.cdfeChaStats[inData.entity].chaProperty.skillRangeRatios * buff1[2].z / 10000f);
//            // scale =5;
//            var pushRate = buff1[3].x;
//            var pushPropRate = buff1[3].z;
//            float diffuseSpeed = buff1[5].x;
//            float skillDuration = angle / diffuseSpeed;
//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentEffectID, 30000003)) return;
//            var buff2 = BuffHelper.UpdateParmeter(inData.globalConfig, currentEffectID, 30000003);
//            var damageRate = buff2[1].x;
//            var damagePropRate = buff2[1].z;


//            //Debug.Log($"scale:{scale},diffuseSpeed{diffuseSpeed}");
//            var skillPrefab = inData.prefabMapData.prefabHashMap["SkillAxe"];
//            var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);

//            var enemyLoc = SetTrans(inData, limitDistance);
//            refData.ecb.SetComponent(inData.sortkey, skillEntity, enemyLoc);
//            refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//            {
//                data = new SkillAxeAttack
//                {
//                    id = currentEffectID,
//                    angle = angle,
//                    diffuseSpeed = diffuseSpeed,
//                    duration = skillDuration,
//                    scale = scale,
//                    tick = 0,
//                    caster = inData.entity,
//                    hp = 0,
//                    isBullet = false,
//                    skillDelay = 0,
//                    stayAttack = false,
//                    stayAttackInterval = 0,
//                    curStayAttackInterval = 0
//                }.ToSkillAttack()
//            });

//            refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//            {
//                buffID = 40000005,
//                buffArgs = new float3x4 { c0 = new float3(pushRate, 209000, pushPropRate) }
//            });
//            refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//            {
//                buffID = 30000003,
//                buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) }
//            });
//        }

//        public bool IsAttackToEnemy(int limitDistance, TimeLineData_ReadOnly inData)
//        {
//            //判断是否对怪
//            float distance = 999f;
//            target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//                inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                inData.cdfeObstacleTag,
//                inData.entity, TargetAttackType.Enemy, ref distance);
//            if (distance > limitDistance)
//            {
//                target = default;
//            }

//            return target != default ? true : false;
//        }

//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }
//    }
//}

