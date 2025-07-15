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
//    ///移动时造成伤害
//    /// </summary>
//    public partial struct GameEvent_1010 : IGameEvent
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

//        public void OnAbsorbBarrage(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//        }

//        public void OnAuto(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//        }

//        public void OnBeDie(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//        }

//        public void OnBeHurt(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//        }

//        public void OnCharacterEnter(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            //Debug.Log("OnCharacterEnter");
//            if (target == default || target == null) return;
//            if (!inData.cdfeAllStats.HasComponent(target)) return;

//            int args0 = 0, args1 = 0, args2 = 0;
//            ref var eventsMap = ref inData.config.value.Value.configTbevent_0s.configTbevent_0s;
//            for (int i = 0; i < eventsMap.Length; i++)
//            {
//                if (eventsMap[i].id == id)
//                {
//                    //args0 = eventsMap[i].para[0];
//                    //args1 = eventsMap[i].para[1];
//                    //args2 = eventsMap[i].para[2];
//                }
//            }

//            if (!refData.cdfeBuffs.HasBuffer(target))
//            {
//                refData.ecb.AddBuffer<BuffOld>(inData.sortKey, target);
//            }

//            refData.ecb.AppendToBuffer(inData.sortKey, target,
//                new Buff_50000006
//                {
//                    tickTime = 0.02f, id = 50000006, permanent = true, xMetresPerInvoke = args0, carrier = target,
//                    caster = inData.selefEntity,
//                    buffArgs = new BuffArgs { args0 = 2, args1 = 5, args2 = args1, args3 = args2 }
//                }.ToBuffOld());
//            //Debug.Log("OnCharacterEnter-----------------------------");
//        }

//        public void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            throw new System.NotImplementedException();
//        }

//        public void OnCharacterStay(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//        }

//        public void OnClose(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//        }

//        public void OnCollider(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//        }


//        public void OnEventRemove(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            if (target == default || target == null) return;
//            if (refData.cdfeBuffs.HasBuffer(target))
//            {
//                var buff = refData.ecb.SetBuffer<BuffOld>(inData.sortKey, target);
//                for (int i = 0; i < buff.Length; i++)
//                {
//                    if (buff[i].Int32_0 == 50000006)
//                    {
//                        buff.RemoveAt(i);
//                    }

//                    break;
//                }
//            }
//            //for(int i=0;i<buff.Length; i++)
//            //{
//            //    if (buff[i].Int32_0== 50000006)
//            //    {
//            //        buff.RemoveAt(i);
//            //    }
//            //    break;
//            //}
//            //refData.cdfeBuffs[target] = buff;
//        }


//        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            refData.ecb.AddBuffer<SkillHitEffectBuffer>(inData.sortKey, inData.selefEntity);
//        }

//        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            throw new System.NotImplementedException();
//        }

//        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            throw new System.NotImplementedException();
//        }

//        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//        }

//        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//        }
//    }
//}

