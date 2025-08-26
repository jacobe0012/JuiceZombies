//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-17 12:15:10
//---------------------------------------------------------------------


using cfg.config;
using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Main
{
    //1007    状态
    //new 
    public partial struct Buff_1007 : IBuff
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

        public void OnOccur(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            argsNew.args1.x = (int)duration;
            duration = MathHelper.MaxNum;


            //if (UnityHelper.TrySpawnSpecialEffect(ref refData.ecb, inData.linkedEntityGroup, inData.sortkey,
            //        inData.cdfeSpecialEffectData, inData.globalConfigData, id, out int specialEffectId,
            //        out int curGroup, out int curSort) && !isDisableSpecial)
            //{
            //    var ins = UnityHelper.SpawnBuffSpecialEffect(ref refData.ecb, inData.sortkey, inData.eT,
            //        inData.cdfeLocalTransform,
            //        inData.gameTimeData,
            //        inData.globalConfigData, specialEffectId, inData.prefabMapData, carrier);
            //     if (ins != Entity.Null)
            //    {
            //        refData.ecb.SetComponent(inData.sortkey, ins, new SpecialEffectData
            //        {
            //            id = specialEffectId,
            //            stateId = id,
            //            groupId = curGroup,
            //            sort = curSort
            //        });
            //    }
            //}
        }


        public void OnRemoved(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
            int atkRange = 10;

            if (!refData.cdfeChaStats.HasComponent(carrier))
            {
                return;
            }

            ref var skilleffectconfig =
                ref inData.globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;

            int effectIndex = -1;
            for (int i = 0; i < skilleffectconfig.Length; i++)
            {
                if (skilleffectconfig[i].id == skillId)
                {
                    effectIndex = i;
                    break;
                }
            }

            UnityEngine.Debug.LogError($"skillid{skillId},stateid:{id}");

            ref var skillEffect = ref skilleffectconfig[effectIndex];


            ref var elements = ref skilleffectconfig[effectIndex].elementList;

            for (int i = 0; i < inData.entities.Length; i++)
            {
                var entity = inData.entities[i];
                if (!refData.cdfeEnemyData.HasComponent(entity)) continue;
                if (entity.Index == carrier.Index) continue;
                float dis = math.distance(refData.cdfeLocalTransform[entity].Position,
                    refData.cdfeLocalTransform[carrier].Position);
                UnityEngine.Debug.Log($"dis:{dis},range:{atkRange}");
                if (dis > atkRange) continue;
                AddElement(ref refData, inData, refData.cdfeChaStats[carrier].chaResource.direction, ref elements,
                    skilleffectconfig[effectIndex].skillId, isBullet, entity);
            }
            // RemoveChangeProp(ref refData, inData);

            //UnityHelper.TrySpawnSpecialEffect(ref refData.ecb, inData.linkedEntityGroup, inData.sortkey,
            //    inData.cdfeSpecialEffectData, inData.globalConfigData, id, out int specialEffectId,
            //    out int curGroup, out int curSort, true);
        }

        public void OnUpdate(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }

        public void OnTick(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
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
            ref var skilleffectconfig =
                ref inData.globalConfigData.value.Value.configTbskill_effectNews.configTbskill_effectNews;

            int effectIndex = -1;
            for (int i = 0; i < skilleffectconfig.Length; i++)
            {
                if (skilleffectconfig[i].id == skillId)
                {
                    effectIndex = i;
                    break;
                }
            }

            UnityEngine.Debug.LogError($"skillid{skillId},stateid:{id}");

            ref var skillEffect = ref skilleffectconfig[effectIndex];

            #region SpecialEffects

            UnityHelper.SpawnSpecialEffect(ref refData.ecb, inData.sortkey, inData.eT, inData.cdfeLocalTransform,
                inData.cdfePostTransformMatrix, inData.gameTimeData,
                inData.globalConfigData,
                ref skillEffect.specialEffects,
                inData.prefabMapData, caster, false, skillEffect.skillId, default, default, inData.bfeSkill[caster], 1);

            #endregion

            duration = argsNew.args1.x;
        }

        public void OnPerUnitMove(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData)
        {
        }


        private readonly void AddElement(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData,
            float3 dir, ref BlobArray<int> elementList, int skillid, bool isBullet, Entity entity)
        {
            ref var elementsTable =
                ref inData.globalConfigData.value.Value.configTbskill_effectElements.configTbskill_effectElements;
            for (int i = 0; i < elementList.Length; i++)
            {
                UnityEngine.Debug.Log($"AddElement:{elementList[i]}");
                //Debug.LogError($"{elementList[i]}");
                //元素表！！！！
                int elementIndex = 0;
                for (int j = 0; j < elementsTable.Length; j++)
                {
                    if (elementsTable[j].id == elementList[i])
                    {
                        elementIndex = j;
                        break;
                    }
                }

                ref var element =
                    ref inData.globalConfigData.value.Value.configTbskill_effectElements.configTbskill_effectElements[
                        elementIndex];

                switch (element.elementType)
                {
                    //输出元素
                    case 1:
                        // Debug.LogError(
                        //     $"target:{target.Index},elements{element.id}:{element.outputType},{element.bonusType},{element.calcType}");
                        refData.ecb.AppendToBuffer(inData.sortkey, entity, new Buff
                        {
                            CurrentTypeId = (Buff.TypeId)element.elementType,
                            Int32_0 = element.id,
                            Single_1 = 0,
                            Int32_2 = 0,
                            Single_3 = 0,
                            Boolean_4 = false,
                            Entity_5 = caster,
                            Entity_6 = entity,
                            int2_7 = 0,
                            Int32_8 = 0,
                            Int32_9 = 0,
                            float3_10 = dir,
                            Int32_11 = element.stateType,
                            BuffArgsNew_12 = new BuffArgsNew
                            {
                                args1 = new int4(element.outputType, 0, 0, 0),
                                args2 = new int4(element.bonusType, 0, 0, 0),
                                args3 = new int4(element.calcType, element.calcTypePara[0],
                                    element.calcTypePara.Length > 1 ? element.calcTypePara[1] : 0, 0),
                                args4 = new int4(0, 0, 0, 0)
                            },
                            Int32_14 = element.power,
                            Boolean_17 = isBullet,
                            Int32_20 = skillid
                        });
                        break;
                    //更改属性
                    case 2:
                        //Debug.LogError($"222222");
                        refData.ecb.AppendToBuffer(inData.sortkey, entity, new Buff
                        {
                            CurrentTypeId = (Buff.TypeId)element
                                .elementType,
                            Int32_0 = element
                                .id,
                            Single_1 = 0,
                            Int32_2 = 0,
                            Single_3 = element.calcTypePara.Length > 0
                                ? element.calcTypePara[0] / 1000f
                                : 0,
                            Boolean_4 = element
                                .calcType == 0
                                ? true
                                : false,
                            Entity_5 = caster,
                            Entity_6 = entity,
                            int2_7 = new int2(element.changeType,
                                element.changeTypePara.Length > 0 ? element.changeTypePara[0] : 0),
                            Int32_8 = 0,
                            Int32_9 = 0,
                            float3_10 = dir,
                            Int32_11 = element
                                .stateType,
                            BuffArgsNew_12 = new BuffArgsNew
                            {
                                args1 = new int4(element
                                        .attrId,
                                    element
                                        .attrIdPara[0],
                                    0,
                                    0),
                                args2 = new int4(0,
                                    0,
                                    0,
                                    0),
                                args3 = new int4(0,
                                    0,
                                    0,
                                    0),
                                args4 = new int4(0,
                                    0,
                                    0,
                                    0)
                            },
                            Int32_14 = element.power,
                            Int32_20 = skillid
                        });
                        break;
                }
            }
        }
    }
}