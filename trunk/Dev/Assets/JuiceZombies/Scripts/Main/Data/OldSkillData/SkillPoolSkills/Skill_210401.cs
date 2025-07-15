//using cfg.blobstruct;
//using System;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;


//namespace Main
//{
//    /// <summary>
//    /// 震撼弹
//    /// </summary>
//    public partial struct Skill_210401 : ISkillOld
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

//        ///<summary>10
//        ///技能从创建到现在总的tick
//        ///</summary>
//        public int totalTick;

//        ///<summary>11
//        ///是否时一次性释放技能
//        ///</summary>
//        public bool isOneShotSkill;

//        ///<summary>12
//        ///是否是确定位置坐标
//        ///</summary>
//        public bool isUseCertainPos;

//        ///<summary>13
//        ///一次性释放技能的坐标
//        ///</summary>
//        public float3 pos;

//        /// <summary>
//        /// ...
//        /// </summary>
//        public int4x4 args;
//        public int limitDistance;

//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;
//            int currentSkillEffectID=0;
//            //拿到当前技能效果id
//            for (int i = 0; i < skillArray.Length; ++i)
//            {
//                if (skillArray[i].id == id)
//                {
//                    currentSkillEffectID = skillArray[i].skillEffectArray[0];
//                }
//            }
//            ref var skillEffectsArray = ref inData.globalConfig.value.Value.configTbskill_effects.configTbskill_effects;
//            for(int i=0;i<skillEffectsArray.Length; ++i)
//            {
//                if (skillEffectsArray[i].id == currentSkillEffectID)
//                {
//                    limitDistance = skillEffectsArray[i].limit[0].y;
//                }
//            }

//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 40000028))return;
//            var buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 40000028);
//            var count = buff1[3].x;

//            for(int i = 0; i < count; i++)
//            {
//                //Debug.Log($"11111111111111111:{i+1}次");
//                GenerateStunGrenade(ref refData, inData,currentSkillEffectID,0.1f*i,i);
//            }


//        }

//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //throw new NotImplementedException();
//        }

//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {

//        }

//        private  void GenerateStunGrenade(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData,int currentSkillEffectID,float skillDelay,int i)
//        {
//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 40000028)) return;
//            var buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 40000028);
//            float scale = buff1[1].x;
//            var scalePropRate = buff1[1].z;
//            scale *= (1 + scalePropRate / 10000f);


//            var buff3 = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 50000010);
//            var throwSpeed = buff3[1].x / 100;
//            //var throwSpeed = 1;
//            var offset = buff3[2].x;

//            //BuffHelper.SetSkillAttackTarget(inData.globalConfig, currentSkillEffectID,);

//            if (!IsAttackToEnemy(limitDistance, inData)) return;


//            var targetPos = GetRandOffsetPos(ref refData, inData, offset, target,i);
//            var flyDis= math.distance(targetPos, inData.cdfeLocalTransform[inData.entity].Position);
//            var flyDuration = flyDis / throwSpeed;
//            //实例化skill1
//            var skillPrefab = inData.prefabMapData.prefabHashMap["skill_stungrenade"];
//            var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);


//            //Debug.Log($"flyDuration:{flyDuration}");
//            var dir= GetRandOffsetPos(ref refData, inData, offset, target, i) - inData.cdfeLocalTransform[inData.entity].Position;
//            float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
//                      new float3(0, 1, 0));
//            Debug.Log($"needAngel{needAngel}");

//            var qua = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));
//            var newpos = new LocalTransform
//            {
//                Position = inData.cdfeLocalTransform[inData.entity].Position,
//                Scale =  inData.cdfeLocalTransform[skillPrefab].Scale*scale,
//                Rotation = qua,
//            };

//            // Debug.Log($"position{inData.cdfeLocalTransform[inData.entity].Position}");
//            refData.ecb.SetComponent(inData.sortkey, skillEntity, newpos);

//            refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//            {
//                data = new SkillAttack_Throw210401
//                {
//                    duration = flyDuration,
//                    tick = 0,
//                    caster = inData.entity,
//                    hp = 999,
//                    isBullet = true,
//                    stayAttack=false,
//                    curStayAttackInterval = 0,
//                    stayAttackInterval = 0,
//                    skillDelay=skillDelay,
//                    id=currentSkillEffectID,
//                    speed=throwSpeed,
//                }.ToSkillAttack()
//            });
//        }


//        private float3 GetRandOffsetPos(ref TimeLineData_ReadWrite refData, TimeLineData_ReadOnly inData, int offset, Entity target,int i)
//        {
//            var targetPos = inData.cdfeLocalTransform[target].Position;

//            var random = Unity.Mathematics.Random.CreateFromIndex((uint)i);

//            // 在圆内生成随机半径和角度
//            float randomRadius = (float)random.NextFloat() * offset;
//            float randomAngle = (float)random.NextFloat() * math.PI * 2;

//            // 将极坐标转换为笛卡尔坐标
//            float xOffset = randomRadius * math.cos(randomAngle);
//            float yOffset = randomRadius * math.sin(randomAngle);

//            // 在中心点上偏移生成的随机坐标
//            float3 randomPoint = new float3(targetPos.x + xOffset, targetPos.y + yOffset,0);
//            //Debug.Log($"randomPoint{randomPoint}");
//            return randomPoint;
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

//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }
//    }
//}

