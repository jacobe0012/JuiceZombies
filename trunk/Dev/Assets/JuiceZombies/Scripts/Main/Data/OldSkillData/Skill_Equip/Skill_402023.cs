//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;
//using UnityEngine.UIElements;


//namespace Main
//{
//    /// <summary>
//    /// 生命值低于50% 打开急救气囊
//    /// </summary>
//    public partial struct Skill_402023 : ISkillOld
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


//        public void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {


//        }


//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //throw new NotImplementedException();
//        }


//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            float duration=0;
//            if (!BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, id, inData.globalConfig, out int needEffectID)) return;
//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, needEffectID, 20002006)) return;
//            var buff = BuffHelper.UpdateParmeter(inData.globalConfig, needEffectID, 20002006);
//            duration = buff[1].x/1000;
//            if (!BuffHelper.TryGetParmeter(inData.globalConfig, needEffectID, 10207020)) return;
//            var buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, needEffectID, 10207020);
//            var speedAddtion = buff1[1].x;
//            if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 402026, inData.globalConfig, out  needEffectID))
//            {
//                cooldown = 30f;
//            }
//            if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 402024, inData.globalConfig, out needEffectID))
//            {
//                duration *= 2;
//            }
//            if (inData.cdfeChaStats[inData.entity].chaResource.hp < inData.cdfeChaStats[inData.entity].chaProperty.maxHp / 2f)
//            {


//                refData.ecb.AppendToBuffer(inData.sortkey, inData.entity, new Buff_20002006
//                {
//                    carrier = inData.entity,
//                    canBeStacked = false,
//                    buffStack = default,
//                    buffArgs = default,
//                    totalMoveDistance = 0,
//                    xMetresPerInvoke = 0,
//                    lastPosition = default,
//                    oldFilter = 0,
//                    duration = duration,
//                    id = 20002006,
//                    priority = 0,
//                    maxStack = 0,
//                    tags = 0,
//                    permanent = false,
//                    caster = inData.entity,
//                    tickTime = 0,
//                    timeElapsed = 0,
//                    ticked = 0
//                }.ToBuffOld());
//                refData.ecb.AppendToBuffer(inData.sortkey, inData.entity, new Buff_10207020
//                {
//                    carrier = inData.entity,
//                    canBeStacked = false,
//                    buffStack = default,
//                    duration = duration,
//                    id = 10207020,
//                    priority = 0,
//                    maxStack = 0,
//                    tags = 0,
//                    permanent = false,
//                    caster = default,
//                    tickTime = 0,
//                    timeElapsed = 0,
//                    ticked = 0,
//                    buffArgs = new BuffArgs
//                    {
//                        args0 = speedAddtion
//                    },
//                    totalMoveDistance = 0,
//                    xMetresPerInvoke = 0,
//                    lastPosition = default
//                }.ToBuffOld());
//            }


//                Debug.Log($"continuousCollCount{inData.cdfeChaStats[inData.entity].chaResource.continuousCollCount}");

//        }

//        public void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }

//        public void OnChargeFinish(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//        }
//    }
//}

