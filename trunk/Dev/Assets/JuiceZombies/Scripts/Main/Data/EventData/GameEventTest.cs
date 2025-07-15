//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using UnityEngine;

//namespace Main
//{
//    public partial struct GameEventTest : IGameEvent
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
//      /// <summary>
//        /// 全局事件 开始时就一直存在 一开始执行 只执行一次
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//      {

//      }
//        /// <summary>
//        /// 事件被销毁时
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        public void OnEventRemove(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            ;
//        }


//        /// <summary>
//        /// 每多少秒执行一次
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            ;
//        }


//        /// <summary>
//        /// 单次执行
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            ;
//        }


//        /// <summary>
//        /// 进入时触发 只触发一次
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//       public void OnCharacterEnter(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            ;
//        }


//        /// <summary>
//        /// 离开地形 单次触发
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        public  void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            ;
//        }

//        /// <summary>
//        /// 碰撞触发 colliderScale<碰撞盒半径时表示碰撞 否则为靠近
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        public void OnCollider(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            ;
//        }

//        /// <summary>
//        /// 死亡触发
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        public void OnBeDie(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            ;
//        }

//        /// <summary>
//        /// 受伤后触发
//        /// </summary>
//        /// <param name="refData"></param>
//        /// <param name="inData"></param>
//        public void OnBeHurt(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            ;
//        }


//        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            ;
//        }
//    }
//}

