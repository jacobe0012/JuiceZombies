using System.Threading;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.XR;

namespace Main
{
    /// <summary>
    ///环境事件 雷
    /// </summary>
    public partial struct GameEvent_104 : IGameEvent
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

        private void  ChangePropertyForAll(ref GameEventData_ReadWrite refData, GameEventData_ReadOnly inData)
        {
         
            //UnityHelper.ChangeProperty(ref refData.ecb, inData.sortkey, ref refData.cdfeChaStats,
                    //ref refData.cdfePlayerData, refData.cdfeLocalTransform, args123.y, skillCount * args123.x,inData.selefEntity);
        }

        public void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnCollider(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }


        public void OnEventRemove(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            //UnityHelper.RemoveChangeProperty(ref refData.cdfeChaStats, ref refData.cdfePlayerData, args123.y, skillCount * args123.x, inData.selefEntity);
        }


        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {

            ref var environmentConfig = ref inData.config.value.Value.configTbenvironments;
            Debug.LogError("OnOccur 104ddddddddddddddddd");
            args123.x = (int)(environmentConfig.Get(id).para[0] / 1000f);
            args123.y = (int)(environmentConfig.Get(id).para[1] / 1000f);
            isRandomEvent = true;

            triggerGap = environmentConfig.Get(id).bossPara[0] / 1000f;
        }

        public void OnTick0(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {

        }


        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
           
        }

        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {


            if (refData.cdfePlayerData[inData.player].playerOtherData.isBossFight)
            {
                ref var environmentConfig = ref inData.config.value.Value.configTbenvironments;
                var buffDuration = environmentConfig.Get(id).bossPara[1] / 1000f+ environmentConfig.Get(id).bossPara[1] / 1000f;
                for (int i = 0; i < inData.entities.Length; i++)
                {
                    var entity = inData.entities[i];
                    if (!refData.cdfeBuffs.HasBuffer(entity) || !refData.cdfeChaStats.HasComponent(entity))
                    {
                        continue;
                    }
                    var buffs = refData.cdfeBuffs[entity];
                    var isNeedAdd = true;
                    //for (int j = 0; j < buffs.Length; j++)
                    //{
                    //    if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_333104)
                    //    {
                    //        isNeedAdd = false;
                    //        break;
                    //    }
                    //}
                    if (isNeedAdd)
                    {
                       refData.ecb.AppendToBuffer(inData.sortKey,entity,new Buff_333104 { duration = buffDuration, carrier = entity, argsNew = new BuffArgsNew { args1 = new int4(environmentConfig.Get(id).bossPara[1] / 1000, environmentConfig.Get(id).bossPara[3] / 1000, environmentConfig.Get(id).bossPara[2], 0) } }.ToBuff() );

                    }
                }
            }
        }


        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            if (refData.cdfeEnviromentData[inData.wbe].env.weather == id)
            {
                Debug.Log($"OnRandom 0000000000");
                ref var environmentConfig = ref inData.config.value.Value.configTbenvironments;
               
                var rand = Unity.Mathematics.Random.CreateFromIndex((uint)durationTick);
                var count = rand.NextInt(1, 4);
                for (int i = 0; i < count; i++)
                {

                    rand = Unity.Mathematics.Random.CreateFromIndex((uint)(durationTick + count * 10));
                    var skillIndex = rand.NextInt(2, 5);
                    var skillID = environmentConfig.Get(id).para[skillIndex];
                    Debug.Log($"OnRandom 222222222:,id:{skillID}");
                    BuffHelper.AddOnceCastSkill(ref refData.ecb, skillID, inData.player, inData.sortKey);
                }

            }
        }

        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
         
        }


        }
    
}