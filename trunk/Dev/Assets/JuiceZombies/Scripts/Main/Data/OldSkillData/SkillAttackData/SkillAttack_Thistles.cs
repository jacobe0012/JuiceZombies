//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    // 用于实现荆棘的逻辑
//    public partial struct SkillAttack_Thistles : ISkillAttack
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

//        public int reduceRate;


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

//        public void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//            if (!refData.storageInfoFromEntity.Exists(target) || inData.cdfeThronTag.HasComponent(target)) return;
//            if (BuffHelper.JudgeEquipExist(inData.entity, inData.bfePlayerEquipSkillBuffer, 405026, inData.config,
//                    out int needEffectID))
//            {
//                if (BuffHelper.TryGetParmeter(inData.config, needEffectID, 20003010))
//                {
//                    var buff = BuffHelper.UpdateParmeter(inData.config, needEffectID, 20003010);
//                    var attackRate = buff[1].x;
//                    var buffDuration = buff[2].x;
//                    refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff_20003010
//                    {
//                        id = 20003010,
//                        priority = 0,
//                        maxStack = 0,
//                        tags = 0,
//                        duration = buffDuration,
//                        permanent = false,
//                        caster = inData.player,
//                        buffArgs = new BuffArgs
//                        {
//                            args0 = attackRate
//                        },
//                        totalMoveDistance = 0,
//                        xMetresPerInvoke = 0,
//                        lastPosition = default,
//                        carrier = target,
//                        canBeStacked = false,
//                        buffStack = default,
//                        tickTime = 1,
//                        timeElapsed = 0,
//                        ticked = 0
//                    }.ToBuffOld());
//                }
//            }

//            refData.ecb.AppendToBuffer(inData.sortkey, target, new Buff_20003001
//            {
//                id = 20000301,
//                priority = 0,
//                maxStack = 0,
//                tags = 0,
//                duration = duration,
//                permanent = false,
//                caster = inData.player,
//                buffArgs = new BuffArgs
//                {
//                    args0 = reduceRate
//                },
//                totalMoveDistance = 0,
//                xMetresPerInvoke = 0,
//                lastPosition = default,
//                carrier = target,
//                canBeStacked = false,
//                buffStack = default,
//                tickTime = 0,
//                timeElapsed = 0,
//                ticked = 0
//            }.ToBuffOld());
//            refData.ecb.AddComponent(inData.sortkey, target, new ThronTag { });
//        }

//        public void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }

//        public void OnAttack(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData)
//        {
//        }
//    }
//}

