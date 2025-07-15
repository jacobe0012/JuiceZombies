//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;


//namespace Main
//{
//    //没有位置大小方向变动的技能实体
//    public partial struct SkillAttack_HammerS2 : ISkillAttack
//    {
//        /// <summary>
//        /// 0
//        /// </summary>
//        public int id;

//        /// <summary>
//        /// 1 一次性释放的技能实体 值=0
//        /// </summary>
//        public float duration;

//        /// <summary>
//        /// 2
//        /// </summary>
//        public int tick;

//        /// <summary>
//        /// 3 技能实体释放者
//        /// </summary>
//        public Entity caster;

//        /// <summary>
//        /// 4 是否是弹幕
//        /// </summary>
//        public bool isBullet;

//        /// <summary>
//        /// 5 弹幕hp
//        /// </summary>
//        public int hp;

//        /// <summary>
//        /// 6 是否是持续性攻击
//        /// </summary>
//        public bool stayAttack;

//        /// <summary>
//        /// 7 持续性攻击间隔
//        /// </summary>
//        public float stayAttackInterval;

//        /// <summary>
//        /// 8 当前持续性攻击间隔
//        /// </summary>
//        public float curStayAttackInterval;

//        /// <summary>
//        /// 技能实体执行延时 单位s  9
//        /// </summary>
//        public float skillDelay;

//        /// <summary>
//        /// 实体的击中目标 10
//        /// </summary>
//        public Entity target;

//        /// <summary>
//        /// 9
//        /// </summary>
//        public Entity skillEntity;

//        /// <summary>
//        /// 每帧做位置变动
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        /// <returns>变动后的LT</returns>
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            return refData.cdfeLocalTransform[inData.entity];
//        }

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            //UnityEngine.Debug.Log("dffdsffffffffffffffff1111111111111111111111");
//            var buff = BuffHelper.UpdateParmeterNew(inData.config, id, 50000005);
//            var gap = buff[0].x / 1000f;
//            var nextEffectID = buff[1].x;
//            ref var skillEffectsArray = ref inData.config.value.Value.configTbskill_effects.configTbskill_effects;
//            int limitDistance = 0;
//            for (int i = 0; i < skillEffectsArray.Length; i++)
//            {
//                if (skillEffectsArray[i].id == nextEffectID)
//                {
//                    limitDistance = skillEffectsArray[i].limit[1].y;
//                    break;
//                }
//            }


//            var buff1 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 40000008);

//            var width = buff1[0].x;
//            var height = buff1[1].x;
//            var pushForce = buff1[3].x;
//            var pushPropRate = buff1[3].z;
//            var diffuseSpeed = buff1[4].x / 100f;

//            var buff2 = BuffHelper.UpdateParmeterNew(inData.config, nextEffectID, 30000003);
//            var damageRate = buff2[0].x;
//            var damagePropRate = buff2[0].z;


//            float3 dir = inData.cdfeChaStats[inData.player].chaResource.direction;
//            if (IsAttackToEnemy(limitDistance, inData, ref refData))
//            {
//                dir = refData.cdfeLocalTransform[target].Position - refData.cdfeLocalTransform[inData.entity].Position;
//            }

//            dir = math.normalizesafe(dir);

//            var skillPrefab = inData.prefabMapData.prefabHashMap["SkillChainHammer"];
//            var skillEntity = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//            var newLoc = refData.cdfeLocalTransform[inData.player];
//            newLoc.Scale = width;
//            refData.ecb.AddComponent(inData.sortkey, skillEntity, new SkillAttackData
//            {
//                data = new SkillAxeFlyAttack
//                {
//                    dir = dir,
//                    id = nextEffectID,
//                    duration = height / diffuseSpeed,
//                    tick = 0,
//                    caster = inData.player,
//                    diffuseSpeed = diffuseSpeed,
//                    hp = 0,
//                    isBullet = false,
//                    skillDelay = gap,
//                    curStayAttackInterval = 0,
//                    stayAttack = false,
//                    stayAttackInterval = 0,
//                    scale = width,
//                    loc = newLoc,
//                }.ToSkillAttack()
//            });

//            refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//            {
//                buffID = 40000008,
//                buffArgs = new float3x4 { c0 = new float3(pushForce, 209000, pushPropRate) }
//            });
//            refData.ecb.AppendToBuffer(inData.sortkey, skillEntity, new SkillHitEffectBuffer
//            {
//                buffID = 30000003,
//                buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate) }
//            });
//        }

//        public bool IsAttackToEnemy(int limitDistance, SkillAttackData_ReadOnly inData,
//            ref SkillAttackData_ReadWrite refData)
//        {
//            //判断是否对怪
//            float distance = 999f;
//            target = BuffHelper.NerestTarget(inData.allEntities, refData.cdfeLocalTransform,
//                inData.cdfeChaStats, inData.cdfePlayerData, inData.cdfeEnemyData, inData.cdfeSpiritData,
//                inData.cdfeObstacleTag,
//                inData.entity, TargetAttackType.Enemy, ref distance);
//            if (distance > limitDistance)
//            {
//                target = default;
//            }

//            return target != default ? true : false;
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }
//    }
//}

