using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    /// <summary>
    ///变更游戏速度
    /// </summary>
    public partial struct GameEvent_34 : IGameEvent
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


        public void OnAbsorbBarrage(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnAuto(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

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

        public void OnCharacterStay(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnClose(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCollider(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }


        public void OnEventRemove(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            //ChangeBackGameVelocity(ref refData,inData);
            //Debug.LogError($"game34 OnEventRemove id{id}");
        }


        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            ChangeGameVelocity(ref refData, inData);
            //Debug.LogError($"game34 OnOccur id{id}");
            //remainTime = args123.z / 1000f;
            
        }

        private void ChangeGameVelocity(ref GameEventData_ReadWrite refData, GameEventData_ReadOnly inData)
        {
            Debug.LogError($"ChangeGameVelocity");
            var temp = refData.cdfeGameTimeData[inData.wbe];

            if (args123.y == 1)
            {
                temp.refreshTime.defaultGameTimeScale = args123.x / 10000f;
                temp.refreshTime.gameTimeScale = temp.refreshTime.defaultGameTimeScale;
            }
            else if (args123.y == 2)
            {
                temp.logicTime.defaultGameTimeScale = args123.x / 10000f;
                temp.logicTime.gameTimeScale = temp.logicTime.defaultGameTimeScale;
            }


            refData.cdfeGameTimeData[inData.wbe] = temp;
        }

        private void ChangeBackGameVelocity(ref GameEventData_ReadWrite refData, GameEventData_ReadOnly inData)
        {
            var temp = refData.cdfeGameTimeData[inData.wbe];

            if (args123.y == 1)
            {
                temp.refreshTime.defaultGameTimeScale = 1;
                temp.refreshTime.gameTimeScale = temp.refreshTime.defaultGameTimeScale;
            }
            else if (args123.y == 2)
            {
                temp.logicTime.defaultGameTimeScale = 1;
                temp.logicTime.gameTimeScale = temp.logicTime.defaultGameTimeScale;
            }


            refData.cdfeGameTimeData[inData.wbe] = temp;
        }

        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            ChangeGameVelocity(ref refData, inData);
        }

        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            ChangeGameVelocity(ref refData, inData);
        }

        public void OnTick0(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            //Debug.LogError($"ChangeGameVelocity");
        }
    }
}