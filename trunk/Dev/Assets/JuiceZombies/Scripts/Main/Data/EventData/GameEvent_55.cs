using System.Diagnostics;
using Unity.Mathematics;

namespace Main
{
    /// <summary>
    ///将【参数1】的技能的效果替换为技能【参数2】
    /// </summary>
    public partial struct GameEvent_55 : IGameEvent
    {
        /// <summary>
        ///事件id 0
        /// </summary>
        public int id;

        /// <summary>
        /// 触发事件类型id 1
        /// </summary>
        public GameEventTriggerType triggerType;

        /// <summary>
        /// 触发间隔时间 2
        /// 当 触发事件类型为 间隔触发(3)时用该参数
        /// </summary>
        public float triggerGap;

        /// <summary>
        /// 指定时间(2)触发时间点 3
        /// </summary>
        public float onceTime;

        /// <summary>
        /// 剩余执行时间 4
        /// </summary>
        public float remainTime;

        /// <summary>
        /// 已经执行时间的tick  5
        /// </summary>
        public int durationTick;

        /// <summary>
        /// 是否永久事件 6
        /// 永久存在
        /// </summary>
        public bool isPermanent;

        /// <summary>
        /// 参数123   7
        /// </summary>
        public int3 args123;

        /// <summary>
        /// 参数456   8
        /// </summary>
        public int3 args456;

        /// <summary>
        /// 数量      9
        /// </summary>
        public int num;


        /// <summary>
        /// 触发次数上限 10
        /// </summary>
        public int maxLimit;

        /// <summary>
        /// 是否启用 11
        /// </summary>
        public bool isActive;

        /// <summary>
        /// 是否是环境事件 12
        /// </summary>
        public bool isEnvEvent;

        /// <summary>
        /// 是否是随机事件 13
        /// </summary>
        public bool isRandomEvent;
        
        /// <summary>
        /// 参数789   14
        /// </summary>
        public int3 args789;

        /// <summary>
        /// 技能id   15
        /// </summary>
        public int skillId;

        public void OnBeDie(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnBeHurt(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCharacterEnter(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCollider(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }


        public void OnEventRemove(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }


        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
          ReplaceSkill(ref refData, inData);
            //Displaceinjury(ref refData, inData);
        }

        private  void ReplaceSkill(ref GameEventData_ReadWrite refData, GameEventData_ReadOnly inData)
        {
            var skills = refData.cdfeSkills[inData.selefEntity];
            var triggers=refData.cdfeTriggers[inData.selefEntity];
            var buffs=refData.cdfeBuffs[inData.selefEntity];
            var gameevents = refData.cdfeGameEvent;
            bool isContains=false;
            foreach(var skill in skills)
            {
                if (skill.Int32_0 == args123.x)
                {
                    isContains = true;
                    break;
                }
            }
            if (isContains)
            {
                UnityEngine.Debug.Log($"55,old:{args123.x},new:{args123.y}");
                BuffHelper.RemoveOldSkill(args123.x, ref skills, ref triggers, ref buffs,ref gameevents);
                BuffHelper.AddSkillByEcb(ref refData.ecb, inData.sortKey, args123.y, inData.selefEntity, 1);
                isPermanent = false;
            }


           
        }

  

        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            ReplaceSkill(ref refData, inData);
            //Displaceinjury(ref refData, inData);
        }

        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
         
            ReplaceSkill(ref refData, inData);
            //Displaceinjury(ref refData, inData);
        }


        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

    }
}