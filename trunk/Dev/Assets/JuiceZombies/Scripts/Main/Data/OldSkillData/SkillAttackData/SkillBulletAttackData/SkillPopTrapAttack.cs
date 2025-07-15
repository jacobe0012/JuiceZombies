//using cfg.blobstruct;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Entities.UniversalDelegates;
//using Unity.Mathematics;
//using Unity.Transforms;

//namespace Main
//{
//    //弹射陷阱
//    public partial struct SkillPopTrapAttack : ISkillAttack
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

//        public LocalTransform localTrans;

//        public int judgeDis;
//        public float3 playerPos;

//        public int trapID;
//        public bool isActive;

//        private int targetNumber;
//        //public int speed;
//        //public int skilleffectid;
//        //private int radius;
//        //private int pushRate;
//        //private int attackRate;
//        //private int attackAddRate;
//        //private int damageValue;

//        /// <summary>
//        /// 每帧做位置变动
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        /// <returns>变动后的LT</returns>
//        /// 
//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {

//        }
//        public LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            return refData.cdfeLocalTransform[inData.entity];
//        }

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            // throw new System.NotImplementedException();
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            UnityEngine.Debug.Log("OnStart-------------------------------------------");
//            if (!BuffHelper.TryGetParmeter(inData.config, trapID, 40000037)) return;
//            var buff = BuffHelper.UpdateParmeter(inData.config, trapID, 40000037);
//            targetNumber = buff[4].x + 1;
//            UnityEngine.Debug.Log($"targetNumber{targetNumber}-------------------------------------------");
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {


//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (targetNumber <= 0)
//            {
//                duration = -1f;
//            }
//            if (targetNumber > 0)
//            {

//                for (int i = 0; i < inData.allEntities.Length; ++i)
//                {
//                    var currentEnemy = inData.allEntities[i];
//                    float dis = math.distance(localTrans.Position, refData.cdfeLocalTransform[currentEnemy].Position);

//                    if (math.abs(dis - judgeDis) < 0.1f&&!inData.cdfePlayerData.HasComponent(currentEnemy))
//                    {
//                        var dir = playerPos - refData.cdfeLocalTransform[currentEnemy].Position;
//                        UnityEngine.Debug.Log("OnUpdate-------------------------------------------");
//                        if (!BuffHelper.TryGetParmeter(inData.config, trapID, 40000037)) return;
//                        var buff = BuffHelper.UpdateParmeter(inData.config, trapID, 40000037);
//                        var flyDuration = buff[0].x/1000f;
//                        var flySpeed = buff[1].x / 100f;
//                        var pushRate = buff[2].x;
//                        var pushAddtionRate = buff[2].z;
//                        var cannonballRadius = buff[3].x;

//                        var attackAdditon = buff[4].x;
//                        var pushAddtion = buff[5].x;
//                        var scaleAddtion = buff[6].x;


//                        if (!BuffHelper.TryGetParmeterNew(inData.config, trapID, 30000003)) return;
//                        var buff2 = BuffHelper.UpdateParmeterNew(inData.config, trapID, 30000003);
//                        var damageRate = buff2[0].x;
//                        var damagePropRate = buff2[0].z;

//                        if (!refData.storageInfoFromEntity.Exists(currentEnemy)) return;

//                        var targetMass = inData.cdfeChaStats[currentEnemy].chaProperty.mass;

//                        var skillPrefab = inData.prefabMapData.prefabHashMap["skill_popgap"];
//                        var popTrap = refData.ecb.Instantiate(inData.sortkey, skillPrefab);
//                        var newLoc = new LocalTransform
//                        {
//                            Position = localTrans.Position,
//                            Scale = refData.cdfeLocalTransform[skillPrefab].Scale * cannonballRadius * (1 + targetMass * scaleAddtion / 10000f),
//                            Rotation = refData.cdfeLocalTransform[skillPrefab].Rotation
//                        };
//                        refData.ecb.SetComponent(inData.sortkey, popTrap, newLoc);

//                        refData.ecb.AddComponent(inData.sortkey, popTrap, new SkillAttackData
//                        {
//                            data = new SkillTrapTransmitAttack
//                            {
//                                duration = flyDuration,
//                                tick = 0,
//                                caster = inData.player,
//                                localTrans = newLoc,
//                                skillDelay = 0,
//                                dir = dir,
//                                flySpeed = flySpeed,
//                                transmitTarget = currentEnemy,
//                                isBullet=true,
//                                hp=999
//                            }.ToSkillAttack()
//                        });


//                        //c2 = new float3(3, 0, 0)
//                        refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, popTrap,
//                        new SkillHitEffectBuffer { buffID = 40000037, buffArgs = new float3x4 { c0 = new float3(pushRate, 209000, pushAddtionRate + targetMass * pushAddtion) } });
//                        refData.ecb.AppendToBuffer<SkillHitEffectBuffer>(inData.sortkey, popTrap,
//                            new SkillHitEffectBuffer { buffID = 30000003, buffArgs = new float3x4 { c0 = new float3(damageRate, 203000, damagePropRate + attackAdditon * targetMass) } });

//                        UnityEngine.Debug.Log($"pushRate:{pushRate},damageRate:{damageRate},targetMass{targetMass},attackAdditon{attackAdditon},pushAddtion{pushAddtion}");


//                        targetNumber--;


//                    }
//                }


//            }
//        }
//    }
//}

