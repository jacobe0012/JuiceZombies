//using cfg.blobstruct;
//using Codice.Client.Common;
//using System;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;
//using UnityEngine.UIElements;


//namespace Main
//{
//    /// <summary>
//    /// boss技能----防御反击
//    /// </summary>
//    public partial struct Skill_606013 : ISkillOld
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

//        private int currentSkillEffectID;


//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {

//            ref var skillArray = ref inData.globalConfig.value.Value.configTbskills.configTbskills;

//            //拿到当前技能效果id

//            for (int i = 0; i < skillArray.Length; ++i)
//            {
//                if (skillArray[i].id == id)
//                {
//                    currentSkillEffectID = skillArray[i].skillEffectArray[0];

//                    break;
//                }
//            }
//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID,out var buff, 20002017)) return;
//            var buffDuration = buff[0].x / 1000f;
//            var limitDis = buff[1].x;
//            var nextSkillEffectID = buff[2].x;

//            refData.ecb.AppendToBuffer(inData.sortkey, caster, new Buff_20002017
//            {
//                id = 20002017,
//                priority = 0,
//                maxStack = 0,
//                tags = 0,
//                tickTime = 0,
//                timeElapsed = 0,
//                ticked = 0,
//                duration = buffDuration,
//                permanent = false,
//                caster = caster,
//                carrier = caster,
//                canBeStacked = false,
//                buffStack = default,
//                buffArgs = new BuffArgs
//                {
//                    args0 = limitDis,
//                    args1 = nextSkillEffectID,
//                    args2 = 0,
//                    args3 = 0,
//                    args4 = 0
//                },
//                totalMoveDistance = 0,
//                xMetresPerInvoke = 0,
//                lastPosition = default
//            }.ToBuffOld());


//        }


//        private float3 GetRandOffsetPos(ref TimeLineData_ReadWrite refData, TimeLineData_ReadOnly inData, int offset, Entity target, int i)
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
//            float3 randomPoint = new float3(targetPos.x + xOffset, targetPos.y + yOffset, 0);
//            //Debug.Log($"randomPoint{randomPoint}");
//            return randomPoint;
//        }


//        private float3 GetRandOffsetPosFilter(ref TimeLineData_ReadWrite refData, TimeLineData_ReadOnly inData, int offset, Entity target)
//        {

//            float3 randomPoint = float3.zero;
//            do
//            {
//                var targetPos = inData.cdfeLocalTransform[target].Position;

//                var random = new Unity.Mathematics.Random();

//                // 在圆内生成随机半径和角度
//                float randomRadius = (float)random.NextFloat() * offset;
//                float randomAngle = (float)random.NextFloat() * math.PI * 2;

//                // 将极坐标转换为笛卡尔坐标
//                float xOffset = randomRadius * math.cos(randomAngle);
//                float yOffset = randomRadius * math.sin(randomAngle);

//                // 在中心点上偏移生成的随机坐标
//                randomPoint = new float3(targetPos.x + xOffset, targetPos.y + yOffset, 0);
//            }
//            while (!IsPosCanUse(randomPoint,ref refData,inData));

//            //Debug.Log($"randomPoint{randomPoint}");
//            return randomPoint;
//        }

//        private bool IsPosCanUse(float3 randomPoint, ref TimeLineData_ReadWrite refData, TimeLineData_ReadOnly inData)
//        {
//             ref var scene_module = ref inData.globalConfig.value.Value.configTbscene_modules.configTbscene_modules;
//            for (int i = 0; i < inData.mapModules.Length; i++)
//            {

//                for(int j=0;j<scene_module.Length; j++)
//                {
//                    if (scene_module[i].id == inData.cdfeMapElementData[inData.mapModules[i]].elementID)
//                    {
//                        var pos = inData.cdfeLocalTransform[inData.mapModules[i]].Position;

//                        Rect rect = new Rect(pos.x, pos.y, scene_module[i].size[0], scene_module[i].size[1]);
//                        if (rect.Contains(randomPoint))
//                        {
//                            return false;
//                        }
//                        break;
//                    }
//                }

//            }
//            return true;
//        }

//        private LocalTransform SetTransToEnemy(TimeLineData_ReadOnly inData, int limitDistance)
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

