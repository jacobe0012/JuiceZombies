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
//    /// 每隔45s获得护盾，格挡伤害
//    /// </summary>
//    public partial struct Skill_404013 : ISkillOld
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

//            int buffDuration = 0;
//            if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, id, inData.globalConfig, out int needEffectID))
//            {
//                var buff = BuffHelper.UpdateParmeter(inData.globalConfig, needEffectID, 20002005);
//                buffDuration = buff[0].x;
//            }

//            if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 404014, inData.globalConfig, out needEffectID))
//            {
//                if (!BuffHelper.TryGetParmeter(inData.globalConfig, needEffectID, 20002005)) return;
//                var buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, needEffectID, 20002005);
//                buffDuration = buff1[0].x;
//            }

//            refData.ecb.AppendToBuffer(inData.sortkey, inData.entity, new Buff_20002001
//                {
//                    id = 20002001,
//                    priority = 0,
//                    maxStack = 0,
//                    tags = 0,
//                    tickTime = 0,
//                    timeElapsed = 0,
//                    ticked = 0,
//                    duration = buffDuration/1000f,
//                    permanent = false,
//                    caster = inData.entity,
//                    carrier = inData.entity,
//                    canBeStacked = false,
//                    buffStack = default,
//                    buffArgs = default
//                }
//                .ToBuffOld());


//        }


//        public void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            //throw new NotImplementedException();
//        }


//        public void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData)
//        {
//            if (BuffHelper.JudgeEquipExist(inData.entity,inData.bfePlayerEquipSkillBuffer, 404016, inData.globalConfig, out int needEffectID))
//            {
//                if (!BuffHelper.TryGetParmeter(inData.globalConfig, needEffectID, 30000025)) return;
//                var buff1 = BuffHelper.UpdateParmeter(inData.globalConfig, needEffectID, 30000025);
//                var killNumber = buff1[0].x;
//                var deduction = buff1[1].x;

//                if (inData.cdfePlayerData[inData.entity].playerData.killEnemy %killNumber == 0)
//                {
//                    cooldown -= deduction / 1000f;
//                }

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

