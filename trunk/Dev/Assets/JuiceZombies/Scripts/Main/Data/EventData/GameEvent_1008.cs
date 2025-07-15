// using ProjectDawn.Navigation;
// using System.Threading;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace Main
// {
//     /// <summary>
//     /// 生成道具
//     /// </summary>
//     public partial struct GameEvent_1008 : IGameEvent
//     {
//         /// <summary>
//         ///事件id 0
//         /// </summary>
//         public int id;
//
//         /// <summary>
//         /// 触发事件类型 1
//         /// </summary>
//         public int triggerType;
//
//         /// <summary>
//         /// 执行事件类型 2
//         /// </summary>
//         public int eventType;
//
//         /// <summary>
//         /// 触发间隔时间 3
//         /// 当触发类型为 间隔触发(7)时用该参数
//         /// </summary>
//         public float triggerGap;
//
//         /// <summary>
//         /// 剩余执行时间 4
//         /// </summary>
//         public float remainTime;
//
//         /// <summary>
//         /// 已经执行时间 5
//         /// </summary>
//         public float duration;
//
//         /// <summary>
//         /// 是否永久事件 6
//         /// 永久存在
//         /// </summary>
//         public bool isPermanent;
//
//         /// <summary>
//         /// 目标 7
//         /// </summary>
//         public Entity target;
//
//         /// <summary>
//         /// 单次触发事件点 8
//         /// </summary>
//         public float onceTime;
//
//         /// <summary>
//         /// 用于触发障碍物靠近等机制 9
//         /// </summary>
//         public int colliderScale;
//
//         /// <summary>
//         /// 触发延迟
//         /// </summary>
//         public float delayTime;
//
//         private float3 targetPos;
//
//         private bool isSkillCast;
//         private int skillID;
//
//         public void OnBeDie(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//         {
//         }
//
//         public void OnBeHurt(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//         {
//         }
//
//         public void OnCharacterEnter(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//         {
//         }
//
//         public void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//         {
//         }
//
//
//         public void OnCollider(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//         {
//         }
//
//
//         public void OnEventRemove(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//         {
//         }
//
//
//         public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//         {
//             targetPos = new float3(0, 0, 0);
//             int args0 = 0, args1 = 0, args3 = 0;
//             ref var eventsMap = ref inData.config.value.Value.configTbevent_0s.configTbevent_0s;
//             for (int i = 0; i < eventsMap.Length; i++)
//             {
//                 if (eventsMap[i].id == id)
//                 {
//                     args0 = eventsMap[i].para1[0];
//                     args1 = eventsMap[i].para2[0];
//                     args3 = eventsMap[i].para3[0];
//                     break;
//                 }
//             }
//
//
//             float3 elemetPos = inData.cdfeLocalTransform[inData.selefEntity].Position;
//             switch (args0)
//             {
//                 case 0:
//                     break;
//                 case 10001:
//                     targetPos = inData.currentMapStartPos;
//                     break;
//                 case 10242048:
//                     targetPos = new float3(inData.currentMapStartPos.x + 1024, inData.currentMapStartPos.y - 2048, 0);
//                     break;
//                 case 1100010001:
//                     targetPos = new float3(elemetPos.x + 1, elemetPos.y + 1, 0);
//                     break;
//                 case 1210242048:
//                     targetPos = new float3(elemetPos.x + 1024, elemetPos.y - 2048, 0);
//                     break;
//             }
//
//             isSkillCast = false;
//             if (args1 == 4)
//             {
//                 isSkillCast = true;
//                 //TODO:每隔一段时间生成技能应该是每隔一段时间释放技能(实体)
//                 skillID = args3;
//             }
//         }
//
//         public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//         {
//         }
//
//         public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//         {
//             //Debug.Log($"OnPerGap-{propName}-{duration}");
//             if (!isSkillCast)
//             {
//                 var propEntity = BuffHelper.AddDataToProps(ref refData.ecb, id, inData.config, inData.prefabMapData,
//                     inData.sortKey, inData.currentMapIndex);
//                 var newLoc = new LocalTransform
//                 {
//                     Position = targetPos,
//                     Scale = inData.cdfeLocalTransform[propEntity].Scale,
//                     Rotation = inData.cdfeLocalTransform[propEntity].Rotation,
//                 };
//                 refData.ecb.SetComponent(inData.sortKey, propEntity, newLoc);
//             }
//             else
//             {
//                 if (!refData.cdfeSkills.HasBuffer(inData.selefEntity))
//                 {
//                     refData.ecb.AddBuffer<Skill>(inData.sortKey, inData.selefEntity);
//                 }
//
//                 BuffHelper.AddOnceCastSkill(ref refData.ecb, skillID, inData.selefEntity, inData.sortKey);
//             }
//         }
//
//
//         public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
//         {
//         }
//     }
// }

