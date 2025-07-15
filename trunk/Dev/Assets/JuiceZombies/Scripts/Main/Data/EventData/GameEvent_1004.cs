//using cfg.config;
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
//    ///造成可叠加伤害 对怪
//    /// </summary>
//    public partial struct GameEvent_1004 : IGameEvent
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
//          //  throw new System.NotImplementedException();
//        }

//        public void OnBeHurt(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }

//        public void OnCharacterEnter(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            if (!inData.storageInfoFromEntity.Exists(target)) return;
//            if(!inData.cdfeEnemyData.HasComponent(target))return;
//            if (!refData.cdfeBuffs.HasBuffer(target))return;
//            //Debug.Log($"OnCharacterEnter,target:{target}, tick{tick}");

//            int args0 = 0, args1 = 0,args2=0;
//            ref var eventsMap = ref inData.config.value.Value.configTbevent_0s.configTbevent_0s;
//            for (int i = 0; i < eventsMap.Length; i++)
//            {
//                if (eventsMap[i].id == id)
//                {
//                    args0 = eventsMap[i].para1[0];
//                    args1 = eventsMap[i].para2[0];
//                    args2 = eventsMap[i].para3[0];
//                }
//            }

//            var damageValue = (args0 + (int)math.floor(inData.cdfeAllStats[target]
//                                                           .chaProperty.maxHp *
//                                                       args1 /
//                                                       10000f));

//            refData.ecb.AppendToBuffer(inData.sortKey, target, new Buff_300000031
//            {
//                id = 9991004,
//                priority = 0,
//                maxStack = args2,
//                tags = 0,
//                tickTime = 1f,
//                timeElapsed = 0,
//                ticked = 0,
//                duration = 0,
//                permanent = true,
//                caster = inData.selefEntity,
//                carrier = target,
//                canBeStacked = false,
//                buffStack = default,
//                buffArgs = new BuffArgs { args0 = damageValue },
//            }.ToBuffOld());


//        }

//        public void OnCollider(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }


//        public void OnEventRemove(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }


//        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            // var physicsCollider = refData.cdfePhysicsCollider[inData.selefEntity];
//            // //var oldFilter = physicsCollider.Value.Value.GetCollisionFilter().CollidesWith;
//            // var newCollider= PhysicsHelper.CreateColliderWithCollidesWith(physicsCollider, (uint)63);
//            // refData.cdfePhysicsCollider[inData.selefEntity]= newCollider;
//        }


//        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//          //  throw new System.NotImplementedException();
//        }

//        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {

//        }

//        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//        }

//        public void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            if (!inData.storageInfoFromEntity.Exists(target)) return;
//            if (!inData.cdfeEnemyData.HasComponent(target)) return;
//            if (!refData.cdfeBuffs.HasBuffer(target)) return;
//            var buff = refData.cdfeBuffs[target];
//            for (int i = 0; i < buff.Length; i++)
//            {
//                if (buff[i].Entity_9 == inData.selefEntity)
//                {
//                    buff.RemoveAt(i);
//                    break;
//                }
//            }
//        }
//    }
//}

