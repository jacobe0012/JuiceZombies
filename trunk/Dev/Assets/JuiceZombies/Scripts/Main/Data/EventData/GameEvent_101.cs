using System.Threading;
using Unity.Mathematics;

namespace Main
{
    /// <summary>
    ///环境事件 风
    /// </summary>
    public partial struct GameEvent_101 : IGameEvent
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

        public int lastTime;

        private int pushRations;
        private float windSpeedRations;
        private float3 windDir;

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
            id = 101;
            isPermanent = true;

            int index = -1;

            ref var environmentConfig = ref inData.config.value.Value.configTbenvironments;
            pushRations=environmentConfig.Get(id).para[0];
            windSpeedRations = environmentConfig.Get(id).bossPara[0] / 1000f;
            var rand = Random.CreateFromIndex(inData.gameRandomData.seed);
            windDir =rand.NextInt(1,2)==1?new float3(1,-1,0):new float3(-1,-1,0);
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
            if (refData.cdfeEnviromentData[inData.wbe].env.weather == id)
            {
                var entities = inData.entities;
                for (int i = 0; i < entities.Length; i++)
                {
                    var entity = entities[i];
                    if (inData.cdfeEnemyData.HasComponent(entity) || refData.cdfePlayerData.HasComponent(entity))
                    {
                       
                        if (refData.cdfePlayerData[inData.player].playerOtherData.isBossFight)
                        {
                            if (refData.cdfePhysicsVelocity.HasComponent(entity))
                            {
                               var temp= refData.cdfePhysicsVelocity[entity];
                                temp.Linear += windSpeedRations * windDir;
                                refData.cdfePhysicsVelocity[entity]=temp;
                            }
                        }
                        else
                        {
                            if (refData.cdfeBuffs.HasBuffer(entity))
                            {
                                var buffs = refData.cdfeBuffs[entity];
                                var isNeedAdd = true;
                                for (int j = 0; j < buffs.Length; j++)
                                {
                                    if (buffs[j].CurrentTypeId == Buff.TypeId.Buff_333101)
                                    {
                                        isNeedAdd = false;
                                        break;
                                    }
                                }

                                if (isNeedAdd)
                                {
                                    refData.ecb.AppendToBuffer<Buff>(inData.sortKey, entity,
                                        new Buff_333101 { carrier = entity, permanent = true }.ToBuff());
                                    UnityHelper.ChangeProperty(ref refData.ecb, inData.sortKey, ref refData.cdfeChaStats,
                                        ref refData.cdfePlayerData, ref refData.cdfePhysicsMass, inData.cdfeLocalTransform,
                                        209020, pushRations, entity);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}