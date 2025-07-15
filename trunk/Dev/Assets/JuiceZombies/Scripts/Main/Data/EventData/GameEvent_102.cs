using System.Data;
using System.Threading;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR;

namespace Main
{
    /// <summary>
    ///环境事件 雨
    /// </summary>
    public partial struct GameEvent_102 : IGameEvent
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

        private int cooddownReduce;
        private int pushRations;
        private int speedRations;
        private int origCoodown;


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
            isEnvEvent = true;

            ref var environmentConfig = ref inData.config.value.Value.configTbenvironments;

            triggerGap = environmentConfig.Get(id).para[0] / 1000f;
            maxLimit = 999999;
            cooddownReduce = environmentConfig.Get(id).para[3];
            pushRations=environmentConfig.Get(id).bossPara[1];
            speedRations = environmentConfig.Get(id).bossPara[0];

            ref var scenModuleConfig = ref inData.config.value.Value.configTbscene_modules;
            origCoodown = scenModuleConfig.Get(1001).duration;
           
        }

        public void OnTick0(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }


        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            if (refData.cdfeEnviromentData[inData.wbe].env.weather == id)
            {
                Debug.Log($"onPerGap,weather:{id}");
                var playerPos = inData.cdfeLocalTransform[inData.player].Position;
                var mapInfo = inData.gameOthersData.mapData.mapSize;
                var smallRect = new MathHelper.Rectangle
                {
                    center = playerPos.xy,
                    width = inData.gameCameraSizeData.width,
                    height = inData.gameCameraSizeData.height
                };
                var mapRectangle = new MathHelper.Rectangle
                {
                    center = float2.zero,
                    width = math.abs(mapInfo.x),
                    height = math.abs(mapInfo.y)
                };
                for (int i = 0; i < inData.entities.Length; i++)
                {
                    if (inData.cdfeMapElementData.HasComponent(inData.entities[i]))
                    {
                        if (inData.cdfeMapElementData[inData.entities[i]].elementID == 1002)
                        {
                            var centerPos = new float2(inData.cdfeLocalTransform[inData.entities[i]].Position.x,
                                inData.cdfeLocalTransform[inData.entities[i]].Position.y);
                            //var rect= new MathHelper.Rectangle { center =centerPos , width = sceneModuleConfig[index].size[0], height = sceneModuleConfig[index].size[1] };
                            if (smallRect.Contains(centerPos))
                            {
                                Debug.LogError($"smallRect.Contains(centerPos)");
                                return;
                            }
                        }
                    }
                }

                float2 point = smallRect.center;
                var rand = Unity.Mathematics.Random.CreateFromIndex((uint)durationTick);
                var num = rand.NextInt(1, 4);
                Debug.Log($"num:{num}");
                for (int i = 0; i < num; i++)
                {
                    point.x = rand.NextFloat((smallRect.center.x - smallRect.width / 2f),
                        (smallRect.center.x + smallRect.width / 2f));
                    point.y = rand.NextFloat((smallRect.center.y - smallRect.height / 2f),
                        (smallRect.center.y + smallRect.height / 2f));


                    //Debug.Log($"point {point}")
                    Entity ins = default;

                    bool isinBoss = refData.cdfePlayerData[inData.player].playerOtherData.isBossFight;
                    float bossScenSize = refData.cdfePlayerData[inData.player].playerOtherData.bossScenePos.y;
                    BuffHelper.GenerateMapModule(inData.cdfePostTransformMatrix,
                        inData.cdfeLocalTransform[inData.player].Position, inData.gameOthersData, ref refData.ecb,
                        inData.config, inData.prefabMapData, inData.sortKey, 1002, new float3(point.xy, 0), ref ins,
                        isinBoss,bossScenSize);
                    Debug.Log($"GenerateMapModule");
                }
            }
        }


        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            for (int i = 0; i < inData.entities.Length; i++)
            {
                var entity = inData.entities[i];
                if (inData.cdfeMapElementData.HasComponent(entity) && inData.cdfeTimeToDieData.HasComponent(entity))
                {
                    if (inData.cdfeMapElementData[entity].elementID == 1001 &&
                        !inData.cdfeMapElementData[entity].isActiveWeather)
                    {
                        refData.ecb.AddComponent(inData.sortKey, entity,
                            new MapElementData { elementID = 201, isActiveWeather = true });
                        var duration = inData.cdfeTimeToDieData[entity].duration;
                        var usedDuration = origCoodown / 1000f - duration;
                        var trueDutation = origCoodown / 1000f * cooddownReduce / 10000f;
                        if (usedDuration >= trueDutation)
                        {
                            refData.ecb.AddComponent(inData.sortKey, entity, new TimeToDieData { duration = 0.02f });
                        }
                        else
                        {
                            var ramin = trueDutation - usedDuration;
                            refData.ecb.AddComponent(inData.sortKey, entity, new TimeToDieData { duration = ramin });
                        }
                    }
                }
            }
            if (refData.cdfePlayerData[inData.player].playerOtherData.isBossFight)
            {
                for (int i = 0; i < inData.entities.Length; i++)
                {
                    var entity = inData.entities[i];
                    var buffs = refData.cdfeBuffs[entity];
                    var isNeedAdd = true;
                    for (int j = 0; j < buffs.Length; j++)
                    {
                        if (buffs[i].CurrentTypeId == Buff.TypeId.Buff_333102)
                        {
                            isNeedAdd = false;
                            break;
                        }
                    }
                    if (isNeedAdd)
                    {
                        UnityHelper.ChangeProperty(ref refData.ecb, inData.sortKey, ref refData.cdfeChaStats,
                                         ref refData.cdfePlayerData, ref refData.cdfePhysicsMass, inData.cdfeLocalTransform,
                                         209020, pushRations, entity);
                        UnityHelper.ChangeProperty(ref refData.ecb, inData.sortKey, ref refData.cdfeChaStats,
                                        ref refData.cdfePlayerData, ref refData.cdfePhysicsMass, inData.cdfeLocalTransform,
                                        207020, speedRations, entity);

                    }
                }
            }
             
        }
    }
}