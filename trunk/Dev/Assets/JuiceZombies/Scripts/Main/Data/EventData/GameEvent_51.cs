using Unity.Mathematics;

namespace Main
{
    /// <summary>
    ///消灭角色周围指定类型实体
    /// </summary>
    public partial struct GameEvent_51 : IGameEvent
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

        public void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
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
            //refData.ecb.AppendToBuffer<Skill>(inData.sortKey, inData.selefEntity, new Skill { CurrentTypeId = (Skill.TypeId)args123.x });

            DestroyArrowd(ref refData, inData);
        }

        private readonly void DestroyArrowd(ref GameEventData_ReadWrite refData, GameEventData_ReadOnly inData)
        {
            for (int i = 0; i < inData.entities.Length; i++)
            {
                var target = inData.entities[i];
                var dis = math.distance(inData.cdfeLocalTransform[inData.selefEntity].Position,
                    inData.cdfeLocalTransform[target].Position);
                if (dis <= args123.x / 1000f && ((int)inData.cdfeTargetData[target].BelongsTo & args123.y) != 0)
                {
                    if (refData.randomData.rand.NextInt(0, 10000) <= args123.z)
                    {
                        refData.ecb.AppendToBuffer(inData.sortKey, inData.wbe, new DamageInfo
                        {
                            attacker = inData.selefEntity,
                            defender = target,
                            tags = new DamageInfoTag
                            {
                                seckillDamage = true,
                            },
                            damage = new Damage
                            {
                            },
                            criticalRate = 0,
                            criticalDamage = 0,
                            hitRate = 1,
                            degree = 0,
                            pos = default,
                            bulletEntity = default
                        });
                    }
                }
            }
        }

        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            DestroyArrowd(ref refData, inData);
        }

        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
            DestroyArrowd(ref refData, inData);
        }


        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }

        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData)
        {
        }
    }
}