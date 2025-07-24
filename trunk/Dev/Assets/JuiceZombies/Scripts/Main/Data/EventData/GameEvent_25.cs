using Unity.Entities;
using Unity.Mathematics;

namespace Main
{
    /// <summary>
    ///事件类型 25 ：执行模版刷新
    /// </summary>
    public partial struct GameEvent_25 : IGameEvent
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
            RefreshModule(ref refData, inData);
        }


        private void RefreshModule(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            int mapType = inData.gameOthersData.mapData.mapType;
            Entity bossNow = Entity.Null;
            float mapHeight, mapWidth;
            bool isinBoss = refData.cdfePlayerData[inData.player].playerOtherData.isBossFight;
            if (isinBoss)
            {
                mapWidth = mapHeight = refData.cdfePlayerData[inData.player].playerOtherData.bossScenePos.y;
            }
            else
            {
                mapHeight = inData.gameOthersData.mapData.mapSize.y;
                mapWidth = inData.gameOthersData.mapData.mapSize.x;
            }

            int eventIndex = -1;
            ref var eventsconfig = ref inData.config.value.Value.configTbevent_0s.configTbevent_0s;
            for (int i = 0; i < eventsconfig.Length; i++)
            {
                if (eventsconfig[i].id == id)
                {
                    eventIndex = i;
                    break;
                }
            }

            if (eventIndex == -1) return;

            //TODO:这两个值跟地图有关
            float3 startPos = float3.zero;
            float3 mapCenterPos = float3.zero;
            if (isinBoss)
            {
                bossNow = inData.gameOthersData.BossEntity;
                switch (mapType)
                {
                    case 1:
                        mapCenterPos = new float3(1999, 0, 0);
                        break;
                    case 2:
                        mapCenterPos = new float3(0, 1999, 0);
                        break;
                    case 3:
                        mapCenterPos = new float3(1999, 0, 0);

                        break;
                    case 4:
                        //TODO:全开放的boss场景选点
                        mapCenterPos = new float3(1999, 1999, 0);
                        break;
                }

                startPos = new float3(mapCenterPos.x - mapWidth / 2f, mapCenterPos.y + mapHeight / 2f, 0);
            }
            else
            {
                switch (mapType)
                {
                    case 1:
                        var curMapIndex = (int)((inData.cdfeLocalTransform[inData.player].Position.y + mapWidth * 3) /
                                                mapHeight) + 1;
                        startPos = new float3(-mapWidth / 2f, -(mapWidth * 3) + mapHeight * curMapIndex, 0);
                        mapCenterPos = new float3(startPos.x + mapWidth / 2f, startPos.y - mapHeight / 2f, 0);
                        break;
                    case 3:
                        //startPos = new float3(-mapWidth / 2f, mapHeight / 2f, 0);
                        break;
                    case 4:
                        //还没想好
                        break;
                }
            }

            //该事件执行刷新模版在boss场景的时候 地形和障碍物不会销毁
            BuffHelper.GenerateMapElement(inData.mapModels, inData.cdfeMapElementData, inData.cdfeLocalTransform,
                inData.gameRandomData.seed, inData.cdfePostTransformMatrix, inData.config, args123.x,
                ref eventsconfig[eventIndex].paraList, mapCenterPos, inData.prefabMapData, refData.ecb, inData.sortKey,
                bossNow, mapType, mapWidth, mapHeight, true, isinBoss, inData.gameRandomData.seed);
        }


        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            RefreshModule(ref refData, inData);
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