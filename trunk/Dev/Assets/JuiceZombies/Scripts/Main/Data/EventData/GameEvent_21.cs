using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    /// <summary>
    ///事件类型 21 ：触发后 以作用方中心的圆环内随机位置生成
    /// </summary>
    public partial struct GameEvent_21 : IGameEvent
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
            GenerateItem(ref refData, inData);
        }

        private void GenerateItem(ref GameEventData_ReadWrite refData, GameEventData_ReadOnly inData)
        {
            bool isinBoss = refData.cdfePlayerData[inData.player].playerOtherData.isBossFight;
            float bossSceneSize = refData.cdfePlayerData[inData.player].playerOtherData.bossScenePos.y;

            Debug.LogError($"GenerateItem");
            for (int i = 0; i < args456.y; i++)
            {
                int maxTimes = 1;
                float3 pos = default;
                do
                {
                    pos = BuffHelper.GetRandomPointInCircle(inData.cdfeLocalTransform[inData.selefEntity].Position,
                         args123.z / 1000f,args456.x / 1000f,inData.gameRandomData.seed);
                    maxTimes++;
                    if (maxTimes > 50)
                    {
                        return;
                    }
                } while (!BuffHelper.IsPosCanUse(pos, inData.config, inData.mapModels,
                             inData.cdfeMapElementData, inData.cdfeLocalTransform));
                if (args456.z == 0f)
                {
                    args456.z = MathHelper.MaxNum;
                }
                BuffHelper.GenerateItemAndAddData(inData.cdfePostTransformMatrix, inData.player, ref refData.ecb, inData.cdfeLocalTransform, inData.cdfeWeaponData,
                    refData.cdfeChaStats, refData.cdfePhysicsMass,
                    inData.cdfeAgentLocomotion, inData.cdfeEnemyData,
                    inData.config,
                    inData.prefabMapData,
                    inData.gameTimeData, inData.gameOthersData, inData.sortKey,
                    args123.x, args123.y, 0, pos, args456.z,isinBoss, bossSceneSize);
            }
        }

        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            GenerateItem(ref refData, inData);
        }

        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            GenerateItem(ref refData, inData);
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