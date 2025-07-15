//using cfg.blobstruct;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    //投掷长矛S 锁链
//    public partial struct SkillAttack_210301S : ISkillAttack
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


//        public float width;

//        public int deSpeedRatios;

//        public int damageRatios;

//        // public int skilleffectid;
//        // private int radius;
//        // private int pushRate;
//        // private int attackRate;
//        // private int attackAddRate;
//        // private int damageValue;


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

//        public void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            ref var bulletConfig = ref inData.config.value.Value.configTbbullets.configTbbullets;
//            ref var skillConfig = ref inData.config.value.Value.configTbskills.configTbskills;
//            ref var skilleffectConfig = ref inData.config.value.Value.configTbskill_effects.configTbskill_effects;

//            if (inData.cdfeEnemyData.HasComponent(target))
//            {
//                Debug.Log($"target{target}");
//                // refData.ecb.AppendToBuffer(inData.sortkey, target, new DamageInfo
//                // {
//                //     attacker = default,
//                //     defender = target,
//                //     tags = new DamageInfoTag
//                //     {
//                //         directDamage = true,
//                //         periodDamage = false,
//                //         reflectDamage = false,
//                //         copyDamage = false,
//                //         seckillDamage = false,
//                //         directHeal = false,
//                //         periodHeal = false
//                //     },
//                //     damage = new Damage
//                //     {
//                //         normal = 20,
//                //         bullet = 0,
//                //         collide = 0,
//                //         environment = 0
//                //     },
//                //     criticalRate = 0,
//                //     criticalDamage = 0,
//                //     hitRate = 1,
//                //     degree = 0,
//                //     pos = default,
//                //     bulletEntity = default,
//                //     disableDamageNumber = false
//                // });
//                refData.ecb.AppendToBuffer(inData.sortkey, target,
//                    new Buff_20003001
//                    {
//                        id = 20003001,
//                        priority = 0,
//                        maxStack = 0,
//                        tags = 0,
//                        tickTime = 0,
//                        timeElapsed = 0,
//                        ticked = 0,
//                        duration = 1,
//                        permanent = false,
//                        caster = default,
//                        buffArgs = new BuffArgs
//                        {
//                            args0 = 5000
//                        },
//                        carrier = target,
//                        canBeStacked = false,
//                        buffStack = default,
//                    }.ToBuffOld());
//            }
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }
//    }
//}

