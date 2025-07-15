//using cfg.blobstruct;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;


//namespace Main
//{
//    //投掷长矛
//    public partial struct Skill_210301 : ISkillOld
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

//        public int width;
//        public int speed;
//        public int pushForce;
//        public int deSpeedRatios;
//        public int damageRatios;

//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }


//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            ref var bulletConfig = ref inData.globalConfig.value.Value.configTbbullets.configTbbullets;
//            ref var skillConfig = ref inData.globalConfig.value.Value.configTbskills.configTbskills;

//            ref var skilleffectConfig = ref inData.globalConfig.value.Value.configTbskill_effects.configTbskill_effects;

//            int skilleffectid = 0;

//            for (int i = 0; i < skillConfig.Length; i++)
//            {
//                if (skillConfig[i].id == id)
//                {
//                    skilleffectid = skillConfig[i].skillEffectArray[0];
//                    break;
//                }
//            }

//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, skilleffectid, out var buff1))
//                return;


//            //var buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, skilleffectid, buffEntityId);
//            var entityDuration = buff1[0].x;
//            width = buff1[1].x;
//            speed = buff1[2].x;
//            pushForce = buff1[3].x;
//            //TODO：
//            int deSpeedRatios = 0;
//            int damageRatios = 0;
//            if (skilleffectid == 2103061)
//            {
//                deSpeedRatios = buff1[4].x;
//                damageRatios = buff1[5].x;
//            }


//            //UpdateParmeter(in inData, skilleffectid);

//            var prefab = inData.prefabMapData.prefabHashMap["SkillSpear"];
//            var entity = refData.ecb.Instantiate(inData.sortkey, prefab);

//            //TODO:限制索敌范围
//            float distance = 9999;

//            target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//                inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                inData.cdfeObstacleTag,
//                inData.entity, TargetAttackType.Enemy, ref distance);

//            float3 dir = new float3(0, 1, 0);
//            if (refData.storageInfoFromEntity.Exists(target))
//            {
//                dir = inData.cdfeLocalTransform[target].Position -
//                      inData.cdfeLocalTransform[inData.entity].Position;
//            }

//            float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir), new float3(0, 1, 0));
//            //Debug.Log($"needAngel{needAngel}");

//            var qua = quaternion.AxisAngle(new float3(0, 0, 1), math.radians(needAngel));
//            var tran = new LocalTransform
//            {
//                Position = inData.cdfeLocalTransform[inData.entity].Position,
//                //TODO:
//                Scale = width,
//                Rotation = qua
//            };

//            refData.ecb.SetComponent(inData.sortkey, entity, tran);

//            //refData.ecb.AddComponent(inData.sortkey, entity, new SkillAttackData
//            //{
//            //    data = new SkillBulletAttack_210301
//            //    {
//            //        id = id,
//            //        duration = entityDuration / 1000f,
//            //        tick = 0,
//            //        caster = inData.entity,
//            //        isBullet = true,
//            //        hp = 9999,
//            //        target = default,
//            //        speed = speed,
//            //        isS = skilleffectid == 2103061,
//            //        width = width,
//            //        deSpeedRatios = deSpeedRatios,
//            //        damageRatios = damageRatios
//            //    }.ToSkillAttack()
//            //});

//            //var temp = refData.cdfeChaStats[inData.entity];
//            var buff2 = BuffHelper.UpdateParmeter(inData.globalConfig, skilleffectid, 30000003);
//            //var damageRate = buff2[1].x;

//            //Debug.LogError($"buff1[3].x{buff1[3].x}buff1[3].y{buff1[3].y}buff1[3].z{buff1[3].z}");
//            refData.ecb.AppendToBuffer(inData.sortkey, entity, new SkillHitEffectBuffer
//            {
//                buffID = 40000003,
//                buffArgs = new float3x4 { c0 = new float3(buff1[3].x, buff1[3].y, buff1[3].z) }
//            });
//            refData.ecb.AppendToBuffer(inData.sortkey, entity, new SkillHitEffectBuffer
//            {
//                buffID = 30000003,
//                buffArgs = new float3x4 { c0 = new float3(buff2[1].x, buff2[1].y, buff2[1].z) }
//            });

//            // refData.ecb.AppendToBuffer(inData.sortkey, entity, new SkillHitEffectBuffer
//            // {
//            //     buffID = 30000004,
//            //     buffArgs = new float3x4 { c0 = new float3(obstDamage, 0, 0) }
//            // });


//            // var skillBaseBallBat = inData.prefabMapData.prefabHashMap["SkillBaseBallBat"];
//            // var entity = refData.ecb.Instantiate(inData.sortkey, skillBaseBallBat);


//            // target = BuffHelper.NerestTarget(inData.allEntities, inData.cdfeLocalTransform,
//            //     inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//            //     inData.entity, TargetAttackType.Enemy);

//            // if (refData.storageInfoLookup.Exists(target))
//            // {
//            //     Debug.Log($"target");
//            //
//            //     var newdir =
//            //         math.normalizesafe(inData.cdfeLocalTransform[target].Position -
//            //                            inData.cdfeLocalTransform[inData.entity].Position);
//            //
//            //
//            //     var newpos = new LocalTransform
//            //     {
//            //         Position = inData.cdfeLocalTransform[inData.entity].Position,
//            //         Scale = inData.cdfeLocalTransform[skillBaseBallBat].Scale,
//            //         Rotation = quaternion.LookRotation(new float3(newdir.x, newdir.y, 0),
//            //             new float3(0, 0, 1))
//            //     };
//            //
//            //     //Quaternion.LookRotation(Vector3.forward, directionToEnemy);
//            //     //Quaternion.LookRotation(Vector3.forward, directionToEnemy);
//            //     // var newrot =
//            //     //     quaternion.LookRotation(new float3(newdir.x, newdir.y, 0),
//            //     //         new float3(0, 1, 0));
//            //
//            //     refData.ecb.SetComponent(inData.sortkey, entity, newpos);
//            // }
//        }

//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }
//    }
//}

