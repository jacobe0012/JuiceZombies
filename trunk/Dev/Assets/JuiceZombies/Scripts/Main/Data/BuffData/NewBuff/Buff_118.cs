//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-07-17 12:15:10
//---------------------------------------------------------------------

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Main
{
    //强攻   状态
    //new 
    public partial struct Buff_118 : IBuff
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


        public float judgeRate;

        /// <summary>
        /// 特效原本scale
        /// </summary>
        public int oldScale;

        public void OnOccur(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            ref var battleConstConfig = ref inData.globalConfigData.value.Value.configTbbattle_constants.configTbbattle_constants;
            for (int i = 0; i < battleConstConfig.Length; i++)
            {
                if (battleConstConfig[i].constantName == (FixedString128Bytes)"status_beyond_atk")
                {
                    //状态-强攻-攻击加成
                    judgeRate = battleConstConfig[i].constantValue / 10000f;
                    break;
                }
            }

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
            tickTime = (int)(0.5f / inData.fdT);

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

        public void OnUpdate(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
         
        }

        public void OnTick(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
         
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
            var temp = refData.cdfeChaStats[carrier];
            if (temp.chaProperty.atk > temp.chaProperty.defaultAtk * judgeRate)
            {
                var loc = inData.cdfeLocalTransform[effectEntity];
                refData.ecb.SetComponent<LocalTransform>(inData.sortkey, effectEntity, new LocalTransform { Position = loc.Position, Scale = oldScale, Rotation = loc.Rotation });
            }
            else
            {
                var loc = inData.cdfeLocalTransform[effectEntity];
                refData.ecb.SetComponent<LocalTransform>(inData.sortkey, effectEntity, new LocalTransform { Position = loc.Position, Scale = 0, Rotation = loc.Rotation });
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