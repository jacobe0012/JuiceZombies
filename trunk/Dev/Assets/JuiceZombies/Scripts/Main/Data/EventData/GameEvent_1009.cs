//using System.Threading;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace Main
//{
//    /// <summary>
//    ///进入地形时降低固定移动速度,只有进入的时候降低一次
//    /// </summary>
//    public partial struct GameEvent_1009 : IGameEvent
//    {

//        /// <summary>
//        ///事件id 0
//        /// </summary>
//        public int id;

//        /// <summary>
//        /// 触发事件类型 1
//        /// </summary>
//        public int triggerType;

//        /// <summary>
//        /// 执行事件类型 2
//        /// </summary>
//        public int eventType;

//        /// <summary>
//        /// 触发间隔时间 3
//        /// 当触发类型为 间隔触发(7)时用该参数
//        /// </summary>
//        public float triggerGap;

//        /// <summary>
//        /// 剩余执行时间 4
//        /// </summary>
//        public float remainTime;

//        /// <summary>
//        /// 已经执行时间 5
//        /// </summary>
//        public float duration;

//        /// <summary>
//        /// 是否永久事件 6
//        /// 永久存在
//        /// </summary>
//        public bool isPermanent;

//        /// <summary>
//        /// 目标 7
//        /// </summary>
//        public Entity target;

//        /// <summary>
//        /// 单次触发事件点 8
//        /// </summary>
//        public float onceTime;

//        /// <summary>
//        /// 用于触发障碍物靠近等机制 9
//        /// </summary>
//        public int colliderScale;

//        /// <summary>
//        /// 触发延迟
//        /// </summary>
//        public float delayTime;

//        public void OnBeDie(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }

//        public void OnBeHurt(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }

//        public void OnCharacterEnter(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            //Debug.Log("OnCharacterEnter");
//            if (!inData.cdfeAllStats.HasComponent(target)|| !inData.cdfeEnemyData.HasComponent(target))
//            {
//                return;
//            }

//            int reduceRata = 0;
//            ref var eventsMap = ref inData.config.value.Value.configTbevent_0s.configTbevent_0s;
//            for (int i = 0; i < eventsMap.Length; i++)
//            {
//                if (eventsMap[i].id == id)
//                {
//                    reduceRata = eventsMap[i].para1[0];
//                    break;
//                }
//            }
//            refData.ecb.AppendToBuffer(inData.sortKey, target, new Buff_20003001
//            {
//                id = 9991009,
//                priority = 0,
//                maxStack = 0,
//                tags = 0,
//                tickTime = 0,
//                timeElapsed = 0,
//                ticked = 0,
//                duration = 0,
//                permanent = true,
//                caster = inData.selefEntity,
//                carrier = target,
//                canBeStacked = false,
//                buffStack = default,
//                buffArgs = default,
//                totalMoveDistance = 0,
//                xMetresPerInvoke = 0,
//                lastPosition = default
//            }.ToBuffOld());
//        }

//        public void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            if (!inData.storageInfoFromEntity.Exists(target) || !refData.cdfeBuffs.HasBuffer(target)) return;
//            var buffs = refData.cdfeBuffs[target];
//            for (int i = 0; i < buffs.Length; i++)
//            {
//                if (buffs[i].Entity_9 == inData.selefEntity&&buffs[i].Int32_0==9991009)
//                {
//                    buffs.RemoveAt(i);
//                    break;
//                }
//            }

//        }

//        public void OnCollider(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }

//        public void OnEventRemove(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {


//        }


//        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }

//        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }

//        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }

//        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }

//        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }
//    }
//}

