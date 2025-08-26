//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-17 12:15:10
//---------------------------------------------------------------------

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Main
{
    //范围间隔伤害   状态
    //new 
    public partial struct Buff_128 : IBuff
    {
        ///<summary>0
        ///buff的id
        ///</summary>
        public int id;

        ///<summary>1
        ///buff添加时间点/刻 替换优先级
        ///</summary>
        public float startTime;

        ///<summary>2
        ///buff已经存在了多少时间了，单位：帧
        ///</summary>
        public int timeElapsed;

        ///<summary>3
        ///剩余多久，单位：秒
        ///</summary>
        public float duration;

        ///<summary>4
        ///是否是一个永久的buff，永久的duration不会减少，但是timeElapsed还会增加
        ///</summary>
        public bool permanent;

        ///<summary>5
        ///buff的施法者是谁，当然可以是空的
        ///</summary>
        public Entity caster;

        ///<summary>6
        ///buff的携带者，实际上是作为参数传递给脚本用，具体是谁，可定是所在控件的this.gameObject了
        ///</summary>
        public Entity carrier;

        /// <summary>7
        /// 替换类型 用于同类buff的替换 仅用于免疫和控制和属性变更 为属性变更时 x是类型 y是上限
        /// </summary>
        public int2 changeType;

        /// <summary>8
        /// 是否能够清除 用于不同类buff的替换 仅用于控制类
        /// new字段
        /// </summary>
        public int canClear;

        /// <summary>9
        /// 护盾层数 仅作用于免疫
        /// new
        /// </summary>
        public int immuneStack;

        /// <summary>10
        /// 从 effect传入的的方向
        /// new
        /// </summary>
        public float3 direction;

        /// <summary>11
        /// buff的状态类型 分无状态 正面状态和负面状态
        /// </summary>
        public int buffState;

        /// <summary>12
        /// float4-1:output 2:bonus_type 3:calc_type and pra
        /// </summary>
        public BuffArgsNew argsNew;

        ///<summary>13
        ///buff的优先级，优先级越低的buff越后面执行，这是一个非常重要的属性
        ///比如经典的“吸收50点伤害”和“受到的伤害100%反弹给攻击者”应该反弹多少，取决于这两个buff的priority谁更高
        ///</summary>
        public int priority;

        /// <summary>14
        /// 元素的概率 
        /// </summary>
        public int power;

        /// <summary>15
        /// 用来处理ontick的逻辑 =0时不执行ontick 
        /// </summary>
        public int tickTime;

        /// <summary>16
        /// 用来处理移动伤害的逻辑  第一个float3的x为每移动多少米 y为当前移动总距离 z为1时该移动伤害才生效！！！！
        /// 第二个记录上帧的位置 
        /// </summary>
        public float3x2 distancePar;

        /// <summary>17
        /// 是否是弹幕
        /// </summary>
        public bool isBullet;

        /// <summary>
        ///  18 buff2里上一份属性变更的值
        /// </summary>
        public int oldValue;

        /// <summary>
        /// 19 是否禁用特效
        /// </summary>
        public bool isDisableSpecial;

        /// <summary>
        /// 20 技能id
        /// </summary>
        public int skillId;


        public float atkRange;

        /// <summary>
        /// 特效原本scale
        /// </summary>
        public int oldScale;

        public void OnOccur(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            ref var battleConstConfig = ref inData.globalConfigData.value.Value.configTbbattle_constants.configTbbattle_constants;
            for (int i = 0; i < battleConstConfig.Length; i++)
            {
                if (battleConstConfig[i].constantName == (FixedString128Bytes)"status_128_damaged_rate")
                {
                    //
                    atkRange = battleConstConfig[i].constantValue / 1000f;
                    break;
                }
            }
            InitPar(ref refData, inData);



            if (UnityHelper.TrySpawnSpecialEffect(ref refData.ecb, inData.linkedEntityGroup, inData.sortkey,
                  inData.cdfeSpecialEffectData, inData.globalConfigData, id, out int specialEffectId,
                  out int curGroup, out int curSort))
            {
                var ins = UnityHelper.SpawnBuffSpecialEffect(ref refData.ecb, inData.sortkey, inData.eT,
                    inData.cdfeLocalTransform,
                    inData.gameTimeData,
                    inData.globalConfigData, specialEffectId, inData.prefabMapData, carrier);
                 if (ins != Entity.Null)
                {
                    refData.ecb.SetComponent(inData.sortkey, ins, new SpecialEffectData
                    {
                        id = specialEffectId,
                        stateId = id,
                        groupId = curGroup,
                        sort = curSort
                    });
                }
            }
           

            ref var effectConfig = ref inData.globalConfigData.value.Value.configTbspecial_effects.configTbspecial_effects;
            ref var stateConfig = ref inData.globalConfigData.value.Value.configTbbattle_statuss.configTbbattle_statuss;
            int effectID = 0;
            for (int i = 0; i < stateConfig.Length; i++)
            {
                if (stateConfig[i].id == id)
                {
                    effectID = stateConfig[i].specialEffects;
                    break;
                }
            }
            for (int i = 0; i < effectConfig.Length; i++)
            {
                if (effectConfig[i].id == effectID)
                {
                    oldScale = effectConfig[i].sizeX > effectConfig[i].sizeY ? effectConfig[i].sizeX : effectConfig[i].sizeY;
                    break;
                }
            }
        }

        public void OnRemoved(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }


        private void InitPar(ref BuffData_ReadWrite refData, BuffData_ReadOnly inData)
        {
            ref var battleStatesConfig =
                ref inData.globalConfigData.value.Value.configTbbattle_statuss.configTbbattle_statuss;
            ref var battleConstConfig =
                ref inData.globalConfigData.value.Value.configTbbattle_constants.configTbbattle_constants;
            for (int i = 0; i < battleStatesConfig.Length; i++)
            {
                if (battleStatesConfig[i].id == id)
                {
                    //dot伤害万分比
                    argsNew.args1.x = battleStatesConfig[i].atk;
                    //ticktime
                    tickTime = (int)((battleStatesConfig[i].atkRate / 1000f) / inData.fdT);
                    break;
                }
            }

            for (int i = 0; i < battleConstConfig.Length; i++)
            {
                if (battleConstConfig[i].constantName == (FixedString128Bytes)"status_move_damage_distance")
                {
                    //移动伤害的移动值
                    distancePar.c0.x = battleConstConfig[i].constantValue / 1000f;
                    break;
                }
            }
        }
        public void OnUpdate(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
         
        }

        public void OnTick(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            UnityEngine.Debug.LogError($"128 OnTick{timeElapsed}  {tickTime}");
            Entity effectEntity = default;
            for (int i = 0; i < inData.linkedEntityGroup.Length; i++)
            {
                var entity = inData.linkedEntityGroup[i].Value;
                if (inData.cdfeSpecialEffectData.HasComponent(entity))
                {
                    var stateID = inData.cdfeSpecialEffectData[entity].stateId;
                    if (stateID == id)
                    {
                        effectEntity = entity;
                        break;
                    }
                }
            }
          
            for(int i = 0;i<inData.entities.Length; i++)
            {

                var entity = inData.entities[i];
                if(!refData.cdfeEnemyData.HasComponent(entity)) continue;
                //var denfenderTarget = inData.cdfeTargetData[entity];
                //var attackerTarget = inData. cdfeTargetData[carrier];
                
                //if (!PhysicsHelper.IsTargetEnabled(attackerTarget, denfenderTarget))
                //{
                //    UnityEngine. Debug.Log($"333333333333333333333333");
                //    continue;
                //}
                float dis = math.distance(refData.cdfeLocalTransform[entity].Position, refData.cdfeLocalTransform[carrier].Position);
                UnityEngine.Debug.Log($"dis:{dis },range:{atkRange}");
                if (dis>atkRange) continue;

                #region 加伤害
                Entity attaker = default;
                var damage = new Damage
                {
                    normal = 0,
                };
                var tags = new DamageInfoTag
                {
                };


                if (inData.cdfeMapElementData.HasComponent(caster))
                {
                    tags.environmentDamage = true;
                    damage.normal = (int)math.floor(refData.cdfeChaStats[carrier]
                                                        .chaProperty.maxHp *
                                                    argsNew.args1.x /
                                                    10000f);
                }
                else
                {
                    tags.directDamage = true;
                    damage.normal = (int)math.floor(refData.cdfeChaStats[carrier]
                                                        .chaProperty.atk *
                                                    argsNew.args1.x /
                                                    10000f);
                    attaker = carrier;
                }

             
                refData.ecb.AppendToBuffer(inData.sortkey, inData.wbe, new DamageInfo
                {
                    attacker = attaker,
                    defender = entity,
                    tags = tags,
                    damage = damage,
                    criticalRate = 0,
                    criticalDamage = 0,
                    hitRate = 1,
                    degree = 0,
                    pos = default,
                    bulletEntity = default,
                });
                #endregion


            }


        }


        public void OnHit(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeHurt(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }


       public void OnKill(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeforeBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnPerUnitMove(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }
    }
}