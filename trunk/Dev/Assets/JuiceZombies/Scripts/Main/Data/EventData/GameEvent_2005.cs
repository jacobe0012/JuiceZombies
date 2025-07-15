//using cfg.config;
//using System;
//using System.Linq;
//using System.Threading;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    /// <summary>
//    ///击退时造成伤害
//    /// </summary>
//    public partial struct GameEvent_2005 : IGameEvent
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


//        private void SetColliderTarget(ref GameEventData_ReadWrite refData, Entity ins, uint target,
//            in GameEventData_ReadOnly indata)
//        {
//            // var physicsCollider = refData.cdfePhysicsCollider[ins];
//            // var collider = PhysicsHelper.CreateColliderWithCollidesWith(physicsCollider, target);
//            //
//            //
//            // refData.ecb.SetComponent(indata.sortKey, ins, collider);
//        }

//        /// <summary>
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
//            Debug.Log("OnBeHurt");
//            int skillID = 0;
//            ref var eventsMap = ref inData.config.value.Value.configTbevent_0s.configTbevent_0s;
//            for (int i = 0; i < eventsMap.Length; i++)
//            {
//                if (eventsMap[i].id == id)
//                {
//                    skillID = eventsMap[i].para3[0];
//                    break;
//                }
//            }


//            refData.ecb.AddBuffer<Skill>(inData.sortKey, inData.selefEntity);
//            BuffHelper.AddOnceCastSkill(ref refData.ecb, 100101, inData.selefEntity, inData.sortKey);
//            //var prefab = inData.prefabMapData.prefabHashMap["CircleSkillAttack"];
//            ////SetColliderTarget(ref refData, prefab, target, in inData);
//            //var ins = refData.ecb.Instantiate(inData.sortKey, prefab);
//            //var newpos = new LocalTransform
//            //{
//            //    Position = inData.cdfeLocalTransform[inData.selefEntity].Position,
//            //    Scale = (args1 + 20) * inData.cdfeLocalTransform[prefab].Scale,
//            //    Rotation = inData.cdfeLocalTransform[prefab].Rotation
//            //};
//            //refData.ecb.SetComponent(inData.sortKey, ins, newpos);


//            //refData.ecb.AddComponent(inData.sortKey, ins,
//            //    new SkillAttackData
//            //    {
//            //        data = new SkillAttack_0
//            //        {
//            //            duration = 0.02f,
//            //            tick = 0,
//            //            caster = inData.player
//            //        }.ToSkillAttack()
//            //    });

//            //refData.ecb.AppendToBuffer(inData.sortKey, ins, new SkillHitEffectBuffer
//            //{
//            //    buffID = 40000003,
//            //    buffArgs = new float3x4
//            //    {
//            //        c0 = new float3(args0 * 10000, 0, 0),
//            //        c1 = default,
//            //        c2 = default,
//            //        c3 = new float3(63, 0, 0)
//            //    }
//            //});
//            //refData.ecb.AppendToBuffer(inData.sortKey, ins, new SkillHitEffectBuffer
//            //{
//            //    buffID = 30000003,
//            //    buffArgs = new float3x4
//            //    {
//            //        c0 = new float3(5000, 0, 0),
//            //        c1 = default,
//            //        c2 = default,
//            //        c3 = new float3(63, 0, 0)
//            //    }
//            //});
//        }


//        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//        {
//            ;
//        }
//    }
//}