//        private LocalTransform SetTransToPlayer(TimeLineData_ReadOnly inData, int limitDistance)
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
//            //if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentEffectID, 40000005)) return;
//            //var buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, currentEffectID, 40000005);
//            //float angle = buff1[1].x;
//            //var scale = buff1[2].x + buff1[2].x *
//            //    (int)(inData.cdfeChaStats[inData.entity].chaProperty.skillRangeRatios * buff1[2].z / 10000f);
//            //// scale =5;
//            //var pushRate = buff1[3].x;
//            //var pushPropRate = buff1[3].z;
//            //float diffuseSpeed = buff1[5].x;
//            //float skillDuration = angle / diffuseSpeed;
//            //if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentEffectID, 30000003)) return;
//            //var buff2 = BuffHelper.UpdateParmeter(inData.globalConfig, currentEffectID, 30000003);
//            //var damageRate = buff2[1].x;
//            //var damagePropRate = buff2[1].z;


//            ////Debug.Log($"scale:{scale},diffuseSpeed{diffuseSpeed}");
//            //var skillPrefab = inData.prefabMapData.prefabHashMap["SkillAxe"];
//            //var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);

//            //var enemyLoc = SetTrans(inData, limitDistance);
//            //refData.ecb.SetComponent(inData.sortkey, skillEntity, enemyLoc);
//            //refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//            //{
//            //    data = new SkillAxeAttack
//            //    {
//            //        id = currentEffectID,
//            //        angle = angle,
//            //        diffuseSpeed = diffuseSpeed,
//            //        duration = skillDuration,
//            //        scale = scale,
//            //        tick = 0,
//            //        caster = inData.entity,
//            //        hp = 0,
//            //        isBullet = false,
//            //        skillDelay = 0,
//            //        stayAttack = false,
//            //        stayAttackInterval = 0,
//            //        curStayAttackInterval = 0
//            //    }.ToSkillAttack()
//            //});

//            //refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//            //{
//            //    buffID = 40000005,
//            //    buffArgs = new float3x4 { c0 = new float3(pushRate, 209000, pushPropRate) }
//            //});
//            //refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//            //{
//            //    buffID = 30000003,
//            //    buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) }
//            //});
//        }

//        public bool IsAttackToEnemy(int limitDistance, TimeLineData_ReadOnly inData)
//        {
//            //判断是否对怪
//            float distance = 999f;
//            target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//                inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                inData.cdfeObstacleTag,
//                inData.entity, TargetAttackType.Player, ref distance);
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

//            //while (attackTimes > 0)
//            //{
//            //    attackTimes--;

//            //    var prefab = inData.prefabMapData.prefabHashMap["skill_transparent_circle"];
//            //    var entity = refData.ecb.Instantiate(inData.sortkey, prefab);


//            //    if (!BuffHelper.TryGetParmeter(inData.globalConfig, currentSkillEffectID, 50000017)) return;
//            //    var buff = BuffHelper.UpdateParmeter(inData.globalConfig, currentSkillEffectID, 50000017);
//            //    int offset = buff[1].x;
//            //    float jumpDuration = buff[0].x / 1000f;


//            //    //TODO:设置对敌目标

//            //    float distance = 9999;
//            //    target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//            //        inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//            //        inData.cdfeObstacleTag,
//            //        inData.entity, TargetAttackType.Player, ref distance);

//            //    float3 finalPos = GetRandOffsetPosFilter(ref refData, inData, offset, target);


//            //    refData.ecb.AddComponent(inData.sortkey, entity,
//            //        new SkillAttackData
//            //        {
//            //            data = new SkillAttack_BossJump
//            //            {
//            //                id = currentSkillEffectID,
//            //                duration = jumpDuration,
//            //                tick = 0,
//            //                caster = inData.entity,
//            //                isBullet = false,
//            //                hp = 0,
//            //                stayAttack = false,
//            //                stayAttackInterval = 0,
//            //                curStayAttackInterval = 0,
//            //                skillDelay = 0,
//            //                target = default,
//            //                flowed = inData.entity,
//            //                finalPoint = finalPos,
//            //                maxDuration = jumpDuration
//            //            }.ToSkillAttack()
//            //        });
//            //}

//            //if (tick == delayTime)
//            //{

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

