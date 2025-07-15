using Unity.Mathematics;

namespace Main
{
    /// <summary>
    ///当前存在的指定目标类型-释放技能
    /// </summary>
    public partial struct GameEvent_12 : IGameEvent
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
        }


        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            // int args0 = 0, args1 = 0;
            // float duration = 0;
            // ref var eventsMap = ref inData.config.value.Value.configTbevent_0s.configTbevent_0s;
            // for (int i = 0; i < eventsMap.Length; i++)
            // {
            //     if (eventsMap[i].id == id)
            //     {
            //         args0 = eventsMap[i].para[0];
            //         args1 = eventsMap[i].para[1];
            //         duration = eventsMap[i].interval/1000f;
            //     }
            // }
            // if (!refData.cdfeBuffs.HasBuffer(inData.selefEntity))
            // {
            //     refData.ecb.AddBuffer<Buff>(inData.sortKey, inData.selefEntity);
            // }
            //// refData.ecb.AddBuffer<SkillHitEffectBuffer>(inData.sortKey, inData.selefEntity);
            // refData.ecb.AppendToBuffer(inData.sortKey, inData.selefEntity, new Buff_2003_GameEvent  {permanent=true, tickTime=duration, carrier = inData.selefEntity, duration = duration, buffArgs = new BuffArgs { args0 = args0, args1 = args1 } }.ToBuffOld());
        }

        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnTick0(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {

        }

        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

    }
}