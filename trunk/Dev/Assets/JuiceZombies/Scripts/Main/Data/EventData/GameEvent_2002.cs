//using System;
//using System.Threading;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    /// <summary>
//    /// 死亡自爆
//    /// </summary>
//    public partial struct GameEvent_2002 : IGameEvent
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
//        }

//        public void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
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
//        }


//        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//            if (!inData.storageInfoFromEntity.Exists(inData.selefEntity)) return;

//            if (!refData.cdfeBuffs.HasBuffer(inData.selefEntity))
//            {
//                refData.ecb.AddBuffer<BuffOld>(inData.sortKey, inData.selefEntity);
//            }

//            refData.ecb.AppendToBuffer(inData.sortKey,inData.selefEntity, new Buff_DieToBoom
//            {
//                id = 2002,
//                priority = 0,
//                maxStack = 0,
//                tags = 0,
//                tickTime = 0f,
//                timeElapsed = 0,
//                ticked = 0,
//                duration = 0,
//                permanent = true,
//                caster = default,
//                carrier = inData.selefEntity,
//                canBeStacked = false,
//                buffStack = default,
//                buffArgs = default,
//                lastpushForcePlus = 0,
//                lastatkPlus = 0
//            }.ToBuffOld());
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

