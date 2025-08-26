//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-17 12:15:10
//---------------------------------------------------------------------

using FMOD.Studio;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Main
{
    //
    public partial struct Trigger_101 : ITrigger
    {
        ///<summary>0
        ///buff的id
        ///</summary>
        public int id;

        ///<summary>1
        ///剩余多少时间了，单位：s
        ///</summary>
        public float duration;

        ///<summary>2
        ///buff已经存在了多少时间了，单位：帧
        ///</summary>
        public int tick;


        ///<summary>3
        ///条件达成后延迟时间 ，单位：s 参数：x類型 y 最大  z当前
        ///</summary>
        public float3 delay;

        ///<summary>4
        ///是否已经达成条件
        ///</summary>
        public bool isTrigger;


        ///<summary>5
        ///触发器类型
        ///</summary>
        public TriggerType triggerType;

        ///<summary>6
        ///触发器类型参数
        ///1.每次数触发参数1=条件次参数2=结果次数   3:当前次数
        ///2.永久触发(光环) 1=触发间隔
        ///3.武器攻击
        ///4.每时间触发参数1=时间(s)参数2=结果次数  3:当前次数
        ///</summary>
        public float4 triggerTypeArgs;


        ///<summary>7
        ///触发器条件类型
        ///0:无条件
        ///1:效果id 来源
        ///2:效果id 目标
        ///3:属性id
        ///4.与目标距离
        ///5.目标生命万分比
        ///6.行为条件 
        ///①击杀目标    参数=target
        ///②受到伤害	
        ///③闪避	
        ///④移动	
        ///⑤复活
        ///</summary>
        public TriggerConditionType triggerConditionType;

        ///<summary>8
        ///触发器条件类型参数
        ///</summary>
        public int4 triggerConditionTypeArgs;

        ///<summary>9
        ///条件类型为effectid时 x为存储的 条件的effectId的次数
        ///条件类型为击杀怪物时 x为存储的 trigger挂上的时候的杀敌数 用于差值计算
        ///  ///条件类型为拾取道具时 x为存储的 条件的道具id的拾取数
        ///</summary>
        public float4 diyArgs;

        /// <summary>10
        /// 所属技能id
        /// </summary>
        public int skillId;

        /// <summary>11
        /// 是否禁用
        /// </summary>
        public bool isDisable;


        /// <summary>12
        /// trigger释放者
        /// </summary>
        public Entity caster;

        /// <summary>13
        /// 是否已经索敌
        /// </summary>
        public bool isSeeked;

        /// <summary>14
        /// 效果触发概率(万分比)
        /// </summary>
        public int power;

        /// <summary>
        /// 光环类的间隔计时 15
        /// </summary>
        public float haloGap;

        /// <summary>
        /// 16 是否武器技能有特殊buff 0：无 1:对caster有 2：对怪有 3：对怪有
        /// </summary>
        public int isWeaponBuff;

        /// <summary>
        /// 17 判断 等于  小于等于 大于等于
        /// </summary>
        public int compareType;

        /// <summary>
        ///18 武器攻击判定成功
        /// </summary>
        public bool isWeaponAttackTrue;

        /// <summary>
        ///19 武器释放判断成功
        /// </summary>
        public bool isWeaponCastTrue;

        /// <summary>
        /// 20 用来取索敌的位置的entity
        /// </summary>
        public Entity locer;

        /// <summary>
        /// 21 用于每时间触发的类型 
        /// </summary>
        public bool isCanJudge;

        /// <summary>
        /// 22 是否是一次性trigger 
        /// </summary>
        public bool isOnceTrigger;

        /// <summary>
        /// 23 杀敌触发的倍率
        /// </summary>
        public int rations;

        /// <summary>
        /// 24 存储蓄力的目标位置和方向
        /// </summary>
        public float3x2 targetposAndDir;

        /// <summary>
        /// 25 存储释放的位置的loc
        /// </summary>
        public LocalTransform castLoc;

        /// <summary>
        /// 26 是否是被trigger加上的trigger
        /// </summary>
        public bool isTriggerAdded;

        /// <summary>
        /// 27 是否是弹幕
        /// </summary>
        public bool isBullet;

        /// <summary>
        /// 28 是否是弹幕伤害
        /// </summary>
        public bool isBulletDamage;

        /// <summary>
        /// 29 是否有额外的武器命中效果
        /// </summary>
        public bool isWeaponHit;

        /// <summary>
        /// 29 是否有额外的武器命中效果
        /// </summary>
        public FixedList128Bytes<EventInstance> audioes;

        public FixedList128Bytes<int> audioIds;


        public void OnStart(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData)
        {
            
        }

        public void OnUpdate(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData)
        {
        }

        public void OnTrigger(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData)
        {
        }
    }
}