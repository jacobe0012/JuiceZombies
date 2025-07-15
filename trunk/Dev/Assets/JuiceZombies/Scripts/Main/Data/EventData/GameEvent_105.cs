using System.Threading;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR;

namespace Main
{
    /// <summary>
    ///环境事件 极光
    /// </summary>
    public partial struct GameEvent_105 : IGameEvent
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

        private void ChangePropertyForAll(ref GameEventData_ReadWrite refData, GameEventData_ReadOnly inData)
        {
            // UnityHelper.ChangeProperty(ref refData.ecb, inData.sortkey, ref refData.cdfeChaStats,
            //          ref refData.cdfePlayerData, refData.cdfeLocalTransform, args123.y, skillCount * args123.x,inData.selefEntity);
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
            int index = -1;
            ref var environmentConfig = ref inData.config.value.Value.configTbenvironments.configTbenvironments;
            for (int i = 0; i < environmentConfig.Length; i++)
            {
                if (environmentConfig[i].id == id)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0)
            {
                args123.x = (int)(environmentConfig[index].para[0] / 1000f);
                args123.y = (int)(environmentConfig[index].para[1] / 1000f);
                isRandomEvent = true;
            }
        }

        public void OnTick0(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }


        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }


        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            var timeData = inData.gameTimeData.refreshTime;
            if (timeData.gameTimeScale < math.EPSILON)
                return;
            int index = -1;
            ref var environmentConfig = ref inData.config.value.Value.configTbenvironments.configTbenvironments;
            for (int i = 0; i < environmentConfig.Length; i++)
            {
                if (environmentConfig[i].id == id)
                {
                    index = i;
                    break;
                }
            }

            ref var envio = ref environmentConfig[index];
            if (timeData.tick > 0 &&
                timeData.tick % (int)(envio.para[0] / 1000f / timeData.gameTimeScale / inData.fDT) == 0)
            {
                Debug.Log($"极光生成怪物 ");

                bool isinBoss = refData.cdfePlayerData[inData.player].playerOtherData.isBossFight;
                float bossScenSize= refData.cdfePlayerData[inData.player].playerOtherData.bossScenePos.y;
                BuffHelper.GenerateItemAndAddData(inData.cdfePostTransformMatrix, inData.player, ref refData.ecb, inData.cdfeLocalTransform, inData.cdfeWeaponData,
                    refData.cdfeChaStats, refData.cdfePhysicsMass,
                    inData.cdfeAgentLocomotion, inData.cdfeEnemyData,
                    inData.config,
                    inData.prefabMapData,
                    inData.gameTimeData, inData.gameOthersData, inData.sortKey,
                    2, envio.para[4], envio.para[3], inData.cdfeLocalTransform[inData.player].Position,0,isinBoss, bossScenSize);
            }
        }
    }
}